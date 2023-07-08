namespace RebatesSimulator.Server.Engines
{
	public class GameEngine : BackgroundService
	{
		private static readonly TimeSpan Interval = TimeSpan.FromMilliseconds(1000);
		private readonly GameState _gameState;

		public GameEngine(GameState gameState)
		{
			_gameState = gameState ?? throw new NullReferenceException(nameof(gameState));
		}

		protected override async Task ExecuteAsync(CancellationToken cancellationToken)
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				var taskDelay = Task.Delay(Interval, cancellationToken);

				var truckCapacity = 1;

				var winner = DemandEngine.GetPlayerForTruckToGoTo(_gameState.Players.Values);

				// Spawn trucks
				if (_gameState.TotalStock > 0)
				{
                    _gameState.Trucks.Add(new Truck(truckCapacity, winner));
                }

				await taskDelay;
			}
		}
	}
}