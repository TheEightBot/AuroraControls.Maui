using System.Reflection;

namespace AuroraControls.DataGrid;

/// <summary>
/// A column type for displaying boolean values as checkboxes.
/// </summary>
public class BooleanColumn : DataGridColumn
{
    /// <summary>
    /// Property for checkbox size.
    /// </summary>
    public static readonly BindableProperty CheckboxSizeProperty =
        BindableProperty.Create(nameof(CheckboxSize), typeof(float), typeof(BooleanColumn), 16f);

    /// <summary>
    /// Property for checkbox color.
    /// </summary>
    public static readonly BindableProperty CheckboxColorProperty =
        BindableProperty.Create(nameof(CheckboxColor), typeof(Color), typeof(BooleanColumn), Colors.Black);

    /// <summary>
    /// Gets or sets the size of the checkbox.
    /// </summary>
    public float CheckboxSize
    {
        get => (float)GetValue(CheckboxSizeProperty);
        set => SetValue(CheckboxSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the color of the checkbox.
    /// </summary>
    public Color CheckboxColor
    {
        get => (Color)GetValue(CheckboxColorProperty);
        set => SetValue(CheckboxColorProperty, value);
    }

    /// <summary>
    /// Gets the boolean value from the data item.
    /// </summary>
    public override object GetCellValue(object item)
    {
        if (item == null || string.IsNullOrEmpty(PropertyPath))
        {
            return false;
        }

        var property = item.GetType().GetProperty(PropertyPath, BindingFlags.Public | BindingFlags.Instance);
        var value = property?.GetValue(item);

        return value is bool b ? b : false;
    }

    /// <summary>
    /// Draws a cell with a checkbox.
    /// </summary>
    public override void DrawCell(SKCanvas canvas, SKRect rect, object value, bool isSelected, SKPaint cellPaint, SKPaint bgPaint)
    {
        float scale = (float)PlatformInfo.ScalingFactor;
        float scaledSize = CheckboxSize * scale;

        // Draw selection background if selected
        if (isSelected)
        {
            bgPaint.Color = new SKColor(0, 120, 215, 50);
            canvas.DrawRect(rect, bgPaint);
        }

        // Calculate checkbox position with pixel alignment
        float x = (float)Math.Round(rect.MidX - (scaledSize / 2f));
        float y = (float)Math.Round(rect.MidY - (scaledSize / 2f));

        using (var checkboxPaint = new SKPaint
        {
            IsAntialias = true,
            SubpixelText = true,
            LcdRenderText = true,
            FilterQuality = SKFilterQuality.High,
            Color = CheckboxColor.ToSKColor(),
            StrokeWidth = scale,
        })
        {
            // Draw checkbox border
            checkboxPaint.Style = SKPaintStyle.Stroke;
            canvas.DrawRect(x, y, scaledSize, scaledSize, checkboxPaint);

            // Draw checkmark if checked
            if (value is bool isChecked && isChecked)
            {
                checkboxPaint.StrokeWidth = scale * 2;

                using (var path = new SKPath())
                {
                    float padding = scaledSize * 0.2f;
                    float left = x + padding;
                    float right = x + scaledSize - padding;
                    float top = y + padding;
                    float bottom = y + scaledSize - padding;
                    float middle = left + ((right - left) * 0.4f);

                    path.MoveTo((float)Math.Round(left), (float)Math.Round(y + (scaledSize * 0.5f)));
                    path.LineTo((float)Math.Round(middle), (float)Math.Round(bottom));
                    path.LineTo((float)Math.Round(right), (float)Math.Round(top));

                    canvas.DrawPath(path, checkboxPaint);
                }
            }
        }
    }

    /// <summary>
    /// Draws the column header.
    /// </summary>
    public override void DrawHeader(SKCanvas canvas, SKRect rect, bool isSelected, SKPaint headerPaint, SKPaint bgPaint, SKPaint borderPaint)
    {
        float scale = (float)PlatformInfo.ScalingFactor;

        // Draw header background
        bgPaint.Color = isSelected ? new SKColor(0, 120, 215) : new SKColor(240, 240, 240);
        canvas.DrawRect(rect, bgPaint);

        // Configure paint for text
        headerPaint.Color = isSelected ? SKColors.White : SKColors.Black;
        headerPaint.TextAlign = SKTextAlign.Center;
        headerPaint.EnsureHasValidFont(HeaderText ?? PropertyPath ?? string.Empty);

        // Draw text with proper baseline alignment
        var text = HeaderText ?? PropertyPath ?? string.Empty;
        var metrics = headerPaint.FontMetrics;
        var textOffset = ((rect.Height - (metrics.Descent - metrics.Ascent)) / 2) - metrics.Ascent;
        canvas.DrawText(text, rect.MidX, rect.Top + textOffset, headerPaint);

        // Draw header border
        borderPaint.StrokeWidth = scale;
        canvas.DrawRect(rect, borderPaint);
    }

    /// <summary>
    /// Measures the content width for a given value.
    /// </summary>
    public override double MeasureContentWidth(object value, float scale)
    {
        // Width is fixed based on checkbox size plus padding
        float padding = 16 * scale;
        return (CheckboxSize * scale) + padding;
    }
}
