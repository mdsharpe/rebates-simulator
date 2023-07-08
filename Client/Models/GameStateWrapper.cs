using System.Reactive.Subjects;
using System.Text.Json;

namespace RebatesSimulator.Client.Models
{
    public class GameStateWrapper
    {
        private readonly GameSignalRClient _signalRClient;
        private IDisposable? _gameStateChanged;

        public GameStateWrapper(GameSignalRClient signalRClient)
        {
            _signalRClient = signalRClient;

            _signalRClient.Opened += SignalRClient_Opened;
            _signalRClient.Closed += SignalRClient_Closed;

            GameState.Subscribe(o =>
            {
                Console.WriteLine(
                    "New game state received: "
                    + JsonSerializer.Serialize(o));
            });
        }

        public readonly BehaviorSubject<GameState?> GameState = new(null);

        private async Task SignalRClient_Opened()
        {
            if (_gameStateChanged is not null)
            {
                _gameStateChanged.Dispose();
            }

            _gameStateChanged = _signalRClient.OnGameStateChanged(GameState.OnNext);
        }

        private async Task SignalRClient_Closed(Exception? arg)
        {
            _gameStateChanged?.Dispose();
        }
    }
}
