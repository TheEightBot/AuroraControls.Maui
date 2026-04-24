using AuroraControls.Effects;

namespace AuroraControls.AttachedProperties;

[Obsolete("Use KeyboardToolbar attached properties instead. See docs/keyboard-toolbar.md for migration guidance.")]
public class KeyboardDoneButton
{
    [Obsolete("Use KeyboardToolbar.ShowProperty instead.")]
    public static BindableProperty ShowProperty =
        BindableProperty
            .Create(
                nameof(ShowProperty), typeof(bool), typeof(KeyboardDoneButton), default(bool),
                defaultBindingMode: BindingMode.Default, propertyChanged: OnKeyboardDoneButtonChanged);

    [Obsolete("Use KeyboardToolbar.GetShow() instead.")]
    public static bool GetShow(BindableObject view) => (bool)view.GetValue(ShowProperty);

    [Obsolete("Use KeyboardToolbar.SetShow() instead.")]
    public static void SetShow(BindableObject view, bool value) => view.SetValue(ShowProperty, value);

    private static void OnKeyboardDoneButtonChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not VisualElement ve)
        {
            return;
        }

        // Forward to the new KeyboardToolbar attached property for backward compat.
        KeyboardToolbar.SetShow(ve, (bool)newValue);
    }
}
