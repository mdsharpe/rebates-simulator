using Microsoft.AspNetCore.SignalR;

namespace RebatesSimulator.Server.Hubs
{
    public class GameHub : Hub<IGameHubClient>
    {
    }
}
