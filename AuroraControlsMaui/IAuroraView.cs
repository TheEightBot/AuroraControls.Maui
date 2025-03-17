namespace AuroraControls;

public interface IAuroraView
{
    public const double SmallControlHeight = 32d;
    public const double SmallControlWidth = 32d;
    public const double StandardControlHeight = 40d;

    public static readonly BindableProperty.BindingPropertyChangedDelegate PropertyChangedInvalidateSurface =
        (bindable, _, _) => (bindable as IAuroraView)?.InvalidateSurface();

    void InvalidateSurface();
}
