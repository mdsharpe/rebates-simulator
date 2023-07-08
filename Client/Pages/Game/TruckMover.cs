namespace RebatesSimulator.Client.Pages.Game
{
    public class TruckMover
    {
        /// <summary>
        /// The proportion of the scene across which each truck moves per second.
        /// I.e. 0.05 means the truck takes 20 seconds to cross the game scene.
        /// </summary>
        public const float TruckSpeed = 0.05F;

        public static (int X, int Y) GetTruckPosition(
            Truck truck,
            int canvasWidth,
            int canvasHeight,
            int warehouseVerticalOffset,
            int middleOfRoadYPosition,
            Dictionary<int, int> playerTurnOffXPositions)
        {
            var elapsedTimeMs = (DateTimeOffset.Now - truck.Birthday).TotalMilliseconds;
            var initialX = truck.SpawnLeft ? 0 : canvasWidth;

            var playerVerticalOffset = warehouseVerticalOffset * (truck.PlayerId <= 1 ? -1 : 1);

            // WIP WIP WIP just teleport for now after 1 second
            if (elapsedTimeMs > 1000)
            {
                Console.WriteLine("Truck arrived");
                return (playerTurnOffXPositions[truck.PlayerId], middleOfRoadYPosition + playerVerticalOffset);
            }
            else
            {
                Console.WriteLine("Truck at start position");
                return (initialX, middleOfRoadYPosition);
            }
        }
    }
}
