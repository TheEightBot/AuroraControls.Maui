using System;
using SkiaSharp;

namespace AuroraControls.ImageProcessing;

/// <summary>
/// Image processor interface.
/// </summary>
public interface IImageProcessor
{
    /// <summary>
    /// Processes the image.
    /// </summary>
    /// <returns>The image.</returns>
    /// <param name="processingImage">Processing image.</param>
    /// <param name="imageProcessor">Image processor.</param>
    SKBitmap ProcessImage(SKBitmap processingImage, ImageProcessingBase imageProcessor);
}
