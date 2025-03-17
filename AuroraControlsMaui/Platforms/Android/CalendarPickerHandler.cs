using Android.App;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;

namespace AuroraControls;

public partial class CalendarPickerHandler : DatePickerHandler
{
    public static PropertyMapper<CalendarPicker, CalendarPickerHandler> NullableDatePickerPropertyMapper =
        new(DatePickerHandler.ViewMapper)
        {
            [nameof(CalendarPicker.Date)] = MapDate,
        };

    public CalendarPickerHandler()
        : base(NullableDatePickerPropertyMapper)
    {
    }

    public override void SetVirtualView(IView view)
    {
        base.SetVirtualView(view);
        TryShowEmptyState();
    }

    protected override DatePickerDialog CreateDatePickerDialog(int year, int month, int day)
    {
        var dialog = new DatePickerDialog(Context!, (o, e) =>
        {
            if (VirtualView is CalendarPicker datePicker)
            {
                datePicker.Date = e.Date;
            }
        }, year, month, day);

        return dialog;
    }

    public static void MapDate(CalendarPickerHandler handler, CalendarPicker view) => handler.TryShowEmptyState();

    public void TryShowEmptyState()
    {
        if (this.VirtualView is not CalendarPicker el)
        {
            return;
        }

        el.Dispatcher.Dispatch(
            () =>
            {
                this.PlatformView.Text =
                    el.Date.HasValue
                        ? el.Date.Value.ToString(el.Format)
                        : string.Empty;
            });
    }
}
