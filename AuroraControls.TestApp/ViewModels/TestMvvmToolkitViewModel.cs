using CommunityToolkit.Mvvm.ComponentModel;

namespace AuroraControls.TestApp.ViewModels;

public partial class TestMvvmToolkitViewModel : ObservableObject
{
    [ObservableProperty]
    private double _doubleValue;

    [ObservableProperty]
    private bool _isToggled = true;

    partial void OnIsToggledChanged(bool value)
    {
        Console.WriteLine($"IsToggled: {value}");
    }
}
