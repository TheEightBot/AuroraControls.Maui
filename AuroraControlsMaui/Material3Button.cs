using System.ComponentModel;

namespace AuroraControls;

/// <summary>
/// Material3 button implementation following Google's Material Design guidelines.
/// Includes ripple effect, elevation, and other Material Design 3 specific styling.
/// </summary>
#pragma warning disable CA1001 // Types that own disposable fields should be disposable
public class Material3Button : AuroraViewBase
#pragma warning restore CA1001 // Types that own disposable fields should be disposable
{
    private readonly string _rippleAnimationName;
    private readonly string _stateAnimationName;
    private readonly SKPath _backgroundPath = new SKPath();
    private SKPoint _lastTouchLocation;
    private double _rippleAnimationPercentage;
    private double _stateAnimationPercentage;
    private bool _pressed;
    private SKPaint _shadowPaint;

    public event EventHandler Clicked;

    /// <summary>
    /// The button variant property (Filled, Outlined, Text, etc.)
    /// </summary>
    public static readonly BindableProperty ButtonVariantProperty =
        BindableProperty.Create(nameof(ButtonVariant), typeof(ButtonVariant), typeof(Material3Button),
            ButtonVariant.Filled,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the button variant.
    /// </summary>
    public ButtonVariant ButtonVariant
    {
        get => (ButtonVariant)GetValue(ButtonVariantProperty);
        set => SetValue(ButtonVariantProperty, value);
    }

    /// <summary>
    /// The primary color property.
    /// </summary>
    public static readonly BindableProperty PrimaryColorProperty =
        BindableProperty.Create(nameof(PrimaryColor), typeof(Color), typeof(Material3Button), Colors.Purple,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the primary color of the button.
    /// </summary>
    public Color PrimaryColor
    {
        get => (Color)GetValue(PrimaryColorProperty);
        set => SetValue(PrimaryColorProperty, value);
    }

    /// <summary>
    /// The on-primary color property (used as text color for filled buttons).
    /// </summary>
    public static readonly BindableProperty OnPrimaryColorProperty =
        BindableProperty.Create(nameof(OnPrimaryColor), typeof(Color), typeof(Material3Button), Colors.White,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the on-primary color.
    /// </summary>
    public Color OnPrimaryColor
    {
        get => (Color)GetValue(OnPrimaryColorProperty);
        set => SetValue(OnPrimaryColorProperty, value);
    }

    /// <summary>
    /// The surface (background) color property.
    /// </summary>
    public static readonly BindableProperty SurfaceColorProperty =
        BindableProperty.Create(nameof(SurfaceColor), typeof(Color), typeof(Material3Button), Colors.White,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the surface color.
    /// </summary>
    public Color SurfaceColor
    {
        get => (Color)GetValue(SurfaceColorProperty);
        set => SetValue(SurfaceColorProperty, value);
    }

    /// <summary>
    /// The corner radius property.
    /// </summary>
    public static readonly BindableProperty CornerRadiusProperty =
        BindableProperty.Create(nameof(CornerRadius), typeof(float), typeof(Material3Button), 20f,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the corner radius.
    /// </summary>
    public float CornerRadius
    {
        get => (float)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    /// <summary>
    /// The elevation property.
    /// </summary>
    public static readonly BindableProperty ElevationProperty =
        BindableProperty.Create(nameof(Elevation), typeof(int), typeof(Material3Button), 0,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the elevation level (0-5).
    /// </summary>
    public int Elevation
    {
        get => (int)GetValue(ElevationProperty);
        set => SetValue(ElevationProperty, Math.Clamp(value, 0, 5));
    }

    /// <summary>
    /// The shadow color property.
    /// </summary>
    public static readonly BindableProperty ShadowColorProperty =
        BindableProperty.Create(nameof(ShadowColor), typeof(Color), typeof(Material3Button),
            Color.FromRgba(0, 0, 0, 0.25),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the shadow color.
    /// </summary>
    public Color ShadowColor
    {
        get => (Color)GetValue(ShadowColorProperty);
        set => SetValue(ShadowColorProperty, value);
    }

    /// <summary>
    /// The text property.
    /// </summary>
    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(Material3Button), string.Empty,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the text for the button.
    /// </summary>
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>
    /// The font size property.
    /// </summary>
    public static readonly BindableProperty FontSizeProperty =
        BindableProperty.Create(nameof(FontSize), typeof(double), typeof(Material3Button), 14.0,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the font size.
    /// </summary>
    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    /// <summary>
    /// The font family property.
    /// </summary>
    public static readonly BindableProperty FontFamilyProperty =
        BindableProperty.Create(nameof(FontFamily), typeof(string), typeof(Material3Button), string.Empty,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the font family.
    /// </summary>
    public string FontFamily
    {
        get => (string)GetValue(FontFamilyProperty);
        set => SetValue(FontFamilyProperty, value);
    }

    /// <summary>
    /// The command property.
    /// </summary>
    public static readonly BindableProperty CommandProperty =
        BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(Material3Button), default(ICommand));

    /// <summary>
    /// Gets or sets the command to execute when the button is clicked.
    /// </summary>
    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    /// <summary>
    /// The command parameter property.
    /// </summary>
    public static readonly BindableProperty CommandParameterProperty =
        BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(Material3Button), default(object));

    /// <summary>
    /// Gets or sets the command parameter.
    /// </summary>
    public object CommandParameter
    {
        get => (object)GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    /// <summary>
    /// The disabled property.
    /// </summary>
    public static readonly BindableProperty IsDisabledProperty =
        BindableProperty.Create(nameof(IsDisabled), typeof(bool), typeof(Material3Button), false,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets a value indicating whether gets or sets whether the button is disabled.
    /// </summary>
    public bool IsDisabled
    {
        get => (bool)GetValue(IsDisabledProperty);
        set => SetValue(IsDisabledProperty, value);
    }

    /// <summary>
    /// The ripple animation duration property.
    /// </summary>
    public static readonly BindableProperty RippleAnimationDurationProperty =
        BindableProperty.Create(nameof(RippleAnimationDuration), typeof(uint), typeof(Material3Button), 350u);

    /// <summary>
    /// Gets or sets the ripple animation duration in milliseconds.
    /// </summary>
    public uint RippleAnimationDuration
    {
        get => (uint)GetValue(RippleAnimationDurationProperty);
        set => SetValue(RippleAnimationDurationProperty, value);
    }

    /// <summary>
    /// The state animation duration property.
    /// </summary>
    public static readonly BindableProperty StateAnimationDurationProperty =
        BindableProperty.Create(nameof(StateAnimationDuration), typeof(uint), typeof(Material3Button), 100u);

    /// <summary>
    /// Gets or sets the state animation duration in milliseconds.
    /// </summary>
    public uint StateAnimationDuration
    {
        get => (uint)GetValue(StateAnimationDurationProperty);
        set => SetValue(StateAnimationDurationProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether this button is pressed.
    /// </summary>
    public bool Pressed
    {
        get => _pressed;
        set
        {
            if (_pressed != value)
            {
                _pressed = value;
                AnimateStateChange(value);
            }
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Material3Button"/> class.
    /// </summary>
    public Material3Button()
    {
        _rippleAnimationName = $"Ripple_{Guid.NewGuid()}";
        _stateAnimationName = $"State_{Guid.NewGuid()}";
        MinimumHeightRequest = 40;
    }

    protected override void Attached()
    {
        this.EnableTouchEvents = true;
        _shadowPaint = new SKPaint { IsAntialias = true };
        base.Attached();
    }

    protected override void Detached()
    {
        _shadowPaint?.Dispose();
        _shadowPaint = null;
        base.Detached();
    }

    protected override void PaintControl(SKSurface surface, SKImageInfo info)
    {
        var canvas = surface.Canvas;
        canvas.Clear();

        // Setup button dimensions with horizontal padding of at least 16dp
        var minWidth = Math.Max(info.Width, 64);
        var rect = new SKRect(0, 0, minWidth, info.Height);

        // Create the background path
        _backgroundPath.Reset();
        _backgroundPath.AddRoundRect(rect, CornerRadius, CornerRadius);

        // First draw shadows if needed
        if (Elevation > 0 && (ButtonVariant == ButtonVariant.Elevated || ButtonVariant == ButtonVariant.Filled))
        {
            using (new SKAutoCanvasRestore(canvas))
            {
                var elevationPixels = Elevation * 2;
                _shadowPaint.Style = SKPaintStyle.Fill;
                _shadowPaint.Color = ShadowColor.ToSKColor();
                _shadowPaint.MaskFilter = SKMaskFilter.CreateBlur(
                    SKBlurStyle.Normal,
                    SKMaskFilter.ConvertRadiusToSigma(elevationPixels * _scale));

                // Draw the shadow slightly offset
                var shadowPath = new SKPath(_backgroundPath);
                canvas.Translate(0, elevationPixels * 0.4f * _scale);
                canvas.DrawPath(shadowPath, _shadowPaint);
            }
        }

        // Draw button background
        using (var backgroundPaint = new SKPaint())
        {
            backgroundPaint.IsAntialias = true;
            backgroundPaint.Style = SKPaintStyle.Fill;

            // Set background color based on variant and state
            Color backgroundColor;
            Color textColor;

            switch (ButtonVariant)
            {
                case ButtonVariant.Filled:
                    backgroundColor = PrimaryColor;
                    textColor = OnPrimaryColor;
                    break;
                case ButtonVariant.Elevated:
                    backgroundColor = SurfaceColor;
                    textColor = PrimaryColor;
                    break;
                case ButtonVariant.Outlined:
                    backgroundColor = SurfaceColor;
                    textColor = PrimaryColor;
                    break;
                case ButtonVariant.Text:
                default:
                    backgroundColor = Colors.Transparent;
                    textColor = PrimaryColor;
                    break;
            }

            // Apply disabled state
            if (IsDisabled)
            {
                backgroundColor = backgroundColor.WithAlpha(0.12f);
                textColor = PrimaryColor.WithAlpha(0.38f);
            }

            // Draw the background
            backgroundPaint.Color = backgroundColor.ToSKColor();
            canvas.DrawPath(_backgroundPath, backgroundPaint);

            // Draw outline for outlined button
            if (ButtonVariant == ButtonVariant.Outlined)
            {
                using (var outlinePaint = new SKPaint())
                {
                    outlinePaint.IsAntialias = true;
                    outlinePaint.Style = SKPaintStyle.Stroke;
                    outlinePaint.StrokeWidth = _scale;
                    outlinePaint.Color = IsDisabled
                        ? PrimaryColor.WithAlpha(0.12f).ToSKColor()
                        : PrimaryColor.ToSKColor();
                    canvas.DrawPath(_backgroundPath, outlinePaint);
                }
            }

            // Draw ripple effect
            if (_lastTouchLocation != SKPoint.Empty && _rippleAnimationPercentage > 0.0d && !IsDisabled)
            {
                using (var ripplePath = new SKPath())
                using (var ripplePaint = new SKPaint())
                {
                    ripplePaint.IsAntialias = true;
                    ripplePaint.Style = SKPaintStyle.Fill;

                    // Use primary color with opacity for the ripple
                    ripplePaint.Color = backgroundColor
                        .WithLuminosity(0.1f + (0.1f * (1 - (float)_rippleAnimationPercentage)))
                        .ToSKColor();

                    // Calculate ripple size
                    var startingRippleSize = Math.Min(info.Width, info.Height) * 0.5f;
                    var maxRippleSize = startingRippleSize +
                                        (float)(Math.Max(info.Width, info.Height) * _rippleAnimationPercentage);
                    var offsetAmount = -maxRippleSize / 2f;
                    var offsetPoint = new SKPoint(_lastTouchLocation.X + offsetAmount, _lastTouchLocation.Y + offsetAmount);
                    var rippleSize = SKRect.Create(offsetPoint, new SKSize(maxRippleSize, maxRippleSize));

                    ripplePath.AddOval(rippleSize);

                    // Constrain ripple to button shape
                    using (var finalRipple = ripplePath.Op(_backgroundPath, SKPathOp.Intersect))
                    {
                        canvas.DrawPath(finalRipple, ripplePaint);
                    }
                }
            }

            // Draw press state overlay
            if (_stateAnimationPercentage > 0.0 && !IsDisabled)
            {
                using (var statePaint = new SKPaint())
                {
                    statePaint.IsAntialias = true;
                    statePaint.Style = SKPaintStyle.Fill;
                    statePaint.Color = PrimaryColor
                        .WithAlpha(0.1f * (float)_stateAnimationPercentage)
                        .ToSKColor();
                    canvas.DrawPath(_backgroundPath, statePaint);
                }
            }

            // Draw text
            if (!string.IsNullOrEmpty(Text))
            {
                using (var textPaint = new SKPaint())
                {
                    textPaint.IsAntialias = true;
                    textPaint.Color = textColor.ToSKColor();
                    textPaint.TextSize = (float)FontSize * _scale;
                    textPaint.TextAlign = SKTextAlign.Center;

                    // Apply font family if specified
                    if (!string.IsNullOrEmpty(FontFamily))
                    {
                        textPaint.Typeface = SKTypeface.FromFamilyName(FontFamily);
                    }
                    else
                    {
                        textPaint.Typeface = PlatformInfo.DefaultTypeface;
                    }

                    // Measure text
                    var textBounds = default(SKRect);
                    textPaint.MeasureText(Text, ref textBounds);

                    // Draw centered text
                    float textX = rect.MidX;
                    float textY = rect.MidY - textBounds.MidY;

                    canvas.DrawText(Text, textX, textY, textPaint);
                }
            }
        }
    }

    protected override void OnTouch(SKTouchEventArgs e)
    {
        e.Handled = true;

        // Ignore touch events when disabled
        if (IsDisabled)
        {
            return;
        }

        // Check if touch is inside the button shape
        bool isTouchInside = _backgroundPath.Contains(e.Location.X, e.Location.Y);

        // Handle different touch events
        switch (e.ActionType)
        {
            case SKTouchAction.Pressed when isTouchInside:
                _lastTouchLocation = e.Location;
                Pressed = true;
                AnimateRipple();
                break;

            case SKTouchAction.Moved:
                Pressed = isTouchInside && e.InContact;
                break;

            case SKTouchAction.Released when isTouchInside && Pressed:
                Pressed = false;

                // Execute command and trigger clicked event
                if (Command?.CanExecute(CommandParameter) ?? false)
                {
                    Command.Execute(CommandParameter);
                }

                Clicked?.Invoke(this, EventArgs.Empty);
                break;

            case SKTouchAction.Cancelled:
            case SKTouchAction.Exited:
                Pressed = false;
                AnimateRipple(reset: true);
                break;

            default:
                Pressed = false;
                break;
        }
    }

    private void AnimateRipple(bool reset = false)
    {
        if (_lastTouchLocation == SKPoint.Empty)
        {
            return;
        }

        this.AbortAnimation(_rippleAnimationName);

        if (reset)
        {
            _rippleAnimationPercentage = 0d;
            this.InvalidateSurface();
            return;
        }

        var rippleAnimation = new Animation(x =>
        {
            _rippleAnimationPercentage = x;
            this.InvalidateSurface();
        });

        rippleAnimation.Commit(
            this,
            _rippleAnimationName,
            length: (uint)RippleAnimationDuration,
            easing: Easing.CubicOut,
            finished: (percent, isFinished) =>
            {
                _lastTouchLocation = SKPoint.Empty;
                _rippleAnimationPercentage = 0d;
                this.InvalidateSurface();
            });
    }

    private void AnimateStateChange(bool pressed, bool reset = false)
    {
        this.AbortAnimation(_stateAnimationName);

        if (reset)
        {
            _stateAnimationPercentage = 0d;
            this.InvalidateSurface();
            return;
        }

        var stateAnimation = new Animation(
            x =>
            {
                _stateAnimationPercentage = x;
                this.InvalidateSurface();
            },
            _stateAnimationPercentage,
            pressed ? 1 : 0);

        stateAnimation.Commit(
            this,
            _stateAnimationName,
            length: (uint)StateAnimationDuration,
            easing: pressed ? Easing.CubicOut : Easing.CubicIn,
            finished: (percent, isFinished) =>
            {
                _stateAnimationPercentage = pressed ? 1 : 0;
                this.InvalidateSurface();
            });
    }
}

/// <summary>
/// Defines the Material 3 button variants.
/// </summary>
public enum ButtonVariant
{
    Filled,
    Outlined,
    Elevated,
    Text,
}
