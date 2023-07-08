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
        private GameStateWrapper GameState { get; set; } = default!;

        [Inject]
        private IJSRuntime JsRuntime { get; set; } = default!;

        [Inject]
        private GameSignalRClient SignalRClient { get; set; } = default!;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            _canvas = await CanvasComponent.CreateCanvas2DAsync();

            await JsRuntime.InvokeVoidAsync("fixCanvasSizes");

            GameState!.GameState
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

                await TrackTruck(truck.TruckId, (truck, position));
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

        private async Task TrackTruck(Guid id, (Truck Truck, MovedTruck Update) truck)
        {
            if (!TruckTracker.TryGetValue(id, out var cachedTruck))
            {
                if (truck.Update.HasDepartedScene)
                {
                    return;
                }

                cachedTruck = new CachedTruck { TruckId = id };
                TruckTracker.Add(id, cachedTruck);
            }

            if (truck.Update.ParkedAtWarehouse && !cachedTruck.ParkedAtWarehouseAnnounced)
            {
                if (truck.Truck.PlayerId == GameState.PlayerId.Value)
                {
                    await SignalRClient.HandleTruckArrival(id);
                }

                cachedTruck.ParkedAtWarehouseAnnounced = true;
            }

            if (truck.Update.HasDepartedScene)
            {
                TruckTracker.Remove(id);

                if (truck.Truck.PlayerId == GameState.PlayerId.Value)
                {
                    await SignalRClient.DestroyTruck(id);
                }
            }
        }
    }
}
