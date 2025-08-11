using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace AuroraControls.TestApp;

public partial class ListViewHideEmptyCellsEffectTestPage : ContentPage
{
    public ListViewHideEmptyCellsEffectTestPage()
    {
        InitializeComponent();
        BindingContext = new ListViewHideEmptyCellsEffectViewModel();
    }
}

public class ListViewHideEmptyCellsEffectViewModel : INotifyPropertyChanged
{
    private readonly string[] _sampleNames = { "Apple", "Banana", "Cherry", "Date", "Elderberry", "Fig", "Grape", "Honeydew", "Kiwi", "Lemon", "Mango", "Orange", "Papaya", "Quince", "Raspberry", "Strawberry", "Tangerine", "Ugli fruit", "Vanilla bean", "Watermelon" };
    private readonly string[] _sampleDescriptions = { "Sweet and crunchy", "Rich in potassium", "Small and tart", "Naturally sweet", "Dark purple berry", "Soft and sweet", "Juicy and versatile", "Refreshing melon", "Tangy green fruit", "Citrus with vitamin C", "Tropical and sweet", "Bright citrus fruit", "Tropical orange fruit", "Aromatic fruit", "Red summer berry", "Classic red berry", "Orange citrus", "Unique citrus", "Aromatic spice", "Large summer fruit" };
    private readonly Random _random = new();

    private double _itemCount = 5;

    public ListViewHideEmptyCellsEffectViewModel()
    {
        AddItemCommand = new Command(OnAddItem);
        RemoveItemCommand = new Command(OnRemoveItem, CanRemoveItem);

        Items = new ObservableCollection<ListItem>();
        InitializeItems();
        UpdateStatusMessage();
    }

    public ObservableCollection<ListItem> Items { get; }

    public double ItemCount
    {
        get => _itemCount;
        set
        {
            _itemCount = value;
            OnPropertyChanged();
            UpdateItemsToCount((int)value);
            UpdateStatusMessage();
        }
    }

    public string StatusMessage => $"Currently showing {Items.Count} items. Empty cells {(DeviceInfo.Platform == DevicePlatform.iOS || DeviceInfo.Platform == DevicePlatform.macOS ? "should be hidden" : "will be visible")} on the right ListView.";

    public string PlatformMessage => $"Current platform: {DeviceInfo.Platform}. Effect only works on iOS and macOS.";

    public ICommand AddItemCommand { get; }

    public ICommand RemoveItemCommand { get; }

    private void InitializeItems()
    {
        Items.Clear();
        for (int i = 0; i < _itemCount; i++)
        {
            AddRandomItem();
        }
    }

    private void UpdateItemsToCount(int targetCount)
    {
        while (Items.Count > targetCount)
        {
            Items.RemoveAt(Items.Count - 1);
        }

        while (Items.Count < targetCount)
        {
            AddRandomItem();
        }

        ((Command)RemoveItemCommand).ChangeCanExecute();
    }

    private void AddRandomItem()
    {
        var index = _random.Next(_sampleNames.Length);
        var item = new ListItem
        {
            Name = $"{_sampleNames[index]} #{Items.Count + 1}",
            Description = _sampleDescriptions[index],
        };
        Items.Add(item);
    }

    private void OnAddItem()
    {
        AddRandomItem();
        ItemCount = Items.Count;
        UpdateStatusMessage();
        ((Command)RemoveItemCommand).ChangeCanExecute();
    }

    private void OnRemoveItem()
    {
        if (Items.Count > 0)
        {
            Items.RemoveAt(Items.Count - 1);
            ItemCount = Items.Count;
            UpdateStatusMessage();
            ((Command)RemoveItemCommand).ChangeCanExecute();
        }
    }

    private bool CanRemoveItem()
    {
        return Items.Count > 1;
    }

    private void UpdateStatusMessage()
    {
        OnPropertyChanged(nameof(StatusMessage));
        OnPropertyChanged(nameof(PlatformMessage));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class ListItem
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}
