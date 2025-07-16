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
    private readonly SKPath _backgroundPath = new();

    private readonly string _rippleAnimationName, _tapAnimationName;

    private readonly object _pictureLock = new();

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
    public static readonly BindableProperty EmbeddedImageNameProperty =
        BindableProperty.Create(nameof(EmbeddedImageName), typeof(string), typeof(Tile),
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
        get => (string)GetValue(EmbeddedImageNameProperty);
        set => SetValue(EmbeddedImageNameProperty, value);
    }

    /// <summary>
    /// The maximum embedded image size property.
    /// </summary>
    public static readonly BindableProperty MaxImageSizeProperty =
        BindableProperty.Create(nameof(MaxImageSize), typeof(Size), typeof(Tile), default(Size),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

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
    public static readonly BindableProperty OverlayColorProperty =
        BindableProperty.Create(nameof(OverlayColor), typeof(Color), typeof(Tile), Colors.Transparent,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the color of the overlay.
    /// </summary>
    /// <value>Takes a Xamarin.Forms.Color. Default value is Xamarin.Forms.Color.Transparent.</value>
    public Color OverlayColor
    {
        get => (Color)GetValue(OverlayColorProperty);
        set => SetValue(OverlayColorProperty, value);
    }

    /// <summary>
    /// The button background color property.
    /// </summary>
    public static readonly BindableProperty ButtonBackgroundColorProperty =
        BindableProperty.Create(nameof(ButtonBackgroundColor), typeof(Color), typeof(Tile), Colors.White,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the color of the button background.
    /// </summary>
    /// <value>Takes a Xamarin.Forms.Color. Default value is Xamarin.Forms.Colors.White.</value>
    public Color ButtonBackgroundColor
    {
        get => (Color)GetValue(ButtonBackgroundColorProperty);
        set => SetValue(ButtonBackgroundColorProperty, value);
    }

    /// <summary>
    /// The color property of the border.
    /// </summary>
    public static readonly BindableProperty BorderColorProperty =
        BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(Tile), Colors.White,
                                propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the color of the border.
    /// </summary>
    /// <value>Takes a Xamarin.Forms.Color. Default value is Xamarin.Forms.Colors.White.</value>
    public Color BorderColor
    {
        get => (Color)GetValue(BorderColorProperty);
        set => SetValue(BorderColorProperty, value);
    }

    /// <summary>
    /// The shadow color property.
    /// </summary>
    public static readonly BindableProperty ShadowColorProperty =
        BindableProperty.Create(nameof(ShadowColor), typeof(Color), typeof(Tile), Color.FromRgba(0d, 0d, 0d, .33d),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the color of the shadow.
    /// </summary>
    /// <value>Takes a Xamarin.Forms.Color. Default value is Xamarin.Forms.Color.FromRgba(0d, 0d, 0d, .33d).</value>
    public Color ShadowColor
    {
        get => (Color)GetValue(ShadowColorProperty);
        set => SetValue(ShadowColorProperty, value);
    }

    /// <summary>
    /// The shadow location property.
    /// </summary>
    public static readonly BindableProperty ShadowLocationProperty =
        BindableProperty.Create(nameof(ShadowLocation), typeof(Point), typeof(Tile), new Point(0, 3),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the shadow location.
    /// </summary>
    /// <value>Takes a Xamarin.Forms.Point. Default value is Point(0, 3).</value>
    public Point ShadowLocation
    {
        get => (Point)GetValue(ShadowLocationProperty);
        set => SetValue(ShadowLocationProperty, value);
    }

    /// <summary>
    /// The shadow blur radius property.
    /// </summary>
    public static readonly BindableProperty ShadowBlurRadiusProperty =
        BindableProperty.Create(nameof(ShadowBlurRadius), typeof(double), typeof(Tile), default(double),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the shadow blur radius.
    /// </summary>
    /// <value>Takes a double value. Default value is default(double).</value>
    public double ShadowBlurRadius
    {
        get => (double)GetValue(ShadowBlurRadiusProperty);
        set => SetValue(ShadowBlurRadiusProperty, value);
    }

    /// <summary>
    /// The border width property.
    /// </summary>
    public static readonly BindableProperty BorderWidthProperty =
        BindableProperty.Create(nameof(BorderWidth), typeof(double), typeof(Tile), 0d,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the width of the border.
    /// </summary>
    /// <value>Takes a double. Default value is 0d.</value>
    public double BorderWidth
    {
        get => (double)GetValue(BorderWidthProperty);
        set => SetValue(BorderWidthProperty, value);
    }

    /// <summary>
    /// The corner radius property.
    /// </summary>
    public static readonly BindableProperty CornerRadiusProperty =
        BindableProperty.Create(nameof(CornerRadius), typeof(double), typeof(Tile), 4d,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the corner radius.
    /// </summary>
    /// <value>Takes a double. Default value is 4d.</value>
    public double CornerRadius
    {
        get => (double)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    /// <summary>
    /// The tiles text property.
    /// </summary>
    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(Tile),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    /// <value>Expects a string value. Default value is default(string).</value>
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>
    /// The font color property.
    /// </summary>
    public static readonly BindableProperty FontColorProperty =
        BindableProperty.Create(nameof(FontColor), typeof(Color), typeof(Tile), Colors.White,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the color of the font.
    /// </summary>
    /// <value>Takes a Xamarin.Forms.Color. Default value is Xamarin.Forms.Color.White.</value>
    public Color FontColor
    {
        get => (Color)GetValue(FontColorProperty);
        set => SetValue(FontColorProperty, value);
    }

    /// <summary>
    /// The font size property.
    /// </summary>
    public static readonly BindableProperty FontSizeProperty =
        BindableProperty.Create(nameof(FontSize), typeof(double), typeof(Tile), PlatformInfo.DefaultButtonFontSize,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the font size.
    /// </summary>
    /// <value>Size as a double.</value>
    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    /// <summary>
    /// The typeface property.
    /// </summary>
    public static readonly BindableProperty TypefaceProperty =
        BindableProperty.Create(nameof(Typeface), typeof(SKTypeface), typeof(Tile),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the typeface.
    /// </summary>
    /// <value>Takes an SKTypeface. Default value is default(SKTypeface).</value>
    public SKTypeface Typeface
    {
        get => (SKTypeface)GetValue(TypefaceProperty);
        set => SetValue(TypefaceProperty, value);
    }

    public static readonly BindableProperty IsIconifiedTextProperty =
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
    public static readonly BindableProperty RipplesProperty =
        BindableProperty.Create(nameof(Ripples), typeof(bool), typeof(Tile), true,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="T:Aurora.Controls.Tile"/> is ripples.
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
        BindableProperty.Create(nameof(TapAnimationDuration), typeof(uint), typeof(Tile), 40u,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the duration of the tap animation.
    /// </summary>
    /// <value>Duration as an uint. Default is 40u.</value>
    public uint TapAnimationDuration
    {
        get => (uint)GetValue(TapAnimationDurationProperty);
        set => SetValue(TapAnimationDurationProperty, value);
    }

    /// <summary>
    /// The command property. Fires on tap.
    /// </summary>
    public static readonly BindableProperty CommandProperty =
        BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(Tile));

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
        BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(Tile));

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

    public static readonly BindableProperty ContentPaddingProperty =
        BindableProperty.Create(nameof(ContentPadding), typeof(Thickness), typeof(Tile), default(Thickness),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    public Thickness ContentPadding
    {
        get => (Thickness)GetValue(ContentPaddingProperty);
        set => SetValue(ContentPaddingProperty, value);
    }

    public static readonly BindableProperty NotificationBadgeProperty =
        BindableProperty.Create(nameof(NotificationBadge), typeof(NotificationBadge), typeof(Tile));

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

        using var backgroundPaint = new SKPaint();
        float borderWidth = (float)this.BorderWidth * _scale;
        float halfBorderWidth = borderWidth * .5f;

        float shadowBlurRadius = (float)this.ShadowBlurRadius * _scale;

        float shadowLocationX = (float)this.ShadowLocation.X * _scale;
        float shadowLocationY = (float)this.ShadowLocation.Y * _scale;

        float cornerRadius = (float)this.CornerRadius * _scale;

        // TODO: Negative Shadows are busted
        var rect =
            new SKRect(
                shadowLocationX + shadowBlurRadius + halfBorderWidth,
                shadowLocationY + shadowBlurRadius + halfBorderWidth,
                info.Width - (shadowLocationX * 2f) - (shadowBlurRadius * 2f) - halfBorderWidth,
                info.Height - (shadowLocationY * 2f) - (shadowBlurRadius * 2f) - halfBorderWidth);

        backgroundPaint.IsAntialias = true;
        backgroundPaint.Style = SKPaintStyle.Fill;
        backgroundPaint.Color = this.ButtonBackgroundColor.ToSKColor();

        canvas.Clear();
        _backgroundPath.Reset();

        if (this.ShadowColor != Colors.White && this.ShadowLocation != Point.Zero)
        {
            using (new SKAutoCanvasRestore(canvas))
            {
                float sigma = SKMaskFilter.ConvertRadiusToSigma(shadowBlurRadius) * (1f - (float)_tapAnimationPercentage);
                _shadowPaint.IsAntialias = true;
                _shadowPaint.Color = this.ShadowColor.ToSKColor();
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
            double translateX = _tapAnimationPercentage * shadowLocationX;
            double translateY = _tapAnimationPercentage * shadowLocationY;

            canvas.Translate(new SKPoint((float)translateX, (float)translateY));

            _backgroundPath.AddRoundRect(rect, cornerRadius, cornerRadius);
            canvas.DrawPath(_backgroundPath, backgroundPaint);

            if (_lastTouchLocation != SKPoint.Empty && _rippleAnimationPercentage > 0.0d)
            {
                using var ripplePath = new SKPath();
                _ripplePaint.IsAntialias = true;
                _ripplePaint.Style = SKPaintStyle.Fill;
                _ripplePaint.Color =
                    this.ButtonBackgroundColor != Colors.White
                        ? this.ButtonBackgroundColor.AddLuminosity(-.2f).MultiplyAlpha((1f - (float)_rippleAnimationPercentage) * .5f).ToSKColor()
                        : Colors.Transparent.ToSKColor();

                float startingRippleSize = Math.Min(info.Width, info.Height) * .75f;
                float maxRippleSize = startingRippleSize + (float)((Math.Max(info.Width, info.Height) * .4) * _rippleAnimationPercentage);
                float offsetAmount = -maxRippleSize / 2f;
                var offsetPoint = new SKPoint(_lastTouchLocation.X + offsetAmount, _lastTouchLocation.Y + offsetAmount);
                var rippleSize = SKRect.Create(offsetPoint, new SKSize(maxRippleSize, maxRippleSize));

                ripplePath.AddOval(rippleSize);

                using var finalRipple = ripplePath.Op(_backgroundPath, SKPathOp.Intersect);
                canvas.DrawPath(finalRipple, _ripplePaint);
            }

            if (borderWidth > 0d && this.BorderColor != Colors.White)
            {
                backgroundPaint.StrokeWidth = borderWidth;
                backgroundPaint.Color = this.BorderColor.ToSKColor();
                backgroundPaint.Shader = null;
                backgroundPaint.Style = SKPaintStyle.Stroke;
                backgroundPaint.IsAntialias = true;

                canvas.DrawPath(_backgroundPath, backgroundPaint);
            }

            var textBounds = SKRect.Empty;

            if (!string.IsNullOrEmpty(this.Text))
            {
                _fontPaint.Color = this.FontColor.ToSKColor();
                _fontPaint.TextSize = (float)this.FontSize * _scale;
                _fontPaint.IsAntialias = true;
                _fontPaint.Typeface = this.Typeface ?? PlatformInfo.DefaultTypeface;

                textBounds = canvas.GetTextContainerRectAt(this.Text, SKPoint.Empty, _fontPaint);

                float textY = rect.Top + rect.Height - (float)this.ContentPadding.Bottom - textBounds.Height - (borderWidth * 2f);

                if (this.IsIconifiedText)
                {
                    canvas.DrawCenteredIconifiedText(this.Text, rect.MidX, textY, _fontPaint);
                }
                else
                {
                    _fontPaint.EnsureHasValidFont(this.Text);
                    canvas.DrawTextAt(this.Text, new SKPoint(rect.MidX, textY), _fontPaint, TextDrawLocation.Centered);
                }
            }

            if (!string.IsNullOrEmpty(_pictureName))
            {
                // TODO: The text measurement here seems not right
                var contentRect = new SKRect(
                    rect.Left + (float)(this.ContentPadding.Left * _scale),
                    rect.Top + (float)(this.ContentPadding.Top * _scale),
                    rect.Right - (float)(this.ContentPadding.Right * _scale),
                    rect.Bottom - textBounds.Height - (float)(this.ContentPadding.Bottom * _scale));

                var imageSize = contentRect.AspectFit(_svg.Picture.CullRect.Size);

                float scaleAmount =
                    this.MaxImageSize == Size.Zero
                        ? Math.Min(imageSize.Width / _svg.Picture.CullRect.Width, imageSize.Height / _svg.Picture.CullRect.Height)
                        : 1f;

                var svgScale = SKMatrix.CreateScale(scaleAmount, scaleAmount);

                var translation =
                    this.MaxImageSize == Size.Zero
                        ? SKMatrix.CreateTranslation(imageSize.Left, imageSize.Top)
                        : SKMatrix.CreateTranslation(imageSize.MidX - (_svg.Picture.CullRect.Width / 2f), imageSize.MidY - (_svg.Picture.CullRect.Height / 2f));

                svgScale = svgScale.PostConcat(translation);

                if (this.OverlayColor != Colors.Transparent)
                {
                    using (new SKAutoCanvasRestore(canvas))
                    {
                        _overlayPaint.BlendMode = SKBlendMode.SrcATop;
                        _overlayPaint.IsAntialias = true;
                        _overlayPaint.Color = this.OverlayColor.ToSKColor();

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

        if (this.NotificationBadge != null)
        {
            float badgeSize = (float)Math.Min(info.Rect.Height * .33d, info.Rect.Width * .33d);

            double maxBadgeSize = this.NotificationBadge.MaxBadgeSize * _scale;
            double minBadgeSize = this.NotificationBadge.MinBadgeSize * _scale;

            if (maxBadgeSize > 0 && badgeSize > maxBadgeSize)
            {
                badgeSize = (float)maxBadgeSize;
            }

            if (minBadgeSize > 0 && badgeSize < minBadgeSize)
            {
                badgeSize = (float)minBadgeSize;
            }

            var badgeRect = new SKRect(info.Rect.Width - badgeSize, 0, info.Rect.Width, badgeSize);
            this.NotificationBadge?.DrawNotificationBadge(surface, badgeRect, false);
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

        bool isTapInside = _backgroundPath.Contains(e.Location.X, e.Location.Y);

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
            finished: (_, _) =>
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
            finished: (_, _) =>
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
        string embeddedImageName = EmbeddedImageName;

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
