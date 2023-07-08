using RebatesSimulator.Shared.Model;

namespace RebatesSimulator.Shared
{
    public interface IGameHubClient
    {
        // Server-to-client methods
        IDisposable OnGameStateChanged(Action<GameState> action);

        // Client-to-server methods
        Task<bool> JoinGame(string name);
    }
}
