namespace AuroraControls.VisualEffects;

public abstract class VisualEffect : BindableObject
{
    /// <summary>
    /// The enabled property.
    /// </summary>
    public static readonly BindableProperty EnabledProperty =
        BindableProperty.Create(nameof(Enabled), typeof(bool), typeof(VisualEffect), true);

    /// <summary>
    /// Gets or sets a value indicating whether gets or sets a value that determines whether the effect is enabled or not.
    /// </summary>
    /// <value><c>true</c> if enabled, otherwise <c>false</c>.</value>
    public bool Enabled
    {
        get => (bool)GetValue(EnabledProperty);
        set => SetValue(EnabledProperty, value);
    }

    public abstract SKImage ApplyEffect(SKImage image, SKSurface surface, SKImageInfo info, SKRect overrideRect);

    public abstract SKImage ApplyEffect(SKImage image, SKSurface surface, GRBackendRenderTarget info, SKRect overrideRect);
}
