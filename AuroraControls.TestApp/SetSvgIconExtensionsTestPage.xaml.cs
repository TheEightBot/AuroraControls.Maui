using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AuroraControls.TestApp;

public partial class SetSvgIconExtensionsTestPage : ContentPage
{
    private readonly string[] _svgIcons = { "logo.svg", "splatoon.svg", "triforce.svg", "dollar_sign.svg", "more.svg" };
    private readonly double[] _sizes = { 16d, 24d, 32d, 48d, 64d };
    private int _currentIconIndex;
    private bool _useColorOverride;
    private double _currentSize = 24d;
    private int _currentSizeIndex = 1; // Start with 24px

    public SetSvgIconExtensionsTestPage()
    {
        InitializeComponent();
        BindingContext = this;
        InitializeIcons();
        SetupToolbarItems();
    }

    private void InitializeIcons()
    {
        // Test Button.SetSvgIcon() with different parameters
        TestButton1.SetSvgIcon("logo.svg");
        TestButton2.SetSvgIcon("splatoon.svg", 32d);
        TestButton3.SetSvgIcon("triforce.svg", 24d, Colors.Blue);

        // Test ImageButton.SetSvgIcon() with different parameters
        TestImageButton1.SetSvgIcon("dollar_sign.svg");
        TestImageButton2.SetSvgIcon("more.svg", 48d);
        TestImageButton3.SetSvgIcon("logo.svg", 40d, Colors.Green);

        // Test Image.SetSvgIcon() with different parameters
        TestImage1.SetSvgIcon("triforce.svg"); // Default size (24px)
        TestImage2.SetSvgIcon("splatoon.svg", 48d); // Custom size
        TestImage3.SetSvgIcon("dollar_sign.svg", 32d, Colors.Red); // Custom size and color

        UpdateStatus("Icons initialized with various SetSvgIcon() configurations");
    }

    private void SetupToolbarItems()
    {
        // Test ToolbarItem.SetSvgIcon()
        var toolbarItem1 = new ToolbarItem { Text = "Tool 1" };
        toolbarItem1.SetSvgIcon("logo.svg");
        toolbarItem1.Clicked += OnToolbarItemClicked;

        var toolbarItem2 = new ToolbarItem { Text = "Tool 2" };
        toolbarItem2.SetSvgIcon("more.svg", 24d, Colors.Purple);
        toolbarItem2.Clicked += OnToolbarItemClicked;

        ToolbarItems.Add(toolbarItem1);
        ToolbarItems.Add(toolbarItem2);

        // Test Page.SetSvgIcon() - Set icon for this page
        this.SetSvgIcon("triforce.svg");
    }

    private void OnButtonClicked(object sender, EventArgs e)
    {
        if (sender is Button button)
        {
            UpdateStatus($"Button clicked: {button.Text}");
        }
    }

    private void OnImageButtonClicked(object sender, EventArgs e)
    {
        UpdateStatus("ImageButton clicked");
    }

    private void OnToolbarItemClicked(object sender, EventArgs e)
    {
        if (sender is ToolbarItem toolbarItem)
        {
            UpdateStatus($"ToolbarItem clicked: {toolbarItem.Text}");
        }
    }

    private void OnShowContextMenuClicked(object sender, EventArgs e)
    {
        // Create context menu with MenuItem.SetSvgIcon()
        var menuItem1 = new MenuFlyoutItem { Text = "Menu Item 1" };
        menuItem1.SetSvgIcon("logo.svg", 20d);
        menuItem1.Clicked += (_, _) => UpdateStatus("Context Menu Item 1 clicked");

        var menuItem2 = new MenuFlyoutItem { Text = "Menu Item 2" };
        menuItem2.SetSvgIcon("splatoon.svg", 20d, Colors.Orange);
        menuItem2.Clicked += (_, _) => UpdateStatus("Context Menu Item 2 clicked");

        var menuFlyout = new MenuFlyout();
        menuFlyout.Add(menuItem1);
        menuFlyout.Add(menuItem2);

        if (sender is Button button)
        {
            FlyoutBase.SetContextFlyout(button, menuFlyout);
        }

        UpdateStatus("Context menu with SetSvgIcon() MenuItems created");
    }

    private void OnChangeIconsClicked(object sender, EventArgs e)
    {
        // Cycle through different icons
        _currentIconIndex = (_currentIconIndex + 1) % _svgIcons.Length;
        var newIcon = _svgIcons[_currentIconIndex];

        // Update all controls with new icons
        TestButton1.SetSvgIcon(newIcon, _currentSize, _useColorOverride ? Colors.Purple : null);
        TestButton2.SetSvgIcon(newIcon, _currentSize, _useColorOverride ? Colors.Teal : null);
        TestButton3.SetSvgIcon(newIcon, _currentSize, _useColorOverride ? Colors.Orange : null);

        TestImageButton1.SetSvgIcon(newIcon, _currentSize, _useColorOverride ? Colors.Navy : null);
        TestImageButton2.SetSvgIcon(newIcon, _currentSize, _useColorOverride ? Colors.Maroon : null);
        TestImageButton3.SetSvgIcon(newIcon, _currentSize, _useColorOverride ? Colors.DarkGreen : null);

        TestImage1.SetSvgIcon(newIcon, _currentSize, _useColorOverride ? Colors.Crimson : null);
        TestImage2.SetSvgIcon(newIcon, _currentSize, _useColorOverride ? Colors.DarkBlue : null);
        TestImage3.SetSvgIcon(newIcon, _currentSize, _useColorOverride ? Colors.DarkOrange : null);

        UpdateStatus($"All icons changed to: {newIcon}");
    }

    private void OnToggleColorClicked(object sender, EventArgs e)
    {
        _useColorOverride = !_useColorOverride;
        OnChangeIconsClicked(sender, e); // Apply the color change
        UpdateStatus($"Color override: {(_useColorOverride ? "Enabled" : "Disabled")}");
    }

    private void OnChangeSizeClicked(object sender, EventArgs e)
    {
        _currentSizeIndex = (_currentSizeIndex + 1) % _sizes.Length;
        _currentSize = _sizes[_currentSizeIndex];
        OnChangeIconsClicked(sender, e); // Apply the size change
        UpdateStatus($"Icon size changed to: {_currentSize}px");
    }

    private void UpdateStatus(string message)
    {
        StatusLabel.Text = $"{DateTime.Now:HH:mm:ss} - {message}";
    }
}
