namespace RebatesSimulator.Shared
{
    public interface IGameHubClient
    {
        Task<bool> JoinGame();
    }
}
