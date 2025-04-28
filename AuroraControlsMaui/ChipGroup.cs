using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.Shapes;

namespace AuroraControls;

public class ChipGroup : Layout<Chip>
{
    private static readonly BindablePropertyKey SelectedChipsPropertyKey = BindableProperty.CreateReadOnly(nameof(SelectedChips), typeof(IEnumerable<Chip>), typeof(ChipGroup), new List<Chip>());

    public static readonly BindableProperty SelectedChipsProperty = SelectedChipsPropertyKey.BindableProperty;

    private readonly PanGestureRecognizer _panGesture;
    private readonly LayoutInfo _layoutInfo = new();

    private double _xOffset;
    private double _maxOffset;
    private double _finalXOffset;

    private Chip? _previouslySelectedChip;

    public event EventHandler<ChipTappedEventArgs> ChipTapped;

    public IEnumerable<Chip> SelectedChips
    {
        get => this.Children.Where(x => x.IsToggled).ToArray();
    }

    public IEnumerable<object> SelectedValues
    {
        get => this.SelectedChips.Select(x => x.Value).ToArray();
        set
        {
            var children = this.Children.ToArray();

            foreach (var child in children)
            {
                child.IsToggled = value.Contains(child.Value);
            }

            this.OnPropertyChanged();
        }
    }

    public object SelectedValue
    {
        get => this.SelectedChips.Select(x => x.Value).FirstOrDefault();
        set
        {
            var children = this.Children.ToArray();

            var hasToggled = false;
            foreach (var child in children)
            {
                if (!hasToggled)
                {
                    var isMatch = child?.Value?.Equals(value) ?? false;
                    child.IsToggled = isMatch;

                    if (isMatch)
                    {
                        hasToggled = true;
                    }

                    continue;
                }

                child.IsToggled = false;
            }

            this.OnPropertyChanged();
        }
    }

    public static BindableProperty ScrollableProperty =
        BindableProperty.Create(nameof(Scrollable), typeof(bool), typeof(ChipGroup), false);

    public bool Scrollable
    {
        get => (bool)this.GetValue(ScrollableProperty);
        set => this.SetValue(ScrollableProperty, value);
    }

    public static BindableProperty MaxRowsBeforeOverflowProperty =
        BindableProperty.Create(nameof(MaxRowsBeforeOverflow), typeof(int), typeof(ChipGroup), -1);

    public int MaxRowsBeforeOverflow
    {
        get => (int)this.GetValue(MaxRowsBeforeOverflowProperty);
        set => this.SetValue(MaxRowsBeforeOverflowProperty, value);
    }

    public static BindableProperty IsOverflowProperty =
        BindableProperty.Create(nameof(IsOverflow), typeof(bool), typeof(ChipGroup), false);

    public bool IsOverflow
    {
        get => (bool)this.GetValue(IsOverflowProperty);
        private set => this.SetValue(IsOverflowProperty, value);
    }

    public static BindableProperty IsSingleSelectionProperty =
        BindableProperty.Create(nameof(IsSingleSelection), typeof(bool), typeof(ChipGroup), false);

    public bool IsSingleSelection
    {
        get => (bool)this.GetValue(IsSingleSelectionProperty);
        set => this.SetValue(IsSingleSelectionProperty, value);
    }

    public static BindableProperty ChipSelectionDetectionProperty =
        BindableProperty.Create(nameof(ChipSelectionDetection), typeof(bool), typeof(ChipGroup), true);

    public bool ChipSelectionDetection
    {
        get => (bool)this.GetValue(ChipSelectionDetectionProperty);
        set => this.SetValue(ChipSelectionDetectionProperty, value);
    }

    public static BindableProperty SpacingProperty = BindableProperty.Create(nameof(Spacing), typeof(double), typeof(ChipGroup), 8d);

    public double Spacing
    {
        get
        {
            return (double)this.GetValue(SpacingProperty);
        }

        set
        {
            this.SetValue(SpacingProperty, value);
            InvalidateLayout();
        }
    }

    public ChipGroup()
    {
        _panGesture = new PanGestureRecognizer();
        this.GestureRecognizers.Add(_panGesture);
    }

    public async Task ScrollToContextAsync(object item, ScrollToPosition scrollToPosition = ScrollToPosition.Start, bool animated = true, uint rate = 16, uint length = 250, Easing easing = null)
    {
        var matchingChip = this.Children.FirstOrDefault(x => x.BindingContext == item);

        if (matchingChip == null)
        {
            return;
        }

        await ScrollToAsync(matchingChip, scrollToPosition, animated, rate, length, easing);
    }

    public async Task ScrollToValueAsync(object item, ScrollToPosition scrollToPosition = ScrollToPosition.Start, bool animated = true, uint rate = 16, uint length = 250, Easing easing = null)
    {
        var matchingChip = this.Children.FirstOrDefault(x => x.Value?.Equals(item) ?? false);

        if (matchingChip == null)
        {
            return;
        }

        await ScrollToAsync(matchingChip, scrollToPosition, animated, rate, length, easing);
    }

    public async Task ScrollToAsync(Chip chip, ScrollToPosition scrollToPosition = ScrollToPosition.Start, bool animated = true, uint rate = 16, uint length = 250, Easing easing = null)
    {
        if (this.Children.Contains(chip))
        {
            var containerXStart = chip.Bounds.Left + Math.Abs(_xOffset);

            var offsetAmount = 0d;

            switch (scrollToPosition)
            {
                case ScrollToPosition.Start:
                    offsetAmount = -containerXStart;
                    break;
                case ScrollToPosition.Center:
                    offsetAmount = -containerXStart + (this.Width * .5d) - (chip.Width * .5d);
                    break;
                default:
                    offsetAmount = -containerXStart + this.Width - chip.Width;
                    break;
            }

            if (animated)
            {
                await this.TransitionTo(
                    nameof(this.ScrollToAsync),
                    x => this.SetXOffset(x, false),
                    _xOffset,
                    offsetAmount,
                    rate,
                    length,
                    easing);

                this.SetXOffset(_xOffset, true);
                return;
            }

            this.SetXOffset(offsetAmount, true);
        }
    }

    protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName is null)
        {
            return;
        }

        if (!propertyName.Equals(WindowProperty.PropertyName))
        {
            return;
        }

        if (this.Window is null)
        {
            this.InternalChildrenOnCollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, Enumerable.Empty<Chip>(), this.Children.ToArray()));

            if (this.Children is INotifyCollectionChanged inccw)
            {
                inccw.CollectionChanged -= this.InternalChildrenOnCollectionChanged;
            }

            this._panGesture.PanUpdated -= this.PanUpdated;

            return;
        }

        this.InternalChildrenOnCollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, this.Children.ToArray()));

        if (this.Children is INotifyCollectionChanged incc)
        {
            incc.CollectionChanged -= this.InternalChildrenOnCollectionChanged;
            incc.CollectionChanged += this.InternalChildrenOnCollectionChanged;
        }

        this._panGesture.PanUpdated -= this.PanUpdated;
        this._panGesture.PanUpdated += this.PanUpdated;
    }

    protected override void LayoutChildren(double x, double y, double width, double height)
    {
        _layoutInfo.ProcessLayout(this.Children, this.Spacing, this.MaxRowsBeforeOverflow, this.Scrollable, width);

        _maxOffset = _layoutInfo.WidthRequest;

        for (int i = 0; i < _layoutInfo.Bounds.Count; i++)
        {
            if (!this.Children[i].IsVisible)
            {
                continue;
            }

            var bounds = _layoutInfo.Bounds[i];

            if (bounds == Rect.Zero)
            {
                continue;
            }

            bounds.Left += x + _xOffset;
            bounds.Top += y;
            LayoutChildIntoBoundingRegion(this.Children[i], bounds);
        }

        this.IsOverflow = _layoutInfo.IsOverflow;
    }

    protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
    {
        _layoutInfo.ProcessLayout(this.Children, this.Spacing, this.MaxRowsBeforeOverflow, this.Scrollable, widthConstraint);
        return new SizeRequest(new Size(widthConstraint, _layoutInfo.HeightRequest));
    }

    private void InternalChildrenOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Move)
        {
            return;
        }

        if (e.OldItems is not null)
        {
            foreach (var t in e.OldItems)
            {
                if (t is not Chip chip)
                {
                    continue;
                }

                chip.Tapped -= this.ChipTappedHandler;
                chip.Removed -= this.ChipRemoved;
                chip.PropertyChanged -= this.ChipPropertyChanged;
            }
        }

        if (e.NewItems is not null)
        {
            foreach (var t in e.NewItems)
            {
                if (t is not Chip chip)
                {
                    continue;
                }

                chip.PropertyChanged -= this.ChipPropertyChanged;
                chip.PropertyChanged += this.ChipPropertyChanged;

                chip.Tapped -= this.ChipTappedHandler;
                chip.Tapped += this.ChipTappedHandler;

                chip.Removed -= this.ChipRemoved;
                chip.Removed += this.ChipRemoved;
            }

            this.OnPropertyChanged(SelectedChipsProperty.PropertyName);
            this.OnPropertyChanged(nameof(this.SelectedValues));
            this.OnPropertyChanged(nameof(this.SelectedValue));
        }

        this.InvalidateLayout();
    }

    private void ChipTappedHandler(object? sender, EventArgs e)
    {
        if (sender is Chip chip)
        {
            this.ChipTapped?.Invoke(this, new ChipTappedEventArgs(chip));
        }
    }

    private void ChipRemoved(object? sender, EventArgs e)
    {
        if (sender is not Chip chip)
        {
            return;
        }

        this.Children.Remove(chip);
    }

    private void ChipPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (sender is not Chip chip || e.PropertyName is null)
        {
            return;
        }

        if (e.PropertyName.Equals(Chip.IsToggledProperty.PropertyName))
        {
            if ((this.IsSingleSelection || chip.IsSingleSelection) && chip.IsToggled)
            {
                foreach (var child in this.Children)
                {
                    if (child != chip)
                    {
                        child.IsToggled = false;
                    }
                }

                if (this.IsSingleSelection && chip.IsToggled)
                {
                    this._previouslySelectedChip = chip;
                }
            }
            else if (chip.IsToggled)
            {
                foreach (var child in this.Children)
                {
                    if (child != chip && child.IsSingleSelection)
                    {
                        child.IsToggled = false;
                    }
                }
            }

            if (!this.Children.Any(x => x.IsToggled) &&
                this._previouslySelectedChip is not null &&
                this.Children.Contains(this._previouslySelectedChip))
            {
                this._previouslySelectedChip.IsToggled = true;
            }

            if (this.ChipSelectionDetection)
            {
                this.OnPropertyChanged(SelectedChipsProperty.PropertyName);
                this.OnPropertyChanged(nameof(this.SelectedValues));
                this.OnPropertyChanged(nameof(this.SelectedValue));
            }
        }
        else if (e.PropertyName.Equals(Chip.HeightProperty.PropertyName) || e.PropertyName.Equals(Chip.HeightProperty.PropertyName))
        {
            this.ForceLayout();
        }
    }

    private void PanUpdated(object? sender, PanUpdatedEventArgs e)
    {
        if (!this.Scrollable)
        {
            return;
        }

        switch (e.StatusType)
        {
            case GestureStatus.Started:
            case GestureStatus.Running:
                this.SetXOffset(_finalXOffset + e.TotalX, false);
                break;
            default:
                this.SetXOffset(_xOffset, true);
                break;
        }
    }

    private void SetXOffset(double xOffset, bool isFinal)
    {
        var width = this.Width;

        var potentialMax = _maxOffset - width;

        if (xOffset > 0.0d || _maxOffset < width)
        {
            _xOffset = 0;
        }
        else if (Math.Abs(xOffset) > potentialMax)
        {
            _xOffset = -potentialMax;
        }
        else
        {
            _xOffset = xOffset;
        }

        if (isFinal)
        {
            _finalXOffset = _xOffset;
        }

        this.InvalidateLayout();

        System.Diagnostics.Debug.WriteLine($"{_xOffset}");
    }

    internal class LayoutInfo
    {
        private readonly List<Rect> _bounds = new();
        private readonly List<Rect> _sizes = new();

        private double _x = 0;
        private double _y = 0;
        private double _rowHeight = 0;
        private double _spacing;
        private int _maxRowsBeforeOverflow;
        private bool _scrollable;

        public List<Rect> Bounds => _bounds;

        public double HeightRequest { get; private set; }

        public double WidthRequest { get; private set; }

        public int Rows { get; private set; }

        public bool IsOverflow => _maxRowsBeforeOverflow > 0 && this.Rows > _maxRowsBeforeOverflow;

        public void ProcessLayout(IList<Chip> views, double spacing, int maxRowsBeforeExpansion, bool scrollable, double widthConstraint)
        {
            _spacing = spacing;
            _maxRowsBeforeOverflow = maxRowsBeforeExpansion;
            _scrollable = scrollable;

            var viewsArray = views.ToArray();

            var sizes = this.SizeViews(viewsArray, widthConstraint);
            this.LayoutViews(viewsArray, sizes, widthConstraint);
        }

        private List<Rect> SizeViews(IList<Chip> views, double widthConstraint)
        {
            _sizes.Clear();

            foreach (var view in views)
            {
                var sizeRequest = view.Measure(widthConstraint, double.PositiveInfinity).Request;
                var viewWidth = sizeRequest.Width;
                var viewHeight = sizeRequest.Height;

                if (viewWidth > widthConstraint)
                {
                    viewWidth = widthConstraint;
                }

                _sizes.Add(new Rect(0, 0, viewWidth, viewHeight));
            }

            return _sizes;
        }

        private void LayoutViews(IList<Chip> views, List<Rect> sizes, double widthConstraint)
        {
            _bounds.Clear();
            _x = 0d;
            _y = 0d;
            this.HeightRequest = 0;

            this.Rows = 1;

            for (int i = 0; i < views.Count(); i++)
            {
                if (!views[i].IsVisible)
                {
                    this.Bounds.Add(Rect.Zero);
                    continue;
                }

                var sizeRect = sizes[i];

                if (!_scrollable)
                {
                    var isNewLine = this.CheckNewLine(sizeRect.Width, widthConstraint);

                    if (isNewLine)
                    {
                        this.Rows++;
                    }

                    if (_maxRowsBeforeOverflow > 0 && this.Rows > _maxRowsBeforeOverflow)
                    {
                        this.Bounds.Add(Rect.Zero);
                        continue;
                    }
                }

                this.UpdateRowHeight(sizeRect.Height);

                var bound = new Rect(_x, _y, sizeRect.Width, sizeRect.Height);
                this.Bounds.Add(bound);

                _x += sizeRect.Width;
                _x += _spacing;
            }

            _x -= _spacing;

            this.HeightRequest = _rowHeight * this.Rows;
            this.WidthRequest = _x;
        }

        private bool CheckNewLine(double viewWidth, double widthConstraint)
        {
            if (!(this._x + viewWidth > widthConstraint))
            {
                return false;
            }

            this._y += this._rowHeight + this._spacing;
            this._x = 0;
            this._rowHeight = 0;

            return true;
        }

        private void UpdateRowHeight(double viewHeight)
        {
            if (viewHeight > _rowHeight)
            {
                _rowHeight = viewHeight;
            }
        }
    }
}

public class ChipTappedEventArgs : EventArgs
{
    public Chip Chip { get; private set; }

    public Chip.ChipState ChipState { get; private set; }

    public ChipTappedEventArgs(Chip chip)
    {
        this.Chip = chip;
        this.ChipState = chip.State;
    }
}
