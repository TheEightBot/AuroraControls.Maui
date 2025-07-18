namespace AuroraControls.ImageProcessing;

/// <summary>
/// Circular image mask.
/// </summary>
public class Circular : ImageProcessingBase, IImageProcessor
{
    /// <summary>
    /// Gets the key.
    /// </summary>
    /// <value>The "Circular" key.</value>
    public override string Key => nameof(Circular);

    /// <summary>
    /// Processes the image and apply circular mask.
    /// </summary>
    /// <returns>The an SKBitmap image.</returns>
    /// <param name="processingImage">Processing SKBitmap image.</param>
    /// <param name="imageProcessor">Image processor.</param>
    public SKBitmap ProcessImage(SKBitmap processingImage, ImageProcessingBase imageProcessor)
    {
        if (imageProcessor is not Circular)
        {
            return processingImage;
        }

        using var canvas = new SKCanvas(processingImage);
        using var paint = new SKPaint();
        paint.BlendMode = SKBlendMode.SrcIn;
        paint.IsAntialias = true;
        paint.Color = SKColors.Transparent;

        int size = Math.Min(processingImage.Info.Width, processingImage.Info.Height);

        float left = (processingImage.Info.Width - size) / 2f;
        float top = (processingImage.Info.Height - size) / 2f;
        float right = left + size;
        float bottom = top + size;

        var rect = new SKRect(left, top, right, bottom);

        using var outer = new SKPath();
        using var cutout = new SKPath();
        outer.AddRect(processingImage.Info.Rect);
        cutout.AddOval(rect);
        using var finalPath = outer.Op(cutout, SKPathOp.Difference);
        canvas.DrawPath(finalPath, paint);
        canvas.Flush();

        return processingImage;
    }
}
