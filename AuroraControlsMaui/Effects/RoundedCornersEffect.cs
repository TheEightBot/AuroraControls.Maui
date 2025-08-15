namespace AuroraControls.Effects;

/// <summary>
/// Rounded corners effect.
/// </summary>
public class RoundedCornersEffect : RoutingEffect
{
    public static readonly BindableProperty HasRoundedCornersProperty =
        BindableProperty.Create("HasRoundedCorners", typeof(bool), typeof(RoundedCornersEffect), false,
            propertyChanged:
            (BindableObject bindable, object _, object newValue) =>
            {
                if (bindable is not View view)
                {
                    return;
                }

                bool roundedCorners = (bool)newValue;
                if (roundedCorners)
                {
                    view.Effects.Add(new RoundedCornersEffect());
                }
                else
                {
                    var toRemove = view.Effects.FirstOrDefault(e => e is RoundedCornersEffect);
                    if (toRemove != null)
                    {
                        view.Effects.Remove(toRemove);
                    }
                }
            });

    public static bool GetHasRoundedCorners(BindableObject view) => (bool)(view?.GetValue(HasRoundedCornersProperty) ?? default(bool));

    public static void SetHasRoundedCorners(BindableObject view, bool value) => view?.SetValue(HasRoundedCornersProperty, value);

    /// <summary>
    /// The corner radius property.
    /// </summary>
    public static readonly BindableProperty CornerRadiusProperty =
        BindableProperty.CreateAttached("CornerRadius", typeof(double), typeof(RoundedCornersEffect), 2d);

    /// <summary>
    /// Gets the corner radius.
    /// </summary>
    /// <returns>The corner radius.</returns>
    /// <param name="view">View to apply effect to.</param>
    public static double GetCornerRadius(BindableObject view) => (double)(view?.GetValue(CornerRadiusProperty) ?? default(double));

    /// <summary>
    /// Sets the corner radius.
    /// </summary>
    /// <param name="view">View to apply effect.</param>
    /// <param name="value">Amount of rounding to apply.</param>
    public static void SetCornerRadius(BindableObject view, double value) => view?.SetValue(CornerRadiusProperty, value);
}
