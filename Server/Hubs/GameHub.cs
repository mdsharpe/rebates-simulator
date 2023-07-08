﻿using Microsoft.AspNetCore.SignalR;
using RebatesSimulator.Server.Models;
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
                    "Player '{name}' joined with connection ID '{connectionId}'.",
                    name,
                    Context.ConnectionId);
            }
            else
            {
                _logger.LogWarning(
                    "Player '{name}' failed to join with connection ID '{connectionId}'.",
                    name,
                    Context.ConnectionId);
            }

            return Task.FromResult(added);
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            _gameState.RemovePlayer(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
