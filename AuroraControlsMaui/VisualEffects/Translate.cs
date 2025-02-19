namespace AuroraControls.VisualEffects;

public class Translate : VisualEffect
{
    /// <summary>
    /// The translation property.
    /// </summary>
    public static BindableProperty TranslationProperty =
        BindableProperty.Create(nameof(Translation), typeof(Point), typeof(Translate), default(Point));

    /// <summary>
    /// Gets or sets translation point.
    /// </summary>
    /// <value>Takes a Point. Default is default(Point).</value>
    public Point Translation
    {
        get => (Point)GetValue(TranslationProperty);
        set => SetValue(TranslationProperty, value);
    }

    public override SKImage ApplyEffect(SKImage image, SKSurface surface, SKImageInfo info, SKRect overrideRect)
    {
        return InternalApplyEffect(surface, info.Rect);
    }

    public override SKImage ApplyEffect(SKImage image, SKSurface surface, GRBackendRenderTarget info, SKRect overrideRect)
    {
        return InternalApplyEffect(surface, info.Rect);
    }

    private SKImage InternalApplyEffect(SKSurface surface, SKRect rect)
    {
        var canvas = surface.Canvas;

        using (var surfaceSnapshot = surface.Snapshot())
        {
            canvas.Clear();

            var translation = this.Translation;
            using (new SKAutoCanvasRestore(canvas))
            {
                var matrix = SKMatrix.CreateTranslation((float)translation.X, (float)translation.Y);

                canvas.SetMatrix(matrix);

                canvas.DrawImage(surfaceSnapshot, rect);
            }
        }

        return surface.Snapshot();
    }
}
