namespace AuroraControls.ImageProcessing;

/// <summary>
/// Invert image processing effect.
/// </summary>
public class Invert : ImageProcessingBase, IImageProcessor
{
    /// <summary>
    /// Gets the key.
    /// </summary>
    /// <value>The "Invert" key.</value>
    public override string Key => nameof(Invert);

    /// <summary>
    /// Processes the inversion image.
    /// </summary>
    /// <returns>An SKBitmap image.</returns>
    /// <param name="processingImage">Image to process.</param>
    /// <param name="imageProcessor">Image processor.</param>
    public SKBitmap ProcessImage(SKBitmap processingImage, ImageProcessingBase imageProcessor)
    {
        using var canvas = new SKCanvas(processingImage);
        using var paint = new SKPaint();
        paint.IsAntialias = true;
        paint.Style = SKPaintStyle.Fill;
        paint.ColorFilter = SKColorFilter.CreateColorMatrix(
        [
            -1f,  0f,  0f, 0f, 255f,
            0f, -1f,  0f, 0f, 255f,
            0f,  0f, -1f, 0f, 255f,
            0f,  0f,  0f, 1f, 0f
        ]);

        canvas.Clear();
        canvas.DrawBitmap(processingImage, processingImage.Info.Rect, paint);
        canvas.Flush();

        return processingImage;
    }
}
