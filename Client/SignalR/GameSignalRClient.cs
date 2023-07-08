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

        public async Task<bool> JoinGame(string name)
        {
            return await HubConnection.InvokeAsync<bool>(nameof(JoinGame), name);
        }

        public async Task AddProduct()
        {
            await HubConnection.InvokeAsync(nameof(AddProduct));
        }

        public Task SetRebate(int newRebate)
        {
            throw new NotImplementedException();
        }

        public Task DestroyTruck(Guid truckId)
        {
            throw new NotImplementedException();
        }

        public Task ExchangeWithTruck(Guid truckId)
        {
            throw new NotImplementedException();
        }

        public IDisposable OnGameStateChanged(Action<GameState> action)
            => HubConnection.On(nameof(OnGameStateChanged), action);
    }
}
