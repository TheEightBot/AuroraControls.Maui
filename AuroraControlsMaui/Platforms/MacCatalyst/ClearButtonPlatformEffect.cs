using Microsoft.Maui.Controls.Platform;

namespace AuroraControls;

/// <summary>
/// MacCatalyst platform effect backing <see cref="AuroraControls.Effects.ClearButtonEffect"/>.
/// </summary>
public class ClearButtonPlatformEffect : PlatformEffect
{
    // TODO Bucket D: share the iOS UITextField.ClearButtonMode + EditingChanged implementation.
    protected override void OnAttached()
    {
    }

    protected override void OnDetached()
    {
    }
}
