using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

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

        public async Task ManufactureProduct()
        {
            await HubConnection.InvokeAsync(nameof(ManufactureProduct));
        }

        public async Task SetRebateRate(decimal rebateRate)
        {
            await HubConnection.InvokeAsync(nameof(SetRebateRate), rebateRate);
        }

        public async Task DestroyTruck(Guid truckId)
        {
            await HubConnection.InvokeAsync(nameof(DestroyTruck), truckId);
        }

        public async Task HandleTruckArrival(Guid truckId)
        {
            await HubConnection.InvokeAsync(nameof(HandleTruckArrival), truckId);
        }

        public async Task UpgradeWarehouse()
        {
            await HubConnection.InvokeAsync(nameof(UpgradeWarehouse));
        }

        public IDisposable OnGameStateChanged(Action<GameState> action)
            => HubConnection.On(nameof(OnGameStateChanged), action);

        public IDisposable OnBalanceChanged(Action<BalanceChange> action)
            => HubConnection.On(nameof(OnBalanceChanged), action);
    }
}
