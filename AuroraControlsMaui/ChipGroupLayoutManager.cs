using Microsoft.Maui.Layouts;

namespace AuroraControls;

public class ChipGroupLayoutManager : ILayoutManager
{
    private readonly List<Rect> _bounds = new();
    private readonly List<Size> _childSizes = new();
    private readonly List<bool> _childVisibility = new();

    public double Spacing { get; set; } = 8.0;

    public int MaxRowsBeforeOverflow { get; set; } = -1;

    public bool Scrollable { get; set; }

    public double XOffset { get; set; }

    public double HeightRequest { get; private set; }

    public double WidthRequest { get; private set; }

    public int Rows { get; private set; }

    public bool IsOverflow => MaxRowsBeforeOverflow > 0 && Rows > MaxRowsBeforeOverflow;

    public Size ArrangeChildren(Rect bounds, ILayoutManager layoutManager, IEnumerable<IView> children)
    {
        var childrenList = children.Where(c => c.Visibility != Visibility.Collapsed).ToList();

        ProcessLayout(childrenList, bounds.Width);

        for (int i = 0; i < Math.Min(childrenList.Count, _bounds.Count); i++)
        {
            var child = childrenList[i] as Chip;
            var childBounds = _bounds[i];
            var shouldBeVisible = i < _childVisibility.Count && _childVisibility[i];

            if (childBounds == Rect.Zero || !shouldBeVisible)
            {
                // Don't arrange chips that are overflowed - they get zero bounds
                // This effectively hides them without modifying their Visibility property
                child.Arrange(Rect.Zero);
                child.IsVisible = false;
                continue;
            }

            // Apply XOffset for scrolling
            childBounds.X += bounds.X + XOffset;
            childBounds.Y += bounds.Y;

            child.Arrange(childBounds);
            child.IsVisible = true;
        }

        return new Size(bounds.Width, HeightRequest);
    }

    public Size Measure(double widthConstraint, double heightConstraint, ILayoutManager layoutManager, IEnumerable<IView> children)
    {
        var childrenList = children.Where(c => c.Visibility != Visibility.Collapsed).ToList();

        ProcessLayout(childrenList, widthConstraint);

        return new Size(widthConstraint, HeightRequest);
    }

    // These are the required interface methods for newer MAUI versions
    public Size ArrangeChildren(Rect bounds)
    {
        return new Size(bounds.Width, HeightRequest);
    }

    public Size Measure(double widthConstraint, double heightConstraint)
    {
        return new Size(widthConstraint, HeightRequest);
    }

    private void ProcessLayout(IList<IView> children, double widthConstraint)
    {
        _childSizes.Clear();
        _bounds.Clear();
        _childVisibility.Clear();

        // Measure all children
        foreach (var child in children)
        {
            var childSize = child.Measure(widthConstraint, double.PositiveInfinity);
            var adjustedWidth = Math.Min(childSize.Width, widthConstraint);
            _childSizes.Add(new Size(adjustedWidth, childSize.Height));
        }

        LayoutChildren(children, widthConstraint);
    }

    private void LayoutChildren(IList<IView> children, double widthConstraint)
    {
        _bounds.Clear();
        _childVisibility.Clear();

        double x = 0;
        double y = 0;
        double rowHeight = 0;
        HeightRequest = 0;
        Rows = 1;

        for (int i = 0; i < children.Count; i++)
        {
            var child = children[i];
            if (child.Visibility == Visibility.Collapsed)
            {
                _bounds.Add(Rect.Zero);
                _childVisibility.Add(false);
                continue;
            }

            var childSize = _childSizes[i];

            if (!Scrollable)
            {
                var isNewLine = CheckNewLine(childSize.Width, x, widthConstraint);
                if (isNewLine)
                {
                    y += rowHeight + Spacing;
                    x = 0;
                    rowHeight = 0;
                    Rows++;
                }

                // Check if we've exceeded the maximum rows and hide the chip
                if (MaxRowsBeforeOverflow > 0 && Rows > MaxRowsBeforeOverflow)
                {
                    _bounds.Add(Rect.Zero);
                    _childVisibility.Add(false); // Mark as not visible
                    continue;
                }
            }

            if (childSize.Height > rowHeight)
            {
                rowHeight = childSize.Height;
            }

            var bounds = new Rect(x, y, childSize.Width, childSize.Height);
            _bounds.Add(bounds);
            _childVisibility.Add(true); // Mark as visible

            x += childSize.Width + Spacing;
        }

        if (x > 0)
        {
            x -= Spacing; // Remove the last spacing
        }

        HeightRequest = y + rowHeight;
        WidthRequest = x;
    }

    private bool CheckNewLine(double childWidth, double currentX, double widthConstraint)
    {
        return currentX + childWidth > widthConstraint && currentX > 0;
    }
}
