using AuroraControls.ImageProcessing;

namespace AuroraControls;

public static class ImageProcessor
{
    public static void ProcessImage(string inputPath, string outputPath, SKEncodedImageFormat imageFormat, int quality, params IImageProcessor[] imageProcessors)
    {
        var bitmap = SKBitmap.Decode(inputPath);

        if (bitmap == null)
        {
            return;
        }

        foreach (var imageProcessor in imageProcessors)
        {
            if (imageProcessor is not ImageProcessingBase ipb)
            {
                continue;
            }

            var outImage = imageProcessor.ProcessImage(bitmap, ipb);

            if (outImage != bitmap)
            {
                bitmap?.Dispose();
                bitmap = null;
            }

            bitmap = outImage;
        }

        using var skwStream = new SKFileWStream(outputPath);
        bitmap.Encode(skwStream, imageFormat, quality);
        skwStream.Flush();
    }
}
