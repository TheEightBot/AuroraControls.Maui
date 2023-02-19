namespace AuroraControls.TestApp;

public class MainPage : ContentPage
{
    public MainPage()
    {
        Content = new VerticalStackLayout
        {
            Padding = 16,
            Spacing = 16,
            Children =
            {
                new Label { HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, Text = "Welcome to .NET MAUI!"},
                new StyledInputLayout
                {
                    Placeholder = "My Placeholder With Rounded Rectangle Placeholder Through",
                    BackgroundColor = Colors.Fuchsia,
                    ActiveColor = Colors.Red,
                    InactiveColor = Colors.Green,
                    BorderStyle = ContainerBorderStyle.RoundedRectanglePlaceholderThrough,
                    Content =
                        new Entry
                        {
                            Text = "This is My Entry",
                            Placeholder = "This is a sample",
                        },
                },
                new StyledInputLayout
                {
                    Placeholder = "My Picker",
                    BackgroundColor = Colors.Chartreuse,
                    BorderStyle = ContainerBorderStyle.RoundedRectanglePlaceholderThrough,
                    Content =
                        new Picker
                        {
                            ItemsSource =
                            new []
                            {
                                "Item 1",
                                "Item 2",
                                "Item 3",
                                "Item 4",
                            },
                        },
                },
                new StyledInputLayout
                {
                    Placeholder = "My Date Picker",
                    BackgroundColor = Colors.Chartreuse,
                    BorderStyle = ContainerBorderStyle.RoundedRectangle,
                    Content =
                        new DatePicker
                        {
                        },
                },
                new StyledInputLayout
                {
                    Placeholder = "My Editor",
                    BackgroundColor = Colors.Chartreuse,
                    BorderStyle = ContainerBorderStyle.Rectangle,
                    Content =
                        new Editor
                        {
                            Placeholder = "Test Entry",
                            AutoSize = EditorAutoSizeOption.TextChanges,
                        },
                },
            },
        };
    }
}
