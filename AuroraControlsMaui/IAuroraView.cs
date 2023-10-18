namespace AuroraControls;

public interface IAuroraView
{
    public const double StandardControlHeight = 40d;

    public static BindableProperty.BindingPropertyChangedDelegate PropertyChangedInvalidateSurface =
        (bindable, _, _) => (bindable as IAuroraView)?.InvalidateSurface();

    void InvalidateSurface();
}
