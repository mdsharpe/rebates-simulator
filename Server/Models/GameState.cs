namespace RebatesSimulator.Server.Models
{
    public class GameState
    {
        public GameState()
        {
        }

        public ICollection<Player> Players { get; } = new List<Player>();
    }
}
