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

            GameState
                .Where(o => o is not null)
                .Subscribe(o =>
                {
                    Console.WriteLine(
                        "New game state received. Players: "
                        + JsonSerializer.Serialize(o?.Players));
                });

            PlayerId
                .Subscribe(o =>
                {
                    Console.WriteLine("Player ID: " + o);
                });

            CurrentPlayer = Observable.CombineLatest(
                GameState,
                PlayerId,
                (gameState, playerId) => gameState?.Players.Values.FirstOrDefault(p => p.Id == playerId));
        }

        public readonly BehaviorSubject<int?> PlayerId = new(null);
        public readonly BehaviorSubject<GameState?> GameState = new(null);

        public readonly IObservable<Player?> CurrentPlayer;

        public void Clear()
        {
            _gameStateChanged?.Dispose();
            PlayerId.OnNext(null);
            GameState.OnNext(null);
        }

        private async Task SignalRClient_Opened()
        {
            _gameStateChanged?.Dispose();
            _gameStateChanged = _signalRClient.OnGameStateChanged(GameState.OnNext);
        }

        private async Task SignalRClient_Closed(Exception? arg)
        {
            _gameStateChanged?.Dispose();
            GameState.OnNext(null);
        }
    }
}
