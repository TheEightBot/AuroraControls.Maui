using System.Diagnostics;
using AuroraControls.TestApp.ViewModels;

namespace AuroraControls.TestApp;

public partial class ChipGroupPage : ContentPage
{
    private readonly Random _random = new();

    public ChipGroupPage()
    {
        InitializeComponent();
    }

    private void OnLayoutModeChanged(object sender, EventArgs e)
    {
        if (sender is Picker picker && BindingContext is ChipGroupViewModel viewModel)
        {
            viewModel.IsScrollable = picker.SelectedItem?.ToString() == "Single Line (Scrollable)";
        }
    }

    private void OnSelectionModeChanged(object sender, EventArgs e)
    {
        if (sender is Picker picker && BindingContext is ChipGroupViewModel viewModel)
        {
            viewModel.AllowMultipleSelection = picker.SelectedItem?.ToString() == "Multiple Selection";
        }
    }

    private void OnHorizontalSpacingChanged(object sender, ValueChangedEventArgs e)
    {
        if (ChipGroupSample != null)
        {
            ChipGroupSample.HorizontalSpacing = e.NewValue;
        }
    }

    private void OnVerticalSpacingChanged(object sender, ValueChangedEventArgs e)
    {
        if (ChipGroupSample != null)
        {
            ChipGroupSample.VerticalSpacing = e.NewValue;
        }
    }

    private void OnAddChipClicked(object sender, EventArgs e)
    {
        if (BindingContext is ChipGroupViewModel viewModel)
        {
            viewModel.AddRandomChip();
        }
    }

    private void OnClearChipsClicked(object sender, EventArgs e)
    {
        if (BindingContext is ChipGroupViewModel viewModel)
        {
            viewModel.ClearChips();
        }
    }

    private void OnScrollToRandomClicked(object sender, EventArgs e)
    {
        if (ChipGroupSample != null && ChipGroupSample.Chips.Count > 0)
        {
            // Get a random chip from the collection
            int randomIndex = _random.Next(ChipGroupSample.Chips.Count);
            var randomChip = ChipGroupSample.Chips[randomIndex];

            // Scroll to the random chip
            bool success = ChipGroupSample.ScrollToChip(randomChip, ScrollToPosition.Center);

            // Provide visual feedback to the user
            if (success)
            {
                // Briefly toggle the chip to provide visual feedback
                randomChip.IsToggled = true;

                // Create a timer to untoggle the chip after a short delay if it's not meant to stay selected
                if (!randomChip.IsToggled)
                {
                    var timer = Application.Current.Dispatcher.CreateTimer();
                    timer.Interval = TimeSpan.FromMilliseconds(500);
                    timer.Tick += (s, args) =>
                    {
                        randomChip.IsToggled = false;
                        timer.Stop();
                    };
                    timer.Start();
                }
            }
            else
            {
                DisplayAlert("Scrolling Failed", "Could not scroll to the selected chip. Make sure the ChipGroup is in scrollable mode.", "OK");
            }
        }
        else
        {
            DisplayAlert("No Chips", "There are no chips to scroll to. Please add some chips first.", "OK");
        }
    }

    private void OnScrollToSelectionClicked(object sender, EventArgs e)
    {
        if (ChipGroupSample != null)
        {
            bool success = ChipGroupSample.ScrollToSelectedChip(ScrollToPosition.Center);

            if (!success)
            {
                // Try to scroll to any selected chip if in multi-selection mode
                if (ChipGroupSample.AllowMultipleSelection && ChipGroupSample.SelectedChips.Count > 0)
                {
                    // Scroll to the first selected chip
                    success = ChipGroupSample.ScrollToChip(ChipGroupSample.SelectedChips[0], ScrollToPosition.MakeVisible);
                }

                if (!success)
                {
                    DisplayAlert("Scrolling Failed", "No selected chip to scroll to, or the ChipGroup is not in scrollable mode.", "OK");
                }
            }
        }
    }

    private void OnScrollToValueClicked(object sender, EventArgs e)
    {
        if (ValuePicker.SelectedItem is string selectedValue && !string.IsNullOrEmpty(selectedValue))
        {
            bool success = ChipGroupSample.ScrollToChipWithValue(selectedValue, ScrollToPosition.Center, true, StringComparer.OrdinalIgnoreCase);

            if (!success)
            {
                DisplayAlert("Scrolling Failed", $"Could not find a chip with value '{selectedValue}' or the ChipGroup is not in scrollable mode.", "OK");
            }
        }
        else
        {
            DisplayAlert("No Value Selected", "Please select a value from the dropdown first.", "OK");
        }
    }

    private void OnSelectValueClicked(object sender, EventArgs e)
    {
        if (ValuePicker.SelectedItem is string selectedValue && !string.IsNullOrEmpty(selectedValue))
        {
            // Either of these approaches work:

            // Option 1: Using the SelectedValue property
            if (BindingContext is ChipGroupViewModel viewModel)
            {
                viewModel.SelectedValue = selectedValue;
            }

            // Option 2: Using the SelectChipByValue method
            // bool success = ChipGroupSample.SelectChipByValue(selectedValue);

            // Provide feedback on success or failure
            DisplayAlert("Information", $"Selected chip with value: '{selectedValue}'", "OK");
        }
        else
        {
            DisplayAlert("No Value Selected", "Please select a value from the dropdown first.", "OK");
        }
    }

    private void OnChipSelectionChanged(object sender, ChipSelectionChangedEventArgs e)
    {
        if (BindingContext is ChipGroupViewModel viewModel && sender is ChipGroup chipGroup)
        {
            // Update selected chips text
            string selectedChips = string.Join(", ", e.SelectedItems.Select(c => c.Text));
            viewModel.SelectedChipsText = string.IsNullOrEmpty(selectedChips) ? "None" : selectedChips;

            // Update selected values text from the ChipGroup's SelectedValues
            string selectedValues = string.Join(", ", chipGroup.SelectedValues);
            viewModel.SelectedValuesText = string.IsNullOrEmpty(selectedValues) ? "None" : selectedValues;

            Debug.WriteLine($"Selection Changed: New={e.NewSelection?.Text ?? "None"}, " +
                           $"Old={e.OldSelection?.Text ?? "None"}, " +
                           $"Total Selected={e.SelectedItems.Count}, " +
                           $"SelectedValue={viewModel.SelectedValue ?? "None"}");
        }
    }

    private async void OnShowSelectedValuesClicked(object sender, EventArgs e)
    {
        if (ChipGroupSample != null)
        {
            // Get the current selected value (for single selection mode)
            string selectedValue = ChipGroupSample.SelectedValue?.ToString() ?? "None";

            // Get all selected values (for multi-selection mode)
            string selectedValues = string.Join(", ", ChipGroupSample.SelectedValues.Select(v => v?.ToString() ?? "null"));
            if (string.IsNullOrEmpty(selectedValues))
            {
                selectedValues = "None";
            }

            await DisplayAlert(
                "Selected Values",
                $"SelectedValue: {selectedValue}\n\nSelectedValues: {selectedValues}",
                "OK");
        }
    }

    private void OnChipTapped(object sender, ChipTappedEventArgs e)
    {
        if (e.Chip?.State == ChipState.ReadOnly)
        {
            DisplayAlert("Chip Tapped", $"Chip: {e.Chip.Text}", "OK");
        }
    }
}
