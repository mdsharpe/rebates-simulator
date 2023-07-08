namespace RebatesSimulator.Shared.Model
{
    public class GameState
    {
        public const int MaxPlayers = 4;

        public Dictionary<string, Player> Players { get; set; } = new();

        public ICollection<Truck> Trucks { get; set; } = new List<Truck>();

        public DateTimeOffset LastChargedRent { get; set; } = DateTimeOffset.Now;

        public bool TryAddPlayer(string connectionId, string name, out Player? player)
        {
            lock (Players)
            {
                if (Players.Count >= MaxPlayers)
                {
                    player = null;
                    return false;
                }

                var id = Enumerable.Range(0, 4)
                    .First(o => !Players.Values.Any(p => p.Id == o));

                if (!string.IsNullOrWhiteSpace(name))
                {
                    name = name.Trim();
                }
                else
                {
                    name = $"{id + 1}UP";
                }

                player = new Player
                {
                    Id = id,
                    ConnectionId = connectionId,
                    Name = name,
                    Balance = GameConstants.StartingBalance,
                    RebateRate = 0.1M
                };

                Players.Add(connectionId, player);
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
    }
}
