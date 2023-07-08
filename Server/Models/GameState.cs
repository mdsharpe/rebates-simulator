namespace RebatesSimulator.Server.Models
{
    public class GameState
    {
        public GameState()
        {
        }

        public ICollection<Player> Players { get; } = new List<Player>();

		public ICollection<Truck> Trucks { get; } = new List<Truck>();

        public int TotalStock => Players.Sum(o => o.Stock);
    }
}
