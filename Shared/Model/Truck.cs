namespace RebatesSimulator.Shared.Model
{
    public class Truck
    {
        public Truck(int capacity, int playerId)
        {
            this.Capacity = capacity;
            this.PlayerId = playerId;
            
        }
        public int Capacity { get ; set; }

        public int PlayerId { get; set; }
    }
}
