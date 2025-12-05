namespace AuroraControls.TestApp;

public partial class TileTestPage : ContentPage
{
    public TileTestPage()
    {
        InitializeComponent();

        // Set initial values
        ImagePicker.SelectedIndex = 0;
        UpdateInfoLabel();
    }

    private void OnMaxImageSizeChanged(object sender, ValueChangedEventArgs e)
    {
        var width = MaxImageWidthSlider.Value;
        var height = MaxImageHeightSlider.Value;

        // If both are 0, set to default (no constraint)
        if (width == 0 && height == 0)
        {
            TestTile.MaxImageSize = Size.Zero;
        }
        else
        {
            // Use a large value (999) if slider is at 0 to effectively mean "no constraint on this dimension"
            TestTile.MaxImageSize = new Size(width == 0 ? 999 : width, height == 0 ? 999 : height);
        }

        UpdateInfoLabel();
    }

    private void OnContentPaddingChanged(object sender, ValueChangedEventArgs e)
    {
        TestTile.ContentPadding = new Thickness(
            PaddingLeftSlider.Value,
            PaddingTopSlider.Value,
            PaddingRightSlider.Value,
            PaddingBottomSlider.Value);

        UpdateInfoLabel();
    }

    private void OnTileSizeChanged(object sender, ValueChangedEventArgs e)
    {
        TestTile.WidthRequest = TileWidthSlider.Value;
        TestTile.HeightRequest = TileHeightSlider.Value;

        UpdateInfoLabel();
    }

    private void OnTextToggled(object sender, ToggledEventArgs e)
    {
        TestTile.Text = e.Value ? "Tile Test" : string.Empty;
        UpdateInfoLabel();
    }

    private void OnImageChanged(object sender, EventArgs e)
    {
        if (ImagePicker.SelectedIndex >= 0)
        {
            TestTile.EmbeddedImageName = ImagePicker.Items[ImagePicker.SelectedIndex];
            UpdateInfoLabel();
        }
    }

    private void OnResetClicked(object sender, EventArgs e)
    {
        // Reset all sliders to defaults
        MaxImageWidthSlider.Value = 0;
        MaxImageHeightSlider.Value = 0;
        PaddingLeftSlider.Value = 8;
        PaddingTopSlider.Value = 8;
        PaddingRightSlider.Value = 8;
        PaddingBottomSlider.Value = 8;
        TileWidthSlider.Value = 200;
        TileHeightSlider.Value = 200;
        TextToggle.IsToggled = true;
        ImagePicker.SelectedIndex = 0;

        UpdateInfoLabel();
    }

    private void UpdateInfoLabel()
    {
        var maxImageSize = TestTile.MaxImageSize;
        var contentPadding = TestTile.ContentPadding;

        var maxSizeText = maxImageSize == Size.Zero || (maxImageSize.Width == 0 && maxImageSize.Height == 0)
            ? "No constraint (scales to fit)"
            : string.Format("Max: {0:F0} x {1:F0}", maxImageSize.Width, maxImageSize.Height);

        InfoLabel.Text = string.Format(
            "Current: {0} | Padding: {1:F0},{2:F0},{3:F0},{4:F0}",
            maxSizeText,
            contentPadding.Left,
            contentPadding.Top,
            contentPadding.Right,
            contentPadding.Bottom);
    }
}
