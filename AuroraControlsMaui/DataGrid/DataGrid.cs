using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace AuroraControls.DataGrid;

/// <summary>
/// A simplified DataGrid control.
/// </summary>
public class DataGrid : AuroraViewBase
{
    /// <summary>
    /// Property for the item source that provides data to the grid.
    /// </summary>
    public static readonly BindableProperty ItemsSourceProperty =
        BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(DataGrid), null,
            propertyChanged: OnItemsSourceChanged);

    /// <summary>
    /// Property for the collection of columns in the grid.
    /// </summary>
    public static readonly BindableProperty ColumnsProperty =
        BindableProperty.Create(nameof(Columns), typeof(ObservableCollection<DataGridColumn>), typeof(DataGrid),
            defaultValueCreator: _ => new ObservableCollection<DataGridColumn>(),
            propertyChanged: OnColumnsChanged);

    /// <summary>
    /// Property for row height.
    /// </summary>
    public static readonly BindableProperty RowHeightProperty =
        BindableProperty.Create(nameof(RowHeight), typeof(int), typeof(DataGrid), 40);

    /// <summary>
    /// Property for header row height.
    /// </summary>
    public static readonly BindableProperty HeaderRowHeightProperty =
        BindableProperty.Create(nameof(HeaderRowHeight), typeof(int), typeof(DataGrid), 45);

    // Fields
    private readonly ObservableCollection<DataGridColumn> _columns;
    private IEnumerable? _itemsSource;
    private INotifyCollectionChanged? _observableItemsSource;
    private float _verticalOffset;
    private float _horizontalOffset;
    private int _firstVisibleRowIndex;
    private int _lastVisibleRowIndex;
    private bool _needsLayout = true;
    private SKPoint? _lastTouchLocation;

    /// <summary>
    /// Gets or sets the data source for the grid.
    /// </summary>
    public IEnumerable ItemsSource
    {
        get => (IEnumerable)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    /// <summary>
    /// Gets or sets the collection of columns.
    /// </summary>
    public ObservableCollection<DataGridColumn> Columns
    {
        get => (ObservableCollection<DataGridColumn>)GetValue(ColumnsProperty);
        set => SetValue(ColumnsProperty, value);
    }

    /// <summary>
    /// Gets or sets the height of data rows.
    /// </summary>
    public int RowHeight
    {
        get => (int)GetValue(RowHeightProperty);
        set => SetValue(RowHeightProperty, value);
    }

    /// <summary>
    /// Gets or sets the height of the header row.
    /// </summary>
    public int HeaderRowHeight
    {
        get => (int)GetValue(HeaderRowHeightProperty);
        set => SetValue(HeaderRowHeightProperty, value);
    }

    public DataGrid()
    {
        // Enable touch handling for scrolling and selection
        EnableTouchEvents = true;

        // Initialize columns collection
        _columns = new ObservableCollection<DataGridColumn>();
        _columns.CollectionChanged += OnColumnsCollectionChanged;
    }

    protected override void PaintControl(SKSurface surface, SKImageInfo info)
    {
        if (_needsLayout)
        {
            CalculateLayout(info);
            _needsLayout = false;
        }

        // Clear the canvas
        surface.Canvas.Clear(SKColors.White);

        // Draw the grid content
        DrawHeaders(surface.Canvas, info);
        DrawCells(surface.Canvas, info);
    }

    private void CalculateLayout(SKImageInfo info)
    {
        if (Columns == null || !Columns.Any())
        {
            return;
        }

        var availableWidth = info.Width;
        var totalExplicitWidth = 0d;
        var autoSizeColumns = new List<DataGridColumn>();

        // First pass: calculate explicit widths
        foreach (var column in Columns.Where(c => c.IsVisible))
        {
            if (column.Width > 0)
            {
                column.ActualWidth = column.Width * _scale;
                totalExplicitWidth += column.ActualWidth;
            }
            else
            {
                autoSizeColumns.Add(column);
            }
        }

        // Second pass: distribute remaining width among auto-size columns
        var remainingWidth = availableWidth - totalExplicitWidth;
        if (autoSizeColumns.Any() && remainingWidth > 0)
        {
            var autoWidth = remainingWidth / autoSizeColumns.Count;
            foreach (var column in autoSizeColumns)
            {
                column.ActualWidth = autoWidth;
            }
        }

        // Third pass: calculate X positions
        var currentX = 0d;
        foreach (var column in Columns.Where(c => c.IsVisible))
        {
            column.X = currentX;
            currentX += column.ActualWidth;
        }

        // Calculate visible row range
        var scaledHeaderRowHeight = HeaderRowHeight * _scale;
        var scaledRowHeight = RowHeight * _scale;
        var totalHeight = info.Height - scaledHeaderRowHeight;
        var rowCount = _itemsSource?.Cast<object>().Count() ?? 0;
        _firstVisibleRowIndex = (int)(_verticalOffset / scaledRowHeight);
        _lastVisibleRowIndex = Math.Min(
            _firstVisibleRowIndex + (int)(totalHeight / scaledRowHeight) + 1,
            rowCount - 1);
    }

    private void DrawCells(SKCanvas canvas, SKImageInfo info)
    {
        if (_itemsSource == null || Columns == null)
        {
            return;
        }

        var items = _itemsSource.Cast<object>().ToList();
        if (!items.Any())
        {
            return;
        }

        var scaledHeaderRowHeight = HeaderRowHeight * _scale;
        var scaledRowHeight = RowHeight * _scale;

        // Draw visible cells
        for (int rowIndex = _firstVisibleRowIndex; rowIndex <= _lastVisibleRowIndex; rowIndex++)
        {
            if (rowIndex >= items.Count)
            {
                break;
            }

            var item = items[rowIndex];
            var y = scaledHeaderRowHeight + (rowIndex * scaledRowHeight) - _verticalOffset;

            foreach (var column in Columns.Where(c => c.IsVisible))
            {
                var cellRect = new SKRect(
                    (float)column.X - _horizontalOffset,
                    y,
                    (float)(column.X + column.ActualWidth) - _horizontalOffset,
                    y + scaledRowHeight);

                // Skip if cell is not visible
                if (cellRect.Right < 0 || cellRect.Left > info.Width)
                {
                    continue;
                }

                var value = column.GetCellValue(item);
                column.DrawCell(canvas, cellRect, value, false); // TODO: Add selection support
            }
        }
    }

    private void DrawHeaders(SKCanvas canvas, SKImageInfo info)
    {
        if (Columns == null)
        {
            return;
        }

        var scaledHeaderRowHeight = HeaderRowHeight * _scale;

        foreach (var column in Columns.Where(c => c.IsVisible))
        {
            var headerRect = new SKRect(
                (float)column.X - _horizontalOffset,
                0,
                (float)(column.X + column.ActualWidth) - _horizontalOffset,
                scaledHeaderRowHeight);

            // Skip if header is not visible
            if (headerRect.Right < 0 || headerRect.Left > info.Width)
            {
                continue;
            }

            column.DrawHeader(canvas, headerRect, false); // TODO: Add header selection support
        }
    }

    // Touch handling for scrolling
    protected override void OnTouch(SKTouchEventArgs e)
    {
        base.OnTouch(e);

        if (!e.InContact)
        {
            _lastTouchLocation = null;
            return;
        }

        switch (e.ActionType)
        {
            case SKTouchAction.Pressed:
            {
                _lastTouchLocation = e.Location;
                break;
            }

            case SKTouchAction.Moved:
            {
                if (_lastTouchLocation.HasValue)
                {
                    var deltaX = e.Location.X - _lastTouchLocation.Value.X;
                    var deltaY = e.Location.Y - _lastTouchLocation.Value.Y;

                    _horizontalOffset = Math.Max(0, _horizontalOffset - deltaX);
                    _verticalOffset = Math.Max(0, _verticalOffset - deltaY);

                    _needsLayout = true;
                    InvalidateSurface();
                }

                _lastTouchLocation = e.Location;
                break;
            }
        }
    }

    private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var grid = (DataGrid)bindable;
        grid.UpdateItemsSource((IEnumerable)oldValue, (IEnumerable)newValue);
    }

    private void UpdateItemsSource(IEnumerable oldValue, IEnumerable newValue)
    {
        if (_observableItemsSource != null)
        {
            _observableItemsSource.CollectionChanged -= OnItemsSourceCollectionChanged;
        }

        _itemsSource = newValue;
        _observableItemsSource = newValue as INotifyCollectionChanged;

        if (_observableItemsSource != null)
        {
            _observableItemsSource.CollectionChanged += OnItemsSourceCollectionChanged;
        }

        _needsLayout = true;
        InvalidateSurface();
    }

    private void OnColumnsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs? e)
    {
        _needsLayout = true;
        InvalidateSurface();
    }

    private void OnItemsSourceCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        _needsLayout = true;
        InvalidateSurface();
    }

    private static void OnColumnsChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var grid = (DataGrid)bindable;
        grid.OnColumnsCollectionChanged(null, null);
    }
}
