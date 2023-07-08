namespace RebatesSimulator.Server.Models
{
    public class Player
    {

        public Player()
        {
            this.Balance = 100000;
            this.Stock = 0;
            this.Rebate = 10;
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public int Stock { get; set; }

        public int Balance { get; set; }

        public int Rebate { get; set; }
    }
}
