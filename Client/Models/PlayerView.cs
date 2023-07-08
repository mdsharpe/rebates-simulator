namespace RebatesSimulator.Client.Models
{
    public class PlayerView
    {
        public required int Id { get; init; }
        public required string Name { get; init; }
        public required decimal Balance { get; init; }
        public required int Stock { get; init; }
        public required bool IsCurrentPlayer { get; init; }
        public required int WarehouseCapacity { get; init; }
    }
}
