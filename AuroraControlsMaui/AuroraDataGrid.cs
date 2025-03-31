using System.Collections;
using System.Collections.Specialized;

namespace AuroraControls;

public class AuroraDataGrid : AuroraViewBase
{
    public IList<GridColumn> Columns { get; set; } = new List<GridColumn>();

    private INotifyCollectionChanged? _observableItemsSource;

    private IEnumerable? _itemsSource;

    public IEnumerable? ItemsSource
    {
        get => _itemsSource;
        set
        {
            if (_observableItemsSource != null)
            {
                _observableItemsSource.CollectionChanged -= OnItemsSourceCollectionChanged;
            }

            _itemsSource = value;

            if (_itemsSource is INotifyCollectionChanged observable)
            {
                _observableItemsSource = observable;
                _observableItemsSource.CollectionChanged += OnItemsSourceCollectionChanged;
            }
            else
            {
                _observableItemsSource = null;
            }

            InvalidateSurface();
        }
    }

    public Func<object, int, DataGridCell?>? CellTemplate { get; set; }

    public bool AutoCalculateRowHeight { get; set; }

    public float DefaultRowHeight { get; set; } = 30;

    private float _scrollOffsetX;

    private float _scrollOffsetY;

    private float _lastTouchX;
    private float _lastTouchY;
    private bool _isTouchScrolling;

    public AuroraDataGrid()
    {
        // Enable touch handling
        _scrollOffsetX = 0;
        _scrollOffsetY = 0;

        this.EnableTouchEvents = true;
    }

    protected override void OnTouch(SKTouchEventArgs e)
    {
        e.Handled = true;

        switch (e.ActionType)
        {
            case SKTouchAction.Pressed:
                _lastTouchX = e.Location.X;
                _lastTouchY = e.Location.Y;

                // Check if a cell was tapped
                var tappedCell = GetCellAt(e.Location);
                if (tappedCell is EditableTextCell editableCell)
                {
                    editableCell.StartEditing(e.Location, Application.Current?.MainPage as ContentPage);
                    return;
                }

                _isTouchScrolling = true;
                break;

            case SKTouchAction.Moved:
                if (_isTouchScrolling)
                {
                    var deltaX = _lastTouchX - e.Location.X;
                    var deltaY = _lastTouchY - e.Location.Y;

                    _lastTouchX = e.Location.X;
                    _lastTouchY = e.Location.Y;

                    ScrollTo(_scrollOffsetX + deltaX, _scrollOffsetY + deltaY);
                }

                break;

            case SKTouchAction.Released:
            case SKTouchAction.Cancelled:
                _isTouchScrolling = false;
                break;
        }

        e.Handled = true;
    }

    private DataGridCell? GetCellAt(SKPoint location)
    {
        float currentY = -_scrollOffsetY;

        foreach (var row in ItemsSource.Cast<object>().Select((item, index) => new { item, index }))
        {
            float rowHeight = GetRowHeight(row.index);

            if (location.Y >= currentY && location.Y <= currentY + rowHeight)
            {
                float currentX = -_scrollOffsetX;

                foreach (var column in Columns.Select((col, index) => new { col, index }))
                {
                    float colWidth = column.col.Width * _scale;

                    if (location.X >= currentX && location.X <= currentX + colWidth)
                    {
                        return CellTemplate?.Invoke(row.item, column.index);
                    }

                    currentX += colWidth;
                }
            }

            currentY += rowHeight;
        }

        return null;
    }

    protected override void PaintControl(SKSurface surface, SKImageInfo info)
    {
        var canvas = surface.Canvas;
        canvas.Clear(SKColors.White);

        if (Columns == null || ItemsSource == null || CellTemplate == null)
        {
            return;
        }

        var items = ItemsSource.Cast<object>().ToList();

        float totalWidth = Columns.Sum(c => c.Width * _scale);
        float totalHeight = items.Sum(item => (AutoCalculateRowHeight ? CalculateRowHeight(item) : DefaultRowHeight) * _scale);

        float currentY = -_scrollOffsetY;
        int visibleRowCount = 0;
        for (int row = 0; row < items.Count; row++)
        {
            float rowHeight = GetRowHeight(row);

            if (currentY + rowHeight > info.Height)
            {
                break;
            }

            currentY += rowHeight;
            visibleRowCount++;
        }

        // Render one additional row vertically
        if (visibleRowCount < items.Count)
        {
            visibleRowCount++;
        }

        currentY = -_scrollOffsetY;

        // Draw grid lines and cells
        for (int row = 0; row < visibleRowCount; row++)
        {
            float rowHeight = GetRowHeight(row);
            float currentX = -_scrollOffsetX; // Adjust for horizontal scrolling

            for (int col = 0; col < Columns.Count; col++)
            {
                float colWidth = Columns[col].Width * _scale;
                var item = row < items.Count ? items[row] : null; // Handle additional row
                var cell = item != null ? CellTemplate(item, col) : null;
                var cellRect = new SKRect(currentX, currentY, currentX + colWidth, currentY + rowHeight);

                // Draw cell background
                canvas.DrawRect(cellRect, new SKPaint { Color = SKColors.LightGray });

                // Draw cell content
                cell?.Render(canvas, cellRect);

                // Draw cell border
                canvas.DrawRect(cellRect, new SKPaint { Color = SKColors.Black, Style = SKPaintStyle.Stroke });

                currentX += colWidth;
            }

            currentY += rowHeight;
        }
    }

    private float GetRowHeight(int rowIndex)
    {
        if (ItemsSource == null)
        {
            return DefaultRowHeight * _scale;
        }

        var items = ItemsSource.Cast<object>().ToList();
        if (rowIndex < 0 || rowIndex >= items.Count)
        {
            return DefaultRowHeight * _scale;
        }

        var item = items[rowIndex];
        return (AutoCalculateRowHeight ? CalculateRowHeight(item) : DefaultRowHeight) * _scale;
    }

    private float CalculateRowHeight(object item)
    {
        float maxHeight = DefaultRowHeight;

        for (int col = 0; col < Columns.Count; col++)
        {
            var cell = CellTemplate(item, col);
            if (cell != null)
            {
                float cellHeight = cell.GetContentHeight() * _scale;
                if (cellHeight > maxHeight)
                {
                    maxHeight = cellHeight;
                }
            }
        }

        return maxHeight;
    }

    public void ScrollTo(float offsetX, float offsetY)
    {
        // Clamp scrolling to prevent going out of bounds
        float maxScrollX = (float)Math.Max(0, Columns.Sum(c => c.Width * _scale) - Width);
        float maxScrollY = (float)Math.Max(0, ItemsSource.Cast<object>().Sum(item => (AutoCalculateRowHeight ? CalculateRowHeight(item) : DefaultRowHeight) * _scale) - Height);

        _scrollOffsetX = Math.Clamp(offsetX, 0, maxScrollX);
        _scrollOffsetY = Math.Clamp(offsetY, 0, maxScrollY);

        InvalidateSurface();
    }

    public void SetScale(float scale)
    {
        _scale = scale;
        InvalidateSurface();
    }

    private void OnItemsSourceCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
            case NotifyCollectionChangedAction.Remove:
            case NotifyCollectionChangedAction.Replace:
            case NotifyCollectionChangedAction.Move:
            case NotifyCollectionChangedAction.Reset:
                // Redraw the grid to reflect changes in the collection
                InvalidateSurface();
                break;
        }
    }
}

public class GridColumn
{
    public string Title { get; set; } = string.Empty;

    public float Width { get; set; } = 100;
}

public abstract class DataGridCell
{
    public int RowIndex { get; set; }

    public int ColumnIndex { get; set; }

    public Color BackgroundColor { get; set; } = Colors.LightGray;

    public Color BorderColor { get; set; } = Colors.Black;

    public abstract void Render(SKCanvas canvas, SKRect cellRect);

    public virtual float GetContentHeight() => 30; // Default content height

    protected SKColor ToSKColor(Color color)
    {
        return new SKColor((byte)(color.Red * 255), (byte)(color.Green * 255), (byte)(color.Blue * 255), (byte)(color.Alpha * 255));
    }
}

public class TextCell : DataGridCell
{
    public string Text { get; set; } = string.Empty;

    public override void Render(SKCanvas canvas, SKRect cellRect)
    {
        // Draw cell background
        var backgroundPaint = new SKPaint { Color = ToSKColor(BackgroundColor) };
        canvas.DrawRect(cellRect, backgroundPaint);

        // Draw text
        var textPaint = new SKPaint
        {
            Color = SKColors.Black,
            TextSize = 14,
            IsAntialias = true,
        };
        canvas.DrawText(Text, cellRect.Left + 5, cellRect.MidY + 5, textPaint);

        // Draw cell border
        var borderPaint = new SKPaint { Color = ToSKColor(BorderColor), Style = SKPaintStyle.Stroke };
        canvas.DrawRect(cellRect, borderPaint);
    }
}

public class NumberCell : DataGridCell
{
    public double Number { get; set; }

    public override void Render(SKCanvas canvas, SKRect cellRect)
    {
        // Draw cell background
        var backgroundPaint = new SKPaint { Color = ToSKColor(BackgroundColor) };
        canvas.DrawRect(cellRect, backgroundPaint);

        // Draw number
        var textPaint = new SKPaint
        {
            Color = SKColors.Black,
            TextSize = 14,
            IsAntialias = true,
        };
        canvas.DrawText(Number.ToString(), cellRect.Left + 5, cellRect.MidY + 5, textPaint);

        // Draw cell border
        var borderPaint = new SKPaint { Color = ToSKColor(BorderColor), Style = SKPaintStyle.Stroke };
        canvas.DrawRect(cellRect, borderPaint);
    }
}

public class CheckboxCell : DataGridCell
{
    public bool IsChecked { get; set; }

    public override void Render(SKCanvas canvas, SKRect cellRect)
    {
        // Draw cell background
        var backgroundPaint = new SKPaint { Color = ToSKColor(BackgroundColor) };
        canvas.DrawRect(cellRect, backgroundPaint);

        // Draw checkbox
        var checkboxSize = Math.Min(cellRect.Width, cellRect.Height) * 0.6f;
        var checkboxRect = new SKRect(
            cellRect.MidX - (checkboxSize / 2),
            cellRect.MidY - (checkboxSize / 2),
            cellRect.MidX + (checkboxSize / 2),
            cellRect.MidY + (checkboxSize / 2));

        var paint = new SKPaint
        {
            Color = ToSKColor(BorderColor),
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2,
        };
        canvas.DrawRect(checkboxRect, paint);

        if (IsChecked)
        {
            var checkPaint = new SKPaint
            {
                Color = ToSKColor(BorderColor),
                Style = SKPaintStyle.Fill,
            };
            canvas.DrawRect(checkboxRect, checkPaint);
        }

        // Draw cell border
        var borderPaint = new SKPaint { Color = ToSKColor(BorderColor), Style = SKPaintStyle.Stroke };
        canvas.DrawRect(cellRect, borderPaint);
    }
}

public class ImageCell : DataGridCell
{
    public SKBitmap? Image { get; set; }

    public override void Render(SKCanvas canvas, SKRect cellRect)
    {
        // Draw cell background
        var backgroundPaint = new SKPaint { Color = ToSKColor(BackgroundColor) };
        canvas.DrawRect(cellRect, backgroundPaint);

        // Draw image
        if (Image != null)
        {
            canvas.DrawBitmap(Image, cellRect);
        }

        // Draw cell border
        var borderPaint = new SKPaint { Color = ToSKColor(BorderColor), Style = SKPaintStyle.Stroke };
        canvas.DrawRect(cellRect, borderPaint);
    }
}

public class EditableTextCell : DataGridCell
{
    public string Text { get; set; } = string.Empty;

    public Action<int, int, string>? TextChanged { get; set; }

    private bool _isEditing;

    public override void Render(SKCanvas canvas, SKRect cellRect)
    {
        // Draw cell background
        var backgroundPaint = new SKPaint { Color = ToSKColor(BackgroundColor) };
        canvas.DrawRect(cellRect, backgroundPaint);

        // Draw text
        var textPaint = new SKPaint
        {
            Color = SKColors.Black,
            TextSize = 14,
            IsAntialias = true,
        };
        canvas.DrawText(Text, cellRect.Left + 5, cellRect.MidY + 5, textPaint);

        // Draw cell border
        var borderPaint = new SKPaint { Color = ToSKColor(BorderColor), Style = SKPaintStyle.Stroke };
        canvas.DrawRect(cellRect, borderPaint);
    }

    public void StartEditing(SKPoint touchPoint, ContentPage parentPage)
    {
        if (_isEditing)
        {
            return;
        }

        _isEditing = true;

        // Create an Entry control for text input
        var entry = new Entry
        {
            Text = Text,
            HorizontalOptions = LayoutOptions.FillAndExpand,
            VerticalOptions = LayoutOptions.Center,
            BackgroundColor = Microsoft.Maui.Graphics.Colors.Transparent,
            TextColor = Microsoft.Maui.Graphics.Colors.Black,
        };

        entry.Completed += (sender, args) =>
        {
            Text = entry.Text ?? string.Empty;
            TextChanged?.Invoke(RowIndex, ColumnIndex, Text);
            parentPage.Content = null; // Remove the Entry from the page
            _isEditing = false;
        };

        // Add the Entry to the page
        parentPage.Content = new StackLayout
        {
            Children = { entry },
            Padding = new Thickness(10),
            BackgroundColor = Microsoft.Maui.Graphics.Colors.White,
        };

        // Focus the Entry to bring up the software keyboard
        entry.Focus();
    }
}
