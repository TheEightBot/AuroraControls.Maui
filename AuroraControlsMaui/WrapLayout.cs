using System.Collections;
using System.Collections.Specialized;
using Microsoft.Maui.Controls.Internals;
using Microsoft.Maui.Layouts;

namespace AuroraControls;

/// <summary>
/// A layout that arranges child elements in rows or columns, wrapping to the next line when space runs out.
/// </summary>
public class WrapLayout : Layout
{
    /// <summary>
    /// The orientation property.
    /// </summary>
    public static readonly BindableProperty OrientationProperty =
        BindableProperty.Create(nameof(Orientation), typeof(StackOrientation), typeof(WrapLayout),
            StackOrientation.Horizontal, propertyChanged: OnLayoutPropertyChanged);

    /// <summary>
    /// The horizontal spacing property.
    /// </summary>
    public static readonly BindableProperty HorizontalSpacingProperty =
        BindableProperty.Create(nameof(HorizontalSpacing), typeof(double), typeof(WrapLayout),
            0.0, propertyChanged: OnLayoutPropertyChanged);

    /// <summary>
    /// The vertical spacing property.
    /// </summary>
    public static readonly BindableProperty VerticalSpacingProperty =
        BindableProperty.Create(nameof(VerticalSpacing), typeof(double), typeof(WrapLayout),
            0.0, propertyChanged: OnLayoutPropertyChanged);

    /// <summary>
    /// The horizontal options property for child alignment.
    /// </summary>
    public static readonly BindableProperty HorizontalOptionsProperty =
        BindableProperty.Create(nameof(HorizontalOptions), typeof(LayoutOptions), typeof(WrapLayout),
            LayoutOptions.Start, propertyChanged: OnLayoutPropertyChanged);

    /// <summary>
    /// The vertical options property for child alignment.
    /// </summary>
    public static readonly BindableProperty VerticalOptionsProperty =
        BindableProperty.Create(nameof(VerticalOptions), typeof(LayoutOptions), typeof(WrapLayout),
            LayoutOptions.Start, propertyChanged: OnLayoutPropertyChanged);

    /// <summary>
    /// Gets or sets the orientation of the wrap layout.
    /// </summary>
    public StackOrientation Orientation
    {
        get => (StackOrientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    /// <summary>
    /// Gets or sets the horizontal spacing between items.
    /// </summary>
    public double HorizontalSpacing
    {
        get => (double)GetValue(HorizontalSpacingProperty);
        set => SetValue(HorizontalSpacingProperty, value);
    }

    /// <summary>
    /// Gets or sets the vertical spacing between items.
    /// </summary>
    public double VerticalSpacing
    {
        get => (double)GetValue(VerticalSpacingProperty);
        set => SetValue(VerticalSpacingProperty, value);
    }

    /// <summary>
    /// Gets or sets the horizontal alignment options for children.
    /// </summary>
    public LayoutOptions HorizontalOptions
    {
        get => (LayoutOptions)GetValue(HorizontalOptionsProperty);
        set => SetValue(HorizontalOptionsProperty, value);
    }

    /// <summary>
    /// Gets or sets the vertical alignment options for children.
    /// </summary>
    public LayoutOptions VerticalOptions
    {
        get => (LayoutOptions)GetValue(VerticalOptionsProperty);
        set => SetValue(VerticalOptionsProperty, value);
    }

    private static void OnLayoutPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is WrapLayout layout)
        {
            layout.InvalidateMeasureNonVirtual(InvalidationTrigger.HorizontalOptionsChanged);
            #if ANDROID
            if (layout.Handler?.PlatformView is Android.Views.View platformView)
            {
                platformView.RequestLayout();
            }
            #endif
        }
    }

    protected override ILayoutManager CreateLayoutManager() => new WrapLayoutManager(this);
}

/// <summary>
/// Layout manager for the WrapLayout.
/// </summary>
public class WrapLayoutManager : ILayoutManager
{
    private readonly WrapLayout _layout;

    public WrapLayoutManager(WrapLayout layout)
    {
        _layout = layout;
    }

    public Size ArrangeChildren(Rect bounds)
    {
        var children = _layout.Children.Where(c => c.Visibility == Visibility.Visible).ToList();
        if (!children.Any())
        {
            return Size.Zero;
        }

        var orientation = _layout.Orientation;
        var horizontalSpacing = _layout.HorizontalSpacing;
        var verticalSpacing = _layout.VerticalSpacing;

        if (orientation == StackOrientation.Horizontal)
        {
            return ArrangeHorizontally(bounds, children, horizontalSpacing, verticalSpacing);
        }

        return ArrangeVertically(bounds, children, horizontalSpacing, verticalSpacing);
    }

    public Size Measure(double widthConstraint, double heightConstraint)
    {
        var children = _layout.Children.Where(c => c.Visibility == Visibility.Visible).ToList();
        if (!children.Any())
        {
            return Size.Zero;
        }

        var orientation = _layout.Orientation;
        var horizontalSpacing = _layout.HorizontalSpacing;
        var verticalSpacing = _layout.VerticalSpacing;

        if (orientation == StackOrientation.Horizontal)
        {
            return MeasureHorizontal(children, widthConstraint, heightConstraint, horizontalSpacing, verticalSpacing);
        }

        return MeasureVertical(children, widthConstraint, heightConstraint, horizontalSpacing, verticalSpacing);
    }

    private Size MeasureHorizontal(List<IView> children, double widthConstraint, double heightConstraint, double horizontalSpacing, double verticalSpacing)
    {
        if (double.IsInfinity(widthConstraint) || widthConstraint <= 0)
        {
            // If no width constraint, arrange all items in a single row
            var totalWidth = 0.0;
            var maxHeight = 0.0;
            var isFirst = true;

            foreach (var child in children)
            {
                var childSize = child.Measure(double.PositiveInfinity, double.PositiveInfinity);
                if (!isFirst)
                {
                    totalWidth += horizontalSpacing;
                }

                totalWidth += childSize.Width;
                maxHeight = Math.Max(maxHeight, childSize.Height);
                isFirst = false;
            }

            return new Size(totalWidth, maxHeight);
        }

        var totalLayoutWidth = 0.0;
        var totalLayoutHeight = 0.0;
        var currentRowWidth = 0.0;
        var currentRowHeight = 0.0;
        var isFirstInRow = true;
        var rowCount = 0;

        foreach (var child in children)
        {
            // Measure the child with available width constraint to get accurate sizing including padding/margins
            var availableWidth = Math.Max(0, widthConstraint - currentRowWidth - (isFirstInRow ? 0 : horizontalSpacing));
            var childSize = child.Measure(availableWidth, double.PositiveInfinity);
            var childWidth = childSize.Width;
            var childHeight = childSize.Height;

            // Calculate what the row width would be if we add this child
            var projectedRowWidth = currentRowWidth;
            if (!isFirstInRow)
            {
                projectedRowWidth += horizontalSpacing;
            }

            projectedRowWidth += childWidth;

            // Check if we need to wrap to next row
            // Always wrap if the projected width exceeds constraint, unless this is the first item in the row
            if (!isFirstInRow && projectedRowWidth > widthConstraint)
            {
                // Finalize current row
                totalLayoutWidth = Math.Max(totalLayoutWidth, currentRowWidth);
                if (rowCount > 0)
                {
                    totalLayoutHeight += verticalSpacing;
                }

                totalLayoutHeight += currentRowHeight;
                rowCount++;

                // Start new row with this child - re-measure with full width available
                var newChildSize = child.Measure(widthConstraint, double.PositiveInfinity);
                currentRowWidth = newChildSize.Width;
                currentRowHeight = newChildSize.Height;
                isFirstInRow = true;
            }
            else
            {
                // Add to current row
                currentRowWidth = projectedRowWidth;
                currentRowHeight = Math.Max(currentRowHeight, childHeight);
                isFirstInRow = false;
            }
        }

        // Finalize last row
        totalLayoutWidth = Math.Max(totalLayoutWidth, currentRowWidth);
        if (rowCount > 0)
        {
            totalLayoutHeight += verticalSpacing;
        }

        totalLayoutHeight += currentRowHeight;

        return new Size(totalLayoutWidth, totalLayoutHeight);
    }

    private Size MeasureVertical(List<IView> children, double widthConstraint, double heightConstraint, double horizontalSpacing, double verticalSpacing)
    {
        if (double.IsInfinity(heightConstraint) || heightConstraint <= 0)
        {
            // If no height constraint, arrange all items in a single column
            var totalHeight = 0.0;
            var maxWidth = 0.0;
            var isFirst = true;

            foreach (var child in children)
            {
                var childSize = child.Measure(double.PositiveInfinity, double.PositiveInfinity);
                if (!isFirst)
                {
                    totalHeight += verticalSpacing;
                }

                totalHeight += childSize.Height;
                maxWidth = Math.Max(maxWidth, childSize.Width);
                isFirst = false;
            }

            return new Size(maxWidth, totalHeight);
        }

        var totalLayoutWidth = 0.0;
        var totalLayoutHeight = 0.0;
        var currentColumnWidth = 0.0;
        var currentColumnHeight = 0.0;
        var isFirstInColumn = true;
        var columnCount = 0;

        foreach (var child in children)
        {
            var childSize = child.Measure(double.PositiveInfinity, double.PositiveInfinity);
            var childWidth = childSize.Width;
            var childHeight = childSize.Height;

            // Calculate what the column height would be if we add this child
            var projectedColumnHeight = currentColumnHeight;
            if (!isFirstInColumn)
            {
                projectedColumnHeight += verticalSpacing;
            }

            projectedColumnHeight += childHeight;

            // Check if we need to wrap to next column
            if (!isFirstInColumn && projectedColumnHeight > heightConstraint)
            {
                // Finalize current column
                totalLayoutHeight = Math.Max(totalLayoutHeight, currentColumnHeight);
                if (columnCount > 0)
                {
                    totalLayoutWidth += horizontalSpacing;
                }

                totalLayoutWidth += currentColumnWidth;
                columnCount++;

                // Start new column with this child
                currentColumnHeight = childHeight;
                currentColumnWidth = childWidth;
                isFirstInColumn = true;
            }
            else
            {
                // Add to current column
                currentColumnHeight = projectedColumnHeight;
                currentColumnWidth = Math.Max(currentColumnWidth, childWidth);
                isFirstInColumn = false;
            }
        }

        // Finalize last column
        totalLayoutHeight = Math.Max(totalLayoutHeight, currentColumnHeight);
        if (columnCount > 0)
        {
            totalLayoutWidth += horizontalSpacing;
        }

        totalLayoutWidth += currentColumnWidth;

        return new Size(totalLayoutWidth, totalLayoutHeight);
    }

    private Size ArrangeHorizontally(Rect bounds, List<IView> children, double horizontalSpacing, double verticalSpacing)
    {
        var x = bounds.X;
        var y = bounds.Y;
        var maxWidth = 0.0;
        var totalHeight = 0.0;

        // First pass: calculate row heights and positions using same logic as measure
        var rows = new List<List<(IView child, Size size)>>();
        var currentRow = new List<(IView child, Size size)>();
        var currentRowWidth = 0.0;
        var isFirstInRow = true;

        foreach (var child in children)
        {
            // Measure the child with available width constraint to get accurate sizing including padding/margins
            var availableWidth = Math.Max(0, bounds.Width - currentRowWidth - (isFirstInRow ? 0 : horizontalSpacing));
            var childSize = child.Measure(availableWidth, double.PositiveInfinity);
            var childWidth = childSize.Width;
            var childHeight = childSize.Height;

            // Calculate what the row width would be if we add this child
            var projectedRowWidth = currentRowWidth;
            if (!isFirstInRow)
            {
                projectedRowWidth += horizontalSpacing;
            }

            projectedRowWidth += childWidth;

            // Check if we need to wrap to next row
            // Always wrap if the projected width exceeds constraint, unless this is the first item in the row
            if (!isFirstInRow && projectedRowWidth > bounds.Width)
            {
                // Finalize current row
                if (currentRow.Any())
                {
                    rows.Add(currentRow);
                    currentRow = new List<(IView child, Size size)>();
                }

                // Start new row with this child - re-measure with full width available
                var newChildSize = child.Measure(bounds.Width, double.PositiveInfinity);
                currentRowWidth = newChildSize.Width;
                isFirstInRow = true;
                currentRow.Add((child, newChildSize));
            }
            else
            {
                currentRowWidth = projectedRowWidth;
                isFirstInRow = false;
                currentRow.Add((child, childSize));
            }
        }

        if (currentRow.Any())
        {
            rows.Add(currentRow);
        }

        // Second pass: arrange children
        var currentY = y;
        for (int i = 0; i < rows.Count; i++)
        {
            var row = rows[i];
            var rowHeight = row.Max(item => item.size.Height);
            var currentX = x;

            foreach (var (child, size) in row)
            {
                var childRect = new Rect(currentX, currentY, size.Width, size.Height);

                // Apply vertical alignment within the row
                if (_layout.VerticalOptions.Alignment == LayoutAlignment.Center)
                {
                    childRect.Y += (rowHeight - size.Height) / 2;
                }
                else if (_layout.VerticalOptions.Alignment == LayoutAlignment.End)
                {
                    childRect.Y += rowHeight - size.Height;
                }

                child.Arrange(childRect);
                currentX += size.Width + horizontalSpacing;
            }

            maxWidth = Math.Max(maxWidth, currentX - horizontalSpacing - x);

            // Add row height to total
            totalHeight += rowHeight;

            // Add vertical spacing only between rows (not after the last row)
            if (i < rows.Count - 1)
            {
                totalHeight += verticalSpacing;
            }

            // Move to next row position
            currentY += rowHeight + (i < rows.Count - 1 ? verticalSpacing : 0);
        }

        return new Size(maxWidth, totalHeight);
    }

    private Size ArrangeVertically(Rect bounds, List<IView> children, double horizontalSpacing, double verticalSpacing)
    {
        var x = bounds.X;
        var y = bounds.Y;
        var maxHeight = 0.0;
        var totalWidth = 0.0;

        // First pass: calculate column widths and positions using same logic as measure
        var columns = new List<List<(IView child, Size size)>>();
        var currentColumn = new List<(IView child, Size size)>();
        var currentColumnHeight = 0.0;
        var isFirstInColumn = true;

        foreach (var child in children)
        {
            var childSize = child.Measure(double.PositiveInfinity, double.PositiveInfinity);
            var childWidth = childSize.Width;
            var childHeight = childSize.Height;

            // Calculate what the column height would be if we add this child
            var projectedColumnHeight = currentColumnHeight;
            if (!isFirstInColumn)
            {
                projectedColumnHeight += verticalSpacing;
            }

            projectedColumnHeight += childHeight;

            // Check if we need to wrap to next column
            if (!isFirstInColumn && projectedColumnHeight > bounds.Height)
            {
                // Finalize current column
                if (currentColumn.Any())
                {
                    columns.Add(currentColumn);
                    currentColumn = new List<(IView child, Size size)>();
                }

                currentColumnHeight = childHeight;
                isFirstInColumn = true;
            }
            else
            {
                currentColumnHeight = projectedColumnHeight;
                isFirstInColumn = false;
            }

            currentColumn.Add((child, childSize));
        }

        if (currentColumn.Any())
        {
            columns.Add(currentColumn);
        }

        // Second pass: arrange children
        var currentX = x;
        for (int i = 0; i < columns.Count; i++)
        {
            var column = columns[i];
            var columnWidth = column.Max(item => item.size.Width);
            var currentY = y;

            foreach (var (child, size) in column)
            {
                var childRect = new Rect(currentX, currentY, size.Width, size.Height);

                // Apply horizontal alignment within the column
                if (_layout.HorizontalOptions.Alignment == LayoutAlignment.Center)
                {
                    childRect.X += (columnWidth - size.Width) / 2;
                }
                else if (_layout.HorizontalOptions.Alignment == LayoutAlignment.End)
                {
                    childRect.X += columnWidth - size.Width;
                }

                child.Arrange(childRect);
                currentY += size.Height + verticalSpacing;
            }

            // Calculate the actual height of this column (without the extra spacing at the end)
            var columnHeight = currentY - verticalSpacing - y;
            maxHeight = Math.Max(maxHeight, columnHeight);

            // Add column width to total
            totalWidth += columnWidth;

            // Add horizontal spacing only between columns (not after the last column)
            if (i < columns.Count - 1)
            {
                totalWidth += horizontalSpacing;
            }

            // Move to next column position
            currentX += columnWidth + (i < columns.Count - 1 ? horizontalSpacing : 0);
        }

        return new Size(totalWidth, maxHeight);
    }
}
