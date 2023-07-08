using Microsoft.AspNetCore.SignalR;
using RebatesSimulator.Server.Hubs;
using RebatesSimulator.Shared;

namespace RebatesSimulator.Server.Engines
{
    public class GameEngine : BackgroundService
    {
        private static readonly TimeSpan Interval = TimeSpan.FromMilliseconds(1000);
        private readonly ILogger<GameEngine> _logger;
        private readonly GameState _gameState;
        private readonly IHubContext<GameHub> _hubContext;

        public GameEngine(
            ILogger<GameEngine> logger,
            GameState gameState,
            IHubContext<GameHub> gameHubContext)
        {
            _logger = logger;
            _gameState = gameState;
            _hubContext = gameHubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var taskDelay = Task.Delay(Interval, cancellationToken);

                if (_gameState.Players.Any())
                {
                    var truckCapacity = GameConstants.truckCapacity;

                    var winner = DemandEngine.GetPlayerForTruckToGoTo(_gameState.Players.Values);

                    var spawnLeft = new Random().NextDouble() >= 0.5;

                    // Spawn trucks
                    if (_gameState.TotalStock > 0)
                    {
                        _gameState.Trucks.Add(new Truck
                        {
                            Capacity = truckCapacity,
                            PlayerId = winner,
                            SpawnLeft = spawnLeft,
                            Birthday = DateTimeOffset.Now
                        });
                    }
                }

                await _hubContext.Clients.All.SendAsync(
                    nameof(IGameHubClient.OnGameStateChanged),
                    _gameState);

                await taskDelay;
            }
        }
    }
}
