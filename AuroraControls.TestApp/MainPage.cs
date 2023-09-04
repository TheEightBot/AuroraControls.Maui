using System.Reactive.Linq;
using AuroraControls.Gauges;
using AuroraControls.TestApp.ViewModels;
using CommunityToolkit.Maui.Markup;
using ReactiveUI;
using ReactiveUI.Maui;

namespace AuroraControls.TestApp;

public class MainPage : ReactiveContentPage<TestRxViewModel>
{
    public static BindableProperty MvvmToolkitViewModelProperty =
        BindableProperty.Create(nameof(MvvmToolkitViewModel), typeof(TestMvvmToolkitViewModel), typeof(MainPage), default(TestMvvmToolkitViewModel));

    public TestMvvmToolkitViewModel MvvmToolkitViewModel
    {
        get => (TestMvvmToolkitViewModel)GetValue(MvvmToolkitViewModelProperty);
        set => SetValue(MvvmToolkitViewModelProperty, value);
    }

    private NumericEntry _rxNumericEntry;

    private NumericEntry _rxNumericIntEntry;

    private Loading.RainbowRing _rainbowRing;

    private bool _isRainbowAnimating;

    private Loading.MaterialCircular _materialCircular;

    private bool _isMaterialAnimating;

    private Loading.Nofriendo _nofriendo;

    private bool _isNofriendoAnimating;

    private Loading.Waves _waves;

    private bool _isWavesAnimating;

    private Loading.CupertinoActivityIndicator _cai;

    private bool _isCaiAnimating;

    private GradientPillButton _pillButton;

    public MainPage()
    {
        ViewModel = new TestRxViewModel();
        MvvmToolkitViewModel = new TestMvvmToolkitViewModel();

        Content =
            new ScrollView
            {
                Content =
                    new VerticalStackLayout
                    {
                        Padding = 16,
                        Spacing = 16,
                        Children =
                        {
                            new Label
                            {
                                HorizontalOptions = LayoutOptions.Center,
                                VerticalOptions = LayoutOptions.Center,
                                Text = "Welcome to .NET MAUI!",
                            },
                            new Button
                            {
                                BackgroundColor = Colors.Fuchsia,
                            }
                                .SetSvgIcon("splatoon.svg", colorOverride: Colors.White),
                            new SegmentedControl
                            {
                                FontFamily = "Clathing",
                                SegmentControlStyle = SegmentedControlStyle.Cupertino,
                                ForegroundTextColor = Colors.CadetBlue,
                                BackgroundTextColor = Colors.DarkSlateGray,
                                Segments =
                                {
                                    new Segment
                                    {
                                        ForegroundColor = Colors.Lime,
                                        Text = "Test 1",
                                    },
                                    new Segment
                                    {
                                        EmbeddedImageName = "splatoon.svg",
                                        ForegroundColor = Colors.Fuchsia,
                                        Text = "Test 2",
                                    },
                                },
                            },
                            new StyledInputLayout
                            {
                                Placeholder = "My Placeholder With Rounded Rectangle Placeholder Through",
                                BackgroundColor = Colors.Fuchsia,
                                ActiveColor = Colors.Red,
                                InactiveColor = Colors.Green,
                                BorderStyle = ContainerBorderStyle.RoundedRectanglePlaceholderThrough,
                                Content =
                                    new Entry
                                    {
                                        Text = "This is My Entry",
                                        Placeholder = "This is a sample",
                                    },
                            },
                            new StyledInputLayout
                            {
                                Placeholder = "Numeric Entry",
                                ActiveColor = Colors.Red,
                                InactiveColor = Colors.Green,
                                BorderStyle = ContainerBorderStyle.RoundedUnderline,
                                Content =
                                    new NumericEntry
                                    {
                                        Placeholder = "This must be a numeric value...",
                                    },
                            },
                            new StyledInputLayout
                            {
                                Placeholder = "Numeric Entry (Rx UI)",
                                ActiveColor = Colors.Red,
                                InactiveColor = Colors.Green,
                                BorderStyle = ContainerBorderStyle.RoundedUnderline,
                                Content =
                                    new NumericEntry
                                    {
                                        Placeholder = "This must be a numeric value...",
                                    }
                                        .Assign(out _rxNumericEntry),
                            },
                            new StyledInputLayout
                            {
                                Placeholder = "Numeric Int Entry (Rx UI)",
                                ActiveColor = Colors.Red,
                                InactiveColor = Colors.Green,
                                BorderStyle = ContainerBorderStyle.RoundedUnderline,
                                Content =
                                    new NumericEntry
                                    {
                                        Placeholder = "This must be an int value...",
                                        ValueType = NumericEntryValueType.Int,
                                    }
                                        .Assign(out _rxNumericIntEntry),
                            },
                            new StyledInputLayout
                            {
                                Placeholder = "My Picker",
                                BackgroundColor = Colors.Chartreuse,
                                BorderStyle = ContainerBorderStyle.RoundedRectanglePlaceholderThrough,
                                Content =
                                    new Picker
                                    {
                                        ItemsSource =
                                            new[]
                                            {
                                                "Item 1",
                                                "Item 2",
                                                "Item 3",
                                                "Item 4",
                                            },
                                    },
                            },
                            new StyledInputLayout
                            {
                                Placeholder = "My Date Picker",
                                BackgroundColor = Colors.Chartreuse,
                                BorderStyle = ContainerBorderStyle.RoundedRectangle,
                                Content =
                                    new DatePicker
                                    {
                                    },
                            },
                            new StyledInputLayout
                            {
                                Placeholder = "My Editor",
                                BackgroundColor = Colors.Chartreuse,
                                BorderStyle = ContainerBorderStyle.Rectangle,
                                Content =
                                    new Editor
                                    {
                                        Placeholder = "Test Entry",
                                        AutoSize = EditorAutoSizeOption.TextChanges,
                                    },
                            },
                            new LinearGauge
                            {
                                 StartingPercent = 10.1d,
                                 EndingPercent = 40.4d,
                                 ProgressBackgroundColor = Colors.Fuchsia,
                                 ProgressColor = Colors.Chartreuse,
                            },
                            new CircularFillGauge
                            {
                                 ProgressPercentage = 46.1d,
                                 ProgressBackgroundColor = Colors.Fuchsia,
                                 ProgressColor = Colors.Chartreuse,
                            },
                            new CircularGauge
                            {
                                 StartingDegree = 10.1d,
                                 EndingDegree = 90.0d,
                                 ProgressBackgroundColor = Colors.Fuchsia,
                                 ProgressColor = Colors.Chartreuse,
                            },
                            (_rainbowRing = new Loading.RainbowRing
                            {
                                HeightRequest = 120d,
                                BackgroundColor = Colors.DarkTurquoise,
                                GestureRecognizers =
                                {
                                    new TapGestureRecognizer
                                    {
                                        Command =
                                            new Command(
                                                () =>
                                                {
                                                    _isRainbowAnimating = !_isRainbowAnimating;

                                                    if (_isRainbowAnimating)
                                                    {
                                                        _rainbowRing.Start();
                                                        return;
                                                    }

                                                    _rainbowRing.Stop();
                                                }),
                                    },
                                },
                            }),
                            (_materialCircular = new Loading.MaterialCircular
                            {
                                HeightRequest = 120d,
                                BackgroundColor = Colors.Fuchsia,
                                GestureRecognizers =
                                {
                                    new TapGestureRecognizer
                                    {
                                        Command =
                                            new Command(
                                                () =>
                                                {
                                                    _isMaterialAnimating = !_isMaterialAnimating;

                                                    if (_isMaterialAnimating)
                                                    {
                                                        _materialCircular.Start();
                                                        return;
                                                    }

                                                    _materialCircular.Stop();
                                                }),
                                    },
                                },
                            }),
                            (_nofriendo = new Loading.Nofriendo
                            {
                                HeightRequest = 120d,
                                BackgroundColor = Colors.Tomato,
                                GestureRecognizers =
                                {
                                    new TapGestureRecognizer
                                    {
                                        Command =
                                            new Command(
                                                () =>
                                                {
                                                    _isNofriendoAnimating = !_isNofriendoAnimating;

                                                    if (_isNofriendoAnimating)
                                                    {
                                                        _nofriendo.Stop();
                                                        return;
                                                    }

                                                    _nofriendo.Start();
                                                }),
                                    },
                                },
                            }),
                            (_waves = new Loading.Waves
                            {
                                HeightRequest = 120d,
                                BackgroundColor = Colors.Aquamarine,
                                GestureRecognizers =
                                {
                                    new TapGestureRecognizer
                                    {
                                        Command =
                                            new Command(
                                                () =>
                                                {
                                                    _isWavesAnimating = !_isWavesAnimating;

                                                    if (_isWavesAnimating)
                                                    {
                                                        _waves.Stop();
                                                        return;
                                                    }

                                                    _waves.Start();
                                                }),
                                    },
                                },
                            }),
                            (_cai = new Loading.CupertinoActivityIndicator
                            {
                                HeightRequest = 120d,
                                BackgroundColor = Colors.Gold,
                                GestureRecognizers =
                                {
                                    new TapGestureRecognizer
                                    {
                                        Command =
                                            new Command(
                                                () =>
                                                {
                                                    _isCaiAnimating = !_isCaiAnimating;

                                                    if (_isCaiAnimating)
                                                    {
                                                        _cai.Stop();
                                                        return;
                                                    }

                                                    _cai.Start();
                                                }),
                                    },
                                },
                            }),
                            new Tile
                            {
                                EmbeddedImageName = "triforce.svg",
                                ButtonBackgroundColor = Colors.Fuchsia,
                            },
                            new SvgImageView
                            {
                                EmbeddedImageName = "splatoon.svg",
                                OverlayColor = Colors.Chartreuse,
                            },
                            new GradientPillButton
                            {
                                Text = "Gradient Pill Button",
                                ButtonBackgroundStartColor = Colors.Fuchsia,
                                ButtonBackgroundEndColor = Colors.Chartreuse,
                                FontColor = Colors.DarkRed,
                                FontFamily = "Clathing",
                            }
                                .Assign(out _pillButton),
                            new Image()
                                .SetSvgIcon("splatoon.svg", 66, Colors.Red),
                            new CupertinoToggleSwitch(),
                        },
                    },
            };

        this.Bind(ViewModel, vm => vm.NullableDoubleValue, ui => ui._rxNumericEntry.Text);
        this.Bind(ViewModel, vm => vm.NullableIntValue, ui => ui._rxNumericIntEntry.Text);

        Observable
            .FromEventPattern(
                x => this.LayoutChanged += x,
                x => this.LayoutChanged -= x)
            .Do(
                _ =>
                {
                    var locationOnPage = PlatformInfo.GetLocationOfView(_pillButton, this);
                    var locationOnParent = PlatformInfo.GetLocationOfView(_pillButton);

                    System.Diagnostics.Debug.WriteLine($"Location In Page: {locationOnPage}");
                    System.Diagnostics.Debug.WriteLine($"Location In Parent: {locationOnParent}");
                    System.Diagnostics.Debug.WriteLine($"MAUI Frame: {_pillButton.Frame}");
                })
            .Subscribe();
    }
}