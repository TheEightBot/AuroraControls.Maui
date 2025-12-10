// <copyright file="AnimationExtensions.cs" company="TheEightBot">
// Copyright (c) TheEightBot. All rights reserved.
// </copyright>

namespace AuroraControls.TestApp;

/// <summary>
/// Extension methods for adding micro-animations and haptic feedback to UI elements.
/// </summary>
public static class AnimationExtensions
{
    /// <summary>
    /// Animates a press/tap effect on a view (scale down, then back up).
    /// </summary>
    /// <param name="view">The view to animate.</param>
    /// <param name="scale">The scale to shrink to (default 0.95).</param>
    /// <param name="duration">Duration in milliseconds (default 100).</param>
    /// <returns>A task representing the animation.</returns>
    public static async Task AnimatePressAsync(this View view, double scale = 0.95, uint duration = 100)
    {
        await view.ScaleTo(scale, duration / 2, Easing.CubicOut);
        await view.ScaleTo(1.0, duration / 2, Easing.CubicIn);
    }

    /// <summary>
    /// Animates a quick bounce effect on a view.
    /// </summary>
    /// <param name="view">The view to animate.</param>
    /// <returns>A task representing the animation.</returns>
    public static async Task AnimateBounceAsync(this View view)
    {
        await view.ScaleTo(1.1, 80, Easing.CubicOut);
        await view.ScaleTo(0.95, 60, Easing.CubicIn);
        await view.ScaleTo(1.0, 80, Easing.CubicOut);
    }

    /// <summary>
    /// Animates a fade-in effect with optional translation.
    /// </summary>
    /// <param name="view">The view to animate.</param>
    /// <param name="translateY">Optional Y translation to animate from.</param>
    /// <param name="duration">Duration in milliseconds.</param>
    /// <returns>A task representing the animation.</returns>
    public static async Task AnimateFadeInAsync(this View view, double translateY = 20, uint duration = 300)
    {
        view.Opacity = 0;
        view.TranslationY = translateY;

        await Task.WhenAll(
            view.FadeTo(1, duration, Easing.CubicOut),
            view.TranslateTo(0, 0, duration, Easing.CubicOut));
    }

    /// <summary>
    /// Animates a ripple/pulse effect around the center of a view.
    /// </summary>
    /// <param name="view">The view to animate.</param>
    /// <returns>A task representing the animation.</returns>
    public static async Task AnimatePulseAsync(this View view)
    {
        var originalOpacity = view.Opacity;
        await view.FadeTo(0.6, 100, Easing.CubicOut);
        await view.FadeTo(originalOpacity, 100, Easing.CubicIn);
    }

    /// <summary>
    /// Shakes a view horizontally to indicate an error.
    /// </summary>
    /// <param name="view">The view to shake.</param>
    /// <returns>A task representing the animation.</returns>
    public static async Task AnimateShakeAsync(this View view)
    {
        await view.TranslateTo(-10, 0, 50, Easing.Linear);
        await view.TranslateTo(10, 0, 50, Easing.Linear);
        await view.TranslateTo(-10, 0, 50, Easing.Linear);
        await view.TranslateTo(10, 0, 50, Easing.Linear);
        await view.TranslateTo(0, 0, 50, Easing.Linear);
    }

    /// <summary>
    /// Triggers haptic feedback if available on the platform.
    /// </summary>
    /// <param name="type">The type of haptic feedback.</param>
    public static void TriggerHaptic(HapticFeedbackType type = HapticFeedbackType.Click)
    {
        try
        {
            HapticFeedback.Default.Perform(type);
        }
        catch
        {
            // Haptics not available on this platform/device
        }
    }
}
