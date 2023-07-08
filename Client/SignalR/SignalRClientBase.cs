using Microsoft.AspNetCore.Components;

namespace RebatesSimulator.Client.SignalR
{
    // https://blog.hagoodit.com/2021/08/28/strongly-typed-signal-r-client-and-server/
    public abstract class SignalRClientBase
        : ISignalRClient, IAsyncDisposable
    {
        protected bool Started { get; private set; }

        protected SignalRClientBase(NavigationManager navigationManager, string hubPath)
        {
            HubConnection = new HubConnectionBuilder()
                .WithUrl(navigationManager.ToAbsoluteUri(hubPath))
                .WithAutomaticReconnect()
                .Build();

            HubConnection.Closed += async (arg) =>
            {
                if (Closed != null)
                {
                    await Closed.Invoke(arg);
                }
            };
        }

        public bool IsConnected =>
            HubConnection.State == HubConnectionState.Connected;

        public event Func<Exception?, Task>? Closed;

        protected HubConnection HubConnection { get; private set; }

        public async ValueTask DisposeAsync()
        {
            if (HubConnection != null)
            {
                await HubConnection.DisposeAsync();
            }
        }

        public async Task Start()
        {
            if (!Started)
            {
                await HubConnection.StartAsync();
                Started = true;
            }
        }
    }
}
