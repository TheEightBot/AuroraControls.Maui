namespace AuroraControls.ImageProcessing;

/// <summary>
/// Resize image processor.
/// </summary>
public class ResizeImage : ImageProcessingBase, IImageProcessor
{
    /// <summary>
    /// Gets the key.
    /// </summary>
    /// <value>The "ResizeImage" key.</value>
    public override string Key => nameof(ResizeImage);

    /// <summary>
    /// The max height property.
    /// </summary>
    public static readonly BindableProperty MaxHeightProperty =
        BindableProperty.Create(nameof(MaxHeight), typeof(int), typeof(ResizeImage), 100);

    /// <summary>
    /// Gets or sets the height of the max.
    /// </summary>
    /// <value>int value representing the desired max height.</value>
    public int MaxHeight
    {
        get => (int)GetValue(MaxHeightProperty);
        set => SetValue(MaxHeightProperty, value);
    }

    /// <summary>
    /// The max width property.
    /// </summary>
    public static readonly BindableProperty MaxWidthProperty =
        BindableProperty.Create(nameof(MaxWidth), typeof(int), typeof(ResizeImage), 100);

    /// <summary>
    /// Gets or sets the width of the max.
    /// </summary>
    /// <value>int value processing the desired max width.</value>
    public int MaxWidth
    {
        get => (int)GetValue(MaxWidthProperty);
        set => SetValue(MaxWidthProperty, value);
    }

    /// <summary>
    /// Processes the image.
    /// </summary>
    /// <returns>The SKBitmap image.</returns>
    /// <param name="processingImage">Processing image.</param>
    /// <param name="imageProcessor">Image processor.</param>
    public SKBitmap ProcessImage(SKBitmap processingImage, ImageProcessingBase imageProcessor)
    {
        if (imageProcessor is ResizeImage)
        {
            var resizeImageProcessor = imageProcessor as ResizeImage;
            int maxHeight = resizeImageProcessor.MaxHeight;
            int maxWidth = resizeImageProcessor.MaxWidth;

            var info = processingImage.Info;

            float supportedScale =
                info.Height > info.Width
                    ? (float)maxHeight / info.Height
                    : (float)maxWidth / info.Width;

            int scaledWidth = (int)(info.Width * supportedScale);
            int scaledHeight = (int)(info.Height * supportedScale);

            var newImageInfo = new SKImageInfo(scaledWidth, scaledHeight, processingImage.Info.ColorType);

            return processingImage.Resize(newImageInfo, SKFilterQuality.High);
        }

        return processingImage;
    }

    /// <summary>
    /// Resizes the image for export as stream.
    /// </summary>
    /// <returns>The image exported as a stream.</returns>
    /// <param name="imageBytes">Image bytes.</param>
    /// <param name="maxHeight">Max height.</param>
    /// <param name="maxWidth">Max width.</param>
    /// <param name="quality">Quality.</param>
    /// <param name="imageFormat">Image format; default is PNG.</param>
    /// <param name="streamDisposesData">If set to <c>true</c> stream disposes data.</param>
    public static Stream ResizeImageForExportAsStream(byte[] imageBytes, int maxHeight = 100, int maxWidth = 100, int quality = 80, SKEncodedImageFormat imageFormat = SKEncodedImageFormat.Png, bool streamDisposesData = true) => ResizeImageInternal(imageBytes, maxHeight, maxWidth, quality, imageFormat).AsStream(streamDisposesData);

    /// <summary>
    /// Resizes the image for export as byte[].
    /// </summary>
    /// <returns>The image exported as byte[].</returns>
    /// <param name="imageBytes">Image bytes.</param>
    /// <param name="maxHeight">Max height.</param>
    /// <param name="maxWidth">Max width.</param>
    /// <param name="quality">Quality.</param>
    /// <param name="imageFormat">Image format.</param>
    public static byte[] ResizeImageForExport(byte[] imageBytes, int maxHeight = 100, int maxWidth = 100, int quality = 80, SKEncodedImageFormat imageFormat = SKEncodedImageFormat.Png) => ResizeImageInternal(imageBytes, maxHeight, maxWidth, quality, imageFormat).ToArray();

    /// <summary>
    /// Internal method used for resizeing the image.
    /// </summary>
    /// <returns>The image as SKData.</returns>
    /// <param name="imageBytes">Image bytes.</param>
    /// <param name="maxHeight">Max height.</param>
    /// <param name="maxWidth">Max width.</param>
    /// <param name="quality">Quality.</param>
    /// <param name="imageFormat">Image format.</param>
    private static SKData ResizeImageInternal(byte[] imageBytes, int maxHeight = 100, int maxWidth = 100, int quality = 80, SKEncodedImageFormat imageFormat = SKEncodedImageFormat.Png)
    {
        using SKData data = SKData.CreateCopy(imageBytes);
        using SKCodec codec = SKCodec.Create(data);
        var info = codec.Info;

        float supportedScale =
            info.Height > info.Width
                ? (float)maxHeight / info.Height
                : (float)maxWidth / info.Width;

        int scaledWidth = (int)(info.Width * supportedScale);
        int scaledHeight = (int)(info.Height * supportedScale);

        // decode the bitmap at the nearest size
        SKBitmap bmp = null;
        try
        {
            bmp = SKBitmap.Decode(codec);

            SKImageInfo desired = new(scaledWidth, scaledHeight);
            bmp = bmp.Resize(desired, SKFilterQuality.High);

            using var image = SKImage.FromBitmap(bmp);
            return image.Encode(imageFormat, quality);
        }
        finally
        {
            bmp?.Dispose();
            bmp = null;
        }
    }
}
