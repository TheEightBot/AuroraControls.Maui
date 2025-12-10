// Copyright (c) Aurora Controls. All rights reserved.

#nullable enable

using Microsoft.Maui.Controls.Shapes;

namespace AuroraControls.TestApp.Pages.Base;

/// <summary>
/// Provides factory methods for creating property editor controls.
/// </summary>
public static class PropertyEditorFactory
{
    /// <summary>
    /// Creates a slider editor for numeric properties.
    /// </summary>
    /// <param name="label">The label text.</param>
    /// <param name="minimum">The minimum value.</param>
    /// <param name="maximum">The maximum value.</param>
    /// <param name="value">The initial value.</param>
    /// <param name="onValueChanged">Callback when value changes.</param>
    /// <param name="format">The number format string.</param>
    /// <returns>The slider editor view.</returns>
    public static View CreateSliderEditor(
        string label,
        double minimum,
        double maximum,
        double value,
        Action<double> onValueChanged,
        string? format = "F1")
    {
        var valueLabel = new Label
        {
            Text = value.ToString(format),
            TextColor = Color.FromArgb("#A0A0A0"),
            FontSize = 12,
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
        };

        var slider = new Slider
        {
            Minimum = minimum,
            Maximum = maximum,
            Value = value,
            MinimumTrackColor = Color.FromArgb("#7C3AED"),
            MaximumTrackColor = Color.FromArgb("#3A3A4E"),
            ThumbColor = Color.FromArgb("#7C3AED"),
        };

        slider.ValueChanged += (s, e) =>
        {
            valueLabel.Text = e.NewValue.ToString(format);
            onValueChanged(e.NewValue);
        };

        return new VerticalStackLayout
        {
            Spacing = 8,
            Children =
            {
                new Grid
                {
                    ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) },
                    Children =
                    {
                        new Label
                        {
                            Text = label,
                            TextColor = Colors.White,
                            FontSize = 14,
                        }.Column(0),
                        valueLabel.Column(1),
                    },
                },
                slider,
            },
        };
    }

    /// <summary>
    /// Creates a switch editor for boolean properties.
    /// </summary>
    /// <param name="label">The label text.</param>
    /// <param name="value">The initial value.</param>
    /// <param name="onValueChanged">Callback when value changes.</param>
    /// <returns>The switch editor view.</returns>
    public static View CreateSwitchEditor(
        string label,
        bool value,
        Action<bool> onValueChanged)
    {
        var toggle = new Switch
        {
            IsToggled = value,
            OnColor = Color.FromArgb("#7C3AED"),
            ThumbColor = Colors.White,
        };

        toggle.Toggled += (s, e) => onValueChanged(e.Value);

        return new Grid
        {
            ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) },
            Children =
            {
                new Label
                {
                    Text = label,
                    TextColor = Colors.White,
                    FontSize = 14,
                    VerticalOptions = LayoutOptions.Center,
                }.Column(0),
                toggle.Column(1),
            },
        };
    }

    /// <summary>
    /// Creates a color picker editor.
    /// </summary>
    /// <param name="label">The label text.</param>
    /// <param name="value">The initial color.</param>
    /// <param name="onValueChanged">Callback when color changes.</param>
    /// <param name="presetColors">Optional preset colors to show.</param>
    /// <returns>The color picker view.</returns>
    public static View CreateColorEditor(
        string label,
        Color value,
        Action<Color> onValueChanged,
        Color[]? presetColors = null)
    {
        presetColors ??=
        [
            Color.FromArgb("#7C3AED"),
            Color.FromArgb("#EC4899"),
            Color.FromArgb("#3B82F6"),
            Color.FromArgb("#14B8A6"),
            Color.FromArgb("#10B981"),
            Color.FromArgb("#F59E0B"),
            Color.FromArgb("#EF4444"),
            Colors.White,
        ];

        var colorGrid = new HorizontalStackLayout { Spacing = 8 };

        foreach (var color in presetColors)
        {
            var colorButton = new Border
            {
                BackgroundColor = color,
                StrokeShape = new RoundRectangle { CornerRadius = 6 },
                Stroke = color == value ? Colors.White : Colors.Transparent,
                StrokeThickness = 2,
                HeightRequest = 32,
                WidthRequest = 32,
            };

            colorButton.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() =>
                {
                    foreach (var child in colorGrid.Children.OfType<Border>())
                    {
                        child.Stroke = Colors.Transparent;
                    }

                    colorButton.Stroke = Colors.White;
                    onValueChanged(color);
                }),
            });

            colorGrid.Add(colorButton);
        }

        return new VerticalStackLayout
        {
            Spacing = 8,
            Children =
            {
                new Label
                {
                    Text = label,
                    TextColor = Colors.White,
                    FontSize = 14,
                },
                colorGrid,
            },
        };
    }

    /// <summary>
    /// Creates a picker editor for enum properties.
    /// </summary>
    /// <typeparam name="T">The enum type.</typeparam>
    /// <param name="label">The label text.</param>
    /// <param name="value">The initial value.</param>
    /// <param name="onValueChanged">Callback when value changes.</param>
    /// <returns>The picker view.</returns>
    public static View CreatePickerEditor<T>(
        string label,
        T value,
        Action<T> onValueChanged)
        where T : struct, Enum
    {
        var values = Enum.GetValues<T>().ToList();
        var picker = new Picker
        {
            ItemsSource = values.Select(v => v.ToString()).ToList(),
            SelectedIndex = values.IndexOf(value),
            TextColor = Colors.White,
            BackgroundColor = Color.FromArgb("#3A3A4E"),
        };

        picker.SelectedIndexChanged += (s, e) =>
        {
            if (picker.SelectedIndex >= 0 && picker.SelectedIndex < values.Count)
            {
                onValueChanged(values[picker.SelectedIndex]);
            }
        };

        return new VerticalStackLayout
        {
            Spacing = 8,
            Children =
            {
                new Label
                {
                    Text = label,
                    TextColor = Colors.White,
                    FontSize = 14,
                },
                picker,
            },
        };
    }

    /// <summary>
    /// Creates a text entry editor.
    /// </summary>
    /// <param name="label">The label text.</param>
    /// <param name="value">The initial value.</param>
    /// <param name="onValueChanged">Callback when value changes.</param>
    /// <param name="placeholder">Optional placeholder text.</param>
    /// <returns>The text entry view.</returns>
    public static View CreateTextEditor(
        string label,
        string value,
        Action<string> onValueChanged,
        string? placeholder = null)
    {
        var entry = new Entry
        {
            Text = value,
            Placeholder = placeholder ?? label,
            TextColor = Colors.White,
            PlaceholderColor = Color.FromArgb("#606060"),
            BackgroundColor = Color.FromArgb("#3A3A4E"),
        };

        entry.TextChanged += (s, e) => onValueChanged(e.NewTextValue ?? string.Empty);

        return new VerticalStackLayout
        {
            Spacing = 8,
            Children =
            {
                new Label
                {
                    Text = label,
                    TextColor = Colors.White,
                    FontSize = 14,
                },
                entry,
            },
        };
    }

    /// <summary>
    /// Creates a section header for grouping properties.
    /// </summary>
    /// <param name="title">The section title.</param>
    /// <returns>The section header view.</returns>
    public static View CreateSectionHeader(string title)
    {
        return new Label
        {
            Text = title,
            TextColor = Color.FromArgb("#7C3AED"),
            FontSize = 12,
            FontAttributes = FontAttributes.Bold,
            CharacterSpacing = 1.5,
            Margin = new Thickness(0, 16, 0, 8),
        };
    }

    /// <summary>
    /// Creates a divider line.
    /// </summary>
    /// <returns>The divider view.</returns>
    public static View CreateDivider()
    {
        return new BoxView
        {
            HeightRequest = 1,
            Color = Color.FromArgb("#3A3A4E"),
            Margin = new Thickness(0, 8),
        };
    }
}

/// <summary>
/// Extension methods for layout positioning.
/// </summary>
public static class LayoutExtensions
{
    /// <summary>
    /// Sets the column for a view in a grid.
    /// </summary>
    /// <typeparam name="T">The view type.</typeparam>
    /// <param name="view">The view.</param>
    /// <param name="column">The column index.</param>
    /// <returns>The view for chaining.</returns>
    public static T Column<T>(this T view, int column)
        where T : View
    {
        Grid.SetColumn(view, column);
        return view;
    }

    /// <summary>
    /// Sets the row for a view in a grid.
    /// </summary>
    /// <typeparam name="T">The view type.</typeparam>
    /// <param name="view">The view.</param>
    /// <param name="row">The row index.</param>
    /// <returns>The view for chaining.</returns>
    public static T Row<T>(this T view, int row)
        where T : View
    {
        Grid.SetRow(view, row);
        return view;
    }

    /// <summary>
    /// Sets the column span for a view in a grid.
    /// </summary>
    /// <typeparam name="T">The view type.</typeparam>
    /// <param name="view">The view.</param>
    /// <param name="span">The column span.</param>
    /// <returns>The view for chaining.</returns>
    public static T ColumnSpan<T>(this T view, int span)
        where T : View
    {
        Grid.SetColumnSpan(view, span);
        return view;
    }

    /// <summary>
    /// Sets the row span for a view in a grid.
    /// </summary>
    /// <typeparam name="T">The view type.</typeparam>
    /// <param name="view">The view.</param>
    /// <param name="span">The row span.</param>
    /// <returns>The view for chaining.</returns>
    public static T RowSpan<T>(this T view, int span)
        where T : View
    {
        Grid.SetRowSpan(view, span);
        return view;
    }
}
