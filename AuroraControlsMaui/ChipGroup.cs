using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Layouts;

namespace AuroraControls;

public class ChipGroup : Layout, ILayoutManager
{
    private readonly PanGestureRecognizer _panGesture;
    private readonly ChipGroupLayoutManager _layoutManager;

    private double _xOffset;
    private double _maxOffset;
    private double _finalXOffset;

    private Chip? _previouslySelectedChip;

    private static readonly BindablePropertyKey SelectedChipsPropertyKey =
        BindableProperty.CreateReadOnly(nameof(SelectedChips), typeof(IEnumerable<Chip>), typeof(ChipGroup), Enumerable.Empty<Chip>());

    public static readonly BindableProperty SelectedChipsProperty = SelectedChipsPropertyKey.BindableProperty;

    public event EventHandler<ChipTappedEventArgs>? ChipTapped;

    public IEnumerable<Chip> SelectedChips
    {
        get => this.Where(x => x is Chip chip && chip.IsToggled).Cast<Chip>().ToArray();
    }

    public IEnumerable<object> SelectedValues
    {
        get => SelectedChips.Select(x => x.Value).ToArray() ?? Enumerable.Empty<object>();
        set
        {
            var children = this.OfType<Chip>().ToArray();

            foreach (var child in children)
            {
                child.IsToggled = value.Contains(child.Value);
            }

            OnPropertyChanged();
        }
    }

    public object? SelectedValue
    {
        get => SelectedChips.Select(x => x.Value).FirstOrDefault();
        set
        {
            var children = this.OfType<Chip>().ToArray();

            var hasToggled = false;
            foreach (var child in children)
            {
                if (!hasToggled)
                {
                    var isMatch = child.Value?.Equals(value) ?? false;
                    child.IsToggled = isMatch;

                    if (isMatch)
                    {
                        hasToggled = true;
                    }

                    continue;
                }

                child.IsToggled = false;
            }

            OnPropertyChanged();
        }
    }

    public static BindableProperty ScrollableProperty =
        BindableProperty.Create(nameof(Scrollable), typeof(bool), typeof(ChipGroup), default(bool), propertyChanged: OnLayoutPropertyChanged);

    public bool Scrollable
    {
        get => (bool)GetValue(ScrollableProperty);
        set => SetValue(ScrollableProperty, value);
    }

    public static BindableProperty MaxRowsBeforeOverflowProperty =
        BindableProperty.Create(nameof(MaxRowsBeforeOverflow), typeof(int), typeof(ChipGroup), -1, propertyChanged: OnLayoutPropertyChanged);

    public int MaxRowsBeforeOverflow
    {
        get => (int)GetValue(MaxRowsBeforeOverflowProperty);
        set => SetValue(MaxRowsBeforeOverflowProperty, value);
    }

    public static BindableProperty IsOverflowProperty =
        BindableProperty.Create(nameof(IsOverflow), typeof(bool), typeof(ChipGroup), default(bool));

    public bool IsOverflow
    {
        get => (bool)GetValue(IsOverflowProperty);
        private set => SetValue(IsOverflowProperty, value);
    }

    public static BindableProperty IsSingleSelectionProperty =
        BindableProperty.Create(nameof(IsSingleSelection), typeof(bool), typeof(ChipGroup), default(bool));

    public bool IsSingleSelection
    {
        get => (bool)GetValue(IsSingleSelectionProperty);
        set => SetValue(IsSingleSelectionProperty, value);
    }

    public static BindableProperty ChipSelectionDetectionProperty =
        BindableProperty.Create(nameof(ChipSelectionDetection), typeof(bool), typeof(ChipGroup), true);

    public bool ChipSelectionDetection
    {
        get => (bool)GetValue(ChipSelectionDetectionProperty);
        set => SetValue(ChipSelectionDetectionProperty, value);
    }

    public static BindableProperty SpacingProperty = BindableProperty.Create(nameof(Spacing), typeof(double), typeof(ChipGroup), 8d, propertyChanged: OnLayoutPropertyChanged);

    public double Spacing
    {
        get => (double)GetValue(SpacingProperty);
        set => SetValue(SpacingProperty, value);
    }

    public ChipGroup()
    {
        _layoutManager = new ChipGroupLayoutManager();
        _panGesture = new PanGestureRecognizer();
        GestureRecognizers.Add(_panGesture);

        // Wire up child collection changes
        ChildAdded += OnChildAdded;
        ChildRemoved += OnChildRemoved;
    }

    private static void OnLayoutPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is ChipGroup chipGroup)
        {
            chipGroup._layoutManager.Spacing = chipGroup.Spacing;
            chipGroup._layoutManager.MaxRowsBeforeOverflow = chipGroup.MaxRowsBeforeOverflow;
            chipGroup._layoutManager.Scrollable = chipGroup.Scrollable;
            chipGroup.InvalidateMeasure();
        }
    }

    protected override ILayoutManager CreateLayoutManager() => this;

    public Size ArrangeChildren(Rect bounds, ILayoutManager layoutManager, IEnumerable<IView> children)
    {
        _layoutManager.XOffset = _xOffset;
        var result = _layoutManager.ArrangeChildren(bounds, layoutManager, children);

        _maxOffset = _layoutManager.WidthRequest;
        IsOverflow = _layoutManager.IsOverflow;

        return result;
    }

    public Size Measure(double widthConstraint, double heightConstraint, ILayoutManager layoutManager, IEnumerable<IView> children)
    {
        _layoutManager.Spacing = Spacing;
        _layoutManager.MaxRowsBeforeOverflow = MaxRowsBeforeOverflow;
        _layoutManager.Scrollable = Scrollable;

        return _layoutManager.Measure(widthConstraint, heightConstraint, layoutManager, children);
    }

    // Required interface methods for newer MAUI versions
    public Size ArrangeChildren(Rect bounds)
    {
        return ArrangeChildren(bounds, this, this);
    }

    public Size Measure(double widthConstraint, double heightConstraint)
    {
        return Measure(widthConstraint, heightConstraint, this, this);
    }

    public async Task ScrollToContextAsync(object item, ScrollToPosition scrollToPosition = ScrollToPosition.Start, bool animated = true, uint rate = 16,
        uint length = 250, Easing? easing = null)
    {
        var matchingChip = this.OfType<Chip>().FirstOrDefault(x => x.BindingContext == item);

        if (matchingChip == null)
        {
            return;
        }

        await ScrollToAsync(matchingChip, scrollToPosition, animated, rate, length, easing);
    }

    public async Task ScrollToValueAsync(object item, ScrollToPosition scrollToPosition = ScrollToPosition.Start, bool animated = true, uint rate = 16,
        uint length = 250, Easing? easing = null)
    {
        var matchingChip = this.OfType<Chip>().FirstOrDefault(x => x.Value?.Equals(item) ?? false);

        if (matchingChip == null)
        {
            return;
        }

        await ScrollToAsync(matchingChip, scrollToPosition, animated, rate, length, easing);
    }

    public async Task ScrollToAsync(Chip chip, ScrollToPosition scrollToPosition = ScrollToPosition.Start, bool animated = true, uint rate = 16,
        uint length = 250, Easing? easing = null)
    {
        if (Contains(chip))
        {
            var containerXStart = chip.Frame.Left + Math.Abs(_xOffset);

            var offsetAmount = 0d;

            switch (scrollToPosition)
            {
                case ScrollToPosition.Start:
                    offsetAmount = -containerXStart;
                    break;
                case ScrollToPosition.Center:
                    offsetAmount = -containerXStart + (Width * .5d) - (chip.Width * .5d);
                    break;
                default:
                    offsetAmount = -containerXStart + Width - chip.Width;
                    break;
            }

            if (animated)
            {
                await this.TransitionTo(
                    nameof(ScrollToAsync),
                    x => SetXOffset(x, false),
                    _xOffset,
                    offsetAmount,
                    rate,
                    length,
                    easing);

                SetXOffset(_xOffset, true);
                return;
            }

            SetXOffset(offsetAmount, true);
        }
    }

    protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName?.Equals(WindowProperty.PropertyName) == true)
        {
            if (Window is null)
            {
                UnsubscribeFromChipEvents();
                _panGesture.PanUpdated -= PanUpdated;
                return;
            }

            SubscribeToChipEvents();
            _panGesture.PanUpdated -= PanUpdated;
            _panGesture.PanUpdated += PanUpdated;
        }
    }

    private void OnChildAdded(object? sender, ElementEventArgs e)
    {
        if (e.Element is Chip chip)
        {
            SubscribeToChipEvents(chip);

            OnPropertyChanged(SelectedChipsProperty.PropertyName);
            OnPropertyChanged(nameof(SelectedValues));
            OnPropertyChanged(nameof(SelectedValue));
        }
    }

    private void OnChildRemoved(object? sender, ElementEventArgs e)
    {
        if (e.Element is Chip chip)
        {
            UnsubscribeFromChipEvents(chip);

            OnPropertyChanged(SelectedChipsProperty.PropertyName);
            OnPropertyChanged(nameof(SelectedValues));
            OnPropertyChanged(nameof(SelectedValue));
        }
    }

    private void SubscribeToChipEvents()
    {
        foreach (var chip in this.OfType<Chip>())
        {
            SubscribeToChipEvents(chip);
        }
    }

    private void SubscribeToChipEvents(Chip chip)
    {
        chip.PropertyChanged -= ChipPropertyChanged;
        chip.PropertyChanged += ChipPropertyChanged;

        chip.Tapped -= ChipTappedHandler;
        chip.Tapped += ChipTappedHandler;

        chip.Removed -= ChipRemoved;
        chip.Removed += ChipRemoved;
    }

    private void UnsubscribeFromChipEvents()
    {
        foreach (var chip in this.OfType<Chip>())
        {
            UnsubscribeFromChipEvents(chip);
        }
    }

    private void UnsubscribeFromChipEvents(Chip chip)
    {
        chip.PropertyChanged -= ChipPropertyChanged;
        chip.Tapped -= ChipTappedHandler;
        chip.Removed -= ChipRemoved;
    }

    private void ChipTappedHandler(object? sender, EventArgs e)
    {
        if (sender is Chip chip)
        {
            ChipTapped?.Invoke(this, new ChipTappedEventArgs(chip));
        }
    }

    private void ChipRemoved(object? sender, EventArgs e)
    {
        if (sender is Chip chip)
        {
            Remove(chip);
        }
    }

    private void ChipPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (sender is not Chip chip)
        {
            return;
        }

        if (e.PropertyName?.Equals(Chip.IsToggledProperty.PropertyName) == true)
        {
            if ((IsSingleSelection || chip.IsSingleSelection) && chip.IsToggled)
            {
                foreach (var child in this.OfType<Chip>())
                {
                    if (child != chip)
                    {
                        child.IsToggled = false;
                    }
                }

                if (IsSingleSelection && chip.IsToggled)
                {
                    _previouslySelectedChip = chip;
                }
            }
            else if (chip.IsToggled)
            {
                foreach (var child in this.OfType<Chip>())
                {
                    if (child != chip && child.IsSingleSelection)
                    {
                        child.IsToggled = false;
                    }
                }
            }

            if (!this.OfType<Chip>().Any(x => x.IsToggled) &&
                _previouslySelectedChip is not null &&
                Contains(_previouslySelectedChip))
            {
                _previouslySelectedChip.IsToggled = true;
            }

            if (ChipSelectionDetection)
            {
                OnPropertyChanged(SelectedChipsProperty.PropertyName);
                OnPropertyChanged(nameof(SelectedValues));
                OnPropertyChanged(nameof(SelectedValue));
            }
        }
        else if (e.PropertyName?.Equals(Chip.HeightProperty.PropertyName) == true ||
                 e.PropertyName?.Equals(Chip.WidthProperty.PropertyName) == true)
        {
            InvalidateMeasure();
        }
    }

    private void PanUpdated(object? sender, PanUpdatedEventArgs e)
    {
        if (!Scrollable)
        {
            return;
        }

        switch (e.StatusType)
        {
            case GestureStatus.Started:
            case GestureStatus.Running:
                SetXOffset(_finalXOffset + e.TotalX, false);
                break;
            default:
                SetXOffset(_xOffset, true);
                break;
        }
    }

    private void SetXOffset(double xOffset, bool isFinal)
    {
        var width = Width;

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

        InvalidateMeasure();

        System.Diagnostics.Debug.WriteLine($"{_xOffset}");
    }
}
