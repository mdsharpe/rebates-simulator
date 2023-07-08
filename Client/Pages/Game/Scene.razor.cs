using Blazor.Extensions;
using Blazor.Extensions.Canvas.Canvas2D;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Collections.Concurrent;

namespace RebatesSimulator.Client.Pages.Game
{
    public partial class Scene : ComponentBase, IDisposable
    {
        private readonly Subject<bool> _disposed = new();
        protected BECanvasComponent CanvasComponent = new();
        private Canvas2DContext? _canvas;
        protected ElementReference PageContainer;
        private readonly ConcurrentDictionary<Guid, CachedTruck> TruckTracker = new();
        private readonly BehaviorSubject<bool> _drawing = new(false);

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

            Observable.Interval(TimeSpan.FromMilliseconds(33))
                .WithLatestFrom(_drawing, (_, drawing) => drawing)
                .Where(drawing => !drawing)
                .WithLatestFrom(GameState.GameState, (_, gs) => gs)
                .TakeUntil(_disposed)
                .Subscribe(async gs =>
                {
                    _drawing.OnNext(true);
                    await DrawTrucks(gs?.Trucks ?? Enumerable.Empty<Truck>());
                    _drawing.OnNext(false);
                });

            base.OnAfterRender(firstRender);
        }

        private async Task DrawTrucks(IEnumerable<Truck> trucks)
        {
            var canvasWidth = await JsRuntime.InvokeAsync<int>("getTrueCanvasWidth");
            var canvasHeight = await JsRuntime.InvokeAsync<int>("getTrueCanvasHeight");

            await _canvas.ClearRectAsync(0, 0, canvasWidth, canvasHeight);

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
            _canvas.Dispose();
            _disposed.OnNext(true);
        }

        private async Task TrackTruck(Guid id, (Truck Truck, MovedTruck Update) truck)
        {
            if (truck.Update.HasDepartedScene)
            {
                if (TruckTracker.TryRemove(id, out _))
                {
                    if (truck.Truck.PlayerId == GameState.PlayerId.Value)
                    {
                        await SignalRClient.DestroyTruck(id);
                    }
                }

                return;
            }

            var cachedTruck = TruckTracker.GetOrAdd(
                id,
                new CachedTruck { TruckId = id });

            if (truck.Update.AtWarehouse && truck.Truck.PlayerId == GameState.PlayerId.Value)
            {
                if (!cachedTruck.ArrivalAtWarehouseAnnounced)
                {
                    lock (cachedTruck)
                    {
                        if (!cachedTruck.ArrivalAtWarehouseAnnounced)
                        {
                            SignalRClient.HandleTruckArrival(id); // Fire-and-forget
                            cachedTruck.ArrivalAtWarehouseAnnounced = true;
                        }
                    }
                }
            }
        }
    }
}
