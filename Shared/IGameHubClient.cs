using RebatesSimulator.Shared.Model;

namespace RebatesSimulator.Shared
{
    public interface IGameHubClient
    {
        // Client-to-server methods
        Task<int?> JoinGame(string name);
        Task AddProduct();
        Task SetRebate(int newRebate);
        Task ExchangeWithTruck(Guid truckId);
        Task DestroyTruck(Guid truckId);

        // Server-to-client methods
        IDisposable OnGameStateChanged(Action<GameState> action);
    }
}
