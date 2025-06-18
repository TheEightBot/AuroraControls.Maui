using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AuroraControls.TestApp.ViewModels;

public partial class ChipGroupViewModel : ObservableObject
{
    private readonly Random _random = new();
    private readonly string[] _chipNames = new[]
    {
        "Apple", "Banana", "Cherry", "Dragon Fruit", "Elderberry",
        "Fig", "Grape", "Honeydew", "Kiwi", "Lemon", "Mango",
        "Nectarine", "Orange", "Papaya", "Quince", "Raspberry",
        "Strawberry", "Tangerine", "Ugli Fruit", "Vanilla", "Watermelon",
    };

    private readonly string[] _icons = new[]
    {
        "more.svg",
        "triforce.svg",
        null,
    };

    private readonly string[] _colors = new[]
    {
        "#FF5252", "#FF4081", "#E040FB", "#7C4DFF", "#536DFE",
        "#448AFF", "#40C4FF", "#18FFFF", "#64FFDA", "#69F0AE",
        "#B2FF59", "#EEFF41", "#FFFF00", "#FFD740", "#FFAB40",
        "#FF6E40", "#8D6E63", "#78909C",
    };

    [ObservableProperty]
    private bool _isScrollable;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelectedSelectionMode))]
    private bool _allowMultipleSelection;

    [ObservableProperty]
    private double _horizontalSpacing = 8;

    [ObservableProperty]
    private double _verticalSpacing = 8;

    [ObservableProperty]
    private string _selectedChipsText = "None";

    [ObservableProperty]
    private object _selectedValue;

    [ObservableProperty]
    private string _selectedValuesText = "None";

    public ObservableCollection<string> LayoutModes { get; } = new()
    {
        "Multi Line (Wrap)",
        "Single Line (Scrollable)",
    };

    public ObservableCollection<string> SelectionModes { get; } = new()
    {
        "Single Selection",
        "Multiple Selection",
    };

    public ObservableCollection<ChipItemViewModel> ChipItems { get; } = new();

    public string SelectedLayoutMode => IsScrollable ? "Single Line (Scrollable)" : "Multi Line (Wrap)";

    public string SelectedSelectionMode => AllowMultipleSelection ? "Multiple Selection" : "Single Selection";

    public ObservableCollection<string> ChipNames { get; } = new();

    public ChipGroupViewModel()
    {
        // Populate the ChipNames collection
        foreach (var name in _chipNames)
        {
            ChipNames.Add(name);
        }

        // Add some initial chips
        for (int i = 0; i < 8; i++)
        {
            AddRandomChip();
        }
    }

    public void AddRandomChip()
    {
        string name = _chipNames[_random.Next(_chipNames.Length)];

        var chip = new ChipItemViewModel
        {
            Name = name,
            Value = name, // Set the Value property to match the Name for demonstration purposes
            IconSource = _icons[_random.Next(_icons.Length)],
            IsClosable = _random.Next(2) == 0,
            BackgroundColor = _colors[_random.Next(_colors.Length)],
        };

        ChipItems.Add(chip);
    }

    public void ClearChips()
    {
        ChipItems.Clear();
        SelectedChipsText = "None";
    }

    /// <summary>
    /// Find a chip item by its value.
    /// </summary>
    /// <param name="value">The value to search for.</param>
    /// <returns>The matching chip item or null if not found.</returns>
    public ChipItemViewModel GetChipByValue(string value)
    {
        return ChipItems.FirstOrDefault(c => string.Equals(c.Value, value, StringComparison.OrdinalIgnoreCase));
    }
}

public partial class ChipItemViewModel : ObservableObject
{
    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string _iconSource;

    [ObservableProperty]
    private bool _isClosable;

    [ObservableProperty]
    private string _backgroundColor;

    [ObservableProperty]
    private string _value;
}
