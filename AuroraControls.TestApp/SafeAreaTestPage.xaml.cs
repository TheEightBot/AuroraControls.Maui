namespace AuroraControls.TestApp;

public partial class SafeAreaTestPage : ContentPage
{
    public SafeAreaTestPage()
    {
        InitializeComponent();
    }

    private void OnBackButtonClicked(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }
}
