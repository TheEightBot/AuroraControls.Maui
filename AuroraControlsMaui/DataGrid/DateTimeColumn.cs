using System.Globalization;
using System.Reflection;

namespace AuroraControls.DataGrid;

/// <summary>
/// A column type for displaying date and time values.
/// </summary>
public class DateTimeColumn : DataGridColumn
{
    /// <summary>
    /// Property for text alignment.
    /// </summary>
    public static readonly BindableProperty TextAlignmentProperty =
        BindableProperty.Create(nameof(TextAlignment), typeof(TextAlignment), typeof(DateTimeColumn), TextAlignment.Start);

    /// <summary>
    /// Property for text color.
    /// </summary>
    public static readonly BindableProperty TextColorProperty =
        BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(DateTimeColumn), Colors.Black);

    /// <summary>
    /// Property for font size.
    /// </summary>
    public static readonly BindableProperty FontSizeProperty =
        BindableProperty.Create(nameof(FontSize), typeof(float), typeof(DateTimeColumn), 14f);

    /// <summary>
    /// Property for date/time format string.
    /// </summary>
    public static readonly BindableProperty FormatStringProperty =
        BindableProperty.Create(nameof(FormatString), typeof(string), typeof(DateTimeColumn), "g");

    /// <summary>
    /// Gets or sets the text alignment.
    /// </summary>
    public TextAlignment TextAlignment
    {
        get => (TextAlignment)GetValue(TextAlignmentProperty);
        set => SetValue(TextAlignmentProperty, value);
    }

    /// <summary>
    /// Gets or sets the text color.
    /// </summary>
    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the font size.
    /// </summary>
    public float FontSize
    {
        get => (float)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the format string.
    /// </summary>
    public string FormatString
    {
        get => (string)GetValue(FormatStringProperty);
        set => SetValue(FormatStringProperty, value);
    }

    private static readonly CultureInfo CurrentCulture = CultureInfo.CurrentCulture;

    /// <summary>
    /// Gets the cell value using reflection.
    /// </summary>
    public override object GetCellValue(object item)
    {
        if (item == null || string.IsNullOrEmpty(PropertyPath))
        {
            return string.Empty;
        }

        var property = item.GetType().GetProperty(PropertyPath, BindingFlags.Public | BindingFlags.Instance);
        var value = property?.GetValue(item);

        if (value is DateTime dateTime)
        {
            return dateTime.ToString(FormatString, CurrentCulture);
        }

        return value?.ToString() ?? string.Empty;
    }

    /// <summary>
    /// Draws a cell with text content.
    /// </summary>
    public override void DrawCell(SKCanvas canvas, SKRect rect, object value, bool isSelected, SKPaint cellPaint, SKPaint bgPaint)
    {
        float scale = (float)PlatformInfo.ScalingFactor;
        float scaledFontSize = FontSize * scale;
        float scaledPadding = 8 * scale;

        // Configure paint for text rendering
        cellPaint.Color = TextColor.ToSKColor();
        cellPaint.TextSize = scaledFontSize;
        cellPaint.TextAlign = TextAlignment switch
        {
            TextAlignment.Start => SKTextAlign.Left,
            TextAlignment.Center => SKTextAlign.Center,
            TextAlignment.End => SKTextAlign.Right,
            _ => SKTextAlign.Left,
        };

        cellPaint.EnsureHasValidFont(value?.ToString() ?? string.Empty);

        // Draw selection background if selected
        if (isSelected)
        {
            bgPaint.Color = new SKColor(0, 120, 215, 50);
            canvas.DrawRect(rect, bgPaint);
        }

        // Calculate text position based on alignment and metrics
        float x = TextAlignment switch
        {
            TextAlignment.Start => rect.Left + scaledPadding,
            TextAlignment.Center => rect.MidX,
            TextAlignment.End => rect.Right - scaledPadding,
            _ => rect.Left + scaledPadding,
        };

        // Draw text with proper baseline alignment
        var text = value?.ToString() ?? string.Empty;
        var metrics = cellPaint.FontMetrics;
        var textOffset = ((rect.Height - (metrics.Descent - metrics.Ascent)) / 2) - metrics.Ascent;
        canvas.DrawText(text, x, rect.Top + textOffset, cellPaint);
    }

    /// <summary>
    /// Draws the column header.
    /// </summary>
    public override void DrawHeader(SKCanvas canvas, SKRect rect, bool isSelected, SKPaint headerPaint, SKPaint bgPaint, SKPaint borderPaint)
    {
        float scale = (float)PlatformInfo.ScalingFactor;
        float scaledFontSize = FontSize * scale;
        float scaledPadding = 8 * scale;

        // Draw header background
        bgPaint.Color = isSelected ? new SKColor(0, 120, 215) : new SKColor(240, 240, 240);
        canvas.DrawRect(rect, bgPaint);

        // Configure header text paint
        headerPaint.Color = isSelected ? SKColors.White : SKColors.Black;
        headerPaint.TextSize = scaledFontSize;
        headerPaint.TextAlign = TextAlignment switch
        {
            TextAlignment.Start => SKTextAlign.Left,
            TextAlignment.Center => SKTextAlign.Center,
            TextAlignment.End => SKTextAlign.Right,
            _ => SKTextAlign.Left,
        };

        headerPaint.EnsureHasValidFont(HeaderText ?? PropertyPath ?? string.Empty);

        float x = TextAlignment switch
        {
            TextAlignment.Start => rect.Left + scaledPadding,
            TextAlignment.Center => rect.MidX,
            TextAlignment.End => rect.Right - scaledPadding,
            _ => rect.Left + scaledPadding,
        };

        // Draw header text with proper baseline alignment
        var text = HeaderText ?? PropertyPath ?? string.Empty;
        var metrics = headerPaint.FontMetrics;
        var textOffset = ((rect.Height - (metrics.Descent - metrics.Ascent)) / 2) - metrics.Ascent;
        canvas.DrawText(text, x, rect.Top + textOffset, headerPaint);

        // Draw header border
        borderPaint.StrokeWidth = scale;
        canvas.DrawRect(rect, borderPaint);
    }

    /// <summary>
    /// Measures the content width for a given value.
    /// </summary>
    public override double MeasureContentWidth(object value, float scale)
    {
        var text = value?.ToString() ?? string.Empty;
        using var paint = new SKPaint
        {
            TextSize = FontSize * scale,
            IsAntialias = true,
            SubpixelText = true,
            LcdRenderText = true,
        };

        var rect = default(SKRect);
        paint.MeasureText(text, ref rect);

        // Add padding (scaled)
        var padding = 16 * scale; // 8 pixels on each side
        return rect.Width + padding;
    }
}
