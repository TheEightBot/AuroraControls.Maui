namespace AuroraControls.ImageProcessing;

/// <summary>
/// Sepia effect.
/// </summary>
public class Sepia : ImageProcessingBase, IImageProcessor
{
    /// <summary>
    /// Gets the key.
    /// </summary>
    /// <value>The "Sepia" key.</value>
    public override string Key => nameof(Sepia);

    /// <summary>
    /// Apply Scale process.
    /// </summary>
    /// <returns>The an SKBitmap image.</returns>
    /// <param name="processingImage">Processing SKBitmap image.</param>
    /// <param name="imageProcessor">Image processor.</param>
    public SKBitmap ProcessImage(SKBitmap processingImage, ImageProcessingBase imageProcessor)
    {
        var bitmap = new SKBitmap(processingImage.Info);

        using (var canvas = new SKCanvas(bitmap))
        using (var paint = new SKPaint())
        {
            paint.ColorFilter = SKColorFilter.CreateColorMatrix(
                [
                    0.393f, 0.769f, 0.189f, 0.0f, 0.0f,
                    0.349f, 0.686f, 0.168f, 0.0f, 0.0f,
                    0.272f, 0.534f, 0.131f, 0.0f, 0.0f,
                    0.0f,   0.0f,   0.0f,   1.0f, 0.0f
                ]);

            canvas.Clear();

            canvas.DrawBitmap(processingImage, processingImage.Info.Rect, paint);

            canvas.Flush();
        }

        return bitmap;
    }
}
