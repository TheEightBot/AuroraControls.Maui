using AuroraControls.Effects;

namespace AuroraControls.AttachedProperties;

/// <summary>
/// Attached properties for the Aurora keyboard accessory toolbar.
/// </summary>
/// <example>
/// XAML usage:
/// <code>
///   xmlns:aurora="clr-namespace:AuroraControls;assembly=AuroraControlsMaui"
///   ...
///   &lt;Entry aurora:KeyboardToolbar.Show="True"
///          aurora:KeyboardToolbar.Title="Close"
///          aurora:KeyboardToolbar.TitleColor="Blue" /&gt;
/// </code>
/// </example>
public static class KeyboardToolbar
{
    // ── Show ─────────────────────────────────────────────────────────────────

    /// <summary>
    /// When <see langword="true"/>, attaches a configurable keyboard accessory toolbar to the control.
    /// The toolbar style is governed by the other <see cref="KeyboardToolbar"/> attached properties
    /// and falls back to <see cref="KeyboardToolbarOptions.Default"/>.
    /// </summary>
    public static readonly BindableProperty ShowProperty =
        BindableProperty.Create(
            "Show",
            typeof(bool),
            typeof(KeyboardToolbar),
            false,
            propertyChanged: OnShowChanged);

    /// <summary>Gets whether the keyboard toolbar is shown for the given element.</summary>
    public static bool GetShow(BindableObject view) => (bool)view.GetValue(ShowProperty);

    /// <summary>Sets whether the keyboard toolbar is shown for the given element.</summary>
    public static void SetShow(BindableObject view, bool value) => view.SetValue(ShowProperty, value);

    // ── ButtonStyle ───────────────────────────────────────────────────────────

    /// <summary>
    /// The button style for the Done button.
    /// Default: <see cref="KeyboardToolbarDoneButtonStyle.Text"/>.
    /// </summary>
    public static readonly BindableProperty ButtonStyleProperty =
        BindableProperty.Create(
            "ButtonStyle",
            typeof(KeyboardToolbarDoneButtonStyle),
            typeof(KeyboardToolbar),
            KeyboardToolbarDoneButtonStyle.Text);

    /// <summary>Gets the Done button style for the given element.</summary>
    public static KeyboardToolbarDoneButtonStyle GetButtonStyle(BindableObject view) =>
        (KeyboardToolbarDoneButtonStyle)view.GetValue(ButtonStyleProperty);

    /// <summary>Sets the Done button style for the given element.</summary>
    public static void SetButtonStyle(BindableObject view, KeyboardToolbarDoneButtonStyle value) =>
        view.SetValue(ButtonStyleProperty, value);

    // ── Title ─────────────────────────────────────────────────────────────────

    /// <summary>
    /// The title text for the Done button (Text style only).
    /// <see langword="null"/> falls back to <see cref="KeyboardToolbarOptions.DefaultTitle"/>.
    /// </summary>
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(
            "Title",
            typeof(string),
            typeof(KeyboardToolbar),
            null);

    /// <summary>Gets the Done button title for the given element.</summary>
    public static string? GetTitle(BindableObject view) => (string?)view.GetValue(TitleProperty);

    /// <summary>Sets the Done button title for the given element.</summary>
    public static void SetTitle(BindableObject view, string? value) => view.SetValue(TitleProperty, value);

    // ── TitleColor ────────────────────────────────────────────────────────────

    /// <summary>
    /// The title color for the Done button (Text style only).
    /// <see langword="null"/> falls back to <see cref="KeyboardToolbarOptions.DefaultTitleColor"/>, then the system tint.
    /// </summary>
    public static readonly BindableProperty TitleColorProperty =
        BindableProperty.Create(
            "TitleColor",
            typeof(Color),
            typeof(KeyboardToolbar),
            null);

    /// <summary>Gets the Done button title color for the given element.</summary>
    public static Color? GetTitleColor(BindableObject view) => (Color?)view.GetValue(TitleColorProperty);

    /// <summary>Sets the Done button title color for the given element.</summary>
    public static void SetTitleColor(BindableObject view, Color? value) => view.SetValue(TitleColorProperty, value);

    // ── BackgroundColor ───────────────────────────────────────────────────────

    /// <summary>
    /// The background color for the accessory view.
    /// <see langword="null"/> falls back to <see cref="KeyboardToolbarOptions.DefaultBackgroundColor"/>, then a system material.
    /// </summary>
    public static readonly BindableProperty BackgroundColorProperty =
        BindableProperty.Create(
            "ToolbarBackgroundColor",
            typeof(Color),
            typeof(KeyboardToolbar),
            null);

    /// <summary>Gets the toolbar background color for the given element.</summary>
    public static Color? GetBackgroundColor(BindableObject view) => (Color?)view.GetValue(BackgroundColorProperty);

    /// <summary>Sets the toolbar background color for the given element.</summary>
    public static void SetBackgroundColor(BindableObject view, Color? value) => view.SetValue(BackgroundColorProperty, value);

    // ── Height ────────────────────────────────────────────────────────────────

    /// <summary>
    /// The explicit height of the accessory view in points.
    /// <c>0</c> means auto-size from content.
    /// Falls back to <see cref="KeyboardToolbarOptions.DefaultHeight"/>.
    /// </summary>
    public static readonly BindableProperty HeightProperty =
        BindableProperty.Create(
            "ToolbarHeight",
            typeof(double),
            typeof(KeyboardToolbar),
            0d);

    /// <summary>Gets the toolbar height for the given element.</summary>
    public static double GetHeight(BindableObject view) => (double)view.GetValue(HeightProperty);

    /// <summary>Sets the toolbar height for the given element.</summary>
    public static void SetHeight(BindableObject view, double value) => view.SetValue(HeightProperty, value);

    // ── FontFamily ────────────────────────────────────────────────────────────

    /// <summary>
    /// The font family for the Done button label (Text style only).
    /// <see langword="null"/> falls back to <see cref="KeyboardToolbarOptions.DefaultFontFamily"/>, then the system font.
    /// </summary>
    public static readonly BindableProperty FontFamilyProperty =
        BindableProperty.Create(
            "ToolbarFontFamily",
            typeof(string),
            typeof(KeyboardToolbar),
            null);

    /// <summary>Gets the toolbar button font family for the given element.</summary>
    public static string? GetFontFamily(BindableObject view) => (string?)view.GetValue(FontFamilyProperty);

    /// <summary>Sets the toolbar button font family for the given element.</summary>
    public static void SetFontFamily(BindableObject view, string? value) => view.SetValue(FontFamilyProperty, value);

    // ── FontSize ──────────────────────────────────────────────────────────────

    /// <summary>
    /// The font size for the Done button label (Text style only).
    /// <c>0</c> falls back to <see cref="KeyboardToolbarOptions.DefaultFontSize"/>, then the system default.
    /// </summary>
    public static readonly BindableProperty FontSizeProperty =
        BindableProperty.Create(
            "ToolbarFontSize",
            typeof(double),
            typeof(KeyboardToolbar),
            0d);

    /// <summary>Gets the toolbar button font size for the given element.</summary>
    public static double GetFontSize(BindableObject view) => (double)view.GetValue(FontSizeProperty);

    /// <summary>Sets the toolbar button font size for the given element.</summary>
    public static void SetFontSize(BindableObject view, double value) => view.SetValue(FontSizeProperty, value);

    // ── Property changed ──────────────────────────────────────────────────────
    private static void OnShowChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not VisualElement ve)
        {
            return;
        }

        var shouldShow = (bool)newValue;
        var existing = ve.Effects.FirstOrDefault(e => e is KeyboardToolbarEffect);

        if (existing != null && !shouldShow)
        {
            ve.Effects.Remove(existing);
        }

        if (shouldShow && existing is null)
        {
            ve.Effects.Add(new KeyboardToolbarEffect());
        }
    }
}
