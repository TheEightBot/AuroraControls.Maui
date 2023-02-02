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
                new FloatLabelEntry
                {
                    Text = "This is My Entry",
                    Placeholder = "My Placeholder",
                    BackgroundColor = Colors.Fuchsia,
                    BorderStyle = ContainerBorderStyle.RoundedRectangle,
                },
            }
        };
    }
}
