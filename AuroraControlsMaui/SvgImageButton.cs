using Svg.Skia;

namespace AuroraControls;

public enum SvgImageButtonBackgroundShape
{
    None,
    Square,
    Circular,
    RoundedSquare,
}

/// <summary>
/// An SVG-based ImageButton control.
/// </summary>
[ContentProperty(nameof(EmbeddedImageName))]
#pragma warning disable CA1001
public class SvgImageButton : AuroraViewBase
#pragma warning restore CA1001
{
    private readonly object _pictureLock = new();

    private SKRect _touchArea;
    private SKSvg _svg;

    private string _pictureName;

    private SKPoint _lastTouchLocation;
    private double _animationPercentage = 0d;

    public event EventHandler Clicked;

    /// <summary>
    /// The name of the embedded image to display.
    /// </summary>
    public static readonly BindableProperty EmbeddedImageNameProperty =
        BindableProperty.Create(nameof(EmbeddedImageName), typeof(string), typeof(SvgImageButton), null,
            propertyChanged:
            async (bindable, _, _) =>
            {
                if (bindable is SvgImageButton cgv)
                {
                    cgv.SetSvgResource();
                    cgv.InvalidateSurface();
                }
            });

    /// <summary>
    /// Gets or sets the name of the embedded image.
    /// </summary>
    /// <value>string value. default value is null.</value>
    public string EmbeddedImageName
    {
        get { return (string)GetValue(EmbeddedImageNameProperty); }
        set { SetValue(EmbeddedImageNameProperty, value); }
    }

    /// <summary>
    /// The maximum embedded image size property.
    /// </summary>
    public static readonly BindableProperty MaxImageSizeProperty =
        BindableProperty.Create(nameof(MaxImageSize), typeof(Size), typeof(SvgImageButton), default(Size),
            propertyChanged:
            async (bindable, _, _) =>
            {
                if (bindable is SvgImageButton cgv)
                {
                    cgv.SetSvgResource();
                    cgv.InvalidateSurface();
                }
            });

    /// <summary>
    /// Gets or sets the maximum image size.
    /// </summary>
    /// <value>Expects a Size.</value>
    public Size MaxImageSize
    {
        get => (Size)GetValue(MaxImageSizeProperty);
        set => SetValue(MaxImageSizeProperty, value);
    }

    /// <summary>
    /// The animated property specifies whether animations should be performed.
    /// </summary>
    public static readonly BindableProperty AnimatedProperty =
        BindableProperty.Create(nameof(Animated), typeof(bool), typeof(SvgImageButton), true);

    public static readonly BindableProperty ImageInsetProperty =
        BindableProperty.Create(nameof(ImageInset), typeof(float), typeof(SvgImageButton), default(float),
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    public float ImageInset
    {
        get => (float)GetValue(ImageInsetProperty);
        set => SetValue(ImageInsetProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="T:Aurora.Controls.ImageButton"/> is animated.
    /// </summary>
    /// <value><c>true</c> if animated; otherwise, <c>false</c>.</value>
    public bool Animated
    {
        get { return (bool)GetValue(AnimatedProperty); }
        set { SetValue(AnimatedProperty, value); }
    }

    public static readonly BindableProperty AnimationScaleAmountProperty =
        BindableProperty.Create(nameof(AnimationScaleAmount), typeof(float), typeof(SvgImageButton), .1f,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    public float AnimationScaleAmount
    {
        get => (float)GetValue(AnimationScaleAmountProperty);
        set => SetValue(AnimationScaleAmountProperty, value);
    }

    /// <summary>
    /// The animation easing property.
    /// </summary>
    public static readonly BindableProperty AnimationEasingProperty =
        BindableProperty.Create(nameof(AnimationEasing), typeof(Easing), typeof(SvgImageButton), Easing.CubicInOut);

    /// <summary>
    /// Gets or sets the animation easing.
    /// </summary>
    /// <value>takes a Easing. Default value is Easing.CubicInOut.</value>
    public Easing AnimationEasing
    {
        get { return (Easing)GetValue(AnimationEasingProperty); }
        set { SetValue(AnimationEasingProperty, value); }
    }

    /// <summary>
    /// The overlay color property.
    /// </summary>
    public static readonly BindableProperty OverlayColorProperty =
        BindableProperty.Create(nameof(OverlayColor), typeof(Color), typeof(SvgImageButton), Colors.Transparent,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the color of the overlay.
    /// </summary>
    /// <value>Expects a Color. Default is Transparent.</value>
    public Color OverlayColor
    {
        get { return (Color)GetValue(OverlayColorProperty); }
        set { SetValue(OverlayColorProperty, value); }
    }

    /// <summary>
    /// The color of the background.
    /// </summary>
    public static new readonly BindableProperty BackgroundColorProperty =
        BindableProperty.Create(nameof(BackgroundColor), typeof(Color), typeof(SvgImageButton), Colors.Transparent,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the background color.
    /// </summary>
    /// <value>Expects a Color. Default Color.Transparent.</value>
    public new Color BackgroundColor
    {
        get { return (Color)GetValue(BackgroundColorProperty); }
        set { SetValue(BackgroundColorProperty, value); }
    }

    public static readonly BindableProperty BackgroundShapeProperty =
        BindableProperty.Create(nameof(BackgroundShape), typeof(SvgImageButtonBackgroundShape), typeof(SvgImageButton), SvgImageButtonBackgroundShape.None,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    public SvgImageButtonBackgroundShape BackgroundShape
    {
        get => (SvgImageButtonBackgroundShape)GetValue(BackgroundShapeProperty);
        set => SetValue(BackgroundShapeProperty, value);
    }

    public static readonly BindableProperty CornerRadiusProperty =
        BindableProperty.Create(nameof(CornerRadius), typeof(double), typeof(SvgImageButton), 8d,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    public double CornerRadius
    {
        get => (double)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    /// <summary>
    /// The command property. Fires on tap.
    /// </summary>
    public static readonly BindableProperty CommandProperty =
        BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(SvgImageButton), default(ICommand));

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
    public static readonly BindableProperty CommandParameterProperty =
        BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(SvgImageButton), default);

    /// <summary>
    /// Gets or sets the command parameter.
    /// </summary>
    /// <value>The command parameter.</value>
    public object CommandParameter
    {
        get { return (object)GetValue(CommandParameterProperty); }
        set { SetValue(CommandParameterProperty, value); }
    }

    protected override void Attached()
    {
        this.EnableTouchEvents = true;

        this.SetSvgResource();
        base.Attached();
    }

    protected override void Detached()
    {
        _svg?.Picture?.Dispose();

        base.Detached();
    }

    /// <summary>
    /// Method that is called when the property that is specified by propertyName is changed.
    /// The surface is automatically invalidated/redrawn whenever <c>HeightProperty</c>, <c>WidthProperty</c> or <c>MarginProperty</c> gets updated.
    /// </summary>
    /// <param name="propertyName">The name of the bound property that changed.</param>
    protected override void OnPropertyChanged(string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName.Equals(HeightProperty.PropertyName) ||
            propertyName.Equals(WidthProperty.PropertyName) ||
            propertyName.Equals(MarginProperty.PropertyName))
        {
            this.SetSvgResource();
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

        // Calculate inset in physical pixels
        float insetPixels = this.ImageInset * _scale;

        // Calculate total available space considering margins
        float totalAvailableWidth = info.Width - (float)this.Margin.Left - (float)this.Margin.Right;
        float totalAvailableHeight = info.Height - (float)this.Margin.Top - (float)this.Margin.Bottom;

        // Calculate the area for the background shape (without insets)
        float backgroundSize = Math.Min(totalAvailableWidth, totalAvailableHeight);

        // Calculate the area available for the SVG (with insets applied)
        float svgAvailableWidth = backgroundSize - (insetPixels * 2);
        float svgAvailableHeight = backgroundSize - (insetPixels * 2);

        float left = (info.Width - backgroundSize) / 2f;
        float top = (info.Height - backgroundSize) / 2f;
        float right = left + backgroundSize;
        float bottom = top + backgroundSize;

        _touchArea = new SKRect(left, top, right, bottom);

        canvas.Clear();

        if (_svg?.Picture != null)
        {
            float svgWidth = _svg.Picture.CullRect.Width;
            float svgHeight = _svg.Picture.CullRect.Height;

            if (svgWidth <= 0 || svgHeight <= 0)
            {
                return;
            }

            // Calculate scale to fit the SVG in the inset-adjusted area
            float scaleX = svgAvailableWidth / svgWidth;
            float scaleY = svgAvailableHeight / svgHeight;
            float scaleAmount = Math.Min(scaleX, scaleY);

            // Apply MaxImageSize constraints if specified
            if (this.MaxImageSize != Size.Zero)
            {
                float maxScaleX = (float)this.MaxImageSize.Width / svgWidth;
                float maxScaleY = (float)this.MaxImageSize.Height / svgHeight;
                float maxScale = Math.Min(maxScaleX, maxScaleY);
                scaleAmount = Math.Min(scaleAmount, maxScale);
            }

            // Apply animation scaling - simpler approach
            float animationScale = 1f - (this.AnimationScaleAmount * (float)_animationPercentage);
            float finalScale = scaleAmount * animationScale;

            // Draw background shape with consistent animation scaling
            using (var backgroundPaint = new SKPaint())
            {
                backgroundPaint.Color = this.BackgroundColor.ToSKColor();
                backgroundPaint.IsAntialias = true;
                backgroundPaint.Style = SKPaintStyle.Fill;

                // Apply the same animation scale to the background shape
                float animatedBackgroundSize = backgroundSize * animationScale;
                float halfSize = animatedBackgroundSize / 2f;
                var shapeRect = new SKRect(
                    info.Rect.MidX - halfSize,
                    info.Rect.MidY - halfSize,
                    info.Rect.MidX + halfSize,
                    info.Rect.MidY + halfSize);

                switch (this.BackgroundShape)
                {
                    case SvgImageButtonBackgroundShape.Circular:
                        canvas.DrawOval(shapeRect, backgroundPaint);
                        break;
                    case SvgImageButtonBackgroundShape.RoundedSquare:
                        var cornerRadius = new SKSize((float)this.CornerRadius, (float)this.CornerRadius);
                        canvas.DrawRoundRect(shapeRect, cornerRadius, backgroundPaint);
                        break;
                    case SvgImageButtonBackgroundShape.Square:
                        canvas.DrawRect(shapeRect, backgroundPaint);
                        break;
                }
            }

            // Draw SVG with proper scaling and centering
            using (new SKAutoCanvasRestore(canvas, true))
            {
                // Calculate the scaled SVG dimensions
                float scaledWidth = svgWidth * finalScale;
                float scaledHeight = svgHeight * finalScale;

                // Center the SVG in the available space
                float translateX = (info.Width - scaledWidth) / 2f;
                float translateY = (info.Height - scaledHeight) / 2f;

                var transform = SKMatrix.CreateScale(finalScale, finalScale);
                transform = transform.PostConcat(SKMatrix.CreateTranslation(translateX, translateY));

                if (this.OverlayColor != Colors.Transparent)
                {
                    using var paint = new SKPaint { BlendMode = SKBlendMode.SrcATop, Style = SKPaintStyle.Fill, IsAntialias = true };
                    canvas.SaveLayer(info.Rect, null);
                    canvas.Clear();

                    canvas.DrawPicture(_svg.Picture, ref transform);

                    paint.Color = this.OverlayColor.ToSKColor();
                    canvas.DrawPaint(paint);
                }
                else
                {
                    canvas.DrawPicture(_svg.Picture, ref transform);
                }
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
            this.Animate(reset: true);
            return;
        }

        bool isTapInside = _touchArea.Contains(e.Location.X, e.Location.Y);

        if (e.ActionType == SKTouchAction.Released && isTapInside)
        {
            _lastTouchLocation = e.Location;
            this.Animate();

            if (this.Command?.CanExecute(this.CommandParameter) ?? false)
            {
                this.Command.Execute(this.CommandParameter);
            }

            this.Clicked?.Invoke(this, EventArgs.Empty);
        }
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

    /// <summary>
    /// Animate the specified reset.
    /// </summary>
    /// <param name="reset">If set to <c>true</c> reset.</param>
    private void Animate(bool reset = false)
    {
        if (_lastTouchLocation == SKPoint.Empty && !this.Animated)
        {
            return;
        }

        const string animName = "ImageButtonAnimating";

        this.AbortAnimation(animName);
        _animationPercentage = 0d;

        if (reset)
        {
            return;
        }

        var rippleAnimation = new Animation(x =>
        {
            _animationPercentage = x;
            this.InvalidateSurface();
        });

        rippleAnimation.Commit(
            this, animName, length: 400, easing: this.AnimationEasing,
            finished: (_, _) =>
            {
                _lastTouchLocation = SKPoint.Empty;
                _animationPercentage = 0d;
                this.InvalidateSurface();
            });
    }
}
