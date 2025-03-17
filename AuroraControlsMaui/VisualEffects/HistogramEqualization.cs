namespace AuroraControls.VisualEffects;

public class HistogramEqualization : VisualEffect
{
    public override SKImage ApplyEffect(SKImage image, SKSurface surface, SKImageInfo info, SKRect overrideRect) => InternalApplyEffect(image, surface, info.Rect);

    public override SKImage ApplyEffect(SKImage image, SKSurface surface, GRBackendRenderTarget info, SKRect overrideRect) => InternalApplyEffect(image, surface, info.Rect);

    private SKImage InternalApplyEffect(SKImage image, SKSurface surface, SKRect rect)
    {
        var canvas = surface.Canvas;

        using (var bitmap = SKBitmap.FromImage(image))
        {
            canvas.Clear();

            byte min = 255, max = 0;

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    var pixel = bitmap.GetPixel(x, y);
                    var intensity = (byte)((pixel.Red + pixel.Green + pixel.Blue) / 3);

                    if (pixel == SKColors.White || pixel == SKColors.Black || pixel == SKColors.Transparent ||
                        (pixel.Red == byte.MinValue && pixel.Green == byte.MinValue && pixel.Blue == byte.MinValue && pixel.Alpha == byte.MinValue))
                    {
                        continue;
                    }

                    if (min > intensity)
                    {
                        min = intensity;
                    }

                    if (max < intensity)
                    {
                        max = intensity;
                    }
                }
            }

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    var pixel = bitmap.GetPixel(x, y);

                    if (pixel == SKColors.White || pixel == SKColors.Black || pixel == SKColors.Transparent ||
                        (pixel.Red == byte.MinValue && pixel.Green == byte.MinValue && pixel.Blue == byte.MinValue && pixel.Alpha == byte.MinValue))
                    {
                        continue;
                    }

                    var red = (pixel.Red - min) * byte.MaxValue / (max - min);
                    var green = (pixel.Green - min) * byte.MaxValue / (max - min);
                    var blue = (pixel.Blue - min) * byte.MaxValue / (max - min);

                    bitmap.SetPixel(x, y, new SKColor((byte)red, (byte)green, (byte)blue));
                }
            }

            canvas.DrawBitmap(bitmap, rect);
        }

        return surface.Snapshot();
    }

    private uint[] ComputeHistogram(SKBitmap bitmap)
    {
        var histo = new uint[256];
        for (int y = 0; y < bitmap.Height; y++)
        {
            for (int x = 0; x < bitmap.Width; x++)
            {
                var pixel = bitmap.GetPixel(x, y);
                var pixelColor = (uint)(pixel.Red + pixel.Green + pixel.Blue) / 3;
                histo[pixelColor]++;
            }
        }

        return histo;
    }
}
