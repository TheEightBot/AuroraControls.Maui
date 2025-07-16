namespace AuroraControls;

internal class NoCacheFileImageSource : ImageSource, INoCacheFileImageSource
{
    public static readonly BindableProperty FileProperty = BindableProperty.Create(nameof(File), typeof(string), typeof(NoCacheFileImageSource));

    public override bool IsEmpty => string.IsNullOrEmpty(File);

    public string File
    {
        get => (string)GetValue(FileProperty);
        set => SetValue(FileProperty, value);
    }

    public override Task<bool> Cancel() => Task.FromResult(false);

    public override string ToString() => $"File: {File}";

    public static implicit operator NoCacheFileImageSource(string file) => (NoCacheFileImageSource)FromFile(file);

    public static implicit operator string(NoCacheFileImageSource file) => file?.File;

    protected override void OnPropertyChanged(string propertyName = null)
    {
        if (propertyName == FileProperty.PropertyName)
        {
            OnSourceChanged();
        }

        base.OnPropertyChanged(propertyName);
    }
}
