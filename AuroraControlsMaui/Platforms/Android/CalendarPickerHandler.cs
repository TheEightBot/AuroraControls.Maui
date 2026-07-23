using Android.App;
using Microsoft.Maui;
using Microsoft.Maui.Handlers;

namespace AuroraControls;

public partial class CalendarPickerHandler : DatePickerHandler
{
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

    protected override DatePickerDialog CreateDatePickerDialog(int year, int month, int day)
    {
        var dialog = new DatePickerDialog(Context!, (_, e) =>
        {
            if (VirtualView is CalendarPicker datePicker)
            {
                datePicker.Date = e.Date;
            }
        }, year, month, day);

        return dialog;
    }

    public static void MapDate(CalendarPickerHandler handler, CalendarPicker view) => handler.TryShowEmptyState();

    public static void MapClearButtonVisibility(CalendarPickerHandler handler, CalendarPicker view)
    {
        // TODO Bucket E: draw the trailing clear drawable on the MauiDatePicker (AppCompatEditText),
        // toggle per ClearButtonVisibility, and call view.ClearValue() when the clear icon is tapped.
    }

    public static void MapFontSize(CalendarPickerHandler handler, CalendarPicker view)
    {
        if (handler.PlatformView != null)
        {
            ApplyFont(handler, view);
        }
    }

    public static void MapFontFamily(CalendarPickerHandler handler, CalendarPicker view)
    {
        if (handler.PlatformView != null)
        {
            ApplyFont(handler, view);
        }
    }

    public static void MapFontAttributes(CalendarPickerHandler handler, CalendarPicker view)
    {
        if (handler.PlatformView != null)
        {
            ApplyFont(handler, view);
        }
    }

    private static void ApplyFont(CalendarPickerHandler handler, CalendarPicker view)
    {
        handler.PlatformView.SetTextSize(Android.Util.ComplexUnitType.Sp, (float)view.FontSize);

        var typeface = GetTypeface(view);
        if (typeface != null)
        {
            handler.PlatformView.Typeface = typeface;
        }
    }

    private static Android.Graphics.Typeface? GetTypeface(CalendarPicker view)
    {
        var fontFamily = view.FontFamily;
        var fontAttributes = view.FontAttributes;

        if (string.IsNullOrEmpty(fontFamily))
        {
            return fontAttributes switch
            {
                FontAttributes.Bold => Android.Graphics.Typeface.DefaultBold,
                FontAttributes.Italic => Android.Graphics.Typeface.Create(Android.Graphics.Typeface.Default, Android.Graphics.TypefaceStyle.Italic),
                _ => Android.Graphics.Typeface.Default,
            };
        }

        var typeface = Android.Graphics.Typeface.Create(fontFamily, Android.Graphics.TypefaceStyle.Normal);
        if (fontAttributes == FontAttributes.Bold)
        {
            typeface = Android.Graphics.Typeface.Create(typeface, Android.Graphics.TypefaceStyle.Bold);
        }
        else if (fontAttributes == FontAttributes.Italic)
        {
            typeface = Android.Graphics.Typeface.Create(typeface, Android.Graphics.TypefaceStyle.Italic);
        }

        return typeface;
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
