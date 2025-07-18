namespace AuroraControls;

public enum TextDrawLocation
{
    At,
    Centered,
    Before,
}

public static class SkiaSharpTextExtensions
{
    public static SKRect GetTextContainerRectAt(
        this SKCanvas canvas, string text, SKPoint drawPoint, SKPaint paint,
        TextDrawLocation horizontalLocation = TextDrawLocation.At, TextDrawLocation verticalLocation = TextDrawLocation.At)
    {
        SKRect textBounds = default;
        paint.EnsureHasValidFont(text);
        paint.MeasureText(text, ref textBounds);

        var textContainerRect = new SKRect(drawPoint.X, drawPoint.Y, drawPoint.X + textBounds.Width, drawPoint.Y + textBounds.Height);

        float horizontalOffset = 0f;
        float verticalOffset = 0f;

        switch (horizontalLocation)
        {
            case TextDrawLocation.Before:
                horizontalOffset = textContainerRect.Width;
                break;
            case TextDrawLocation.Centered:
                horizontalOffset = textContainerRect.Width * .5f;
                break;
        }

        switch (verticalLocation)
        {
            case TextDrawLocation.Before:
                verticalOffset = textContainerRect.Height;
                break;
            case TextDrawLocation.Centered:
                verticalOffset = textContainerRect.Height * .5f;
                break;
        }

        return new SKRect(
            textContainerRect.Left - horizontalOffset,
            textContainerRect.Top - verticalOffset,
            textContainerRect.Left - horizontalOffset + textContainerRect.Width,
            textContainerRect.Top - verticalOffset + textContainerRect.Height);
    }

    public static (SKPoint Start, SKPoint End) GetBaselineAt(this SKCanvas canvas, string text, SKPoint drawPoint, SKPaint textPaint, SKPaint baselinePaint, float baselinePadding = 0f)
    {
        SKRect textBounds = default;
        textPaint.EnsureHasValidFont(text);
        textPaint.MeasureText(text, ref textBounds);

        float left = drawPoint.X; // We will always use this as a starting point
        float top = drawPoint.Y;
        float width = textBounds.Width;
        float baseline = -textBounds.Top + top + baselinePadding + baselinePaint.StrokeWidth;

        return (new SKPoint(left, baseline), new SKPoint(left + width, baseline));
    }

    public static SKRect GetTextDrawPointAt(this SKCanvas canvas, string text, SKPoint drawPoint, SKPaint paint)
    {
        SKRect textBounds = default;
        paint.EnsureHasValidFont(text);
        paint.MeasureText(text, ref textBounds);

        float top = drawPoint.Y;
        float textLeft = -textBounds.Left + drawPoint.X;
        float baseline = -textBounds.Top + top;

        return new SKRect(
            textLeft,
            baseline,
            textBounds.Width + textLeft,
            textBounds.Height + baseline);
    }

    public static void DrawTextAt(this SKCanvas canvas, string text, SKPoint drawPoint, SKPaint paint)
    {
        var fontDrawLocation = canvas.GetTextDrawPointAt(text, drawPoint, paint);

        canvas.DrawText(text, fontDrawLocation.Location, paint);
    }

    public static void DrawTextAt(
        this SKCanvas canvas, string text, SKPoint drawPoint, SKPaint paint,
        TextDrawLocation horizontalLocation = TextDrawLocation.At, TextDrawLocation verticalLocation = TextDrawLocation.At)
    {
        var containerRect = canvas.GetTextContainerRectAt(text, drawPoint, paint, horizontalLocation, verticalLocation);
        var fontDrawLocation = canvas.GetTextDrawPointAt(text, containerRect.Location, paint);

        canvas.DrawText(text, fontDrawLocation.Location, paint);
    }

    public static void DrawTextAtBaseline(
        this SKCanvas canvas, string text, SKPoint drawPoint, SKPaint textPaint, SKPaint baselinePaint, float baselinePadding = 0f,
        TextDrawLocation horizontalLocation = TextDrawLocation.At, TextDrawLocation verticalLocation = TextDrawLocation.At)
    {
        var containerRect = canvas.GetTextContainerRectAt(text, drawPoint, textPaint, horizontalLocation, verticalLocation);
        var baselinePoints = canvas.GetBaselineAt(text, containerRect.Location, textPaint, baselinePaint, baselinePadding);
        canvas.DrawLine(baselinePoints.Start, baselinePoints.End, baselinePaint);
    }

    public static void EnsureHasValidFont(this SKPaint fontPaint, string text)
    {
        fontPaint.Typeface ??= PlatformInfo.DefaultTypeface;

        // TODO: There is likely a better/faster way to do all of this
        if (!string.IsNullOrEmpty(text) && !fontPaint.Typeface.ContainsGlyphs(text))
        {
            foreach (var fontFamily in FontCache.Instance.RegisteredFonts)
            {
                foreach (var font in fontFamily.Value)
                {
                    if (font.ContainsGlyphs(text))
                    {
                        fontPaint.Typeface = font;
                        return;
                    }
                }
            }

            // If nothing good is found, YOLO and hope for a match...
            var matchedTypeface = SKFontManager.Default.MatchCharacter(text[0]);
            if (matchedTypeface is not null && fontPaint.Typeface != matchedTypeface)
            {
                fontPaint.Typeface = matchedTypeface;
            }
        }
    }
}
