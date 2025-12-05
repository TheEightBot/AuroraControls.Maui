// <copyright file="IThemeService.cs" company="Velocity Systems">
// Copyright (c) Velocity Systems. All rights reserved.
// </copyright>

#nullable enable

namespace AuroraControls.TestApp.Services;

/// <summary>
/// Service interface for managing application themes.
/// </summary>
public interface IThemeService
{
    /// <summary>
    /// Gets or sets the current app theme.
    /// </summary>
    AppTheme CurrentTheme { get; set; }

    /// <summary>
    /// Gets a value indicating whether the current theme is dark.
    /// </summary>
    bool IsDarkTheme { get; }

    /// <summary>
    /// Toggles between light and dark themes.
    /// </summary>
    void ToggleTheme();

    /// <summary>
    /// Sets the theme to a specific value.
    /// </summary>
    /// <param name="theme">The theme to set.</param>
    void SetTheme(AppTheme theme);

    /// <summary>
    /// Occurs when the theme changes.
    /// </summary>
    event EventHandler<AppTheme>? ThemeChanged;
}
