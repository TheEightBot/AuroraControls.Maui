using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using UIKit;

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

    protected override void ConnectHandler(MauiDatePicker platformView)
    {
        base.ConnectHandler(platformView);

        if (platformView.InputView is UIDatePicker dp)
        {
            dp.PreferredDatePickerStyle = UIDatePickerStyle.Inline;
            dp.Mode = UIDatePickerMode.Date;
        }

        if (platformView.InputAccessoryView is UIToolbar tb)
        {
            var clearButton = new UIBarButtonItem("Clear", UIBarButtonItemStyle.Plain,
                (sender, e) =>
                {
                    if (this.VirtualView is not CalendarPicker el)
                    {
                        return;
                    }

                    el.Unfocus();
                    el.Date = null;
                });
            var items = new List<UIBarButtonItem>(tb.Items);
            items.Insert(0, clearButton);
            tb.Items = items.ToArray();
        }
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
                this.PlatformView.Text =
                    el.Date.HasValue
                        ? el.Date.Value.ToString(el.Format)
                        : string.Empty;
            });
    }
}
