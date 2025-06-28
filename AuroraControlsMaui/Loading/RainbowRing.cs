namespace AuroraControls.Loading;

#pragma warning disable CA1001 // Types that own disposable fields should be disposable
public class RainbowRing : SceneViewBase
#pragma warning restore CA1001 // Types that own disposable fields should be disposable
{
    private const int RingCount = 6; // Reduced from 8 for better performance

    private readonly SKColor[] _colors =
    {
        SKColor.Parse("#f44336"), // Red
        SKColor.Parse("#ff9800"), // Orange
        SKColor.Parse("#ffeb3b"), // Yellow
        SKColor.Parse("#4caf50"), // Green
        SKColor.Parse("#2196f3"), // Blue
        SKColor.Parse("#9c27b0"),  // Purple
    };

    private SKPaint _ringPaint;
    private float _ringRadius;
    private bool _initialized;

    /// <summary>
    /// Specifies the ring thickness.
    /// </summary>
    public static readonly BindableProperty RingThicknessProperty =
        BindableProperty.Create(nameof(RingThickness), typeof(double), typeof(RainbowRing), 8d);

    /// <summary>
    /// Gets or sets the thickness of the ring.
    /// </summary>
    /// <value>Thickness as a double. Default is 12d.</value>
    public double RingThickness
    {
        get => (double)GetValue(RingThicknessProperty);
        set => SetValue(RingThicknessProperty, value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RainbowRing"/> class.
    /// </summary>
    public RainbowRing()
    {
        MinimumHeightRequest = 88;

        // Slow down the animation significantly
        Length = 3000; // 3 seconds per cycle instead of 1.6 seconds
        Rate = 60;     // ~30 FPS instead of ~60 FPS
    }

    protected override void Attached()
    {
        _ringPaint = new SKPaint
        {
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = (float)RingThickness,
            StrokeCap = SKStrokeCap.Round,
        };

        base.Attached();
    }

    protected override void Detached()
    {
        _ringPaint?.Dispose();
        _ringPaint = null;
        base.Detached();
    }

    /// <summary>
    /// This is the method used to draw our control on the SKCanvas. This method is fired every time <c>this.InvalidateSurface();</c> is called, resulting in a "redrawing" of the control.
    /// </summary>
    /// <param name="surface">The skia surface to paint on the controls.</param>
    /// <param name="info">Information about the skia image.</param>
    /// <param name="percentage">The animation percentage.</param>
    protected override SKImage PaintScene(SKSurface surface, SKImageInfo info, double percentage)
    {
        if (!_initialized)
        {
            var minDimension = Math.Min(info.Width, info.Height);
            _ringRadius = (minDimension * 0.3f) - (float)RingThickness;
            _initialized = true;
        }

        var canvas = surface.Canvas;
        canvas.Clear();

        var centerX = info.Rect.MidX;
        var centerY = info.Rect.MidY;

        // Draw each ring with a slight offset and rotation
        for (int i = 0; i < RingCount; i++)
        {
            // Calculate rotation for this ring
            var ringRotation = (percentage * 360.0) + (i * 60.0); // Each ring offset by 60 degrees

            // Calculate opacity based on position in cycle
            var opacity = (byte)(128 + (127 * Math.Sin((percentage * Math.PI * 2) + (i * Math.PI / 3))));

            // Set color with calculated opacity
            _ringPaint.Color = _colors[i].WithAlpha(opacity);

            // Calculate ring position (slight spiral effect)
            var radiusOffset = (float)(Math.Sin((percentage * Math.PI * 2) + (i * Math.PI / 6)) * 5);
            var currentRadius = _ringRadius + radiusOffset;

            // Draw the ring
            canvas.Save();
            canvas.RotateDegrees((float)ringRotation, centerX, centerY);

            // Draw arc instead of full circle for more dynamic effect
            var sweepAngle = 300f; // 300 degree arc
            var startAngle = (float)(percentage * 360);

            var rect = new SKRect(
                centerX - currentRadius,
                centerY - currentRadius,
                centerX + currentRadius,
                centerY + currentRadius);

            canvas.DrawArc(rect, startAngle, sweepAngle, false, _ringPaint);
            canvas.Restore();
        }

        return surface.Snapshot();
    }
}
