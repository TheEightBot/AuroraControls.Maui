using AuroraControls.Effects;

namespace AuroraControls.AttachedProperties;

public enum KeyboardReturnKeyType
{
    Default,
    Next,
    Go,
    Done,
    Send,
    Search,
}

public class KeyboardInput
{
    private const string
        ReturnKeyName = "ReturnKey",
        NextVisualElementName = "NextVisualElement";

    public static readonly BindableProperty NextVisualElementProperty =
        BindableProperty.CreateAttached(NextVisualElementName, typeof(VisualElement),
            typeof(Nullable), null, defaultBindingMode: BindingMode.Default,
            propertyChanged: OnPropertyChanged);

    public static readonly BindableProperty ReturnKeyProperty =
        BindableProperty.CreateAttached(ReturnKeyName, typeof(KeyboardReturnKeyType),
            typeof(Nullable), KeyboardReturnKeyType.Default, defaultBindingMode: BindingMode.Default, propertyChanged: OnPropertyChanged);

    public static VisualElement GetNextVisualElement(BindableObject view) => (VisualElement)view.GetValue(NextVisualElementProperty);

    public static void SetNextVisualElement(BindableObject view, VisualElement value) => view.SetValue(NextVisualElementProperty, value);

    public static KeyboardReturnKeyType GetReturnKey(BindableObject view) => (KeyboardReturnKeyType)view.GetValue(ReturnKeyProperty);

    public static void SetReturnKey(BindableObject view, KeyboardReturnKeyType value) => view.SetValue(ReturnKeyProperty, value);

    private static void OnPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var ve = bindable as VisualElement;
        if (ve == null)
        {
            return;
        }

        var foundEffect = ve.Effects.FirstOrDefault(x => x is KeyboardReturnKeyTypeNameEffect);

        if (foundEffect is not null)
        {
            return;
        }

        ve.Effects.Add(new KeyboardReturnKeyTypeNameEffect());
    }
}
