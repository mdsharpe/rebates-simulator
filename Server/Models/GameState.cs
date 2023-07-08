namespace RebatesSimulator.Server.Models
{
    public class GameState
    {

        public GameState(ICollection<Player> players)
        {
            Players = players;
        }

        public ICollection<Player> Players { get; set; }
    }
}
