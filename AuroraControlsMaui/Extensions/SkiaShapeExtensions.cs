namespace AuroraControls;

/// <summary>
/// Skia shape extensions.
/// </summary>
public static class SkiaShapeExtensions
{
    /// <summary>
    /// Center the specified rect.
    /// </summary>
    /// <returns>The center of the provided SKRect.</returns>
    /// <param name="rect">Rect.</param>
    public static SKPoint Center(this SKRect rect)
    {
        return new SKPoint(rect.MidX, rect.MidY);
    }

    public static bool IsDistanceGreaterThan(this SKPoint pointA, SKPoint pointB, int maxDistance)
    {
        return Math.Abs(pointA.X - pointB.X) > maxDistance || Math.Abs(pointA.Y - pointB.Y) > maxDistance;
    }

    public static SKRect Inflate(this SKRect rect, Thickness thickness)
    {
        rect.Left -= (float)thickness.Left;
        rect.Top -= (float)thickness.Top;
        rect.Right += (float)thickness.Right;
        rect.Bottom += (float)thickness.Bottom;

        return rect;
    }

    public static SKRectI Inflate(this SKRectI rect, Thickness thickness)
    {
        rect.Left -= (int)thickness.Left;
        rect.Top -= (int)thickness.Top;
        rect.Right += (int)thickness.Right;
        rect.Bottom += (int)thickness.Bottom;

        return rect;
    }

    public static SKRect Subtract(this SKRect rect, Thickness thickness)
    {
        rect.Left += (float)thickness.Left;
        rect.Top += (float)thickness.Top;
        rect.Right -= (float)thickness.Right;
        rect.Bottom -= (float)thickness.Bottom;

        return rect;
    }

    public static SKRectI Subtract(this SKRectI rect, Thickness thickness)
    {
        rect.Left += (int)thickness.Left;
        rect.Top += (int)thickness.Top;
        rect.Right -= (int)thickness.Right;
        rect.Bottom -= (int)thickness.Bottom;

        return rect;
    }
}
