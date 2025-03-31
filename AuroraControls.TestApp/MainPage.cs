using System.Reactive.Linq;
using AuroraControls.Effects;
using AuroraControls.Gauges;
using AuroraControls.TestApp.ViewModels;
using CommunityToolkit.Maui.Markup;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using ReactiveUI.Maui;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace AuroraControls.TestApp;

public class MainPage : ReactiveContentPage<TestRxViewModel>
{
    public static readonly BindableProperty MvvmToolkitViewModelProperty =
        BindableProperty.Create(nameof(MvvmToolkitViewModel), typeof(TestMvvmToolkitViewModel), typeof(MainPage),
            default(TestMvvmToolkitViewModel));

    public TestMvvmToolkitViewModel MvvmToolkitViewModel
    {
        get => (TestMvvmToolkitViewModel)GetValue(MvvmToolkitViewModelProperty);
        set => SetValue(MvvmToolkitViewModelProperty, value);
    }

    private StyledInputLayout _opacitySil;

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

    private CupertinoTextToggleSwitch _cupertinoToggleSwitch;

    private Button _viewImageProcessingButton;

    private Button _viewCardViewLayoutButton;

    private Button _viewCalendarViewButton;

    private Button _viewToggleIssueButton;

    private SvgImageView _svgImageView;

    private Button _svgImageViewTapped;

    private Button _touchDrawImage;

    private int _imageEffectCounter;

    private CalendarPicker _calendarPicker;

    private Button _clearNullableDatePicker;

    public MainPage(ILogger<TestRxViewModel> logger)
    {
        var val = 123;
        var stuff = new TestMvvmToolkitViewModel();
        logger.LogError($"My Value: {val}\tMy ViewModel: {stuff}");
        logger.LogError("My Value: {val}\tMy ViewModel: {stuff}", val, stuff);

        ViewModel = new TestRxViewModel();
        MvvmToolkitViewModel = new TestMvvmToolkitViewModel();

        this.ToolbarItems.Add(
            new ToolbarItem()
                .SetSvgIcon("more.svg"));

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
                            new Button { Text = "View Image Processing", }
                                .BindClicked(async () => await this.Navigation.PushAsync(new ImageProcessing()))
                                .Assign(out _viewImageProcessingButton),
                            new Button { Text = "View Card View Layout", }
                                .BindClicked(async () => await this.Navigation.PushAsync(new CardViewLayoutPage()))
                                .Assign(out _viewCardViewLayoutButton),
                            new Button { Text = "View Calendar View", }
                                .BindClicked(async () => await this.Navigation.PushAsync(new CalendarViewPage()))
                                .Assign(out _viewCalendarViewButton),
                            new Button { Text = "View touch Draw Image", }
                                .BindClicked(async () => await this.Navigation.PushAsync(new TouchDrawLettersImagePage()))
                                .Assign(out _touchDrawImage),
                            new Button { Text = "View Toggle Issue Page", }
                                .BindClicked(async () => await this.Navigation.PushAsync(new ToggleBoxCollectionViewIssuePage()))
                                .Assign(out _viewToggleIssueButton),
                            new Button { Text = "View Data Grid", }
                                .BindClicked(async () => await this.Navigation.PushAsync(new DataGridPage())),
                            new ToggleBox
                            {
                                ToggledBackgroundColor = Colors.Fuchsia,
                                CheckColor = Colors.Chartreuse,
                                BorderColor = Colors.Chocolate,
                                BackgroundColor = Colors.Aquamarine,
                            },
                            new StyledInputLayout
                            {
                                Command = new Command(() => this.DisplayAlert("Command Tapped", "You have successfully tapped the command", "Great, Thanks!")),
                                Content =
                                    new Entry { Placeholder = "Styled input layout with command", },
                            },
                            new StyledInputLayout
                            {
                                Placeholder = "Nullable Calendar Picker",
                                Content =
                                    new CalendarPicker
                                    {
                                        IsEnabled = false,
                                        TextColor = Colors.Black,
                                        Date = DateTime.Now,
                                    }
                                        .Assign(out this._calendarPicker),
                            },
                            new Button { Text = "Clear Nullable Date Picker", }
                                .Assign(out _clearNullableDatePicker),
                            new StyledInputLayout
                            {
                                Content =
                                    new NPicker.DatePicker { Placeholder = "Nullable NDate Picker", },
                            },
                            new Grid
                            {
                                ColumnDefinitions = Columns.Define(Auto, Star, Auto),
                                RowDefinitions = Rows.Define(Auto, Auto, Auto),
                                Children =
                                {
                                    new CupertinoTextToggleSwitch()
                                        {
                                            EnabledText = "Enabled",
                                            DisabledText = "Disabled",
                                            TrackDisabledColor = Color.FromRgba("#ef361a"),
                                            TrackEnabledColor = Color.FromRgba("#4694f2"),
                                            DisabledFontColor = Colors.White,
                                            EnabledFontColor = Colors.White,
                                        }
                                        .Bind(
                                            CupertinoTextToggleSwitch.IsToggledProperty,
                                            nameof(TestRxViewModel.IsToggled),
                                            mode: BindingMode.TwoWay)
                                        .Row(0).Column(2)
                                        .Assign(out _cupertinoToggleSwitch),
                                    new CupertinoTextToggleSwitch()
                                        {
                                            EnabledText = "Enabled",
                                            DisabledText = "Disabled",
                                            TrackDisabledColor = Color.FromRgba("#ef361a"),
                                            TrackEnabledColor = Color.FromRgba("#4694f2"),
                                            DisabledFontColor = Colors.White,
                                            EnabledFontColor = Colors.White,
                                        }
                                        .Bind(
                                            CupertinoTextToggleSwitch.IsToggledProperty,
                                            nameof(TestRxViewModel.IsToggled),
                                            mode: BindingMode.TwoWay)
                                        .Row(1).Column(2),
                                    new Switch()
                                        .Bind(
                                            Switch.IsToggledProperty,
                                            nameof(TestRxViewModel.IsToggled),
                                            mode: BindingMode.TwoWay)
                                        .Row(2).Column(2),
                                },
                            },
                            new Button { BackgroundColor = Colors.Fuchsia, }
                                .SetSvgIcon("splatoon.svg", colorOverride: Colors.White),
                            new SegmentedControl
                            {
                                FontFamily = "Clathing",
                                SegmentControlStyle = SegmentedControlStyle.Cupertino,
                                ForegroundTextColor = Colors.CadetBlue,
                                BackgroundTextColor = Colors.DarkSlateGray,
                                Segments =
                                {
                                    new Segment { ForegroundColor = Colors.Lime, Text = "Test 1", },
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
                                    Opacity = .25d,
                                    BackgroundColor = Colors.Fuchsia,
                                    ActiveColor = Colors.Red,
                                    InactiveColor = Colors.Green,
                                    PlaceholderColor = Colors.Purple,
                                    BorderStyle = ContainerBorderStyle.RoundedRectanglePlaceholderThrough,
                                    Content =
                                        new Entry
                                        {
                                            Placeholder =
                                                "My Placeholder With Rounded Rectangle Placeholder Through",
                                            Text = "This is My Entry",
                                        },
                                }
                                .Assign(out _opacitySil),
                            new Slider { Value = .5d, Minimum = 0d, Maximum = 1d, }
                                .Bind(nameof(IView.Opacity), source: _opacitySil),
                            new StyledInputLayout
                            {
                                Placeholder = "Numeric Entry",
                                ActiveColor = Colors.Red,
                                InactiveColor = Colors.Green,
                                BorderStyle = ContainerBorderStyle.RoundedUnderline,
                                Content =
                                    new NumericEntry { Placeholder = "This must be a numeric value...", },
                            },
                            new StyledInputLayout
                            {
                                Placeholder = "Numeric Entry (Rx UI)",
                                ActiveColor = Colors.Red,
                                InactiveColor = Colors.Green,
                                BorderStyle = ContainerBorderStyle.RoundedUnderline,
                                Content =
                                    new NumericEntry { Placeholder = "This must be a numeric value...", }
                                        .Assign(out _rxNumericEntry),
                            },
                            new StyledInputLayout
                            {
                                Placeholder = "Numeric Int Entry (Rx UI)",
                                ActiveColor = Colors.Red,
                                InactiveColor = Colors.Green,
                                BorderStyle = ContainerBorderStyle.RoundedUnderline,
                                InternalMargin = new Thickness(16, 8),
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
                                            new[] { "Item 1", "Item 2", "Item 3", "Item 4", },
                                    },
                            },
                            new StyledInputLayout
                            {
                                Placeholder = "My Date Picker",
                                BackgroundColor = Colors.Chartreuse,
                                BorderStyle = ContainerBorderStyle.RoundedRectangle,
                                Content =
                                    new DatePicker { },
                            },
                            new StyledInputLayout
                            {
                                Placeholder = "My Editor",
                                BackgroundColor = Colors.Chartreuse,
                                BorderStyle = ContainerBorderStyle.Rectangle,
                                Content =
                                    new Editor
                                    {
                                        Placeholder = "Test Entry", AutoSize = EditorAutoSizeOption.TextChanges,
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
                            new Tile { EmbeddedImageName = "triforce.svg", ButtonBackgroundColor = Colors.Fuchsia, },
                            new SvgImageView { EmbeddedImageName = "splatoon.svg", OverlayColor = Colors.Chartreuse, }
                                .Assign(out _svgImageView),
                            new Button { Text = "Update Effects" }
                                .Assign(out _svgImageViewTapped),
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

        this._clearNullableDatePicker.Clicked +=
            (sender, args) => { this._calendarPicker.Date = null; };

        this._svgImageViewTapped.Clicked +=
            (sender, args) =>
            {
                this._svgImageView.VisualEffects.Clear();

                var rngesus = new Random(Guid.NewGuid().GetHashCode());

                switch (_imageEffectCounter)
                {
                    case 0:
                        _svgImageView.VisualEffects.Add(
                            new VisualEffects.Pixelate
                            {
                                PixelSize = rngesus.Next(10, 25),
                            });
                        break;
                    case 1:
                        _svgImageView.VisualEffects.Add(new VisualEffects.Sepia());
                        break;
                    case 2:
                        _svgImageView.VisualEffects.Add(new VisualEffects.Grayscale());
                        break;
                    case 3:
                        _svgImageView.VisualEffects.Add(new VisualEffects.BlackAndWhite());
                        break;
                    case 4:
                        _svgImageView.VisualEffects.Add(new VisualEffects.Invert());
                        break;
                    case 5:
                        _svgImageView.VisualEffects.Add(new VisualEffects.HighContrast());
                        break;
                    case 6:
                        _svgImageView.VisualEffects.Add(
                            new VisualEffects.Rotate
                            {
                                RotationDegrees = rngesus.Next(-360, 360),
                            });
                        break;
                    case 7:
                        _svgImageView.VisualEffects.Add(
                            new VisualEffects.Scale
                            {
                                ScaleAmount = (float)(rngesus.Next(0, 2) + rngesus.NextDouble()),
                            });
                        break;
                }

                _imageEffectCounter++;

                if (_imageEffectCounter > 7)
                {
                    _imageEffectCounter = -1;
                }
            };

        this.Bind(
            ViewModel,
            vm => vm.NullableDoubleValue,
            ui => ui._rxNumericEntry.Text,
            x => x?.ToString("N2") ?? string.Empty,
            x => double.TryParse(x, out var parsed) ? parsed : null);

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
