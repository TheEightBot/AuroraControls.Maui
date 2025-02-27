using CoreGraphics;
using Foundation;
using Microsoft.Maui.Controls.Platform;
using UIKit;
using Entry = Microsoft.Maui.Controls.Entry;

namespace AuroraControls.Maui;

public class NullableCalendarDatePickerPlatormEffect : PlatformEffect
{
    private UIViewController? _pickerViewController;
    private UINavigationController? _navigationController;
    private bool _isPopoverPresented;

    protected override void OnAttached()
    {
        if (this.Control is UITextField textField)
        {
            textField.EditingDidBegin += this.OnEditingDidBegin;
        }
    }

    private void OnEditingDidBegin(object sender, EventArgs e)
    {
        this.PresentPopoverPicker();
        if (this.Control is UITextField textField)
        {
            textField.ResignFirstResponder();
        }
    }

    private void PresentPopoverPicker()
    {
        if (this._isPopoverPresented)
        {
            return;
        }

        var datePicker = new UIDatePicker
        {
            Mode = UIDatePickerMode.Date,
            TimeZone = NSTimeZone.LocalTimeZone,
            PreferredDatePickerStyle = UIDatePickerStyle.Inline,
        };

        if (this.GetCurrentDate() is DateTime currentDate)
        {
            datePicker.Date = DateTimeToNSDate(currentDate);
        }

        var doneButton = new UIBarButtonItem(
            UIBarButtonSystemItem.Done,
            (s, e) =>
            {
                this.UpdateSelectedDate(datePicker);
                this.DismissPopover();
            });

        var clearButton = new UIBarButtonItem(
            "Clear",
            UIBarButtonItemStyle.Plain,
            (s, e) =>
            {
                this.ClearSelection();
                this.DismissPopover();
            });

        this._pickerViewController = new UIViewController
        {
            View = datePicker, PreferredContentSize = datePicker.SizeThatFits(CGSize.Empty)
        };

        this._pickerViewController.NavigationItem.RightBarButtonItems = [doneButton];
        this._pickerViewController.NavigationItem.LeftBarButtonItems = [clearButton];

        this._navigationController = new UINavigationController(this._pickerViewController)
        {
            ModalPresentationStyle = UIModalPresentationStyle.Popover,
        };

        var popover = this._navigationController.PopoverPresentationController;
        if (popover != null)
        {
            popover.SourceView = this.Control;
            popover.SourceRect = this.Control.Bounds;
            popover.PermittedArrowDirections = UIPopoverArrowDirection.Any;
            popover.Delegate = new PopoverDismissDelegate(() => this._isPopoverPresented = false);
        }

        var window = this.Control.Window ?? this.GetKeyWindow();
        window?.RootViewController?.PresentViewController(this._navigationController, true, null);

        this._isPopoverPresented = true;
    }

    private UIWindow? GetKeyWindow()
    {
        return UIApplication.SharedApplication
            .ConnectedScenes
            .OfType<UIWindowScene>()
            .SelectMany(s => s.Windows)
            .FirstOrDefault(w => w.IsKeyWindow);
    }

    private DateTime? GetCurrentDate()
    {
        if (this.Element is Entry entry && !string.IsNullOrEmpty(entry.Text))
        {
            return DateTime.TryParse(entry.Text, out var date) ? date : null;
        }

        return null;
    }

    private void UpdateSelectedDate(UIDatePicker datePicker)
    {
        if (this.Element is Entry entry)
        {
            entry.Text = NSDateToDateTime(datePicker.Date).ToString("d");
        }
    }

    private void ClearSelection()
    {
        if (this.Element is Entry entry)
        {
            entry.Text = string.Empty;
        }
    }

    private void DismissPopover()
    {
        if (!this._isPopoverPresented)
        {
            return;
        }

        this._navigationController?.DismissViewController(true, null);
        this._isPopoverPresented = false;

        this._pickerViewController?.Dispose();
        this._navigationController?.Dispose();
        this._pickerViewController = null;
        this._navigationController = null;
    }

    protected override void OnDetached()
    {
        if (this.Control is UITextField textField)
        {
            textField.EditingDidBegin -= this.OnEditingDidBegin;
        }

        this.DismissPopover();
    }

    private class PopoverDismissDelegate : UIPopoverPresentationControllerDelegate
    {
        private readonly Action _onDismiss;

        public PopoverDismissDelegate(Action onDismiss) => this._onDismiss = onDismiss;

        public override void DidDismissPopover(UIPopoverPresentationController controller) => this._onDismiss?.Invoke();
    }

    private static NSDate DateTimeToNSDate(DateTime date)
    {
        var reference = new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return NSDate.FromTimeIntervalSinceReferenceDate((date.ToUniversalTime() - reference).TotalSeconds);
    }

    private static DateTime NSDateToDateTime(NSDate date)
    {
        var reference = new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return reference.AddSeconds(date.SecondsSinceReferenceDate).ToLocalTime();
    }
}
