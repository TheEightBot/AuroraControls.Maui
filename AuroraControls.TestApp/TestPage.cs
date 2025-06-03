using System.Reactive.Disposables;
using AuroraControls.TestApp.ViewModels;
using CommunityToolkit.Maui.Markup;
using ReactiveUI;
using ReactiveUI.Maui;

namespace AuroraControls.TestApp;

public class TestPage : ReactiveContentPage<TestRxViewModel>
{
    private CalendarPicker _entry;

    private StyledInputLayout _sil;

    public TestPage()
    {
        ViewModel = new TestRxViewModel();

        Content =
            new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                Children =
                {
                    new CalendarPicker()
                    {
                    }
                        .Assign(out _entry),
                },
            };

        //// this.OneWayBind(ViewModel, x => x.NullableDecimalValue, x => x._label.Text);

        this.Bind(ViewModel, x => x.NullableDateTimeValue, x => x._entry.Date);
    }
}
