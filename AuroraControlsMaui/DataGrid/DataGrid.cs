using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.Maui.Dispatching;

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

    private const float ScrollDecelerationRate = 0.95f;
    private const float MinScrollVelocity = 1f;
    private readonly IDispatcherTimer? _scrollTimer;
    private readonly ObservableCollection<DataGridColumn> _columns;
    private IEnumerable? _itemsSource;
    private INotifyCollectionChanged? _observableItemsSource;
    private float _verticalOffset;
    private float _horizontalOffset;
    private int _firstVisibleRowIndex;
    private int _lastVisibleRowIndex;
    private bool _needsLayout = true;
    private SKPoint? _lastTouchLocation;
    private SKPoint? _lastTouchVelocity;
    private DateTime _lastTouchTime;
    private float _maxHorizontalOffset;
    private float _maxVerticalOffset;

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

        // Initialize scroll timer for inertial scrolling
        _scrollTimer = Application.Current?.Dispatcher?.CreateTimer();
        if (_scrollTimer != null)
        {
            _scrollTimer.Interval = TimeSpan.FromMilliseconds(16); // ~60fps
            _scrollTimer.Tick += OnScrollTimerTick;
        }
    }

    private void UpdateScrollBounds(SKImageInfo info)
    {
        if (Columns?.Any() != true || _itemsSource == null)
        {
            _maxHorizontalOffset = 0;
            _maxVerticalOffset = 0;
            return;
        }

        // Calculate total content width
        var totalWidth = Columns.Where(c => c.IsVisible).Sum(c => c.ActualWidth);
        _maxHorizontalOffset = Math.Max(0, (float)(totalWidth - info.Width));

        // Calculate total content height
        var rowCount = _itemsSource.Cast<object>().Count();
        var totalHeight = HeaderRowHeight + (rowCount * RowHeight);
        _maxVerticalOffset = Math.Max(0, totalHeight - info.Height);

        // Clamp current offsets to valid range
        _horizontalOffset = Math.Min(_maxHorizontalOffset, Math.Max(0, _horizontalOffset));
        _verticalOffset = Math.Min(_maxVerticalOffset, Math.Max(0, _verticalOffset));
    }

    private void OnScrollTimerTick(object? sender, EventArgs e)
    {
        if (_lastTouchVelocity == null || (_lastTouchVelocity.Value.X == 0 && _lastTouchVelocity.Value.Y == 0))
        {
            _scrollTimer?.Stop();
            return;
        }

        // Apply velocity with deceleration
        _horizontalOffset = Math.Min(_maxHorizontalOffset, Math.Max(0, _horizontalOffset - _lastTouchVelocity.Value.X));
        _verticalOffset = Math.Min(_maxVerticalOffset, Math.Max(0, _verticalOffset - _lastTouchVelocity.Value.Y));

        // Decelerate
        _lastTouchVelocity = new SKPoint(
            _lastTouchVelocity.Value.X * ScrollDecelerationRate,
            _lastTouchVelocity.Value.Y * ScrollDecelerationRate);

        // Stop if velocity is too low
        if (Math.Abs(_lastTouchVelocity.Value.X) < MinScrollVelocity &&
            Math.Abs(_lastTouchVelocity.Value.Y) < MinScrollVelocity)
        {
            _lastTouchVelocity = null;
            _scrollTimer?.Stop();
        }

        _needsLayout = true;
        InvalidateSurface();
    }

    protected override void OnTouch(SKTouchEventArgs e)
    {
        base.OnTouch(e);

        var currentTime = DateTime.Now;

        if (!e.InContact)
        {
            // Calculate final velocity when touch is released
            if (_lastTouchLocation.HasValue && _lastTouchTime != default)
            {
                var timeDelta = (float)(currentTime - _lastTouchTime).TotalSeconds;
                if (timeDelta > 0)
                {
                    _lastTouchVelocity = new SKPoint(
                        (e.Location.X - _lastTouchLocation.Value.X) / timeDelta * 0.5f,
                        (e.Location.Y - _lastTouchLocation.Value.Y) / timeDelta * 0.5f);
                    _scrollTimer?.Start();
                }
            }

            _lastTouchLocation = null;
            return;
        }

        switch (e.ActionType)
        {
            case SKTouchAction.Pressed:
                _scrollTimer?.Stop();
                _lastTouchVelocity = null;
                _lastTouchLocation = e.Location;
                _lastTouchTime = currentTime;
                break;

            case SKTouchAction.Moved:
                if (_lastTouchLocation.HasValue)
                {
                    var deltaX = e.Location.X - _lastTouchLocation.Value.X;
                    var deltaY = e.Location.Y - _lastTouchLocation.Value.Y;

                    _horizontalOffset = Math.Min(_maxHorizontalOffset, Math.Max(0, _horizontalOffset - deltaX));
                    _verticalOffset = Math.Min(_maxVerticalOffset, Math.Max(0, _verticalOffset - deltaY));

                    _needsLayout = true;
                    InvalidateSurface();
                }

                _lastTouchLocation = e.Location;
                _lastTouchTime = currentTime;
                break;
        }

        e.Handled = true;
    }

    protected override void PaintControl(SKSurface surface, SKImageInfo info)
    {
        UpdateScrollBounds(info);

        if (_needsLayout)
        {
            CalculateLayout(info);
            _needsLayout = false;
        }

        // Clear the canvas
        surface.Canvas.Clear(SKColors.White);

        // Draw content
        DrawHeaders(surface.Canvas, info);
        DrawCells(surface.Canvas, info);
    }

    private void CalculateLayout(SKImageInfo info)
    {
        if (Columns == null || !Columns.Any())
        {
            return;
        }

        float scale = (float)PlatformInfo.ScalingFactor;
        var availableWidth = info.Width;
        var totalExplicitWidth = 0d;
        var autoWidthColumns = new List<DataGridColumn>();
        var autoSizeColumns = new List<DataGridColumn>();

        // First pass: calculate explicit widths and identify auto columns
        foreach (var column in Columns.Where(c => c.IsVisible))
        {
            if (column.Width > 0)
            {
                column.ActualWidth = column.Width;
                totalExplicitWidth += column.Width;
            }
            else if (column.AutoSize)
            {
                autoSizeColumns.Add(column);
            }
            else
            {
                autoWidthColumns.Add(column);
            }
        }

        // Second pass: calculate content-based widths for auto-size columns
        if (autoSizeColumns.Any() && _itemsSource != null)
        {
            var items = _itemsSource.Cast<object>().ToList();
            foreach (var column in autoSizeColumns)
            {
                // Start with the header width
                double maxWidth = column.MeasureContentWidth(column.HeaderText ?? column.PropertyPath ?? string.Empty, scale);

                // Check each cell's content width
                foreach (var item in items)
                {
                    var value = column.GetCellValue(item);
                    var contentWidth = column.MeasureContentWidth(value, scale);
                    maxWidth = Math.Max(maxWidth, contentWidth);
                }

                column.ActualWidth = maxWidth;
                totalExplicitWidth += maxWidth;
            }
        }

        // Third pass: distribute remaining width among auto-width columns
        var remainingWidth = availableWidth - totalExplicitWidth;
        if (autoWidthColumns.Any() && remainingWidth > 0)
        {
            var autoWidth = remainingWidth / autoWidthColumns.Count;
            foreach (var column in autoWidthColumns)
            {
                column.ActualWidth = autoWidth;
            }
        }
        else if (autoWidthColumns.Any())
        {
            // If no space remains, give auto-width columns a minimum width
            var minWidth = 50 * scale;
            foreach (var column in autoWidthColumns)
            {
                column.ActualWidth = minWidth;
            }
        }

        // Fourth pass: calculate X positions
        var currentX = 0d;
        foreach (var column in Columns.Where(c => c.IsVisible))
        {
            column.X = currentX;
            currentX += column.ActualWidth;
        }

        // Calculate visible row range
        var totalHeight = info.Height - HeaderRowHeight;
        var rowCount = _itemsSource?.Cast<object>().Count() ?? 0;
        _firstVisibleRowIndex = (int)(_verticalOffset / RowHeight);
        _lastVisibleRowIndex = Math.Min(
            _firstVisibleRowIndex + (int)(totalHeight / RowHeight) + 1,
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

        // Draw visible cells
        for (int rowIndex = _firstVisibleRowIndex; rowIndex <= _lastVisibleRowIndex; rowIndex++)
        {
            if (rowIndex >= items.Count)
            {
                break;
            }

            var item = items[rowIndex];
            var y = HeaderRowHeight + (rowIndex * RowHeight) - _verticalOffset;

            foreach (var column in Columns.Where(c => c.IsVisible))
            {
                var cellRect = new SKRect(
                    (float)column.X - _horizontalOffset,
                    y,
                    (float)(column.X + column.ActualWidth) - _horizontalOffset,
                    y + RowHeight);

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

        foreach (var column in Columns.Where(c => c.IsVisible))
        {
            var headerRect = new SKRect(
                (float)column.X - _horizontalOffset,
                0,
                (float)(column.X + column.ActualWidth) - _horizontalOffset,
                HeaderRowHeight);

            // Skip if header is not visible
            if (headerRect.Right < 0 || headerRect.Left > info.Width)
            {
                continue;
            }

            column.DrawHeader(canvas, headerRect, false); // TODO: Add header selection support
        }
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

    private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is DataGrid grid)
        {
            if (grid._observableItemsSource != null)
            {
                grid._observableItemsSource.CollectionChanged -= grid.OnItemsSourceCollectionChanged;
            }

            grid._itemsSource = newValue as IEnumerable;
            grid._observableItemsSource = newValue as INotifyCollectionChanged;

            if (grid._observableItemsSource != null)
            {
                grid._observableItemsSource.CollectionChanged += grid.OnItemsSourceCollectionChanged;
            }

            grid._needsLayout = true;
            grid.InvalidateSurface();
        }
    }
}
