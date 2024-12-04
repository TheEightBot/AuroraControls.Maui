using AuroraControls;

[assembly: Microsoft.Maui.Controls.XmlnsPrefix(Constants.XamlNamespace, Constants.CommunityToolkitNamespacePrefix + nameof(AuroraControls.Gauges))]
[assembly: Microsoft.Maui.Controls.XmlnsPrefix(Constants.XamlNamespace, Constants.CommunityToolkitNamespacePrefix + nameof(AuroraControls.Loading))]
[assembly: Microsoft.Maui.Controls.XmlnsPrefix(Constants.XamlNamespace, Constants.CommunityToolkitNamespacePrefix + nameof(AuroraControls.VisualEffects))]
[assembly: Microsoft.Maui.Controls.XmlnsPrefix(Constants.XamlNamespace, Constants.CommunityToolkitNamespace)]

[assembly: Microsoft.Maui.Controls.XmlnsDefinition(Constants.XamlNamespace, "aurora")]

namespace AuroraControls;

#pragma warning disable SA1649
internal static class Constants
#pragma warning restore SA1649
{
    public const string XamlNamespace = "http://auroracontrols.maui/schemas/controls";
    public const string CommunityToolkitNamespace = $"{nameof(AuroraControls)}";
    public const string CommunityToolkitNamespacePrefix = $"{CommunityToolkitNamespace}.";
}
