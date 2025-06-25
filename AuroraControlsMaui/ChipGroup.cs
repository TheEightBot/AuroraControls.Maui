using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.Maui.Layouts;

namespace AuroraControls;

public class ChipGroup : ContentView, IDisposable
{
    private readonly ObservableCollection<Chip> _chips = new();
    private readonly List<Chip> _selectedChips = new();
    private bool _disposed;
    private bool _isUpdating;
    private Chip? _selectedChip;
    private ScrollView? _scrollView;
    private FlexLayout? _chipContainer;

    /// <summary>
    /// Event that fires when chip selection changes
    /// </summary>
    public event EventHandler<ChipSelectionChangedEventArgs>? SelectionChanged;

    /// <summary>
    /// Event that fires when a chip in the group is tapped
    /// </summary>
    public event EventHandler<ChipTappedEventArgs>? ChipTapped;

    /// <summary>
    /// Gets the collection of chips in this group.
    /// </summary>
    public IList<Chip> Chips => _chips;

    /// <summary>
    /// Controls whether the ChipGroup allows horizontal scrolling (single line) or wraps content (multi-line).
    /// </summary>
    public static readonly BindableProperty IsScrollableProperty =
        BindableProperty.Create(nameof(IsScrollable), typeof(bool), typeof(ChipGroup), false,
            propertyChanged: OnLayoutPropertyChanged);

    /// <summary>
    /// Gets or sets a value indicating whether the ChipGroup allows horizontal scrolling (true) or wraps chips (false).
    /// </summary>
    public bool IsScrollable
    {
        get => (bool)GetValue(IsScrollableProperty);
        set => SetValue(IsScrollableProperty, value);
    }

    /// <summary>
    /// Controls whether multiple chips can be selected simultaneously.
    /// </summary>
    public static readonly BindableProperty AllowMultipleSelectionProperty =
        BindableProperty.Create(nameof(AllowMultipleSelection), typeof(bool), typeof(ChipGroup), false,
            propertyChanged: OnAllowMultipleSelectionChanged);

    /// <summary>
    /// Gets or sets a value indicating whether multiple chips can be selected simultaneously.
    /// </summary>
    public bool AllowMultipleSelection
    {
        get => (bool)GetValue(AllowMultipleSelectionProperty);
        set => SetValue(AllowMultipleSelectionProperty, value);
    }

    /// <summary>
    /// The currently selected chip in single-selection mode.
    /// </summary>
    public static readonly BindableProperty SelectedChipProperty =
        BindableProperty.Create(nameof(SelectedChip), typeof(Chip), typeof(ChipGroup), null,
            BindingMode.TwoWay, propertyChanged: OnSelectedChipChanged);

    /// <summary>
    /// Gets or sets the currently selected chip in single-selection mode.
    /// </summary>
    public Chip? SelectedChip
    {
        get => (Chip?)GetValue(SelectedChipProperty);
        set => SetValue(SelectedChipProperty, value);
    }

    /// <summary>
    /// The currently selected chips in multi-selection mode.
    /// </summary>
    public static readonly BindableProperty SelectedChipsProperty =
        BindableProperty.Create(nameof(SelectedChips), typeof(IList<Chip>), typeof(ChipGroup), null,
            BindingMode.OneWay);

    /// <summary>
    /// Gets the currently selected chips in multi-selection mode.
    /// </summary>
    public IList<Chip> SelectedChips => _selectedChips;

    /// <summary>
    /// The horizontal spacing between chips.
    /// </summary>
    public static readonly BindableProperty HorizontalSpacingProperty =
        BindableProperty.Create(nameof(HorizontalSpacing), typeof(double), typeof(ChipGroup), 8.0,
            propertyChanged: OnLayoutPropertyChanged);

    /// <summary>
    /// Gets or sets the horizontal spacing between chips.
    /// </summary>
    public double HorizontalSpacing
    {
        get => (double)GetValue(HorizontalSpacingProperty);
        set => SetValue(HorizontalSpacingProperty, value);
    }

    /// <summary>
    /// The vertical spacing between chips when in multi-line mode.
    /// </summary>
    public static readonly BindableProperty VerticalSpacingProperty =
        BindableProperty.Create(nameof(VerticalSpacing), typeof(double), typeof(ChipGroup), 8.0,
            propertyChanged: OnLayoutPropertyChanged);

    /// <summary>
    /// Gets or sets the vertical spacing between chips when in multi-line mode.
    /// </summary>
    public double VerticalSpacing
    {
        get => (double)GetValue(VerticalSpacingProperty);
        set => SetValue(VerticalSpacingProperty, value);
    }

    /// <summary>
    /// The start offset to insert empty space before the first chip.
    /// </summary>
    public static readonly BindableProperty ChipInsetProperty =
        BindableProperty.Create(nameof(ChipInset), typeof(double), typeof(ChipGroup), 0.0,
            propertyChanged: OnLayoutPropertyChanged);

    /// <summary>
    /// Gets or sets the start offset to insert empty space before the first chip.
    /// </summary>
    public double ChipInset
    {
        get => (double)GetValue(ChipInsetProperty);
        set => SetValue(ChipInsetProperty, value);
    }

    /// <summary>
    /// The source collection of items to create chips from.
    /// </summary>
    public static readonly BindableProperty ItemsSourceProperty =
        BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(ChipGroup), null,
            propertyChanged: OnItemsSourceChanged);

    /// <summary>
    /// Gets or sets the source collection of items to create chips from.
    /// </summary>
    public IEnumerable? ItemsSource
    {
        get => (IEnumerable?)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    /// <summary>
    /// The template to use for creating chips from the ItemsSource.
    /// </summary>
    public static readonly BindableProperty ItemTemplateProperty =
        BindableProperty.Create(nameof(ItemTemplate), typeof(DataTemplate), typeof(ChipGroup), null,
            propertyChanged: OnItemTemplateChanged);

    /// <summary>
    /// Gets or sets the template to use for creating chips from the ItemsSource.
    /// </summary>
    public DataTemplate? ItemTemplate
    {
        get => (DataTemplate?)GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    /// <summary>
    /// The currently selected chip value in single-selection mode.
    /// </summary>
    public static readonly BindableProperty SelectedValueProperty =
        BindableProperty.Create(nameof(SelectedValue), typeof(object), typeof(ChipGroup), null,
            BindingMode.TwoWay, propertyChanged: OnSelectedValueChanged);

    /// <summary>
    /// Gets or sets the value of the currently selected chip in single-selection mode.
    /// </summary>
    public object? SelectedValue
    {
        get => GetValue(SelectedValueProperty);
        set => SetValue(SelectedValueProperty, value);
    }

    /// <summary>
    /// The currently selected chip values in multi-selection mode.
    /// </summary>
    public static readonly BindableProperty SelectedValuesProperty =
        BindableProperty.Create(nameof(SelectedValues), typeof(IList<object>), typeof(ChipGroup), null,
            BindingMode.OneWay);

    /// <summary>
    /// Gets the values of currently selected chips in multi-selection mode.
    /// </summary>
    public IList<object> SelectedValues => _selectedChips.Select(chip => chip.Value).ToList();

    public ChipGroup()
    {
        _chips.CollectionChanged += OnChipsCollectionChanged;
        InitializeLayout();
    }

    /// <summary>
    /// Scrolls to make the specified chip visible in the viewport.
    /// </summary>
    /// <param name="chip">The chip to scroll to.</param>
    /// <returns>True if the chip was found and scrolled to, false otherwise.</returns>
    public bool ScrollToChip(Chip chip, ScrollToPosition position, bool animated = true)
    {
        // Only proceed if we're in scrollable mode and have a valid ScrollView
        if (!IsScrollable || _scrollView == null || !_chips.Contains(chip))
        {
            return false;
        }

        // Calculate scroll position - in horizontal mode, we want to scroll to the chip's X position
        var chipBounds = chip.Bounds;
        var scrollPosition = new Point(chipBounds.X, 0);

        // Scroll to the chip - animated
        _scrollView.ScrollToAsync(chip, position, animated);
        return true;
    }

    /// <summary>
    /// Finds a chip with a matching value and scrolls to make it visible.
    /// </summary>
    /// <param name="value">The value to match against chip's Value.</param>
    /// <param name="comparer">Optional custom comparer for value matching. If null, the default equality comparison is used.</param>
    /// <returns>True if a matching chip was found and scrolled to, false otherwise.</returns>
    public bool ScrollToChipWithValue<T>(T value, ScrollToPosition position, bool animated = true, IEqualityComparer<T>? comparer = null)
    {
        if (!IsScrollable || _scrollView == null || _chips.Count == 0)
        {
            return false;
        }

        // Find chip with matching value
        Chip? matchingChip = null;
        comparer ??= EqualityComparer<T>.Default;

        matchingChip = _chips.FirstOrDefault(c =>
        {
            if (c.Value == null)
            {
                return false;
            }

            // Check if the chip's value is of type T or can be converted to T
            if (c.Value is T chipValue)
            {
                return comparer.Equals(chipValue, value);
            }

            // If types don't match exactly, try to compare as objects
            return value?.Equals(c.Value) == true || c.Value.Equals(value);
        });

        // If found, scroll to the chip
        if (matchingChip != null)
        {
            return ScrollToChip(matchingChip, position, animated);
        }

        return false;
    }

    /// <summary>
    /// Scrolls to the currently selected chip, if any.
    /// </summary>
    /// <returns>True if a selected chip was found and scrolled to, false otherwise.</returns>
    public bool ScrollToSelectedChip(ScrollToPosition position, bool animated = true)
    {
        if (SelectedChip != null)
        {
            return ScrollToChip(SelectedChip, position, animated);
        }

        return false;
    }

    private void InitializeLayout()
    {
        // Create layout containers
        _chipContainer = new FlexLayout
        {
            Direction = FlexDirection.Row,
            Wrap = IsScrollable ? FlexWrap.NoWrap : FlexWrap.Wrap,
            JustifyContent = FlexJustify.Start,
            AlignItems = FlexAlignItems.Center,
        };

        ApplyChipInset();

        if (IsScrollable)
        {
            _scrollView = new ScrollView
            {
                Orientation = ScrollOrientation.Horizontal,
                Content = _chipContainer,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Never,
            };
            Content = _scrollView;
        }
        else
        {
            Content = _chipContainer;
        }

        UpdateSpacing();
    }

    private void UpdateSpacing()
    {
        if (_chipContainer == null)
        {
            return;
        }

        // Set spacing
        foreach (var child in _chipContainer.Children)
        {
            if (child is not Chip chip)
            {
                continue;
            }

            FlexLayout.SetBasis(chip, FlexBasis.Auto);
            FlexLayout.SetGrow(chip, 0);
            FlexLayout.SetShrink(chip, 0);

            chip.Margin = new Thickness(0, 0, this.HorizontalSpacing, this.IsScrollable ? 0 : this.VerticalSpacing);
        }
    }

    private void ApplyChipInset()
    {
        if (_chipContainer == null)
        {
            return;
        }

        // Apply inner padding if specified
        if (this.ChipInset > 0)
        {
            _chipContainer.Padding = new Thickness(this.ChipInset, 0);
        }
    }

    /// <summary>
    /// Handles changes to the AllowMultipleSelection property.
    /// Updates chip configurations and selection state.
    /// </summary>
    private static void OnAllowMultipleSelectionChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not ChipGroup chipGroup)
        {
            return;
        }

        bool allowMultipleSelection = (bool)newValue;

        // Update the single selection flag on all chips
        chipGroup.UpdateChipSelectionMode();

        // Handle the selection state transition
        if (allowMultipleSelection)
        {
            // Switching from single to multiple selection:
            // If there's a selected chip, keep it as the only selection
            if (chipGroup.SelectedChip != null)
            {
                if (!chipGroup._selectedChips.Contains(chipGroup.SelectedChip))
                {
                    chipGroup._selectedChips.Clear();
                    chipGroup._selectedChips.Add(chipGroup.SelectedChip);
                }
            }
        }
        else
        {
            // Switching from multiple to single selection
            Chip? chipToKeepSelected = null;

            // Determine which chip should remain selected
            if (chipGroup._selectedChips.Count > 0)
            {
                // Keep the first selected chip
                chipToKeepSelected = chipGroup._selectedChips[0];
            }

            // First, untoggle all chips that shouldn't be selected
            foreach (var chip in chipGroup._chips)
            {
                if (chip != chipToKeepSelected && chip.IsToggled)
                {
                    chip.IsToggled = false;
                }
            }

            // Update the selection collections and properties
            chipGroup._selectedChips.Clear();

            if (chipToKeepSelected != null)
            {
                chipGroup._selectedChips.Add(chipToKeepSelected);
                chipGroup._selectedChip = chipToKeepSelected;
                chipGroup.SetValue(SelectedChipProperty, chipToKeepSelected);
                chipGroup.SetValue(SelectedValueProperty, chipToKeepSelected.Value);
            }
            else
            {
                chipGroup._selectedChip = null;
                chipGroup.SetValue(SelectedChipProperty, null);
                chipGroup.SetValue(SelectedValueProperty, null);
            }
        }

        // Make sure SelectedValues is notified of changes
        chipGroup.UpdateSelectedValues();

        // Notify about the selection change
        chipGroup.NotifySelectionChanged(null, chipGroup._selectedChip);
    }

    private static void OnLayoutPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is ChipGroup chipGroup)
        {
            chipGroup.UpdateLayout();
        }
    }

    private static void OnSelectedChipChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not ChipGroup chipGroup)
        {
            return;
        }

        if (chipGroup._isUpdating)
        {
            return;
        }

        var newChip = newValue as Chip;
        var oldChip = oldValue as Chip;

        try
        {
            chipGroup._isUpdating = true;

            // Update old chip
            if (oldChip != null && chipGroup._chips.Contains(oldChip))
            {
                oldChip.IsToggled = false;
            }

            // Update new chip
            if (newChip != null && chipGroup._chips.Contains(newChip))
            {
                newChip.IsToggled = true;
                chipGroup._selectedChip = newChip;

                // Update SelectedValue to match the new chip's Value
                chipGroup.SetValue(SelectedValueProperty, newChip.Value);

                // In single selection mode, clear other selections
                if (!chipGroup.AllowMultipleSelection)
                {
                    chipGroup._selectedChips.Clear();
                    chipGroup._selectedChips.Add(newChip);
                }
                else
                {
                    chipGroup.UpdateSelectedValues();
                }
            }
            else
            {
                chipGroup._selectedChip = null;

                // Clear SelectedValue when no chip is selected
                chipGroup.SetValue(SelectedValueProperty, null);

                if (!chipGroup.AllowMultipleSelection)
                {
                    chipGroup._selectedChips.Clear();
                }
                else
                {
                    chipGroup.UpdateSelectedValues();
                }
            }
        }
        finally
        {
            chipGroup._isUpdating = false;

            // Notify selection changes
            chipGroup.NotifySelectionChanged(oldChip, newChip);
        }
    }

    private static void OnSelectedValueChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not ChipGroup chipGroup)
        {
            return;
        }

        if (chipGroup._isUpdating)
        {
            return;
        }

        // Find chip with the specified value
        Chip? matchingChip = null;

        try
        {
            chipGroup._isUpdating = true;

            if (newValue != null)
            {
                matchingChip = chipGroup._chips.FirstOrDefault(c =>
                {
                    if (c.Value == null)
                    {
                        return newValue == null;
                    }

                    return c.Value.Equals(newValue) || newValue.Equals(c.Value);
                });
            }
        }
        finally
        {
            chipGroup._isUpdating = false;
        }

        // Update selection
        chipGroup.SelectedChip = matchingChip;
    }

    /// <summary>
    /// Updates the SelectedValues collection and raises property changed notification.
    /// </summary>
    private void UpdateSelectedValues()
    {
        // For the OneWay binding of SelectedValues
        OnPropertyChanged(nameof(SelectedValues));
    }

    /// <summary>
    /// Selects a chip by its value.
    /// </summary>
    /// <param name="value">The value to search for.</param>
    /// <returns>True if a chip with the matching value was found and selected.</returns>
    public bool SelectChipByValue(object value)
    {
        if (value == null)
        {
            return false;
        }

        var chip = _chips.FirstOrDefault(c =>
        {
            if (c.Value == null)
            {
                return false;
            }

            return c.Value.Equals(value) || value.Equals(c.Value);
        });

        if (chip != null)
        {
            SelectedChip = chip;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Finds and returns a chip by its value.
    /// </summary>
    /// <param name="value">The value to search for.</param>
    /// <returns>The chip with the matching value or null if not found.</returns>
    public Chip? GetChipByValue(object value)
    {
        if (value == null)
        {
            return null;
        }

        return _chips.FirstOrDefault(c =>
        {
            if (c.Value == null)
            {
                return false;
            }

            return c.Value.Equals(value) || value.Equals(c.Value);
        });
    }

    private void NotifySelectionChanged(Chip? oldSelection, Chip? newSelection)
    {
        var args = new ChipSelectionChangedEventArgs
        {
            OldSelection = oldSelection,
            NewSelection = newSelection,
            SelectedItems = new List<Chip>(_selectedChips),
        };

        SelectionChanged?.Invoke(this, args);
    }

    private void OnChipsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (_chipContainer == null)
        {
            return;
        }

        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                if (e.NewItems != null)
                {
                    foreach (Chip newChip in e.NewItems)
                    {
                        AddChip(newChip);
                    }
                }

                break;

            case NotifyCollectionChangedAction.Remove:
                if (e.OldItems != null)
                {
                    foreach (Chip oldChip in e.OldItems)
                    {
                        RemoveChip(oldChip);
                    }
                }

                // If all chips are removed, ensure selected values are cleared
                if (_chips.Count == 0)
                {
                    ClearSelection();
                }

                break;

            case NotifyCollectionChangedAction.Replace:
                if (e.OldItems != null)
                {
                    foreach (Chip oldChip in e.OldItems)
                    {
                        RemoveChip(oldChip);
                    }
                }

                if (e.NewItems != null)
                {
                    foreach (Chip newChip in e.NewItems)
                    {
                        AddChip(newChip);
                    }
                }

                break;

            case NotifyCollectionChangedAction.Reset:
                _chipContainer.Clear();

                // When the collection is reset/cleared, clear all selections
                if (_chips.Count == 0)
                {
                    ClearSelection();
                }
                else
                {
                    foreach (var chip in _chips)
                    {
                        AddChip(chip);
                    }
                }

                break;
        }

        UpdateLayout();
    }

    private void AddChip(Chip chip)
    {
        // Configure the chip
        chip.IsSingleSelection = !AllowMultipleSelection;

        // Attach event handlers
        chip.Toggled += OnChipToggled;
        chip.Removed += OnChipRemoved;
        chip.SizeChanged += OnChipSizeChanged;
        chip.Tapped += OnChipTapped;

        // Set initial spacing
        if (_chipContainer != null)
        {
            FlexLayout.SetBasis(chip, FlexBasis.Auto);
            FlexLayout.SetGrow(chip, 0);
            FlexLayout.SetShrink(chip, 0);

            chip.Margin = new Thickness(0, 0, HorizontalSpacing, IsScrollable ? 0 : VerticalSpacing);
        }

        // Add to the container
        _chipContainer?.Add(chip);
    }

    private void RemoveChip(Chip chip)
    {
        // Detach event handlers
        chip.Toggled -= OnChipToggled;
        chip.Removed -= OnChipRemoved;
        chip.SizeChanged -= OnChipSizeChanged;
        chip.Tapped -= OnChipTapped;

        // Remove from container
        _chipContainer?.Remove(chip);

        // Update selections if needed
        if (chip.IsToggled)
        {
            if (SelectedChip == chip)
            {
                SelectedChip = null;
            }

            _selectedChips.Remove(chip);
        }
    }

    private void OnChipToggled(object? sender, bool isToggled)
    {
        if (sender is not Chip chip || _isUpdating)
        {
            return;
        }

        _isUpdating = true;

        if (isToggled)
        {
            // Handle single selection mode
            if (!AllowMultipleSelection)
            {
                foreach (var otherChip in _chips.Where(c => c != chip && c.IsToggled))
                {
                    otherChip.IsToggled = false;
                }

                _selectedChips.Clear();
                _selectedChip = chip;
            }

            // Add to selected chips
            if (!_selectedChips.Contains(chip))
            {
                _selectedChips.Add(chip);
            }

            // Update SelectedChip and SelectedValue for single selection mode
            if (!AllowMultipleSelection)
            {
                SetValue(SelectedChipProperty, chip);
                SetValue(SelectedValueProperty, chip.Value);
            }
            else
            {
                UpdateSelectedValues();
            }
        }
        else
        {
            // Remove from selected chips
            _selectedChips.Remove(chip);

            if (_selectedChip == chip)
            {
                _selectedChip = null;
                SetValue(SelectedChipProperty, null);
                SetValue(SelectedValueProperty, null);
            }
            else if (AllowMultipleSelection)
            {
                // For multi-selection mode, we also need to trigger a notification
                // even though SelectedChip property doesn't change
                SetValue(SelectedChipProperty, SelectedChip);

                UpdateSelectedValues();
            }
            else
            {
                // For single selection mode, ensure notification is triggered even if this
                // isn't the currently selected chip (could be a programmatic deselection)
                SetValue(SelectedChipProperty, _selectedChip);
                SetValue(SelectedValueProperty, _selectedChip?.Value);
            }
        }

        _isUpdating = false;

        // Handle selection changed event with correct old/new parameters
        if (isToggled)
        {
            // When chip is selected, it's the new selection
            NotifySelectionChanged(null, chip);
        }
        else
        {
            // When chip is deselected, it's the old selection
            NotifySelectionChanged(chip, _selectedChip); // Pass current selectedChip as the new selection
        }
    }

    private void OnChipRemoved(object? sender, EventArgs e)
    {
        if (sender is not Chip chip)
        {
            return;
        }

        _chips.Remove(chip);
    }

    private void OnChipSizeChanged(object? sender, EventArgs e)
    {
        // Request a layout update when a chip size changes
        UpdateLayout();
    }

    private void OnChipTapped(object? sender, EventArgs e)
    {
        if (sender is not Chip chip)
        {
            return;
        }

        // Create event arguments with the chip and its index
        var args =
            new ChipTappedEventArgs
            {
                Chip = chip,
            };

        // Raise the event
        ChipTapped?.Invoke(this, args);
    }

    /// <summary>
    /// Updates the selection mode (IsSingleSelection) on all chips.
    /// </summary>
    private void UpdateChipSelectionMode()
    {
        foreach (var chip in _chips)
        {
            chip.IsSingleSelection = !AllowMultipleSelection;
        }
    }

    /// <summary>
    /// Clears all chip selections and updates the SelectedValue and SelectedValues properties.
    /// </summary>
    public void ClearSelection()
    {
        if (_isUpdating)
        {
            return;
        }

        _isUpdating = true;
        try
        {
            // Clear selected chip reference
            _selectedChip = null;

            // Clear the selected chips collection
            if (_selectedChips.Count > 0)
            {
                // Untoggle all selected chips
                foreach (var chip in _selectedChips.ToList())
                {
                    chip.IsToggled = false;
                }

                _selectedChips.Clear();
            }

            // Update bindable properties
            SetValue(SelectedChipProperty, null);
            SetValue(SelectedValueProperty, null);

            // Notify for one-way binding properties
            UpdateSelectedValues();

            // Raise the selection changed event
            NotifySelectionChanged(null, null);
        }
        finally
        {
            _isUpdating = false;
        }
    }

    private void UpdateLayout()
    {
        if (_chipContainer == null)
        {
            return;
        }

        // Update container orientation and wrapping behavior
        _chipContainer.Wrap = IsScrollable ? FlexWrap.NoWrap : FlexWrap.Wrap;

        ApplyChipInset();

        // Remove or add scrollview as needed
        if (IsScrollable && Content != _scrollView)
        {
            if (_scrollView == null)
            {
                _scrollView = new ScrollView
                {
                    Orientation = ScrollOrientation.Horizontal,
                    Content = _chipContainer,
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Never,
                    Padding = new Thickness(0),
                    Margin = new Thickness(0),
                };
            }
            else
            {
                _scrollView.Content = _chipContainer;
            }

            Content = _scrollView;
        }
        else if (!IsScrollable && Content != _chipContainer)
        {
            Content = _chipContainer;
        }

        UpdateSpacing();
        InvalidateLayout();
    }

    private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not ChipGroup chipGroup)
        {
            return;
        }

        // Clear old collection change subscription
        if (oldValue is INotifyCollectionChanged oldCollection)
        {
            oldCollection.CollectionChanged -= chipGroup.OnItemsSourceCollectionChanged;
        }

        // Clear existing chips
        chipGroup._chips.Clear();

        // Clear all selections when the item source changes
        chipGroup.ClearSelection();

        // Add new items
        if (newValue is IEnumerable items)
        {
            // Subscribe to collection changes if available
            if (newValue is INotifyCollectionChanged newCollection)
            {
                newCollection.CollectionChanged += chipGroup.OnItemsSourceCollectionChanged;
            }

            // Create chips for initial items
            chipGroup.CreateChipsFromItems(items);
        }
    }

    private void OnItemsSourceCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        // Handle changes in the source collection
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                if (e.NewItems != null && ItemTemplate != null)
                {
                    foreach (var item in e.NewItems)
                    {
                        CreateChipFromItem(item);
                    }
                }

                break;

            case NotifyCollectionChangedAction.Remove:
                if (e.OldItems != null)
                {
                    foreach (var item in e.OldItems)
                    {
                        var chip = _chips.FirstOrDefault(c => c.BindingContext == item);
                        if (chip != null)
                        {
                            _chips.Remove(chip);
                        }
                    }
                }

                break;

            case NotifyCollectionChangedAction.Reset:
                _chips.Clear();

                // Clear all selections when the collection is reset
                ClearSelection();

                if (sender is IEnumerable items)
                {
                    CreateChipsFromItems(items);
                }

                break;
        }
    }

    private static void OnItemTemplateChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not ChipGroup chipGroup || chipGroup.ItemsSource == null)
        {
            return;
        }

        // Recreate chips with the new template
        chipGroup._chips.Clear();
        chipGroup.CreateChipsFromItems(chipGroup.ItemsSource);
    }

    private void CreateChipsFromItems(IEnumerable items)
    {
        if (ItemTemplate == null)
        {
            return;
        }

        foreach (var item in items)
        {
            CreateChipFromItem(item);
        }
    }

    private void CreateChipFromItem(object item)
    {
        if (ItemTemplate == null)
        {
            return;
        }

        // Create a chip from the template
        var content = ItemTemplate.CreateContent();
        if (content is Chip chip)
        {
            chip.BindingContext = item;
            _chips.Add(chip);
        }
        else if (content is View view)
        {
            // Template returned another view type, try to find a Chip inside it
            var embeddedChip = FindChip(view);
            if (embeddedChip != null)
            {
                embeddedChip.BindingContext = item;
                _chips.Add(embeddedChip);
            }
        }
    }

    private Chip? FindChip(View view)
    {
        if (view is Chip chip)
        {
            return chip;
        }

        if (view is Layout layout)
        {
            foreach (var child in layout.Children)
            {
                if (child is View childView)
                {
                    var result = FindChip(childView);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }
        }

        return null;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        if (disposing)
        {
            // Unsubscribe from events
            _chips.CollectionChanged -= OnChipsCollectionChanged;

            // Dispose managed resources
            foreach (var chip in _chips.ToList())
            {
                chip.Toggled -= OnChipToggled;
                chip.Removed -= OnChipRemoved;
                chip.SizeChanged -= OnChipSizeChanged;

                if (chip is IDisposable disposableChip)
                {
                    disposableChip.Dispose();
                }
            }

            _chips.Clear();
            _selectedChips.Clear();

            // Clear references
            _selectedChip = null;
            _scrollView = null;
            _chipContainer = null;

            // Remove content
            Content = null;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~ChipGroup()
    {
        Dispose(false);
    }
}

public class ChipSelectionChangedEventArgs : EventArgs
{
    /// <summary>
    /// Gets or sets the chip that was deselected (may be null).
    /// </summary>
    public Chip? OldSelection { get; set; }

    /// <summary>
    /// Gets or sets the chip that was selected (may be null).
    /// </summary>
    public Chip? NewSelection { get; set; }

    /// <summary>
    /// Gets or sets all currently selected chips.
    /// </summary>
    public IList<Chip> SelectedItems { get; set; } = new List<Chip>();
}
