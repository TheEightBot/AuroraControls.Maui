namespace AuroraControls.VisualEffects;

public class Hue : VisualEffect
{
    /// <summary>
    /// The amount of hue.
    /// </summary>
    public static readonly BindableProperty HueAmountProperty =
        BindableProperty.Create(nameof(HueAmount), typeof(double), typeof(Hue), 0d);

    /// <summary>
    /// Gets or sets the hue amount.
    /// </summary>
    /// <value>Hue amount as a double. Default is 0d.</value>
    public double HueAmount
    {
        get => (double)GetValue(HueAmountProperty);
        set => SetValue(HueAmountProperty, value.Clamp(-180, 180d));
    }

    public override SKImage ApplyEffect(SKImage image, SKSurface surface, SKImageInfo info, SKRect overrideRect) => InternalApplyEffect(surface, info.Rect);

    public override SKImage ApplyEffect(SKImage image, SKSurface surface, GRBackendRenderTarget info, SKRect overrideRect) => InternalApplyEffect(surface, info.Rect);

    private SKImage InternalApplyEffect(SKSurface surface, SKRect rect)
    {
        using (var paint = new SKPaint { })
        using (var surfaceImage = surface.Snapshot())
        using (var colorFilter = SKColorFilter.CreateColorMatrix(GetHueMatrix(HueAmount)))
        {
            paint.ColorFilter = colorFilter;
            surface.Canvas.Clear();
            surface.Canvas.DrawImage(surfaceImage, rect, paint);
        }

        return surface.Snapshot();
    }

    private float[] GetHueMatrix(double hueAdjustment)
    {
        hueAdjustment = hueAdjustment / 180 * Math.PI;
        var cosVal = (float)Math.Cos(hueAdjustment);
        var sinVal = (float)Math.Sin(hueAdjustment);
        const float lumR = 0.213f;
        const float lumG = 0.715f;
        const float lumB = 0.072f;

        return
        [
            lumR + (cosVal * (1 - lumR)) + (sinVal * (-lumR)), lumG + (cosVal * -lumG) + (sinVal * -lumG), lumB + (cosVal * -lumB) + (sinVal * (1 - lumB)), 0, 0,
            lumR + (cosVal * -lumR) + (sinVal * 0.143f), lumG + (cosVal * (1 - lumG)) + (sinVal * 0.140f), lumB + (cosVal * -lumB) + (sinVal * -0.283f), 0, 0,
            lumR + (cosVal * -lumR) + (sinVal * -(1 - lumR)), lumG + (cosVal * -lumG) + (sinVal * lumG), lumB + (cosVal * (1 - lumB)) + (sinVal * lumB), 0, 0,
            0f, 0f, 0f, 1f, 0f,
        ];
    }
}
