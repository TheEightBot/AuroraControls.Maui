namespace AuroraControls.Effects;

/// <summary>
/// Rounded corners effect.
/// </summary>
public class RoundedCornersEffect : RoutingEffect
{
    public static readonly BindableProperty HasRoundedCornersProperty =
        BindableProperty.Create("HasRoundedCorners", typeof(bool), typeof(RoundedCornersEffect), default(bool),
            propertyChanged:
            (BindableObject bindable, object oldValue, object newValue) =>
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

    public static bool GetHasRoundedCorners(BindableObject view) => (bool)(view?.GetValue(CornerRadiusProperty) ?? default(bool));

    public static void SetHasRoundedCorners(BindableObject view, bool value) => view?.SetValue(CornerRadiusProperty, value);

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

    /// <summary>
    /// The border size property.
    /// </summary>
    public static readonly BindableProperty BorderSizeProperty =
        BindableProperty.CreateAttached("BorderSize", typeof(double), typeof(RoundedCornersEffect), 0d);

    /// <summary>
    /// Gets the border size.
    /// </summary>
    /// <returns>The border size.</returns>
    /// <param name="view">View to apply effect to.</param>
    public static double GetBorderSize(BindableObject view) => (double)(view?.GetValue(BorderSizeProperty) ?? default(double));

    /// <summary>
    /// Sets the border size.
    /// </summary>
    /// <param name="view">View to apply effect.</param>
    /// <param name="value">Amount of border size to apply.</param>
    public static void SetBorderSize(BindableObject view, double value) => view?.SetValue(BorderSizeProperty, value);

    /// <summary>
    /// The border color property.
    /// </summary>
    public static readonly BindableProperty BorderColorProperty =
        BindableProperty.CreateAttached("BorderSize", typeof(Color), typeof(RoundedCornersEffect), Colors.Transparent);

    /// <summary>
    /// Gets the border color.
    /// </summary>
    /// <returns>The border color.</returns>
    /// <param name="view">View to apply effect to.</param>
    public static Color GetBorderColor(BindableObject view) => (Color)(view?.GetValue(BorderColorProperty) ?? default(Color));

    /// <summary>
    /// Sets the border color.
    /// </summary>
    /// <param name="view">View to apply effect.</param>
    /// <param name="value">Border color to apply.</param>
    public static void SetBorderColor(BindableObject view, Color value) => view?.SetValue(BorderColorProperty, value);
}
