using System.Collections.ObjectModel;
using System.Numerics;

namespace AuroraControls;

/// <summary>
/// A MAUI control for drawing smooth signatures using Bézier curve interpolation.
/// Based on android-signaturepad by gcacace (https://github.com/gcacace/android-signaturepad).
/// </summary>
#pragma warning disable CA1001
public class SignaturePad : AuroraViewBase
#pragma warning restore CA1001
{
    private readonly List<SignaturePoint> _points = new();
    private readonly List<TimedBezierCurve> _curves = new();
    private readonly object _syncLock = new();

    private float _lastVelocity;
    private float _lastWidth;
    private SignaturePoint? _lastPoint;
    private SKPath _path = new();
    private bool _isDrawing;

    /// <summary>
    /// Gets or sets the minimum width of the stroke.
    /// </summary>
    public static readonly BindableProperty MinWidthProperty =
        BindableProperty.Create(nameof(MinWidth), typeof(float), typeof(SignaturePad), 3f,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the maximum width of the stroke.
    /// </summary>
    public static readonly BindableProperty MaxWidthProperty =
        BindableProperty.Create(nameof(MaxWidth), typeof(float), typeof(SignaturePad), 7f,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the color of the pen.
    /// </summary>
    public static readonly BindableProperty PenColorProperty =
        BindableProperty.Create(nameof(PenColor), typeof(Color), typeof(SignaturePad), Colors.Black,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the weight used to modify new velocity based on the previous velocity.
    /// </summary>
    public static readonly BindableProperty VelocityFilterWeightProperty =
        BindableProperty.Create(nameof(VelocityFilterWeight), typeof(float), typeof(SignaturePad), 0.9f,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets whether double-clicking clears the pad.
    /// </summary>
    public static readonly BindableProperty ClearOnDoubleClickProperty =
        BindableProperty.Create(nameof(ClearOnDoubleClick), typeof(bool), typeof(SignaturePad), false);

    /// <summary>
    /// Gets or sets a value indicating whether the signature pad has been signed.
    /// </summary>
    public static readonly BindableProperty IsSignedProperty =
        BindableProperty.Create(nameof(IsSigned), typeof(bool), typeof(SignaturePad), false,
            BindingMode.OneWayToSource);

    /// <summary>
    /// Gets or sets the minimum width of the stroke.
    /// </summary>
    public float MinWidth
    {
        get => (float)GetValue(MinWidthProperty);
        set => SetValue(MinWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum width of the stroke.
    /// </summary>
    public float MaxWidth
    {
        get => (float)GetValue(MaxWidthProperty);
        set => SetValue(MaxWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets the color of the pen.
    /// </summary>
    public Color PenColor
    {
        get => (Color)GetValue(PenColorProperty);
        set => SetValue(PenColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the weight used to modify new velocity based on the previous velocity.
    /// </summary>
    public float VelocityFilterWeight
    {
        get => (float)GetValue(VelocityFilterWeightProperty);
        set => SetValue(VelocityFilterWeightProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether gets or sets whether double-clicking clears the pad.
    /// </summary>
    public bool ClearOnDoubleClick
    {
        get => (bool)GetValue(ClearOnDoubleClickProperty);
        set => SetValue(ClearOnDoubleClickProperty, value);
    }

    /// <summary>
    /// Gets a value indicating whether the signature pad has been signed.
    /// </summary>
    public bool IsSigned
    {
        get => (bool)GetValue(IsSignedProperty);
        private set => SetValue(IsSignedProperty, value);
    }

    /// <summary>
    /// Event triggered when the pad is touched.
    /// </summary>
    public event EventHandler? StartedSigning;

    /// <summary>
    /// Event triggered when the pad is signed.
    /// </summary>
    public event EventHandler? Signed;

    /// <summary>
    /// Event triggered when the pad is cleared.
    /// </summary>
    public event EventHandler? Cleared;

    /// <summary>
    /// Initializes a new instance of the <see cref="SignaturePad"/> class.
    /// </summary>
    public SignaturePad()
    {
        MinimumHeightRequest = 100;
        MinimumWidthRequest = 100;
    }

    /// <summary>
    /// Called when the control is attached to the visual tree.
    /// </summary>
    protected override void Attached()
    {
        EnableTouchEvents = true;
        base.Attached();
    }

    /// <summary>
    /// Called when touch events occur on the canvas.
    /// </summary>
    /// <param name="e">Touch event arguments.</param>
    protected override void OnTouch(SKTouchEventArgs e)
    {
        e.Handled = true;

        switch (e.ActionType)
        {
            case SKTouchAction.Pressed:
                HandleTouchStart(e);
                break;

            case SKTouchAction.Moved:
                HandleTouchMove(e);
                break;

            case SKTouchAction.Released:
                HandleTouchEnd();
                break;

            case SKTouchAction.Cancelled:
            case SKTouchAction.Exited:
                _isDrawing = false;
                break;
        }
    }

    /// <summary>
    /// Handles the paint surface event.
    /// </summary>
    /// <param name="surface">The surface to paint on.</param>
    /// <param name="info">Information about the surface.</param>
    protected override void PaintControl(SKSurface surface, SKImageInfo info)
    {
        var canvas = surface.Canvas;
        canvas.Clear(SKColors.Transparent);

        using var paint = new SKPaint
        {
            Color = PenColor.ToSKColor(),
            Style = SKPaintStyle.Stroke,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round,
            IsAntialias = true,
        };

        lock (_syncLock)
        {
            // Draw all curves with varying width
            foreach (var curve in _curves)
            {
                DrawBezier(canvas, curve, paint);
            }
        }
    }

    /// <summary>
    /// Clears the signature.
    /// </summary>
    public void Clear()
    {
        lock (_syncLock)
        {
            _path.Reset();
            _points.Clear();
            _curves.Clear();
            _lastPoint = null;
            _lastVelocity = 0f;
            _lastWidth = 0f;
            _isDrawing = false;
            IsSigned = false;
        }

        InvalidateSurface();
        Cleared?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Gets a bitmap of the signature with a white background.
    /// </summary>
    /// <returns>A bitmap of the signature.</returns>
    public SKBitmap GetSignatureBitmap(Color? backgroundColor = null)
    {
        var bitmap = new SKBitmap((int)CanvasSize.Width, (int)CanvasSize.Height);

        using var canvas = new SKCanvas(bitmap);
        canvas.Clear((backgroundColor ?? Colors.White).ToSKColor());

        using var paint = new SKPaint
        {
            Color = PenColor.ToSKColor(),
            Style = SKPaintStyle.Stroke,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round,
            IsAntialias = true,
        };

        lock (_syncLock)
        {
            foreach (var curve in _curves)
            {
                DrawBezier(canvas, curve, paint);
            }
        }

        return bitmap;
    }

    public Stream GetSignatureImageStream(SKEncodedImageFormat format, int quality = 100, Color? backgroundColor = null)
    {
        using var imageBitmap = GetSignatureBitmap(backgroundColor);

        return imageBitmap.Encode(format, quality).AsStream();
    }

    /// <summary>
    /// Gets a bitmap of the signature with a transparent background.
    /// </summary>
    /// <returns>A bitmap of the signature.</returns>
    public SKBitmap GetTransparentSignatureBitmap()
    {
        var bitmap = new SKBitmap((int)CanvasSize.Width, (int)CanvasSize.Height);

        using var canvas = new SKCanvas(bitmap);
        canvas.Clear(SKColors.Transparent);

        using var paint = new SKPaint
        {
            Color = PenColor.ToSKColor(),
            Style = SKPaintStyle.Stroke,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round,
            IsAntialias = true,
        };

        lock (_syncLock)
        {
            foreach (var curve in _curves)
            {
                DrawBezier(canvas, curve, paint);
            }
        }

        return bitmap;
    }

    /// <summary>
    /// Gets an SVG representation of the signature.
    /// </summary>
    /// <returns>A string containing the SVG data.</returns>
    public string GetSignatureSvg()
    {
        float width = (float)CanvasSize.Width;
        float height = (float)CanvasSize.Height;

        var svg = new System.Text.StringBuilder();
        svg
            .AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\"?>")
            .AppendLine($"<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"{width}\" height=\"{height}\" viewBox=\"0 0 {width} {height}\">");

        var penColorHex = $"#{PenColor.ToHex().Substring(3)}";

        lock (_syncLock)
        {
            // Each curve becomes a path in the SVG
            foreach (var curve in _curves)
            {
                var startPoint = curve.StartPoint;
                var control1 = curve.Control1;
                var control2 = curve.Control2;
                var endPoint = curve.EndPoint;

                svg
                    .AppendLine($"<path fill=\"none\" stroke=\"{penColorHex}\" stroke-width=\"{curve.Width}\" ")
                    .AppendLine($"d=\"M {startPoint.X},{startPoint.Y} C {control1.X},{control1.Y} {control2.X},{control2.Y} {endPoint.X},{endPoint.Y}\"/>");
            }
        }

        svg.AppendLine("</svg>");
        return svg.ToString();
    }

    private void HandleTouchStart(SKTouchEventArgs e)
    {
        _isDrawing = true;
        var point = CreateSignaturePoint(e.Location, DateTimeOffset.Now.ToUnixTimeMilliseconds());

        lock (_syncLock)
        {
            _points.Clear();
            _lastPoint = point;
            _points.Add(point);
            _path.Reset();
            _path.MoveTo(point.X, point.Y);
        }

        StartedSigning?.Invoke(this, EventArgs.Empty);
        InvalidateSurface();
    }

    private void HandleTouchMove(SKTouchEventArgs e)
    {
        if (!_isDrawing)
        {
            return;
        }

        var point = CreateSignaturePoint(e.Location, DateTimeOffset.Now.ToUnixTimeMilliseconds());

        lock (_syncLock)
        {
            if (_lastPoint != null)
            {
                // Add the current point to the path
                _points.Add(point);

                // When we have enough points, create a cubic bezier curve
                if (_points.Count > 2)
                {
                    // Get the last 3 points to construct a cubic bezier
                    var p0 = _points[_points.Count - 3];
                    var p1 = _points[_points.Count - 2];
                    var p2 = _points[_points.Count - 1];

                    // The stroke width is based on velocity
                    var velocity = CalculateVelocity(p1, p2);
                    var width = CalculateStrokeWidth(velocity);

                    // Calculate control points
                    var c1 = new SKPoint((p0.X + p1.X) / 2, (p0.Y + p1.Y) / 2);
                    var c2 = new SKPoint((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);

                    // Create a bezier curve
                    var curve = new TimedBezierCurve
                    {
                        StartPoint = c1,
                        Control1 = new SKPoint(p1.X, p1.Y),
                        Control2 = new SKPoint(p2.X, p2.Y),
                        EndPoint = c2,
                        Width = width,
                        Timestamp = p1.Timestamp,
                    };

                    _curves.Add(curve);

                    // Update path
                    _path.Reset();
                    _path.MoveTo(curve.StartPoint);
                    _path.CubicTo(curve.Control1, curve.Control2, curve.EndPoint);

                    // Remember last values
                    _lastVelocity = velocity;
                    _lastWidth = width;
                }

                _lastPoint = point;
                IsSigned = true;
            }
        }

        InvalidateSurface();
    }

    private void HandleTouchEnd()
    {
        _isDrawing = false;

        if (IsSigned)
        {
            Signed?.Invoke(this, EventArgs.Empty);
        }
    }

    private SignaturePoint CreateSignaturePoint(SKPoint location, long timestamp)
    {
        return new SignaturePoint { X = location.X, Y = location.Y, Timestamp = timestamp };
    }

    private float CalculateVelocity(SignaturePoint p1, SignaturePoint p2)
    {
        float timeDelta = Math.Max(1, p2.Timestamp - p1.Timestamp);
        float distance = (float)Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        float velocity = distance / timeDelta;

        if (_lastVelocity > 0)
        {
            // Apply velocity filter
            return (VelocityFilterWeight * velocity) + ((1 - VelocityFilterWeight) * _lastVelocity);
        }

        return velocity;
    }

    private float CalculateStrokeWidth(float velocity)
    {
        // Map velocity to stroke width
        // Slower velocity = thicker line
        float normalizedVelocity = Math.Clamp(velocity, 0, 10);
        float strokeWidth = Math.Max(MinWidth, MaxWidth - (normalizedVelocity * ((MaxWidth - MinWidth) / 10f)));

        if (_lastWidth > 0)
        {
            // Smooth transitions
            strokeWidth = (VelocityFilterWeight * strokeWidth) + ((1 - VelocityFilterWeight) * _lastWidth);
        }

        return strokeWidth;
    }

    private void DrawBezier(SKCanvas canvas, TimedBezierCurve curve, SKPaint paint)
    {
        paint.StrokeWidth = curve.Width;

        using var path = new SKPath();
        path.MoveTo(curve.StartPoint);
        path.CubicTo(curve.Control1, curve.Control2, curve.EndPoint);

        canvas.DrawPath(path, paint);
    }
}

/// <summary>
/// Represents a point in a signature with a timestamp.
/// </summary>
public class SignaturePoint
{
    public float X { get; set; }

    public float Y { get; set; }

    public long Timestamp { get; set; }
}

/// <summary>
/// Represents a Bézier curve with a specific width and timestamp.
/// </summary>
public class TimedBezierCurve
{
    public SKPoint StartPoint { get; set; }

    public SKPoint Control1 { get; set; }

    public SKPoint Control2 { get; set; }

    public SKPoint EndPoint { get; set; }

    public float Width { get; set; }

    public long Timestamp { get; set; }
}
