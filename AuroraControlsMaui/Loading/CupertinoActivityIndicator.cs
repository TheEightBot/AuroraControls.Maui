using System.ComponentModel;

namespace AuroraControls.Loading;

/// <summary>
/// CupertinoActivityIndicator loading animation.
/// </summary>
#pragma warning disable CA1001 // Types that own disposable fields should be disposable
public class CupertinoActivityIndicator : SceneViewBase
#pragma warning restore CA1001 // Types that own disposable fields should be disposable
{
    private SKPicture _loadingIndicator;

    private bool _needsRefresh;

    private SKPaint _foregroundPaint;

    private SKColor _foregroundColor = ((Color)IndicatorColorProperty.DefaultValue).ToSKColor();

    /// <summary>
    /// The number of segments to display.
    /// </summary>
    public static BindableProperty SegmentsProperty =
        BindableProperty.Create(nameof(Segments), typeof(uint), typeof(CupertinoActivityIndicator), 16u,
            propertyChanged: static (bindable, _, _) =>
                (bindable as CupertinoActivityIndicator)._needsRefresh = true);

    /// <summary>
    /// Gets or sets the number of segments in the indicator.
    /// </summary>
    /// <value>The number of segments as an uint. Default is 16u.</value>
    [TypeConverter(typeof(UIntTypeConverter))]
    public uint Segments
    {
        get => (uint)GetValue(SegmentsProperty);
        set => SetValue(SegmentsProperty, value);
    }

    /// <summary>
    /// The length of the segments.
    /// </summary>
    public static BindableProperty LengthPercentProperty =
        BindableProperty.Create(nameof(LengthPercent), typeof(double), typeof(CupertinoActivityIndicator), .2d,
            propertyChanged: static (bindable, _, _) =>
                (bindable as CupertinoActivityIndicator)._needsRefresh = true);

    /// <summary>
    /// Gets or sets the length percentage.
    /// </summary>
    /// <value>Percent as a double. Default is .2d.</value>
    public double LengthPercent
    {
        get => (double)GetValue(LengthPercentProperty);
        set => SetValue(LengthPercentProperty, value);
    }

    /// <summary>
    /// The width of the segments.
    /// </summary>
    public static BindableProperty WidthPercentProperty =
        BindableProperty.Create(nameof(WidthPercent), typeof(double), typeof(CupertinoActivityIndicator), .25d,
            propertyChanged: static (bindable, _, _) =>
                (bindable as CupertinoActivityIndicator)._needsRefresh = true);

    /// <summary>
    /// Gets or sets the width percentage.
    /// </summary>
    /// <value>Percent as a double. Default is .25d.</value>
    public double WidthPercent
    {
        get => (double)GetValue(WidthPercentProperty);
        set => SetValue(WidthPercentProperty, value);
    }

    /// <summary>
    /// The indicator color.
    /// </summary>
    public static BindableProperty IndicatorColorProperty =
        BindableProperty.Create(nameof(IndicatorColor), typeof(Color), typeof(CupertinoActivityIndicator), Color.FromArgb("#C7C7C9"),
            propertyChanged: static (bindable, _, newValue) =>
            {
                var cai = bindable as CupertinoActivityIndicator;
                cai._foregroundColor = ((Color)newValue).ToSKColor();
                cai._needsRefresh = true;
            });

    /// <summary>
    /// Gets or sets the color of the indicator.
    /// </summary>
    /// <value>Expects a Xamarin.Forms.Color. Default is Color.DarkGray.</value>
    public Color IndicatorColor
    {
        get => (Color)GetValue(IndicatorColorProperty);
        set => SetValue(IndicatorColorProperty, value);
    }

    /// <summary>
    /// The corner radius.
    /// </summary>
    public static BindableProperty CornerRadiusProperty =
        BindableProperty.Create(nameof(CornerRadius), typeof(double), typeof(CupertinoActivityIndicator), 4d,
            propertyChanged: static (bindable, _, _) =>
                (bindable as CupertinoActivityIndicator)._needsRefresh = true);

    /// <summary>
    /// Gets or sets the corner radius.
    /// </summary>
    /// <value>Radius as a double. Default is 4d.</value>
    public double CornerRadius
    {
        get => (double)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public CupertinoActivityIndicator()
    {
        MinimumHeightRequest = IAuroraView.StandardControlHeight;
    }

    protected override void Attached()
    {
        _foregroundPaint =
            new SKPaint
            {
                Style = SKPaintStyle.Fill,
            };

        base.Attached();
    }

    protected override void Detached()
    {
        base.Detached();

        _foregroundPaint?.Dispose();
        _foregroundPaint = null;

        _loadingIndicator?.Dispose();
        _loadingIndicator = null;
    }

    protected override SKImage PaintScene(SKSurface surface, SKImageInfo info, double percentage)
    {
        var canvas = surface.Canvas;

        var startingRotation = 360f * (float)percentage;

        var midX = info.Rect.MidX;
        var midY = info.Rect.MidY;

        if (_needsRefresh || _loadingIndicator == null)
        {
            using (var recorder = new SKPictureRecorder())
            using (var recordingCanvas = recorder.BeginRecording(info.Rect))
            {
                var minLength = Math.Min(info.Width, info.Height) * .5f;
                var yStart = info.Rect.MidY - minLength;
                var length = minLength * (float)LengthPercent;

                var width = length * (float)WidthPercent;

                var halfCanvasWidth = info.Width * .5f;

                var drawRect = new SKRect(halfCanvasWidth - (width * .5f), yStart, halfCanvasWidth + (width * .5f), yStart + length);

                var segments = (float)Segments;

                var cornerRadius = (float)CornerRadius * _scale;

                var rotationAmount = 360f / segments;
                var alphaAmount = 1f / segments;

                recordingCanvas.Clear();

                for (int i = 0; i < segments; i++)
                {
                    _foregroundPaint.Color = _foregroundColor.WithAlpha(alphaAmount * (i + .1f));
                    recordingCanvas.RotateDegrees(rotationAmount, midX, midY);
                    recordingCanvas.DrawRoundRect(drawRect, cornerRadius, cornerRadius, _foregroundPaint);
                }

                _loadingIndicator = recorder.EndRecording();
            }

            _needsRefresh = false;
        }

        using (new SKAutoCanvasRestore(canvas))
        {
            canvas.Clear();
            canvas.RotateDegrees(startingRotation, midX, midY);
            canvas.DrawPicture(_loadingIndicator);
        }

        canvas.Flush();
        return surface.Snapshot();
    }
}