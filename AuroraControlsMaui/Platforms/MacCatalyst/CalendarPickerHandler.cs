using Foundation;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using UIKit;

namespace AuroraControls;

public partial class CalendarPickerHandler : DatePickerHandler
{
    /*
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

    public static void MapDate(CalendarPickerHandler handler, CalendarPicker view)
    {
        handler.TryShowEmptyState();
    }

    public void TryShowEmptyState()
    {
        if (this.VirtualView is not CalendarPicker el)
        {
            return;
        }

        el.Dispatcher.Dispatch(
            () =>
            {
                if (!el.Date.HasValue)
                {
                    VirtualView.Unfocus();
                }

                PlatformView.Alpha =
                    el.Date.HasValue
                        ? 1f
                        : 0f;
            });
    }
    */
}
