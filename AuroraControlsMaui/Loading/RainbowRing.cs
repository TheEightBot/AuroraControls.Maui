using System;

namespace AuroraControls.Loading;

#pragma warning disable CA1001 // Types that own disposable fields should be disposable
public class RainbowRing : SceneViewBase
#pragma warning restore CA1001 // Types that own disposable fields should be disposable
{
    private const int RingCount = 8;

    private readonly SKColor[] _colors =
        new[]
        {
            SKColor.Parse("#99f44336"),
            SKColor.Parse("#99ff5722"),
            SKColor.Parse("#99ffeb3b"),
            SKColor.Parse("#9900bcd4"),
            SKColor.Parse("#993f51b5"),
            SKColor.Parse("#99673ab7"),
            SKColor.Parse("#999c27b0"),
            SKColor.Parse("#bffafafa"),
        };

    private readonly Random _rng = new Random(Guid.NewGuid().GetHashCode());

    private readonly Point[] _movementAmount = new Point[RingCount];

    private bool _firstRun = true;

    private float _maxLength;
    private int _maxBuffer;

    private SKPaint _ringPaint;
    private SKPaint _blurPaint;

    /// <summary>
    /// Specifies the ring thickness.
    /// </summary>
    public static readonly BindableProperty RingThicknessProperty =
        BindableProperty.Create(nameof(RingThickness), typeof(double), typeof(RainbowRing), 12d,
            propertyChanged:
                static (bindable, _, newValue) =>
                {
                    if (bindable is RainbowRing current && current._ringPaint != null)
                    {
                        var value = (float)(double)newValue;
                        current._ringPaint.StrokeWidth = value;
                        current._blurPaint.ImageFilter = SKImageFilter.CreateBlur(value, value);
                    }
                });

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
    public RainbowRing() => MinimumHeightRequest = 88;

    protected override void Attached()
    {
        var ringThickness = (float)this.RingThickness;

        _ringPaint =
            new SKPaint()
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = ringThickness,
            };

        _blurPaint =
            new SKPaint()
            {
                FilterQuality = SKFilterQuality.Medium,
                ImageFilter = SKImageFilter.CreateBlur(ringThickness, ringThickness),
            };

        base.Attached();
    }

    protected override void Detached()
    {
        base.Detached();

        var ringPaint = _ringPaint;
        _ringPaint = null;
        ringPaint?.Dispose();
        ringPaint = null;

        var blurPaint = _blurPaint;
        _blurPaint = null;
        blurPaint?.Dispose();
        blurPaint = null;
    }

    /// <summary>
    /// This is the method used to draw our control on the SKCanvas. This method is fired every time <c>this.InvalidateSurface();</c> is called, resulting in a "redrawing" of the control.
    /// </summary>
    /// <param name="surface">The skia surface to paint on the controls.</param>
    /// <param name="info">Information about the skia image.</param>
    /// <param name="percentage">The animation percentage.</param>
    protected override SKImage PaintScene(SKSurface surface, SKImageInfo info, double percentage)
    {
        if (_firstRun)
        {
            _firstRun = false;

            _maxLength = Math.Min(info.Rect.Width, info.Rect.Height) * .5f * .8f;

            _maxBuffer = (int)(Math.Min(info.Rect.Width, info.Rect.Height) * .5f * .2f);

            UpdateAnimationValues();
        }

        var canvas = surface.Canvas;

        canvas.Clear();

        var progress = (float)percentage;

        for (int i = 0; i < RingCount; i++)
        {
            var movementX =
                progress < .5f
                    ? (float)_movementAmount[i].X * (progress * 2f)
                    : (float)_movementAmount[i].X * (1f - ((progress - .5f) * 2f));

            var movementY =
                progress < .5f
                    ? (float)_movementAmount[i].Y * (progress * 2f)
                    : (float)_movementAmount[i].Y * (1f - ((progress - .5f) * 2f));

            if (_ringPaint != null)
            {
                _ringPaint.Color = _colors[i];
            }

            if (_ringPaint != null)
            {
                canvas.DrawOval(info.Rect.MidX + movementX, info.Rect.MidY + movementY, _maxLength, _maxLength, _ringPaint);
            }
        }

        canvas.RotateDegrees(progress * 360f, info.Rect.MidX, info.Rect.MidY);

        canvas.DrawImage(surface.Snapshot(), SKPoint.Empty, _blurPaint);

        canvas.Flush();

        return surface.Snapshot();
    }

    private void UpdateAnimationValues()
    {
        var maxMovement = _maxBuffer - (int)RingThickness;

        for (int i = 0; i < RingCount; i++)
        {
            _movementAmount[i] = new Point(_rng.Next(-maxMovement, maxMovement), _rng.Next(-maxMovement, maxMovement));
        }
    }
}
