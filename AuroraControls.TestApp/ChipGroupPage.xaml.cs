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

    private void OnChipSelectionChanged(object sender, ChipSelectionChangedEventArgs e)
    {
        if (BindingContext is ChipGroupViewModel viewModel)
        {
            string selectedChips = string.Join(", ", e.SelectedItems.Select(c => c.Text));
            viewModel.SelectedChipsText = string.IsNullOrEmpty(selectedChips) ? "None" : selectedChips;

            Debug.WriteLine($"Selection Changed: New={e.NewSelection?.Text ?? "None"}, " +
                           $"Old={e.OldSelection?.Text ?? "None"}, " +
                           $"Total Selected={e.SelectedItems.Count}");
        }
    }
}
