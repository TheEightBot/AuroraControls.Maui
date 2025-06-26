namespace AuroraControls;

/// <summary>
/// Gradient circular button.
/// </summary>
#pragma warning disable CA1001
public class GradientCircularButton : AuroraViewBase
#pragma warning restore CA1001
{
    private readonly SKPath _backgroundPath = new SKPath();

    private SKPoint _lastTouchLocation;
    private double _rippleAnimationPercentage;
    private bool _tapped;

    public event EventHandler Clicked;

    /// <summary>
    /// The button background color property.
    /// </summary>
    public static BindableProperty ButtonBackgroundColorProperty =
        BindableProperty.Create(nameof(ButtonBackgroundColor), typeof(Color), typeof(GradientCircularButton), Colors.Transparent,
            propertyChanged: (bindable, _, _) => (bindable as IAuroraView)?.InvalidateSurface());

    /// <summary>
    /// Gets or sets the color of the button background.
    /// </summary>
    /// <value>Expects a Xamarin.Forms.Color. Default color is Xamarin.Forms.Color.Default.</value>
    public Color ButtonBackgroundColor
    {
        get => (Color)GetValue(ButtonBackgroundColorProperty);
        set => SetValue(ButtonBackgroundColorProperty, value);
    }

    /// <summary>
    /// Specifies the border width property..
    /// </summary>
    public static BindableProperty BorderWidthProperty =
        BindableProperty.Create(nameof(BorderWidth), typeof(double), typeof(GradientCircularButton), 0d,
            propertyChanged: (bindable, _, _) => (bindable as IAuroraView)?.InvalidateSurface());

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
    public static BindableProperty BorderColorProperty =
        BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(GradientCircularButton), Colors.White,
            propertyChanged: (bindable, _, _) => (bindable as IAuroraView)?.InvalidateSurface());

    /// <summary>
    /// Gets or sets the color of the border.
    /// </summary>
    /// <value>Expects a Xamarin.Forms.Color. Default is Xamarin.Forms.Color.White.</value>
    public Color BorderColor
    {
        get => (Color)GetValue(BorderColorProperty);
        set => SetValue(BorderColorProperty, value);
    }

    /// <summary>
    /// The shadow color property.
    /// </summary>
    public static BindableProperty ShadowColorProperty =
        BindableProperty.Create(nameof(ShadowColor), typeof(Color), typeof(GradientCircularButton), Color.FromRgba(0d, 0d, 0d, .33d),
            propertyChanged: (bindable, _, _) => (bindable as IAuroraView)?.InvalidateSurface());

    /// <summary>
    /// Gets or sets the color of the shadow.
    /// </summary>
    /// <value>Expects a Xamarin.Forms.Color. Default value is Xamarin.Forms.Color.FromRgba(0d, 0d, 0d, .33d).</value>
    public Color ShadowColor
    {
        get => (Color)GetValue(ShadowColorProperty);
        set => SetValue(ShadowColorProperty, value);
    }

    /// <summary>
    /// The shadow location property.
    /// </summary>
    public static BindableProperty ShadowLocationProperty =
        BindableProperty.Create(nameof(ShadowLocation), typeof(Point), typeof(GradientCircularButton), default(Point),
            propertyChanged: (bindable, _, _) => (bindable as IAuroraView)?.InvalidateSurface());

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
    public static BindableProperty GradientAngleProperty =
        BindableProperty.Create(nameof(GradientAngle), typeof(double), typeof(GradientCircularButton), 90.0,
            propertyChanged: (bindable, _, _) => (bindable as IAuroraView)?.InvalidateSurface());

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
    public static BindableProperty RipplesProperty =
        BindableProperty.Create(nameof(Ripples), typeof(bool), typeof(GradientCircularButton), true,
            propertyChanged: (bindable, _, _) => (bindable as IAuroraView)?.InvalidateSurface());

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
    public static BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(GradientCircularButton),
            propertyChanged: (bindable, _, _) => (bindable as IAuroraView)?.InvalidateSurface());

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
    public static BindableProperty FontColorProperty =
        BindableProperty.Create(nameof(FontColor), typeof(Color), typeof(GradientCircularButton), Colors.White,
            propertyChanged: (bindable, _, _) => (bindable as IAuroraView)?.InvalidateSurface());

    public static BindableProperty FontSizeProperty =
        BindableProperty.Create(nameof(FontSize), typeof(double), typeof(GradientCircularButton), PlatformInfo.DefaultButtonFontSize,
            propertyChanged: (bindable, _, _) => (bindable as IAuroraView)?.InvalidateSurface());

    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the color of the font.
    /// </summary>
    /// <value>Expects a Xamarin.Forms.Color. Default value is Color.White.</value>
    public Color FontColor
    {
        get => (Color)GetValue(FontColorProperty);
        set => SetValue(FontColorProperty, value);
    }

    /// <summary>
    /// The typeface property.
    /// </summary>
    public static BindableProperty TypefaceProperty =
        BindableProperty.Create(nameof(Typeface), typeof(SKTypeface), typeof(GradientCircularButton),
            propertyChanged: (bindable, _, _) => (bindable as IAuroraView)?.InvalidateSurface());

    /// <summary>
    /// Gets or sets the typeface for the button.
    /// </summary>
    /// <value>Expects a SKTypeface. Default default(SKTypeface).</value>
    public SKTypeface Typeface
    {
        get => (SKTypeface)GetValue(TypefaceProperty);
        set => SetValue(TypefaceProperty, value);
    }

    public static BindableProperty IsIconifiedTextProperty =
        BindableProperty.Create(nameof(IsIconifiedText), typeof(bool), typeof(GradientCircularButton), false,
            propertyChanged: (bindable, _, _) => (bindable as IAuroraView)?.InvalidateSurface());

    public bool IsIconifiedText
    {
        get => (bool)GetValue(IsIconifiedTextProperty);
        set => SetValue(IsIconifiedTextProperty, value);
    }

    /// <summary>
    /// The command property that fires on tap.
    /// </summary>
    public static BindableProperty CommandProperty =
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
    public static BindableProperty CommandParameterProperty =
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
            if (this._tapped == value)
            {
                return;
            }

            this._tapped = value;
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
        var scale = info.Height / (float)Height;

        using var backgroundPaint = new SKPaint();
        var size = Math.Min(info.Width - this.Margin.Left - this.Margin.Right, info.Height - this.Margin.Top - this.Margin.Bottom);

        var left = (info.Width - (float)size) / 2f;
        var top = (info.Height - (float)size) / 2f;
        var right = left + (float)size;
        var bottom = top + (float)size;

        var rect = new SKRect(left, top, right, bottom);

        SKPoint gradientPointStart = SKPoint.Empty, gradientPointEnd = SKPoint.Empty;

        var gradientAngle = this.GradientAngle % 360;
        var angleInRadians = (float)(gradientAngle * Math.PI / 180.0);

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
        this._backgroundPath.Reset();

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

            this._backgroundPath.AddOval(rect);
            canvas.DrawPath(this._backgroundPath, backgroundPaint);

            if (this._lastTouchLocation != SKPoint.Empty && this._rippleAnimationPercentage > 0.0d)
            {
                using var ripplePath = new SKPath();
                using var ripplePaint = new SKPaint();
                ripplePaint.IsAntialias = true;
                ripplePaint.Style = SKPaintStyle.Fill;
                ripplePaint.Color =
                    !this.ButtonBackgroundColor.Equals(Colors.Transparent)
                        ? this.ButtonBackgroundColor.AddLuminosity(-.2f).MultiplyAlpha((1f - (float)this._rippleAnimationPercentage) * .5f).ToSKColor()
                        : Colors.Transparent.ToSKColor();

                var startingRippleSize = (float)size * .5f;
                var maxRippleSize = startingRippleSize + ((float)(size * .4f) * (float)this._rippleAnimationPercentage);
                var offsetAmount = -maxRippleSize / 2f;
                var offsetPoint = new SKPoint(this._lastTouchLocation.X + offsetAmount, this._lastTouchLocation.Y + offsetAmount);
                var rippleSize = SKRect.Create(offsetPoint, new SKSize(maxRippleSize, maxRippleSize));
                ripplePath.AddOval(rippleSize);
                using var finalRipple = ripplePath.Op(this._backgroundPath, SKPathOp.Intersect);
                canvas.DrawPath(finalRipple, ripplePaint);
            }

            if (this.BorderWidth > 0d && !this.BorderColor.Equals(Colors.Transparent))
            {
                backgroundPaint.StrokeWidth = (float)this.BorderWidth;
                backgroundPaint.Color = this.BorderColor.ToSKColor();
                backgroundPaint.Shader = null;
                backgroundPaint.Style = SKPaintStyle.Stroke;

                canvas.DrawPath(this._backgroundPath, backgroundPaint);
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

        var isTapInside = _backgroundPath.Contains(e.Location.X, e.Location.Y);

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

        var animName = nameof(Ripples);

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
