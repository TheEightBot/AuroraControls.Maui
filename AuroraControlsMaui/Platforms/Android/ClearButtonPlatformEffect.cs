using Microsoft.Maui.Controls.Platform;

namespace AuroraControls;

/// <summary>
/// Android platform effect backing <see cref="AuroraControls.Effects.ClearButtonEffect"/>.
/// </summary>
public class ClearButtonPlatformEffect : PlatformEffect
{
    // TODO Bucket E: draw a trailing compound clear drawable on the AppCompatEditText, toggle it per
    // ClearButtonVisibility (WhileEditing => focused + has value), and intercept taps via a touch
    // listener to empty the value and invoke the command / raise ClearButtonClicked.
    protected override void OnAttached()
    {
    }

    protected override void OnDetached()
    {
    }
}
