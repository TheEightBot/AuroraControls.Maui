namespace AuroraControls.TestApp;

public partial class ImageProcessing : ContentPage
{
    private List<AuroraControls.ImageProcessing.ImageProcessingBase> _imageProcessing = new();

    private int _index = 0;

    private AuroraControls.ImageProcessing.Blur _blur;
    private AuroraControls.ImageProcessing.Circular _circular;
    private AuroraControls.ImageProcessing.Grayscale _grayscale;
    private AuroraControls.ImageProcessing.Invert _invert;
    private AuroraControls.ImageProcessing.Scale _scale;
    private AuroraControls.ImageProcessing.Sepia _sepia;

    private Random _rngesus = new Random(Guid.NewGuid().GetHashCode());

    public ImageProcessing()
    {
        InitializeComponent();

        _blur = new AuroraControls.ImageProcessing.Blur { };
        _circular = new AuroraControls.ImageProcessing.Circular();
        _grayscale = new AuroraControls.ImageProcessing.Grayscale();
        _invert = new AuroraControls.ImageProcessing.Invert();
        _scale = new AuroraControls.ImageProcessing.Scale();
        _sepia = new AuroraControls.ImageProcessing.Sepia();

        _imageProcessing.AddRange([_blur, _circular, _grayscale, _invert, _scale, _sepia]);
    }

    private void Handle_ValueChanged(object? sender, ValueChangedEventArgs e)
    {
        _blur.BlurAmount = e.NewValue;
    }

    private void Handle_Clicked(object? sender, System.EventArgs e)
    {
        if (_index > _imageProcessing.Count - 1)
        {
            _index = 0;
        }

        var processingEffect = _imageProcessing.ElementAt(_index);

        if (ImageProcessingEffect.ImageProcessingEffects.Contains(processingEffect))
        {
            ImageProcessingEffect.ImageProcessingEffects.Remove(processingEffect);
        }

        ImageProcessingEffect.ImageProcessingEffects.Add(processingEffect);

        _index++;
    }
}
