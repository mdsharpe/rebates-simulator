using Microsoft.AspNetCore.Components;

namespace RebatesSimulator.Client.SignalR
{
    public class GameSignalRClient
        : SignalRClientBase, IGameSignalRClient
    {
        public GameSignalRClient(NavigationManager navigationManager)
            : base(navigationManager, "/GameHub")
        {
        }

        public async Task CountChanged(int newValue)
        {
            await HubConnection.SendAsync(nameof(CountChanged), newValue);
        }

        public void OnCountChanged(Action<int> action)
        {
            // Don't attach the handler once the connection is started to prevent duplicate handling
            // This could also be done per page instead
            if (!Started)
            {
                HubConnection.On(nameof(CountChanged), action);
            }
        }
    }
}
