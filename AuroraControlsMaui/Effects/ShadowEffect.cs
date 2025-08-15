namespace AuroraControls.Effects;

/// <summary>
/// Shadow effect.
/// </summary>
public class ShadowEffect : RoutingEffect
{
    public static readonly BindableProperty HasShadowProperty =
        BindableProperty.CreateAttached("HasShadow", typeof(bool), typeof(ShadowEffect), false,
            propertyChanged: OnHasShadowChanged);

    public static double GetHasShadow(BindableObject view) => (double)view.GetValue(HasShadowProperty);

    public static void SetHasShadow(BindableObject view, bool value) => view.SetValue(HasShadowProperty, value);

    private static void OnHasShadowChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not View view || newValue is not bool hasShadow)
        {
            return;
        }

        if (hasShadow)
        {
            view.Effects.Add(new ShadowEffect());
        }
        else
        {
            var toRemove = view.Effects.FirstOrDefault(e => e is ShadowEffect);
            if (toRemove != null)
            {
                view.Effects.Remove(toRemove);
            }
        }
    }

    /// <summary>
    /// The elevation distance property.
    /// </summary>
    public static readonly BindableProperty ElevationProperty =
        BindableProperty.CreateAttached("Elevation", typeof(double), typeof(ShadowEffect), 2d);

    /// <summary>
    /// Gets the elevation distance.
    /// </summary>
    /// <returns>The elevation distance.</returns>
    /// <param name="view">View.</param>
    public static double GetElevation(BindableObject view) => (double)view.GetValue(ElevationProperty);

    /// <summary>
    /// Sets the elevation distance.
    /// </summary>
    /// <param name="view">View to set elevation on.</param>
    /// <param name="value">Amount of elevation.</param>
    public static void SetElevation(BindableObject view, double value) => view.SetValue(ElevationProperty, value);

    /// <summary>
    /// The shadow color property.
    /// </summary>
    public static readonly BindableProperty ShadowColorProperty =
        BindableProperty.CreateAttached("ShadowColor", typeof(Color), typeof(ShadowEffect), Colors.Transparent);

    /// <summary>
    /// Gets the shadow color.
    /// </summary>
    /// <returns>The shadow color.</returns>
    /// <param name="view">View to get shadow color from.</param>
    public static Color GetShadowColor(BindableObject view) => (Color)view.GetValue(ShadowColorProperty);

    /// <summary>
    /// Sets the shadow color.
    /// </summary>
    /// <param name="view">View to apply shadow color to.</param>
    /// <param name="value">Shadow color to apply.</param>
    public static void SetShadowColor(BindableObject view, Color value) => view.SetValue(ShadowColorProperty, value);
}
