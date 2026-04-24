// <copyright file="NumericFormatting.cs" company="TheEightBot">
// Copyright (c) TheEightBot. All rights reserved.
// </copyright>

using System.Globalization;

namespace AuroraControls.Converters;

/// <summary>
/// Attached properties for applying numeric formatting to Entry controls.
/// Provides a declarative way to configure numeric formatting directly in XAML.
/// </summary>
/// <example>
/// XAML usage:
/// <code>
/// &lt;!-- Apply currency formatting --&gt;
/// &lt;Entry Text="{Binding Price}"
///        aurora:NumericFormatting.Format="C2"
///        aurora:NumericFormatting.FormatOnUnfocus="True" /&gt;
///
/// &lt;!-- Apply percent formatting with constraints --&gt;
/// &lt;Entry Text="{Binding Discount}"
///        aurora:NumericFormatting.Format="P0"
///        aurora:NumericFormatting.MinValue="0"
///        aurora:NumericFormatting.MaxValue="100" /&gt;
/// </code>
/// </example>
public static class NumericFormatting
{
    /// <summary>
    /// Attached property for the format string.
    /// </summary>
    public static readonly BindableProperty FormatProperty =
        BindableProperty.CreateAttached(
            "Format",
            typeof(string),
            typeof(NumericFormatting),
            null,
            propertyChanged: OnFormatPropertyChanged);

    /// <summary>
    /// Attached property for enabling format on unfocus.
    /// </summary>
    public static readonly BindableProperty FormatOnUnfocusProperty =
        BindableProperty.CreateAttached(
            "FormatOnUnfocus",
            typeof(bool),
            typeof(NumericFormatting),
            true,
            propertyChanged: OnPropertyChanged);

    /// <summary>
    /// Attached property for the culture.
    /// </summary>
    public static readonly BindableProperty CultureProperty =
        BindableProperty.CreateAttached(
            "Culture",
            typeof(CultureInfo),
            typeof(NumericFormatting),
            null,
            propertyChanged: OnPropertyChanged);

    /// <summary>
    /// Attached property for the minimum value.
    /// </summary>
    public static readonly BindableProperty MinValueProperty =
        BindableProperty.CreateAttached(
            "MinValue",
            typeof(double?),
            typeof(NumericFormatting),
            null,
            propertyChanged: OnPropertyChanged);

    /// <summary>
    /// Attached property for the maximum value.
    /// </summary>
    public static readonly BindableProperty MaxValueProperty =
        BindableProperty.CreateAttached(
            "MaxValue",
            typeof(double?),
            typeof(NumericFormatting),
            null,
            propertyChanged: OnPropertyChanged);

    /// <summary>
    /// Attached property for allowing negative values.
    /// </summary>
    public static readonly BindableProperty AllowNegativeProperty =
        BindableProperty.CreateAttached(
            "AllowNegative",
            typeof(bool),
            typeof(NumericFormatting),
            true,
            propertyChanged: OnPropertyChanged);

    /// <summary>
    /// Attached property for allowing decimal values.
    /// </summary>
    public static readonly BindableProperty AllowDecimalProperty =
        BindableProperty.CreateAttached(
            "AllowDecimal",
            typeof(bool),
            typeof(NumericFormatting),
            true,
            propertyChanged: OnPropertyChanged);

    // Internal property to store the behavior instance
    private static readonly BindableProperty BehaviorProperty =
        BindableProperty.CreateAttached(
            "Behavior",
            typeof(NumericEntryBehavior),
            typeof(NumericFormatting),
            null);

    /// <summary>
    /// Gets the format string for the specified element.
    /// </summary>
    public static string? GetFormat(BindableObject element)
    {
        return (string?)element.GetValue(FormatProperty);
    }

    /// <summary>
    /// Sets the format string for the specified element.
    /// </summary>
    public static void SetFormat(BindableObject element, string? value)
    {
        element.SetValue(FormatProperty, value);
    }

    /// <summary>
    /// Gets the format on unfocus setting for the specified element.
    /// </summary>
    public static bool GetFormatOnUnfocus(BindableObject element)
    {
        return (bool)element.GetValue(FormatOnUnfocusProperty);
    }

    /// <summary>
    /// Sets the format on unfocus setting for the specified element.
    /// </summary>
    public static void SetFormatOnUnfocus(BindableObject element, bool value)
    {
        element.SetValue(FormatOnUnfocusProperty, value);
    }

    /// <summary>
    /// Gets the culture for the specified element.
    /// </summary>
    public static CultureInfo? GetCulture(BindableObject element)
    {
        return (CultureInfo?)element.GetValue(CultureProperty);
    }

    /// <summary>
    /// Sets the culture for the specified element.
    /// </summary>
    public static void SetCulture(BindableObject element, CultureInfo? value)
    {
        element.SetValue(CultureProperty, value);
    }

    /// <summary>
    /// Gets the minimum value for the specified element.
    /// </summary>
    public static double? GetMinValue(BindableObject element)
    {
        return (double?)element.GetValue(MinValueProperty);
    }

    /// <summary>
    /// Sets the minimum value for the specified element.
    /// </summary>
    public static void SetMinValue(BindableObject element, double? value)
    {
        element.SetValue(MinValueProperty, value);
    }

    /// <summary>
    /// Gets the maximum value for the specified element.
    /// </summary>
    public static double? GetMaxValue(BindableObject element)
    {
        return (double?)element.GetValue(MaxValueProperty);
    }

    /// <summary>
    /// Sets the maximum value for the specified element.
    /// </summary>
    public static void SetMaxValue(BindableObject element, double? value)
    {
        element.SetValue(MaxValueProperty, value);
    }

    /// <summary>
    /// Gets the allow negative setting for the specified element.
    /// </summary>
    public static bool GetAllowNegative(BindableObject element)
    {
        return (bool)element.GetValue(AllowNegativeProperty);
    }

    /// <summary>
    /// Sets the allow negative setting for the specified element.
    /// </summary>
    public static void SetAllowNegative(BindableObject element, bool value)
    {
        element.SetValue(AllowNegativeProperty, value);
    }

    /// <summary>
    /// Gets the allow decimal setting for the specified element.
    /// </summary>
    public static bool GetAllowDecimal(BindableObject element)
    {
        return (bool)element.GetValue(AllowDecimalProperty);
    }

    /// <summary>
    /// Sets the allow decimal setting for the specified element.
    /// </summary>
    public static void SetAllowDecimal(BindableObject element, bool value)
    {
        element.SetValue(AllowDecimalProperty, value);
    }

    private static void OnFormatPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not Entry entry)
        {
            return;
        }

        var format = newValue as string;

        if (string.IsNullOrEmpty(format))
        {
            // Remove behavior if format is cleared
            RemoveBehavior(entry);
        }
        else
        {
            // Add or update behavior
            EnsureBehavior(entry);
        }
    }

    private static void OnPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not Entry entry)
        {
            return;
        }

        // Update the behavior if it exists
        var behavior = GetBehavior(entry);
        if (behavior is not null)
        {
            UpdateBehavior(entry, behavior);
        }
    }

    private static void EnsureBehavior(Entry entry)
    {
        var behavior = GetBehavior(entry);

        if (behavior is null)
        {
            behavior = new NumericEntryBehavior();
            entry.Behaviors.Add(behavior);
            SetBehavior(entry, behavior);
        }

        UpdateBehavior(entry, behavior);
    }

    private static void RemoveBehavior(Entry entry)
    {
        var behavior = GetBehavior(entry);

        if (behavior is not null)
        {
            entry.Behaviors.Remove(behavior);
            SetBehavior(entry, null);
        }
    }

    private static void UpdateBehavior(Entry entry, NumericEntryBehavior behavior)
    {
        behavior.Format = GetFormat(entry) ?? "N2";
        behavior.FormatOnUnfocus = GetFormatOnUnfocus(entry);
        behavior.Culture = GetCulture(entry);
        behavior.MinValue = GetMinValue(entry);
        behavior.MaxValue = GetMaxValue(entry);
        behavior.AllowNegative = GetAllowNegative(entry);
        behavior.AllowDecimal = GetAllowDecimal(entry);
    }

    private static NumericEntryBehavior? GetBehavior(BindableObject element)
    {
        return (NumericEntryBehavior?)element.GetValue(BehaviorProperty);
    }

    private static void SetBehavior(BindableObject element, NumericEntryBehavior? value)
    {
        element.SetValue(BehaviorProperty, value);
    }
}