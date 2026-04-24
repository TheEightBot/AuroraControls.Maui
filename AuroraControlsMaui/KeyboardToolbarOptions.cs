namespace AuroraControls;

/// <summary>
/// Global configuration for the Aurora keyboard accessory toolbar.
/// Configure via <see cref="AuroraControlBuilder.UseAuroraControls{T}(Microsoft.Maui.Hosting.MauiAppBuilder, System.Action{KeyboardToolbarOptions}?)"/>.
/// </summary>
public sealed class KeyboardToolbarOptions
{
    private static readonly KeyboardToolbarOptions _default = new();

    /// <summary>Gets the global singleton instance.</summary>
    public static KeyboardToolbarOptions Default => _default;

    /// <summary>
    /// Gets or sets a value indicating whether Aurora suppresses MAUI's built-in Done accessory toolbar on every
    /// <see cref="Microsoft.Maui.Controls.Entry"/> and <see cref="Microsoft.Maui.Controls.Editor"/>.
    /// Individual controls can still opt-in via <c>KeyboardToolbar.Show="True"</c>.
    /// Default: <see langword="false"/> (preserve MAUI default).
    /// </summary>
    public bool IsGloballyHidden { get; set; }

    /// <summary>
    /// Gets or sets the default button style used when the per-control <c>KeyboardToolbar.ButtonStyle</c> is not set.
    /// Default: <see cref="KeyboardToolbarDoneButtonStyle.Text"/>.
    /// </summary>
    public KeyboardToolbarDoneButtonStyle DefaultButtonStyle { get; set; } = KeyboardToolbarDoneButtonStyle.Text;

    /// <summary>
    /// Gets or sets the default title text for the Done button (Text style only).
    /// Default: <c>"Done"</c>.
    /// </summary>
    public string DefaultTitle { get; set; } = "Done";

    /// <summary>
    /// Gets or sets the default title color for the Done button (Text style only).
    /// <see langword="null"/> falls back to the system tint color.
    /// </summary>
    public Color? DefaultTitleColor { get; set; }

    /// <summary>
    /// Gets or sets the default background color for the accessory view.
    /// <see langword="null"/> uses a translucent system material.
    /// </summary>
    public Color? DefaultBackgroundColor { get; set; }

    /// <summary>
    /// Gets or sets the default accessory view height in points.
    /// <c>0</c> means auto-size from content.
    /// </summary>
    public double DefaultHeight { get; set; }

    /// <summary>
    /// Gets or sets the default font family for the Done button label (Text style only).
    /// <see langword="null"/> uses the system font.
    /// </summary>
    public string? DefaultFontFamily { get; set; }

    /// <summary>
    /// Gets or sets the default font size for the Done button label (Text style only).
    /// <c>0</c> uses the system default.
    /// </summary>
    public double DefaultFontSize { get; set; }

    /// <inheritdoc cref="IsGloballyHidden"/>
    public KeyboardToolbarOptions WithIsGloballyHidden(bool value)
    {
        IsGloballyHidden = value;
        return this;
    }

    /// <inheritdoc cref="DefaultButtonStyle"/>
    public KeyboardToolbarOptions WithDefaultButtonStyle(KeyboardToolbarDoneButtonStyle value)
    {
        DefaultButtonStyle = value;
        return this;
    }

    /// <inheritdoc cref="DefaultTitle"/>
    public KeyboardToolbarOptions WithDefaultTitle(string value)
    {
        DefaultTitle = value;
        return this;
    }

    /// <inheritdoc cref="DefaultTitleColor"/>
    public KeyboardToolbarOptions WithDefaultTitleColor(Color? value)
    {
        DefaultTitleColor = value;
        return this;
    }

    /// <inheritdoc cref="DefaultBackgroundColor"/>
    public KeyboardToolbarOptions WithDefaultBackgroundColor(Color? value)
    {
        DefaultBackgroundColor = value;
        return this;
    }

    /// <inheritdoc cref="DefaultHeight"/>
    public KeyboardToolbarOptions WithDefaultHeight(double value)
    {
        DefaultHeight = value;
        return this;
    }

    /// <inheritdoc cref="DefaultFontFamily"/>
    public KeyboardToolbarOptions WithDefaultFontFamily(string? value)
    {
        DefaultFontFamily = value;
        return this;
    }

    /// <inheritdoc cref="DefaultFontSize"/>
    public KeyboardToolbarOptions WithDefaultFontSize(double value)
    {
        DefaultFontSize = value;
        return this;
    }
}
