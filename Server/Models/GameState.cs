namespace RebatesSimulator.Server.Models
{
    public class GameState
    {
        public const int MaxPlayers = 4;

        public GameState()
        {
        }

        public ICollection<Player> Players { get; } = new List<Player>();

        public bool TryAddPlayer(string connectionId, string name)
        {
            if (Players.Count >= MaxPlayers)
            {
                return false;
            }

            Players.Add(new Player
            {
                ConnectionId = connectionId,
                Name = name
            });

            return true;
        }
    }
}
