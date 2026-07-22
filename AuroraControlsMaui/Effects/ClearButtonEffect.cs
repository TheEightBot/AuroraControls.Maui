namespace AuroraControls.Effects;

/// <summary>
/// Routing effect that renders an Entry-style inline clear ("X") button on a picker-like input control
/// and governs its visibility via the <see cref="Microsoft.Maui.ClearButtonVisibility"/> contract.
/// Apply via the <c>ClearButton.ClearButtonVisibility</c> attached property, or add directly to
/// <see cref="Microsoft.Maui.Controls.Element.Effects"/>.
/// </summary>
/// <remarks>
/// The effect renders and toggles the clear affordance and empties the native text when tapped, but it
/// does not know what "cleared" means for an arbitrary control. Handle <see cref="ClearButtonClicked"/>
/// (or set <c>ClearButton.Command</c>) to reset the bound value.
/// </remarks>
public class ClearButtonEffect : RoutingEffect
{
    /// <summary>
    /// Raised when the user taps the clear button. The native text is emptied automatically;
    /// handle this (or use the <c>ClearButton.Command</c> attached property) to reset the bound value.
    /// </summary>
    public event EventHandler? ClearButtonClicked;

    /// <summary>Raises <see cref="ClearButtonClicked"/>. Invoked by the platform effect on clear.</summary>
    internal void RaiseClearButtonClicked() => ClearButtonClicked?.Invoke(this, EventArgs.Empty);
}
