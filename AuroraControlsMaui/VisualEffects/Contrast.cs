namespace AuroraControls.VisualEffects;

public class Contrast : VisualEffect
{
    private readonly double[] _deltaIndex =
        [
            0,    0.01f, 0.02, 0.04, 0.05, 0.06, 0.07, 0.08, 0.1,  0.11,
            0.12, 0.14, 0.15, 0.16, 0.17, 0.18, 0.20, 0.21, 0.22, 0.24,
            0.25, 0.27, 0.28, 0.30, 0.32, 0.34, 0.36, 0.38, 0.40, 0.42,
            0.44, 0.46, 0.48, 0.5,  0.53, 0.56, 0.59, 0.62, 0.65, 0.68,
            0.71, 0.74, 0.77, 0.80, 0.83, 0.86, 0.89, 0.92, 0.95, 0.98,
            1.0,  1.06, 1.12, 1.18, 1.24, 1.30, 1.36, 1.42, 1.48, 1.54,
            1.60, 1.66, 1.72, 1.78, 1.84, 1.90, 1.96, 2.0,  2.12, 2.25,
            2.37, 2.50, 2.62, 2.75, 2.87, 3.0,  3.2,  3.4,  3.6,  3.8,
            4.0,  4.3,  4.7,  4.9,  5.0,  5.5,  6.0,  6.5,  6.8,  7.0,
            7.3,  7.5,  7.8,  8.0,  8.4,  8.7,  9.0,  9.4,  9.6,  9.8,
            10.0,
        ];

    /// <summary>
    /// The contrast amount property.
    /// </summary>
    public static readonly BindableProperty ContrastAmountProperty =
        BindableProperty.Create(nameof(ContrastAmount), typeof(int), typeof(Contrast), 0);

    /// <summary>
    /// Gets or sets contrast amount.
    /// </summary>
    /// <value>Contrast as an int. Default is 0.</value>
    public int ContrastAmount
    {
        get => (int)GetValue(ContrastAmountProperty);
        set => SetValue(ContrastAmountProperty, value.Clamp(-100, 100));
    }

    public override SKImage ApplyEffect(SKImage image, SKSurface surface, SKImageInfo info, SKRect overrideRect) => InternalApplyEffect(surface, info.Rect);

    public override SKImage ApplyEffect(SKImage image, SKSurface surface, GRBackendRenderTarget info, SKRect overrideRect) => InternalApplyEffect(surface, info.Rect);

    private SKImage InternalApplyEffect(SKSurface surface, SKRect rect)
    {
        using (var paint = new SKPaint { })
        using (var surfaceImage = surface.Snapshot())
        using (var colorFilter = SKColorFilter.CreateColorMatrix(GetContrastMatrix(ContrastAmount)))
        {
            paint.ColorFilter = colorFilter;
            surface.Canvas.Clear();
            surface.Canvas.DrawImage(surfaceImage, rect, paint);
        }

        return surface.Snapshot();
    }

    private float[] GetContrastMatrix(int contrastAdjustment)
    {
        float x = 0f;

        if (contrastAdjustment < 0)
        {
            x = 127f + ((contrastAdjustment / 100f) * 127f);
        }
        else
        {
            x = contrastAdjustment % 1;
            if (Math.Abs(x) < float.Epsilon)
            {
                x = (float)this._deltaIndex[contrastAdjustment];
            }
            else
            {
                x = (float)((this._deltaIndex[contrastAdjustment << 0] * (1 - x)) + (this._deltaIndex[(contrastAdjustment << 0) + 1] * x)); // use linear interpolation for more granularity.
            }

            x = (x * 127) + 127;
        }

        return
        [
            x / 127f, 0f, 0f, 0f, 0.5f * (127f - x),
            0f, x / 127f, 0f, 0f, 0.5f * (127f - x),
            0f, 0f, x / 127f, 0f, 0.5f * (127f - x),
            0f, 0f, 0f, 1f, 0f,
        ];
    }
}
