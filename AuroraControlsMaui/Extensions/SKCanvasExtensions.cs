namespace AuroraControls;

internal static class SKCanvasExtensions
{
    public static void DrawTextCenteredVertically(this SKCanvas canvas, string text, SKPoint point, SKPaint paint)
    {
        using (new SKAutoCanvasRestore(canvas))
        {
            var size = default(SKRect);

            paint.EnsureHasValidFont(text);
            paint.MeasureText(text, ref size);

            var textRows = canvas.TextRows(text);

            canvas.Translate(0, (textRows * ((-paint.FontMetrics.Ascent + paint.FontMetrics.Descent) / 2f)) - paint.FontMetrics.Descent);

            canvas.DrawText(text, point.X, point.Y, paint);
        }
    }

    public static void DrawCenteredText(this SKCanvas canvas, string text, float midX, float midY, SKPaint paint)
    {
        using (new SKAutoCanvasRestore(canvas))
        {
            paint.EnsureHasValidFont(text);
            var size = canvas.TextSize(text, paint);
            var textRows = canvas.TextRows(text);

            canvas.Translate(-((float)size.Width * .5f), textRows * (((-paint.FontMetrics.Ascent + paint.FontMetrics.Descent) / 2f) - paint.FontMetrics.Descent));

            canvas.DrawText(text, midX, midY, paint);
        }
    }

    public static void DrawMultiLineText(this SKCanvas canvas, string text, float x, float y, SKPaint paint)
    {
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        var clonedPaint = paint.Clone();

        paint.EnsureHasValidFont(text);

        var splitText = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

        var size = default(SKRect);
        var heightOffset = -paint.FontMetrics.Ascent;

        foreach (var split in splitText)
        {
            if (string.IsNullOrEmpty(split))
            {
                paint.MeasureText("Xy", ref size);
            }
            else
            {
                paint.MeasureText(split, ref size);
                canvas.DrawText(split, x, y + heightOffset, paint);
            }

            heightOffset += paint.FontMetrics.Descent - paint.FontMetrics.Ascent + paint.FontMetrics.Leading;
        }
    }

    public static Size TextSize(this SKCanvas canvas, string text, SKPaint paint)
    {
        if (canvas is null)
        {
            throw new ArgumentNullException(nameof(canvas));
        }

        if (paint is null)
        {
            throw new ArgumentNullException(nameof(paint));
        }

        var size = default(SKRect);

        paint.EnsureHasValidFont(text);

        var maxWidth = 0f;
        var maxHeight = 0f;

        var splitText = text?.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

        foreach (var split in splitText)
        {
            if (!string.IsNullOrEmpty(split))
            {
                paint.MeasureText(split, ref size);

                if (size.Width > maxWidth)
                {
                    maxWidth = size.Width;
                }
            }

            maxHeight += paint.FontMetrics.Descent - paint.FontMetrics.Ascent + paint.FontMetrics.Leading;
        }

        return new Size(maxWidth, maxHeight);
    }

    public static SKRect TextLocation(this SKCanvas canvas, string text, float x, float y, SKPaint paint)
    {
        if (canvas is null)
        {
            throw new ArgumentNullException(nameof(canvas));
        }

        if (paint is null)
        {
            throw new ArgumentNullException(nameof(paint));
        }

        var size = default(SKRect);

        paint.EnsureHasValidFont(text);

        var maxWidth = 0f;
        var maxHeight = 0f;

        var offset = 0f;

        switch (paint.TextAlign)
        {
            case SKTextAlign.Center:
                offset = .5f;
                break;
            case SKTextAlign.Right:
                offset = .5f;
                break;
            default:
                break;
        }

        var splitText = text?.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

        foreach (var split in splitText)
        {
            if (string.IsNullOrEmpty(split))
            {
                paint.MeasureText("Xy", ref size);
                maxHeight += size.Height;
            }
            else
            {
                paint.MeasureText(split, ref size);
                maxHeight += size.Height;

                if (size.Width > maxWidth)
                {
                    maxWidth = size.Width;
                }
            }
        }

        var xLocation = x - (maxWidth * offset);
        var yLocation = y - Math.Abs(paint.FontMetrics.Ascent);
        return new SKRect(xLocation, yLocation, xLocation + maxWidth, yLocation + maxHeight);
    }

    public static int TextRows(this SKCanvas canvas, string text)
    {
        if (canvas is null)
        {
            throw new ArgumentNullException(nameof(canvas));
        }

        return Math.Max(text?.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)?.Length ?? 1, 1);
    }

    public static void DrawIconifiedText(this SKCanvas canvas, IEnumerable<SKTextRun> runs, float x, float y, SKPaint paint)
    {
        if (canvas is null)
        {
            throw new ArgumentNullException(nameof(canvas));
        }

        if (runs is null)
        {
            throw new ArgumentNullException(nameof(runs));
        }

        if (paint is null)
        {
            throw new ArgumentNullException(nameof(paint));
        }

        using (new SKAutoCanvasRestore(canvas))
        {
            var precalculatedDraw = new List<(SKTextRun Run, float Width, float VerticalCenter, SKPaint Paint)>();

            var totalWidth = 0f;

            foreach (var run in runs)
            {
                if (run.Text?.Length > 0)
                {
                    var newPaint = paint.Clone();

                    if (run.Typeface is not null)
                    {
                        newPaint.Typeface = run.Typeface;
                    }

                    newPaint.EnsureHasValidFont(run.Text);

                    if (run.FontSize is not null)
                    {
                        newPaint.TextSize = run.FontSize.Value;
                    }

                    if (run.Color is not null)
                    {
                        newPaint.Color = run.Color.Value;
                    }

                    var rect = default(SKRect);
                    var width = newPaint.MeasureText(run.Text, ref rect);

                    totalWidth += width;

                    precalculatedDraw.Add((run, width, ((-paint.FontMetrics.Ascent + paint.FontMetrics.Descent) / 2f) - paint.FontMetrics.Descent, newPaint));
                }
            }

            canvas.Translate(x - (totalWidth * .5f), 0);

            var offset = 0f;

            foreach (var draw in precalculatedDraw)
            {
                var yOffset = y + draw.VerticalCenter;
                canvas.Translate(0f, yOffset);

                canvas.DrawText(draw.Run.Text, offset + draw.Run.Offset.X, 0, draw.Paint);
                offset += draw.Width;
                draw.Paint?.Dispose();

                canvas.Translate(0f, -yOffset);
            }
        }
    }

    public static void DrawCenteredIconifiedText(this SKCanvas canvas, string text, float x, float y, SKPaint paint, bool toUppercase = false) => DrawCenteredIconifiedText(canvas, text, x, y, SKTextRunLookup.Instance, paint, toUppercase);

    public static void DrawCenteredIconifiedText(SKCanvas canvas, string text, float x, float y, SKTextRunLookup lookup, SKPaint paint, bool toUppercase = false)
    {
        if (canvas is null)
        {
            throw new ArgumentNullException(nameof(canvas));
        }

        if (text is null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        if (paint is null)
        {
            throw new ArgumentNullException(nameof(paint));
        }

        var runs = SKTextRun.Create(text, lookup, toUppercase);
        DrawIconifiedText(canvas, runs, x, y, paint);
    }

    public static SKRect MeasureIconifiedText(this SKPaint paint, string text, bool toUppercase = false)
    {
        var runs = SKTextRun.Create(text, SKTextRunLookup.Instance, toUppercase);

        var width = 0f;
        var maxHeight = 0f;

        foreach (var run in runs)
        {
            if (run.Text?.Length > 0)
            {
                using (var newPaint = paint.Clone())
                {
                    if (run.Typeface is not null)
                    {
                        newPaint.Typeface = run.Typeface;
                    }

                    newPaint.EnsureHasValidFont(run.Text);

                    if (run.FontSize is not null)
                    {
                        newPaint.TextSize = run.FontSize.Value;
                    }

                    if (run.Color is not null)
                    {
                        newPaint.Color = run.Color.Value;
                    }

                    var rect = default(SKRect);
                    newPaint.MeasureText(run.Text, ref rect);
                    width += rect.Width;

                    if (rect.Height > maxHeight)
                    {
                        maxHeight = rect.Height;
                    }
                }
            }
        }

        return new SKRect(0, 0, width, maxHeight);
    }

    public static void DrawText(this SKCanvas canvas, IEnumerable<SKTextRun> runs, float x, float y, SKPaint paint)
    {
        if (canvas is null)
        {
            throw new ArgumentNullException(nameof(canvas));
        }

        if (runs is null)
        {
            throw new ArgumentNullException(nameof(runs));
        }

        if (paint is null)
        {
            throw new ArgumentNullException(nameof(paint));
        }

        foreach (var run in runs)
        {
            using (var newPaint = paint.Clone())
            {
                if (run.Typeface is not null)
                {
                    newPaint.Typeface = run.Typeface;
                }

                newPaint.EnsureHasValidFont(run.Text);

                if (run.FontSize is not null)
                {
                    newPaint.TextSize = run.FontSize.Value;
                }

                if (run.Color is not null)
                {
                    newPaint.Color = run.Color.Value;
                }

                if (run.Text?.Length > 0)
                {
                    canvas.DrawText(run.Text, x + run.Offset.X, y + run.Offset.Y, newPaint);
                    x += newPaint.MeasureText(run.Text);
                }
            }
        }
    }

    public static void DrawIconifiedText(this SKCanvas canvas, string text, float x, float y, SKPaint paint) => canvas.DrawIconifiedText(text, x, y, SKTextRunLookup.Instance, paint);

    private static void DrawIconifiedText(this SKCanvas canvas, string text, float x, float y, SKTextRunLookup lookup, SKPaint paint)
    {
        if (canvas is null)
        {
            throw new ArgumentNullException(nameof(canvas));
        }

        if (text is null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        if (paint is null)
        {
            throw new ArgumentNullException(nameof(paint));
        }

        var runs = SKTextRun.Create(text, lookup);
        canvas.DrawText(runs, x, y, paint);
    }

    public static Size IconifiedTextSize(this SKCanvas canvas, string text, SKPaint paint)
    {
        if (canvas is null)
        {
            throw new ArgumentNullException(nameof(canvas));
        }

        if (text is null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        if (paint is null)
        {
            throw new ArgumentNullException(nameof(paint));
        }

        var runs = SKTextRun.Create(text, SKTextRunLookup.Instance);

        double height = 0d, width = 0d;

        foreach (var run in runs)
        {
            if (run.Text?.Length > 0)
            {
                using (var newPaint = paint.Clone())
                {
                    if (run.Typeface is not null)
                    {
                        newPaint.Typeface = run.Typeface;
                    }

                    newPaint.EnsureHasValidFont(run.Text);

                    if (run.FontSize is not null)
                    {
                        newPaint.TextSize = run.FontSize.Value;
                    }

                    if (run.Color is not null)
                    {
                        newPaint.Color = run.Color.Value;
                    }

                    var rect = default(SKRect);
                    var measuredWidth = newPaint.MeasureText(run.Text, ref rect);

                    if (rect.Height > height)
                    {
                        height = rect.Height;
                    }

                    width += measuredWidth;
                }
            }
        }

        return new Size(width, height);
    }
}
