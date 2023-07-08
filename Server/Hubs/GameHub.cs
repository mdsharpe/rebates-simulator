using Microsoft.AspNetCore.SignalR;
using RebatesSimulator.Server.Models;
using RebatesSimulator.Shared;

namespace RebatesSimulator.Server.Hubs
{
    public class GameHub : Hub<IGameHubClient>
    {
        private readonly GameState _gameState;

        public GameHub(GameState gameState)
        {
            _gameState = gameState;
        }

        public async Task<bool> JoinGame()
        {
            return _gameState.TryAddPlayer(Context.ConnectionId);
        }
    }
}
