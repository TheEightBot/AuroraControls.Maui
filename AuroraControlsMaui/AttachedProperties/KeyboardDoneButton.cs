using AuroraControls.Effects;

namespace AuroraControls.AttachedProperties;

public class KeyboardDoneButton
{
    public static BindableProperty ShowProperty =
        BindableProperty
            .Create(
                nameof(ShowProperty), typeof(bool), typeof(KeyboardDoneButton), default(bool),
                defaultBindingMode: BindingMode.Default, propertyChanged: OnKeyboardDoneButtonChanged);

    public static bool GetShow(BindableObject view) => (bool)view.GetValue(ShowProperty);

    public static void SetShow(BindableObject view, bool value) => view.SetValue(ShowProperty, value);

    private static void OnKeyboardDoneButtonChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not VisualElement ve)
        {
            return;
        }

        var shouldApply = (bool)newValue;

        var foundEffect = ve.Effects.FirstOrDefault(x => x is ShowKeyboardDoneButtonEffect);

        if (foundEffect != null && !shouldApply)
        {
            ve.Effects.Remove(foundEffect);
        }

        if (shouldApply && foundEffect is null)
        {
            ve.Effects.Add(new ShowKeyboardDoneButtonEffect());
        }
    }
}
