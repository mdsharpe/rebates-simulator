namespace RebatesSimulator.Shared.Model
{
    public class GameState
    {
        public const int MaxPlayers = 4;

        public Dictionary<string, Player> Players { get; set; } = new();

        public bool TryAddPlayer(string connectionId, string name)
        {
            lock (Players)
            {
                if (Players.Count >= MaxPlayers)
                {
                    return false;
                }

                var id = Enumerable.Range(0, 4)
                    .First(o => !Players.Values.Any(p => p.Id == o));

                Players.Add(
                    connectionId,
                    new Player
                    {
                        Id = id,
                        ConnectionId = connectionId,
                        Name = name,
                        Balance = 100000,
                        Rebate = 10
                    });
            }

            return true;
        }

        public bool RemovePlayer(string connectionId)
        {
            lock (Players)
            {
                return Players.Remove(connectionId);
            }
        }

        public ICollection<Truck> Trucks { get; set; } = new List<Truck>();

        public int TotalStock => Players.Values.Sum(o => o.Stock);
    }
}
