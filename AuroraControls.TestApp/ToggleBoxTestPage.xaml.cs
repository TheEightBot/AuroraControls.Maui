using Microsoft.Maui.Controls;

namespace AuroraControls.TestApp;

public partial class ToggleBoxTestPage : ContentPage
{
    public ToggleBoxTestPage()
    {
        InitializeComponent();
        InitializeUI();
    }

    private void InitializeUI()
    {
        // Initialize pickers
        ShapePicker.ItemsSource = Enum.GetNames(typeof(ToggleBoxShape));
        ShapePicker.SelectedIndex = 0;

        CheckTypePicker.ItemsSource = Enum.GetNames(typeof(ToggleBoxCheckType));
        CheckTypePicker.SelectedIndex = 0;

        // Set initial state for the preview toggle box
        PreviewToggleBox.Shape = ToggleBoxShape.Square;
        PreviewToggleBox.CheckType = ToggleBoxCheckType.Check;
        PreviewToggleBox.BorderWidth = (int)BorderWidthSlider.Value;
        PreviewToggleBox.MarkWidth = (int)MarkWidthSlider.Value;
        PreviewToggleBox.CornerRadius = CornerRadiusSlider.Value;
        PreviewToggleBox.IsToggled = ToggleSwitch.IsToggled;
    }

    private void OnToggleSwitchToggled(object sender, ToggledEventArgs e)
    {
        if (PreviewToggleBox != null)
        {
            PreviewToggleBox.IsToggled = e.Value;
        }
    }

    private void OnShapePickerSelectedIndexChanged(object sender, EventArgs e)
    {
        if (PreviewToggleBox == null || ShapePicker.SelectedItem == null)
        {
            return;
        }

        if (Enum.TryParse<ToggleBoxShape>(ShapePicker.SelectedItem.ToString(), out var shape))
        {
            PreviewToggleBox.Shape = shape;
        }
    }

    private void OnCheckTypePickerSelectedIndexChanged(object sender, EventArgs e)
    {
        if (PreviewToggleBox == null || CheckTypePicker.SelectedItem == null)
        {
            return;
        }

        if (Enum.TryParse<ToggleBoxCheckType>(CheckTypePicker.SelectedItem.ToString(), out var checkType))
        {
            PreviewToggleBox.CheckType = checkType;
        }
    }

    private void OnBorderWidthSliderValueChanged(object sender, ValueChangedEventArgs e)
    {
        if (PreviewToggleBox != null)
        {
            PreviewToggleBox.BorderWidth = (int)e.NewValue;
        }
    }

    private void OnMarkWidthSliderValueChanged(object sender, ValueChangedEventArgs e)
    {
        if (PreviewToggleBox != null)
        {
            PreviewToggleBox.MarkWidth = (int)e.NewValue;
        }
    }

    private void OnCornerRadiusSliderValueChanged(object sender, ValueChangedEventArgs e)
    {
        if (PreviewToggleBox != null)
        {
            PreviewToggleBox.CornerRadius = e.NewValue;
        }
    }
}
