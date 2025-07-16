namespace AuroraControls.Gauges;

/// <summary>
/// Circular gauge.
/// </summary>
public class CircularGauge : AuroraViewBase
{
    /// <summary>
    /// The progress starting degree property.
    /// </summary>
    public static readonly BindableProperty StartingDegreeProperty =
        BindableProperty.Create(nameof(StartingDegree), typeof(double), typeof(CircularGauge), default(double),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the starting degree.
    /// </summary>
    /// <value>The starting degree as a double. Default value is default(double).</value>
    public double StartingDegree
    {
        get => (double)GetValue(StartingDegreeProperty);
        set => SetValue(StartingDegreeProperty, value.Clamp(-360, 360));
    }

    /// <summary>
    /// The progress ending degree property.
    /// </summary>
    public static readonly BindableProperty EndingDegreeProperty =
        BindableProperty.Create(nameof(EndingDegree), typeof(double), typeof(CircularGauge), default(double),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the ending degree.
    /// </summary>
    /// <value>The ending degree as a double. Default value is default(double).</value>
    public double EndingDegree
    {
        get => (double)GetValue(EndingDegreeProperty);
        set => SetValue(EndingDegreeProperty, value.Clamp(-360, 360));
    }

    /// <summary>
    /// The progress thickness property.
    /// </summary>
    public static readonly BindableProperty ProgressThicknessProperty =
        BindableProperty.Create(nameof(ProgressThickness), typeof(double), typeof(CircularGauge), 12d,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the progress thickness.
    /// </summary>
    /// <value>Takes a double. Default value is 12d.</value>
    public double ProgressThickness
    {
        get => (double)GetValue(ProgressThicknessProperty);
        set => SetValue(ProgressThicknessProperty, value);
    }

    /// <summary>
    /// The progress color property.
    /// </summary>
    public static readonly BindableProperty ProgressColorProperty =
        BindableProperty.Create(nameof(ProgressColor), typeof(Color), typeof(CircularGauge), Colors.White,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the color of the progress.
    /// </summary>
    /// <value>Takes a Xamarin.Forms.Color. Default value is Xamarin.Forms.Color.Default.</value>
    public Color ProgressColor
    {
        get => (Color)GetValue(ProgressColorProperty);
        set => SetValue(ProgressColorProperty, value);
    }

    /// <summary>
    /// The progress background color property.
    /// </summary>
    public static readonly BindableProperty ProgressBackgroundColorProperty =
        BindableProperty.Create(nameof(ProgressBackgroundColor), typeof(Color), typeof(CircularGauge), Colors.Gray,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the color of the progress background.
    /// </summary>
    /// <value>Takes a Xamarin.Forms.Color. Default value is Xamarin.Forms.Color.Default.</value>
    public Color ProgressBackgroundColor
    {
        get => (Color)GetValue(ProgressBackgroundColorProperty);
        set => SetValue(ProgressBackgroundColorProperty, value);
    }

    /// <summary>
    /// The end cap type property.
    /// </summary>
    public static readonly BindableProperty EndCapTypeProperty =
        BindableProperty.Create(nameof(EndCapType), typeof(EndCapType), typeof(CircularGauge), EndCapType.Rounded,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the type of the end cap for the progress portion.
    /// </summary>
    /// <value>Takes an EndCapType. Default is EndCapType.Rounded.</value>
    public EndCapType EndCapType
    {
        get => (EndCapType)GetValue(EndCapTypeProperty);
        set => SetValue(EndCapTypeProperty, value);
    }

    public CircularGauge() => MinimumHeightRequest = IAuroraView.StandardControlHeight;

    /// <summary>
    /// This is the method used to draw our control on the SKCanvas. This method is fired every time <c>this.InvalidateSurface();</c> is called, resulting in a "redrawing" of the control.
    /// </summary>
    /// <param name="surface">The skia surface to paint on the controls.</param>
    /// <param name="info">Information about the skia image.</param>
    protected override void PaintControl(SKSurface surface, SKImageInfo info)
    {
        var canvas = surface.Canvas;

        using var progressPaint = new SKPaint();
        using var progressPath = new SKPath();
        using var progressBackgroundPaint = new SKPaint();
        using var backgroundProgressPath = new SKPath();
        progressPaint.IsAntialias = true;

        switch (this.EndCapType)
        {
            case EndCapType.Square:
                progressPaint.StrokeCap = SKStrokeCap.Butt;
                break;
            case EndCapType.Rounded:
                progressPaint.StrokeCap = SKStrokeCap.Round;
                break;
        }

        float progressThickness = (float)this.ProgressThickness * this._scale;

        progressPaint.Style = SKPaintStyle.Stroke;
        progressPaint.Color = this.ProgressColor.ToSKColor();
        progressPaint.StrokeWidth = progressThickness;

        progressBackgroundPaint.IsAntialias = true;
        progressBackgroundPaint.Style = SKPaintStyle.Stroke;
        progressBackgroundPaint.Color = this.ProgressBackgroundColor.ToSKColor();
        progressBackgroundPaint.StrokeWidth = progressThickness;

        float size = Math.Min(info.Width, info.Height) - progressThickness;

        float left = (info.Width - size) / 2f;
        float top = (info.Height - size) / 2f;
        float right = left + size;
        float bottom = top + size;

        var arcRect = new SKRect(left, top, right, bottom);

        backgroundProgressPath.AddArc(arcRect, 0, 360);
        progressPath.AddArc(arcRect, (float)this.StartingDegree, (float)this.EndingDegree);

        canvas.Clear();
        canvas.DrawPath(backgroundProgressPath, progressBackgroundPaint);

        if (this.StartingDegree != this.EndingDegree)
        {
            canvas.DrawPath(progressPath, progressPaint);
        }
    }

    /// <summary>
    /// Transition animation from the starting to the ending degree.
    /// </summary>
    /// <returns>A Task boolean when the Task is complete.</returns>
    /// <param name="startingDegree">Starting degree from which to begin the transition animation.</param>
    /// <param name="endingDegree">Ending degree to which the transition will complete.</param>
    /// <param name="rate">The time, in milliseconds, between frames.</param>
    /// <param name="length">The number of milliseconds over which to interpolate the animation.</param>
    /// <param name="easing">The easing function to use to transision in, out, or in and out of the animation.</param>
    public async Task<bool> TransitionTo(double? startingDegree = null, double? endingDegree = null, uint rate = 16, uint length = 250, Easing easing = null)
    {
        if (!startingDegree.HasValue && !endingDegree.HasValue)
        {
            return false;
        }

        var transitions = new List<Task<bool>>();

        if (startingDegree.HasValue)
        {
            transitions.Add(this.TransitionTo(c => c.StartingDegree, startingDegree.Value, rate, length, easing));
        }

        if (endingDegree.HasValue)
        {
            transitions.Add(this.TransitionTo(c => c.EndingDegree, endingDegree.Value, rate, length, easing));
        }

        await Task.WhenAll(transitions);

        return transitions.TrueForAll(x => !x.IsCanceled && !x.IsFaulted && x.IsCompleted && x.Result);
    }
}
