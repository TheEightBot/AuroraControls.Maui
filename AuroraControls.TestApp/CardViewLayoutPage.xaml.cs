using AuroraControls;

namespace AuroraControls.TestApp;

public partial class CardViewLayoutPage : ContentPage
{
    public CardViewLayoutPage()
    {
        InitializeComponent();
    }

    private async void UpdateValues_Clicked(object sender, System.EventArgs e)
    {
        var rngesus = new Random(Guid.NewGuid().GetHashCode());

        var elevation = rngesus.Next(0, 8);
        var cornerRadius = rngesus.Next(0, 24);
        var borderSize = rngesus.Next(0, 10);
        var borderColor = Color.FromRgb(rngesus.Next(0, 255), rngesus.Next(0, 255), rngesus.Next(0, 255));
        var backgroundColor = Color.FromRgb(rngesus.Next(0, 255), rngesus.Next(0, 255), rngesus.Next(0, 255));

        await Task.WhenAll(
            control.TransitionTo(c => c.CornerRadius, cornerRadius),
            control.TransitionTo(c => c.BorderSize, borderSize),
            control.ColorTo(c => c.BorderColor, borderColor),
            control.ColorTo(c => c.BackgroundColor, backgroundColor),
            control.TransitionTo(c => c.Elevation, elevation),
            content.ThicknessTo(c => c.Padding, new Thickness(borderSize * .5d)));
    }
}
