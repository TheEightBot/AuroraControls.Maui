// <copyright file="SvgShellTabStressShell.cs" company="TheEightBot">
// Copyright (c) TheEightBot. All rights reserved.
// </copyright>

namespace AuroraControls.TestApp.Pages.Imaging;

/// <summary>
/// Shell-based tab stress scenario for the disappearing-SVG-icon investigation.
/// Swapped in as the window root; its bottom tab bar icons come from the IconCache
/// (the GetDrawableAsync path), and an auto-cycle loop switches tabs continuously
/// so tab icon reloads can be observed under cache-trim and lifecycle churn.
/// </summary>
public class SvgShellTabStressShell : Shell
{
    private static readonly (string Svg, Color Color)[] TabIcons =
    {
        ("triforce.svg", Colors.White),
        ("dollar_sign.svg", Colors.MediumPurple),
        ("splatoon.svg", Colors.Orange),
    };

    private readonly Action _restore;
    private readonly Label _statusLabel;
    private readonly Button _cycleButton;
    private readonly TabBar _tabBar;

    private bool _cycling;
    private int _cycleCount;

    public SvgShellTabStressShell(Action restore)
    {
        _restore = restore;
        Title = "Shell Tab Stress";

        var iconCache = IPlatformApplication.Current?.Services.GetService<IIconCache>();

        _statusLabel = new Label { Text = "Shell tab stress ready", FontSize = 14 };

        _cycleButton = new Button { Text = "Start Tab Cycling" };
        _cycleButton.Clicked += OnCycleClicked;

        var returnButton = new Button { Text = "Return To Main App" };
        returnButton.Clicked += (_, _) =>
        {
            _cycling = false;
            _restore();
        };

        _tabBar = new TabBar();

        for (var i = 0; i < TabIcons.Length; i++)
        {
            var page = BuildTabPage(i);

            var shellContent = new ShellContent
            {
                Title = $"Tab {i + 1}",
                Content = page,
                Route = $"svgstresstab{i}",
            };

            var (svg, color) = TabIcons[i];
            iconCache?
                .ImageSourceFromSvg(svg, 24d, colorOverride: color)
                .AsAsyncSourceFor(src => shellContent.Icon = src);

            _tabBar.Items.Add(shellContent);
        }

        Items.Add(_tabBar);
    }

    private ContentPage BuildTabPage(int index)
    {
        var layout = new VerticalStackLayout { Padding = 16, Spacing = 8 };

        if (index == 0)
        {
            layout.Children.Add(_statusLabel);
            layout.Children.Add(_cycleButton);

            var returnButton = new Button { Text = "Return To Main App" };
            returnButton.Clicked += (_, _) =>
            {
                _cycling = false;
                _restore();
            };
            layout.Children.Add(returnButton);
        }
        else
        {
            layout.Children.Add(new Label { Text = $"Shell tab {index + 1} content" });
        }

        var grid = new Grid { ColumnSpacing = 8, RowSpacing = 8 };

        for (var c = 0; c < 5; c++)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        }

        for (var i = 0; i < 10; i++)
        {
            var image = new Image { WidthRequest = 40, HeightRequest = 40, Aspect = Aspect.AspectFit };
            image.SetSvgIcon(
                TabIcons[(i + index) % TabIcons.Length].Svg,
                26d,
                i % 2 == 0 ? Colors.CadetBlue : Colors.IndianRed);

            var row = i / 5;

            if (grid.RowDefinitions.Count <= row)
            {
                grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            }

            grid.Add(image, i % 5, row);
        }

        layout.Children.Add(grid);

        var page = new ContentPage
        {
            Title = $"Tab {index + 1}",
            Content = new ScrollView { Content = layout },
        };

        var toolbarItem = new ToolbarItem { Text = $"S{index + 1}" };
        toolbarItem.SetSvgIcon("more.svg", 24d, Colors.White);
        page.ToolbarItems.Add(toolbarItem);

        return page;
    }

    private async void OnCycleClicked(object? sender, EventArgs e)
    {
        if (_cycling)
        {
            _cycling = false;
            _cycleButton.Text = "Start Tab Cycling";
            return;
        }

        _cycling = true;
        _cycleButton.Text = "Stop Tab Cycling";
        _cycleCount = 0;

        try
        {
            while (_cycling)
            {
                foreach (var item in _tabBar.Items.ToList())
                {
                    if (!_cycling)
                    {
                        break;
                    }

                    CurrentItem = item;
                    await Task.Delay(500);
                }

                _cycleCount++;
                _statusLabel.Text = $"Completed {_cycleCount} tab cycles";
            }
        }
        finally
        {
            _cycling = false;
            _cycleButton.Text = "Start Tab Cycling";
        }
    }
}
