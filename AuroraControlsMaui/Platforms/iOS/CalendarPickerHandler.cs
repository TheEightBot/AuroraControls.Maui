using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using UIKit;

namespace AuroraControls;

public partial class CalendarPickerHandler : DatePickerHandler, IDisposable
{
    private UIBarButtonItem? _clearButton;
    private UIBarButtonItem? _doneButton;

    public static PropertyMapper<CalendarPicker, CalendarPickerHandler> NullableDatePickerPropertyMapper =
        new(ViewMapper)
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
            // Wrap UIDatePicker in a UIView to workaround UpdateMode=Inline sizing issues when within InputView
            var container = new UIView();
            container.BackgroundColor = UIColor.Clear;
            container.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;
            container.Frame = new CoreGraphics.CGRect(0, 0, dp.Frame.Width, dp.Frame.Height);

            dp.Center = new CoreGraphics.CGPoint(container.Frame.Width / 2, container.Frame.Height / 2);
            dp.AutoresizingMask = UIViewAutoresizing.FlexibleMargins;
            dp.LayoutMargins = new UIEdgeInsets(0, 0, SafeAreaInfo.GetSafeArea().Bottom, 0);

            dp.PreferredDatePickerStyle = UIDatePickerStyle.Inline;
            dp.Mode = UIDatePickerMode.Date;

            // Add ValueChanged event handler to update the Date when user selects from calendar
            dp.ValueChanged += OnDatePickerValueChanged;

            container.AddSubview(dp);

            platformView.InputView = container;
        }

        if (platformView.InputAccessoryView is UIToolbar tb)
        {
            _clearButton = new UIBarButtonItem();
            _clearButton.Style = UIBarButtonItemStyle.Plain;
            _clearButton.Title = "Clear";
            _clearButton.Clicked += ClearButtonOnClicked;

            _doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done);
            _doneButton.Clicked += DoneButtonOnClicked;

            tb.Items = [_clearButton, new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace), _doneButton];
        }
    }

    protected override void DisconnectHandler(MauiDatePicker platformView)
    {
        if (platformView.InputView is UIDatePicker dp)
        {
            dp.ValueChanged -= OnDatePickerValueChanged;
        }

        if (_clearButton != null)
        {
            _clearButton.Clicked -= ClearButtonOnClicked;
        }

        if (_doneButton != null)
        {
            _doneButton.Clicked -= DoneButtonOnClicked;
        }

        base.DisconnectHandler(platformView);
    }

    public static void MapDate(CalendarPickerHandler handler, CalendarPicker view) => handler.TryShowEmptyState();

    private void OnDatePickerValueChanged(object sender, EventArgs e)
    {
        if (sender is UIDatePicker datePicker && this.VirtualView is CalendarPicker calendarPicker)
        {
            if (calendarPicker.UpdateMode == CalendarPickerUpdateMode.Immediately)
            {
                calendarPicker.Date = datePicker.Date.ToDateTime();

                TryShowEmptyState();
            }
        }
    }

    private void DoneButtonOnClicked(object? sender, EventArgs e)
    {
        if (
            this.PlatformView?.InputView?.Subviews.FirstOrDefault() is not UIDatePicker datePicker ||
            this.VirtualView is not CalendarPicker calendarPicker ||
            calendarPicker.UpdateMode != CalendarPickerUpdateMode.WhenDone)
        {
            return;
        }

        calendarPicker.Date = datePicker.Date.ToDateTime();

        this.TryShowEmptyState();

        this.PlatformView.ResignFirstResponder();
    }

    private void ClearButtonOnClicked(object? sender, EventArgs e)
    {
        if (this.VirtualView is not CalendarPicker el)
        {
            return;
        }

        el.Unfocus();
        el.Date = null;
    }

    private void TryShowEmptyState()
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

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        _clearButton?.Dispose();
        _doneButton?.Dispose();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~CalendarPickerHandler() => Dispose(false);
}
