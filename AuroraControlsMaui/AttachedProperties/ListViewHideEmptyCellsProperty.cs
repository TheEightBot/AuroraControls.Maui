/*
 using AuroraControls.Effects;

namespace AuroraControls.AttachedProperties;

public class ListViewHideEmptyCellsProperty
{
    public const string HideEmptyCellsPropertyName = "HideEmptyCells";

    public static bool GetHideEmptyCells(BindableObject view)
    {
        return (bool)view.GetValue(HideEmptyCellsProperty);
    }

    public static void SetHideEmptyCells(BindableObject view, object value)
    {
        view.SetValue(HideEmptyCellsProperty, value);
    }

    public static BindableProperty HideEmptyCellsProperty =
        BindableProperty.CreateAttached(HideEmptyCellsPropertyName, typeof(bool),
            typeof(ListView), false, defaultBindingMode: BindingMode.Default, propertyChanged: OnEventNameChanged);

    private static void OnEventNameChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var listView = bindable as ListView;
        if (listView == null)
        {
            return;
        }

        var enable = (bool)newValue;

        var existingHideEmptyCellsEffect = listView.Effects.FirstOrDefault(x => x is ListViewHideEmptyCellsEffect);

        if (enable && existingHideEmptyCellsEffect == null)
        {
            listView.Effects.Add(new ListViewHideEmptyCellsEffect());
        }
        else if (existingHideEmptyCellsEffect != null)
        {
            listView.Effects.Remove(existingHideEmptyCellsEffect);
        }
    }
}
*/
