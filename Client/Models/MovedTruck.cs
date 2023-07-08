namespace RebatesSimulator.Client.Models
{
    public record MovedTruck(
        int X,
        int Y,
        bool ParkedAtWarehouse = false,
        bool HasDepartedScene = false);
}
