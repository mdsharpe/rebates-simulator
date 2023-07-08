namespace RebatesSimulator.Shared.Model
{
    public class Player
    {
        public required string ConnectionId { get; init; }

        public required int Id { get; init; }

        public required string Name { get; init; }

        public int Stock { get; set; }

        public int WarehouseCapacity { get; set; } = 50;

        public decimal Balance { get; set; }

        public decimal RebateRate { get; set; }
    }
}
