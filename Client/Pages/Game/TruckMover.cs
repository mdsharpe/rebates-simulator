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
            var playerTurnOffPosition = playerTurnOffXPositions[truck.PlayerId];
            var distanceToTurnOffPosition = Math.Abs(playerTurnOffPosition - initialX);

            var totalDisplacementPx = Convert.ToInt32(canvasWidth * TruckSpeed * (elapsedTimeMs / 1000));

            // We haven't yet reached the warehouse
            if (totalDisplacementPx < distanceToTurnOffPosition)
            {
                var x = initialX + totalDisplacementPx * (truck.SpawnLeft ? 1 : -1);
                return (x, middleOfRoadYPosition);
            }

            if (totalDisplacementPx > distanceToTurnOffPosition + 2 * warehouseVerticalOffset)
            {
                var x = initialX + (totalDisplacementPx - 2 * warehouseVerticalOffset) * (truck.SpawnLeft ? 1 : -1);
                return (x, middleOfRoadYPosition);
            }
            else
            {
                // We're in the offroad
                return (playerTurnOffPosition, middleOfRoadYPosition + playerVerticalOffset);
            }
        }
    }
}
