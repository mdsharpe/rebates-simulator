using Microsoft.AspNetCore.SignalR;
using RebatesSimulator.Server.Hubs;
using RebatesSimulator.Shared;

namespace RebatesSimulator.Server.Engines
{
    public class GameBusinessLogic
    {
        private readonly Random _rng = new();
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

        public async Task Trucks()
        {
            var shouldSpawnTruck = _gameState.Players.Values.Sum(o => o.Stock) > 0
                && _rng.Next(1, 4) == 1;

            if (shouldSpawnTruck)
            {
                var truckCapacity = GameConstants.TruckCapacity;

                var winner = GetPlayerForTruckToGoTo(_gameState.Players.Values);

                var spawnLeft = _rng.NextDouble() >= 0.5;

                // Spawn trucks
                _gameState.Trucks.Add(new Truck
                {
                    Capacity = truckCapacity,
                    PlayerId = winner,
                    SpawnLeft = spawnLeft,
                    Birthday = DateTimeOffset.Now,
                    TruckId = Guid.NewGuid()
                });
            }
        }

        public async Task Rent()
        {
            if (DateTimeOffset.Now.Subtract(_gameState.LastChargedRent) < TimeSpan.FromSeconds(15))
            {
                return;
            }

            _gameState.LastChargedRent = DateTimeOffset.Now;

            foreach (var player in _gameState.Players.Values)
            {
                await HandleBalanceChanged(player, -GameConstants.Rent, "Rent charged");
            }
        }

        public async Task ManufactureProducts(Player player, int n)
        {
            if (player.Stock + n > player.WarehouseCapacity || player.Balance < GameConstants.ProductManufactureCost * n)
            {
                return;
            }

            player.Stock += n;
            await HandleBalanceChanged(player, GameConstants.ProductManufactureCost * -n, "Product manufactured");
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

        private int GetPlayerForTruckToGoTo(ICollection<Player> players)
        {
            var playerDemands = players
                .Select(player =>
                {
                    var advertisingBonus = _rng.NextDouble() + 0.5;
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
    }
}
