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
    public override object GetValue(object item)
    {
        if (item == null || string.IsNullOrEmpty(PropertyPath))
        {
            return null;
        }

        var property = item.GetType().GetProperty(PropertyPath);
        var value = property?.GetValue(item);

        return value is bool b ? b : false;
    }

    /// <summary>
    /// Draws a checkbox cell.
    /// </summary>
    public override void DrawCell(SKCanvas canvas, SKRect rect, object value, bool isSelected)
    {
        bool isChecked = value is bool b && b;

        // Calculate checkbox position
        float centerX = rect.MidX;
        float centerY = rect.MidY;
        float halfSize = CheckboxSize / 2;

        var checkboxRect = new SKRect(
            centerX - halfSize,
            centerY - halfSize,
            centerX + halfSize,
            centerY + halfSize);

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

        // Draw checkbox
        using (var paint = new SKPaint
        {
            Color = CheckboxColor.ToSKColor(),
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1.5f,
            IsAntialias = true,
        })
        {
            canvas.DrawRect(checkboxRect, paint);
        }

        // Draw checkmark if checked
        if (isChecked)
        {
            using var checkPaint = new SKPaint
            {
                Color = CheckboxColor.ToSKColor(),
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 2f,
                IsAntialias = true,
            };

            // Draw checkmark
            var path = new SKPath();
            path.MoveTo(checkboxRect.Left + (CheckboxSize * 0.2f), checkboxRect.MidY);
            path.LineTo(checkboxRect.Left + (CheckboxSize * 0.45f), checkboxRect.Bottom - (CheckboxSize * 0.2f));
            path.LineTo(checkboxRect.Right - (CheckboxSize * 0.2f), checkboxRect.Top + (CheckboxSize * 0.2f));

            canvas.DrawPath(path, checkPaint);
        }

        // Draw right border
        using var borderPaint = new SKPaint
        {
            Color = new SKColor(220, 220, 220),
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1,
        };
        canvas.DrawLine(rect.Right, rect.Top, rect.Right, rect.Bottom, borderPaint);
    }

    /// <summary>
    /// Draws the column header.
    /// </summary>
    public override void DrawHeader(SKCanvas canvas, SKRect rect, bool isSelected)
    {
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
            TextSize = 14,
            TextAlign = SKTextAlign.Center,
            FakeBoldText = true,
        };

        canvas.DrawText(HeaderText ?? PropertyPath ?? string.Empty, rect.MidX, rect.MidY + (14 / 3), paint);

        // Draw header border
        using var borderPaint = new SKPaint
        {
            Color = new SKColor(200, 200, 200),
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1,
        };
        canvas.DrawRect(rect, borderPaint);
    }
}
