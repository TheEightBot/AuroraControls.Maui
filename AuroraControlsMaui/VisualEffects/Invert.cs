namespace AuroraControls.VisualEffects;

public class Invert : VisualEffect
{
    private static readonly float[] _colorMatrix =
        [
            -1f,    0f,     0f,     0f,     255,
            0f,     -1f,    0f,     0f,     255,
            0f,     0f,     -1f,    0f,     255,
            0f,     0f,     0f,     1f,     0f,
        ];

    private readonly SKColorFilter _colorFilter;

    public Invert() => _colorFilter = SKColorFilter.CreateColorMatrix(_colorMatrix);

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
