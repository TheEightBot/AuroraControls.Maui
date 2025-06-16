using AuroraControls.VisualEffects;

namespace AuroraControls.TestApp;

public partial class SvgImageViewTestPage : ContentPage
{
    private readonly List<string> _svgImages = new()
    {
        "splatoon.svg",
        "triforce.svg",
        "more.svg",
    };

    private readonly Dictionary<string, Color> _colors = new()
    {
        { "Transparent", Colors.Transparent },
        { "Red", Colors.Red },
        { "Blue", Colors.Blue },
        { "Green", Colors.Green },
        { "Purple", Colors.Purple },
        { "Orange", Colors.Orange },
        { "Cyan", Colors.Cyan },
        { "Yellow", Colors.Yellow },
        { "Pink", Colors.Pink },
        { "Brown", Colors.Brown },
        { "Gray", Colors.Gray },
        { "Black", Colors.Black },
        { "White", Colors.White },
        { "Navy", Colors.Navy },
        { "Teal", Colors.Teal },
    };

    public SvgImageViewTestPage()
    {
        InitializeComponent();
        InitializeControls();
    }

    private void InitializeControls()
    {
        // Initialize SVG picker
        SvgPicker.ItemsSource = _svgImages;
        SvgPicker.SelectedIndex = 0;

        // Initialize color picker
        ColorPicker.ItemsSource = _colors.Keys.ToList();
        ColorPicker.SelectedIndex = 0;

        // Set initial values for sliders
        WidthSlider.Value = 80;
        HeightSlider.Value = 80;
        MaxWidthSlider.Value = 0;
        MaxHeightSlider.Value = 0;

        // Update labels
        UpdateSliderLabels();
    }

    private void OnSvgPickerSelectedIndexChanged(object sender, EventArgs e)
    {
        if (SvgPicker.SelectedItem is string selectedSvg && PreviewSvgImageView != null)
        {
            PreviewSvgImageView.EmbeddedImageName = selectedSvg;
        }
    }

    private void OnColorPickerSelectedIndexChanged(object sender, EventArgs e)
    {
        if (ColorPicker.SelectedItem is string selectedColorName &&
            PreviewSvgImageView != null &&
            _colors.TryGetValue(selectedColorName, out var color))
        {
            PreviewSvgImageView.OverlayColor = color;
        }
    }

    private void OnWidthSliderValueChanged(object sender, ValueChangedEventArgs e)
    {
        if (PreviewSvgImageView != null)
        {
            PreviewSvgImageView.WidthRequest = e.NewValue;
            WidthLabel.Text = ((int)e.NewValue).ToString();
        }
    }

    private void OnHeightSliderValueChanged(object sender, ValueChangedEventArgs e)
    {
        if (PreviewSvgImageView != null)
        {
            PreviewSvgImageView.HeightRequest = e.NewValue;
            HeightLabel.Text = ((int)e.NewValue).ToString();
        }
    }

    private void OnMaxSizeSliderValueChanged(object sender, ValueChangedEventArgs e)
    {
        if (PreviewSvgImageView != null)
        {
            var maxWidth = MaxWidthSlider.Value;
            var maxHeight = MaxHeightSlider.Value;

            PreviewSvgImageView.MaxImageSize = new Size(maxWidth, maxHeight);

            MaxWidthLabel.Text = ((int)maxWidth).ToString();
            MaxHeightLabel.Text = ((int)maxHeight).ToString();
        }
    }

    private void UpdateSliderLabels()
    {
        WidthLabel.Text = ((int)WidthSlider.Value).ToString();
        HeightLabel.Text = ((int)HeightSlider.Value).ToString();
        MaxWidthLabel.Text = ((int)MaxWidthSlider.Value).ToString();
        MaxHeightLabel.Text = ((int)MaxHeightSlider.Value).ToString();
    }

    // Visual Effects Event Handlers
    private void OnClearEffectsClicked(object sender, EventArgs e)
    {
        EffectsSvgImageView.VisualEffects.Clear();
    }

    private void OnSepiaEffectClicked(object sender, EventArgs e)
    {
        EffectsSvgImageView.VisualEffects.Clear();
        EffectsSvgImageView.VisualEffects.Add(new Sepia());
    }

    private void OnGrayscaleEffectClicked(object sender, EventArgs e)
    {
        EffectsSvgImageView.VisualEffects.Clear();
        EffectsSvgImageView.VisualEffects.Add(new Grayscale());
    }

    private void OnBlackWhiteEffectClicked(object sender, EventArgs e)
    {
        EffectsSvgImageView.VisualEffects.Clear();
        EffectsSvgImageView.VisualEffects.Add(new BlackAndWhite());
    }

    private void OnInvertEffectClicked(object sender, EventArgs e)
    {
        EffectsSvgImageView.VisualEffects.Clear();
        EffectsSvgImageView.VisualEffects.Add(new Invert());
    }

    private void OnHighContrastEffectClicked(object sender, EventArgs e)
    {
        EffectsSvgImageView.VisualEffects.Clear();
        EffectsSvgImageView.VisualEffects.Add(new HighContrast());
    }
}
