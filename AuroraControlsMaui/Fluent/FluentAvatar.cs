using System.Text;
using Svg.Skia;

namespace AuroraControls;

/// <summary>
/// Represents a Fluent 2 design style avatar component based on AuroraViewBase.
/// </summary>
#pragma warning disable CA1001 // Types that own disposable fields should be disposable
public class FluentAvatar : AuroraViewBase
#pragma warning restore CA1001 // Types that own disposable fields should be disposable
{
    private readonly SKPaint _backgroundPaint = new();
    private readonly SKPaint _textPaint = new();
    private readonly SKPaint _iconPaint = new();
    private readonly SKPaint _outlinePaint = new();
    private readonly SKPaint _presencePaint = new();
    private readonly SKPaint _badgePaint = new();
    private readonly SKPaint _maskPaint = new();
    private readonly SKPath _clipPath = new();
    private readonly SKPath _presencePath = new();
    private readonly SKPath _badgePath = new();

    private SKImage _cachedImage;
    private string _initials;

    /// <summary>
    /// The size property.
    /// </summary>
    public static readonly BindableProperty SizeProperty =
        BindableProperty.Create(nameof(Size), typeof(AvatarSize), typeof(FluentAvatar), AvatarSize.Medium,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the avatar size.
    /// </summary>
    public AvatarSize Size
    {
        get => (AvatarSize)GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    /// <summary>
    /// The shape property.
    /// </summary>
    public static readonly BindableProperty ShapeProperty =
        BindableProperty.Create(nameof(Shape), typeof(AvatarShape), typeof(FluentAvatar), AvatarShape.Circle,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the avatar shape.
    /// </summary>
    public AvatarShape Shape
    {
        get => (AvatarShape)GetValue(ShapeProperty);
        set => SetValue(ShapeProperty, value);
    }

    /// <summary>
    /// The name property.
    /// </summary>
    public static readonly BindableProperty NameProperty =
        BindableProperty.Create(nameof(Name), typeof(string), typeof(FluentAvatar), string.Empty,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                if (bindable is FluentAvatar avatar && newValue is string name)
                {
                    avatar._initials = avatar.GenerateInitials(name);
                    IAuroraView.PropertyChangedInvalidateSurface(bindable, oldValue, newValue);
                }
            });

    /// <summary>
    /// Gets or sets the name used to generate initials when no image is present.
    /// </summary>
    public string Name
    {
        get => (string)GetValue(NameProperty);
        set => SetValue(NameProperty, value);
    }

    /// <summary>
    /// The image source property.
    /// </summary>
    public static readonly BindableProperty ImageSourceProperty =
        BindableProperty.Create(nameof(ImageSource), typeof(ImageSource), typeof(FluentAvatar), null,
            propertyChanged: async (bindable, oldValue, newValue) =>
            {
                if (bindable is FluentAvatar avatar && newValue is ImageSource source)
                {
                    avatar._cachedImage?.Dispose();
                    avatar._cachedImage = null;

                    if (source != null)
                    {
                        avatar._cachedImage = await source.ImageFromSource();
                    }

                    IAuroraView.PropertyChangedInvalidateSurface(bindable, oldValue, newValue);
                }
            });

    /// <summary>
    /// Gets or sets the image source.
    /// </summary>
    public ImageSource ImageSource
    {
        get => (ImageSource)GetValue(ImageSourceProperty);
        set => SetValue(ImageSourceProperty, value);
    }

    /// <summary>
    /// The SVG icon name property.
    /// </summary>
    public static readonly BindableProperty IconNameProperty =
        BindableProperty.Create(nameof(IconName), typeof(string), typeof(FluentAvatar), null,
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
    /// The background color property.
    /// </summary>
    public static readonly BindableProperty BackgroundColorProperty =
        BindableProperty.Create(nameof(BackgroundColor), typeof(Color), typeof(FluentAvatar), Colors.LightGray,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the avatar background color.
    /// </summary>
    public Color BackgroundColor
    {
        get => (Color)GetValue(BackgroundColorProperty);
        set => SetValue(BackgroundColorProperty, value);
    }

    /// <summary>
    /// The foreground (text) color property.
    /// </summary>
    public static readonly BindableProperty ForegroundColorProperty =
        BindableProperty.Create(nameof(ForegroundColor), typeof(Color), typeof(FluentAvatar), Colors.Black,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the foreground (text) color.
    /// </summary>
    public Color ForegroundColor
    {
        get => (Color)GetValue(ForegroundColorProperty);
        set => SetValue(ForegroundColorProperty, value);
    }

    /// <summary>
    /// The active property indicating if the avatar should be highlighted.
    /// </summary>
    public static readonly BindableProperty IsActiveProperty =
        BindableProperty.Create(nameof(IsActive), typeof(bool), typeof(FluentAvatar), false,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets a value indicating whether gets or sets whether the avatar is active/highlighted.
    /// </summary>
    public bool IsActive
    {
        get => (bool)GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    /// <summary>
    /// The presence property.
    /// </summary>
    public static readonly BindableProperty PresenceProperty =
        BindableProperty.Create(nameof(Presence), typeof(AvatarPresence), typeof(FluentAvatar), AvatarPresence.None,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the presence indicator.
    /// </summary>
    public AvatarPresence Presence
    {
        get => (AvatarPresence)GetValue(PresenceProperty);
        set => SetValue(PresenceProperty, value);
    }

    /// <summary>
    /// The out-of-office property.
    /// </summary>
    public static readonly BindableProperty IsOutOfOfficeProperty =
        BindableProperty.Create(nameof(IsOutOfOffice), typeof(bool), typeof(FluentAvatar), false,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets a value indicating whether gets or sets whether the avatar should display an out of office indicator.
    /// </summary>
    public bool IsOutOfOffice
    {
        get => (bool)GetValue(IsOutOfOfficeProperty);
        set => SetValue(IsOutOfOfficeProperty, value);
    }

    /// <summary>
    /// The badge text property.
    /// </summary>
    public static readonly BindableProperty BadgeTextProperty =
        BindableProperty.Create(nameof(BadgeText), typeof(string), typeof(FluentAvatar), null,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the badge text (for notification count, etc).
    /// </summary>
    public string BadgeText
    {
        get => (string)GetValue(BadgeTextProperty);
        set => SetValue(BadgeTextProperty, value);
    }

    /// <summary>
    /// The font family property.
    /// </summary>
    public static readonly BindableProperty FontFamilyProperty =
        BindableProperty.Create(nameof(FontFamily), typeof(string), typeof(FluentAvatar), null,
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
    /// The custom color scheme index property.
    /// </summary>
    public static readonly BindableProperty ColorSchemeIndexProperty =
        BindableProperty.Create(nameof(ColorSchemeIndex), typeof(int), typeof(FluentAvatar), -1,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the custom color scheme index.
    /// </summary>
    public int ColorSchemeIndex
    {
        get => (int)GetValue(ColorSchemeIndexProperty);
        set => SetValue(ColorSchemeIndexProperty, value);
    }

    /// <summary>
    /// The custom ring color property.
    /// </summary>
    public static readonly BindableProperty RingColorProperty =
        BindableProperty.Create(nameof(RingColor), typeof(Color), typeof(FluentAvatar), Colors.Transparent,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the color of the ring around the avatar.
    /// </summary>
    public Color RingColor
    {
        get => (Color)GetValue(RingColorProperty);
        set => SetValue(RingColorProperty, value);
    }

    /// <summary>
    /// The auto-size property, determines if the control should automatically size itself
    /// to fit all content without clipping.
    /// </summary>
    public static readonly BindableProperty AutoSizeProperty =
        BindableProperty.Create(nameof(AutoSize), typeof(bool), typeof(FluentAvatar), true,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets a value indicating whether the control should automatically size itself.
    /// </summary>
    public bool AutoSize
    {
        get => (bool)GetValue(AutoSizeProperty);
        set => SetValue(AutoSizeProperty, value);
    }

    private Size _requiredSize;

    /// <summary>
    /// Initializes a new instance of the <see cref="FluentAvatar"/> class.
    /// </summary>
    public FluentAvatar()
    {
        MinimumHeightRequest = IAuroraView.StandardControlHeight;
        MinimumWidthRequest = IAuroraView.StandardControlHeight;

        // Initialize the paints with default values
        _textPaint.IsAntialias = true;
        _textPaint.TextAlign = SKTextAlign.Center;

        _backgroundPaint.IsAntialias = true;
        _backgroundPaint.Style = SKPaintStyle.Fill;

        _iconPaint.IsAntialias = true;

        _outlinePaint.IsAntialias = true;
        _outlinePaint.Style = SKPaintStyle.Stroke;

        _presencePaint.IsAntialias = true;
        _presencePaint.Style = SKPaintStyle.Fill;

        _badgePaint.IsAntialias = true;
        _badgePaint.Style = SKPaintStyle.Fill;

        _maskPaint.IsAntialias = true;
        _maskPaint.BlendMode = SKBlendMode.DstIn;

        SizeChanged += OnSizeChanged;
    }

    private void OnSizeChanged(object sender, EventArgs e)
    {
        InvalidateMeasure();
    }

    protected override void OnPropertyChanged(string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        // For properties that might affect required size, recalculate and update
        if (propertyName == SizeProperty.PropertyName ||
            propertyName == ShapeProperty.PropertyName ||
            propertyName == PresenceProperty.PropertyName ||
            propertyName == BadgeTextProperty.PropertyName ||
            propertyName == IsActiveProperty.PropertyName)
        {
            CalculateRequiredSize();
            UpdateControlSize();
        }
    }

    protected override void Attached()
    {
        base.Attached();

        // Calculate initial size when the control is attached
        CalculateRequiredSize();
        UpdateControlSize();
    }

    protected override void Detached()
    {
        _cachedImage?.Dispose();
        _cachedImage = null;
        SizeChanged -= OnSizeChanged;
        base.Detached();
    }

    /// <summary>
    /// Calculates the size required to display all elements without clipping.
    /// </summary>
    private void CalculateRequiredSize()
    {
        // Get base avatar diameter
        float avatarDiameter = GetSizeDiameter();
        float avatarRadius = avatarDiameter / 2;
        float strokeWidth = 2f;

        // Calculate space needed for the presence indicator
        float presencePadding = 0;
        if (Presence != AvatarPresence.None)
        {
            float presenceSize = avatarRadius * 0.35f;

            // The presence indicator extends beyond the avatar by its radius
            presencePadding = presenceSize;
        }

        // Calculate space needed for the badge
        float badgePadding = 0;
        if (!string.IsNullOrEmpty(BadgeText))
        {
            float badgeSize = avatarRadius * 0.35f;

            // The badge extends beyond the avatar by its radius
            badgePadding = badgeSize;
        }

        // Calculate space needed for the ring/outline
        float outlinePadding = 0;
        if (IsActive || RingColor != Colors.Transparent)
        {
            outlinePadding = strokeWidth;
        }

        // The total width and height needed is the avatar diameter plus all the additional padding
        float totalWidth = avatarDiameter + (2 * Math.Max(presencePadding, badgePadding)) + (2 * outlinePadding);
        float totalHeight = totalWidth; // Keep it square for now

        _requiredSize = new Size(totalWidth, totalHeight);
    }

    /// <summary>
    /// Updates the control's size based on calculated requirements if AutoSize is enabled.
    /// </summary>
    private void UpdateControlSize()
    {
        if (AutoSize && _requiredSize.Width > 0 && _requiredSize.Height > 0)
        {
            // Only set if different to avoid layout cycles
            if (Math.Abs(WidthRequest - _requiredSize.Width) > 0.1 ||
                Math.Abs(HeightRequest - _requiredSize.Height) > 0.1)
            {
                WidthRequest = _requiredSize.Width;
                HeightRequest = _requiredSize.Height;
            }
        }
    }

    protected override void PaintControl(SKSurface surface, SKImageInfo info)
    {
        var canvas = surface.Canvas;
        canvas.Clear();

        // Calculate dimensions based on avatar size
        float baseDiameter = GetSizeDiameter() * _scale;
        float baseRadius = baseDiameter / 2f;
        float strokeWidth = 2f * _scale;

        // Get the actual available space to draw
        float availableWidth = info.Width;
        float availableHeight = info.Height;

        // Determine the maximum size we can use while maintaining the aspect ratio
        float actualDiameter = Math.Min(
            availableWidth - (strokeWidth * 2f),
            availableHeight - (strokeWidth * 2f));

        // Scale the radius accordingly
        float actualRadius = actualDiameter / 2;

        // Center the avatar in the available space
        float centerX = availableWidth / 2f;
        float centerY = availableHeight / 2f;

        // Create the clipping path based on shape
        _clipPath.Reset();
        if (Shape == AvatarShape.Circle)
        {
            _clipPath.AddCircle(centerX, centerY, actualRadius);
        }
        else if (Shape == AvatarShape.Square)
        {
            float cornerRadius = actualDiameter * 0.1f; // 10% corner radius for slightly rounded square
            _clipPath.AddRoundRect(
                new SKRect(centerX - actualRadius, centerY - actualRadius, centerX + actualRadius,
                    centerY + actualRadius),
                cornerRadius, cornerRadius);
        }

        // Set colors based on color scheme or use custom colors
        Color backgroundColor = BackgroundColor;
        Color foregroundColor = ForegroundColor;
        if (ColorSchemeIndex >= 0 && ColorSchemeIndex < AvatarColorSchemes.Schemes.Length)
        {
            var scheme = AvatarColorSchemes.Schemes[ColorSchemeIndex];
            backgroundColor = scheme.BackgroundColor;
            foregroundColor = scheme.ForegroundColor;
        }
        else if (string.IsNullOrEmpty(Name) == false && ColorSchemeIndex == -1)
        {
            // Generate a deterministic color based on the name if no custom index is set
            int nameHash = Name.GetHashCode();
            int colorIndex = Math.Abs(nameHash) % AvatarColorSchemes.Schemes.Length;
            var scheme = AvatarColorSchemes.Schemes[colorIndex];
            backgroundColor = scheme.BackgroundColor;
            foregroundColor = scheme.ForegroundColor;
        }

        // Draw background
        _backgroundPaint.Color = backgroundColor.ToSKColor();
        canvas.DrawPath(_clipPath, _backgroundPaint);

        using (new SKAutoCanvasRestore(canvas))
        {
            // Clip to the avatar shape
            canvas.ClipPath(_clipPath, SKClipOperation.Intersect, true);

            // Draw image if available
            if (_cachedImage != null)
            {
                // Scale image to fit
                var imageRect = new SKRect(centerX - actualRadius, centerY - actualRadius, centerX + actualRadius,
                    centerY + actualRadius);
                canvas.DrawImage(_cachedImage, imageRect);
            }
            else if (!string.IsNullOrEmpty(IconName))
            {
                // Draw icon
                DrawIcon(canvas, centerX, centerY, actualDiameter * 0.6f);
            }
            else if (!string.IsNullOrEmpty(_initials))
            {
                // Draw initials
                _textPaint.Color = foregroundColor.ToSKColor();

                // Set font size based on avatar size
                float fontSize = actualDiameter * 0.4f; // 40% of diameter

                _textPaint.TextSize = fontSize;

                // Set font family if specified
                if (!string.IsNullOrEmpty(FontFamily))
                {
                    _textPaint.Typeface = SKTypeface.FromFamilyName(FontFamily);
                }
                else
                {
                    _textPaint.Typeface = PlatformInfo.DefaultTypeface;
                }

                // Center the text vertically
                var textBounds = default(SKRect);
                _textPaint.MeasureText(_initials, ref textBounds);
                float textY = centerY - textBounds.MidY;
                canvas.DrawText(_initials, centerX, textY, _textPaint);
            }
        }

        // Draw ring/outline if active or ring color is set
        if (IsActive || RingColor != Colors.Transparent)
        {
            _outlinePaint.StrokeWidth = strokeWidth;
            _outlinePaint.Color = (RingColor != Colors.Transparent ? RingColor : Colors.Blue).ToSKColor();
            canvas.DrawPath(_clipPath, _outlinePaint);
        }

        // Draw presence indicator if specified
        if (Presence != AvatarPresence.None)
        {
            DrawPresenceIndicator(canvas, centerX, centerY, actualRadius, strokeWidth);
        }

        // Draw badge if specified
        if (!string.IsNullOrEmpty(BadgeText))
        {
            DrawBadge(canvas, centerX, centerY, actualRadius, BadgeText);
        }
    }

    private void DrawPresenceIndicator(SKCanvas canvas, float centerX, float centerY, float avatarRadius,
        float strokeWidth)
    {
        float presenceSize = avatarRadius * 0.25f; // 25% of avatar radius

        // Position at bottom right
        float presenceX = centerX + avatarRadius - (presenceSize * 0.85f) - strokeWidth;
        float presenceY = centerY + avatarRadius - (presenceSize * 0.85f) - strokeWidth;

        _presencePath.Reset();
        _presencePath.AddCircle(presenceX, presenceY, presenceSize);

        // Set presence color
        SKColor presenceColor;
        SKColor ringColor = Colors.White.ToSKColor();

        switch (Presence)
        {
            case AvatarPresence.Available:
                presenceColor = Colors.Green.ToSKColor();
                break;
            case AvatarPresence.Away:
                presenceColor = Colors.Orange.ToSKColor();
                break;
            case AvatarPresence.Busy:
                presenceColor = Colors.Red.ToSKColor();
                break;
            case AvatarPresence.DoNotDisturb:
                presenceColor = Colors.DarkRed.ToSKColor();
                break;
            case AvatarPresence.Offline:
                presenceColor = Colors.Gray.ToSKColor();
                break;
            default:
                return; // Don't draw if None
        }

        // Draw background ring
        _outlinePaint.Color = ringColor;
        _outlinePaint.StrokeWidth = strokeWidth;
        canvas.DrawCircle(presenceX, presenceY, presenceSize, _outlinePaint);

        // Draw presence circle
        _presencePaint.Color = presenceColor;
        canvas.DrawPath(_presencePath, _presencePaint);

        // If Out of Office, draw the OOF indicator
        if (IsOutOfOffice && Presence != AvatarPresence.Offline)
        {
            // Draw a small "clock" symbol or diagonal line
            _outlinePaint.Color = ringColor;
            _outlinePaint.StrokeWidth = presenceSize * 0.15f;

            // Draw diagonal line for OOF
            canvas.DrawLine(
                presenceX - (presenceSize * 0.5f),
                presenceY - (presenceSize * 0.5f),
                presenceX + (presenceSize * 0.5f),
                presenceY + (presenceSize * 0.5f),
                _outlinePaint);
        }

        // For Do Not Disturb, draw the line through the circle
        if (Presence == AvatarPresence.DoNotDisturb)
        {
            _outlinePaint.Color = ringColor;
            _outlinePaint.StrokeWidth = presenceSize * 0.3f;
            canvas.DrawLine(
                presenceX - (presenceSize * 0.5f),
                presenceY,
                presenceX + (presenceSize * 0.5f),
                presenceY,
                _outlinePaint);
        }
    }

    private void DrawBadge(SKCanvas canvas, float centerX, float centerY, float avatarRadius, string badgeText)
    {
        // Determine badge size - adapt based on content length
        float badgeRadius = avatarRadius * 0.35f;

        // Position at top right
        float badgeX = centerX + avatarRadius - (badgeRadius * 0.85f);
        float badgeY = centerY - avatarRadius + (badgeRadius * 0.85f);

        // Create badge path
        _badgePath.Reset();
        _badgePath.AddCircle(badgeX, badgeY, badgeRadius);

        // Draw badge background
        _badgePaint.Color = Colors.Red.ToSKColor();
        canvas.DrawPath(_badgePath, _badgePaint);

        // Draw badge text
        _textPaint.Color = Colors.White.ToSKColor();
        _textPaint.TextSize = badgeRadius * 1.2f;
        _textPaint.TextAlign = SKTextAlign.Center;

        // Center the text vertically
        var textBounds = default(SKRect);
        _textPaint.MeasureText(badgeText, ref textBounds);
        float textY = badgeY - textBounds.MidY;

        canvas.DrawText(badgeText, badgeX, textY, _textPaint);
    }

    private void DrawIcon(SKCanvas canvas, float centerX, float centerY, float size)
    {
        // Implementation will depend on how SVG resources are handled in the project
        // This is a placeholder for the actual implementation
        using (var svg = new SKSvg())
        {
            try
            {
                using var stream = EmbeddedResourceLoader.Load(IconName);

                if (stream != null)
                {
                    svg.Load(stream);

                    if (svg.Picture != null)
                    {
                        // Calculate scaling to fit in the specified size
                        float scaleX = size / svg.Picture.CullRect.Width;
                        float scaleY = size / svg.Picture.CullRect.Height;
                        float scale = Math.Min(scaleX, scaleY);

                        var matrix = SKMatrix.CreateScale(scale, scale);

                        // Center the icon
                        float transX = centerX - (svg.Picture.CullRect.Width * scale / 2);
                        float transY = centerY - (svg.Picture.CullRect.Height * scale / 2);
                        matrix = matrix.PostConcat(SKMatrix.CreateTranslation(transX, transY));

                        // Set icon color
                        _iconPaint.Color = ForegroundColor.ToSKColor();
                        canvas.DrawPicture(svg.Picture, ref matrix, _iconPaint);
                    }
                }
            }
            catch (Exception)
            {
                // Log or handle the error as needed
            }
        }
    }

    private float GetSizeDiameter()
    {
        return Size switch
        {
            AvatarSize.Tiny => 16,
            AvatarSize.ExtraSmall => 24,
            AvatarSize.Small => 32,
            AvatarSize.Medium => 40,
            AvatarSize.Large => 56,
            AvatarSize.ExtraLarge => 72,
            AvatarSize.Huge => 96,
            _ => 40, // Default to Medium
        };
    }

    private string GenerateInitials(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return string.Empty;
        }

        var initials = new StringBuilder();
        var nameParts = name.Split(new[] { ' ', '.', '-', '_', ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

        if (nameParts.Length == 0)
        {
            return string.Empty;
        }

        // Add first letter of first name
        if (nameParts[0].Length > 0)
        {
            initials.Append(char.ToUpperInvariant(nameParts[0][0]));
        }

        // Add first letter of last name if available
        if (nameParts.Length > 1 && nameParts[^1].Length > 0)
        {
            initials.Append(char.ToUpperInvariant(nameParts[^1][0]));
        }
        else if (nameParts[0].Length > 1)
        {
            initials.Append(char.ToUpperInvariant(nameParts[0][1])); // Use first two letters of single name
        }

        return initials.ToString();
    }
}

/// <summary>
/// Defines avatar size options.
/// </summary>
public enum AvatarSize
{
    Tiny,
    ExtraSmall,
    Small,
    Medium,
    Large,
    ExtraLarge,
    Huge,
}

/// <summary>
/// Defines avatar shape options.
/// </summary>
public enum AvatarShape
{
    Circle,
    Square,
}

/// <summary>
/// Defines avatar presence status options.
/// </summary>
public enum AvatarPresence
{
    None,
    Available,
    Away,
    Busy,
    DoNotDisturb,
    Offline,
}

/// <summary>
/// Provides predefined color schemes for avatars.
/// </summary>
public static class AvatarColorSchemes
{
    public static readonly (Color BackgroundColor, Color ForegroundColor)[] Schemes =
    {
        (Color.FromArgb("#4F6BED"), Colors.White), (Color.FromArgb("#D83B01"), Colors.White),
        (Color.FromArgb("#217346"), Colors.White), (Color.FromArgb("#8764B8"), Colors.White),
        (Color.FromArgb("#038387"), Colors.White), (Color.FromArgb("#881798"), Colors.White),
        (Color.FromArgb("#0078D4"), Colors.White), (Color.FromArgb("#CA5010"), Colors.White),
        (Color.FromArgb("#8E562E"), Colors.White), (Color.FromArgb("#107C10"), Colors.White),
        (Color.FromArgb("#C239B3"), Colors.White), (Color.FromArgb("#00B7C3"), Colors.White),
    };
}
