﻿namespace AuroraControls.VisualEffects;

public class Pixelate : VisualEffect
{
    public enum PixelationPixelType
    {
        Square,
        Circle,
        Diamond,
    }

    /// <summary>
    /// The pixel size property.
    /// </summary>
    public static readonly BindableProperty PixelSizeProperty =
        BindableProperty.Create(nameof(PixelSize), typeof(int), typeof(Pixelate), 10);

    /// <summary>
    /// Gets or sets the pixel size.
    /// </summary>
    /// <value>Pixel size as an int. Default is 10.</value>
    public int PixelSize
    {
        get => (int)GetValue(PixelSizeProperty);
        set => SetValue(PixelSizeProperty, value);
    }

    /// <summary>
    /// The pixel type property.
    /// </summary>
    public static readonly BindableProperty PixelTypeProperty =
        BindableProperty.Create(nameof(PixelType), typeof(PixelationPixelType), typeof(Pixelate), PixelationPixelType.Square);

    /// <summary>
    /// Gets or sets the pixel type.
    /// </summary>
    /// <value>Takes a PixelationPixelType. Default is PixelationPixelType.Square.</value>
    public PixelationPixelType PixelType
    {
        get => (PixelationPixelType)GetValue(PixelTypeProperty);
        set => SetValue(PixelTypeProperty, value);
    }

    public override SKImage ApplyEffect(SKImage image, SKSurface surface, SKImageInfo info, SKRect overrideRect) => ApplyEffectInternal(surface, info.Rect);

    public override SKImage ApplyEffect(SKImage image, SKSurface surface, GRBackendRenderTarget info, SKRect overrideRect) => ApplyEffectInternal(surface, info.Rect);

    private SKImage ApplyEffectInternal(SKSurface surface, SKRect rect)
    {
        if (PixelSize <= 0)
        {
            return surface.Snapshot();
        }

        var canvas = surface.Canvas;

        float width = rect.Width;
        float height = rect.Height;

        float columns = (width / PixelSize) + 1;
        float rows = (height / PixelSize) + 1;

        float halfSize = PixelSize * .5f;

        float halfDiamondSize = PixelSize * .5f;

        // TODO: This seems super inefficient from a memory perspective
        var surfaceSnapshot = surface.Snapshot();
        using (var bitmap = SKBitmap.FromImage(surfaceSnapshot))
        using (var paint = new SKPaint())
        {
            surfaceSnapshot?.Dispose();
            surfaceSnapshot = null;

            canvas.Clear();

            paint.Style = SKPaintStyle.Fill;

            var pixelationType = this.PixelType;

            for (int row = 0; row < rows; row++)
            {
                float y = (row - 0.5f) * PixelSize;

                // normalize y so shapes around edges get color
                for (int column = 0; column < columns; column++)
                {
                    float x = (column - 0.5f) * PixelSize;

                    // normalize y so shapes around edges get color
                    int columnLoc = (int)Math.Min(column * PixelSize, width - PixelSize);
                    int rowLoc = (int)Math.Min(row * PixelSize, height - PixelSize);

                    try
                    {
                        paint.Color = bitmap.GetPixel(columnLoc, rowLoc);
                    }
                    catch
                    {
                    }

                    switch (pixelationType)
                    {
                        case PixelationPixelType.Square:
                            canvas.DrawRect(new SKRect(x - halfSize, y - halfSize, x + halfSize, y + halfSize), paint);
                            break;
                        case PixelationPixelType.Circle:
                            canvas.DrawCircle(x, y, halfSize, paint);
                            break;
                        case PixelationPixelType.Diamond:

                            using (new SKAutoCanvasRestore(canvas))
                            {
                                canvas.Translate(x, y);
                                canvas.RotateDegrees(45);
                                canvas.DrawRect(new SKRect(-halfDiamondSize, -halfDiamondSize, halfDiamondSize, halfDiamondSize), paint);
                            }

                            break;
                    }
                }
            }
        }

        return surface.Snapshot();
    }
}
