namespace RebatesSimulator.Shared.Model
{
    public class BalanceChange
    {
        public required int PlayerId { get; init; }
        public required decimal Amount { get; init; }
        public required string Reason { get; init; }
    }
}
