using Microsoft.Maui.Controls.Platform;

namespace AuroraControls;

/// <summary>
/// iOS platform effect backing <see cref="AuroraControls.Effects.ClearButtonEffect"/>.
/// </summary>
public class ClearButtonPlatformEffect : PlatformEffect
{
    // TODO Bucket C: map ClearButton.ClearButtonVisibility -> UITextField.ClearButtonMode,
    // intercept the native clear via EditingChanged, empty the value, then invoke the command
    // / raise ClearButtonClicked.
    protected override void OnAttached()
    {
    }

    protected override void OnDetached()
    {
    }
}
