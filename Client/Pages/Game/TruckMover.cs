namespace RebatesSimulator.Client.Pages.Game
{
    public class TruckMover
    {
        /// <summary>
        /// The proportion of the scene across which each truck moves per second.
        /// I.e. 0.05 means the truck takes 20 seconds to cross the game scene.
        /// </summary>
        public const float TruckSpeed = 0.1F;

        /// <summary>
        /// Source: scenery.png counting pixels 🙃
        /// </summary>
        private static readonly double[] _playerTurnOffXPositionsAsRatios = new double[]
        {
            (double)655 / 1536,
            (double)1021 / 1536,
            (double)515 / 1536,
            (double)838 / 1536
        };

        public static MovedTruck GetTruckPosition(
            Truck truck,
            int canvasWidth,
            int warehouseVerticalOffset,
            int middleOfRoadYPosition)
        {
            var playerTurnOffPosition = Convert.ToInt32(_playerTurnOffXPositionsAsRatios[truck.PlayerId] * canvasWidth);

            var elapsedTimeMs = (DateTimeOffset.Now - truck.Birthday).TotalMilliseconds;
            var initialX = truck.SpawnLeft ? 0 : canvasWidth;

            var distanceToTurnOffPosition = Math.Abs(playerTurnOffPosition - initialX);

            var totalDisplacementPx = Convert.ToInt32(canvasWidth * TruckSpeed * (elapsedTimeMs / 1000));

            // We haven't yet reached the warehouse
            if (totalDisplacementPx < distanceToTurnOffPosition)
            {
                var x = initialX + totalDisplacementPx * (truck.SpawnLeft ? 1 : -1);
                return new MovedTruck(x, middleOfRoadYPosition);
            }

            if (totalDisplacementPx < distanceToTurnOffPosition + 2 * warehouseVerticalOffset)
            {
                // We're in the offroad
                var distanceAlongOffroad = warehouseVerticalOffset
                    - Math.Abs(totalDisplacementPx - (distanceToTurnOffPosition + warehouseVerticalOffset));

                var playerVerticalOffset = distanceAlongOffroad * (truck.PlayerId <= 1 ? -1 : 1);

                return new MovedTruck(
                    playerTurnOffPosition,
                    middleOfRoadYPosition + playerVerticalOffset,
                    AtWarehouse: true);
            }
            else
            {
                var hasDepartedScene = totalDisplacementPx > canvasWidth + 2 * warehouseVerticalOffset;

                // Left the warehouse and offroad
                var x = initialX + (totalDisplacementPx - 2 * warehouseVerticalOffset) * (truck.SpawnLeft ? 1 : -1);
                return new MovedTruck(x, middleOfRoadYPosition, HasDepartedScene: hasDepartedScene);
            }
        }
    }
}
