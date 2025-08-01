using CommunityToolkit.Maui.Markup;

namespace AuroraControls.TestApp;

public partial class WrapLayoutTestPage : ContentPage
{
    private WrapLayout _demoWrapLayout;
    private Picker _orientationPicker;
    private Slider _horizontalSpacingSlider;
    private Slider _verticalSpacingSlider;
    private Picker _horizontalOptionsPicker;
    private Picker _verticalOptionsPicker;
    private Label _horizontalSpacingLabel;
    private Label _verticalSpacingLabel;
    private Button _addItemButton;
    private Button _removeItemButton;
    private Button _clearItemsButton;
    private Frame _demoFrame;
    private int _itemCounter = 1;

    public WrapLayoutTestPage()
    {
        Title = "WrapLayout Test";

        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Spacing = 20,
                Padding = 20,
                Children =
                {
                    new Label
                    {
                        Text = "WrapLayout Interactive Demo",
                        FontSize = 24,
                        FontAttributes = FontAttributes.Bold,
                        HorizontalOptions = LayoutOptions.Center,
                    },

                    // Configuration Panel
                    new Frame
                    {
                        BackgroundColor = Colors.LightGray,
                        Padding = 15,
                        Content = new VerticalStackLayout
                        {
                            Spacing = 15,
                            Children =
                            {
                                new Label { Text = "Configuration", FontSize = 18, FontAttributes = FontAttributes.Bold },

                                // Orientation
                                new HorizontalStackLayout
                                {
                                    Spacing = 10,
                                    Children =
                                    {
                                        new Label { Text = "Orientation:", WidthRequest = 120, VerticalOptions = LayoutOptions.Center },
                                        new Picker
                                        {
                                            ItemsSource = new[] { "Horizontal", "Vertical" },
                                            SelectedIndex = 0,
                                            WidthRequest = 150,
                                        }.Assign(out _orientationPicker),
                                    },
                                },

                                // Horizontal Spacing
                                new VerticalStackLayout
                                {
                                    Children =
                                    {
                                        new Label { Text = "Horizontal Spacing: 10" }.Assign(out _horizontalSpacingLabel),
                                        new Slider
                                        {
                                            Minimum = 0,
                                            Maximum = 50,
                                            Value = 10,
                                        }.Assign(out _horizontalSpacingSlider),
                                    },
                                },

                                // Vertical Spacing
                                new VerticalStackLayout
                                {
                                    Children =
                                    {
                                        new Label { Text = "Vertical Spacing: 10" }.Assign(out _verticalSpacingLabel),
                                        new Slider
                                        {
                                            Minimum = 0,
                                            Maximum = 50,
                                            Value = 10,
                                        }.Assign(out _verticalSpacingSlider),
                                    },
                                },

                                // Horizontal Options
                                new HorizontalStackLayout
                                {
                                    Spacing = 10,
                                    Children =
                                    {
                                        new Label { Text = "H-Alignment:", WidthRequest = 120, VerticalOptions = LayoutOptions.Center },
                                        new Picker
                                        {
                                            ItemsSource = new[] { "Start", "Center", "End", "Fill", "StartAndExpand", "CenterAndExpand", "EndAndExpand", "FillAndExpand" },
                                            SelectedIndex = 0,
                                            WidthRequest = 150,
                                        }.Assign(out _horizontalOptionsPicker),
                                    },
                                },

                                // Vertical Options
                                new HorizontalStackLayout
                                {
                                    Spacing = 10,
                                    Children =
                                    {
                                        new Label { Text = "V-Alignment:", WidthRequest = 120, VerticalOptions = LayoutOptions.Center },
                                        new Picker
                                        {
                                            ItemsSource = new[] { "Start", "Center", "End", "Fill", "StartAndExpand", "CenterAndExpand", "EndAndExpand", "FillAndExpand" },
                                            SelectedIndex = 0,
                                            WidthRequest = 150,
                                        }.Assign(out _verticalOptionsPicker),
                                    },
                                },

                                // Item Management
                                new HorizontalStackLayout
                                {
                                    Spacing = 10,
                                    Children =
                                    {
                                        new Button { Text = "Add Item" }.Assign(out _addItemButton),
                                        new Button { Text = "Remove Item" }.Assign(out _removeItemButton),
                                        new Button { Text = "Clear All" }.Assign(out _clearItemsButton),
                                    },
                                },
                            },
                        },
                    },

                    // Demo Container
                    new Label { Text = "Live Demo", FontSize = 18, FontAttributes = FontAttributes.Bold },
                    new Frame
                    {
                        BackgroundColor = Colors.White,
                        BorderColor = Colors.Gray,
                        HeightRequest = 300,
                        Padding = 10,
                    }.Assign(out _demoFrame),

                    // Static Examples
                    new Label { Text = "Static Examples", FontSize = 18, FontAttributes = FontAttributes.Bold },

                    // Horizontal Example
                    new Label { Text = "Horizontal Wrap (Buttons)", FontSize = 14, FontAttributes = FontAttributes.Bold },
                    new Frame
                    {
                        BackgroundColor = Colors.LightBlue,
                        Padding = 10,
                        Content = new WrapLayout
                        {
                            Orientation = StackOrientation.Horizontal,
                            HorizontalSpacing = 5,
                            VerticalSpacing = 5,
                            Children =
                            {
                                CreateButton("Short", Colors.Red),
                                CreateButton("Medium Button", Colors.Blue),
                                CreateButton("Very Long Button Text", Colors.Green),
                                CreateButton("Btn", Colors.Orange),
                                CreateButton("Another Long One", Colors.Purple),
                                CreateButton("Small", Colors.Teal),
                                CreateButton("Test", Colors.Pink),
                            },
                        },
                    },

                    // Vertical Example
                    new Label { Text = "Vertical Wrap (Labels)", FontSize = 14, FontAttributes = FontAttributes.Bold },
                    new Frame
                    {
                        BackgroundColor = Colors.LightGreen,
                        HeightRequest = 200,
                        Padding = 10,
                        Content = new WrapLayout
                        {
                            Orientation = StackOrientation.Vertical,
                            HorizontalSpacing = 8,
                            VerticalSpacing = 4,
                            Children =
                            {
                                CreateLabel("Label 1"),
                                CreateLabel("Label 2"),
                                CreateLabel("Label 3"),
                                CreateLabel("Label 4"),
                                CreateLabel("Label 5"),
                                CreateLabel("Label 6"),
                                CreateLabel("Label 7"),
                                CreateLabel("Label 8"),
                                CreateLabel("Label 9"),
                                CreateLabel("Label 10"),
                            },
                        },
                    },

                    // Mixed Content Example
                    new Label { Text = "Mixed Content Types", FontSize = 14, FontAttributes = FontAttributes.Bold },
                    new Frame
                    {
                        BackgroundColor = Colors.LightYellow,
                        Padding = 10,
                        Content = new WrapLayout
                        {
                            Orientation = StackOrientation.Horizontal,
                            HorizontalSpacing = 10,
                            VerticalSpacing = 10,
                            Children =
                            {
                                new Entry { Placeholder = "Name", WidthRequest = 120 },
                                new Entry { Placeholder = "Email", WidthRequest = 150 },
                                new Button { Text = "Submit", BackgroundColor = Colors.Green, TextColor = Colors.White },
                                new CheckBox(),
                                new Label { Text = "I agree", VerticalOptions = LayoutOptions.Center },
                                new Slider { WidthRequest = 100, Maximum = 100, Value = 50 },
                                new Label { Text = "Volume", VerticalOptions = LayoutOptions.Center },
                                new Switch(),
                                new Label { Text = "Notifications", VerticalOptions = LayoutOptions.Center },
                            },
                        },
                    },
                },
            },
        };

        InitializeDemoLayout();
        SetupEventHandlers();
    }

    private void InitializeDemoLayout()
    {
        _demoWrapLayout = new WrapLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalSpacing = 10,
            VerticalSpacing = 10,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
        };

        // Add initial items
        for (int i = 1; i <= 8; i++)
        {
            _demoWrapLayout.Children.Add(CreateDemoItem($"Item {i}", GetRandomColor()));
            _itemCounter = i + 1;
        }

        _demoFrame.Content = _demoWrapLayout;
    }

    private void SetupEventHandlers()
    {
        _orientationPicker.SelectedIndexChanged += (s, e) =>
        {
            _demoWrapLayout.Orientation = _orientationPicker.SelectedIndex == 0
                ? StackOrientation.Horizontal
                : StackOrientation.Vertical;
        };

        _horizontalSpacingSlider.ValueChanged += (s, e) =>
        {
            _demoWrapLayout.HorizontalSpacing = e.NewValue;
            _horizontalSpacingLabel.Text = $"Horizontal Spacing: {e.NewValue:F0}";
        };

        _verticalSpacingSlider.ValueChanged += (s, e) =>
        {
            _demoWrapLayout.VerticalSpacing = e.NewValue;
            _verticalSpacingLabel.Text = $"Vertical Spacing: {e.NewValue:F0}";
        };

        _horizontalOptionsPicker.SelectedIndexChanged += (s, e) =>
        {
            _demoWrapLayout.HorizontalOptions = GetLayoutOptions(_horizontalOptionsPicker.SelectedIndex);
        };

        _verticalOptionsPicker.SelectedIndexChanged += (s, e) =>
        {
            _demoWrapLayout.VerticalOptions = GetLayoutOptions(_verticalOptionsPicker.SelectedIndex);
        };

        _addItemButton.Clicked += (s, e) =>
        {
            _demoWrapLayout.Children.Add(CreateDemoItem($"Item {_itemCounter}", GetRandomColor()));
            _itemCounter++;
        };

        _removeItemButton.Clicked += (s, e) =>
        {
            if (_demoWrapLayout.Children.Count > 0)
            {
                _demoWrapLayout.Children.RemoveAt(_demoWrapLayout.Children.Count - 1);
            }
        };

        _clearItemsButton.Clicked += (s, e) =>
        {
            _demoWrapLayout.Children.Clear();
            _itemCounter = 1;
        };
    }

    private LayoutOptions GetLayoutOptions(int index)
    {
        return index switch
        {
            0 => LayoutOptions.Start,
            1 => LayoutOptions.Center,
            2 => LayoutOptions.End,
            3 => LayoutOptions.Fill,
            4 => LayoutOptions.StartAndExpand,
            5 => LayoutOptions.CenterAndExpand,
            6 => LayoutOptions.EndAndExpand,
            7 => LayoutOptions.FillAndExpand,
            _ => LayoutOptions.Start,
        };
    }

    private View CreateDemoItem(string text, Color color)
    {
        var random = new Random();
        var itemType = random.Next(0, 3);

        return itemType switch
        {
            0 => new Button
            {
                Text = text,
                BackgroundColor = color,
                TextColor = Colors.White,
                Padding = new Thickness(8, 4),
                CornerRadius = 5,
                WidthRequest = random.Next(60, 150),
            },
            1 => new Label
            {
                Text = text,
                BackgroundColor = color,
                TextColor = Colors.White,
                Padding = new Thickness(8, 4),
                WidthRequest = random.Next(50, 120),
                HorizontalTextAlignment = TextAlignment.Center,
            },
            _ => new Frame
            {
                BackgroundColor = color,
                Padding = new Thickness(5),
                CornerRadius = 3,
                Content = new Label
                {
                    Text = text,
                    TextColor = Colors.White,
                    FontSize = 12,
                },
            },
        };
    }

    private Button CreateButton(string text, Color color)
    {
        return new Button
        {
            Text = text,
            BackgroundColor = color,
            TextColor = Colors.White,
            Padding = new Thickness(10, 5),
            CornerRadius = 5,
        };
    }

    private Label CreateLabel(string text)
    {
        return new Label
        {
            Text = text,
            BackgroundColor = Colors.White,
            Padding = new Thickness(8, 4),
        };
    }

    private Color GetRandomColor()
    {
        var colors = new[]
        {
            Colors.Red, Colors.Blue, Colors.Green, Colors.Orange, Colors.Purple,
            Colors.Teal, Colors.Pink, Colors.Brown, Colors.Gray, Colors.Cyan,
            Colors.Magenta, Colors.Lime, Colors.Indigo, Colors.Coral, Colors.Gold,
        };
        var random = new Random();
        return colors[random.Next(colors.Length)];
    }
}
