using System.Reactive;
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

    [Reactive]
    public DateTime? NullableDateTimeValue { get; set; } = DateTime.Now;

    [Reactive]
    public ReactiveCommand<Unit, Unit> ResetValues { get; private set; }

    [Reactive]
    public bool IsToggled { get; set; }

    public TestRxViewModel()
    {
        ResetValues =
            ReactiveCommand
                .Create(
                    () =>
                    {
                        NullableDoubleValue = null;
                        NullableIntValue = null;
                        NullableDateTimeValue = null;
                    });

        this.WhenAnyValue(x => x.NullableDoubleValue)
            .Do(x => System.Diagnostics.Debug.WriteLine($"Nullable Double Value: {x}"))
            .Subscribe();

        this.WhenAnyValue(x => x.NullableIntValue)
            .Do(x => System.Diagnostics.Debug.WriteLine($"Nullable Int Value: {x}"))
            .Subscribe();

        this.WhenAnyValue(x => x.NullableDateTimeValue)
            .Do(x => System.Diagnostics.Debug.WriteLine($"Nullable Date Time Value: {x}"))
            .Subscribe();

        this.WhenAnyValue(x => x.IsToggled)
            .Do(x => System.Diagnostics.Debug.WriteLine($"Is Toggled: {x}"))
            .Subscribe();
    }
}
