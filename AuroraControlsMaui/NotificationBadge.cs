namespace AuroraControls;

[ContentProperty(nameof(NotificationCount))]
#pragma warning disable CA1001 // Types that own disposable fields should be disposable
public class NotificationBadge : AuroraViewBase
#pragma warning restore CA1001 // Types that own disposable fields should be disposable
{
    private SKPaint _badgePaint;
    private SKPaint _fontPaint;

    public event EventHandler<ValueChangedEventArgs> NotificationCountChanged;

    /// <summary>
    /// The color of the badge.
    /// </summary>
    public static readonly BindableProperty BadgeColorProperty =
        BindableProperty.Create(nameof(BadgeColor), typeof(Color), typeof(NotificationBadge), Colors.Red.MultiplyAlpha(.8f),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the badge color.
    /// </summary>
    /// <value>Expects a Xamarin.Forms.Color. Default value is Color.Red.</value>
    public Color BadgeColor
    {
        get => (Color)GetValue(BadgeColorProperty);
        set => SetValue(BadgeColorProperty, value);
    }

    /// <summary>
    /// The font color.
    /// </summary>
    public static readonly BindableProperty FontColorProperty =
        BindableProperty.Create(nameof(FontColor), typeof(Color), typeof(NotificationBadge), Colors.White,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the font color.
    /// </summary>
    /// <value>Expects a Xamarin.Forms.Color. Default value is Color.White.</value>
    public Color FontColor
    {
        get => (Color)GetValue(FontColorProperty);
        set => SetValue(FontColorProperty, value);
    }

    /// <summary>
    /// The symbol for when there are too many notifications.
    /// </summary>
    public static readonly BindableProperty TooManyNotificationsSymbolProperty =
        BindableProperty.Create(nameof(TooManyNotificationsSymbol), typeof(string), typeof(NotificationBadge), "∞",
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the symbol for too many notifications.
    /// </summary>
    /// <value>Symbol to display as a string. Default is infinity symbol (∞).</value>
    public string TooManyNotificationsSymbol
    {
        get => (string)GetValue(TooManyNotificationsSymbolProperty);
        set => SetValue(TooManyNotificationsSymbolProperty, value);
    }

    /// <summary>
    /// The font size.
    /// </summary>
    public static readonly BindableProperty FontSizeProperty =
        BindableProperty.Create(nameof(FontSize), typeof(double), typeof(NotificationBadge), 16d,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the font size.
    /// </summary>
    /// <value>The size of the font as a double. Default is 16d.</value>
    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    /// <summary>
    /// The typeface.
    /// </summary>
    public static readonly BindableProperty TypefaceProperty =
        BindableProperty.Create(nameof(Typeface), typeof(SKTypeface), typeof(NotificationBadge), default(SKTypeface),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the typeface.
    /// </summary>
    /// <value>Expects a SKTypeface. Default default(SKTypeface).</value>
    public SKTypeface Typeface
    {
        get => (SKTypeface)GetValue(TypefaceProperty);
        set => SetValue(TypefaceProperty, value);
    }

    /// <summary>
    /// The maximum badge size.
    /// </summary>
    public static readonly BindableProperty MaxBadgeSizeProperty =
        BindableProperty.Create(nameof(MaxBadgeSize), typeof(double), typeof(NotificationBadge), 0d,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the maximum badge size.
    /// </summary>
    /// <value>Maximum size as a double. Default is 0d.</value>
    public double MaxBadgeSize
    {
        get => (double)GetValue(MaxBadgeSizeProperty);
        set => SetValue(MaxBadgeSizeProperty, value);
    }

    /// <summary>
    /// The minimum badge size.
    /// </summary>
    public static readonly BindableProperty MinBadgeSizeProperty =
        BindableProperty.Create(nameof(MinBadgeSize), typeof(double), typeof(NotificationBadge), 0d,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the minimum badge size.
    /// </summary>
    /// <value>Minimum size as a double. Default is 0d.</value>
    public double MinBadgeSize
    {
        get => (double)GetValue(MinBadgeSizeProperty);
        set => SetValue(MinBadgeSizeProperty, value);
    }

    /// <summary>
    /// The property to hide the badge if the value is zero.
    /// </summary>
    public static readonly BindableProperty HideBadgeIfZeroProperty =
        BindableProperty.Create(nameof(HideBadgeIfZero), typeof(bool), typeof(NotificationBadge), true,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets a value indicating whether the badge is hidden when zero.
    /// </summary>
    /// <value><c>true</c> if enabled, otherwise <c>false</c>.</value>
    public bool HideBadgeIfZero
    {
        get => (bool)GetValue(HideBadgeIfZeroProperty);
        set => SetValue(HideBadgeIfZeroProperty, value);
    }

    /// <summary>
    /// The has shadow property.
    /// </summary>
    public static readonly BindableProperty HasShadowProperty =
        BindableProperty.Create(nameof(HasShadow), typeof(bool), typeof(NotificationBadge), true,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets a value indicating whether the badge has a shadow.
    /// </summary>
    /// <value><c>true</c> if enabled, otherwise <c>false</c>.</value>
    public bool HasShadow
    {
        get => (bool)GetValue(HasShadowProperty);
        set => SetValue(HasShadowProperty, value);
    }

    /// <summary>
    /// The spread of the shadow.
    /// </summary>
    public static readonly BindableProperty ShadowSpreadProperty =
        BindableProperty.Create(nameof(ShadowSpread), typeof(double), typeof(NotificationBadge), 2d,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets spread of the shadow.
    /// </summary>
    /// <value>The size of the shadow spread as a double. Default is 2d.</value>
    public double ShadowSpread
    {
        get => (double)GetValue(ShadowSpreadProperty);
        set => SetValue(ShadowSpreadProperty, value);
    }

    /// <summary>
    /// The notification count.
    /// </summary>
    public static readonly BindableProperty NotificationCountProperty =
        BindableProperty.Create(nameof(NotificationCount), typeof(int), typeof(NotificationBadge), default(int),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the notification count.
    /// </summary>
    /// <value>The notification count as an int. Default is 0.</value>
    public int NotificationCount
    {
        get => (int)GetValue(NotificationCountProperty);
        set
        {
            SetValue(NotificationCountProperty, value);
            NotificationCountChanged?.Invoke(this, new ValueChangedEventArgs(this.NotificationCount, value));
        }
    }

    protected override void Attached()
    {
        _badgePaint = new();
        _fontPaint = new();

        base.Attached();
    }

    protected override void Detached()
    {
        _badgePaint?.Dispose();
        _fontPaint?.Dispose();

        base.Detached();
    }

    protected override void PaintControl(SKSurface surface, SKImageInfo info) => DrawNotificationBadge(surface, info.Rect, true);

    public void DrawNotificationBadge(SKSurface surface, SKRect rect, bool clear = true)
    {
        var canvas = surface.Canvas;

        if (clear)
        {
            canvas.Clear();
        }

        _badgePaint.IsAntialias = true;
        _badgePaint.FilterQuality = SKFilterQuality.High;
        _badgePaint.Color = BadgeColor.ToSKColor();

        var shadowSpread = HasShadow ? (float)ShadowSpread * _scale : 0f;

        if (shadowSpread > 0)
        {
            _badgePaint.ImageFilter =
                SKImageFilter.CreateDropShadow(
                    0, 0, shadowSpread, shadowSpread,
                    SKColors.Black);
        }

        var minLength = Math.Min(rect.Height, rect.Width);

        var maxBadgeSize = MaxBadgeSize * _scale;

        if (maxBadgeSize > 0 && minLength > maxBadgeSize)
        {
            minLength = (float)maxBadgeSize;
        }

        _fontPaint.Color = FontColor.ToSKColor();
        _fontPaint.TextSize = (float)FontSize * _scale;
        _fontPaint.Typeface = Typeface ?? PlatformInfo.DefaultTypeface;
        _fontPaint.IsAntialias = true;
        _fontPaint.TextAlign = SKTextAlign.Center;

        var notificationCount = NotificationCount;
        var notificationText = notificationCount > 99 ? TooManyNotificationsSymbol : notificationCount.ToString();

        _fontPaint.EnsureHasValidFont(notificationText);

        var hideBadgeCondition = !(notificationCount <= 0 && HideBadgeIfZero);

        if (hideBadgeCondition)
        {
            canvas.DrawCircle(rect.MidX, rect.MidY, (minLength * .5f) - (shadowSpread * 2f), _badgePaint);

            if (!string.IsNullOrEmpty(notificationText))
            {
                try
                {
                    var textFits = false;
                    var midY = 0f;

                    minLength *= .7f;

                    while (!textFits)
                    {
                        var textBounds = SKRect.Empty;
                        _fontPaint.MeasureText(notificationText, ref textBounds);

                        if (textBounds.Width < minLength && textBounds.Height < minLength)
                        {
                            midY = textBounds.MidY;
                            break;
                        }

                        _fontPaint.TextSize -= .5f;
                    }

                    canvas.DrawText(notificationText, rect.MidX, rect.MidY - midY, _fontPaint);
                }
                catch (ArgumentNullException)
                {
                }
            }
        }
    }
}
