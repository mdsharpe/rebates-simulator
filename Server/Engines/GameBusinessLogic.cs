using Microsoft.AspNetCore.SignalR;
using RebatesSimulator.Server.Hubs;
using RebatesSimulator.Shared;

namespace RebatesSimulator.Server.Engines
{
    public class GameBusinessLogic
    {
        private readonly ILogger<GameBusinessLogic> _logger;
        private readonly GameState _gameState;
        private readonly IHubContext<GameHub> _hubContext;

        public GameBusinessLogic(
            ILogger<GameBusinessLogic> logger,
            GameState gameState,
            IHubContext<GameHub> gameHubContext)
        {
            _logger = logger;
            _gameState = gameState;
            _hubContext = gameHubContext;
        }

        public int GetPlayerForTruckToGoTo(ICollection<Player> players)
        {
            var playerDemands = players
                .Select(player =>
                {
                    var randomness = new Random();
                    var advertisingBonus = randomness.NextDouble() + 0.5;
                    var demandLevel = Convert.ToInt32(player.RebateRate * 100) * player.Stock * advertisingBonus;
                    return new
                    {
                        player,
                        demandLevel
                    };
                })
                .ToArray();

            var demandSum = playerDemands.Select(o => o.demandLevel).DefaultIfEmpty(1).Sum();

            var truckProbabilities = playerDemands
                .Select(player =>
                {
                    return new
                    {
                        player,
                        demandLevel = player.demandLevel / demandSum
                    };
                })
                .ToArray();

            var winningPlayer = truckProbabilities.OrderByDescending(p => p.demandLevel).First();
            return winningPlayer.player.player.Id;
        }

        public async Task ManufactureProduct(Player player)
        {
            if (player.Stock >= player.WarehouseCapacity || player.Balance < GameConstants.ProductManufactureCost)
            {
                return;
            }

            player.Stock++;
            await HandleBalanceChanged(player, GameConstants.ProductManufactureCost * -1, "Product manufactured");
        }

        public async Task HandleTruckArrival(Player player)
        {
            if (player.Stock >= GameConstants.TruckCapacity)
            {
                player.Stock -= GameConstants.TruckCapacity;

                var earnings = GameConstants.SellPrice * GameConstants.TruckCapacity * (1M - player.RebateRate);
                await HandleBalanceChanged(player, earnings, "Net earnings");
            }
            else
            {
                await HandleBalanceChanged(player, GameConstants.FineAmount * -1, "Fined; insufficient stock");
            }
        }

        public async Task HandleBalanceChanged(Player player, decimal amount, string reason)
        {
            player.Balance += amount;

            if (amount < 0 && player.Balance <= 0)
            {
                _gameState.RemovePlayer(player.ConnectionId);
            }

            await _hubContext.Clients.All.SendAsync(
                nameof(IGameHubClient.OnBalanceChanged),
                new BalanceChange
                {
                    PlayerId = player.Id,
                    Amount = amount,
                    Reason = reason
                });
        }

        public async Task HandleWarehouseUpgrade(Player player)
        {
            var (increaseAmount, cost) = WarehouseUpgrades.GetDetailsOfNextWarehouseUpgrade(player);

            player.WarehouseCapacity += increaseAmount;
            await HandleBalanceChanged(player, -cost, "Warehouse upgraded");
        }
    }
}
