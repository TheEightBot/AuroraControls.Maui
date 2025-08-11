using Topten.RichTextKit;

namespace AuroraControls;

/// <summary>
/// Gradient pill button.
/// </summary>
#pragma warning disable CA1001 // Types that own disposable fields should be disposable
public class GradientPillButton : AuroraViewBase
#pragma warning restore CA1001 // Types that own disposable fields should be disposable
{
    private SKPath _backgroundPath;
    private SKPoint _lastTouchLocation;
    private double _rippleAnimationPercentage;

    private bool _tapped;

    public event EventHandler Clicked;

    /// <summary>
    /// The button background start color property.
    /// </summary>
    public static readonly BindableProperty ButtonBackgroundStartColorProperty =
        BindableProperty.Create(nameof(ButtonBackgroundStartColor), typeof(Color), typeof(GradientPillButton), Colors.Transparent,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the start color of the button background.
    /// </summary>
    /// <value>Expects a Color. The default value is Colors.Transparent.</value>
    public Color ButtonBackgroundStartColor
    {
        get => (Color)GetValue(ButtonBackgroundStartColorProperty);
        set => SetValue(ButtonBackgroundStartColorProperty, value);
    }

    /// <summary>
    /// The button background end color property.
    /// </summary>
    public static readonly BindableProperty ButtonBackgroundEndColorProperty =
        BindableProperty.Create(nameof(ButtonBackgroundEndColor), typeof(Color), typeof(GradientPillButton),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the end color of the button background.
    /// </summary>
    /// <value>Expects a Color. The default value is Colors.Transparent.</value>
    public Color ButtonBackgroundEndColor
    {
        get => (Color)GetValue(ButtonBackgroundEndColorProperty);
        set => SetValue(ButtonBackgroundEndColorProperty, value);
    }

    /// <summary>
    /// The gradient direction property.
    /// </summary>
    public static readonly BindableProperty GradientDirectionProperty =
        BindableProperty.Create(nameof(GradientDirection), typeof(GradientDirection), typeof(GradientPillButton), GradientDirection.Horizontal,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the gradient direction.
    /// </summary>
    /// <value>Expects GradientDirection enum case. Default value is GradientDirection.Horizontal.</value>
    public GradientDirection GradientDirection
    {
        get => (GradientDirection)GetValue(GradientDirectionProperty);
        set => SetValue(GradientDirectionProperty, value);
    }

    /// <summary>
    /// The border color property.
    /// </summary>
    public static readonly BindableProperty BorderColorProperty =
        BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(GradientPillButton),
                                propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the color of the border.
    /// </summary>
    /// <value>Expects a Color. The default value is Colors.Transparent.</value>
    public Color BorderColor
    {
        get => (Color)GetValue(BorderColorProperty);
        set => SetValue(BorderColorProperty, value);
    }

    /// <summary>
    /// The shadow color property.
    /// </summary>
    public static readonly BindableProperty ShadowColorProperty =
        BindableProperty.Create(nameof(ShadowColor), typeof(Color), typeof(GradientPillButton), Color.FromRgba(0d, 0d, 0d, .33d),
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
        BindableProperty.Create(nameof(ShadowLocation), typeof(Point), typeof(GradientPillButton), new Point(0, 3),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the shadow location.
    /// </summary>
    /// <value>Takes a point with x and y offsets. Default value is new Point(0, 3).</value>
    public Point ShadowLocation
    {
        get => (Point)GetValue(ShadowLocationProperty);
        set => SetValue(ShadowLocationProperty, value);
    }

    /// <summary>
    /// The shadow blur radius property.
    /// </summary>
    public static readonly BindableProperty ShadowBlurRadiusProperty =
        BindableProperty.Create(nameof(ShadowBlurRadius), typeof(double), typeof(GradientPillButton), default(double),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the shadow blur radius.
    /// </summary>
    /// <value>The shadow blur radius. Default value is default(double).</value>
    public double ShadowBlurRadius
    {
        get => (double)GetValue(ShadowBlurRadiusProperty);
        set => SetValue(ShadowBlurRadiusProperty, value);
    }

    /// <summary>
    /// The border width property.
    /// </summary>
    public static readonly BindableProperty BorderWidthProperty =
        BindableProperty.Create(nameof(BorderWidth), typeof(double), typeof(GradientPillButton), 0d,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the width of the border.
    /// </summary>
    /// <value>Expects a double value. Default is 0d.</value>
    public double BorderWidth
    {
        get => (double)GetValue(BorderWidthProperty);
        set => SetValue(BorderWidthProperty, value);
    }

    /// <summary>
    /// The text property.
    /// </summary>
    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(GradientPillButton),
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
        BindableProperty.Create(nameof(FontColor), typeof(Color), typeof(GradientPillButton), Colors.White,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the color of the font.
    /// </summary>
    /// <value>Expects a Color. Default value is Color.White.</value>
    public Color FontColor
    {
        get => (Color)GetValue(FontColorProperty);
        set => SetValue(FontColorProperty, value);
    }

    public static readonly BindableProperty FontSizeProperty =
        BindableProperty.Create(nameof(FontSize), typeof(double), typeof(GradientPillButton), PlatformInfo.DefaultButtonFontSize,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    /// <summary>
    /// The typeface property.
    /// </summary>
    public static readonly BindableProperty FontFamilyProperty =
        BindableProperty.Create(nameof(FontFamily), typeof(string), typeof(GradientPillButton),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the typeface for the button.
    /// </summary>
    /// <value>Expects a string. Dfault default(string).</value>
    public string FontFamily
    {
        get => (string)GetValue(FontFamilyProperty);
        set => SetValue(FontFamilyProperty, value);
    }

    public static readonly BindableProperty IsIconifiedTextProperty =
        BindableProperty.Create(nameof(IsIconifiedText), typeof(bool), typeof(GradientPillButton), default(bool),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    public bool IsIconifiedText
    {
        get => (bool)GetValue(IsIconifiedTextProperty);
        set => SetValue(IsIconifiedTextProperty, value);
    }

    /// <summary>
    /// The ripples property.
    /// </summary>
    public static readonly BindableProperty RipplesProperty =
        BindableProperty.Create(nameof(Ripples), typeof(bool), typeof(GradientPillButton), true,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="T:Aurora.Controls.GradientPillButton"/> is ripples.
    /// </summary>
    /// <value><c>true</c> if ripples; otherwise, <c>false</c>.</value>
    public bool Ripples
    {
        get => (bool)GetValue(RipplesProperty);
        set => SetValue(RipplesProperty, value);
    }

    /// <summary>
    /// The command property. Fires on tap.
    /// </summary>
    public static readonly BindableProperty CommandProperty =
        BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(GradientPillButton));

    /// <summary>
    /// Gets or sets the command.
    /// </summary>
    /// <value>The command.</value>
    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    /// <summary>
    /// The command parameter property.
    /// </summary>
    public static readonly BindableProperty CommandParameterProperty =
        BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(GradientPillButton));

    /// <summary>
    /// Gets or sets the command parameter.
    /// </summary>
    /// <value>The command parameter. default value is default(object).</value>
    public object CommandParameter
    {
        get => (object)GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="T:Aurora.Controls.GradientPillButton"/> is tapped.
    /// </summary>
    /// <value><c>true</c> if tapped; otherwise, <c>false</c>.</value>
    public bool Tapped
    {
        get => _tapped;

        set
        {
            if (_tapped != value)
            {
                _tapped = value;
                this.InvalidateSurface();
            }
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GradientPillButton"/> class.
    /// </summary>
    public GradientPillButton() => MinimumHeightRequest = IAuroraView.StandardControlHeight;

    protected override void Attached()
    {
        this.EnableTouchEvents = true;
        _backgroundPath = new();
        base.Attached();
    }

    protected override void Detached()
    {
        _backgroundPath.Dispose();
        base.Detached();
    }

    /// <summary>
    /// This is the method used to draw our control on the SKCanvas. This method is fired every time <c>this.InvalidateSurface();</c> is called, resulting in a "redrawing" of the control.
    /// </summary>
    /// <param name="surface">The skia surface to paint on the controls.</param>
    /// <param name="info">Information about the skia image.</param>
    protected override void PaintControl(SKSurface surface, SKImageInfo info)
    {
        var canvas = surface.Canvas;

        if (!Equals(this.ButtonBackgroundStartColor, Colors.Transparent) && Equals(this.ButtonBackgroundEndColor, Colors.Transparent))
        {
            ButtonBackgroundEndColor = ButtonBackgroundStartColor.WithHue(.5f);
        }

        using var backgroundPaint = new SKPaint();
        float halfBorder = (float)this.BorderWidth / 2f;

        float scale = info.Height / (float)this.Height;
        var rect = new SKRect((float)this.ShadowLocation.X + (float)this.ShadowBlurRadius + halfBorder, (float)this.ShadowLocation.Y + (float)this.ShadowBlurRadius + halfBorder,
            info.Width - (float)this.ShadowLocation.X - (float)this.ShadowBlurRadius - halfBorder, info.Height - (float)this.ShadowLocation.Y - (float)this.ShadowBlurRadius - halfBorder);

        SKPoint gradientPointStart = SKPoint.Empty, gradientPointEnd = SKPoint.Empty;

        switch (this.GradientDirection)
        {
            case GradientDirection.Horizontal:
                gradientPointStart = new SKPoint(rect.Left, 0);
                gradientPointEnd = new SKPoint(rect.Right, 0);
                break;
            case GradientDirection.Vertical:
                gradientPointStart = new SKPoint(0f, 0f);
                gradientPointEnd = new SKPoint(0f, rect.Bottom);
                break;
            default:
                break;
        }

        var shader =
            SKShader
                .CreateLinearGradient(
                    gradientPointStart, gradientPointEnd,
                    new SKColor[] { this.ButtonBackgroundStartColor.ToSKColor(), this.ButtonBackgroundEndColor.ToSKColor() },
                    new float[] { 0, 1 },
                    SKShaderTileMode.Clamp);

        backgroundPaint.IsAntialias = true;
        backgroundPaint.Style = SKPaintStyle.Fill;
        backgroundPaint.Shader = shader;

        canvas.Clear();

        if (_backgroundPath is null)
        {
            return;
        }

        _backgroundPath.Reset();

        if (this.ShadowColor != Colors.Transparent && this.ShadowLocation != Point.Zero)
        {
            using var shadowPaint = new SKPaint();
            using (new SKAutoCanvasRestore(canvas))
            {
                shadowPaint.IsAntialias = true;
                shadowPaint.Color = this.ShadowColor.ToSKColor();
                shadowPaint.Style = SKPaintStyle.Fill;
                shadowPaint.MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, SKMaskFilter.ConvertRadiusToSigma((float)this.ShadowBlurRadius));

                canvas.Translate(this.ShadowLocation.ToSKPoint());
                canvas.DrawRoundRect(rect, info.Height / 2f, info.Height / 2f, shadowPaint);
            }
        }

        using (new SKAutoCanvasRestore(canvas))
        {
            if (this.Tapped)
            {
                canvas.Translate(this.ShadowLocation.ToSKPoint());
            }

            _backgroundPath.AddRoundRect(rect, info.Height / 2f, info.Height / 2f);
            canvas.DrawPath(_backgroundPath, backgroundPaint);

            if (_lastTouchLocation != SKPoint.Empty && _rippleAnimationPercentage > 0.0d)
            {
                using var ripplePath = new SKPath();
                using var ripplePaint = new SKPaint();
                ripplePaint.IsAntialias = true;
                ripplePaint.Style = SKPaintStyle.Fill;
                ripplePaint.Color =
                    this.ButtonBackgroundStartColor != Colors.Transparent
                        ? this.ButtonBackgroundEndColor.AddLuminosity(-.2f).MultiplyAlpha((1 - (float)_rippleAnimationPercentage) * .5f).ToSKColor()
                        : Colors.Transparent.ToSKColor();

                float startingRippleSize = Math.Min(info.Width, info.Height) * 1.5f;
                float maxRippleSize = startingRippleSize + (float)((Math.Max(info.Width, info.Height) * .4) * _rippleAnimationPercentage);
                float offsetAmount = -maxRippleSize / 2f;
                var offsetPoint = new SKPoint(_lastTouchLocation.X + offsetAmount, _lastTouchLocation.Y + offsetAmount);
                var rippleSize = SKRect.Create(offsetPoint, new SKSize(maxRippleSize, maxRippleSize));
                ripplePath.AddOval(rippleSize);

                using var finalRipple = ripplePath.Op(_backgroundPath, SKPathOp.Intersect);
                canvas.DrawPath(finalRipple, ripplePaint);
            }

            if (this.BorderWidth > 0d && this.BorderColor != Colors.Transparent)
            {
                backgroundPaint.StrokeWidth = (float)this.BorderWidth;
                backgroundPaint.Color = this.BorderColor.ToSKColor();
                backgroundPaint.Shader = null;
                backgroundPaint.Style = SKPaintStyle.Stroke;

                canvas.DrawPath(_backgroundPath, backgroundPaint);
            }

            if (!string.IsNullOrEmpty(this.Text))
            {
                var text =
                    new RichString()
                        .Add(
                            this.Text,
                            textColor: this.FontColor.ToSKColor(),
                            fontSize: (float)this.FontSize * scale,
                            fontFamily: this.FontFamily)
                        .Alignment(Topten.RichTextKit.TextAlignment.Center);

                var drawPoint =
                    new SKPoint(
                        rect.MidX - (text.MeasuredWidth * .5f),
                        rect.MidY - (text.MeasuredHeight * .5f));

                text.Paint(canvas, drawPoint);
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

            if (Command?.CanExecute(CommandParameter) ?? false)
            {
                Command.Execute(CommandParameter);
            }

            Clicked?.Invoke(this, EventArgs.Empty);

            return;
        }

        Tapped = e.InContact && isTapInside;
    }

    /// <summary>
    /// Fires ripple animation.
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
