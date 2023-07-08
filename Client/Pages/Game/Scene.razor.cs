using Blazor.Extensions;
using Blazor.Extensions.Canvas;
using Blazor.Extensions.Canvas.Canvas2D;
using Microsoft.AspNetCore.Components;

namespace RebatesSimulator.Client.Pages.Game
{
    public partial class Scene : ComponentBase
    {
        protected BECanvasComponent CanvasComponent = new();
        private Canvas2DContext? _canvas;
        protected ElementReference Scenery;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            _canvas = await CanvasComponent.CreateCanvas2DAsync();

            Console.WriteLine("Drawing image");
            await _canvas.DrawImageAsync(Scenery, 0, 0);

            base.OnAfterRender(firstRender);
        }

        protected override async Task OnParametersSetAsync()
        {
        }
    }
}
