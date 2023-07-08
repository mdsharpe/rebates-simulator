namespace RebatesSimulator.Client.Models
{
    public record MovedTruck(
        int X,
        int Y,
        bool AtWarehouse = false,
        bool HasDepartedScene = false);
}
