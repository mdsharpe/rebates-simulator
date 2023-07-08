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

        public async Task<int?> JoinGame(string name)
        {
            return await HubConnection.InvokeAsync<int?>(nameof(JoinGame), name);
        }

        public async Task AddProduct()
        {
            await HubConnection.InvokeAsync(nameof(AddProduct));
        }

        public async Task SetRebate(int newRebate)
        {
            await HubConnection.InvokeAsync(nameof(SetRebate), newRebate);
        }

        public async Task DestroyTruck(Guid truckId)
        {
            await HubConnection.InvokeAsync(nameof(DestroyTruck), truckId);
        }

        public async Task ExchangeWithTruck(Guid truckId)
        {
            await HubConnection.InvokeAsync(nameof(ExchangeWithTruck), truckId);
        }

        public IDisposable OnGameStateChanged(Action<GameState> action)
            => HubConnection.On(nameof(OnGameStateChanged), action);
    }
}
