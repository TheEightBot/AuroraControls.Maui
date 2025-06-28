namespace AuroraControls;

/// <summary>
/// Step indicator for displaying progress through a multi-step workflow.
/// </summary>
public class StepIndicator : AuroraViewBase
{
    private readonly List<SKPath> _stepPaths = new List<SKPath>();

    /// <summary>
    /// The number of steps to display in workflow.
    /// </summary>
    public static BindableProperty NumberOfStepsProperty =
        BindableProperty.Create(nameof(NumberOfSteps), typeof(int), typeof(StepIndicator), 0,
            propertyChanged: (bindable, _, newValue) =>
            {
                var newCount = (int)newValue;
                var stepIndicator = bindable as StepIndicator;

                if (stepIndicator == null)
                {
                    return;
                }

                for (int i = stepIndicator._stepPaths.Count - 1; i >= 0; i--)
                {
                    var stepPath = stepIndicator._stepPaths[i];
                    stepIndicator._stepPaths.RemoveAt(i);
                    stepPath.Dispose();
                }

                stepIndicator._stepPaths
                    .AddRange(
                        Enumerable
                            .Range(0, newCount)
                            .Select(_ => new SKPath())
                            .ToList());

                stepIndicator?.InvalidateSurface();
            });

    /// <summary>
    /// Gets or sets the number of steps.
    /// </summary>
    /// <value>Takes an int value. default value is 0.</value>
    public int NumberOfSteps
    {
        get { return (int)GetValue(NumberOfStepsProperty); }
        set { SetValue(NumberOfStepsProperty, value); }
    }

    /// <summary>
    /// The current step in the workflow.
    /// </summary>
    public static BindableProperty CurrentStepProperty =
        BindableProperty.Create(nameof(CurrentStep), typeof(int), typeof(StepIndicator), 0,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the current step.
    /// </summary>
    /// <value>Takes an int. Default value is default(int).</value>
    public int CurrentStep
    {
        get { return (int)GetValue(CurrentStepProperty); }
        set { SetValue(CurrentStepProperty, value); }
    }

    public static BindableProperty DrawConnectingLineProperty =
        BindableProperty.Create(nameof(DrawConnectingLine), typeof(bool), typeof(StepIndicator), true,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    public bool DrawConnectingLine
    {
        get => (bool)GetValue(DrawConnectingLineProperty);
        set => SetValue(DrawConnectingLineProperty, value);
    }

    /// <summary>
    /// The display step number property. Specifies whether the step number should be displayed.
    /// </summary>
    public static BindableProperty DisplayStepNumberProperty =
        BindableProperty.Create(nameof(DisplayStepNumber), typeof(bool), typeof(StepIndicator), true,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="T:AuroraControls.StepIndicator"/> display step number.
    /// </summary>
    /// <value><c>true</c> if display step number; otherwise, <c>false</c>.</value>
    public bool DisplayStepNumber
    {
        get { return (bool)GetValue(DisplayStepNumberProperty); }
        set { SetValue(DisplayStepNumberProperty, value); }
    }

    /// <summary>
    /// Sets the line color property.
    /// </summary>
    public static BindableProperty LineColorProperty =
        BindableProperty.Create(nameof(LineColor), typeof(Color), typeof(StepIndicator),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the color of the line.
    /// </summary>
    /// <value>Takes a Xamarin.Forms.Color. Default value is default(Xamarin.Forms.Color).</value>
    public Color LineColor
    {
        get { return (Color)GetValue(LineColorProperty); }
        set { SetValue(LineColorProperty, value); }
    }

    /// <summary>
    /// Defines a line width property for the step indicator.
    /// </summary>
    public static BindableProperty LineWidthProperty =
        BindableProperty.Create(nameof(LineWidth), typeof(double), typeof(StepIndicator), 0d,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the width of the line.
    /// </summary>
    /// <value>Takes a double. Default value is default(double).</value>
    public double LineWidth
    {
        get { return (double)GetValue(LineWidthProperty); }
        set { SetValue(LineWidthProperty, value); }
    }

    /// <summary>
    /// The highlight color property refers to the color of the highlighted step.
    /// </summary>
    public static BindableProperty HighlightColorProperty =
        BindableProperty.Create(nameof(HighlightColor), typeof(Color), typeof(StepIndicator),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the color of the highlight.
    /// </summary>
    /// <value>Takes a Xamarin.Forms.Color. Default value is default(Xamarin.Forms.Color).</value>
    public Color HighlightColor
    {
        get { return (Color)GetValue(HighlightColorProperty); }
        set { SetValue(HighlightColorProperty, value); }
    }

    /// <summary>
    /// The color for the inactive color property.
    /// </summary>
    public static BindableProperty InactiveColorProperty =
        BindableProperty.Create(nameof(InactiveColor), typeof(Color), typeof(StepIndicator),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the color of the inactive.
    /// </summary>
    /// <value>Takes a Xamarin.Forms.Color. Default value is default(Xamarin.Forms.Color).</value>
    public Color InactiveColor
    {
        get { return (Color)GetValue(InactiveColorProperty); }
        set { SetValue(InactiveColorProperty, value); }
    }

    /// <summary>
    /// The font color property.
    /// </summary>
    public static BindableProperty FontColorProperty =
        BindableProperty.Create(nameof(FontColor), typeof(Color), typeof(StepIndicator),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the color of the font.
    /// </summary>
    /// <value>Takes a Xamarin.Forms.Color. Default value is default(Xamarin.Forms.Color).</value>
    public Color FontColor
    {
        get { return (Color)GetValue(FontColorProperty); }
        set { SetValue(FontColorProperty, value); }
    }

    public static BindableProperty TypefaceProperty =
        BindableProperty.Create(nameof(Typeface), typeof(SKTypeface), typeof(StepIndicator),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    public SKTypeface? Typeface
    {
        get => (SKTypeface?)GetValue(TypefaceProperty);
        set => SetValue(TypefaceProperty, value);
    }

    /// <summary>
    /// The padding around the control.
    /// </summary>
    public static BindableProperty PaddingProperty =
        BindableProperty.Create(nameof(Padding), typeof(Thickness), typeof(StepIndicator), default(Thickness),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the padding.
    /// </summary>
    /// <value>Takes a Xamarin.Forms.Thickness. Default value is default(Xamarin.Forms.Thickness).</value>
    public Thickness Padding
    {
        get { return (Thickness)GetValue(PaddingProperty); }
        set { SetValue(PaddingProperty, value); }
    }

    /// <summary>
    /// The Switch when tapping a step property.
    /// </summary>
    public static BindableProperty SwitchOnStepTapProperty =
        BindableProperty.Create(nameof(SwitchOnStepTap), typeof(bool), typeof(StepIndicator), false,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets a value indicating whether gets or sets a value determing whether to switch screens when user taps a step..
    /// </summary>
    /// <value><c>true</c> if enabled, otherwise <c>false</c>.</value>
    public bool SwitchOnStepTap
    {
        get => (bool)GetValue(SwitchOnStepTapProperty);
        set => SetValue(SwitchOnStepTapProperty, value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StepIndicator"/> class.
    /// </summary>
    public StepIndicator()
    {
    }

    protected override void Attached()
    {
        this.EnableTouchEvents = true;
        base.Attached();
    }

    /// <summary>
    /// Method that is called when the property that is specified by propertyName is changed.
    /// The surface is automatically invalidated/redrawn whenever <c>HeightProperty</c>, <c>WidthProperty</c> or <c>MarginProperty</c> gets updated.
    /// </summary>
    /// <param name="propertyName">The name of the bound property that changed.</param>
    protected override void OnPropertyChanged(string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName.Equals(VisualElement.HeightProperty.PropertyName) ||
            propertyName.Equals(VisualElement.WidthProperty.PropertyName) ||
            propertyName.Equals(View.MarginProperty.PropertyName))
        {
            this.InvalidateSurface();
        }
    }

    /// <summary>
    /// This is the method used to draw our control on the SKCanvas. This method is fired every time <c>this.InvalidateSurface();</c> is called, resulting in a "redrawing" of the control.
    /// </summary>
    /// <param name="surface">The skia surface to paint on the controls.</param>
    /// <param name="info">Information about the skia image.</param>
    protected override void PaintControl(SKSurface surface, SKImageInfo info)
    {
        var canvas = surface.Canvas;

        var drawConnectingLine = this.DrawConnectingLine;

        using (var paint = new SKPaint())
        using (var fontPaint = new SKPaint())
        using (var path = new SKPath())
        {
            var progressCircleSize = (float)((info.Height - this.Padding.VerticalThickness) / 4f) * .8f;

            // Calculate stroke widths that scale with circle sizes
            var baseStrokeWidth = (float)this.LineWidth;
            var progressStrokeWidth = baseStrokeWidth * 1.5f; // Larger stroke for current step
            var previousStrokeWidth = baseStrokeWidth * 1.2f; // Medium stroke for completed steps

            // Calculate the actual circle sizes (radius to center of stroke)
            var nextStepCircleSize = progressCircleSize * .5f;
            var previousStepCircleSize = progressCircleSize * .8f;

            // Calculate the maximum outer radius (circle radius + half stroke width)
            var progressMaxRadius = progressCircleSize + (progressStrokeWidth / 2f);
            var previousMaxRadius = previousStepCircleSize + (previousStrokeWidth / 2f);
            var nextStepMaxRadius = nextStepCircleSize + (baseStrokeWidth / 2f);

            // Use the largest possible radius for padding calculations
            var maxRadius = Math.Max(Math.Max(progressMaxRadius, previousMaxRadius), nextStepMaxRadius);

            paint.IsAntialias = true;
            paint.StrokeCap = SKStrokeCap.Round;
            paint.Style = SKPaintStyle.Stroke;
            paint.Color = this.LineColor.ToSKColor();
            paint.StrokeWidth = baseStrokeWidth;

            fontPaint.IsAntialias = true;
            fontPaint.Color = this.FontColor.ToSKColor();
            fontPaint.TextSize = progressCircleSize * 0.6f; // Scale font size with circle
            fontPaint.Typeface = this.Typeface ?? PlatformInfo.DefaultTypeface;
            fontPaint.TextAlign = SKTextAlign.Center;
            fontPaint.TextEncoding = SKTextEncoding.Utf8;

            // Use the maximum radius to ensure proper padding
            var start = maxRadius + (float)this.Padding.Left;
            var end = info.Width - maxRadius - (float)this.Padding.Right;

            var verticalCenter = (float)((info.Height / 2f) + (this.Padding.Top * .5f) - (this.Padding.Bottom * .5f));

            canvas.Clear();

            // Only draw connecting line if we have enough space and it's enabled
            if (this.DrawConnectingLine && start < end)
            {
                path.MoveTo(start, verticalCenter);
                path.LineTo(end, verticalCenter);
                canvas.DrawPath(path, paint);
            }

            if (this.NumberOfSteps > 0 && start < end)
            {
                var stepSize = this.NumberOfSteps > 1 ? (end - start) / (float)(this.NumberOfSteps - 1) : 0f;

                for (int i = 0; i < this.NumberOfSteps; i++)
                {
                    var circlePath = this._stepPaths.ElementAtOrDefault(i);

                    if (circlePath == null)
                    {
                        continue;
                    }

                    circlePath.Reset();

                    var centerX = start + (stepSize * i);

                    paint.Style = SKPaintStyle.StrokeAndFill;
                    paint.BlendMode = SKBlendMode.Src;
                    paint.Color = this.LineColor.ToSKColor();

                    // Future steps (not yet reached)
                    if (i > this.CurrentStep - 1)
                    {
                        paint.StrokeWidth = baseStrokeWidth;
                        paint.Color = this.InactiveColor.ToSKColor();
                        circlePath.AddCircle(centerX, verticalCenter, nextStepCircleSize);
                        canvas.DrawPath(circlePath, paint);
                        continue;
                    }

                    using (var strokePath = new SKPath())
                    {
                        // Completed steps (before current)
                        if (i < this.CurrentStep - 1)
                        {
                            paint.StrokeWidth = previousStrokeWidth;
                            paint.Color = this.LineColor.ToSKColor();
                            strokePath.AddCircle(centerX, verticalCenter, previousStepCircleSize);
                            canvas.DrawPath(strokePath, paint);

                            paint.Color = this.InactiveColor.ToSKColor();
                            paint.BlendMode = SKBlendMode.SrcOver;
                            paint.Style = SKPaintStyle.Fill;
                            circlePath.AddCircle(centerX, verticalCenter, previousStepCircleSize - (previousStrokeWidth / 2f));
                            canvas.DrawPath(circlePath, paint);
                            paint.Style = SKPaintStyle.StrokeAndFill;
                        }

                        // Current step
                        else if (i == this.CurrentStep - 1)
                        {
                            paint.StrokeWidth = progressStrokeWidth;
                            paint.Color = this.LineColor.ToSKColor();
                            strokePath.AddCircle(centerX, verticalCenter, progressCircleSize);
                            canvas.DrawPath(strokePath, paint);

                            paint.Color = this.HighlightColor.ToSKColor();
                            paint.BlendMode = SKBlendMode.SrcOver;
                            paint.Style = SKPaintStyle.Fill;
                            circlePath.AddCircle(centerX, verticalCenter, progressCircleSize - (progressStrokeWidth / 2f));
                            canvas.DrawPath(circlePath, paint);
                            paint.Style = SKPaintStyle.StrokeAndFill;
                        }
                    }

                    // Draw step numbers if enabled
                    if (this.DisplayStepNumber)
                    {
                        var currentStepText = (i + 1).ToString();
                        var textRect = default(SKRect);
                        fontPaint.EnsureHasValidFont(currentStepText);
                        fontPaint.MeasureText(currentStepText, ref textRect);

                        canvas.DrawText(currentStepText, centerX, verticalCenter - textRect.MidY, fontPaint);
                    }
                }
            }
        }
    }

    /// <summary>
    /// SKCanvas method that fires on touch.
    /// </summary>
    /// <param name="e">Provides data for the SKCanvasView.Touch or SKGLView.Touch event.</param>
    protected override void OnTouch(SKTouchEventArgs e)
    {
        base.OnTouch(e);

        if (this.SwitchOnStepTap)
        {
            for (int index = 0; index < this._stepPaths.Count; index++)
            {
                var stepPath = this._stepPaths[index];
                if (e.InContact == true && stepPath.Contains(e.Location.X, e.Location.Y))
                {
                    System.Diagnostics.Debug.WriteLine($"Tapped {index} : {e}");
                    this.CurrentStep = index + 1;
                    break;
                }
            }
        }

        e.Handled = true;
    }
}
