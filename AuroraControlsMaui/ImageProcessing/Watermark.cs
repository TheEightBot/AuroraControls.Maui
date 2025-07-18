namespace AuroraControls.ImageProcessing;

/// <summary>
///     Watermark effect for images.
/// </summary>
public class Watermark : ImageProcessingBase, IImageProcessor
{
    public enum WatermarkLocation
    {
        Start,
        Center,
        End,
    }

    public static readonly BindableProperty BackgroundColorProperty =
        BindableProperty.Create(nameof(BackgroundColor), typeof(Color), typeof(Watermark));

    public static readonly BindableProperty BackgroundCornerRadiusProperty =
        BindableProperty.Create(nameof(BackgroundCornerRadius), typeof(double), typeof(Watermark), 4d);

    public static readonly BindableProperty FontSizeProperty =
        BindableProperty.Create(nameof(FontSize), typeof(double), typeof(Watermark), 24d);

    public static readonly BindableProperty ForegroundColorProperty =
        BindableProperty.Create(nameof(ForegroundColor), typeof(Color), typeof(Watermark), Colors.White.MultiplyAlpha(.5f));

    public static readonly BindableProperty HorizontalWatermarkLocationProperty =
        BindableProperty.Create(nameof(HorizontalWatermarkLocation), typeof(WatermarkLocation), typeof(Watermark), WatermarkLocation.End);

    public static readonly BindableProperty TypefaceProperty =
        BindableProperty.Create(nameof(Typeface), typeof(SKTypeface), typeof(Watermark));

    public static readonly BindableProperty VerticalWatermarkLocationProperty =
        BindableProperty.Create(nameof(VerticalWatermarkLocation), typeof(WatermarkLocation), typeof(Watermark), WatermarkLocation.End);

    public static readonly BindableProperty WatermarkPaddingProperty =
        BindableProperty.Create(nameof(WatermarkPadding), typeof(double), typeof(Watermark), 8d);

    public static readonly BindableProperty WatermarkTextProperty =
        BindableProperty.Create(nameof(WatermarkText), typeof(string), typeof(Watermark));

    /// <summary>
    ///     Gets the key.
    /// </summary>
    /// <value>The "Watermark" key.</value>
    public override string Key => nameof(Watermark);

    public WatermarkLocation HorizontalWatermarkLocation
    {
        get => (WatermarkLocation)this.GetValue(HorizontalWatermarkLocationProperty);
        set => this.SetValue(HorizontalWatermarkLocationProperty, value);
    }

    public WatermarkLocation VerticalWatermarkLocation
    {
        get => (WatermarkLocation)this.GetValue(VerticalWatermarkLocationProperty);
        set => this.SetValue(VerticalWatermarkLocationProperty, value);
    }

    public double WatermarkPadding
    {
        get => (double)this.GetValue(WatermarkPaddingProperty);
        set => this.SetValue(WatermarkPaddingProperty, value);
    }

    public string WatermarkText
    {
        get => (string)this.GetValue(WatermarkTextProperty);
        set => this.SetValue(WatermarkTextProperty, value);
    }

    public double FontSize
    {
        get => (double)this.GetValue(FontSizeProperty);
        set => this.SetValue(FontSizeProperty, value);
    }

    public SKTypeface Typeface
    {
        get => (SKTypeface)this.GetValue(TypefaceProperty);
        set => this.SetValue(TypefaceProperty, value);
    }

    public Color ForegroundColor
    {
        get => (Color)this.GetValue(ForegroundColorProperty);
        set => this.SetValue(ForegroundColorProperty, value);
    }

    public Color BackgroundColor
    {
        get => (Color)this.GetValue(BackgroundColorProperty);
        set => this.SetValue(BackgroundColorProperty, value);
    }

    public double BackgroundCornerRadius
    {
        get => (double)this.GetValue(BackgroundCornerRadiusProperty);
        set => this.SetValue(BackgroundCornerRadiusProperty, value);
    }

    public SKBitmap ProcessImage(SKBitmap processingImage, ImageProcessingBase imageProcessor)
    {
        string text = this.WatermarkText;

        if (processingImage == null || string.IsNullOrEmpty(text))
        {
            return processingImage;
        }

        if (imageProcessor is Watermark)
        {
            using SKCanvas canvas = new(processingImage);
            using SKPaint paint = new();
            paint.IsAntialias = true;
            paint.Style = SKPaintStyle.Fill;
            paint.TextSize = (float)this.FontSize;
            paint.Typeface = this.Typeface ?? PlatformInfo.DefaultTypeface;

            paint.EnsureHasValidFont(text);

            SKRect measuredText = SKRect.Empty;
            paint.MeasureText(text, ref measuredText);
            float x = 0f;
            float y = 0f;

            SKRect rect = new(0f, 0f, processingImage.Width, processingImage.Height);

            switch (this.HorizontalWatermarkLocation)
            {
                case WatermarkLocation.Center:
                    x = rect.MidX - measuredText.MidX;
                    break;
                case WatermarkLocation.End:
                    x = rect.Left + rect.Width - measuredText.Width - (float)this.WatermarkPadding;
                    break;
                case WatermarkLocation.Start:
                default:
                    x = rect.Left + (float)this.WatermarkPadding;
                    break;
            }

            switch (this.VerticalWatermarkLocation)
            {
                case WatermarkLocation.Center:
                    y = rect.MidY - (measuredText.Height / 2f);
                    break;
                case WatermarkLocation.End:
                    y = rect.Top + rect.Height - (measuredText.Height / 2f) - (float)this.WatermarkPadding;
                    break;
                case WatermarkLocation.Start:
                default:
                    y = rect.Top + (measuredText.Height / 2f) + (float)this.WatermarkPadding;
                    break;
            }

            paint.Color = this.BackgroundColor.ToSKColor();
            float halfPadding = (float)this.WatermarkPadding / 2f;
            canvas.DrawRoundRect(
                x - halfPadding, y - (measuredText.Height / 2f) - halfPadding,
                measuredText.Width + (float)this.WatermarkPadding, measuredText.Height + (float)this.WatermarkPadding,
                (float)this.BackgroundCornerRadius, (float)this.BackgroundCornerRadius,
                paint);

            paint.Color = this.ForegroundColor.ToSKColor();
            canvas.DrawTextCenteredVertically(text, new SKPoint(x, y), paint);

            canvas.Flush();

            return processingImage;
        }

        return processingImage;
    }
}
