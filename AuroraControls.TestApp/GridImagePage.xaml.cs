using Microsoft.Maui.Controls;

namespace AuroraControls.TestApp;

public partial class GridImagePage : ContentPage
{
    public GridImagePage()
    {
        InitializeComponent();

        img.SetSvgIcon("dollar_sign.svg", 74);
        imgBtn.SetSvgIcon("dollar_sign.svg", 74);
    }
}
