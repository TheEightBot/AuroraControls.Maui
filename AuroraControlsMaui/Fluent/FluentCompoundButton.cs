using System.Runtime.Intrinsics.X86;
using ExCSS;
using Svg.Skia;
using Topten.RichTextKit;
using Color = Microsoft.Maui.Graphics.Color;
using Colors = Microsoft.Maui.Graphics.Colors;

namespace AuroraControls.Fluent;

/// <summary>
/// Represents a Fluent 2 design style compound button component based on AuroraViewBase.
/// </summary>
#pragma warning disable CA1001 // Types that own disposable fields should be disposable
public class FluentCompoundButton : AuroraViewBase
#pragma warning restore CA1001 // Types that own disposable fields should be disposable
{
    private readonly SKPaint _backgroundPaint = new();
    private readonly SKPaint _borderPaint = new();
    private readonly SKPaint _textPaint = new();
    private readonly SKPaint _descriptionPaint = new();
    private readonly SKPaint _iconPaint = new();
    private readonly SKPaint _ripplePaint = new();
    private readonly SKPath _backgroundPath = new();

    private string _rippleAnimationName;
    private string _pressAnimationName;
    private double _rippleAnimationPercentage;
    private double _pressAnimationPercentage;
    private SKPoint _lastTouchLocation;

    private SKSvg? _svg;
    private bool _loading;

    /// <summary>
    /// The primary text property.
    /// </summary>
    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(FluentCompoundButton), string.Empty,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the primary text displayed on the button.
    /// </summary>
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>
    /// The description text property.
    /// </summary>
    public static readonly BindableProperty DescriptionProperty =
        BindableProperty.Create(nameof(Description), typeof(string), typeof(FluentCompoundButton), string.Empty,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the secondary description text displayed below the main text.
    /// </summary>
    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    /// <summary>
    /// The icon name property.
    /// </summary>
    public static readonly BindableProperty IconNameProperty =
        BindableProperty.Create(nameof(IconName), typeof(string), typeof(FluentCompoundButton), null,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the embedded SVG icon name.
    /// </summary>
    public string IconName
    {
        get => (string)GetValue(IconNameProperty);
        set => SetValue(IconNameProperty, value);
    }

    /// <summary>
    /// The icon position property.
    /// </summary>
    public static readonly BindableProperty IconPositionProperty =
        BindableProperty.Create(nameof(IconPosition), typeof(IconPosition), typeof(FluentCompoundButton),
            IconPosition.Before, propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the icon position relative to text.
    /// </summary>
    public IconPosition IconPosition
    {
        get => (IconPosition)GetValue(IconPositionProperty);
        set => SetValue(IconPositionProperty, value);
    }

    /// <summary>
    /// The appearance property.
    /// </summary>
    public static readonly BindableProperty AppearanceProperty =
        BindableProperty.Create(nameof(Appearance), typeof(ButtonAppearance), typeof(FluentCompoundButton),
            ButtonAppearance.Secondary, propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the button appearance style.
    /// </summary>
    public ButtonAppearance Appearance
    {
        get => (ButtonAppearance)GetValue(AppearanceProperty);
        set => SetValue(AppearanceProperty, value);
    }

    /// <summary>
    /// The shape property.
    /// </summary>
    public static readonly BindableProperty ShapeProperty =
        BindableProperty.Create(nameof(Shape), typeof(ButtonShape), typeof(FluentCompoundButton),
            ButtonShape.Rounded, propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the button shape style.
    /// </summary>
    public ButtonShape Shape
    {
        get => (ButtonShape)GetValue(ShapeProperty);
        set => SetValue(ShapeProperty, value);
    }

    /// <summary>
    /// The size property.
    /// </summary>
    public static readonly BindableProperty SizeProperty =
        BindableProperty.Create(nameof(Size), typeof(ButtonSize), typeof(FluentCompoundButton),
            ButtonSize.Medium, propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the button size.
    /// </summary>
    public ButtonSize Size
    {
        get => (ButtonSize)GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    /// <summary>
    /// The disabled property.
    /// </summary>
    public static readonly BindableProperty IsDisabledProperty =
        BindableProperty.Create(nameof(IsDisabled), typeof(bool), typeof(FluentCompoundButton), false,
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
    /// The loading property.
    /// </summary>
    public static readonly BindableProperty IsLoadingProperty =
        BindableProperty.Create(nameof(IsLoading), typeof(bool), typeof(FluentCompoundButton), false,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets a value indicating whether gets or sets whether the button is in loading state.
    /// </summary>
    public bool IsLoading
    {
        get => (bool)GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    /// <summary>
    /// The primary color property.
    /// </summary>
    public static readonly BindableProperty ColorProperty =
        BindableProperty.Create(nameof(Color), typeof(Color), typeof(FluentCompoundButton), null,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the primary color for the button.
    /// </summary>
    public Color Color
    {
        get => (Color)GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    /// <summary>
    /// The text color property.
    /// </summary>
    public static readonly BindableProperty TextColorProperty =
        BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(FluentCompoundButton), null,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the text color for the button.
    /// </summary>
    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    /// <summary>
    /// The description text color property.
    /// </summary>
    public static readonly BindableProperty DescriptionColorProperty =
        BindableProperty.Create(nameof(DescriptionColor), typeof(Color), typeof(FluentCompoundButton), null,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the description text color for the button.
    /// </summary>
    public Color DescriptionColor
    {
        get => (Color)GetValue(DescriptionColorProperty);
        set => SetValue(DescriptionColorProperty, value);
    }

    /// <summary>
    /// The icon color property.
    /// </summary>
    public static readonly BindableProperty IconColorProperty =
        BindableProperty.Create(nameof(IconColor), typeof(Color), typeof(FluentCompoundButton), null,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the icon color for the button.
    /// </summary>
    public Color IconColor
    {
        get => (Color)GetValue(IconColorProperty);
        set => SetValue(IconColorProperty, value);
    }

    /// <summary>
    /// The background color property.
    /// </summary>
    public static readonly BindableProperty BackgroundColorProperty =
        BindableProperty.Create(nameof(BackgroundColor), typeof(Color), typeof(FluentCompoundButton),
            Colors.Transparent,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the background color for the button.
    /// </summary>
    public new Color BackgroundColor
    {
        get => (Color)GetValue(BackgroundColorProperty);
        set => SetValue(BackgroundColorProperty, value);
    }

    /// <summary>
    /// The border color property.
    /// </summary>
    public static readonly BindableProperty BorderColorProperty =
        BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(FluentCompoundButton), null,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the border color for the button.
    /// </summary>
    public Color BorderColor
    {
        get => (Color)GetValue(BorderColorProperty);
        set => SetValue(BorderColorProperty, value);
    }

    /// <summary>
    /// The command property.
    /// </summary>
    public static readonly BindableProperty CommandProperty =
        BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(FluentCompoundButton), null);

    /// <summary>
    /// Gets or sets the command that is invoked when the button is tapped.
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
        BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(FluentCompoundButton), null);

    /// <summary>
    /// Gets or sets the parameter that is passed to Command.
    /// </summary>
    public object CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    /// <summary>
    /// The pressed property.
    /// </summary>
    public static readonly BindableProperty IsPressedProperty =
        BindableProperty.Create(nameof(IsPressed), typeof(bool), typeof(FluentCompoundButton), false,
            BindingMode.OneWayToSource);

    /// <summary>
    /// Gets a value indicating whether gets whether the button is currently being pressed.
    /// </summary>
    public bool IsPressed
    {
        get => (bool)GetValue(IsPressedProperty);
        private set => SetValue(IsPressedProperty, value);
    }

    /// <summary>
    /// The tapped event.
    /// </summary>
    public event EventHandler Tapped;

    /// <summary>
    /// Initializes a new instance of the <see cref="FluentCompoundButton"/> class.
    /// </summary>
    public FluentCompoundButton()
    {
        MinimumHeightRequest = IAuroraView.StandardControlHeight;
        _rippleAnimationName = $"Ripple_{Guid.NewGuid()}";
        _pressAnimationName = $"Press_{Guid.NewGuid()}";

        // Initialize the paints with default values
        _backgroundPaint.IsAntialias = true;
        _backgroundPaint.Style = SKPaintStyle.Fill;

        _borderPaint.IsAntialias = true;
        _borderPaint.Style = SKPaintStyle.Stroke;

        _textPaint.IsAntialias = true;
        _textPaint.TextAlign = SKTextAlign.Left;

        _descriptionPaint.IsAntialias = true;
        _descriptionPaint.TextAlign = SKTextAlign.Left;

        _iconPaint.IsAntialias = true;

        _ripplePaint.IsAntialias = true;
        _ripplePaint.Style = SKPaintStyle.Fill;
    }

    protected override void Attached()
    {
        this.EnableTouchEvents = true;

        if (!string.IsNullOrEmpty(IconName))
        {
            LoadSvgResource();
        }

        base.Attached();
    }

    protected override void Detached()
    {
        _svg?.Dispose();
        _svg = null;

        this.AbortAnimation(_rippleAnimationName);
        this.AbortAnimation(_pressAnimationName);

        base.Detached();
    }

    private void LoadSvgResource()
    {
        if (string.IsNullOrEmpty(IconName))
        {
            _svg?.Dispose();
            _svg = null;
            return;
        }

        try
        {
            _svg?.Dispose();
            _svg = new SKSvg();
            using var stream = EmbeddedResourceLoader.Load(IconName);
            if (stream != null)
            {
                _svg.Load(stream);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to load SVG: {ex.Message}");
            _svg?.Dispose();
            _svg = null;
        }
    }

    protected override void OnPropertyChanged(string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName == IconNameProperty.PropertyName && IsAttached)
        {
            LoadSvgResource();
        }
        else if (propertyName == IsLoadingProperty.PropertyName)
        {
            _loading = IsLoading;
            if (_loading)
            {
                // Start loading animation if we implement it
            }
        }
    }

    protected override void OnTouch(SKTouchEventArgs e)
    {
        e.Handled = true;

        if (IsDisabled || IsLoading)
        {
            return;
        }

        if (e.ActionType == SKTouchAction.Cancelled || e.ActionType == SKTouchAction.Exited)
        {
            IsPressed = false;
            AnimatePress(false);
            return;
        }

        if (e.ActionType == SKTouchAction.Pressed)
        {
            _lastTouchLocation = e.Location;
            IsPressed = true;
            AnimatePress(true);
            AnimateRipple();
        }

        if (e.ActionType == SKTouchAction.Released)
        {
            IsPressed = false;
            AnimatePress(false);

            // Detect if the release is within the button bounds
            var bounds = new SKRect(0, 0, CanvasSize.Width, CanvasSize.Height);
            if (bounds.Contains(e.Location))
            {
                Tapped?.Invoke(this, EventArgs.Empty);

                if (Command?.CanExecute(CommandParameter) ?? false)
                {
                    Command.Execute(CommandParameter);
                }
            }

            return;
        }
    }

    protected override void PaintControl(SKSurface surface, SKImageInfo info)
    {
        var canvas = surface.Canvas;
        canvas.Clear();

        float cornerRadius = 4 * _scale;
        float borderWidth = 1 * _scale;

        switch (Shape)
        {
            case ButtonShape.Square:
                cornerRadius = 0;
                break;
            case ButtonShape.Rounded:
                cornerRadius = 4 * _scale;
                break;
            case ButtonShape.Circular:
                cornerRadius = Math.Min(info.Width, info.Height) / 2;
                break;
        }

        // Configure button based on size
        float verticalPadding;
        float horizontalPadding;
        float iconSize;
        float primaryFontSize;
        float secondaryFontSize;
        float iconSpacing;

        switch (Size)
        {
            case ButtonSize.Small:
                verticalPadding = 4 * _scale;
                horizontalPadding = 12 * _scale;
                iconSize = 12 * _scale;
                primaryFontSize = 12 * _scale;
                secondaryFontSize = 10 * _scale;
                iconSpacing = 6 * _scale;
                break;
            case ButtonSize.Large:
                verticalPadding = 12 * _scale;
                horizontalPadding = 20 * _scale;
                iconSize = 20 * _scale;
                primaryFontSize = 16 * _scale;
                secondaryFontSize = 12 * _scale;
                iconSpacing = 12 * _scale;
                break;
            case ButtonSize.Medium:
            default:
                verticalPadding = 8 * _scale;
                horizontalPadding = 16 * _scale;
                iconSize = 16 * _scale;
                primaryFontSize = 14 * _scale;
                secondaryFontSize = 12 * _scale;
                iconSpacing = 8 * _scale;
                break;
        }

        // Get colors based on appearance, state and custom colors
        Color backgroundColor = Colors.Transparent;
        Color textColor = Colors.Black;
        Color descriptionColor = Colors.Gray;
        Color borderColor = Colors.Gray.WithAlpha(0.3f);
        Color iconColor = Colors.Black;

        // Apply custom colors if set
        if (Color != null)
        {
            switch (Appearance)
            {
                case ButtonAppearance.Accent:
                    backgroundColor = Color;
                    textColor = Colors.White;
                    descriptionColor = Colors.White.WithAlpha(0.9f);
                    iconColor = Colors.White;
                    borderColor = Color.WithAlpha(0.1f);
                    break;
                case ButtonAppearance.Outline:
                case ButtonAppearance.Subtle:
                case ButtonAppearance.Secondary:
                default:
                    textColor = Color;
                    descriptionColor = Color.WithAlpha(0.8f);
                    iconColor = Color;
                    borderColor = Color.WithAlpha(0.3f);
                    backgroundColor = Color.WithAlpha(0.1f);
                    break;
            }
        }
        else
        {
            // Default colors based on appearance
            switch (Appearance)
            {
                case ButtonAppearance.Accent:
                    backgroundColor = Color.FromArgb("#0078D4"); // Fluent primary blue
                    textColor = Colors.White;
                    descriptionColor = Colors.White.WithAlpha(0.9f);
                    iconColor = Colors.White;
                    break;
                case ButtonAppearance.Outline:
                    backgroundColor = Colors.Transparent;
                    textColor = Color.FromArgb("#201F1E"); // Fluent black
                    descriptionColor = Color.FromArgb("#605E5C"); // Fluent gray
                    iconColor = Color.FromArgb("#201F1E");
                    borderColor = Color.FromArgb("#8A8886"); // Fluent border
                    break;
                case ButtonAppearance.Subtle:
                    backgroundColor = Colors.Transparent;
                    textColor = Color.FromArgb("#201F1E");
                    descriptionColor = Color.FromArgb("#605E5C");
                    iconColor = Color.FromArgb("#201F1E");
                    borderColor = Colors.Transparent;
                    break;
                case ButtonAppearance.Secondary:
                default:
                    backgroundColor = Color.FromArgb("#F3F2F1"); // Fluent light gray
                    textColor = Color.FromArgb("#201F1E");
                    descriptionColor = Color.FromArgb("#605E5C");
                    iconColor = Color.FromArgb("#201F1E");
                    borderColor = Color.FromArgb("#F3F2F1");
                    break;
            }
        }

        // Apply custom colors if specified
        if (TextColor != null)
        {
            textColor = TextColor;
        }

        if (DescriptionColor != null)
        {
            descriptionColor = DescriptionColor;
        }

        if (IconColor != null)
        {
            iconColor = IconColor;
        }

        if (BorderColor != null)
        {
            borderColor = BorderColor;
        }

        if (BackgroundColor != Colors.Transparent)
        {
            backgroundColor = BackgroundColor;
        }

        // Adjust opacity for disabled state
        if (IsDisabled)
        {
            backgroundColor = backgroundColor.WithAlpha(0.3f);
            textColor = textColor.WithAlpha(0.3f);
            descriptionColor = descriptionColor.WithAlpha(0.3f);
            iconColor = iconColor.WithAlpha(0.3f);
            borderColor = borderColor.WithAlpha(0.3f);
        }

        // Adjust for pressed state
        if (IsPressed && !IsDisabled && !IsLoading)
        {
            backgroundColor = backgroundColor.AddLuminosity(-0.1f);
            borderColor = borderColor.AddLuminosity(-0.1f);
        }

        // Set paint colors
        _backgroundPaint.Color = backgroundColor.ToSKColor();
        _textPaint.Color = textColor.ToSKColor();
        _descriptionPaint.Color = descriptionColor.ToSKColor();
        _iconPaint.Color = iconColor.ToSKColor();
        _borderPaint.Color = borderColor.ToSKColor();
        _borderPaint.StrokeWidth = borderWidth;

        // Set font sizes
        _textPaint.TextSize = primaryFontSize;
        _textPaint.Typeface = PlatformInfo.DefaultTypeface;
        _descriptionPaint.TextSize = secondaryFontSize;
        _descriptionPaint.Typeface = PlatformInfo.DefaultTypeface;

        // Calculate layout
        var buttonRect = new SKRect(
            borderWidth / 2,
            borderWidth / 2,
            info.Width - (borderWidth / 2),
            info.Height - (borderWidth / 2));

        // Reset background path
        _backgroundPath.Reset();
        _backgroundPath.AddRoundRect(buttonRect, cornerRadius, cornerRadius);

        // Draw background
        canvas.DrawPath(_backgroundPath, _backgroundPaint);

        // Draw border if needed
        if (Appearance == ButtonAppearance.Outline || borderWidth > 0)
        {
            canvas.DrawPath(_backgroundPath, _borderPaint);
        }

        // Draw ripple effect if active
        if (_rippleAnimationPercentage > 0 && !IsDisabled && !IsLoading)
        {
            using (var ripplePath = new SKPath())
            {
                _ripplePaint.Color = textColor.WithAlpha(0.1f).ToSKColor();

                var startingRippleSize = Math.Min(info.Width, info.Height) * 0.75f;
                var maxRippleSize = startingRippleSize +
                                    (float)((Math.Max(info.Width, info.Height) * 0.4) * _rippleAnimationPercentage);

                var offsetAmount = -maxRippleSize / 2f;
                var offsetPoint = new SKPoint(
                    _lastTouchLocation.X + offsetAmount,
                    _lastTouchLocation.Y + offsetAmount);

                var rippleRect = SKRect.Create(
                    offsetPoint,
                    new SKSize(maxRippleSize, maxRippleSize));

                ripplePath.AddOval(rippleRect);

                using (var finalRipple = ripplePath.Op(_backgroundPath, SKPathOp.Intersect))
                {
                    canvas.DrawPath(finalRipple, _ripplePaint);
                }
            }
        }

        // Set content area with padding
        var contentRect = new SKRect(
            buttonRect.Left + horizontalPadding,
            buttonRect.Top + verticalPadding,
            buttonRect.Right - horizontalPadding,
            buttonRect.Bottom - verticalPadding);

        // Calculate icon position and size if needed
        float iconLeft = 0;
        float iconTop = 0;
        bool hasIcon = _svg != null && _svg.Picture != null;
        float svgScaleFactor =
            hasIcon
                ? iconSize / Math.Max(_svg.Picture.CullRect.Width, _svg.Picture.CullRect.Height)
                : 0f;

        float scaledIconWidth =
            hasIcon
                ? _svg.Picture.CullRect.Width * svgScaleFactor
                : 0f;

        // Calculate text metrics
        var textBounds = default(SKRect);
        _textPaint.MeasureText(Text, ref textBounds);
        if (hasIcon)
        {
            textBounds.Right -= _svg.Picture.CullRect.Width * svgScaleFactor;
        }

        var descriptionBounds = default(SKRect);
        if (!string.IsNullOrEmpty(Description))
        {
            _descriptionPaint.MeasureText(Description, ref descriptionBounds);

            if (hasIcon)
            {
                descriptionBounds.Right -= _svg.Picture.CullRect.Width * svgScaleFactor;
            }
        }

        // Vertical centering calculations
        float totalTextHeight = textBounds.Height;
        if (!string.IsNullOrEmpty(Description))
        {
            totalTextHeight += descriptionBounds.Height + (2 * _scale); // Spacing between text and description
        }

        float textTop = contentRect.MidY - (totalTextHeight / 2);
        float descriptionTop = textTop + textBounds.Height + (2 * _scale);

        // Horizontal positioning based on icon position
        float textLeft;

        if (hasIcon)
        {
            if (IconPosition == IconPosition.Before)
            {
                iconLeft = contentRect.Left;
                textLeft = iconLeft + iconSize + iconSpacing;

                // Center icon vertically
                iconTop = contentRect.MidY - (iconSize / 2);
            }

            // IconPosition.After
            else
            {
                textLeft = contentRect.Left;
                iconLeft = textLeft + Math.Max(textBounds.Width, descriptionBounds.Width) + iconSpacing;

                // Center icon vertically
                iconTop = contentRect.MidY - (iconSize / 2);
            }

            // Draw icon
            var iconMatrix = SKMatrix.CreateScaleTranslation(
                svgScaleFactor, svgScaleFactor,
                iconLeft, iconTop);

            canvas.DrawPicture(_svg.Picture, ref iconMatrix, _iconPaint);
        }
        else
        {
            textLeft = contentRect.Left;
        }

        // Check if in loading state
        if (IsLoading)
        {
            // Draw a simple loading spinner (in a real implementation, you might use a proper spinner control)
            float spinnerSize = iconSize;
            float spinnerStrokeWidth = spinnerSize * 0.1f;
            float spinnerRadius = (spinnerSize / 2) - (spinnerStrokeWidth / 2);

            float spinnerLeft = textLeft - spinnerSize - iconSpacing;
            float spinnerTop = contentRect.MidY - (spinnerSize / 2);

            using var spinnerPaint = new SKPaint
            {
                Color = textColor.ToSKColor(),
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = spinnerStrokeWidth,
                StrokeCap = SKStrokeCap.Round,
            };

            float spinnerAngle = (float)((DateTimeOffset.Now.ToUnixTimeMilliseconds() % 2000) / 2000.0 * 360);

            canvas.DrawArc(
                new SKRect(
                    spinnerLeft,
                    spinnerTop,
                    spinnerLeft + spinnerSize,
                    spinnerTop + spinnerSize),
                spinnerAngle,
                270,
                false,
                spinnerPaint);

            // Shift text to make room for spinner
            textLeft = spinnerLeft + spinnerSize + iconSpacing;
        }

        // Draw primary text
        if (!string.IsNullOrEmpty(Text))
        {
            canvas.DrawText(Text, textLeft, textTop - textBounds.Top, _textPaint);
        }

        // Draw description text
        if (!string.IsNullOrEmpty(Description))
        {
            // canvas.DrawText(Description, textLeft, descriptionTop - descriptionBounds.Top, _descriptionPaint);
            var descriptionText =
                new RichString()
                    .Add(
                        Description,
                        textColor: this._descriptionPaint.Color,
                        fontSize: _descriptionPaint.TextSize)
                    .Alignment(Topten.RichTextKit.TextAlignment.Left);
            descriptionText.EllipsisEnabled = true;
            descriptionText.MaxWidth = info.Width - scaledIconWidth;
            descriptionText.MaxLines = 1;

            var descriptionTextDrawPoint = new SKPoint(textLeft, descriptionTop);

            descriptionText.Paint(canvas, descriptionTextDrawPoint);
        }
    }

    private void AnimateRipple()
    {
        this.AbortAnimation(_rippleAnimationName);
        _rippleAnimationPercentage = 0;

        var rippleAnimation = new Animation(
            callback: v =>
            {
                _rippleAnimationPercentage = v;
                this.InvalidateSurface();
            },
            start: 0,
            end: 1,
            easing: Easing.SinOut);

        rippleAnimation.Commit(
            owner: this,
            name: _rippleAnimationName,
            length: 500,
            finished: (v, c) =>
            {
                _rippleAnimationPercentage = 0;
                this.InvalidateSurface();
            });
    }

    private void AnimatePress(bool pressed)
    {
        this.AbortAnimation(_pressAnimationName);

        var pressAnimation = new Animation(
            callback: v =>
            {
                _pressAnimationPercentage = v;
                this.InvalidateSurface();
            },
            start: _pressAnimationPercentage,
            end: pressed ? 1 : 0,
            easing: pressed ? Easing.CubicOut : Easing.CubicIn);

        pressAnimation.Commit(
            owner: this,
            name: _pressAnimationName,
            length: 150,
            finished: (v, c) =>
            {
                _pressAnimationPercentage = pressed ? 1 : 0;
                this.InvalidateSurface();
            });
    }
}

/// <summary>
/// Defines button appearance styles.
/// </summary>
public enum ButtonAppearance
{
    /// <summary>
    /// Accent appearance (solid primary color).
    /// </summary>
    Accent,

    /// <summary>
    /// Secondary appearance (neutral background).
    /// </summary>
    Secondary,

    /// <summary>
    /// Outline appearance (border only).
    /// </summary>
    Outline,

    /// <summary>
    /// Subtle appearance (no border, no background).
    /// </summary>
    Subtle,
}

/// <summary>
/// Defines button shape styles.
/// </summary>
public enum ButtonShape
{
    /// <summary>
    /// Rounded corners (default).
    /// </summary>
    Rounded,

    /// <summary>
    /// Square corners.
    /// </summary>
    Square,

    /// <summary>
    /// Circular shape.
    /// </summary>
    Circular,
}

/// <summary>
/// Defines button sizes.
/// </summary>
public enum ButtonSize
{
    /// <summary>
    /// Small button size.
    /// </summary>
    Small,

    /// <summary>
    /// Medium button size.
    /// </summary>
    Medium,

    /// <summary>
    /// Large button size.
    /// </summary>
    Large,
}

/// <summary>
/// Defines icon position.
/// </summary>
public enum IconPosition
{
    /// <summary>
    /// Icon appears before text.
    /// </summary>
    Before,

    /// <summary>
    /// Icon appears after text.
    /// </summary>
    After,
}
