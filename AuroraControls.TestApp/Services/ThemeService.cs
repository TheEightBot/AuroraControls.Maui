// <copyright file="ThemeService.cs" company="Velocity Systems">
// Copyright (c) Velocity Systems. All rights reserved.
// </copyright>

#nullable enable

namespace AuroraControls.TestApp.Services;

/// <summary>
/// Service for managing application themes.
/// </summary>
public class ThemeService : IThemeService
{
    private const string ThemePreferenceKey = "app_theme";

    /// <inheritdoc/>
    public event EventHandler<AppTheme>? ThemeChanged;

    /// <inheritdoc/>
    public AppTheme CurrentTheme
    {
        get => Application.Current?.RequestedTheme ?? AppTheme.Light;
        set => SetTheme(value);
    }

    /// <inheritdoc/>
    public bool IsDarkTheme => CurrentTheme == AppTheme.Dark;

    /// <inheritdoc/>
    public void ToggleTheme()
    {
        var newTheme = IsDarkTheme ? AppTheme.Light : AppTheme.Dark;
        SetTheme(newTheme);
    }

    /// <inheritdoc/>
    public void SetTheme(AppTheme theme)
    {
        if (Application.Current is null)
        {
            return;
        }

        Application.Current.UserAppTheme = theme;
        Preferences.Default.Set(ThemePreferenceKey, (int)theme);
        ThemeChanged?.Invoke(this, theme);
    }

    /// <summary>
    /// Loads the saved theme preference on app startup.
    /// </summary>
    public void LoadSavedTheme()
    {
        if (Application.Current is null)
        {
            return;
        }

        var savedTheme = Preferences.Default.Get(ThemePreferenceKey, (int)AppTheme.Unspecified);

        if (savedTheme != (int)AppTheme.Unspecified)
        {
            Application.Current.UserAppTheme = (AppTheme)savedTheme;
        }
    }
}
