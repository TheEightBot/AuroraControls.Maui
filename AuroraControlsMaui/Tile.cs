using Svg.Skia;

namespace AuroraControls;

/// <summary>
/// Tile.
/// </summary>
[ContentProperty(nameof(Text))]
#pragma warning disable CA1001 // Types that own disposable fields should be disposable
public class Tile : AuroraViewBase
#pragma warning restore CA1001 // Types that own disposable fields should be disposable
{
    private readonly SKPath _backgroundPath = new SKPath();

    private readonly string _rippleAnimationName, _tapAnimationName;

    private readonly object _pictureLock = new object();

    private SKPoint _lastTouchLocation;
    private double _rippleAnimationPercentage, _tapAnimationPercentage;

    private SKPaint _shadowPaint;
    private SKPaint _ripplePaint;
    private SKPaint _fontPaint;
    private SKPaint _overlayPaint;

    private SKSvg _svg;

    private string _pictureName;

    private bool _tapped;

    public event EventHandler Clicked;

    /// <summary>
    /// The embedded svg image name property.
    /// </summary>
    public static BindableProperty EmbeddedImageNameProperty =
        BindableProperty.Create(nameof(EmbeddedImageName), typeof(string), typeof(Tile), null,
            propertyChanged:
                static (bindable, _, _) =>
                {
                    if (bindable is Tile tile)
                    {
                        tile.SetSvgResource();
                        tile.InvalidateSurface();
                    }
                });

    /// <summary>
    /// Gets or sets the name of the embedded image.
    /// </summary>
    /// <value>Takes a string. Default value is null.</value>
    public string EmbeddedImageName
    {
        get { return (string)GetValue(EmbeddedImageNameProperty); }
        set { SetValue(EmbeddedImageNameProperty, value); }
    }

    /// <summary>
    /// The maximum embedded image size property.
    /// </summary>
    public static BindableProperty MaxImageSizeProperty =
        BindableProperty.Create(nameof(MaxImageSize), typeof(Size), typeof(Tile), default(Size),
            propertyChanged:
                static (bindable, _, _) =>
                {
                    if (bindable is Tile tile)
                    {
                        tile.InvalidateSurface();
                    }
                });

    /// <summary>
    /// Gets or sets the maximum image size.
    /// </summary>
    /// <value>Expects a Xamarin.Forms.Size.</value>
    public Size MaxImageSize
    {
        get => (Size)GetValue(MaxImageSizeProperty);
        set => SetValue(MaxImageSizeProperty, value);
    }

    /// <summary>
    /// The overlay color property.
    /// </summary>
    public static BindableProperty OverlayColorProperty =
        BindableProperty.Create(nameof(OverlayColor), typeof(Color), typeof(Tile), Colors.Transparent,
            propertyChanged: static (bindable, _, _) =>
            {
                if (bindable is Tile tile)
                {
                    tile.InvalidateSurface();
                }
            });

    /// <summary>
    /// Gets or sets the color of the overlay.
    /// </summary>
    /// <value>Takes a Xamarin.Forms.Color. Default value is Xamarin.Forms.Color.Transparent.</value>
    public Color OverlayColor
    {
        get { return (Color)GetValue(OverlayColorProperty); }
        set { SetValue(OverlayColorProperty, value); }
    }

    /// <summary>
    /// The button background color property.
    /// </summary>
    public static BindableProperty ButtonBackgroundColorProperty =
        BindableProperty.Create(nameof(ButtonBackgroundColor), typeof(Color), typeof(Tile), Colors.White,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the color of the button background.
    /// </summary>
    /// <value>Takes a Xamarin.Forms.Color. Default value is Xamarin.Forms.Colors.White.</value>
    public Color ButtonBackgroundColor
    {
        get { return (Color)GetValue(ButtonBackgroundColorProperty); }
        set { SetValue(ButtonBackgroundColorProperty, value); }
    }

    /// <summary>
    /// The color property of the border.
    /// </summary>
    public static BindableProperty BorderColorProperty =
        BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(Tile), Colors.White,
                                propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the color of the border.
    /// </summary>
    /// <value>Takes a Xamarin.Forms.Color. Default value is Xamarin.Forms.Colors.White.</value>
    public Color BorderColor
    {
        get { return (Color)GetValue(BorderColorProperty); }
        set { SetValue(BorderColorProperty, value); }
    }

    /// <summary>
    /// The shadow color property.
    /// </summary>
    public static BindableProperty ShadowColorProperty =
        BindableProperty.Create(nameof(ShadowColor), typeof(Color), typeof(Tile), Color.FromRgba(0d, 0d, 0d, .33d),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the color of the shadow.
    /// </summary>
    /// <value>Takes a Xamarin.Forms.Color. Default value is Xamarin.Forms.Color.FromRgba(0d, 0d, 0d, .33d).</value>
    public Color ShadowColor
    {
        get { return (Color)GetValue(ShadowColorProperty); }
        set { SetValue(ShadowColorProperty, value); }
    }

    /// <summary>
    /// The shadow location property.
    /// </summary>
    public static BindableProperty ShadowLocationProperty =
        BindableProperty.Create(nameof(ShadowLocation), typeof(Point), typeof(Tile), new Point(0, 3),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the shadow location.
    /// </summary>
    /// <value>Takes a Xamarin.Forms.Point. Default value is Point(0, 3).</value>
    public Point ShadowLocation
    {
        get { return (Point)GetValue(ShadowLocationProperty); }
        set { SetValue(ShadowLocationProperty, value); }
    }

    /// <summary>
    /// The shadow blur radius property.
    /// </summary>
    public static BindableProperty ShadowBlurRadiusProperty =
        BindableProperty.Create(nameof(ShadowBlurRadius), typeof(double), typeof(Tile), default(double),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the shadow blur radius.
    /// </summary>
    /// <value>Takes a double value. Default value is default(double).</value>
    public double ShadowBlurRadius
    {
        get { return (double)GetValue(ShadowBlurRadiusProperty); }
        set { SetValue(ShadowBlurRadiusProperty, value); }
    }

    /// <summary>
    /// The border width property.
    /// </summary>
    public static BindableProperty BorderWidthProperty =
        BindableProperty.Create(nameof(BorderWidth), typeof(double), typeof(Tile), 0d,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the width of the border.
    /// </summary>
    /// <value>Takes a double. Default value is 0d.</value>
    public double BorderWidth
    {
        get { return (double)GetValue(BorderWidthProperty); }
        set { SetValue(BorderWidthProperty, value); }
    }

    /// <summary>
    /// The corner radius property.
    /// </summary>
    public static BindableProperty CornerRadiusProperty =
        BindableProperty.Create(nameof(CornerRadius), typeof(double), typeof(Tile), 4d,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the corner radius.
    /// </summary>
    /// <value>Takes a double. Default value is 4d.</value>
    public double CornerRadius
    {
        get { return (double)GetValue(CornerRadiusProperty); }
        set { SetValue(CornerRadiusProperty, value); }
    }

    /// <summary>
    /// The tiles text property.
    /// </summary>
    public static BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(Tile), default(string),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    /// <value>Expects a string value. Default value is default(string).</value>
    public string Text
    {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }

    /// <summary>
    /// The font color property.
    /// </summary>
    public static BindableProperty FontColorProperty =
        BindableProperty.Create(nameof(FontColor), typeof(Color), typeof(Tile), Colors.White,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the color of the font.
    /// </summary>
    /// <value>Takes a Xamarin.Forms.Color. Default value is Xamarin.Forms.Color.White.</value>
    public Color FontColor
    {
        get { return (Color)GetValue(FontColorProperty); }
        set { SetValue(FontColorProperty, value); }
    }

    /// <summary>
    /// The font size property.
    /// </summary>
    public static BindableProperty FontSizeProperty =
        BindableProperty.Create(nameof(FontSize), typeof(double), typeof(Tile), PlatformInfo.DefaultButtonFontSize,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the font size.
    /// </summary>
    /// <value>Size as a double.</value>
    public double FontSize
    {
        get { return (double)GetValue(FontSizeProperty); }
        set { SetValue(FontSizeProperty, value); }
    }

    /// <summary>
    /// The typeface property.
    /// </summary>
    public static BindableProperty TypefaceProperty =
        BindableProperty.Create(nameof(Typeface), typeof(SKTypeface), typeof(Tile), default(SKTypeface),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the typeface.
    /// </summary>
    /// <value>Takes an SKTypeface. Default value is default(SKTypeface).</value>
    public SKTypeface Typeface
    {
        get { return (SKTypeface)GetValue(TypefaceProperty); }
        set { SetValue(TypefaceProperty, value); }
    }

    public static BindableProperty IsIconifiedTextProperty =
        BindableProperty.Create(nameof(IsIconifiedText), typeof(bool), typeof(Tile), default(bool),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    public bool IsIconifiedText
    {
        get => (bool)GetValue(IsIconifiedTextProperty);
        set => SetValue(IsIconifiedTextProperty, value);
    }

    /// <summary>
    /// The ripples property specifies whether the ripple animation should be performed.
    /// </summary>
    public static BindableProperty RipplesProperty =
        BindableProperty.Create(nameof(Ripples), typeof(bool), typeof(Tile), true,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="T:Aurora.Controls.Tile"/> is ripples.
    /// </summary>
    /// <value><c>true</c> if ripples; otherwise, <c>false</c>.</value>
    public bool Ripples
    {
        get { return (bool)GetValue(RipplesProperty); }
        set { SetValue(RipplesProperty, value); }
    }

    /// <summary>
    /// The tap animation duration property.
    /// </summary>
    public static BindableProperty TapAnimationDurationProperty =
        BindableProperty.Create(nameof(TapAnimationDuration), typeof(uint), typeof(Tile), 40u,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the duration of the tap animation.
    /// </summary>
    /// <value>Duration as an uint. Default is 40u.</value>
    public uint TapAnimationDuration
    {
        get { return (uint)GetValue(TapAnimationDurationProperty); }
        set { SetValue(TapAnimationDurationProperty, value); }
    }

    /// <summary>
    /// The command property. Fires on tap.
    /// </summary>
    public static BindableProperty CommandProperty =
        BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(Tile), default(ICommand));

    /// <summary>
    /// Gets or sets the command.
    /// </summary>
    /// <value>Takes a System.Windows.Input.ICommand. Default value is default(ICommand).</value>
    public ICommand Command
    {
        get { return (ICommand)GetValue(CommandProperty); }
        set { SetValue(CommandProperty, value); }
    }

    /// <summary>
    /// The command parameter property.
    /// </summary>
    public static BindableProperty CommandParameterProperty =
        BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(Tile), default(object));

    /// <summary>
    /// Gets or sets the command parameter.
    /// </summary>
    /// <value>The command parameter.</value>
    public object CommandParameter
    {
        get { return (object)GetValue(CommandParameterProperty); }
        set { SetValue(CommandParameterProperty, value); }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="T:Aurora.Controls.Tile"/> is tapped.
    /// </summary>
    /// <value><c>true</c> if tapped; otherwise, <c>false</c>.</value>
    public bool Tapped
    {
        get
        {
            return _tapped;
        }

        set
        {
            if (_tapped != value)
            {
                _tapped = value;
                AnimateTap(value);
            }
        }
    }

    public static BindableProperty ContentPaddingProperty =
        BindableProperty.Create(nameof(ContentPadding), typeof(Thickness), typeof(Tile), default(Thickness),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    public Thickness ContentPadding
    {
        get { return (Thickness)GetValue(ContentPaddingProperty); }
        set { SetValue(ContentPaddingProperty, value); }
    }

    public static BindableProperty NotificationBadgeProperty =
        BindableProperty.Create(nameof(NotificationBadge), typeof(NotificationBadge), typeof(Tile), default(NotificationBadge));

    public NotificationBadge NotificationBadge
    {
        get => (NotificationBadge)GetValue(NotificationBadgeProperty);
        set => SetValue(NotificationBadgeProperty, value);
    }

    public Tile()
    {
        _rippleAnimationName = $"{nameof(Ripples)}_{Guid.NewGuid()}";
        _tapAnimationName = $"{nameof(TapAnimationDuration)}_{Guid.NewGuid()}";

        MinimumHeightRequest = IAuroraView.StandardControlHeight;
    }

    protected override void Attached()
    {
        this.EnableTouchEvents = true;

        _shadowPaint = new();
        _ripplePaint = new();
        _fontPaint = new();
        _overlayPaint = new();

        SetSvgResource();

        base.Attached();
    }

    protected override void Detached()
    {
        _shadowPaint?.Dispose();
        _ripplePaint?.Dispose();
        _fontPaint?.Dispose();
        _overlayPaint?.Dispose();
        _svg?.Dispose();

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

        using (var backgroundPaint = new SKPaint())
        {
            var borderWidth = (float)this.BorderWidth * _scale;
            var halfBorderWidth = borderWidth * .5f;

            var shadowBlurRadius = (float)ShadowBlurRadius * _scale;

            var shadowLocationX = (float)ShadowLocation.X * _scale;
            var shadowLocationY = (float)ShadowLocation.Y * _scale;

            var cornerRadius = (float)CornerRadius * _scale;

            // TODO: Negative Shadows are busted
            var rect =
                new SKRect(
                    shadowLocationX + shadowBlurRadius + halfBorderWidth,
                    shadowLocationY + shadowBlurRadius + halfBorderWidth,
                    info.Width - (shadowLocationX * 2f) - (shadowBlurRadius * 2f) - halfBorderWidth,
                    info.Height - (shadowLocationY * 2f) - (shadowBlurRadius * 2f) - halfBorderWidth);

            backgroundPaint.IsAntialias = true;
            backgroundPaint.Style = SKPaintStyle.Fill;
            backgroundPaint.Color = ButtonBackgroundColor.ToSKColor();

            canvas.Clear();
            _backgroundPath.Reset();

            if (ShadowColor != Colors.White && ShadowLocation != Point.Zero)
            {
                using (new SKAutoCanvasRestore(canvas))
                {
                    var sigma = SKMaskFilter.ConvertRadiusToSigma(shadowBlurRadius) * (1f - (float)_tapAnimationPercentage);
                    _shadowPaint.IsAntialias = true;
                    _shadowPaint.Color = ShadowColor.ToSKColor();
                    _shadowPaint.Style = SKPaintStyle.Fill;
                    _shadowPaint.IsAntialias = true;
                    _shadowPaint.MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, sigma);

                    canvas.Translate(new SKPoint(shadowLocationX, shadowLocationY));

                    var shadowRect = new SKRect(rect.Left + sigma, rect.Top + sigma, rect.Right - (sigma * 2f), rect.Bottom - (sigma * 2f));

                    canvas.DrawRoundRect(shadowRect, cornerRadius, cornerRadius, _shadowPaint);
                }
            }

            using (new SKAutoCanvasRestore(canvas))
            {
                var translateX = _tapAnimationPercentage * shadowLocationX;
                var translateY = _tapAnimationPercentage * shadowLocationY;

                canvas.Translate(new SKPoint((float)translateX, (float)translateY));

                _backgroundPath.AddRoundRect(rect, cornerRadius, cornerRadius);
                canvas.DrawPath(_backgroundPath, backgroundPaint);

                if (_lastTouchLocation != SKPoint.Empty && _rippleAnimationPercentage > 0.0d)
                {
                    using (var ripplePath = new SKPath())
                    {
                        _ripplePaint.IsAntialias = true;
                        _ripplePaint.Style = SKPaintStyle.Fill;
                        _ripplePaint.Color =
                            ButtonBackgroundColor != Colors.White
                                ? ButtonBackgroundColor.AddLuminosity(-.2f).MultiplyAlpha((1f - (float)_rippleAnimationPercentage) * .5f).ToSKColor()
                                : Colors.Transparent.ToSKColor();

                        var startingRippleSize = Math.Min(info.Width, info.Height) * .75f;
                        var maxRippleSize = startingRippleSize + (float)((Math.Max(info.Width, info.Height) * .4) * _rippleAnimationPercentage);
                        var offsetAmount = -maxRippleSize / 2f;
                        var offsetPoint = new SKPoint(_lastTouchLocation.X + offsetAmount, _lastTouchLocation.Y + offsetAmount);
                        var rippleSize = SKRect.Create(offsetPoint, new SKSize(maxRippleSize, maxRippleSize));

                        ripplePath.AddOval(rippleSize);

                        using (var finalRipple = ripplePath.Op(_backgroundPath, SKPathOp.Intersect))
                        {
                            canvas.DrawPath(finalRipple, _ripplePaint);
                        }
                    }
                }

                if (borderWidth > 0d && BorderColor != Colors.White)
                {
                    backgroundPaint.StrokeWidth = borderWidth;
                    backgroundPaint.Color = BorderColor.ToSKColor();
                    backgroundPaint.Shader = null;
                    backgroundPaint.Style = SKPaintStyle.Stroke;
                    backgroundPaint.IsAntialias = true;

                    canvas.DrawPath(_backgroundPath, backgroundPaint);
                }

                var textBounds = SKRect.Empty;

                if (!string.IsNullOrEmpty(Text))
                {
                    _fontPaint.Color = FontColor.ToSKColor();
                    _fontPaint.TextSize = (float)FontSize * _scale;
                    _fontPaint.IsAntialias = true;
                    _fontPaint.Typeface = Typeface ?? PlatformInfo.DefaultTypeface;

                    textBounds = canvas.GetTextContainerRectAt(Text, SKPoint.Empty, _fontPaint);

                    var textY = rect.Top + rect.Height - (float)ContentPadding.Bottom - textBounds.Height - (borderWidth * 2f);

                    if (IsIconifiedText)
                    {
                        canvas.DrawCenteredIconifiedText(Text, rect.MidX, textY, _fontPaint);
                    }
                    else
                    {
                        _fontPaint.EnsureHasValidFont(Text);
                        canvas.DrawTextAt(Text, new SKPoint(rect.MidX, textY), _fontPaint, TextDrawLocation.Centered, TextDrawLocation.At);
                    }
                }

                if (!string.IsNullOrEmpty(_pictureName))
                {
                    // TODO: The text measurement here seems not right
                    var contentRect = new SKRect(
                        rect.Left + (float)(ContentPadding.Left * _scale),
                        rect.Top + (float)(ContentPadding.Top * _scale),
                        rect.Right - (float)(ContentPadding.Right * _scale),
                        rect.Bottom - textBounds.Height - (float)(ContentPadding.Bottom * _scale));

                    var imageSize = contentRect.AspectFit(_svg.Picture.CullRect.Size);

                    var scaleAmount =
                        this.MaxImageSize == Size.Zero
                            ? (float)Math.Min(imageSize.Width / _svg.Picture.CullRect.Width, imageSize.Height / _svg.Picture.CullRect.Height)
                            : 1f;

                    var svgScale = SKMatrix.CreateScale(scaleAmount, scaleAmount);

                    var translation =
                        this.MaxImageSize == Size.Zero
                            ? SKMatrix.CreateTranslation(imageSize.Left, imageSize.Top)
                            : SKMatrix.CreateTranslation(imageSize.MidX - (_svg.Picture.CullRect.Width / 2f), imageSize.MidY - (_svg.Picture.CullRect.Height / 2f));

                    svgScale = svgScale.PostConcat(translation);

                    if (OverlayColor != Colors.Transparent)
                    {
                        using (new SKAutoCanvasRestore(canvas))
                        {
                            _overlayPaint.BlendMode = SKBlendMode.SrcATop;
                            _overlayPaint.IsAntialias = true;
                            _overlayPaint.Color = OverlayColor.ToSKColor();

                            canvas.SaveLayer(null);
                            canvas.Clear();
                            canvas.DrawPicture(_svg.Picture, ref svgScale);
                            canvas.DrawPaint(_overlayPaint);
                        }
                    }
                    else
                    {
                        canvas.DrawPicture(_svg.Picture, ref svgScale);
                    }
                }
            }

            if (NotificationBadge != null)
            {
                var badgeSize = (float)Math.Min(info.Rect.Height * .33d, info.Rect.Width * .33d);

                var maxBadgeSize = NotificationBadge.MaxBadgeSize * _scale;
                var minBadgeSize = NotificationBadge.MinBadgeSize * _scale;

                if (maxBadgeSize > 0 && badgeSize > maxBadgeSize)
                {
                    badgeSize = (float)maxBadgeSize;
                }

                if (minBadgeSize > 0 && badgeSize < minBadgeSize)
                {
                    badgeSize = (float)minBadgeSize;
                }

                var badgeRect = new SKRect(info.Rect.Width - badgeSize, 0, info.Rect.Width, badgeSize);
                NotificationBadge?.DrawNotificationBadge(surface, badgeRect, false);
            }
        }
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

        if (Ripples && e.ActionType == SKTouchAction.Pressed && isTapInside)
        {
            _lastTouchLocation = e.Location;
            AnimateRipple();
        }

        if (e.ActionType == SKTouchAction.Released && isTapInside)
        {
            Tapped = false;

            _lastTouchLocation = e.Location;

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

        this.AbortAnimation(_rippleAnimationName);
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
            this, _rippleAnimationName, length: 500, easing: Easing.CubicOut,
            finished: (percent, isFinished) =>
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
            finished: (percent, isFinished) =>
            {
                _tapAnimationPercentage = tapped ? 1 : 0;
                this.InvalidateSurface();
            });
    }

    /// <summary>
    /// Sets the svg resource.
    /// </summary>
    private void SetSvgResource()
    {
        var embeddedImageName = EmbeddedImageName;

        if (string.IsNullOrEmpty(embeddedImageName))
        {
            _pictureName = null;
            return;
        }

        if (!string.IsNullOrEmpty(_pictureName) && _pictureName.Equals(embeddedImageName))
        {
            return;
        }

        lock (_pictureLock)
        {
            using var imageStream = EmbeddedResourceLoader.Load(embeddedImageName);
            _svg = new SKSvg();
            _svg.Load(imageStream);
            _pictureName = embeddedImageName;
        }
    }
}