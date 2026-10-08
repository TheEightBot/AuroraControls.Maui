using Microsoft.Maui;
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
            [nameof(CalendarPicker.FontSize)] = MapFontSize,
            [nameof(CalendarPicker.FontFamily)] = MapFontFamily,
            [nameof(CalendarPicker.FontAttributes)] = MapFontAttributes,
            [nameof(CalendarPicker.ClearButtonVisibility)] = MapClearButtonVisibility,
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

    protected override MauiDatePicker CreatePlatformView() => new CalendarPickerField();

    protected override void ConnectHandler(MauiDatePicker platformView)
    {
        // Wrapped before base.ConnectHandler so MAUI does not set the date on the wheels picker;
        // the date is applied when the inline calendar is built on first edit.
        if (platformView.InputView is UIDatePicker dp)
        {
            var container = new UIView();
            container.BackgroundColor = UIColor.Clear;
            container.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;

            // Add ValueChanged event handler to update the Date when user selects from calendar
            dp.ValueChanged += OnDatePickerValueChanged;

            container.AddSubview(dp);

            platformView.InputView = container;
        }

        if (platformView is CalendarPickerField field)
        {
            field.WillBecomeFirstResponder = PrepareInlineCalendar;
        }

        base.ConnectHandler(platformView);

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

        // The native clear ("X") button empties the text; mirror that back to a null Date.
        platformView.EditingChanged += OnPlatformViewEditingChanged;
    }

    protected override void DisconnectHandler(MauiDatePicker platformView)
    {
        platformView.EditingChanged -= OnPlatformViewEditingChanged;

        if (platformView is CalendarPickerField field)
        {
            field.WillBecomeFirstResponder = null;
        }

        var dp = platformView.InputView as UIDatePicker ?? platformView.InputView?.Subviews.OfType<UIDatePicker>().FirstOrDefault();

        if (dp != null)
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

    public static void MapClearButtonVisibility(CalendarPickerHandler handler, CalendarPicker view)
    {
        if (handler.PlatformView is null)
        {
            return;
        }

        handler.PlatformView.ClearButtonMode =
            view.ClearButtonVisibility == ClearButtonVisibility.WhileEditing
                ? UITextFieldViewMode.WhileEditing
                : UITextFieldViewMode.Never;
    }

    public static void MapFontSize(CalendarPickerHandler handler, CalendarPicker view)
    {
        if (handler.PlatformView != null)
        {
            var font = GetUIFont(view);
            handler.PlatformView.Font = font;
        }
    }

    public static void MapFontFamily(CalendarPickerHandler handler, CalendarPicker view)
    {
        if (handler.PlatformView != null)
        {
            var font = GetUIFont(view);
            handler.PlatformView.Font = font;
        }
    }

    public static void MapFontAttributes(CalendarPickerHandler handler, CalendarPicker view)
    {
        if (handler.PlatformView != null)
        {
            var font = GetUIFont(view);
            handler.PlatformView.Font = font;
        }
    }

    private static UIKit.UIFont GetUIFont(CalendarPicker view)
    {
        var fontSize = (nfloat)view.FontSize;
        var fontFamily = view.FontFamily;
        var fontAttributes = view.FontAttributes;

        if (string.IsNullOrEmpty(fontFamily))
        {
            if (fontAttributes == FontAttributes.Bold)
            {
                return UIKit.UIFont.BoldSystemFontOfSize(fontSize);
            }
            else if (fontAttributes == FontAttributes.Italic)
            {
                return UIKit.UIFont.ItalicSystemFontOfSize(fontSize);
            }
            else
            {
                return UIKit.UIFont.SystemFontOfSize(fontSize);
            }
        }
        else
        {
            var font = UIKit.UIFont.FromName(fontFamily, fontSize);
            if (font != null)
            {
                if (fontAttributes == FontAttributes.Bold)
                {
                    var descriptor = font.FontDescriptor.CreateWithTraits(UIKit.UIFontDescriptorSymbolicTraits.Bold);
                    font = UIKit.UIFont.FromDescriptor(descriptor, fontSize);
                }
                else if (fontAttributes == FontAttributes.Italic)
                {
                    var descriptor = font.FontDescriptor.CreateWithTraits(UIKit.UIFontDescriptorSymbolicTraits.Italic);
                    font = UIKit.UIFont.FromDescriptor(descriptor, fontSize);
                }

                return font;
            }

            return UIKit.UIFont.SystemFontOfSize(fontSize);
        }
    }

    private void OnPlatformViewEditingChanged(object? sender, EventArgs e)
    {
        // The field is read-only (the date is chosen via the wheel), so an empty-text edit can only
        // originate from the native clear ("X") button. Programmatic text updates do not raise EditingChanged.
        if (this.VirtualView is CalendarPicker calendarPicker && string.IsNullOrEmpty(this.PlatformView?.Text))
        {
            calendarPicker.ClearValue();
        }
    }

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
            this.PlatformView?.InputView?.Subviews.FirstOrDefault() is UIDatePicker datePicker &&
            this.VirtualView is CalendarPicker calendarPicker &&
            calendarPicker.UpdateMode == CalendarPickerUpdateMode.WhenDone)
        {
            calendarPicker.Date = datePicker.Date.ToDateTime();

            this.TryShowEmptyState();
        }

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

    private void PrepareInlineCalendar()
    {
        if (this.PlatformView?.InputView is not { } container ||
            container.Subviews.OfType<UIDatePicker>().FirstOrDefault() is not { } dp ||
            dp.PreferredDatePickerStyle == UIDatePickerStyle.Inline)
        {
            return;
        }

        var bottomInset = SafeAreaInfo.GetSafeArea().Bottom;

        dp.PreferredDatePickerStyle = UIDatePickerStyle.Inline;
        dp.Mode = UIDatePickerMode.Date;
        dp.LayoutMargins = new UIEdgeInsets(0, 0, bottomInset, 0);

        if (this.VirtualView?.Date is DateTime date)
        {
            dp.Date = date.ToNSDate();
        }

        // The inline calendar only reports its real size once the style is set. Give the wrapper a fixed height
        // so UIKit sizes the input view to the calendar instead of stretching it to the keyboard height.
        var size = dp.SizeThatFits(new CoreGraphics.CGSize(10000, 10000));

        container.Frame = new CoreGraphics.CGRect(0, 0, size.Width, size.Height + bottomInset);

        dp.Frame = new CoreGraphics.CGRect(0, 0, size.Width, size.Height);
        dp.AutoresizingMask = UIViewAutoresizing.FlexibleLeftMargin | UIViewAutoresizing.FlexibleRightMargin;
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

    private sealed class CalendarPickerField : MauiDatePicker
    {
        public Action? WillBecomeFirstResponder { get; set; }

        public override bool BecomeFirstResponder()
        {
            WillBecomeFirstResponder?.Invoke();
            return base.BecomeFirstResponder();
        }
    }
}
