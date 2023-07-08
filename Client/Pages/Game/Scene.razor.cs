using Blazor.Extensions;
using Blazor.Extensions.Canvas;
using Blazor.Extensions.Canvas.Canvas2D;
using Microsoft.AspNetCore.Components;
using System.Reactive.Linq;
using System.Reactive.Subjects;


namespace RebatesSimulator.Client.Pages.Game
{
    public partial class Scene : ComponentBase
    {
        protected BECanvasComponent CanvasComponent = new();
        private Canvas2DContext? _canvas;
        protected ElementReference Scenery;

        [Inject]
        private GameStateWrapper? GameStateWrapper { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            _canvas = await CanvasComponent.CreateCanvas2DAsync();

            Console.WriteLine("Drawing image");
            await Task.Delay(1000);
            await _canvas.DrawImageAsync(Scenery, 0, 0);

            GameStateWrapper!.GameState
                .Where(gs => gs is not null)
                .Subscribe(async gs => await DrawScene(gs!));

            base.OnAfterRender(firstRender);
        }

        protected override async Task OnParametersSetAsync()
        {
            
        }

        private async Task DrawScene(GameState gameState)
        {
            foreach (var truck in gameState.Trucks)
            {
                var position = TruckMover.GetTruckPosition(
                    truck,
                    69,
                    69,
                    69,
                    34,
                    new() { { 0, 0 }, { 1, 1 }, { 2, 2 }, { 3, 3 } });

                await _canvas.FillRectAsync(position.X, position.Y, 30, 30);
            }
        }
    }
}
