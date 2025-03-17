namespace AuroraControls.VisualEffects;

public class Sepia : VisualEffect
{
    private static readonly float[] _colorMatrix =
    [
        0.393f, 0.349f, 0.272f, 0.0f, 0.0f,
        0.769f, 0.686f, 0.534f, 0.0f, 0.0f,
        0.189f, 0.168f, 0.131f, 0.0f, 0.0f,
        0.0f,  0.0f,  0.0f,  1.0f, 0.0f,
    ];

    private readonly SKColorFilter _colorFilter;

    public Sepia() => _colorFilter = SKColorFilter.CreateColorMatrix(_colorMatrix);

    public override SKImage ApplyEffect(SKImage image, SKSurface surface, SKImageInfo info, SKRect overrideRect) => InternalApplyEffect(surface, info.Rect);

    public override SKImage ApplyEffect(SKImage image, SKSurface surface, GRBackendRenderTarget info, SKRect overrideRect) => InternalApplyEffect(surface, info.Rect);

    private SKImage InternalApplyEffect(SKSurface surface, SKRect rect)
    {
        using (var paint = new SKPaint { ColorFilter = _colorFilter, })
        using (var surfaceImage = surface.Snapshot())
        {
            surface.Canvas.Clear();
            surface.Canvas.DrawImage(surfaceImage, rect, paint);
        }

        return surface.Snapshot();
    }
}
