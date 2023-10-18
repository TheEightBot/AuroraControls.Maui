using CommunityToolkit.Mvvm.ComponentModel;

namespace AuroraControls.TestApp.ViewModels;

public partial class TestMvvmToolkitViewModel : ObservableObject
{
    [ObservableProperty]
    private double _doubleValue;
}