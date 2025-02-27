namespace AuroraControls.VisualEffects;

public class Saturation : VisualEffect
{
    /// <summary>
    /// The saturation amount property.
    /// </summary>
    public static BindableProperty SaturationAmountProperty =
        BindableProperty.Create(nameof(SaturationAmount), typeof(double), typeof(Saturation), 0d);

    /// <summary>
    /// Gets or sets the saturation amount.
    /// </summary>
    /// <value>Saturation amount as a double. Default is 0d.</value>
    public double SaturationAmount
    {
        get => (double)GetValue(SaturationAmountProperty);
        set => SetValue(SaturationAmountProperty, value.Clamp(-100d, 100d));
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
        using (var colorFilter = SKColorFilter.CreateColorMatrix(GetSaturationMatrix((float)SaturationAmount)))
        {
            paint.ColorFilter = colorFilter;
            surface.Canvas.Clear();
            surface.Canvas.DrawImage(surfaceImage, rect, paint);
        }

        return surface.Snapshot();
    }

    private float[] GetSaturationMatrix(float saturationAdjustment)
    {
        var x = 1 + ((saturationAdjustment > 0) ? 3 * saturationAdjustment / 100f : saturationAdjustment / 100f);
        const float lumR = 0.3086f;
        const float lumG = 0.6094f;
        const float lumB = 0.0820f;

        return
        [
            (lumR * (1 - x)) + x, lumG * (1 - x), lumB * (1 - x), 0f, 0f,
            lumR * (1 - x), (lumG * (1 - x)) + x, lumB * (1 - x), 0f, 0f,
            lumR * (1 - x), lumG * (1 - x), (lumB * (1 - x)) + x, 0f, 0f,
            0f, 0f, 0f, 1f, 0f,
        ];
    }
}
