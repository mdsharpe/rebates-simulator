namespace RebatesSimulator.Server.Models
{
    public class GameState
    {
        public const int MaxPlayers = 4;

        public Dictionary<string, Player> Players { get; } = new();

        public bool TryAddPlayer(string connectionId, string name)
        {
            if (Players.Count >= MaxPlayers)
            {
                return false;
            }

            Players.Add(
                connectionId,
                new Player
                {
                    ConnectionId = connectionId,
                    Name = name
                });

            return true;
        }

        public bool RemovePlayer(string connectionId)
        {
            return Players.Remove(connectionId);
        }

        public ICollection<Truck> Trucks { get; } = new List<Truck>();

        public int TotalStock => Players.Values.Sum(o => o.Stock);
    }
}
