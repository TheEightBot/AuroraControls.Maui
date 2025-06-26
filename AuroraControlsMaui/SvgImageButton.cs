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
    private readonly object _pictureLock = new object();

    private SKRect _touchArea;
    private SKSvg _svg;

    private string _pictureName;

    private SKPoint _lastTouchLocation;
    private double _animationPercentage = 0d;

    public event EventHandler Clicked;

    /// <summary>
    /// The name of the embedded image to display.
    /// </summary>
    public static BindableProperty EmbeddedImageNameProperty =
        BindableProperty.Create(nameof(EmbeddedImageName), typeof(string), typeof(SvgImageButton), null,
            propertyChanged:
            async (bindable, oldValue, newValue) =>
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
    public static BindableProperty MaxImageSizeProperty =
        BindableProperty.Create(nameof(MaxImageSize), typeof(Size), typeof(SvgImageButton), default(Size),
            propertyChanged:
            async (bindable, oldValue, newValue) =>
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
    public static BindableProperty AnimatedProperty =
        BindableProperty.Create(nameof(Animated), typeof(bool), typeof(SvgImageButton), true);

    public static BindableProperty ImageInsetProperty =
        BindableProperty.Create(nameof(ImageInset), typeof(float), typeof(SvgImageButton), default(float),
            propertyChanged: (bindable, oldValue, newValue) => (bindable as IAuroraView)?.InvalidateSurface());

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

    public static BindableProperty AnimationScaleAmountProperty =
        BindableProperty.Create(nameof(AnimationScaleAmount), typeof(float), typeof(SvgImageButton), .1f,
            propertyChanged: (bindable, oldValue, newValue) => (bindable as IAuroraView)?.InvalidateSurface());

    public float AnimationScaleAmount
    {
        get => (float)GetValue(AnimationScaleAmountProperty);
        set => SetValue(AnimationScaleAmountProperty, value);
    }

    /// <summary>
    /// The animation easing property.
    /// </summary>
    public static BindableProperty AnimationEasingProperty =
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
    public static BindableProperty OverlayColorProperty =
        BindableProperty.Create(nameof(OverlayColor), typeof(Color), typeof(SvgImageButton), Colors.Transparent,
            propertyChanged: (bindable, oldValue, newValue) => (bindable as IAuroraView)?.InvalidateSurface());

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
    public static new BindableProperty BackgroundColorProperty =
        BindableProperty.Create(nameof(BackgroundColor), typeof(Color), typeof(SvgImageButton), Colors.Transparent,
            propertyChanged: (bindable, oldValue, newValue) => (bindable as IAuroraView)?.InvalidateSurface());

    /// <summary>
    /// Gets or sets the background color.
    /// </summary>
    /// <value>Expects a Color. Default Color.Transparent.</value>
    public new Color BackgroundColor
    {
        get { return (Color)GetValue(BackgroundColorProperty); }
        set { SetValue(BackgroundColorProperty, value); }
    }

    public static BindableProperty BackgroundShapeProperty =
        BindableProperty.Create(nameof(BackgroundShape), typeof(SvgImageButtonBackgroundShape), typeof(SvgImageButton), SvgImageButtonBackgroundShape.None,
            propertyChanged: (bindable, oldValue, newValue) => (bindable as IAuroraView)?.InvalidateSurface());

    public SvgImageButtonBackgroundShape BackgroundShape
    {
        get => (SvgImageButtonBackgroundShape)GetValue(BackgroundShapeProperty);
        set => SetValue(BackgroundShapeProperty, value);
    }

    public static BindableProperty CornerRadiusProperty =
        BindableProperty.Create(nameof(CornerRadius), typeof(double), typeof(SvgImageButton), 8d,
            propertyChanged: (bindable, oldValue, newValue) => (bindable as IAuroraView)?.InvalidateSurface());

    public double CornerRadius
    {
        get => (double)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    /// <summary>
    /// The command property. Fires on tap.
    /// </summary>
    public static BindableProperty CommandProperty =
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
    public static BindableProperty CommandParameterProperty =
        BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(SvgImageButton), default(object));

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
        this._svg?.Picture?.Dispose();

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

        if (propertyName.Equals(VisualElement.HeightProperty.PropertyName) ||
            propertyName.Equals(VisualElement.WidthProperty.PropertyName) ||
            propertyName.Equals(View.MarginProperty.PropertyName))
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

        var size = Math.Min((int)(info.Width - this.Margin.Left - this.Margin.Right), (int)(info.Height - this.Margin.Top - this.Margin.Bottom)) - (this.ImageInset * _scale);

        var left = (info.Width - (float)size) / 2f;
        var top = (info.Height - (float)size) / 2f;
        var right = left + (float)size;
        var bottom = top + (float)size;

        this._touchArea = new SKRect(left, top, right, bottom);

        canvas.Clear();

        if (this._svg != null)
        {
            var scaleAmount =
                this.MaxImageSize == Size.Zero
                    ? (float)Math.Min(this._touchArea.Width / this._svg.Picture.CullRect.Width, this._touchArea.Height / this._svg.Picture.CullRect.Height)
                    : 1f;

            var adjustmentAmount = (float)((scaleAmount * this.AnimationScaleAmount) * this._animationPercentage);
            scaleAmount = Math.Min(scaleAmount, scaleAmount + (this._animationPercentage > .5d ? -adjustmentAmount : adjustmentAmount));

            using (var backgroundPaint = new SKPaint())
            {
                backgroundPaint.Color = this.BackgroundColor.ToSKColor();
                backgroundPaint.IsAntialias = true;
                backgroundPaint.Style = SKPaintStyle.Fill;

                var halfSize = ((float)(Math.Min((int)(info.Rect.Height - this.Margin.Top - this.Margin.Bottom), (int)(info.Rect.Width - this.Margin.Left - this.Margin.Right)) / 2f)) * (1f - adjustmentAmount);

                var shapeRect =
                    new SKRect(
                        info.Rect.MidX - halfSize, info.Rect.MidY - halfSize,
                        info.Rect.MidX + halfSize, info.Rect.MidY + halfSize);

                // draw fill
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

            using (new SKAutoCanvasRestore(canvas, true))
            {
                var scale = SKMatrix.CreateScale(scaleAmount, scaleAmount);

                var translation = SKMatrix.CreateTranslation((info.Width - (this._svg.Picture.CullRect.Width * scaleAmount)) / 2f, (info.Height - (this._svg.Picture.CullRect.Height * scaleAmount)) / 2f);

                scale = scale.PostConcat(translation);

                if (this.OverlayColor != Colors.Transparent)
                {
                    using (var paint = new SKPaint { BlendMode = SKBlendMode.SrcATop, Style = SKPaintStyle.Fill, IsAntialias = true })
                    {
                        canvas.SaveLayer(info.Rect, null);
                        canvas.Clear();

                        canvas.DrawPicture(this._svg.Picture, ref scale);

                        paint.Color = this.OverlayColor.ToSKColor();
                        canvas.DrawPaint(paint);
                    }
                }
                else
                {
                    canvas.DrawPicture(this._svg.Picture, ref translation);
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

        var isTapInside = this._touchArea.Contains(e.Location.X, e.Location.Y);

        if (e.ActionType == SKTouchAction.Released && isTapInside)
        {
            this._lastTouchLocation = e.Location;
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

    /// <summary>
    /// Animate the specified reset.
    /// </summary>
    /// <param name="reset">If set to <c>true</c> reset.</param>
    private void Animate(bool reset = false)
    {
        if (this._lastTouchLocation == SKPoint.Empty && !this.Animated)
        {
            return;
        }

        const string animName = "ImageButtonAnimating";

        this.AbortAnimation(animName);
        this._animationPercentage = 0d;

        if (reset)
        {
            return;
        }

        var rippleAnimation = new Animation(x =>
        {
            this._animationPercentage = x;
            this.InvalidateSurface();
        });

        rippleAnimation.Commit(
            this, animName, length: 250, easing: this.AnimationEasing,
            finished: (percent, isFinished) =>
            {
                this._lastTouchLocation = SKPoint.Empty;
                this._animationPercentage = 0d;
                this.InvalidateSurface();
            });
    }
}
