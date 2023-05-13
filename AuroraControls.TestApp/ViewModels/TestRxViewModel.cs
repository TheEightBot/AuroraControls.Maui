using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace AuroraControls.TestApp.ViewModels;

public class TestRxViewModel : ReactiveObject
{
    [Reactive]
    public double? NullableDoubleValue { get; set; }

    [Reactive]
    public int? NullableIntValue { get; set; }

    public TestRxViewModel()
    {
        this.WhenAnyValue(x => x.NullableDoubleValue)
            .Do(x => System.Diagnostics.Debug.WriteLine($"Nullable Double Value: {x}"))
            .Subscribe();

        this.WhenAnyValue(x => x.NullableIntValue)
            .Do(x => System.Diagnostics.Debug.WriteLine($"Nullable Int Value: {x}"))
            .Subscribe();
    }
}