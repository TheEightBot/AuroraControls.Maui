namespace AuroraControls.Loading;

/// <summary>
/// Waves loading animation.
/// </summary>
public class Waves : LoadingViewBase
{
    private readonly Random _rng;

    private readonly double _randomSeed;

    /// <summary>
    /// Specifies the number of waves.
    /// </summary>
    public static readonly BindableProperty WaveCountProperty =
        BindableProperty.Create(nameof(WaveCount), typeof(int), typeof(Waves), 5,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the wave count.
    /// </summary>
    /// <value>The wave count int. Default wave count value is 5.</value>
    public int WaveCount
    {
        get => (int)GetValue(WaveCountProperty);
        set => SetValue(WaveCountProperty, value);
    }

    /// <summary>
    /// The wave height property. Specifies the height of the wave.
    /// </summary>
    public static readonly BindableProperty WaveHeightProperty =
        BindableProperty.Create(nameof(WaveHeight), typeof(double), typeof(Waves), 40d,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the height of the wave.
    /// </summary>
    /// <value>Height as a double. Default is 40d.</value>
    public double WaveHeight
    {
        get => (double)GetValue(WaveHeightProperty);
        set => SetValue(WaveHeightProperty, value);
    }

    /// <summary>
    /// The wave stacks property.
    /// </summary>
    public static readonly BindableProperty WaveStacksProperty =
        BindableProperty.Create(nameof(WaveStacks), typeof(int), typeof(Waves), 3,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the wave stacks.
    /// </summary>
    /// <value>The wave stacks. Default value is 3.</value>
    public int WaveStacks
    {
        get => (int)GetValue(WaveStacksProperty);
        set => SetValue(WaveStacksProperty, value);
    }

    /// <summary>
    /// The foreground wave color property. Specifies the foreground wave color.
    /// </summary>
    public static readonly BindableProperty ForegroundWaveColorProperty =
        BindableProperty.Create(nameof(ForegroundWaveColor), typeof(Color), typeof(Waves), Colors.SkyBlue,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the color of the foreground wave.
    /// </summary>
    /// <value>Takes a Color. Default value is default(Color).</value>
    public Color ForegroundWaveColor
    {
        get => (Color)GetValue(ForegroundWaveColorProperty);
        set => SetValue(ForegroundWaveColorProperty, value);
    }

    /// <summary>
    /// The background wave color property.
    /// </summary>
    public static readonly BindableProperty BackgroundWaveColorProperty =
        BindableProperty.Create(nameof(BackgroundWaveColor), typeof(Color), typeof(Waves), Colors.White,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the color of the background wave.
    /// </summary>
    /// <value>Takes a Color. Default value is default(Color).</value>
    public Color BackgroundWaveColor
    {
        get => (Color)GetValue(BackgroundWaveColorProperty);
        set => SetValue(BackgroundWaveColorProperty, value);
    }

    public Waves()
    {
        _rng = new Random(Guid.NewGuid().GetHashCode());
        _randomSeed = _rng.NextDouble();
    }

    /// <summary>
    /// This is the method used to draw our control on the SKCanvas. This method is fired every time <c>this.InvalidateSurface();</c> is called, resulting in a "redrawing" of the control.
    /// </summary>
    /// <param name="surface">The skia surface to paint on the controls.</param>
    /// <param name="info">Information about the skia image.</param>
    protected override void PaintControl(SKSurface surface, SKImageInfo info)
    {
        var canvas = surface.Canvas;

        float waveLength = info.Width / (float)WaveCount;

        float halfHeight = info.Height / 2f;

        float stackPercentage = (100f / WaveStacks) / 100f;

        canvas.Clear();

        using (new SKAutoCanvasRestore(canvas))
        using (var foregroundPaint = new SKPaint())
        using (var foregroundPath = new SKPath())
        {
            foregroundPaint.Color = ForegroundWaveColor.ToSKColor();
            foregroundPaint.Style = SKPaintStyle.Fill;
            foregroundPaint.StrokeWidth = 10f;
            foregroundPaint.IsAntialias = true;

            for (int waveStack = WaveStacks - 1; waveStack >= 0; waveStack--)
            {
                foregroundPath.Reset();

                float waveHeight = (float)(WaveHeight + (WaveHeight * _randomSeed * (AnimatingPercentage < .5d ? AnimatingPercentage : 1 - AnimatingPercentage)));

                float stackWaveLength = info.Width / (float)(WaveCount * (waveStack + 1));
                float stackWaveHeight = waveHeight * stackPercentage * (WaveStacks - waveStack);
                float stackHalfHeight = halfHeight - (waveHeight / WaveStacks * waveStack);

                foregroundPath.MoveTo(new SKPoint(0, stackHalfHeight));

                for (int waveCount = 0; waveCount < (WaveCount * (waveStack + 1)) + 2; waveCount++)
                {
                    if (waveCount % 2 == 0)
                    {
                        foregroundPath.QuadTo((stackWaveLength * waveCount) + (stackWaveLength / 2f), stackHalfHeight - stackWaveHeight, (stackWaveLength * waveCount) + stackWaveLength, stackHalfHeight);
                    }
                    else
                    {
                        foregroundPath.QuadTo((stackWaveLength * waveCount) + (stackWaveLength / 2f), stackHalfHeight + stackWaveHeight, (stackWaveLength * waveCount) + stackWaveLength, stackHalfHeight);
                    }
                }

                foregroundPath.LineTo(foregroundPath.Bounds.Width, info.Height);
                foregroundPath.LineTo(0, info.Height);
                foregroundPath.Close();

                foregroundPaint.Color = ForegroundWaveColor.Lerp(BackgroundWaveColor, stackPercentage * waveStack).ToSKColor();

                canvas.SetMatrix(SKMatrix.CreateTranslation(-stackWaveLength * 2f * (float)AnimatingPercentage, 0f));

                canvas.DrawPath(foregroundPath, foregroundPaint);
            }
        }
    }
}
