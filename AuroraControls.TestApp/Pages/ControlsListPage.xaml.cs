// <copyright file="ControlsListPage.xaml.cs" company="TheEightBot">
// Copyright (c) TheEightBot. All rights reserved.
// </copyright>

using System.Collections.ObjectModel;

namespace AuroraControls.TestApp.Pages;

/// <summary>
/// Page displaying all available controls with search functionality.
/// </summary>
public partial class ControlsListPage : ContentPage
{
    private readonly List<ControlItem> _allControls;

    /// <summary>
    /// Initializes a new instance of the <see cref="ControlsListPage"/> class.
    /// </summary>
    public ControlsListPage()
    {
        _allControls = CreateControlsList();
        FilteredControls = new ObservableCollection<ControlItem>(_allControls);
        InitializeComponent();
    }

    /// <summary>
    /// Gets the filtered controls collection for display.
    /// </summary>
    public ObservableCollection<ControlItem> FilteredControls { get; }

    private static List<ControlItem> CreateControlsList() => new()
    {
        // Buttons
        new ControlItem("Gradient Pill Button", "Beautiful gradient buttons", "🔘 Buttons", Color.FromArgb("#7C3AED"), typeof(GradientCircularButtonTestPage)),
        new ControlItem("Gradient Circular Button", "Circular action buttons", "🔘 Buttons", Color.FromArgb("#EC4899"), typeof(GradientCircularButtonTestPage)),
        new ControlItem("Cupertino Button", "iOS-style buttons", "🔘 Buttons", Color.FromArgb("#3B82F6"), typeof(CupertinoButtonTestPage)),
        new ControlItem("Tile", "Interactive tile buttons", "🔘 Buttons", Color.FromArgb("#14B8A6"), typeof(TileTestPage)),
        new ControlItem("SVG Image Button", "SVG-based buttons", "🔘 Buttons", Color.FromArgb("#8B5CF6"), typeof(SvgImageButtonTestPage)),

        // Input Controls
        new ControlItem("Toggle Box", "Animated toggle switches", "📝 Input Controls", Color.FromArgb("#22C55E"), typeof(ToggleBoxTestPage)),
        new ControlItem("Styled Input Layout", "Material-style inputs", "📝 Input Controls", Color.FromArgb("#F59E0B"), typeof(StyledInputLayoutTestPage)),
        new ControlItem("Chip Group", "Tag-style chip selection", "📝 Input Controls", Color.FromArgb("#8B5CF6"), typeof(Chips.ChipGroupDemoPage)),
        new ControlItem("Step Indicator", "Multi-step progress", "📝 Input Controls", Color.FromArgb("#3B82F6"), typeof(StepIndicatorTestPage)),
        new ControlItem("Numeric Entry", "Numeric value input", "📝 Input Controls", Color.FromArgb("#EC4899"), typeof(Inputs.NumericEntryDemoPage)),
        new ControlItem("Cupertino Toggle Switch", "iOS-style toggle", "📝 Input Controls", Color.FromArgb("#14B8A6"), typeof(Inputs.CupertinoToggleSwitchDemoPage)),
        new ControlItem("Segmented Control", "Multi-style segments", "📝 Input Controls", Color.FromArgb("#7C3AED"), typeof(Inputs.SegmentedControlDemoPage)),

        // Calendar
        new ControlItem("Calendar View", "Full calendar control", "📅 Calendar & Date", Color.FromArgb("#3B82F6"), typeof(Calendar.CalendarViewDemoPage)),
        new ControlItem("Calendar Picker", "Nullable date picker", "📅 Calendar & Date", Color.FromArgb("#7C3AED"), typeof(Calendar.CalendarPickerDemoPage)),

        // Animations
        new ControlItem("Confetti View", "Celebration animations", "🎉 Animations", Color.FromArgb("#22C55E"), typeof(ConfettiViewTestPage)),
        new ControlItem("Cutout Overlay", "Spotlight overlay effects", "🎉 Animations", Color.FromArgb("#EC4899"), typeof(CutoutOverlayViewTestPage)),

        // Images & Graphics
        new ControlItem("SVG Image View", "Vector graphics display", "🖼️ Images & Graphics", Color.FromArgb("#F59E0B"), typeof(Imaging.SvgImageViewDemoPage)),
        new ControlItem("Signature Pad", "Capture signatures", "🖼️ Images & Graphics", Color.FromArgb("#EF4444"), typeof(SignaturePadPage)),
        new ControlItem("Touch Draw Letters", "Draw letter input", "🖼️ Images & Graphics", Color.FromArgb("#14B8A6"), typeof(Imaging.TouchDrawLettersDemoPage)),
        new ControlItem("Grid Image", "Grid image display", "🖼️ Images & Graphics", Color.FromArgb("#7C3AED"), typeof(GridImagePage)),

        // Layouts
        new ControlItem("Wrap Layout", "Flowing wrap layout", "📐 Layouts", Color.FromArgb("#3B82F6"), typeof(WrapLayoutTestPage)),
        new ControlItem("Card View Layout", "Card-based layouts", "📐 Layouts", Color.FromArgb("#EC4899"), typeof(CardViewLayoutPage)),
    };

    private void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        var searchText = e.NewTextValue?.Trim().ToLowerInvariant() ?? string.Empty;
        ClearButton.IsVisible = !string.IsNullOrEmpty(searchText);

        FilteredControls.Clear();

        var filtered = string.IsNullOrEmpty(searchText)
            ? _allControls
            : _allControls.Where(c =>
                c.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                c.Description.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                c.Category.Contains(searchText, StringComparison.OrdinalIgnoreCase)).ToList();

        foreach (var control in filtered)
        {
            FilteredControls.Add(control);
        }

        if (!string.IsNullOrEmpty(searchText))
        {
            ResultsLabel.Text = $"{filtered.Count} of {_allControls.Count} controls";
            ResultsLabel.IsVisible = true;
        }
        else
        {
            ResultsLabel.IsVisible = false;
        }
    }

    private void OnClearSearchTapped(object? sender, TappedEventArgs e)
    {
        SearchEntry.Text = string.Empty;
        SearchEntry.Unfocus();
    }

    private async void OnControlSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is ControlItem selectedControl)
        {
            // Clear selection
            ControlsCollection.SelectedItem = null;

            // Navigate to the control's page
            if (selectedControl.PageType != null)
            {
                var page = (Page?)Activator.CreateInstance(selectedControl.PageType);
                if (page != null)
                {
                    await Navigation.PushAsync(page);
                }
            }
        }
    }
}

/// <summary>
/// Represents a control item in the list.
/// </summary>
public class ControlItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ControlItem"/> class.
    /// </summary>
    /// <param name="name">The control name.</param>
    /// <param name="description">The control description.</param>
    /// <param name="category">The category.</param>
    /// <param name="accentColor">The accent color.</param>
    /// <param name="pageType">The page type to navigate to.</param>
    public ControlItem(string name, string description, string category, Color accentColor, Type? pageType)
    {
        Name = name;
        Description = description;
        Category = category;
        AccentColor = accentColor;
        PageType = pageType;
    }

    /// <summary>
    /// Gets the control name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the control description.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Gets the category.
    /// </summary>
    public string Category { get; }

    /// <summary>
    /// Gets the accent color.
    /// </summary>
    public Color AccentColor { get; }

    /// <summary>
    /// Gets the page type to navigate to.
    /// </summary>
    public Type? PageType { get; }
}
