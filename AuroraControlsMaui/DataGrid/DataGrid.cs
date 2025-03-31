using System.Collections;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using Microsoft.Maui.Dispatching;

namespace AuroraControls.DataGrid;

/// <summary>
/// A simplified DataGrid control.
/// </summary>
public class DataGrid : AuroraViewBase, IDisposable
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
    private const int MaxSampledRowsForAutoSize = 100;
    private const int MaxPaintPoolSize = 32;
    private readonly IDispatcherTimer? _scrollTimer;
    private readonly ObservableCollection<DataGridColumn> _columns;
    private readonly Dictionary<string, SKRect> _textMeasurementCache = new();
    private readonly ConcurrentQueue<SKPaint> _paintPool = new();
    private readonly SKPaint _cellPaint;
    private readonly SKPaint _headerPaint;
    private readonly SKPaint _borderPaint;
    private readonly SKPaint _bgPaint;
    private readonly Stopwatch _renderStopwatch = new();
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
    private bool _disposed;

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

        // Initialize reusable paint objects with optimized text rendering
        _cellPaint = new SKPaint
        {
            IsAntialias = true, SubpixelText = true, LcdRenderText = true, FilterQuality = SKFilterQuality.High,
        };

        _headerPaint = new SKPaint
        {
            IsAntialias = true,
            SubpixelText = true,
            LcdRenderText = true,
            FilterQuality = SKFilterQuality.High,
            FakeBoldText = true,
        };

        _borderPaint = new SKPaint
        {
            Color = new SKColor(220, 220, 220), Style = SKPaintStyle.Stroke, IsAntialias = true,
        };

        _bgPaint = new SKPaint { Style = SKPaintStyle.Fill };

        // Initialize scroll timer for inertial scrolling
        _scrollTimer = Application.Current?.Dispatcher?.CreateTimer();
        if (_scrollTimer != null)
        {
            _scrollTimer.Interval = TimeSpan.FromMilliseconds(16); // ~60fps
            _scrollTimer.Tick += OnScrollTimerTick;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _cellPaint?.Dispose();
                _headerPaint?.Dispose();
                _borderPaint?.Dispose();
                _bgPaint?.Dispose();

                while (_paintPool.TryDequeue(out var paint))
                {
                    paint.Dispose();
                }
            }

            _disposed = true;
        }
    }

    ~DataGrid()
    {
        Dispose(false);
    }

    private string GetCacheKey(object value, float fontSize, SKTextAlign alignment)
    {
        return $"{value}_{fontSize}_{alignment}";
    }

    private SKRect MeasureTextWithCache(object value, float fontSize, SKTextAlign alignment, SKPaint paint)
    {
        var key = GetCacheKey(value, fontSize, alignment);
        if (_textMeasurementCache.TryGetValue(key, out var cachedRect))
        {
            return cachedRect;
        }

        var text = value?.ToString() ?? string.Empty;
        var rect = default(SKRect);
        paint.TextSize = fontSize;
        paint.TextAlign = alignment;
        paint.MeasureText(text, ref rect);
        _textMeasurementCache[key] = rect;
        return rect;
    }

    private void ClearMeasurementCache()
    {
        _textMeasurementCache.Clear();
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
        _renderStopwatch.Restart();

        var scrollBoundsStart = _renderStopwatch.ElapsedMilliseconds;
        UpdateScrollBounds(info);
        var scrollBoundsDuration = _renderStopwatch.ElapsedMilliseconds - scrollBoundsStart;
        Debug.WriteLine($"DataGrid scroll bounds update took {scrollBoundsDuration}ms");

        if (_needsLayout)
        {
            var layoutStart = _renderStopwatch.ElapsedMilliseconds;
            CalculateLayout(info);
            _needsLayout = false;
            var layoutDuration = _renderStopwatch.ElapsedMilliseconds - layoutStart;
            Debug.WriteLine($"DataGrid layout calculation took {layoutDuration}ms");
        }

        // Clear the canvas
        surface.Canvas.Clear(SKColors.White);

        // Draw content
        var headersStart = _renderStopwatch.ElapsedMilliseconds;
        DrawHeaders(surface.Canvas, info);
        var headersDuration = _renderStopwatch.ElapsedMilliseconds - headersStart;
        Debug.WriteLine($"DataGrid headers rendering took {headersDuration}ms");

        var cellsStart = _renderStopwatch.ElapsedMilliseconds;
        DrawCells(surface.Canvas, info);
        var cellsDuration = _renderStopwatch.ElapsedMilliseconds - cellsStart;
        Debug.WriteLine($"DataGrid cells rendering took {cellsDuration}ms");

        _renderStopwatch.Stop();
        var totalDuration = _renderStopwatch.ElapsedMilliseconds;
        Debug.WriteLine($"DataGrid total render time: {totalDuration}ms");
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
        var minAutoWidth = 50 * scale;

        // Pre-filter visible columns to avoid multiple enumerations
        var visibleColumns = Columns.Where(c => c.IsVisible).ToArray();
        if (visibleColumns.Length == 0)
        {
            return;
        }

        // Single pass to categorize columns and calculate explicit widths
        var autoWidthColumns = new List<DataGridColumn>(visibleColumns.Length);
        var autoSizeColumns = new List<DataGridColumn>(visibleColumns.Length);

        foreach (var column in visibleColumns)
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

        // Calculate auto-size column widths only if we have items
        if (autoSizeColumns.Count > 0 && _itemsSource != null)
        {
            // Cache header text measurements
            var headerWidths = new Dictionary<DataGridColumn, double>(autoSizeColumns.Count);
            foreach (var column in autoSizeColumns)
            {
                var headerText = column.HeaderText ?? column.PropertyPath ?? string.Empty;
                headerWidths[column] = column.MeasureContentWidth(headerText, scale);
            }

            // Only enumerate items once and calculate all column widths in parallel
            var items = _itemsSource.Cast<object>().ToArray();
            if (items.Length > 0)
            {
                // Initialize width tracking
                var columnMaxWidths = new double[autoSizeColumns.Count];
                for (int i = 0; i < autoSizeColumns.Count; i++)
                {
                    columnMaxWidths[i] = headerWidths[autoSizeColumns[i]];
                }

                // Sample a subset of rows for large datasets
                var samplingStep = Math.Max(1, items.Length / MaxSampledRowsForAutoSize);
                var sampledItems = items.Length > MaxSampledRowsForAutoSize
                    ? items.Where((_, index) => index % samplingStep == 0).ToArray()
                    : items;

                // Calculate max widths for all columns in a single pass through sampled items
                foreach (var item in sampledItems)
                {
                    for (int colIndex = 0; colIndex < autoSizeColumns.Count; colIndex++)
                    {
                        var column = autoSizeColumns[colIndex];
                        var value = column.GetCellValue(item);
                        var width = column.MeasureContentWidth(value, scale);
                        if (width > columnMaxWidths[colIndex])
                        {
                            columnMaxWidths[colIndex] = width;
                        }
                    }
                }

                // Add a small buffer to account for sampling
                if (items.Length > MaxSampledRowsForAutoSize)
                {
                    for (int i = 0; i < columnMaxWidths.Length; i++)
                    {
                        columnMaxWidths[i] *= 1.1; // Add 10% buffer
                    }
                }

                // Apply calculated widths
                for (int i = 0; i < autoSizeColumns.Count; i++)
                {
                    var column = autoSizeColumns[i];
                    column.ActualWidth = columnMaxWidths[i];
                    totalExplicitWidth += column.ActualWidth;
                }
            }
            else
            {
                // No items, just use header widths
                foreach (var column in autoSizeColumns)
                {
                    column.ActualWidth = headerWidths[column];
                    totalExplicitWidth += column.ActualWidth;
                }
            }
        }

        // Distribute remaining width to auto-width columns
        var remainingWidth = Math.Max(0, availableWidth - totalExplicitWidth);
        if (autoWidthColumns.Count > 0)
        {
            var autoWidth = remainingWidth > 0 ?
                remainingWidth / autoWidthColumns.Count :
                minAutoWidth;

            foreach (var column in autoWidthColumns)
            {
                column.ActualWidth = autoWidth;
            }
        }

        // Calculate X positions in a single pass
        var currentX = 0d;
        foreach (var column in visibleColumns)
        {
            column.X = currentX;
            currentX += column.ActualWidth;
        }

        // Calculate visible row range
        if (_itemsSource != null)
        {
            var totalHeight = info.Height - HeaderRowHeight;
            _firstVisibleRowIndex = (int)(_verticalOffset / RowHeight);
            _lastVisibleRowIndex = Math.Min(
                _firstVisibleRowIndex + (int)(totalHeight / RowHeight) + 1,
                (_itemsSource as ICollection)?.Count ?? _itemsSource.Cast<object>().Count() - 1);
        }
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

        _borderPaint.StrokeWidth = (float)PlatformInfo.ScalingFactor;

        // Create a small pool of paints for this draw operation
        var cellPaints = new List<SKPaint>();
        for (int i = 0; i < Math.Min(4, MaxPaintPoolSize); i++)
        {
            cellPaints.Add(AcquirePaint());
        }

        var currentPaintIndex = 0;

        try
        {
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

                    // Rotate through available paints
                    var paint = cellPaints[currentPaintIndex];
                    currentPaintIndex = (currentPaintIndex + 1) % cellPaints.Count;

                    var value = column.GetCellValue(item);
                    column.DrawCell(canvas, cellRect, value, false, paint, _bgPaint);

                    // Draw right border
                    canvas.DrawLine(cellRect.Right, cellRect.Top, cellRect.Right, cellRect.Bottom, _borderPaint);
                }

                // Draw bottom border
                canvas.DrawLine(0, y + RowHeight, info.Width, y + RowHeight, _borderPaint);
            }
        }
        finally
        {
            // Return paints to the pool
            foreach (var paint in cellPaints)
            {
                ReleasePaint(paint);
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

            column.DrawHeader(canvas, headerRect, false, _headerPaint, _bgPaint, _borderPaint);
        }

        // Draw bottom border of header row
        _borderPaint.StrokeWidth = (float)PlatformInfo.ScalingFactor;
        canvas.DrawLine(0, HeaderRowHeight, info.Width, HeaderRowHeight, _borderPaint);
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

    private SKPaint AcquirePaint()
    {
        if (_paintPool.TryDequeue(out var paint))
        {
            return paint;
        }

        return new SKPaint
        {
            IsAntialias = true,
            SubpixelText = true,
            LcdRenderText = true,
            FilterQuality = SKFilterQuality.High,
        };
    }

    private void ReleasePaint(SKPaint paint)
    {
        if (_paintPool.Count < MaxPaintPoolSize)
        {
            paint.Reset();
            _paintPool.Enqueue(paint);
        }
        else
        {
            paint.Dispose();
        }
    }
}
