// Copyright (c) Aurora Controls. All rights reserved.

using System.Collections.ObjectModel;

namespace AuroraControls.TestApp.Pages;

/// <summary>
/// Represents a group of controls in a category.
/// </summary>
public class ControlGroup : List<ControlItem>
{
    /// <summary>
    /// Gets or sets the name of the group.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the icon for the group.
    /// </summary>
    public string Icon { get; set; } = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="ControlGroup"/> class.
    /// </summary>
    /// <param name="name">The name of the group.</param>
    /// <param name="icon">The icon for the group.</param>
    /// <param name="items">The items in the group.</param>
    public ControlGroup(string name, string icon, IEnumerable<ControlItem> items)
        : base(items)
    {
        Name = name;
        Icon = icon;
    }
}

/// <summary>
/// Represents a single control item in the list.
/// </summary>
public class ControlItem
{
    /// <summary>
    /// Gets or sets the name of the control.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the control.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the icon for the control.
    /// </summary>
    public string Icon { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the navigation route for the control.
    /// </summary>
    public string Route { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the category of the control.
    /// </summary>
    public string Category { get; set; } = string.Empty;
}

/// <summary>
/// Controls list page showing all available controls grouped by category.
/// </summary>
public partial class ControlsListPage : ContentPage
{
    private readonly List<ControlGroup> _allGroups;
    private readonly ObservableCollection<ControlGroup> _filteredGroups;

    /// <summary>
    /// Initializes a new instance of the <see cref="ControlsListPage"/> class.
    /// </summary>
    public ControlsListPage()
    {
        InitializeComponent();

        _allGroups = CreateControlGroups();
        _filteredGroups = new ObservableCollection<ControlGroup>(_allGroups);

        ControlsCollection.ItemsSource = _filteredGroups;
        ControlsCollection.IsGrouped = true;
    }

    private static List<ControlGroup> CreateControlGroups()
    {
        return
        [
            new ControlGroup(
                "Buttons",
                "🔘",
                [
                    new ControlItem { Name = "Gradient Pill Button", Description = "Gradient-filled pill-shaped button", Icon = "💊", Route = "gradientpillbutton", Category = "Buttons", },
                    new ControlItem { Name = "Gradient Circular Button", Description = "Circular button with gradient fill", Icon = "⭕", Route = "gradientcircularbutton", Category = "Buttons", },
                    new ControlItem { Name = "Cupertino Button", Description = "iOS-style button", Icon = "🍎", Route = "cupertinobutton", Category = "Buttons", },
                    new ControlItem { Name = "Tile", Description = "Tappable tile control", Icon = "🔲", Route = "tile", Category = "Buttons", },
                    new ControlItem { Name = "SVG Image Button", Description = "Button with SVG icon", Icon = "🖼️", Route = "svgimagebutton", Category = "Buttons", },
                ]),

            new ControlGroup(
                "Inputs",
                "📝",
                [
                    new ControlItem { Name = "Styled Input Layout", Description = "Floating label input fields", Icon = "✏️", Route = "styledinputlayout", Category = "Inputs", },
                    new ControlItem { Name = "Numeric Entry", Description = "Number-only entry field", Icon = "🔢", Route = "numericentry", Category = "Inputs", },
                    new ControlItem { Name = "Toggle Box", Description = "Checkbox-style toggle", Icon = "☑️", Route = "togglebox", Category = "Inputs", },
                    new ControlItem { Name = "Cupertino Toggle Switch", Description = "iOS-style toggle switch", Icon = "🎚️", Route = "cupertinotoggleswitch", Category = "Inputs", },
                    new ControlItem { Name = "Segmented Control", Description = "Segmented selection control", Icon = "🔀", Route = "segmentedcontrol", Category = "Inputs", },
                ]),

            new ControlGroup(
                "Calendar",
                "📅",
                [
                    new ControlItem { Name = "Calendar View", Description = "Full calendar display", Icon = "📆", Route = "calendarview", Category = "Calendar", },
                    new ControlItem { Name = "Calendar Picker", Description = "Date selection picker", Icon = "📅", Route = "calendarpicker", Category = "Calendar", },
                ]),

            new ControlGroup(
                "Chips",
                "🏷️",
                [
                    new ControlItem { Name = "Chip Group", Description = "Multi-select chip collection", Icon = "🏷️", Route = "chipgroup", Category = "Chips", },
                ]),

            new ControlGroup(
                "Images",
                "🖼️",
                [
                    new ControlItem { Name = "SVG Image View", Description = "SVG rendering view", Icon = "🎨", Route = "svgimageview", Category = "Images", },
                    new ControlItem { Name = "Touch Draw", Description = "Freehand drawing canvas", Icon = "✍️", Route = "touchdraw", Category = "Images", },
                    new ControlItem { Name = "Signature Pad", Description = "Signature capture control", Icon = "🖊️", Route = "signaturepad", Category = "Images", },
                    new ControlItem { Name = "Cutout Overlay", Description = "Image with cutout overlay", Icon = "✂️", Route = "cutoutoverlay", Category = "Images", },
                ]),

            new ControlGroup(
                "Progress",
                "📊",
                [
                    new ControlItem { Name = "Gauges", Description = "Arc and radial gauges", Icon = "📈", Route = "gauges", Category = "Progress", },
                    new ControlItem { Name = "Step Indicator", Description = "Multi-step progress", Icon = "🔢", Route = "stepindicator", Category = "Progress", },
                    new ControlItem { Name = "Loading Indicators", Description = "Activity spinners", Icon = "⏳", Route = "loadingindicators", Category = "Progress", },
                ]),

            new ControlGroup(
                "Animations",
                "🎬",
                [
                    new ControlItem { Name = "Confetti", Description = "Celebration effects", Icon = "🎉", Route = "confetti", Category = "Animations", },
                    new ControlItem { Name = "Visual Effects", Description = "Blur and other effects", Icon = "✨", Route = "visualeffects", Category = "Animations", },
                ]),

            new ControlGroup(
                "Layouts",
                "📐",
                [
                    new ControlItem { Name = "Wrap Layout", Description = "Flowing wrap layout", Icon = "↩️", Route = "wraplayout", Category = "Layouts", },
                    new ControlItem { Name = "Card View Layout", Description = "Card-based layout", Icon = "🃏", Route = "cardviewlayout", Category = "Layouts", },
                ]),
        ];
    }

    private async void OnControlSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is ControlItem item)
        {
            ControlsCollection.SelectedItem = null;

            try
            {
                await Shell.Current.GoToAsync(item.Route);
            }
            catch
            {
                await DisplayAlert("Coming Soon", $"The {item.Name} demo page is coming in a future update.", "OK");
            }
        }
    }

    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        var searchText = e.NewTextValue?.ToLowerInvariant() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(searchText))
        {
            _filteredGroups.Clear();
            foreach (var group in _allGroups)
            {
                _filteredGroups.Add(group);
            }
        }
        else
        {
            _filteredGroups.Clear();
            foreach (var group in _allGroups)
            {
                var matchingItems = group
                    .Where(item =>
                        item.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                        item.Description.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                        item.Category.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (matchingItems.Count > 0)
                {
                    _filteredGroups.Add(new ControlGroup(group.Name, group.Icon, matchingItems));
                }
            }
        }
    }
}
