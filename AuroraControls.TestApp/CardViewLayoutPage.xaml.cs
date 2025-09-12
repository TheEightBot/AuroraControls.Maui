namespace AuroraControls.TestApp;

public partial class CardViewLayoutPage : ContentPage
{
    public CardViewLayoutPage()
    {
        InitializeComponent();
    }

    private void OnCornerRadiusChanged(object sender, ValueChangedEventArgs e)
    {
        sampleCard.CornerRadius = e.NewValue;
    }

    private void OnElevationChanged(object sender, ValueChangedEventArgs e)
    {
        sampleCard.Elevation = e.NewValue;
    }

    private void OnBorderSizeChanged(object sender, ValueChangedEventArgs e)
    {
        sampleCard.BorderSize = e.NewValue;
    }

    private void OnShadowColorChanged(object sender, EventArgs e)
    {
        if (sender is Button button)
        {
            Color shadowColor = button.Text switch
            {
                "Gray" => Colors.Gray,
                "Blue" => Colors.Blue,
                "Red" => Colors.Red,
                "Green" => Colors.Green,
                "Purple" => Colors.Purple,
                _ => Color.FromArgb("#576076"), // Default shadow color
            };

            sampleCard.ShadowColor = shadowColor;
        }
    }

    private void OnBorderColorChanged(object sender, EventArgs e)
    {
        if (sender is Button button)
        {
            Color borderColor = button.Text switch
            {
                "None" => Colors.Transparent,
                "Black" => Colors.Black,
                "Blue" => Colors.Blue,
                "Red" => Colors.Red,
                "Green" => Colors.Green,
                _ => Colors.Transparent,
            };

            sampleCard.BorderColor = borderColor;
        }
    }

    private void OnBackgroundColorChanged(object sender, EventArgs e)
    {
        if (sender is Button button)
        {
            Color backgroundColor = button.Text switch
            {
                "White" => Colors.White,
                "Light Blue" => Colors.LightBlue,
                "Light Green" => Colors.LightGreen,
                "Light Pink" => Colors.LightPink,
                "Light Yellow" => Colors.LightYellow,
                _ => Colors.White,
            };

            cardContent.BackgroundColor = backgroundColor;
        }
    }
}
