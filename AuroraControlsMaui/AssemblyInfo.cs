[assembly: XmlnsDefinition(Constants.XamlNamespace, Constants.AuroraNamespace)]
[assembly: XmlnsDefinition(Constants.XamlNamespace, Constants.AuroraNamespacePrefix + nameof(AuroraControls.DataGrid))]
[assembly: XmlnsDefinition(Constants.XamlNamespace, Constants.AuroraNamespacePrefix + nameof(AuroraControls.Effects))]
[assembly: XmlnsDefinition(Constants.XamlNamespace, Constants.AuroraNamespacePrefix + nameof(AuroraControls.Gauges))]
[assembly: XmlnsDefinition(Constants.XamlNamespace, Constants.AuroraNamespacePrefix + nameof(AuroraControls.Loading))]
[assembly: XmlnsDefinition(Constants.XamlNamespace, Constants.AuroraNamespacePrefix + nameof(AuroraControls.VisualEffects))]

[assembly: Microsoft.Maui.Controls.XmlnsPrefix(Constants.XamlNamespace, "aurora")]

public static class Constants
{
    public const string XamlNamespace = "http://auroracontrols.maui/controls";
    public const string AuroraNamespace = $"{nameof(AuroraControls)}";
    public const string AuroraNamespacePrefix = $"{AuroraNamespace}.";
}
