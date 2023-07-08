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
            if (player.Stock < 50 && player.Balance >= 100)
            {
                player.Stock++;
                player.Balance -= 100;

                if (player.Balance == 0)
                {
                    // eliminate the player
                    _gameState.RemovePlayer(Context.ConnectionId);
                }
            }

            return Task.CompletedTask;
        }
    }
}
