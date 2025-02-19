namespace AuroraControls.VisualEffects;

public class Brightness : VisualEffect
{
    /// <summary>
    /// The brightness amount property.
    /// </summary>
    public static BindableProperty BrightnessAmountProperty =
        BindableProperty.Create(nameof(BrightnessAmount), typeof(double), typeof(Brightness), 0d);

    /// <summary>
    /// Gets or sets brightness amount.
    /// </summary>
    /// <value>Brightness as a double. Default is 0d.</value>
    public double BrightnessAmount
    {
        get => (double)GetValue(BrightnessAmountProperty);
        set => SetValue(BrightnessAmountProperty, value.Clamp(-255d, 255d));
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
        using (var paint = new SKPaint { })
        using (var surfaceImage = surface.Snapshot())
        using (var colorFilter = SKColorFilter.CreateColorMatrix(GetBrightnessMatrix((float)BrightnessAmount)))
        {
            paint.ColorFilter = colorFilter;
            surface.Canvas.Clear();
            surface.Canvas.DrawImage(surfaceImage, rect, paint);
        }

        return surface.Snapshot();
    }

    private float[] GetBrightnessMatrix(float brightnessAdjustment)
    {
        return
        [
            1f, 0f, 0f, 0f, brightnessAdjustment,
            0f, 1f, 0f, 0f, brightnessAdjustment,
            0f, 0f, 1f, 0f, brightnessAdjustment,
            0f, 0f, 0f, 1f, 0f,
        ];
    }
}
