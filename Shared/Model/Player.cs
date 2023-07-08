namespace RebatesSimulator.Shared.Model
{
    public class Player
    {
        public required string ConnectionId { get; init; }

        public required int Id { get; init; }

        public required string Name { get; init; }

        public int Stock { get; set; } = 49;

        public int Balance { get; set; }

        public int Rebate { get; set; }
    }
}
