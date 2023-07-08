namespace RebatesSimulator.Shared.Model
{
    public class Truck
    {
        public Truck(int capacity, int playerId, bool spawnLeft)
        {
            this.Capacity = capacity;
            this.PlayerId = playerId;
            this.SpawnLeft = spawnLeft;
            
        }
        public int Capacity { get ; set; }

        public int PlayerId { get; set; }

        public bool SpawnLeft { get; set; }
    }
}
