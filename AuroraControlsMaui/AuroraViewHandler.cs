using SkiaSharp.Views.Maui.Handlers;

namespace AuroraControls;

public class AuroraViewHandler : SKCanvasViewHandler
{
    public AuroraViewHandler()
         : base(SKCanvasViewMapper, SKCanvasViewCommandMapper)
    {
    }
}
