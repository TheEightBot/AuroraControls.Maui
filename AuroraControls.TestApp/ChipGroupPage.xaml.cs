using System.Collections.ObjectModel;
using System.ComponentModel;

namespace AuroraControls.TestApp;

public partial class ChipGroupPage : ContentPage
{
    private readonly ObservableCollection<string> _statusMessages = new();
    private int _dynamicChipCounter = 1;

    public ChipGroupPage()
    {
        InitializeComponent();
        UpdateStatus("ChipGroup test page loaded. Try interacting with the different chip groups!");

        // Setup property changed handlers to track selection changes
        BasicChipGroup.PropertyChanged +=
            (object s, PropertyChangedEventArgs e) =>
            {
                if (e.PropertyName == nameof(ChipGroup.SelectedValues))
                {
                    UpdateBasicSelectionDisplay();
                }
            };

        SingleSelectionChipGroup.PropertyChanged += (object s, PropertyChangedEventArgs e) =>
        {
            if (e.PropertyName == nameof(ChipGroup.SelectedValue))
            {
                UpdateSingleSelectionDisplay();
            }
        };

        ScrollableChipGroup.PropertyChanged += (object s, PropertyChangedEventArgs e) =>
        {
            if (e.PropertyName == nameof(ChipGroup.SelectedValues))
            {
                UpdateScrollableSelectionDisplay();
            }
        };

        MaxRowsChipGroup.PropertyChanged += (object s, PropertyChangedEventArgs e) =>
        {
            if (e.PropertyName == nameof(ChipGroup.SelectedValues))
            {
                UpdateMaxRowsSelectionDisplay();
            }

            if (e.PropertyName == nameof(ChipGroup.IsOverflow))
            {
                UpdateMaxRowsOverflowDisplay();
            }
        };

        RemovableChipGroup.PropertyChanged += (object s, PropertyChangedEventArgs e) =>
        {
            if (e.PropertyName == nameof(ChipGroup.SelectedValues))
            {
                UpdateRemovableSelectionDisplay();
            }
        };

        StyledChipGroup.PropertyChanged += (object s, PropertyChangedEventArgs e) =>
        {
            if (e.PropertyName == nameof(ChipGroup.SelectedValues))
            {
                UpdateStyledSelectionDisplay();
            }
        };

        // Initial display updates
        UpdateAllSelectionDisplays();
        UpdateMaxRowsOverflowDisplay();
    }

    private void OnChipTapped(object sender, ChipTappedEventArgs e)
    {
        UpdateStatus($"Basic ChipGroup: '{e.Chip.Text}' tapped (Value: {e.Chip.Value})");
    }

    private void OnSingleSelectionChipTapped(object sender, ChipTappedEventArgs e)
    {
        UpdateStatus($"Single Selection: '{e.Chip.Text}' tapped (Value: {e.Chip.Value})");
    }

    private void OnScrollableChipTapped(object sender, ChipTappedEventArgs e)
    {
        UpdateStatus($"Scrollable ChipGroup: '{e.Chip.Text}' tapped (Value: {e.Chip.Value})");
    }

    private void OnMaxRowsChipTapped(object sender, ChipTappedEventArgs e)
    {
        UpdateStatus($"Max Rows ChipGroup: '{e.Chip.Text}' tapped (Value: {e.Chip.Value})");
    }

    private void OnRemovableChipTapped(object sender, ChipTappedEventArgs e)
    {
        UpdateStatus($"Removable ChipGroup: '{e.Chip.Text}' tapped (Value: {e.Chip.Value})");
    }

    private void OnStyledChipTapped(object sender, ChipTappedEventArgs e)
    {
        UpdateStatus($"Styled ChipGroup: '{e.Chip.Text}' tapped (Value: {e.Chip.Value})");
    }

    private async void OnScrollToStartClicked(object sender, EventArgs e)
    {
        try
        {
            var firstChip = ScrollableChipGroup.Children.FirstOrDefault() as Chip;
            if (firstChip != null)
            {
                await ScrollableChipGroup.ScrollToAsync(firstChip, ScrollToPosition.Start, animated: true);
                UpdateStatus("Scrolled to start of scrollable chip group");
            }
        }
        catch (Exception ex)
        {
            UpdateStatus($"Error scrolling to start: {ex.Message}");
        }
    }

    private async void OnScrollToEndClicked(object sender, EventArgs e)
    {
        try
        {
            var lastChip = ScrollableChipGroup.Children.LastOrDefault() as Chip;
            if (lastChip != null)
            {
                await ScrollableChipGroup.ScrollToAsync(lastChip, ScrollToPosition.End, animated: true);
                UpdateStatus("Scrolled to end of scrollable chip group");
            }
        }
        catch (Exception ex)
        {
            UpdateStatus($"Error scrolling to end: {ex.Message}");
        }
    }

    private void OnSelectFirstThreeClicked(object sender, EventArgs e)
    {
        try
        {
            var firstThreeValues = BasicChipGroup.Children.Take(3).Select(c => (c as Chip).Value).ToList();
            BasicChipGroup.SelectedValues = firstThreeValues;
            UpdateStatus($"Selected first 3 chips in basic group: {string.Join(", ", firstThreeValues)}");
        }
        catch (Exception ex)
        {
            UpdateStatus($"Error selecting first three: {ex.Message}");
        }
    }

    private void OnClearSelectionsClicked(object sender, EventArgs e)
    {
        try
        {
            BasicChipGroup.SelectedValues = new List<object>();
            SingleSelectionChipGroup.SelectedValue = null;
            ScrollableChipGroup.SelectedValues = new List<object>();
            MaxRowsChipGroup.SelectedValues = new List<object>();
            RemovableChipGroup.SelectedValues = new List<object>();
            StyledChipGroup.SelectedValues = new List<object>();

            UpdateStatus("Cleared all selections in all chip groups");
        }
        catch (Exception ex)
        {
            UpdateStatus($"Error clearing selections: {ex.Message}");
        }
    }

    private void OnAddDynamicChipClicked(object sender, EventArgs e)
    {
        try
        {
            var newChip = new Chip
            {
                Text = $"Dynamic {_dynamicChipCounter}",
                Value = $"dynamic_{_dynamicChipCounter}",
                BackgroundColor = Colors.LightPink,
                ToggledBackgroundColor = Colors.DeepPink,
            };

            BasicChipGroup.Children.Add(newChip);
            _dynamicChipCounter++;

            UpdateStatus($"Added dynamic chip: {newChip.Text}");
        }
        catch (Exception ex)
        {
            UpdateStatus($"Error adding dynamic chip: {ex.Message}");
        }
    }

    private void OnRemoveLastChipClicked(object sender, EventArgs e)
    {
        try
        {
            if (BasicChipGroup.Children.Count > 0)
            {
                var lastChip = BasicChipGroup.Children.Last() as Chip;
                var chipText = lastChip.Text;
                BasicChipGroup.Children.Remove(lastChip);
                UpdateStatus($"Removed chip: {chipText}");
            }
            else
            {
                UpdateStatus("No chips to remove from basic group");
            }
        }
        catch (Exception ex)
        {
            UpdateStatus($"Error removing chip: {ex.Message}");
        }
    }

    private void OnToggleScrollableClicked(object sender, EventArgs e)
    {
        try
        {
            ScrollableChipGroup.Scrollable = !ScrollableChipGroup.Scrollable;
            UpdateStatus($"Scrollable ChipGroup scrolling: {(ScrollableChipGroup.Scrollable ? "Enabled" : "Disabled")}");
        }
        catch (Exception ex)
        {
            UpdateStatus($"Error toggling scrollable: {ex.Message}");
        }
    }

    private void OnToggleSelectionModeClicked(object sender, EventArgs e)
    {
        try
        {
            BasicChipGroup.IsSingleSelection = !BasicChipGroup.IsSingleSelection;

            // Clear selections when changing mode
            BasicChipGroup.SelectedValues = new List<object>();

            var mode = BasicChipGroup.IsSingleSelection ? "Single Selection" : "Multi-Selection";
            UpdateStatus($"Basic ChipGroup mode changed to: {mode}");
        }
        catch (Exception ex)
        {
            UpdateStatus($"Error toggling selection mode: {ex.Message}");
        }
    }

    private void UpdateAllSelectionDisplays()
    {
        UpdateBasicSelectionDisplay();
        UpdateSingleSelectionDisplay();
        UpdateScrollableSelectionDisplay();
        UpdateMaxRowsSelectionDisplay();
        UpdateRemovableSelectionDisplay();
        UpdateStyledSelectionDisplay();
    }

    private void UpdateBasicSelectionDisplay()
    {
        try
        {
            var selected = BasicChipGroup.SelectedValues?.ToList() ?? new List<object>();
            var mode = BasicChipGroup.IsSingleSelection ? " (Single)" : " (Multi)";
            BasicSelectionLabel.Text = selected.Any()
                ? $"Selected{mode}: {string.Join(", ", selected)}"
                : $"Selected{mode}: None";
        }
        catch (Exception ex)
        {
            BasicSelectionLabel.Text = $"Error: {ex.Message}";
        }
    }

    private void UpdateSingleSelectionDisplay()
    {
        try
        {
            var selected = SingleSelectionChipGroup.SelectedValue;
            SingleSelectionLabel.Text = selected != null
                ? $"Selected: {selected}"
                : "Selected: None";
        }
        catch (Exception ex)
        {
            SingleSelectionLabel.Text = $"Error: {ex.Message}";
        }
    }

    private void UpdateScrollableSelectionDisplay()
    {
        try
        {
            var selected = ScrollableChipGroup.SelectedValues?.ToList() ?? new List<object>();
            ScrollableSelectionLabel.Text = selected.Any()
                ? $"Selected: {string.Join(", ", selected)}"
                : "Selected: None";
        }
        catch (Exception ex)
        {
            ScrollableSelectionLabel.Text = $"Error: {ex.Message}";
        }
    }

    private void UpdateMaxRowsSelectionDisplay()
    {
        try
        {
            var selected = MaxRowsChipGroup.SelectedValues?.ToList() ?? new List<object>();
            MaxRowsSelectionLabel.Text = selected.Any()
                ? $"Selected: {string.Join(", ", selected)}"
                : "Selected: None";
        }
        catch (Exception ex)
        {
            MaxRowsSelectionLabel.Text = $"Error: {ex.Message}";
        }
    }

    private void UpdateMaxRowsOverflowDisplay()
    {
        try
        {
            MaxRowsLabel.Text = $"Overflow Status: {(MaxRowsChipGroup.IsOverflow ? "Yes" : "No")}";
        }
        catch (Exception ex)
        {
            MaxRowsLabel.Text = $"Overflow Error: {ex.Message}";
        }
    }

    private void UpdateRemovableSelectionDisplay()
    {
        try
        {
            var selected = RemovableChipGroup.SelectedValues?.ToList() ?? new List<object>();
            RemovableSelectionLabel.Text = selected.Any()
                ? $"Selected: {string.Join(", ", selected)}"
                : "Selected: None";
        }
        catch (Exception ex)
        {
            RemovableSelectionLabel.Text = $"Error: {ex.Message}";
        }
    }

    private void UpdateStyledSelectionDisplay()
    {
        try
        {
            var selected = StyledChipGroup.SelectedValues?.ToList() ?? new List<object>();
            StyledSelectionLabel.Text = selected.Any()
                ? $"Selected: {string.Join(", ", selected)}"
                : "Selected: None";
        }
        catch (Exception ex)
        {
            StyledSelectionLabel.Text = $"Error: {ex.Message}";
        }
    }

    private void UpdateStatus(string message)
    {
        try
        {
            StatusLabel.Text = $"{DateTime.Now:HH:mm:ss} - {message}";

            // Optional: Keep a history of status messages
            _statusMessages.Insert(0, StatusLabel.Text);
            if (_statusMessages.Count > 10)
            {
                _statusMessages.RemoveAt(_statusMessages.Count - 1);
            }
        }
        catch (Exception ex)
        {
            StatusLabel.Text = $"Status update error: {ex.Message}";
        }
    }
}
