namespace AuroraControls.ImageProcessing;

/// <summary>
/// Grayscale image effect.
/// </summary>
public class Grayscale : ImageProcessingBase, IImageProcessor
{
    /// <summary>
    /// Gets the key.
    /// </summary>
    /// <value>The "Grayscale" key.</value>
    public override string Key => nameof(Grayscale);

    /// <summary>
    /// Apply grayscale filter.
    /// </summary>
    /// <returns>The an SKBitmap image.</returns>
    /// <param name="processingImage">Processing SKBitmap image.</param>
    /// <param name="imageProcessor">Image processor.</param>
    public SKBitmap ProcessImage(SKBitmap processingImage, ImageProcessingBase imageProcessor)
    {
        var bitmap = new SKBitmap(processingImage.Info);

        using var canvas = new SKCanvas(bitmap);
        using var paint = new SKPaint();

        paint.ColorFilter = SKColorFilter.CreateColorMatrix(
        [
            0.21f, 0.72f, 0.07f, 0.0f, 0.0f,
            0.21f, 0.72f, 0.07f, 0.0f, 0.0f,
            0.21f, 0.72f, 0.07f, 0.0f, 0.0f,
            0.0f,  0.0f,  0.0f,  1.0f, 0.0f
        ]);

        canvas.Clear();
        canvas.DrawBitmap(processingImage, processingImage.Info.Rect, paint);
        canvas.Flush();

        return bitmap;
    }
}
