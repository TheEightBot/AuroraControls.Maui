namespace AuroraControls.ImageProcessing;

/// <summary>
/// Scale image.
/// </summary>
public class Scale : ImageProcessingBase, IImageProcessor
{
    /// <summary>
    /// Gets the key.
    /// </summary>
    /// <value>The "Scale" key.</value>
    public override string Key => nameof(Scale);

    /// <summary>
    /// The scale amount property.
    /// </summary>
    public static readonly BindableProperty ScaleAmountProperty =
        BindableProperty.Create(nameof(ScaleAmount), typeof(double), typeof(Scale), 1d);

    /// <summary>
    /// Gets or sets the scale amount.
    /// </summary>
    /// <value>Expects a double. Default value is 1d.</value>
    public double ScaleAmount
    {
        get => (double)GetValue(ScaleAmountProperty);
        set => SetValue(ScaleAmountProperty, value);
    }

    /// <summary>
    /// Apply Scale process.
    /// </summary>
    /// <returns>The an SKBitmap image.</returns>
    /// <param name="processingImage">Processing SKBitmap image.</param>
    /// <param name="imageProcessor">Image processor.</param>
    public SKBitmap ProcessImage(SKBitmap processingImage, ImageProcessingBase imageProcessor)
    {
        if (imageProcessor is Scale)
        {
            var scaleProcessor = imageProcessor as Scale;

            float scaleAmount = (float)scaleProcessor.ScaleAmount;

            var info =
                new SKImageInfo(
                    (int)(processingImage.Info.Rect.Width * scaleAmount),
                    (int)(processingImage.Info.Rect.Height * scaleAmount),
                    processingImage.Info.ColorType);

            return processingImage.Resize(info, SKFilterQuality.High);
        }

        return processingImage;
    }
}
