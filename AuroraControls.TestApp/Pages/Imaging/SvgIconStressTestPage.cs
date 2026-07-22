// <copyright file="SvgIconStressTestPage.cs" company="TheEightBot">
// Copyright (c) TheEightBot. All rights reserved.
// </copyright>

namespace AuroraControls.TestApp.Pages.Imaging;

/// <summary>
/// Stress harness for the Android disappearing-SVG-icon defect.
/// Hammers navigation (push/pop) and tab switching while a grid of IconCache-driven
/// icons is on screen, then inspects the platform ImageViews to detect icons that
/// have gone blank (null drawable or recycled bitmap).
/// </summary>
public class SvgIconStressTestPage : ContentPage
{
    private static readonly string[] SvgIcons = { "logo.svg", "splatoon.svg", "triforce.svg", "dollar_sign.svg", "more.svg" };

    private static readonly Color?[] IconColors = { null, Colors.MediumPurple, Colors.Teal, Colors.OrangeRed, Colors.SteelBlue };

    private static readonly double[] IconSizes = { 20d, 24d, 32d, 40d };

    private readonly List<VisualElement> _iconHosts = new();

    private readonly Label _statusLabel;
    private readonly Label _resultLabel;
    private readonly Button _stressButton;
    private readonly Switch _cycleTabsSwitch;

    private bool _stressRunning;
    private int _iteration;
    private int _totalBroken;

    public SvgIconStressTestPage()
    {
        Title = "SVG Icon Stress";

        _statusLabel = new Label { Text = "Idle", FontSize = 14 };
        _resultLabel = new Label { Text = "No checks run yet", FontSize = 14, LineBreakMode = LineBreakMode.WordWrap };

        _stressButton = new Button { Text = "Start Auto-Stress" };
        _stressButton.Clicked += OnStressClicked;

        var pushOnceButton = new Button { Text = "Push Child Once" };
        pushOnceButton.Clicked += async (_, _) => await Navigation.PushAsync(new SvgIconStressChildPage(autoPop: false));

        var checkButton = new Button { Text = "Check Icons Now" };
        checkButton.Clicked += (_, _) => RunIconCheck();

        var gcButton = new Button { Text = "Force GC" };
        gcButton.Clicked += (_, _) => ForceGc();

        _cycleTabsSwitch = new Switch { IsToggled = true };

        var iconGrid = BuildIconGrid();

        SetupToolbarItems();

        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = 16,
                Spacing = 12,
                Children =
                {
                    _statusLabel,
                    _resultLabel,
                    _stressButton,
                    pushOnceButton,
                    checkButton,
                    gcButton,
                    new HorizontalStackLayout
                    {
                        Spacing = 8,
                        Children =
                        {
                            new Label { Text = "Cycle tabs during stress", VerticalOptions = LayoutOptions.Center },
                            _cycleTabsSwitch,
                        },
                    },
                    iconGrid,
                },
            },
        };
    }

    private Grid BuildIconGrid()
    {
        const int columns = 5;
        var grid = new Grid { ColumnSpacing = 8, RowSpacing = 8 };

        for (var c = 0; c < columns; c++)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        }

        // 25 icon hosts: a mix of Image and ImageButton with unique size/color combos so
        // each resolves through its own IconCache entry.
        for (var i = 0; i < 25; i++)
        {
            var svg = SvgIcons[i % SvgIcons.Length];
            var color = IconColors[(i / SvgIcons.Length) % IconColors.Length];
            var size = IconSizes[i % IconSizes.Length];

            VisualElement host;

            if (i % 3 == 2)
            {
                var imageButton = new ImageButton
                {
                    WidthRequest = 48,
                    HeightRequest = 48,
                    BackgroundColor = Colors.Transparent,
                };
                imageButton.SetSvgIcon(svg, size, color);
                host = imageButton;
            }
            else
            {
                var image = new Image
                {
                    WidthRequest = 48,
                    HeightRequest = 48,
                    Aspect = Aspect.AspectFit,
                };
                image.SetSvgIcon(svg, size, color);
                host = image;
            }

            _iconHosts.Add(host);

            var row = i / columns;

            if (grid.RowDefinitions.Count <= row)
            {
                grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            }

            grid.Add(host, i % columns, row);
        }

        return grid;
    }

    private void SetupToolbarItems()
    {
        var toolbarItem1 = new ToolbarItem { Text = "T1" };
        toolbarItem1.SetSvgIcon("logo.svg");
        ToolbarItems.Add(toolbarItem1);

        var toolbarItem2 = new ToolbarItem { Text = "T2" };
        toolbarItem2.SetSvgIcon("more.svg", 24d, Colors.MediumPurple);
        ToolbarItems.Add(toolbarItem2);
    }

    private async void OnStressClicked(object? sender, EventArgs e)
    {
        if (_stressRunning)
        {
            _stressRunning = false;
            _stressButton.Text = "Start Auto-Stress";
            return;
        }

        _stressRunning = true;
        _stressButton.Text = "Stop Auto-Stress";
        _iteration = 0;
        _totalBroken = 0;

        try
        {
            while (_stressRunning)
            {
                _iteration++;
                _statusLabel.Text = $"Iteration {_iteration}: pushing child…";

                await Navigation.PushAsync(new SvgIconStressChildPage(autoPop: true));

                // Wait for the child page to appear, render, and pop itself.
                await Task.Delay(900);

                if (_cycleTabsSwitch.IsToggled &&
                    Application.Current?.Windows.FirstOrDefault()?.Page is TabbedPage tabbedPage &&
                    tabbedPage.Children.Count > 1)
                {
                    var original = tabbedPage.CurrentPage;
                    tabbedPage.CurrentPage = tabbedPage.Children.First(p => p != original);
                    await Task.Delay(350);
                    tabbedPage.CurrentPage = original;
                    await Task.Delay(350);
                }

                if (_iteration % 5 == 0)
                {
                    ForceGc();
                    await Task.Delay(150);
                }

                var (ok, broken, detail) = InspectIcons();
                _totalBroken += broken;

                _statusLabel.Text = $"Iteration {_iteration}: {ok} OK, {broken} broken (cumulative broken: {_totalBroken})";

                if (broken > 0)
                {
                    _resultLabel.TextColor = Colors.Red;
                    _resultLabel.Text = $"BROKEN ICONS DETECTED at iteration {_iteration}:\n{detail}";
                    System.Diagnostics.Debug.WriteLine($"[SvgStress] BROKEN at iteration {_iteration}: {detail}");
                    Console.WriteLine($"[SvgStress] BROKEN at iteration {_iteration}: {detail}");
                }
            }
        }
        finally
        {
            _stressRunning = false;
            _stressButton.Text = "Start Auto-Stress";
            _statusLabel.Text = $"Stopped after {_iteration} iterations. Cumulative broken: {_totalBroken}";
        }
    }

    private void RunIconCheck()
    {
        var (ok, broken, detail) = InspectIcons();

        _resultLabel.TextColor = broken > 0 ? Colors.Red : Colors.Green;
        _resultLabel.Text = broken > 0
            ? $"{ok} OK, {broken} BROKEN:\n{detail}"
            : $"All {ok} icons OK";
    }

    private static void ForceGc()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
#if ANDROID
        Java.Lang.JavaSystem.Gc();
#endif
        Console.WriteLine("[SvgStress] Forced GC");
    }

    /// <summary>
    /// Inspects the platform views backing each icon host and reports how many have
    /// visibly lost their image (null drawable or recycled bitmap).
    /// </summary>
    private (int Ok, int Broken, string Detail) InspectIcons()
    {
        var ok = 0;
        var broken = 0;
        var details = new System.Text.StringBuilder();

#if ANDROID
        for (var i = 0; i < _iconHosts.Count; i++)
        {
            var host = _iconHosts[i];

            if (host.Handler?.PlatformView is not Android.Widget.ImageView imageView)
            {
                broken++;
                details.AppendLine($"#{i} ({host.GetType().Name}): no platform ImageView");
                continue;
            }

            var drawable = imageView.Drawable;

            if (drawable is null)
            {
                broken++;
                details.AppendLine($"#{i} ({host.GetType().Name}): drawable is null");
                continue;
            }

            if (drawable is Android.Graphics.Drawables.BitmapDrawable bitmapDrawable)
            {
                var bitmap = bitmapDrawable.Bitmap;

                if (bitmap is null)
                {
                    broken++;
                    details.AppendLine($"#{i} ({host.GetType().Name}): BitmapDrawable.Bitmap is null");
                    continue;
                }

                if (bitmap.IsRecycled)
                {
                    broken++;
                    details.AppendLine($"#{i} ({host.GetType().Name}): bitmap IS RECYCLED");
                    continue;
                }
            }

            ok++;
        }
#else
        ok = _iconHosts.Count;
#endif

        return (ok, broken, details.ToString());
    }
}

/// <summary>
/// Child page used by the stress harness; carries its own set of SVG icons and
/// toolbar items so pushing/popping it exercises image handler create/dispose churn.
/// </summary>
public class SvgIconStressChildPage : ContentPage
{
    private static readonly string[] SvgIcons = { "triforce.svg", "dollar_sign.svg", "more.svg", "logo.svg", "splatoon.svg" };

    private readonly bool _autoPop;
    private bool _popped;

    public SvgIconStressChildPage(bool autoPop)
    {
        _autoPop = autoPop;
        Title = "Stress Child";

        var layout = new VerticalStackLayout { Padding = 16, Spacing = 8 };
        layout.Children.Add(new Label { Text = "Child page with its own icons" });

        var grid = new Grid { ColumnSpacing = 8, RowSpacing = 8 };

        for (var c = 0; c < 5; c++)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        }

        for (var i = 0; i < 15; i++)
        {
            var image = new Image { WidthRequest = 44, HeightRequest = 44, Aspect = Aspect.AspectFit };
            image.SetSvgIcon(SvgIcons[i % SvgIcons.Length], 28d, i % 2 == 0 ? Colors.DarkSlateBlue : null);

            var row = i / 5;

            if (grid.RowDefinitions.Count <= row)
            {
                grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            }

            grid.Add(image, i % 5, row);
        }

        layout.Children.Add(grid);

        var toolbarItem = new ToolbarItem { Text = "Child" };
        toolbarItem.SetSvgIcon("splatoon.svg");
        ToolbarItems.Add(toolbarItem);

        Content = new ScrollView { Content = layout };
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_autoPop && !_popped)
        {
            _popped = true;

            // Let the icons finish their async load/render before popping.
            await Task.Delay(450);

            if (Navigation.NavigationStack.LastOrDefault() == this)
            {
                await Navigation.PopAsync();
            }
        }
    }
}
