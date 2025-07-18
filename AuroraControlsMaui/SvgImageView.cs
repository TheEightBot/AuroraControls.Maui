using Svg.Skia;
using Color = Microsoft.Maui.Graphics.Color;

namespace AuroraControls;

/// <summary>
/// An SVG based ImageView control.
/// </summary>
#pragma warning disable CA1001 // Types that own disposable fields should be disposable
public class SvgImageView : AuroraViewBase
#pragma warning restore CA1001 // Types that own disposable fields should be disposable
{
    private readonly object _pictureLock = new();

    private SKPaint _overlayPaint;

    private SKSvg _svg;

    private string _pictureName;

    /// <summary>
    /// The name of the embedded image to display.
    /// </summary>
    public static readonly BindableProperty EmbeddedImageNameProperty =
        BindableProperty.Create(nameof(EmbeddedImageName), typeof(string), typeof(SvgImageView),
            propertyChanged:
                (bindable, oldValue, newValue) =>
                {
                    if (bindable is SvgImageView cgv)
                    {
                        string? oldName = oldValue as string;
                        string? newName = newValue as string;

                        if (oldName?.Equals(newName) ?? false)
                        {
                            return;
                        }

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
        get => (string)GetValue(EmbeddedImageNameProperty);
        set => SetValue(EmbeddedImageNameProperty, value);
    }

    /// <summary>
    /// The overlay color property.
    /// </summary>
    public static readonly BindableProperty OverlayColorProperty =
        BindableProperty.Create(nameof(OverlayColor), typeof(Color), typeof(SvgImageView), Colors.Transparent,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the color of the overlay.
    /// </summary>
    /// <value>Expects a Xamarin.Forms.Color. Default is Transparent.</value>
    public Color OverlayColor
    {
        get => (Color)GetValue(OverlayColorProperty);
        set => SetValue(OverlayColorProperty, value);
    }

    /// <summary>
    /// The maximum embedded image size property.
    /// </summary>
    public static readonly BindableProperty MaxImageSizeProperty =
        BindableProperty.Create(nameof(MaxImageSize), typeof(Size), typeof(SvgImageView), default(Size),
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
    /// Initializes a new instance of the <see cref="SvgImageView"/> class.
    /// </summary>
    public SvgImageView() => MinimumHeightRequest = IAuroraView.StandardControlHeight;

    protected override void Attached()
    {
        this.EnableTouchEvents = true;

        _overlayPaint = new();

        SetSvgResource();

        base.Attached();
    }

    protected override void Detached()
    {
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

        double size = Math.Min(info.Width - Margin.Left - Margin.Right, info.Height - Margin.Top - Margin.Bottom);

        float left = (info.Width - (float)size) / 2f;
        float top = (info.Height - (float)size) / 2f;
        float right = left + (float)size;
        float bottom = top + (float)size;

        var displayArea = new SKRect(left, top, right, bottom);

        canvas.Clear();

        if (!string.IsNullOrEmpty(_pictureName))
        {
            float scaleAmount =
                this.MaxImageSize == Size.Zero
                    ? Math.Min(displayArea.Width / _svg.Picture.CullRect.Width, displayArea.Height / _svg.Picture.CullRect.Height)
                    : 1f;

            var scale = SKMatrix.CreateScale(scaleAmount, scaleAmount);

            var translation = SKMatrix.CreateTranslation((info.Width - (_svg.Picture.CullRect.Width * scaleAmount)) / 2f, (info.Height - (_svg.Picture.CullRect.Height * scaleAmount)) / 2f);

            scale = scale.PostConcat(translation);

            canvas.DrawPicture(_svg.Picture, ref scale);

            if (OverlayColor != Colors.Transparent)
            {
                using (new SKAutoCanvasRestore(canvas))
                {
                    _overlayPaint.BlendMode = SKBlendMode.SrcIn;

                    _overlayPaint.Color = OverlayColor.ToSKColor();
                    _overlayPaint.IsAntialias = true;

                    canvas.DrawPaint(_overlayPaint);
                }
            }
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
}
