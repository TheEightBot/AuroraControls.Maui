using System.Diagnostics;
using SkiaSharp;

namespace AuroraControls.TestApp;

public class SignaturePadPage : ContentPage
{
    private readonly SignaturePad _signaturePad;
    private readonly Button _clearButton;
    private readonly Button _saveButton;
    private readonly Label _statusLabel;

    public SignaturePadPage()
    {
        Title = "Signature Pad";

        // Create the signature pad
        _signaturePad = new SignaturePad
        {
            HeightRequest = 300,
            WidthRequest = 400,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            BackgroundColor = Colors.White,
            PenColor = Colors.Blue,
            MinWidth = 2,
            MaxWidth = 5,
            VelocityFilterWeight = 0.9f,
            ClearOnDoubleClick = true,
        };

        // Create buttons
        _clearButton = new Button
        {
            Text = "Clear",
            HorizontalOptions = LayoutOptions.Center,
        };
        _clearButton.Clicked += OnClearButtonClicked;

        _saveButton = new Button
        {
            Text = "Save Signature",
            IsEnabled = false,
            HorizontalOptions = LayoutOptions.Center,
        };
        _saveButton.Clicked += OnSaveButtonClicked;

        // Create status label
        _statusLabel = new Label
        {
            Text = "Draw your signature above",
            HorizontalOptions = LayoutOptions.Center,
            FontAttributes = FontAttributes.Italic,
        };

        // Subscribe to events
        _signaturePad.StartedSigning += OnStartedSigning;
        _signaturePad.Signed += OnSigned;
        _signaturePad.Cleared += OnCleared;

        // Build the page layout
        Content = new VerticalStackLayout
        {
            Padding = new Thickness(20),
            Spacing = 15,
            Children =
            {
                new Label
                {
                    Text = "Signature Pad Demo",
                    FontSize = 24,
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.Center,
                },
                new Frame
                {
                    BorderColor = Colors.LightGray,
                    CornerRadius = 5,
                    Padding = 2,
                    Content = _signaturePad,
                },
                _statusLabel,
                new HorizontalStackLayout
                {
                    HorizontalOptions = LayoutOptions.Center,
                    Spacing = 10,
                    Children = { _clearButton, _saveButton, },
                },
            },
        };
    }

    private void OnStartedSigning(object sender, EventArgs e)
    {
        _statusLabel.Text = "Signing...";
    }

    private void OnSigned(object sender, EventArgs e)
    {
        _statusLabel.Text = "Signature completed";
        _saveButton.IsEnabled = true;
    }

    private void OnCleared(object sender, EventArgs e)
    {
        _statusLabel.Text = "Signature cleared";
        _saveButton.IsEnabled = false;
    }

    private void OnClearButtonClicked(object sender, EventArgs e)
    {
        _signaturePad.Clear();
    }

    private async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        try
        {
            // Get the signature bitmap with white background
            var bitmap = _signaturePad.GetSignatureBitmap();

            // Save bitmap to file
            var fileName = $"signature_{DateTime.Now:yyyyMMdd_HHmmss}.png";
            var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

            using (var stream = File.OpenWrite(filePath))
            {
                // Save as PNG
                bitmap.Encode(SKEncodedImageFormat.Png, 100).SaveTo(stream);
            }

            _statusLabel.Text = $"Signature saved: {fileName}";

            // Optionally, you could also save the SVG
            var svgContent = _signaturePad.GetSignatureSvg();
            var svgPath = Path.Combine(FileSystem.CacheDirectory, $"signature_{DateTime.Now:yyyyMMdd_HHmmss}.svg");
            File.WriteAllText(svgPath, svgContent);

            await DisplayAlert("Signature Saved", $"Signature has been saved to:\n{filePath}", "OK");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error saving signature: {ex.Message}");
            await DisplayAlert("Error", "Could not save the signature", "OK");
        }
    }
}
