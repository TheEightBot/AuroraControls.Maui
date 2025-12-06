// <copyright file="ExpandableSection.xaml.cs" company="TheEightBot">
// Copyright (c) TheEightBot. All rights reserved.
// </copyright>

namespace AuroraControls.TestApp.Controls;

/// <summary>
/// An expandable section control for grouping properties in demo pages.
/// Features a header with icon, title, and expand/collapse functionality.
/// </summary>
public partial class ExpandableSection : ContentView
{
    /// <summary>
    /// Bindable property for the section title.
    /// </summary>
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(
            nameof(Title),
            typeof(string),
            typeof(ExpandableSection),
            "Section");

    /// <summary>
    /// Bindable property for the section icon.
    /// </summary>
    public static readonly BindableProperty IconProperty =
        BindableProperty.Create(
            nameof(Icon),
            typeof(string),
            typeof(ExpandableSection),
            string.Empty);

    /// <summary>
    /// Bindable property for whether the section is expanded.
    /// </summary>
    public static readonly BindableProperty IsExpandedProperty =
        BindableProperty.Create(
            nameof(IsExpanded),
            typeof(bool),
            typeof(ExpandableSection),
            false,
            BindingMode.TwoWay,
            propertyChanged: OnIsExpandedChanged);

    /// <summary>
    /// Bindable property for the section content.
    /// </summary>
    public static readonly BindableProperty SectionContentProperty =
        BindableProperty.Create(
            nameof(SectionContent),
            typeof(View),
            typeof(ExpandableSection));

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpandableSection"/> class.
    /// </summary>
    public ExpandableSection()
    {
        InitializeComponent();
        UpdateExpandIndicator();
    }

    /// <summary>
    /// Event raised when the expanded state changes.
    /// </summary>
    public event EventHandler<bool>? ExpandedChanged;

    /// <summary>
    /// Gets or sets the section title.
    /// </summary>
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>
    /// Gets or sets the section icon (emoji or text).
    /// </summary>
    public string Icon
    {
        get => (string)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the section is expanded.
    /// </summary>
    public bool IsExpanded
    {
        get => (bool)GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }

    /// <summary>
    /// Gets or sets the content of the expandable section.
    /// </summary>
    public View? SectionContent
    {
        get => (View?)GetValue(SectionContentProperty);
        set => SetValue(SectionContentProperty, value);
    }

    private static void OnIsExpandedChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is ExpandableSection section && newValue is bool isExpanded)
        {
            section.UpdateExpandIndicator();
            section.AnimateExpansion(isExpanded);
            section.ExpandedChanged?.Invoke(section, isExpanded);
        }
    }

    private void OnHeaderTapped(object? sender, TappedEventArgs e)
    {
        AnimationExtensions.TriggerHaptic();
        IsExpanded = !IsExpanded;
    }

    private void UpdateExpandIndicator()
    {
        ExpandIndicator.Text = IsExpanded ? "▾" : "▸";
    }

    private async void AnimateExpansion(bool isExpanded)
    {
        // Rotate the expand indicator
        await ExpandIndicator.RotateTo(isExpanded ? 90 : 0, 150, Easing.CubicOut);

        // Animate content visibility with fade
        if (isExpanded)
        {
            ContentArea.Opacity = 0;
            ContentArea.IsVisible = true;
            await ContentArea.FadeTo(1, 150, Easing.CubicOut);
        }
        else
        {
            await ContentArea.FadeTo(0, 100, Easing.CubicIn);
            ContentArea.IsVisible = false;
            ContentArea.Opacity = 1;
        }
    }
}
