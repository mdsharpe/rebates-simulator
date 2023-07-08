using Microsoft.AspNetCore.SignalR;
using RebatesSimulator.Shared;

namespace RebatesSimulator.Server.Hubs
{
    public class GameHub : Hub<IGameHubClient>
    {
        private readonly ILogger<GameHub> _logger;
        private readonly GameState _gameState;

        public GameHub(
            ILogger<GameHub> logger,
            GameState gameState)
        {
            _logger = logger;
            _gameState = gameState;
        }

        public Task<bool> JoinGame(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                name = name.Trim();
            }
            else
            {
                name = Context.ConnectionId;
            }

            var added = _gameState.TryAddPlayer(Context.ConnectionId, name);

            if (added)
            {
                _logger.LogInformation(
                    "Player '{name}' with connection ID '{connectionId}' joined.",
                    name,
                    Context.ConnectionId);
            }
            else
            {
                _logger.LogWarning(
                    "Player '{name}' with connection ID '{connectionId}' failed to join.",
                    name,
                    Context.ConnectionId);
            }

            return Task.FromResult(added);
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            _gameState.RemovePlayer(Context.ConnectionId);

            _logger.LogInformation(
                "Player with connection ID '{connectionId}' left.",
                Context.ConnectionId);

            return base.OnDisconnectedAsync(exception);
        }

        public Task AddProduct()
        {
            // get the player
            var player = _gameState.Players[Context.ConnectionId];

            // check their stock and balance
            if (player.Stock < GameConstants.warehouseCapacity && player.Balance >= GameConstants.productManufactureCost)
            {
                player.Stock++;
                player.Balance -= GameConstants.productManufactureCost;

                if (player.Balance == 0)
                {
                    // eliminate the player
                    _gameState.RemovePlayer(Context.ConnectionId);
                }
            }

            return Task.CompletedTask;
        }

        public Task SetRebate(int newRebate)
        {
            var player = _gameState.Players[Context.ConnectionId];

            player.Rebate = newRebate;

            return Task.CompletedTask;
        }

        public Task ExchangeWithTruck(Guid truckId)
        {
            var playerIdFromTruck = _gameState.Trucks
                .Where(t => t.TruckId == truckId)
                .Select(t => t.PlayerId)
                .FirstOrDefault();

            var player = _gameState.Players
                .Where(p => p.Value.Id == playerIdFromTruck)
                .Select(p => p.Value)
                .First();

            // ToDo: reimplement when we decide to set fluctuating truck capacities
            // var truckCapacity = _gameState.Trucks
            //    .Where(t => t.TruckId == truckId)
            //    .Select(t => t.Capacity)
            //    .FirstOrDefault();

            var truckCapacity = GameConstants.truckCapacity;

            if (player.Stock > truckCapacity)
            {
                player.Stock -= truckCapacity;

                player.Balance += GameConstants.sellPrice * (player.Rebate) / 100;

            } 
            else
            {
                _logger.LogWarning("You have been fined");
                player.Balance -= GameConstants.fineAmount;
            }

            return Task.CompletedTask;
        }
    }
}
