using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AuroraControls.TestApp;

public class ToggleBoxCollectionViewIssuePage : ContentPage
{
    public ToggleBoxCollectionViewIssueViewModel ViewModel
    {
        get;
        private set;
    }

    public ToggleBoxCollectionViewIssuePage()
    {
        BindingContext = ViewModel =
            new ToggleBoxCollectionViewIssueViewModel
            {
                Items = CreateItems(),
            };

        Content =
            new Grid()
            {
                RowDefinitions = GridRowsColumns.Rows.Define(GridLength.Auto, GridLength.Auto, GridLength.Auto, GridLength.Auto, GridLength.Star),
                Children =
                {
                    new Label
                    {
                      Text = "Switch Toggle",
                    }
                        .Row(0),
                    new Switch()
                        .Bind(
                            Switch.IsToggledProperty,
                            getter: static (ToggleBoxCollectionViewIssueViewModel vm) => vm.SwitchToggle,
                            setter: static (ToggleBoxCollectionViewIssueViewModel vm, bool value) => vm.SwitchToggle = value)
                        .Row(1),
                    new Label
                    {
                        Text = "Toggle All",
                    }
                        .Row(2),
                    new Switch()
                        .Bind(
                            Switch.IsToggledProperty,
                            getter: static (ToggleBoxCollectionViewIssueViewModel vm) => vm.ToggleAll,
                            setter: static (ToggleBoxCollectionViewIssueViewModel vm, bool value) => vm.ToggleAll = value)
                        .Row(3),
                    new CollectionView
                        {
                            ItemTemplate = new DataTemplate(typeof(ToggleBoxView)),
                        }
                        .Bind(
                            CollectionView.ItemsSourceProperty,
                            getter: static (ToggleBoxCollectionViewIssueViewModel vm) => vm.Items)
                        .Row(4),
                },
            };
    }

    public List<ToggleBoxViewModel> CreateItems()
    {
        var items = new List<ToggleBoxViewModel>();

        for (int i = 0; i < 100; i++)
        {
            items.Add(new ToggleBoxViewModel());
        }

        return items;
    }
}

public class ToggleBoxView : ContentView
{
    public ToggleBoxView()
    {
        Content =
            new Grid
            {
                HeightRequest = 60,
                ColumnDefinitions = GridRowsColumns.Columns.Define(GridLength.Star, GridLength.Star, GridLength.Star, GridLength.Star),
                Children =
                {
                    new ToggleBox
                    {
                        CheckColor = Colors.Fuchsia,
                        BorderColor = Colors.Fuchsia,
                    }
                        .Bind(
                            ToggleBox.IsToggledProperty,
                            getter: static (ToggleBoxViewModel vm) => vm.ToggleOne,
                            setter: static (ToggleBoxViewModel vm, bool value) => vm.ToggleOne = value)
                        .Bind(
                            ToggleBox.IsVisibleProperty,
                            getter: static (ToggleBoxViewModel vm) => vm.ToggleOneIsVisible)
                        .Row(0).Column(0),
                    new ToggleBox
                    {
                        CheckColor = Colors.Chartreuse,
                        BorderColor = Colors.Chartreuse,
                    }
                        .Bind(
                            ToggleBox.IsToggledProperty,
                            getter: static (ToggleBoxViewModel vm) => vm.ToggleTwo,
                            setter: static (ToggleBoxViewModel vm, bool value) => vm.ToggleTwo = value)
                        .Bind(
                            ToggleBox.IsVisibleProperty,
                            getter: static (ToggleBoxViewModel vm) => vm.ToggleOneIsVisible,
                            convert: value => !value)
                        .Row(0).Column(0),
                },
            };
    }
}

public partial class ToggleBoxCollectionViewIssueViewModel : ObservableObject
{
    [ObservableProperty]
    public partial bool SwitchToggle { get; set; }

    [ObservableProperty]
    public partial bool ToggleAll { get; set; }

    [ObservableProperty]
    public partial List<ToggleBoxViewModel> Items { get; set; }

    partial void OnToggleAllChanged(bool oldValue, bool newValue)
    {
        this.ProcessToggled();
    }

    partial void OnSwitchToggleChanged(bool value)
    {
        this.ProcessToggled();
    }

    private void ProcessToggled()
    {
        foreach (var item in Items)
        {
            item.ToggleOneIsVisible = !SwitchToggle;
            item.ToggleOne = !SwitchToggle ? ToggleAll : item.ToggleOne;
            item.ToggleTwo = SwitchToggle ? ToggleAll : item.ToggleTwo;
        }
    }
}

public partial class ToggleBoxViewModel : ObservableObject
{
    [ObservableProperty]
    public partial bool ToggleOneIsVisible { get; set; } = true;

    [ObservableProperty]
    public partial bool ToggleOne { get; set; }

    [ObservableProperty]
    public partial bool ToggleTwo { get; set; }
}
