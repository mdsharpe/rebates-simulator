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
        private readonly GameBusinessLogic _businessLogic;

        public GameEngine(
            ILogger<GameEngine> logger,
            GameState gameState,
            IHubContext<GameHub> gameHubContext,
            GameBusinessLogic businessLogic)
        {
            _logger = logger;
            _gameState = gameState;
            _hubContext = gameHubContext;
            _businessLogic = businessLogic;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var taskDelay = Task.Delay(Interval, cancellationToken);

                await _businessLogic.Trucks();
                await _businessLogic.Rent();
               
                await _hubContext.Clients.All.SendAsync(
                    nameof(IGameHubClient.OnGameStateChanged),
                    _gameState);

                await taskDelay;
            }
        }
    }
}
