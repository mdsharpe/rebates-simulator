using RebatesSimulator.Shared.Model;

namespace RebatesSimulator.Shared
{
    public interface IGameHubClient
    {
        // Client-to-server methods
        Task<int?> JoinGame(string name);
        Task ManufactureProduct();
        Task SetRebateRate(decimal rebateRate);
        Task HandleTruckArrival(Guid truckId);
        Task DestroyTruck(Guid truckId);

        // Server-to-client methods
        IDisposable OnGameStateChanged(Action<GameState> action);
        IDisposable OnBalanceChanged(Action<BalanceChange> action);
    }
}
