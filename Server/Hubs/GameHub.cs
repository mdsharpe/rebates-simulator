using Microsoft.AspNetCore.SignalR;
using RebatesSimulator.Server.Engines;
using RebatesSimulator.Shared;

namespace RebatesSimulator.Server.Hubs
{
    public class GameHub : Hub<IGameHubClient>
    {
        private readonly ILogger<GameHub> _logger;
        private readonly GameState _gameState;
        private readonly GameBusinessLogic _businessLogic;

        public GameHub(
            ILogger<GameHub> logger,
            GameState gameState,
            GameBusinessLogic businessLogic)
        {
            _logger = logger;
            _gameState = gameState;
            _businessLogic = businessLogic;
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            _gameState.RemovePlayer(Context.ConnectionId);

            _logger.LogInformation(
                "Player with connection ID '{connectionId}' left.",
                Context.ConnectionId);

            return base.OnDisconnectedAsync(exception);
        }

        public Task<int?> JoinGame(string name)
        {
            var added = _gameState.TryAddPlayer(Context.ConnectionId, name, out var player);

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

            return Task.FromResult(player?.Id);
        }

        public async Task ManufactureProduct(int n)
        {
            if (!_gameState.Players.TryGetValue(Context.ConnectionId, out var player))
            {
                return;
            }

            await _businessLogic.ManufactureProducts(player, n);
        }

        public async Task HandleTruckArrival(Guid truckId)
        {
            if (!_gameState.Players.TryGetValue(Context.ConnectionId, out var player))
            {
                return;
            }

            var truck = _gameState.Trucks.Single(t => t.TruckId == truckId);

            if (player.Id != truck.PlayerId)
            {
                throw new InvalidOperationException();
            }

            await _businessLogic.HandleTruckArrival(player);
        }

        public async Task SetRebateRate(decimal rebateRate)
        {
            if (!_gameState.Players.TryGetValue(Context.ConnectionId, out var player))
            {
                return;
            }

            player.RebateRate = rebateRate;
        }

        public async Task DestroyTruck(Guid truckId)
        {
            var truck = _gameState.Trucks.Single(t => t.TruckId == truckId);

            _gameState.Trucks.Remove(truck);
        }

        public async Task UpgradeWarehouse()
        {
            var player = _gameState.Players[Context.ConnectionId];

            await _businessLogic.HandleWarehouseUpgrade(player);
        }
    }
}
