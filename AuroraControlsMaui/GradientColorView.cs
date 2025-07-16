using System.ComponentModel;

namespace AuroraControls;

#pragma warning disable CA1001 // Types that own disposable fields should be disposable
public class GradientColorView : AuroraViewBase
#pragma warning disable CA1001 // Types that own disposable fields should be disposable
{
    private readonly string _rippleAnimationName, _tapAnimationName;
    private readonly SKPath _backgroundPath = new();
    private SKPoint _lastTouchLocation;
    private double _rippleAnimationPercentage;
    private double _tapAnimationPercentage;
    private bool _tapped;

    /// <summary>
    /// The gradient rotation angle property.
    /// </summary>
    public static readonly BindableProperty GradientRotationAngleProperty =
        BindableProperty.Create(nameof(GradientRotationAngle), typeof(double), typeof(GradientColorView), 0d,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the angle of rotation for the gradient.
    /// </summary>
    /// <value>Rotation angle as a double. Default is 0d.</value>
    public double GradientRotationAngle
    {
        get => (double)GetValue(GradientRotationAngleProperty);
        set => SetValue(GradientRotationAngleProperty, value.Clamp(-360, 360));
    }

    /// <summary>
    /// The gradient start color property.
    /// </summary>
    public static readonly BindableProperty GradientStartColorProperty =
        BindableProperty.Create(nameof(GradientStartColor), typeof(Color), typeof(GradientColorView), default(Color),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the start color of the gradient.
    /// </summary>
    /// <value>Expects a Xamarin.Forms.Color. Default color is Xamarin.Forms.Color.Default.</value>
    public Color GradientStartColor
    {
        get => (Color)GetValue(GradientStartColorProperty);
        set => SetValue(GradientStartColorProperty, value);
    }

    /// <summary>
    /// The gradient stop color property.
    /// </summary>
    public static readonly BindableProperty GradientStopColorProperty =
        BindableProperty.Create(nameof(GradientStopColor), typeof(Color), typeof(GradientColorView), default(Color),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the stop color of the gradient.
    /// </summary>
    /// <value>Expects a Xamarin.Forms.Color. Default color is Xamarin.Forms.Color.Default.</value>
    public Color GradientStopColor
    {
        get => (Color)GetValue(GradientStopColorProperty);
        set => SetValue(GradientStopColorProperty, value);
    }

    /// <summary>
    /// The ripples property specifies whether the ripple animation should be performed.
    /// </summary>
    public static readonly BindableProperty RipplesProperty =
        BindableProperty.Create(nameof(Ripples), typeof(bool), typeof(GradientColorView), true,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="T:Aurora.Controls.GradientColorView"/> is ripples.
    /// </summary>
    /// <value><c>true</c> if ripples; otherwise, <c>false</c>.</value>
    public bool Ripples
    {
        get => (bool)GetValue(RipplesProperty);
        set => SetValue(RipplesProperty, value);
    }

    /// <summary>
    /// The tap animation duration property.
    /// </summary>
    public static readonly BindableProperty TapAnimationDurationProperty =
        BindableProperty.Create(nameof(TapAnimationDuration), typeof(uint), typeof(GradientColorView), 40u,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the duration of the tap animation.
    /// </summary>
    /// <value>Duration as an uint. Default is 40u.</value>
    [TypeConverter(typeof(UIntTypeConverter))]
    public uint TapAnimationDuration
    {
        get => (uint)GetValue(TapAnimationDurationProperty);
        set => SetValue(TapAnimationDurationProperty, value);
    }

    /// <summary>
    /// The command property. Fires on tap.
    /// </summary>
    public static readonly BindableProperty CommandProperty =
        BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(GradientColorView), default(ICommand));

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
        BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(GradientColorView), default(object));

    /// <summary>
    /// Gets or sets the command parameter.
    /// </summary>
    /// <value>The command parameter.</value>
    public object CommandParameter
    {
        get => (object)GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="T:Aurora.Controls.Tile"/> is tapped.
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
                AnimateTap(value);
            }
        }
    }

    public GradientColorView()
    {
        _rippleAnimationName = $"{nameof(Ripples)}_{Guid.NewGuid()}";
        _tapAnimationName = $"{nameof(TapAnimationDuration)}_{Guid.NewGuid()}";
    }

    protected override void Attached()
    {
        this.EnableTouchEvents = true;
        base.Attached();
    }

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

    protected override void PaintControl(SKSurface surface, SKImageInfo info)
    {
        var canvas = surface.Canvas;

        using var overlayPaint = new SKPaint();
        using var shader =
            SKShader
                .CreateLinearGradient(
                    new SKPoint(0, 0), new SKPoint(info.Width, 0),
                    new SKColor[] { this.GradientStartColor.ToSKColor(), this.GradientStopColor.ToSKColor() },
                    new float[] { 0, 1 },
                    SKShaderTileMode.Clamp);
        overlayPaint.BlendMode = SKBlendMode.Color;
        overlayPaint.Shader = shader;
        overlayPaint.IsAntialias = true;

        double size = Math.Min(info.Width - this.Margin.Left - this.Margin.Right, info.Height - this.Margin.Top - this.Margin.Bottom);

        float left = (info.Width - (float)size) / 2f;
        float top = (info.Height - (float)size) / 2f;

        canvas.Clear();
        _backgroundPath.Reset();

        _backgroundPath.AddRect(new SKRect(0, 0, info.Width, info.Height));

        if (_lastTouchLocation != SKPoint.Empty && _rippleAnimationPercentage > 0.0d)
        {
            using var ripplePath = new SKPath();
            using var ripplePaint = new SKPaint();
            ripplePaint.IsAntialias = true;
            ripplePaint.Style = SKPaintStyle.Fill;
            ripplePaint.Color =
                this.GradientStartColor != Colors.Transparent
                    ? this.GradientStartColor.AddLuminosity(-.2f).MultiplyAlpha((1f - (float)_rippleAnimationPercentage) * .5f).ToSKColor()
                    : Colors.Transparent.ToSKColor();

            float startingRippleSize = Math.Min(info.Width, info.Height) * .75f;
            float maxRippleSize = startingRippleSize + (float)((Math.Max(info.Width, info.Height) * .4) * _rippleAnimationPercentage);
            float offsetAmount = -maxRippleSize / 2f;
            var offsetPoint = new SKPoint(_lastTouchLocation.X + offsetAmount, _lastTouchLocation.Y + offsetAmount);
            var rippleSize = SKRect.Create(offsetPoint, new SKSize(maxRippleSize, maxRippleSize));
            ripplePath.AddOval(rippleSize);

            using var finalRipple = ripplePath.Op(_backgroundPath, SKPathOp.Intersect);
            canvas.DrawPath(finalRipple, ripplePaint);
        }

        SKMatrix.CreateTranslation(info.Width / 2f, info.Height / 2f);

        using (new SKAutoCanvasRestore(canvas))
        {
            canvas.RotateDegrees((float)this.GradientRotationAngle, info.Width / 2f, info.Height / 2f);

            canvas.DrawPaint(overlayPaint);
        }
    }

    protected override void OnTouch(SKTouchEventArgs e)
    {
        e.Handled = true;

        if (e.ActionType == SKTouchAction.Cancelled || e.ActionType == SKTouchAction.Exited)
        {
            Tapped = false;
            AnimateRipple(reset: true);
            return;
        }

        if (Ripples && e.ActionType == SKTouchAction.Pressed)
        {
            _lastTouchLocation = e.Location;
            AnimateRipple();
        }

        if (e.ActionType == SKTouchAction.Released)
        {
            Tapped = false;

            _lastTouchLocation = e.Location;

            if (Command?.CanExecute(CommandParameter) ?? false)
            {
                Command.Execute(CommandParameter);
            }

            return;
        }

        Tapped = e.InContact;
    }

    private void AnimateRipple(bool reset = false)
    {
        if (_lastTouchLocation == SKPoint.Empty)
        {
            return;
        }

        this.AbortAnimation(_rippleAnimationName);
        _rippleAnimationPercentage = 0d;

        if (reset)
        {
            return;
        }

        var rippleAnimation = new Animation(
            x =>
            {
                _rippleAnimationPercentage = x;
                this.InvalidateSurface();
            });

        rippleAnimation.Commit(
            this, _rippleAnimationName, length: 500, easing: Easing.CubicOut,
            finished:
                (_, _) =>
                {
                    _lastTouchLocation = SKPoint.Empty;
                    _rippleAnimationPercentage = 0d;
                    this.InvalidateSurface();
                });
    }

    private void AnimateTap(bool tapped, bool reset = false)
    {
        this.AbortAnimation(_tapAnimationName);

        if (reset)
        {
            return;
        }

        var rippleAnimation =
            new Animation(
                x =>
                {
                    _tapAnimationPercentage = x;
                    this.InvalidateSurface();
                },
                _tapAnimationPercentage,
                tapped ? 1 : 0);

        rippleAnimation.Commit(
            this, _tapAnimationName, length: TapAnimationDuration, easing: tapped ? Easing.CubicOut : Easing.CubicIn,
            finished:
                (_, _) =>
                {
                    _tapAnimationPercentage = tapped ? 1 : 0;
                    this.InvalidateSurface();
                });
    }
}
