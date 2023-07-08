using Microsoft.AspNetCore.SignalR;
using RebatesSimulator.Server.Hubs;

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

        public async Task ManufactureProduct(Player player)
        {
            if (player.Stock >= GameConstants.WarehouseCapacity || player.Balance < GameConstants.ProductManufactureCost)
            {
                return;
            }

            player.Stock++;
            await HandleBalanceChanged(player, GameConstants.ProductManufactureCost * -1);
        }

        public async Task HandleTruckArrival(Player player)
        {
            // ToDo: reimplement when we decide to set fluctuating truck capacities
            // var truckCapacity = _gameState.Trucks
            //    .Where(t => t.TruckId == truckId)
            //    .Select(t => t.Capacity)
            //    .FirstOrDefault();

            var truckCapacity = GameConstants.TruckCapacity;

            if (player.Stock > truckCapacity)
            {
                player.Stock -= truckCapacity;

                player.Balance += GameConstants.SellPrice * (1 - (player.Rebate) / 100);
            }
            else
            {
                player.Balance -= GameConstants.FineAmount;
            }
        }

        private async Task HandleBalanceChanged(Player player, decimal amount)
        {
            player.Balance += amount;

            if (player.Balance <= 0)
            {
                _gameState.RemovePlayer(player.ConnectionId);
            }

            ////await _hubContext.Clients.All.SendAsync(
            ////    nameof(IGameHubClient.OnGameStateChanged),
            ////                    _gameState);
        }
    }
}
