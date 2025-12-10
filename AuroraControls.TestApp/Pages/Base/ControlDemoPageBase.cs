// Copyright (c) Aurora Controls. All rights reserved.

#nullable enable

using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Controls.Shapes;

namespace AuroraControls.TestApp.Pages.Base;

/// <summary>
/// Base class for all control demo pages providing common functionality.
/// </summary>
public abstract class ControlDemoPageBase : ContentPage, INotifyPropertyChanged
{
    private bool _isPropertyPanelExpanded = true;

    /// <summary>
    /// Gets the title displayed in the header.
    /// </summary>
    public abstract string DemoTitle { get; }

    /// <summary>
    /// Gets a brief description of the control.
    /// </summary>
    public abstract string DemoDescription { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the property panel is expanded.
    /// </summary>
    public bool IsPropertyPanelExpanded
    {
        get => _isPropertyPanelExpanded;
        set => SetProperty(ref _isPropertyPanelExpanded, value);
    }

    /// <summary>
    /// Creates the control preview content.
    /// </summary>
    /// <returns>The preview view.</returns>
    protected abstract View CreateControlPreview();

    /// <summary>
    /// Creates the property editor panel content.
    /// </summary>
    /// <returns>The property editor view.</returns>
    protected abstract View CreatePropertyEditors();

    /// <summary>
    /// Creates the complete page layout with control preview and property editors.
    /// </summary>
    /// <returns>The layout view.</returns>
    protected View CreateDemoLayout()
    {
        var layout = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Star },
            },
            RowSpacing = 0,
        };

        layout.Add(CreateHeader(), 0, 0);
        layout.Add(CreateMainContent(), 0, 1);

        return layout;
    }

    /// <summary>
    /// Property changed event.
    /// </summary>
    public new event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Sets a property value and raises PropertyChanged if the value changed.
    /// </summary>
    /// <typeparam name="T">The property type.</typeparam>
    /// <param name="field">The backing field.</param>
    /// <param name="value">The new value.</param>
    /// <param name="propertyName">The property name.</param>
    /// <returns>True if the value changed.</returns>
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    /// <summary>
    /// Raises the PropertyChanged event.
    /// </summary>
    /// <param name="propertyName">The property name.</param>
    protected new void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private View CreateHeader()
    {
        var header = new Border
        {
            BackgroundColor = Color.FromArgb("#1E1E2E"),
            Padding = new Thickness(16),
            StrokeThickness = 0,
            Content = new VerticalStackLayout
            {
                Spacing = 4,
                Children =
                {
                    new Label
                    {
                        Text = DemoTitle,
                        FontSize = 24,
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Colors.White,
                    },
                    new Label
                    {
                        Text = DemoDescription,
                        FontSize = 14,
                        TextColor = Color.FromArgb("#A0A0A0"),
                    },
                },
            },
        };

        return header;
    }

    private View CreateMainContent()
    {
        var scrollView = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = new Thickness(16),
                Spacing = 16,
                Children =
                {
                    CreatePreviewSection(),
                    CreatePropertySection(),
                },
            },
        };

        return scrollView;
    }

    private View CreatePreviewSection()
    {
        var preview = new Border
        {
            BackgroundColor = Color.FromArgb("#2A2A3E"),
            StrokeShape = new RoundRectangle { CornerRadius = 16 },
            Stroke = Color.FromArgb("#3A3A4E"),
            StrokeThickness = 1,
            Padding = new Thickness(24),
            MinimumHeightRequest = 200,
            Content = new VerticalStackLayout
            {
                Spacing = 16,
                Children =
                {
                    new Label
                    {
                        Text = "Preview",
                        FontSize = 12,
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Color.FromArgb("#7C3AED"),
                        CharacterSpacing = 1.5,
                    },
                    new Border
                    {
                        BackgroundColor = Colors.Transparent,
                        Stroke = Colors.Transparent,
                        Padding = 0,
                        Content = CreateControlPreview(),
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                    },
                },
            },
        };

        return preview;
    }

    private View CreatePropertySection()
    {
        var toggleButton = new Button
        {
            Text = "▼ Properties",
            BackgroundColor = Colors.Transparent,
            TextColor = Color.FromArgb("#7C3AED"),
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Start,
            Padding = new Thickness(0),
        };

        var propertyContent = new VerticalStackLayout
        {
            Spacing = 12,
            IsVisible = true,
        };

        propertyContent.SetBinding(IsVisibleProperty, new Binding(nameof(IsPropertyPanelExpanded), source: this));

        toggleButton.Clicked += (s, e) =>
        {
            IsPropertyPanelExpanded = !IsPropertyPanelExpanded;
            toggleButton.Text = IsPropertyPanelExpanded ? "▼ Properties" : "▶ Properties";
        };

        propertyContent.Add(CreatePropertyEditors());

        var section = new Border
        {
            BackgroundColor = Color.FromArgb("#2A2A3E"),
            StrokeShape = new RoundRectangle { CornerRadius = 16 },
            Stroke = Color.FromArgb("#3A3A4E"),
            StrokeThickness = 1,
            Padding = new Thickness(16),
            Content = new VerticalStackLayout
            {
                Spacing = 12,
                Children =
                {
                    toggleButton,
                    propertyContent,
                },
            },
        };

        return section;
    }
}
