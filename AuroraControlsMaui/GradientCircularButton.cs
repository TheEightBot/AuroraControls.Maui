namespace AuroraControls;

/// <summary>
/// Gradient circular button.
/// </summary>
#pragma warning disable CA1001
public class GradientCircularButton : AuroraViewBase
#pragma warning restore CA1001
{
    private readonly SKPath _backgroundPath = new();

    private SKPoint _lastTouchLocation;
    private double _rippleAnimationPercentage;
    private bool _tapped;

    public event EventHandler Clicked;

    /// <summary>
    /// The button background color property.
    /// </summary>
    public static readonly BindableProperty ButtonBackgroundColorProperty =
        BindableProperty.Create(nameof(ButtonBackgroundColor), typeof(Color), typeof(GradientCircularButton), Colors.Transparent,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the color of the button background.
    /// </summary>
    /// <value>Expects a Color. Default color is Color.Default.</value>
    public Color ButtonBackgroundColor
    {
        get => (Color)GetValue(ButtonBackgroundColorProperty);
        set => SetValue(ButtonBackgroundColorProperty, value);
    }

    /// <summary>
    /// Specifies the border width property..
    /// </summary>
    public static readonly BindableProperty BorderWidthProperty =
        BindableProperty.Create(nameof(BorderWidth), typeof(double), typeof(GradientCircularButton), 0d,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the width of the border.
    /// </summary>
    /// <value>A double that defines the width of the border. Default value is default(double) or 0d.</value>
    public double BorderWidth
    {
        get => (double)GetValue(BorderWidthProperty);
        set => SetValue(BorderWidthProperty, value);
    }

    /// <summary>
    /// The border color property.
    /// </summary>
    public static readonly BindableProperty BorderColorProperty =
        BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(GradientCircularButton), Colors.White,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the color of the border.
    /// </summary>
    /// <value>Expects a Color. Default is Color.White.</value>
    public Color BorderColor
    {
        get => (Color)GetValue(BorderColorProperty);
        set => SetValue(BorderColorProperty, value);
    }

    /// <summary>
    /// The shadow color property.
    /// </summary>
    public static readonly BindableProperty ShadowColorProperty =
        BindableProperty.Create(nameof(ShadowColor), typeof(Color), typeof(GradientCircularButton), Color.FromRgba(0d, 0d, 0d, .33d),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the color of the shadow.
    /// </summary>
    /// <value>Expects a Color. Default value is Color.FromRgba(0d, 0d, 0d, .33d).</value>
    public Color ShadowColor
    {
        get => (Color)GetValue(ShadowColorProperty);
        set => SetValue(ShadowColorProperty, value);
    }

    /// <summary>
    /// The shadow location property.
    /// </summary>
    public static readonly BindableProperty ShadowLocationProperty =
        BindableProperty.Create(nameof(ShadowLocation), typeof(Point), typeof(GradientCircularButton), default(Point),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the shadow location.
    /// </summary>
    /// <value>The shadow location as a Point. Default value is default(Point).</value>
    public Point ShadowLocation
    {
        get => (Point)GetValue(ShadowLocationProperty);
        set => SetValue(ShadowLocationProperty, value);
    }

    /// <summary>
    /// The gradient angle property.
    /// </summary>
    public static readonly BindableProperty GradientAngleProperty =
        BindableProperty.Create(nameof(GradientAngle), typeof(double), typeof(GradientCircularButton), 90.0,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the gradient angle in degrees.
    /// </summary>
    /// <value>A double representing the gradient angle in degrees. Default value is 90.0 (vertical gradient from top to bottom).</value>
    public double GradientAngle
    {
        get => (double)GetValue(GradientAngleProperty);
        set => SetValue(GradientAngleProperty, value);
    }

    /// <summary>
    /// The ripples property.
    /// </summary>
    public static readonly BindableProperty RipplesProperty =
        BindableProperty.Create(nameof(Ripples), typeof(bool), typeof(GradientCircularButton), true,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="T:Aurora.Controls.GradientCircularButton"/> is ripples.
    /// </summary>
    /// <value><c>true</c> if ripples; otherwise, <c>false</c>.</value>
    public bool Ripples
    {
        get => (bool)GetValue(RipplesProperty);
        set => SetValue(RipplesProperty, value);
    }

    /// <summary>
    /// The text property.
    /// </summary>
    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(GradientCircularButton),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the text for the button.
    /// </summary>
    /// <value>string value for text. Default is default(string).</value>
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>
    /// The font color property.
    /// </summary>
    public static readonly BindableProperty FontColorProperty =
        BindableProperty.Create(nameof(FontColor), typeof(Color), typeof(GradientCircularButton), Colors.White,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    public static readonly BindableProperty FontSizeProperty =
        BindableProperty.Create(nameof(FontSize), typeof(double), typeof(GradientCircularButton), PlatformInfo.DefaultButtonFontSize,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the color of the font.
    /// </summary>
    /// <value>Expects a Color. Default value is Color.White.</value>
    public Color FontColor
    {
        get => (Color)GetValue(FontColorProperty);
        set => SetValue(FontColorProperty, value);
    }

    /// <summary>
    /// The typeface property.
    /// </summary>
    public static readonly BindableProperty TypefaceProperty =
        BindableProperty.Create(nameof(Typeface), typeof(SKTypeface), typeof(GradientCircularButton),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the typeface for the button.
    /// </summary>
    /// <value>Expects a SKTypeface. Default default(SKTypeface).</value>
    public SKTypeface Typeface
    {
        get => (SKTypeface)GetValue(TypefaceProperty);
        set => SetValue(TypefaceProperty, value);
    }

    public static readonly BindableProperty IsIconifiedTextProperty =
        BindableProperty.Create(nameof(IsIconifiedText), typeof(bool), typeof(GradientCircularButton), false,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    public bool IsIconifiedText
    {
        get => (bool)GetValue(IsIconifiedTextProperty);
        set => SetValue(IsIconifiedTextProperty, value);
    }

    /// <summary>
    /// The command property that fires on tap.
    /// </summary>
    public static readonly BindableProperty CommandProperty =
        BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(GradientCircularButton));

    /// <summary>
    /// Gets or sets the command.
    /// </summary>
    /// <value>Takes a System.Windows.Input.ICommand. Default value is default(ICommand).</value>
    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    /// <summary>
    /// The command parameter property.
    /// </summary>
    public static readonly BindableProperty CommandParameterProperty =
        BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(GradientCircularButton));

    /// <summary>
    /// Gets or sets the command parameter.
    /// </summary>
    /// <value>The command parameter.</value>
    public object CommandParameter
    {
        get => this.GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="T:Aurora.Controls.GradientCircularButton"/> is tapped.
    /// </summary>
    /// <value><c>true</c> if tapped; otherwise, <c>false</c>.</value>
    public bool Tapped
    {
        get => _tapped;

        set
        {
            if (_tapped == value)
            {
                return;
            }

            _tapped = value;
            this.InvalidateSurface();
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GradientCircularButton"/> class.
    /// </summary>
    public GradientCircularButton()
    {
    }

    protected override void Attached()
    {
        this.EnableTouchEvents = true;
        base.Attached();
    }

    protected override void Detached()
    {
        _backgroundPath.Dispose();
        base.Detached();
    }

/// <summary>
    /// Method that is called when the property that is specified by propertyName is changed.
    /// The surface is automatically invalidated/redrawn whenever <c>HeightProperty</c>, <c>WidthProperty</c> or <c>MarginProperty</c> gets updated.
    /// </summary>
    /// <param name="propertyName">The name of the bound property that changed.</param>
    protected override void OnPropertyChanged(string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName.Equals(HeightProperty.PropertyName) ||
            propertyName.Equals(WidthProperty.PropertyName) ||
            propertyName.Equals(MarginProperty.PropertyName))
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
        float scale = info.Height / (float)Height;

        using var backgroundPaint = new SKPaint();
        double size = Math.Min(info.Width - this.Margin.Left - this.Margin.Right, info.Height - this.Margin.Top - this.Margin.Bottom);

        float left = (info.Width - (float)size) / 2f;
        float top = (info.Height - (float)size) / 2f;
        float right = left + (float)size;
        float bottom = top + (float)size;

        var rect = new SKRect(left, top, right, bottom);

        SKPoint gradientPointStart = SKPoint.Empty, gradientPointEnd = SKPoint.Empty;

        double gradientAngle = this.GradientAngle % 360;
        float angleInRadians = (float)(gradientAngle * Math.PI / 180.0);

        gradientPointStart = new SKPoint(
            (float)(((float)Math.Cos(angleInRadians) * size * 0.5f) + rect.MidX),
            (float)(((float)Math.Sin(angleInRadians) * size * 0.5f) + rect.MidY));

        gradientPointEnd = new SKPoint(
            (float)(((float)Math.Cos(angleInRadians + Math.PI) * size * 0.5f) + rect.MidX),
            (float)(((float)Math.Sin(angleInRadians + Math.PI) * size * 0.5f) + rect.MidY));

        var shader =
            SKShader
                .CreateLinearGradient(
                    gradientPointStart, gradientPointEnd,
                    [this.ButtonBackgroundColor.ToSKColor(), this.ButtonBackgroundColor.WithHue(.5f).ToSKColor(),],
                    [0, 1,],
                    SKShaderTileMode.Clamp);

        backgroundPaint.IsAntialias = true;
        backgroundPaint.Style = SKPaintStyle.Fill;
        backgroundPaint.Shader = shader;

        canvas.Clear();
        _backgroundPath.Reset();

        if (!this.ShadowColor.Equals(Colors.Transparent) && this.ShadowLocation != Point.Zero)
        {
            using var shadowPaint = new SKPaint();
            using (new SKAutoCanvasRestore(canvas))
            {
                shadowPaint.IsAntialias = true;
                shadowPaint.Color = this.ShadowColor.ToSKColor();
                shadowPaint.Style = SKPaintStyle.Fill;
                shadowPaint.MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 1);

                canvas.Translate(this.ShadowLocation.ToSKPoint());
                canvas.DrawOval(rect, shadowPaint);
            }
        }

        using (new SKAutoCanvasRestore(canvas))
        {
            if (this.Tapped)
            {
                canvas.Translate(this.ShadowLocation.ToSKPoint());
            }

            _backgroundPath.AddOval(rect);
            canvas.DrawPath(_backgroundPath, backgroundPaint);

            if (_lastTouchLocation != SKPoint.Empty && _rippleAnimationPercentage > 0.0d)
            {
                using var ripplePath = new SKPath();
                using var ripplePaint = new SKPaint();
                ripplePaint.IsAntialias = true;
                ripplePaint.Style = SKPaintStyle.Fill;
                ripplePaint.Color =
                    !this.ButtonBackgroundColor.Equals(Colors.Transparent)
                        ? this.ButtonBackgroundColor.AddLuminosity(-.2f).MultiplyAlpha((1f - (float)_rippleAnimationPercentage) * .5f).ToSKColor()
                        : Colors.Transparent.ToSKColor();

                float startingRippleSize = (float)size * .5f;
                float maxRippleSize = startingRippleSize + ((float)(size * .4f) * (float)_rippleAnimationPercentage);
                float offsetAmount = -maxRippleSize / 2f;
                var offsetPoint = new SKPoint(_lastTouchLocation.X + offsetAmount, _lastTouchLocation.Y + offsetAmount);
                var rippleSize = SKRect.Create(offsetPoint, new SKSize(maxRippleSize, maxRippleSize));
                ripplePath.AddOval(rippleSize);
                using var finalRipple = ripplePath.Op(_backgroundPath, SKPathOp.Intersect);
                canvas.DrawPath(finalRipple, ripplePaint);
            }

            if (this.BorderWidth > 0d && !this.BorderColor.Equals(Colors.Transparent))
            {
                backgroundPaint.StrokeWidth = (float)this.BorderWidth;
                backgroundPaint.Color = this.BorderColor.ToSKColor();
                backgroundPaint.Shader = null;
                backgroundPaint.Style = SKPaintStyle.Stroke;

                canvas.DrawPath(_backgroundPath, backgroundPaint);
            }

            if (!string.IsNullOrEmpty(this.Text))
            {
                using var fontPaint = new SKPaint();
                fontPaint.Color = this.FontColor.ToSKColor();
                fontPaint.TextSize = (float)this.FontSize * scale;
                fontPaint.IsAntialias = true;
                fontPaint.Typeface = this.Typeface ?? PlatformInfo.DefaultTypeface;

                if (this.IsIconifiedText)
                {
                    canvas.DrawCenteredIconifiedText(this.Text, rect.MidX, rect.MidY, fontPaint);
                }
                else
                {
                    canvas.DrawCenteredText(this.Text, rect.MidX, rect.MidY, fontPaint);
                }
            }
        }

        shader?.Dispose();
    }

    /// <summary>
    /// SKCanvas method that fires on touch.
    /// </summary>
    /// <param name="e">Provides data for the SKCanvasView.Touch or SKGLView.Touch event.</param>
    protected override void OnTouch(SKTouchEventArgs e)
    {
        e.Handled = true;

        if (e.ActionType == SKTouchAction.Cancelled || e.ActionType == SKTouchAction.Exited)
        {
            Tapped = false;
            AnimateRipple(reset: true);
            return;
        }

        bool isTapInside = _backgroundPath.Contains(e.Location.X, e.Location.Y);

        if (e.ActionType == SKTouchAction.Released && isTapInside)
        {
            Tapped = false;

            if (Ripples)
            {
                _lastTouchLocation = e.Location;
                AnimateRipple();
            }

            if (Command != null && Command.CanExecute(CommandParameter))
            {
                Command.Execute(CommandParameter);
            }

            this.Clicked?.Invoke(this, EventArgs.Empty);

            return;
        }

        Tapped = e.InContact && isTapInside;
    }

    /// <summary>
    /// Animates the ripple.
    /// </summary>
    /// <param name="reset">If set to <c>true</c> reset.</param>
    private void AnimateRipple(bool reset = false)
    {
        if (_lastTouchLocation == SKPoint.Empty)
        {
            return;
        }

        string animName = nameof(Ripples);

        this.AbortAnimation(animName);
        _rippleAnimationPercentage = 0d;

        if (reset)
        {
            return;
        }

        var rippleAnimation = new Animation(x =>
        {
            _rippleAnimationPercentage = x;
            this.InvalidateSurface();
        });

        rippleAnimation.Commit(
            this, animName, length: 500, easing: Easing.CubicOut,
            finished: (_, _) =>
            {
                _lastTouchLocation = SKPoint.Empty;
                _rippleAnimationPercentage = 0d;
                this.InvalidateSurface();
            });
    }
}
