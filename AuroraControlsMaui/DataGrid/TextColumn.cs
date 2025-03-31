using System.Reflection;

namespace AuroraControls.DataGrid;

/// <summary>
/// A column type for displaying text data.
/// </summary>
public class TextColumn : DataGridColumn
{
    /// <summary>
    /// Property for text alignment.
    /// </summary>
    public static readonly BindableProperty TextAlignmentProperty =
        BindableProperty.Create(nameof(TextAlignment), typeof(TextAlignment), typeof(TextColumn), TextAlignment.Start);

    /// <summary>
    /// Property for text color.
    /// </summary>
    public static readonly BindableProperty TextColorProperty =
        BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(TextColumn), Colors.Black);

    /// <summary>
    /// Property for font size.
    /// </summary>
    public static readonly BindableProperty FontSizeProperty =
        BindableProperty.Create(nameof(FontSize), typeof(float), typeof(TextColumn), 14f);

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

        return value?.ToString() ?? string.Empty;
    }

    /// <summary>
    /// Draws a cell with text content.
    /// </summary>
    public override void DrawCell(SKCanvas canvas, SKRect rect, object value, bool isSelected)
    {
        float scale = (float)PlatformInfo.ScalingFactor;
        float scaledFontSize = FontSize * scale;
        float scaledPadding = 8 * scale;

        using var paint = new SKPaint
        {
            Color = TextColor.ToSKColor(),
            IsAntialias = true,
            TextSize = scaledFontSize,
            TextAlign = TextAlignment switch
            {
                TextAlignment.Start => SKTextAlign.Left,
                TextAlignment.Center => SKTextAlign.Center,
                TextAlignment.End => SKTextAlign.Right,
                _ => SKTextAlign.Left,
            },
        };

        // Draw selection background if selected
        if (isSelected)
        {
            using var bgPaint = new SKPaint
            {
                Color = new SKColor(0, 120, 215, 50),
                Style = SKPaintStyle.Fill,
            };
            canvas.DrawRect(rect, bgPaint);
        }

        // Calculate text position based on alignment
        float x = TextAlignment switch
        {
            TextAlignment.Start => rect.Left + scaledPadding,
            TextAlignment.Center => rect.MidX,
            TextAlignment.End => rect.Right - scaledPadding,
            _ => rect.Left + scaledPadding,
        };

        // Draw text
        var text = value?.ToString() ?? string.Empty;
        canvas.DrawText(text, x, rect.MidY + (scaledFontSize / 3), paint);

        // Draw right border
        using var borderPaint = new SKPaint
        {
            Color = new SKColor(220, 220, 220),
            Style = SKPaintStyle.Stroke,
            StrokeWidth = scale,
        };
        canvas.DrawLine(rect.Right, rect.Top, rect.Right, rect.Bottom, borderPaint);
    }

    /// <summary>
    /// Draws the column header.
    /// </summary>
    public override void DrawHeader(SKCanvas canvas, SKRect rect, bool isSelected)
    {
        float scale = (float)PlatformInfo.ScalingFactor;
        float scaledFontSize = FontSize * scale;
        float scaledPadding = 8 * scale;

        // Draw header background
        using var bgPaint = new SKPaint
        {
            Color = isSelected ? new SKColor(0, 120, 215) : new SKColor(240, 240, 240),
            Style = SKPaintStyle.Fill,
        };
        canvas.DrawRect(rect, bgPaint);

        // Draw header text
        using var paint = new SKPaint
        {
            Color = isSelected ? SKColors.White : SKColors.Black,
            IsAntialias = true,
            TextSize = scaledFontSize,
            TextAlign = TextAlignment switch
            {
                TextAlignment.Start => SKTextAlign.Left,
                TextAlignment.Center => SKTextAlign.Center,
                TextAlignment.End => SKTextAlign.Right,
                _ => SKTextAlign.Left,
            },
            FakeBoldText = true,
        };

        float x = TextAlignment switch
        {
            TextAlignment.Start => rect.Left + scaledPadding,
            TextAlignment.Center => rect.MidX,
            TextAlignment.End => rect.Right - scaledPadding,
            _ => rect.Left + scaledPadding,
        };

        canvas.DrawText(HeaderText ?? PropertyPath ?? string.Empty, x, rect.MidY + (scaledFontSize / 3), paint);

        // Draw header border
        using var borderPaint = new SKPaint
        {
            Color = new SKColor(200, 200, 200),
            Style = SKPaintStyle.Stroke,
            StrokeWidth = scale,
        };
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
        };

        var rect = default(SKRect);
        paint.MeasureText(text, ref rect);

        // Add padding (scaled)
        var padding = 16 * scale; // 8 pixels on each side
        return rect.Width + padding;
    }
}
