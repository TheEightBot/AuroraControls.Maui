namespace AuroraControls.Effects;

/// <summary>
/// Shadow effect.
/// </summary>
public class ShadowEffect : RoutingEffect
{
    public static readonly BindableProperty HasShadowProperty =
        BindableProperty.CreateAttached("HasShadow", typeof(bool), typeof(ShadowEffect), true,
            propertyChanged: OnHasShadowChanged);

    public static double GetHasShadow(BindableObject view)
    {
        return (double)view.GetValue(HasShadowProperty);
    }

    public static void SetHasShadow(BindableObject view, bool value)
    {
        view.SetValue(HasShadowProperty, value);
    }

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
    public static double GetElevation(BindableObject view)
    {
        return (double)view.GetValue(ElevationProperty);
    }

    /// <summary>
    /// Sets the elevation distance.
    /// </summary>
    /// <param name="view">View to set elevation on.</param>
    /// <param name="value">Amount of elevation.</param>
    public static void SetElevation(BindableObject view, double value)
    {
        view.SetValue(ElevationProperty, value);
    }

    /// <summary>
    /// The corner radius property.
    /// </summary>
    public static readonly BindableProperty CornerRadiusProperty =
        BindableProperty.CreateAttached("CornerRadius", typeof(double), typeof(ShadowEffect), 2d);

    /// <summary>
    /// Gets the corner radius.
    /// </summary>
    /// <returns>The corner radius.</returns>
    /// <param name="view">View to get corder radius from.</param>
    public static double GetCornerRadius(BindableObject view)
    {
        return (double)view.GetValue(CornerRadiusProperty);
    }

    /// <summary>
    /// Sets the corner radius.
    /// </summary>
    /// <param name="view">View to apply rounding to.</param>
    /// <param name="value">Amount of rounding to apply.</param>
    public static void SetCornerRadius(BindableObject view, double value)
    {
        view.SetValue(CornerRadiusProperty, value);
    }
}
