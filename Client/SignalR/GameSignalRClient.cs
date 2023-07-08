using Microsoft.AspNetCore.Components;

namespace RebatesSimulator.Client.SignalR
{
    public class GameSignalRClient
        : SignalRClientBase, IGameSignalRClient
    {
        public GameSignalRClient(NavigationManager navigationManager)
            : base(navigationManager, "/gamehub")
        {
        }

        ////public async Task CountChanged(int newValue)
        ////{
        ////    await HubConnection.SendAsync(nameof(CountChanged), newValue);
        ////}

        public async Task<bool> JoinGame(string name)
        {
            return await HubConnection.InvokeAsync<bool>(nameof(JoinGame), name);
        }

        public IDisposable OnGameStateChanged(Action<GameState> action)
            => HubConnection.On(nameof(OnGameStateChanged), action);
    }
}
