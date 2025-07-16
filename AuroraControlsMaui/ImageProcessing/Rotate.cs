namespace AuroraControls.ImageProcessing;

public class Rotate : ImageProcessingBase, IImageProcessor
{
    public enum RotationDegrees
    {
        Zero = 0,
        Ninety = 90,
        OneHundredAndEighty = 180,
        TwoHundredAndSeventy = 270,
        NegativeNinety = -90,
        NegativeOneHundredAndEighty = -180,
        NegativeTwoHundredAndSeventy = -270,
    }

    /// <summary>
    /// Gets the key.
    /// </summary>
    /// <value>The "ResizeImage" key.</value>
    public override string Key => nameof(Rotate);

    public static readonly BindableProperty RotationAmountProperty =
        BindableProperty.Create(nameof(RotationDegrees), typeof(object), typeof(Rotate), RotationDegrees.Zero);

    public RotationDegrees RotationAmount
    {
        get => (RotationDegrees)GetValue(RotationAmountProperty);
        set => SetValue(RotationAmountProperty, value);
    }

    /// <summary>
    /// Processes the image.
    /// </summary>
    /// <returns>The SKBitmap image.</returns>
    /// <param name="processingImage">Processing image.</param>
    /// <param name="imageProcessor">Image processor.</param>
    public SKBitmap ProcessImage(SKBitmap processingImage, ImageProcessingBase imageProcessor)
    {
        if (imageProcessor is AuroraControls.ImageProcessing.Rotate processor)
        {
            int width = 0;
            int height = 0;

            switch (processor.RotationAmount)
            {
                case RotationDegrees.Ninety:
                case RotationDegrees.TwoHundredAndSeventy:
                case RotationDegrees.NegativeNinety:
                case RotationDegrees.NegativeTwoHundredAndSeventy:
                    width = processingImage.Height;
                    height = processingImage.Width;
                    break;
                default:
                    height = processingImage.Height;
                    width = processingImage.Width;
                    break;
            }

            var bitmap = new SKBitmap(width, height, processingImage.AlphaType == SKAlphaType.Opaque);
            using var canvas = new SKCanvas(bitmap);
            canvas.Translate(width, 0);
            canvas.RotateDegrees((float)processor.RotationAmount);
            canvas.DrawBitmap(processingImage, 0, 0);
            canvas.Flush();

            return bitmap;
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

            SKImageInfo desired = new SKImageInfo(scaledWidth, scaledHeight);
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
