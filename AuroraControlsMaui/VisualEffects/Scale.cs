namespace AuroraControls.VisualEffects;

public class Scale : VisualEffect
{
    /// <summary>
    /// The scale amount property.
    /// </summary>
    public static BindableProperty ScaleAmountProperty =
        BindableProperty.Create(nameof(ScaleAmount), typeof(float), typeof(Scale), 1f);

    /// <summary>
    /// Gets or sets the amount of scaling.
    /// </summary>
    /// <value>Scale amount as a float. Default is 1f.</value>
    public float ScaleAmount
    {
        get { return (float)GetValue(ScaleAmountProperty); }
        set { SetValue(ScaleAmountProperty, value); }
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

            using (new SKAutoCanvasRestore(canvas))
            {
                var matrix = SKMatrix.CreateScale(ScaleAmount, ScaleAmount, rect.MidX, rect.MidY);

                canvas.SetMatrix(matrix);

                canvas.DrawImage(surfaceSnapshot, rect);
            }
        }

        return surface.Snapshot();
    }
}
