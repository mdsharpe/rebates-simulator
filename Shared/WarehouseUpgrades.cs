using RebatesSimulator.Shared.Model;

namespace RebatesSimulator.Shared
{
    public static class WarehouseUpgrades
    {
        public static (int IncreaseAmount, int Cost) GetDetailsOfNextWarehouseUpgrade(Player player)
        {
            var increaseAmount = player.WarehouseCapacity switch
            {
                <= 1000 => 50,
                <= 2500 => 100,
                <= 10_000 => 500,
                _ => 1000
            };

            var cost = Convert.ToInt32(Math.Pow(player.WarehouseCapacity, 1.1));

            return (increaseAmount, cost);
        }
    }
}
