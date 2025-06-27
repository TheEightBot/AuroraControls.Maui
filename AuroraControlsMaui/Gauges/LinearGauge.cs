using Color = Microsoft.Maui.Graphics.Color;

namespace AuroraControls.Gauges;

/// <summary>
/// Linear gauge orientation options.
/// </summary>
public enum LinearGaugeOrientation
{
    Horizontal,
    ReverseHorizontal,
    Vertical,
    ReverseVertical,
}

/// <summary>
/// Linear gauge.
/// </summary>
public class LinearGauge : AuroraViewBase
{
    /// <summary>
    /// The starting percent property.
    /// </summary>
    public static readonly BindableProperty StartingPercentProperty =
        BindableProperty.Create(nameof(StartingPercent), typeof(double), typeof(LinearGauge), default(double),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the starting percent.
    /// </summary>
    /// <value>Percent as a double. Default is default(double).</value>
    public double StartingPercent
    {
        get => (double)GetValue(StartingPercentProperty);
        set => SetValue(StartingPercentProperty, value.Clamp(0, 100));
    }

    /// <summary>
    /// The ending percent property.
    /// </summary>
    public static readonly BindableProperty EndingPercentProperty =
        BindableProperty.Create(nameof(EndingPercent), typeof(double), typeof(LinearGauge), default(double),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the ending percent.
    /// </summary>
    /// <value>Percent as a double. Default is default(double).</value>
    public double EndingPercent
    {
        get => (double)GetValue(EndingPercentProperty);
        set => SetValue(EndingPercentProperty, value.Clamp(0, 100));
    }

    /// <summary>
    /// The progress thickness property.
    /// </summary>
    public static readonly BindableProperty ProgressThicknessProperty =
        BindableProperty.Create(nameof(ProgressThickness), typeof(double), typeof(LinearGauge), 12d,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the progress thickness.
    /// </summary>
    /// <value>Thickness as a double. Default is 12d.</value>
    public double ProgressThickness
    {
        get => (double)GetValue(ProgressThicknessProperty);
        set => SetValue(ProgressThicknessProperty, value);
    }

    /// <summary>
    /// The progress color property.
    /// </summary>
    public static readonly BindableProperty ProgressColorProperty =
        BindableProperty.Create(nameof(ProgressColor), typeof(Color), typeof(LinearGauge), Colors.White,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the color of the progress.
    /// </summary>
    /// <value>Expects a Xamarin.Forms.Color. Default is Xamarin.Forms.Color.Default.</value>
    public Color ProgressColor
    {
        get => (Color)GetValue(ProgressColorProperty);
        set => SetValue(ProgressColorProperty, value);
    }

    /// <summary>
    /// The orientation property.
    /// </summary>
    public static readonly BindableProperty OrientationProperty =
        BindableProperty.Create(nameof(Orientation), typeof(LinearGaugeOrientation), typeof(LinearGauge), LinearGaugeOrientation.Horizontal,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the orientation.
    /// </summary>
    /// <value>Takes a LinearGaugeOrientation. Default is LinearGaugeOrientation.Horizontal.</value>
    public LinearGaugeOrientation Orientation
    {
        get => (LinearGaugeOrientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    /// <summary>
    /// The end cap type property.
    /// </summary>
    public static readonly BindableProperty EndCapTypeProperty =
        BindableProperty.Create(nameof(EndCapType), typeof(EndCapType), typeof(LinearGauge), EndCapType.Rounded,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the end type of the cap.
    /// </summary>
    /// <value>Takes an EndCapType. Default is EndCapType.Rounded.</value>
    public EndCapType EndCapType
    {
        get => (EndCapType)GetValue(EndCapTypeProperty);
        set => SetValue(EndCapTypeProperty, value);
    }

    /// <summary>
    /// The progress background color property.
    /// </summary>
    public static readonly BindableProperty ProgressBackgroundColorProperty =
        BindableProperty.Create(nameof(ProgressBackgroundColor), typeof(Color), typeof(LinearGauge), Colors.Gray,
                                propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the color of the progress background.
    /// </summary>
    /// <value>Expects a Xamarin.Forms.Color. Default is Xamarin.Forms.Color.Default.</value>
    public Color ProgressBackgroundColor
    {
        get => (Color)GetValue(ProgressBackgroundColorProperty);
        set => SetValue(ProgressBackgroundColorProperty, value);
    }

    public LinearGauge() => MinimumHeightRequest = 22;

    /// <summary>
    /// This is the method used to draw our control on the SKCanvas. This method is fired every time <c>this.InvalidateSurface();</c> is called, resulting in a "redrawing" of the control.
    /// </summary>
    /// <param name="surface">The skia surface to paint on the controls.</param>
    /// <param name="info">Information about the skia image.</param>
    protected override void PaintControl(SKSurface surface, SKImageInfo info)
    {
        var canvas = surface.Canvas;

        using (var progressPaint = new SKPaint())
        using (var progressPath = new SKPath())
        using (var progressBackgroundPaint = new SKPaint())
        using (var backgroundProgressPath = new SKPath())
        {
            SKStrokeCap endCapType = SKStrokeCap.Round;

            switch (EndCapType)
            {
                case EndCapType.Rounded:
                    endCapType = SKStrokeCap.Round;
                    break;
                case EndCapType.Square:
                    endCapType = SKStrokeCap.Square;
                    break;
            }

            float scaledProgressThickness = _scale * (float)ProgressThickness;

            progressPaint.IsAntialias = true;
            progressPaint.StrokeCap = endCapType;
            progressPaint.Style = SKPaintStyle.Stroke;
            progressPaint.Color = ProgressColor.ToSKColor();
            progressPaint.StrokeWidth = scaledProgressThickness;

            progressBackgroundPaint.IsAntialias = true;
            progressBackgroundPaint.StrokeCap = endCapType;
            progressBackgroundPaint.Style = SKPaintStyle.Stroke;
            progressBackgroundPaint.Color = ProgressBackgroundColor.ToSKColor();
            progressBackgroundPaint.StrokeWidth = scaledProgressThickness;

            var orientation = this.Orientation;

            switch (orientation)
            {
                case LinearGaugeOrientation.Horizontal:
                case LinearGaugeOrientation.ReverseHorizontal:
                    double startHorizontal = ((info.Width * (this.StartingPercent / 100)) + scaledProgressThickness).Clamp(scaledProgressThickness, info.Width - scaledProgressThickness);
                    double endHorizontal = ((info.Width * (this.EndingPercent / 100)) - scaledProgressThickness).Clamp(scaledProgressThickness, info.Width - scaledProgressThickness);

                    float startBackgroundHorizontal = scaledProgressThickness.Clamp(scaledProgressThickness, info.Width - scaledProgressThickness);
                    float endBackgroundHorizontal = (info.Width - scaledProgressThickness).Clamp(scaledProgressThickness, info.Width - scaledProgressThickness);

                    float centerVertical = info.Height / 2f;

                    progressPath.MoveTo((float)startHorizontal, centerVertical);
                    progressPath.LineTo((float)endHorizontal, centerVertical);

                    backgroundProgressPath.MoveTo(startBackgroundHorizontal, centerVertical);
                    backgroundProgressPath.LineTo(endBackgroundHorizontal, centerVertical);
                    break;

                case LinearGaugeOrientation.Vertical:
                case LinearGaugeOrientation.ReverseVertical:
                    double startVertical = ((info.Height * (this.StartingPercent / 100)) + scaledProgressThickness).Clamp(scaledProgressThickness, info.Height - scaledProgressThickness);
                    double endVertical = ((info.Height * (this.EndingPercent / 100)) - scaledProgressThickness).Clamp(scaledProgressThickness, info.Height - scaledProgressThickness);

                    float startBackgroundVertical = scaledProgressThickness.Clamp(scaledProgressThickness, info.Height - scaledProgressThickness);
                    float endBackgroundVertical = (info.Height - scaledProgressThickness).Clamp(scaledProgressThickness, info.Height - scaledProgressThickness);

                    float centerHorizontal = info.Width / 2f;

                    progressPath.MoveTo(centerHorizontal, (float)startVertical);
                    progressPath.LineTo(centerHorizontal, (float)endVertical);

                    backgroundProgressPath.MoveTo(centerHorizontal, startBackgroundVertical);
                    backgroundProgressPath.LineTo(centerHorizontal, endBackgroundVertical);
                    break;
            }

            canvas.Clear();
            canvas.DrawPath(backgroundProgressPath, progressBackgroundPaint);

            if (StartingPercent != EndingPercent)
            {
                using (new SKAutoCanvasRestore(canvas, true))
                {
                    if (orientation == LinearGaugeOrientation.ReverseHorizontal)
                    {
                        canvas.Scale(-1, 1, info.Width * .5f, 0f);
                    }
                    else if (orientation == LinearGaugeOrientation.ReverseVertical)
                    {
                        canvas.Scale(1, -1, 0f, info.Height * .5f);
                    }

                    canvas.DrawPath(progressPath, progressPaint);
                }
            }
        }
    }

    /// <summary>
    /// Transition animation from the starting to the ending percentage.
    /// </summary>
    /// <returns>A Task boolean when the Task is complete.</returns>
    /// <param name="startingPercentage">Starting percentage from which to begin the transition animation.</param>
    /// <param name="endingPercentage">Ending percentage to which the transition will complete.</param>
    /// <param name="rate">The time, in milliseconds, between frames.</param>
    /// <param name="length">The number of milliseconds over which to interpolate the animation.</param>
    /// <param name="easing">The easing function to use to transision in, out, or in and out of the animation.</param>
    public async Task<bool> TransitionTo(double? startingPercentage = null, double? endingPercentage = null, uint rate = 16, uint length = 250, Easing easing = null)
    {
        if (!startingPercentage.HasValue && !endingPercentage.HasValue)
        {
            return false;
        }

        var transitions = new List<Task<bool>>();

        if (startingPercentage.HasValue)
        {
            transitions.Add(this.TransitionTo(c => c.StartingPercent, startingPercentage.Value, rate, length, easing));
        }

        if (endingPercentage.HasValue)
        {
            transitions.Add(this.TransitionTo(c => c.EndingPercent, endingPercentage.Value, rate, length, easing));
        }

        await Task.WhenAll(transitions);

        return transitions.TrueForAll(x => !x.IsCanceled && !x.IsFaulted && x.IsCompleted && x.Result);
    }
}
