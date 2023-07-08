using Blazor.Extensions;
using Blazor.Extensions.Canvas.Canvas2D;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace RebatesSimulator.Client.Pages.Game
{
    public partial class Scene : ComponentBase, IDisposable
    {
        private Subject<bool> _disposed = new();
        protected BECanvasComponent CanvasComponent = new();
        private Canvas2DContext? _canvas;
        protected ElementReference PageContainer;
        private readonly Dictionary<Guid, CachedTruck> TruckTracker = new();

        [Inject]
        private GameStateWrapper? GameStateWrapper { get; set; }

        [Inject]
        private IJSRuntime JsRuntime { get; set; } = default!;

        [Inject]
        private GameSignalRClient SignalRClient { get; set; } = default!;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            _canvas = await CanvasComponent.CreateCanvas2DAsync();

            await JsRuntime.InvokeVoidAsync("fixCanvasSizes");

            GameStateWrapper!.GameState
                .TakeUntil(_disposed)
                .Where(gs => gs is not null)
                .CombineLatest(Observable.Interval(TimeSpan.FromMilliseconds(30)))
                .Subscribe(async o => await DrawTrucks(o.First!.Trucks));

            base.OnAfterRender(firstRender);
        }

        private async Task DrawTrucks(ICollection<Truck> trucks)
        {
            var canvasWidth = await JsRuntime.InvokeAsync<int>("getTrueCanvasWidth");
            var canvasHeight = await JsRuntime.InvokeAsync<int>("getTrueCanvasHeight");

            await _canvas.ClearRectAsync(0, 0, canvasWidth, canvasHeight);

            Console.WriteLine($"{trucks.Count} trucks in scene");
            foreach (var truck in trucks)
            {
                var position = TruckMover.GetTruckPosition(
                    truck,
                    canvasWidth,
                    canvasHeight / 7, // ish
                    canvasHeight / 2);

                await _canvas.SetFillStyleAsync(GetColourFromPlayerId(truck.PlayerId));
                await _canvas.FillRectAsync(position.X, position.Y, 30, 30);

                await TrackTruck(truck.TruckId, position);
            }
        }

        private static string GetColourFromPlayerId(int playerId)
            => playerId switch
            {
                0 => "red",
                1 => "yellow",
                2 => "green",
                3 => "blue",
                _ => throw new NotSupportedException(),
            };

        public void Dispose()
        {
            _disposed.OnNext(true);
        }

        private async Task TrackTruck(Guid id, MovedTruck update)
        {
            if (!TruckTracker.TryGetValue(id, out var truck))
            {
                if (update.HasDepartedScene)
                {
                    return;
                }

                truck = new CachedTruck { TruckId = id };
                TruckTracker.Add(id, truck);
            }

            if (update.ParkedAtWarehouse && !truck.ParkedAtWarehouseAnnounced)
            {
                await SignalRClient.HandleTruckArrival(id);
                truck.ParkedAtWarehouseAnnounced = true;
            }

            if (update.HasDepartedScene)
            {
                TruckTracker.Remove(id);
                await SignalRClient.DestroyTruck(id);
            }
        }
    }
}
