namespace AuroraControls;

internal class NoCacheFileImageSource : ImageSource, INoCacheFileImageSource
{
    public static readonly BindableProperty FileProperty = BindableProperty.Create(nameof(File), typeof(string), typeof(NoCacheFileImageSource), default(string));

    /// <include file="../../docs/Microsoft.Maui.Controls/FileImageSource.xml" path="//Member[@MemberName='IsEmpty']/Docs/*" />
    public override bool IsEmpty => string.IsNullOrEmpty(File);

    /// <include file="../../docs/Microsoft.Maui.Controls/FileImageSource.xml" path="//Member[@MemberName='File']/Docs/*" />
    public string File
    {
        get { return (string)GetValue(FileProperty); }
        set { SetValue(FileProperty, value); }
    }

    /// <include file="../../docs/Microsoft.Maui.Controls/FileImageSource.xml" path="//Member[@MemberName='Cancel']/Docs/*" />
    public override Task<bool> Cancel()
    {
        return Task.FromResult(false);
    }

    /// <include file="../../docs/Microsoft.Maui.Controls/FileImageSource.xml" path="//Member[@MemberName='ToString']/Docs/*" />
    public override string ToString()
    {
        return $"File: {File}";
    }

    public static implicit operator NoCacheFileImageSource(string file)
    {
        return (NoCacheFileImageSource)FromFile(file);
    }

    public static implicit operator string(NoCacheFileImageSource file)
    {
        return file?.File;
    }

    protected override void OnPropertyChanged(string propertyName = null)
    {
        if (propertyName == FileProperty.PropertyName)
        {
            OnSourceChanged();
        }

        base.OnPropertyChanged(propertyName);
    }
}
