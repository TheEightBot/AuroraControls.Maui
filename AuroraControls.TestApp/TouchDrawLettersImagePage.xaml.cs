using AuroraControls.VisualEffects;

namespace AuroraControls.TestApp;

public partial class TouchDrawLettersImagePage : ContentPage
{
    private char _counter = 'A';
    private int _fontSize = 24;
    private Random _rngesus = new Random(Guid.NewGuid().GetHashCode());

    public TouchDrawLettersImagePage()
    {
        InitializeComponent();

        this.control.VisualEffects.Add(
            new Watermark
            {
                WatermarkText = "Eight-Bot",
                ForegroundColor = Colors.Fuchsia,
                BackgroundColor = Colors.Black.MultiplyAlpha(.5f),
            });
    }

    private void AddLetter_Clicked(object sender, System.EventArgs e)
    {
        _counter++;

        this.control
            .TouchDrawLetters
            .Add(
                new TouchDrawLetter
                {
                    Value = _counter.ToString(),
                    ForegroundColorOverride =
                        Color.FromRgba(_rngesus.Next(0, 255), _rngesus.Next(0, 255), _rngesus.Next(0, 255),
                            _rngesus.Next(0, 255)),
                    BackgroundColorOverride = Color.FromRgba(_rngesus.Next(0, 255), _rngesus.Next(0, 255),
                        _rngesus.Next(0, 255), _rngesus.Next(0, 255)),
                });

        this.control.VisualEffects.Add(
            new Watermark
            {
                WatermarkText = $"Eight-Bot {_counter}",
                FontSize = _fontSize++,
                ForegroundColor =
                    Color.FromRgba(_rngesus.Next(0, 255), _rngesus.Next(0, 255), _rngesus.Next(0, 255),
                        _rngesus.Next(0, 255)),
                BackgroundColor =
                    Color.FromRgba(_rngesus.Next(0, 255), _rngesus.Next(0, 255), _rngesus.Next(0, 255),
                        _rngesus.Next(0, 255)),
                HorizontalWatermarkLocation = Extensions.RandomEnum<Watermark.WatermarkLocation>(_rngesus),
                VerticalWatermarkLocation = Extensions.RandomEnum<Watermark.WatermarkLocation>(_rngesus),
            });
    }

    private async void Export_Clicked(object sender, System.EventArgs e)
    {
        _counter = 'A';

        var fileName = $"touchletters-{Guid.NewGuid()}.jpg";
        var outputFile = Path.Combine(FileSystem.CacheDirectory, $"dest-{fileName}");

        using (var exportStream = this.control.ExportImage(SkiaSharp.SKEncodedImageFormat.Png, 100, 2000, 2000))
        using (var fileStream = new FileStream(outputFile, FileMode.OpenOrCreate))
        {
            await exportStream.CopyToAsync(fileStream).ConfigureAwait(false);
            await exportStream.FlushAsync().ConfigureAwait(false);
        }
    }
}
