namespace RebatesSimulator.Shared.Model
{
    public class Truck
    {
        public required int Capacity { get ; init; }

        public required int PlayerId { get; init; }

        public required bool SpawnLeft { get; init; }

        public required DateTimeOffset Birthday { get; init; }
    }
}
