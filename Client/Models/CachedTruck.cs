namespace RebatesSimulator.Client.Models
{
    public class CachedTruck
    {
        public required Guid TruckId { get; init; }
        public bool ArrivalAtWarehouseAnnounced { get; set; }
    }
}
