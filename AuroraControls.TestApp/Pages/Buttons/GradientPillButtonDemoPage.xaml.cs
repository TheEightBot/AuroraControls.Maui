// Copyright (c) Aurora Controls. All rights reserved.

namespace AuroraControls.TestApp.Pages.Buttons;

/// <summary>
/// Demo page for GradientPillButton control with interactive property editors.
/// </summary>
public partial class GradientPillButtonDemoPage : ContentPage
{
    private readonly Color[] _colorPalette =
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

    private int _clickCount;
    private Color _startColor;
    private Color _endColor;

    /// <summary>
    /// Initializes a new instance of the <see cref="GradientPillButtonDemoPage"/> class.
    /// </summary>
    public GradientPillButtonDemoPage()
    {
        InitializeComponent();

        _startColor = Color.FromArgb("#7C3AED");
        _endColor = Color.FromArgb("#EC4899");

        InitializeColorPickers();
        DirectionPicker.SelectedIndex = 0;
        UpdateCodeExample();
    }

    private void InitializeColorPickers()
    {
        foreach (var color in _colorPalette)
        {
            var colorButton = CreateColorButton(color, isStartColor: true);
            if (color.ToArgbHex() == _startColor.ToArgbHex())
            {
                colorButton.Stroke = Colors.White;
            }

            StartColorPicker.Add(colorButton);
        }

        foreach (var color in _colorPalette)
        {
            var colorButton = CreateColorButton(color, isStartColor: false);
            if (color.ToArgbHex() == _endColor.ToArgbHex())
            {
                colorButton.Stroke = Colors.White;
            }

            EndColorPicker.Add(colorButton);
        }
    }

    private Border CreateColorButton(Color color, bool isStartColor)
    {
        var button = new Border
        {
            BackgroundColor = color,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 6 },
            Stroke = Colors.Transparent,
            StrokeThickness = 2,
            HeightRequest = 28,
            WidthRequest = 28,
        };

        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += (s, e) =>
        {
            var picker = isStartColor ? StartColorPicker : EndColorPicker;
            foreach (var child in picker.Children.OfType<Border>())
            {
                child.Stroke = Colors.Transparent;
            }

            button.Stroke = Colors.White;

            if (isStartColor)
            {
                _startColor = color;
                DemoButton.ButtonBackgroundStartColor = color;
            }
            else
            {
                _endColor = color;
                DemoButton.ButtonBackgroundEndColor = color;
            }

            UpdateCodeExample();
        };
        button.GestureRecognizers.Add(tapGesture);

        return button;
    }

    private void OnDemoButtonClicked(object sender, EventArgs e)
    {
        _clickCount++;
        ClickCountLabel.Text = $"Clicks: {_clickCount}";
    }

    private void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        DemoButton.Text = e.NewTextValue ?? string.Empty;
        UpdateCodeExample();
    }

    private void OnDirectionChanged(object sender, EventArgs e)
    {
        if (DirectionPicker.SelectedIndex >= 0)
        {
            DemoButton.GradientDirection = DirectionPicker.SelectedIndex switch
            {
                0 => GradientDirection.Horizontal,
                1 => GradientDirection.Vertical,
                _ => GradientDirection.Horizontal,
            };
            UpdateCodeExample();
        }
    }

    private void OnFontSizeChanged(object sender, ValueChangedEventArgs e)
    {
        var value = Math.Round(e.NewValue);
        FontSizeLabel.Text = value.ToString();
        DemoButton.FontSize = value;
        UpdateCodeExample();
    }

    private void OnBorderWidthChanged(object sender, ValueChangedEventArgs e)
    {
        var value = Math.Round(e.NewValue, 1);
        BorderWidthLabel.Text = value.ToString("F1");
        DemoButton.BorderWidth = value;
        UpdateCodeExample();
    }

    private void OnShadowBlurChanged(object sender, ValueChangedEventArgs e)
    {
        var value = Math.Round(e.NewValue);
        ShadowBlurLabel.Text = value.ToString();
        DemoButton.ShadowBlurRadius = value;
        UpdateCodeExample();
    }

    private void OnWidthChanged(object sender, ValueChangedEventArgs e)
    {
        var value = Math.Round(e.NewValue);
        WidthLabel.Text = value.ToString();
        DemoButton.WidthRequest = value;
        UpdateCodeExample();
    }

    private void OnHeightChanged(object sender, ValueChangedEventArgs e)
    {
        var value = Math.Round(e.NewValue);
        HeightLabel.Text = value.ToString();
        DemoButton.HeightRequest = value;
        UpdateCodeExample();
    }

    private void UpdateCodeExample()
    {
        var direction = DirectionPicker.SelectedIndex switch
        {
            0 => "Horizontal",
            1 => "Vertical",
            _ => "Horizontal",
        };

        CodeExampleLabel.Text = $@"<aurora:GradientPillButton
    Text=""{DemoButton.Text}""
    ButtonBackgroundStartColor=""{_startColor.ToArgbHex()}""
    ButtonBackgroundEndColor=""{_endColor.ToArgbHex()}""
    GradientDirection=""{direction}""
    FontSize=""{(int)DemoButton.FontSize}""
    BorderWidth=""{DemoButton.BorderWidth:F1}""
    ShadowBlurRadius=""{(int)DemoButton.ShadowBlurRadius}""
    WidthRequest=""{(int)DemoButton.WidthRequest}""
    HeightRequest=""{(int)DemoButton.HeightRequest}"" />";
    }
}
