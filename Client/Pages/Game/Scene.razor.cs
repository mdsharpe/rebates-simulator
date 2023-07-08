using Blazor.Extensions;
using Blazor.Extensions.Canvas.Canvas2D;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Reactive;
using System.Reactive.Linq;

namespace RebatesSimulator.Client.Pages.Game
{
    public partial class Scene : ComponentBase
    {
        protected BECanvasComponent CanvasComponent = new();
        private Canvas2DContext? _canvas;
        protected ElementReference PageContainer;

        [Inject]
        private GameStateWrapper? GameStateWrapper { get; set; }

        [Inject]
        private IJSRuntime JsRuntime { get; set; } = default!;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            _canvas = await CanvasComponent.CreateCanvas2DAsync();

            await JsRuntime.InvokeVoidAsync("fixCanvasSizes");

            //Console.WriteLine("Drawing image");
            //await Task.Delay(1000);
            //await _canvas.DrawImageAsync(Scenery, 0, 0);

            GameStateWrapper!.GameState
                .Where(gs => gs is not null)
                .CombineLatest(Observable.Interval(TimeSpan.FromMilliseconds(50)))
                .Subscribe(async o => await DrawScene(o));

            base.OnAfterRender(firstRender);
        }

        protected override async Task OnParametersSetAsync()
        {
            
        }

        private async Task DrawScene((GameState GameState, long _) foo)
        {
            //Console.WriteLine("Drawing scene");

            var canvasWidth = await JsRuntime.InvokeAsync<int>("getTrueCanvasWidth");
            var canvasHeight = await JsRuntime.InvokeAsync<int>("getTrueCanvasHeight");

            await _canvas.ClearRectAsync(0, 0, canvasWidth, canvasHeight);

            foreach (var truck in foo.GameState.Trucks)
            {
                var position = TruckMover.GetTruckPosition(
                    truck,
                    canvasWidth,
                    canvasHeight,
                    canvasHeight / 7, // ish
                    canvasHeight / 2,
                    new()
                    {
                        { 0, Convert.ToInt32(canvasWidth * 0.5) },
                        { 1, Convert.ToInt32(canvasWidth * 0.7) },
                        { 2, Convert.ToInt32(canvasWidth * 0.4) },
                        { 3, Convert.ToInt32(canvasWidth * 0.6) }
                    });

                await _canvas.FillRectAsync(position.X, position.Y, 30, 30);
            }
        }
    }
}
