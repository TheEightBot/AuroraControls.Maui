using System.Windows.Input;
using AuroraControls.Effects;

namespace AuroraControls.AttachedProperties;

/// <summary>
/// Attached properties that add an Entry-style inline clear ("X") button to a picker-like input control,
/// reusing MAUI's <see cref="Microsoft.Maui.ClearButtonVisibility"/> contract.
/// </summary>
/// <example>
/// XAML usage:
/// <code>
///   xmlns:aurora="clr-namespace:AuroraControls.AttachedProperties;assembly=AuroraControls"
///   ...
///   &lt;DatePicker aurora:ClearButton.ClearButtonVisibility="WhileEditing"
///               aurora:ClearButton.Command="{Binding ClearDateCommand}" /&gt;
/// </code>
/// </example>
public static class ClearButton
{
    // ── ClearButtonVisibility ─────────────────────────────────────────────────

    /// <summary>
    /// Controls whether (and when) the inline clear button is shown, using the same
    /// <see cref="Microsoft.Maui.ClearButtonVisibility"/> semantics as <c>Entry.ClearButtonVisibility</c>.
    /// Setting anything other than <see cref="ClearButtonVisibility.Never"/> attaches the
    /// <see cref="ClearButtonEffect"/>; setting <see cref="ClearButtonVisibility.Never"/> removes it.
    /// </summary>
    public static readonly BindableProperty ClearButtonVisibilityProperty =
        BindableProperty.Create(
            "ClearButtonVisibility",
            typeof(ClearButtonVisibility),
            typeof(ClearButton),
            ClearButtonVisibility.Never,
            propertyChanged: OnClearButtonVisibilityChanged);

    /// <summary>Gets the clear-button visibility for the given element.</summary>
    public static ClearButtonVisibility GetClearButtonVisibility(BindableObject view) =>
        (ClearButtonVisibility)view.GetValue(ClearButtonVisibilityProperty);

    /// <summary>Sets the clear-button visibility for the given element.</summary>
    public static void SetClearButtonVisibility(BindableObject view, ClearButtonVisibility value) =>
        view.SetValue(ClearButtonVisibilityProperty, value);

    // ── Command ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Command invoked when the user taps the clear button. The native text is emptied automatically;
    /// use this to reset the bound value (e.g. set a nullable date/selection to null).
    /// </summary>
    public static readonly BindableProperty CommandProperty =
        BindableProperty.Create(
            "Command",
            typeof(ICommand),
            typeof(ClearButton),
            null);

    /// <summary>Gets the clear command for the given element.</summary>
    public static ICommand? GetCommand(BindableObject view) => (ICommand?)view.GetValue(CommandProperty);

    /// <summary>Sets the clear command for the given element.</summary>
    public static void SetCommand(BindableObject view, ICommand? value) => view.SetValue(CommandProperty, value);

    // ── CommandParameter ──────────────────────────────────────────────────────

    /// <summary>Parameter passed to <see cref="CommandProperty"/> when the clear button is tapped.</summary>
    public static readonly BindableProperty CommandParameterProperty =
        BindableProperty.Create(
            "CommandParameter",
            typeof(object),
            typeof(ClearButton),
            null);

    /// <summary>Gets the clear command parameter for the given element.</summary>
    public static object? GetCommandParameter(BindableObject view) => view.GetValue(CommandParameterProperty);

    /// <summary>Sets the clear command parameter for the given element.</summary>
    public static void SetCommandParameter(BindableObject view, object? value) =>
        view.SetValue(CommandParameterProperty, value);

    // ── Property changed ──────────────────────────────────────────────────────
    private static void OnClearButtonVisibilityChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not VisualElement ve)
        {
            return;
        }

        var visibility = (ClearButtonVisibility)newValue;
        var existing = ve.Effects.FirstOrDefault(e => e is ClearButtonEffect);

        if (visibility == ClearButtonVisibility.Never)
        {
            if (existing != null)
            {
                ve.Effects.Remove(existing);
            }

            return;
        }

        if (existing is null)
        {
            ve.Effects.Add(new ClearButtonEffect());
        }
    }
}
