# AuroraControls.Maui TestApp Redesign Plan

> **Branch:** `feature/testapp-redesign`  
> **Created:** December 5, 2025  
> **Status:** 🚧 In Progress

## 📋 Overview

This document outlines the comprehensive redesign of the AuroraControls.Maui TestApp. The goal is to create a premium, modern testing experience that showcases all controls with interactive property editors, following 2025 mobile design trends and Material Design 3 (M3) Expressive guidelines.

---

## 🎯 Design Philosophy

### 2025 Mobile Design Principles
- **Emotion-Driven UX** - Vibrant colors, intuitive motion, and expressive shapes (M3 Expressive)
- **Thumb-Zone Optimization** - Primary actions reachable with one hand
- **Generous White Space** - Clean layouts with ample breathing room
- **Micro-Interactions** - Subtle animations for feedback and delight
- **Dark Mode First** - Design for both light and dark themes
- **Adaptive Components** - Responsive to different screen sizes

### Key Design Decisions
1. **~~AppShell Navigation~~** → **TabbedPage Navigation** - Shell is incompatible with AuroraControls (see critical note below)
2. **Card-Based Layouts** - Controls showcased in elevated cards with shadows
3. **Property Inspector Panel** - Bottom sheet / expandable panel for live property editing
4. **Preview + Code Mode** - Toggle between visual preview and usage examples
5. **Search & Filter** - Quick access to specific controls
6. **NavigationPage Wrappers** - Each tab wrapped in NavigationPage for push/pop navigation

---

## 🚨 CRITICAL: Shell Incompatibility

> **Discovery Date:** December 5, 2025  
> **Issue:** Shell causes app to hang indefinitely on splash screen  
> **Root Cause:** Unknown incompatibility between MAUI Shell and AuroraControls library  
> **Solution:** Use TabbedPage with NavigationPage wrappers instead

### What We Tried:
- Simple Shell with TabBar → App hangs on splash
- Shell with minimal content → App hangs on splash
- Programmatic Shell creation → App hangs on splash
- Removing UseAuroraControls → Shell STILL hangs (not an AuroraControls issue)
- TabbedPage with NavigationPage → ✅ WORKS

### Current Architecture:
```csharp
// App.xaml.cs - Working Implementation
var tabbedPage = new TabbedPage
{
    Children =
    {
        new NavigationPage(new ShowcasePage()) { Title = "Showcase" },
        new NavigationPage(new ControlsListPage()) { Title = "Controls" },
        new NavigationPage(new EffectsPage()) { Title = "Effects" },
        new NavigationPage(new SettingsPage()) { Title = "Settings" }
    }
};
return new Window(tabbedPage);
```

### Navigation Pattern (Use Instead of Shell.GoToAsync):
```csharp
// ✅ CORRECT: Use Navigation.PushAsync
await Navigation.PushAsync(new GradientCircularButtonTestPage());

// ✅ CORRECT: Navigate back
await Navigation.PopAsync();

// ❌ WRONG: Shell navigation (causes hangs)
// await Shell.Current.GoToAsync("someroute");
```

---

## 🗂️ Control Inventory

### Core UI Controls
| Control | Properties Count | Priority | Demo Page |
|---------|-----------------|----------|-----------|
| `ToggleBox` | 12 | High | ✅ Exists |
| `CupertinoToggleSwitch` | 8 | High | ⬜ Create |
| `CupertinoTextToggleSwitch` | 12 | High | ⬜ Create |
| `StyledInputLayout` | 20+ | High | ✅ Exists |
| `NumericEntry` | 8 | Medium | ⬜ Create |
| `CalendarPicker` | 6 | Medium | ⬜ Create |
| `CalendarView` | 25+ | High | ✅ Exists |
| `SegmentedControl` | 12 | High | ⬜ Create |
| `ChipGroup` / `Chip` | 20+ | High | ✅ Exists |

### Button Controls
| Control | Properties Count | Priority | Demo Page |
|---------|-----------------|----------|-----------|
| `GradientPillButton` | 18 | High | ⬜ Create |
| `GradientCircularButton` | 15 | High | ✅ Exists |
| `CupertinoButton` | 10 | Medium | ✅ Exists |
| `Tile` | 20+ | High | ✅ Exists |
| `SvgImageButton` | 8 | Medium | ✅ Exists |

### Image & Graphics Controls
| Control | Properties Count | Priority | Demo Page |
|---------|-----------------|----------|-----------|
| `SvgImageView` | 6 | High | ✅ Exists |
| `TouchDrawLettersImage` | 10+ | Medium | ✅ Exists |
| `SignaturePad` | 8 | Medium | ✅ Exists |
| `ConfettiView` | 12 | Medium | ✅ Exists |
| `CutoutOverlayView` | 8 | Medium | ✅ Exists |

### Progress & Loading Controls
| Control | Properties Count | Priority | Demo Page |
|---------|-----------------|----------|-----------|
| `RainbowRing` | 3 | Medium | ⬜ Create |
| `MaterialCircular` | 4 | Medium | ⬜ Create |
| `Nofriendo` | 5 | Medium | ⬜ Create |
| `Waves` | 4 | Medium | ⬜ Create |
| `CupertinoActivityIndicator` | 4 | Medium | ⬜ Create |
| `StepIndicator` | 15 | High | ✅ Exists |

### Gauge Controls
| Control | Properties Count | Priority | Demo Page |
|---------|-----------------|----------|-----------|
| `LinearGauge` | 8 | High | ⬜ Create |
| `CircularGauge` | 10 | High | ⬜ Create |
| `CircularFillGauge` | 8 | High | ⬜ Create |

### Layout Controls
| Control | Properties Count | Priority | Demo Page |
|---------|-----------------|----------|-----------|
| `WrapLayout` | 6 | Medium | ✅ Exists |
| `CardViewLayout` | 8 | Medium | ✅ Exists |

### Visual Effects
| Effect | Properties Count | Priority | Demo Page |
|--------|-----------------|----------|-----------|
| `Pixelate` | 2 | Medium | ⬜ Group Page |
| `Sepia` | 1 | Medium | ⬜ Group Page |
| `Grayscale` | 1 | Medium | ⬜ Group Page |
| `BlackAndWhite` | 1 | Medium | ⬜ Group Page |
| `Invert` | 1 | Medium | ⬜ Group Page |
| `HighContrast` | 1 | Medium | ⬜ Group Page |
| `Rotate` | 2 | Medium | ⬜ Group Page |
| `Scale` | 2 | Medium | ⬜ Group Page |
| `Brightness` | 2 | Medium | ⬜ Group Page |
| `Contrast` | 2 | Medium | ⬜ Group Page |
| `Hue` | 2 | Medium | ⬜ Group Page |
| `Saturation` | 2 | Medium | ⬜ Group Page |
| `Skew` | 3 | Medium | ⬜ Group Page |
| `ThreeDee` | 4 | Low | ⬜ Group Page |
| `Translate` | 3 | Medium | ⬜ Group Page |
| `Watermark` | 5 | Medium | ⬜ Group Page |

### Image Processing
| Processor | Properties Count | Priority | Demo Page |
|-----------|-----------------|----------|-----------|
| `Blur` | 2 | Medium | ✅ Exists |
| `Circular` | 2 | Medium | ✅ Exists |
| `Grayscale` | 1 | Medium | ✅ Exists |
| `Invert` | 1 | Medium | ✅ Exists |
| `ResizeImage` | 3 | Medium | ✅ Exists |
| `Rotate` | 2 | Medium | ✅ Exists |
| `Scale` | 2 | Medium | ✅ Exists |
| `Sepia` | 1 | Medium | ✅ Exists |
| `Watermark` | 5 | Medium | ✅ Exists |

### Effects (Platform-Specific)
| Effect | Properties Count | Priority | Demo Page |
|--------|-----------------|----------|-----------|
| `KeyboardReturnKeyTypeEffect` | 2 | Low | ✅ Exists |
| `ListViewHideEmptyCellsEffect` | 1 | Low | ✅ Exists |
| `ShowKeyboardDoneButtonEffect` | 1 | Low | ✅ Exists |
| `SafeAreaEffect` | 4 | Medium | ⬜ Create |
| `RoundedCornersEffect` | 2 | Medium | ⬜ Create |
| `ShadowEffect` | 5 | Medium | ⬜ Create |

### Extensions
| Extension | Priority | Demo Page |
|-----------|----------|-----------|
| `SetSvgIcon` | High | ✅ Exists |

---

## 🏗️ New Architecture

### Navigation Structure

```
┌─────────────────────────────────────────────────────────────┐
│                        AppShell                              │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│  ┌─────────────────────────────────────────────────────┐    │
│  │                  ShellContent                        │    │
│  │                                                      │    │
│  │   • Showcase (Home)                                  │    │
│  │   • Controls Browser                                 │    │
│  │   • Individual Control Pages (pushed via routes)     │    │
│  │   • Settings                                         │    │
│  │                                                      │    │
│  └─────────────────────────────────────────────────────┘    │
│                                                              │
├─────────────────────────────────────────────────────────────┤
│  🏠 Showcase  │  🎨 Controls  │  ⚡ Effects  │  ⚙️ Settings  │
│  (Tab 1)      │  (Tab 2)      │  (Tab 3)     │  (Tab 4)      │
└─────────────────────────────────────────────────────────────┘
```

### AppShell Implementation

```xml
<!-- AppShell.xaml -->
<Shell xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
       xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
       xmlns:pages="clr-namespace:AuroraControls.TestApp.Pages"
       x:Class="AuroraControls.TestApp.AppShell">

    <!-- IMPORTANT: Shell.FlyoutBehavior must be Disabled for bottom tabs -->
    <Shell.FlyoutBehavior>Disabled</Shell.FlyoutBehavior>

    <TabBar>
        <ShellContent 
            Title="Showcase" 
            Icon="icon_home.png"
            Route="showcase"
            ContentTemplate="{DataTemplate pages:ShowcasePage}" />
        
        <ShellContent 
            Title="Controls" 
            Icon="icon_controls.png"
            Route="controls"
            ContentTemplate="{DataTemplate pages:ControlsListPage}" />
        
        <ShellContent 
            Title="Effects" 
            Icon="icon_effects.png"
            Route="effects"
            ContentTemplate="{DataTemplate pages:EffectsPage}" />
        
        <ShellContent 
            Title="Settings" 
            Icon="icon_settings.png"
            Route="settings"
            ContentTemplate="{DataTemplate pages:SettingsPage}" />
    </TabBar>

</Shell>
```

### ⚠️ Critical: Route Registration

**All detail pages MUST be registered in AppShell.xaml.cs constructor:**

```csharp
// AppShell.xaml.cs
public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        
        // ═══════════════════════════════════════════════════════════════
        // ROUTE REGISTRATION - Required for Shell.GoToAsync() navigation
        // ═══════════════════════════════════════════════════════════════
        
        // Button Controls
        Routing.RegisterRoute("gradientpillbutton", typeof(GradientPillButtonDemoPage));
        Routing.RegisterRoute("gradientcircularbutton", typeof(GradientCircularButtonDemoPage));
        Routing.RegisterRoute("cupertinobutton", typeof(CupertinoButtonDemoPage));
        Routing.RegisterRoute("tile", typeof(TileDemoPage));
        Routing.RegisterRoute("svgimagebutton", typeof(SvgImageButtonDemoPage));
        
        // Input Controls
        Routing.RegisterRoute("styledinputlayout", typeof(StyledInputLayoutDemoPage));
        Routing.RegisterRoute("numericentry", typeof(NumericEntryDemoPage));
        Routing.RegisterRoute("togglebox", typeof(ToggleBoxDemoPage));
        Routing.RegisterRoute("cupertinotoggleswitch", typeof(CupertinoToggleSwitchDemoPage));
        Routing.RegisterRoute("segmentedcontrol", typeof(SegmentedControlDemoPage));
        
        // Calendar Controls
        Routing.RegisterRoute("calendarview", typeof(CalendarViewDemoPage));
        Routing.RegisterRoute("calendarpicker", typeof(CalendarPickerDemoPage));
        
        // Chip Controls
        Routing.RegisterRoute("chipgroup", typeof(ChipGroupDemoPage));
        
        // Image Controls
        Routing.RegisterRoute("svgimageview", typeof(SvgImageViewDemoPage));
        Routing.RegisterRoute("touchdraw", typeof(TouchDrawDemoPage));
        Routing.RegisterRoute("signaturepad", typeof(SignaturePadDemoPage));
        Routing.RegisterRoute("cutoutoverlay", typeof(CutoutOverlayDemoPage));
        
        // Progress Controls
        Routing.RegisterRoute("gauges", typeof(GaugesDemoPage));
        Routing.RegisterRoute("stepindicator", typeof(StepIndicatorDemoPage));
        Routing.RegisterRoute("loadingindicators", typeof(LoadingIndicatorsDemoPage));
        
        // Animation Controls
        Routing.RegisterRoute("confetti", typeof(ConfettiDemoPage));
        Routing.RegisterRoute("visualeffects", typeof(VisualEffectsDemoPage));
        
        // Layout Controls
        Routing.RegisterRoute("wraplayout", typeof(WrapLayoutDemoPage));
        Routing.RegisterRoute("cardviewlayout", typeof(CardViewLayoutDemoPage));
        
        // Effects
        Routing.RegisterRoute("imageprocessing", typeof(ImageProcessingDemoPage));
        Routing.RegisterRoute("platformeffects", typeof(PlatformEffectsDemoPage));
    }
}
```

### Navigation Patterns

```csharp
// ═══════════════════════════════════════════════════════════════
// NAVIGATION BEST PRACTICES FOR SHELL
// ═══════════════════════════════════════════════════════════════

// ✅ CORRECT: Navigate to registered route (relative)
await Shell.Current.GoToAsync("gradientpillbutton");

// ✅ CORRECT: Navigate with absolute path 
await Shell.Current.GoToAsync("//controls/gradientpillbutton");

// ✅ CORRECT: Navigate back
await Shell.Current.GoToAsync("..");

// ✅ CORRECT: Pass parameters via query string
await Shell.Current.GoToAsync($"gradientpillbutton?preset=vibrant");

// ✅ CORRECT: Pass complex objects via dictionary
var navigationParameter = new Dictionary<string, object>
{
    { "ControlInfo", selectedControl }
};
await Shell.Current.GoToAsync("gradientpillbutton", navigationParameter);

// ❌ WRONG: Using Navigation.PushAsync with Shell (may cause issues)
// await Navigation.PushAsync(new SomePage());

// ❌ WRONG: Unregistered routes will throw exceptions
// await Shell.Current.GoToAsync("unregisteredpage");
```

### Receiving Navigation Parameters

```csharp
// Demo page must implement IQueryAttributable for parameters
[QueryProperty(nameof(Preset), "preset")]
public partial class GradientPillButtonDemoPage : ContentPage, IQueryAttributable
{
    public string Preset { get; set; }
    
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("ControlInfo", out var controlInfo))
        {
            // Handle complex object parameter
        }
    }
}
```

### Page Hierarchy

```
📱 AppShell
├── 🏠 TabBar → ShellContent (Route: "showcase")
│   └── ShowcasePage
│       └── → GoToAsync("gradientpillbutton") etc.
│
├── 🎨 TabBar → ShellContent (Route: "controls")
│   └── ControlsListPage
│       ├── 🔘 Buttons & Actions
│       │   ├── → GoToAsync("gradientpillbutton")
│       │   ├── → GoToAsync("gradientcircularbutton")
│       │   ├── → GoToAsync("cupertinobutton")
│       │   ├── → GoToAsync("tile")
│       │   └── → GoToAsync("svgimagebutton")
│       │
│       ├── 📝 Input Controls
│       │   ├── → GoToAsync("styledinputlayout")
│       │   ├── → GoToAsync("numericentry")
│       │   ├── → GoToAsync("togglebox")
│       │   ├── → GoToAsync("cupertinotoggleswitch")
│       │   └── → GoToAsync("segmentedcontrol")
│       │
│       ├── 📅 Date & Calendar
│       │   ├── → GoToAsync("calendarview")
│       │   └── → GoToAsync("calendarpicker")
│       │
│       ├── 🏷️ Chips & Tags
│       │   └── → GoToAsync("chipgroup")
│       │
│       ├── 🖼️ Images & Graphics
│       │   ├── → GoToAsync("svgimageview")
│       │   ├── → GoToAsync("touchdraw")
│       │   ├── → GoToAsync("signaturepad")
│       │   └── → GoToAsync("cutoutoverlay")
│       │
│       ├── 📊 Gauges & Progress
│       │   ├── → GoToAsync("gauges")
│       │   └── → GoToAsync("stepindicator")
│       │
│       ├── ⏳ Loading Indicators
│       │   └── → GoToAsync("loadingindicators")
│       │
│       ├── 🎉 Animations & Effects
│       │   ├── → GoToAsync("confetti")
│       │   └── → GoToAsync("visualeffects")
│       │
│       └── 📐 Layout Controls
│           ├── → GoToAsync("wraplayout")
│           └── → GoToAsync("cardviewlayout")
│
├── ⚡ TabBar → ShellContent (Route: "effects")
│   └── EffectsPage
│       ├── → GoToAsync("imageprocessing")
│       └── → GoToAsync("platformeffects")
│
└── ⚙️ TabBar → ShellContent (Route: "settings")
    └── SettingsPage
        ├── Theme Toggle (Light/Dark)
        ├── Accent Color Picker
        └── App Info
```

---

## 🎨 Design System

### Color Palette (M3 Expressive Inspired)

```
Primary Colors:
├── Aurora Purple:    #7C3AED (Primary)
├── Aurora Pink:      #EC4899 (Secondary)
├── Aurora Blue:      #3B82F6 (Tertiary)
└── Aurora Teal:      #14B8A6 (Accent)

Surface Colors (Light):
├── Background:       #FAFAFA
├── Surface:          #FFFFFF
├── Surface Variant:  #F3F4F6
└── Outline:          #E5E7EB

Surface Colors (Dark):
├── Background:       #0F0F0F
├── Surface:          #1A1A1A
├── Surface Variant:  #262626
└── Outline:          #404040

Semantic Colors:
├── Success:          #22C55E
├── Warning:          #F59E0B
├── Error:            #EF4444
└── Info:             #3B82F6
```

### Typography Scale

```
Display:      28sp, Bold
Headline:     24sp, SemiBold
Title:        20sp, SemiBold
Body Large:   16sp, Regular
Body:         14sp, Regular
Label:        12sp, Medium
Caption:      11sp, Regular
```

### Spacing System

```
XS:   4dp
S:    8dp
M:    16dp
L:    24dp
XL:   32dp
XXL:  48dp
```

### Component Tokens

```
Card Elevation:       8dp
Card Corner Radius:   16dp
Button Corner Radius: 12dp
Input Corner Radius:  8dp
Bottom Sheet Radius:  24dp (top)
Navigation Bar Height: 80dp
```

---

## 📐 Control Demo Page Template

Each control demo page will follow a consistent structure:

```
┌─────────────────────────────────────────────┐
│ ← [Control Name]               [Code] [?]  │  ← Header with navigation
├─────────────────────────────────────────────┤
│                                             │
│  ┌───────────────────────────────────────┐  │
│  │                                       │  │
│  │        LIVE PREVIEW AREA              │  │  ← Interactive preview
│  │                                       │  │
│  │         [Control Here]                │  │
│  │                                       │  │
│  └───────────────────────────────────────┘  │
│                                             │
│  ┌───────────────────────────────────────┐  │
│  │ Presets                         ▼     │  │  ← Quick preset selector
│  │ [Default] [Vibrant] [Minimal] [Custom]│  │
│  └───────────────────────────────────────┘  │
│                                             │
├─────────────────────────────────────────────┤
│  Properties                          ⌃     │  ← Expandable property panel
│                                             │
│  ▸ Colors                                  │
│    Primary Color     [████████] #7C3AED    │
│    Secondary Color   [████████] #EC4899    │
│                                             │
│  ▸ Dimensions                              │
│    Width            [═══════●═══] 200      │
│    Height           [═══●═══════] 48       │
│    Corner Radius    [═════●═════] 12       │
│                                             │
│  ▸ Behavior                                │
│    IsEnabled        [●═══════════] ✓       │
│    IsToggled        [═══════════●] ✗       │
│                                             │
│  ▸ Typography                              │
│    Font Size        [═════●═════] 16       │
│    Font Family      [Picker     ▼]         │
│                                             │
└─────────────────────────────────────────────┘
```

### Property Editor Components

| Property Type | Editor Component |
|--------------|------------------|
| `Color` | Color picker with hex input |
| `double` / `float` | Slider with numeric input |
| `int` | Stepper with numeric input |
| `bool` | Toggle switch |
| `enum` | Segmented control or picker |
| `string` | Text entry |
| `Thickness` | 4-value input (LTRB) |
| `Point` | 2-value input (X, Y) |
| `Size` | 2-value input (W, H) |

---

## 📱 Showcase Page Design

The Showcase page serves as the app's hero landing page:

```
┌─────────────────────────────────────────────┐
│              Aurora Controls                │
│          Beautiful MAUI Controls            │
├─────────────────────────────────────────────┤
│                                             │
│  ┌───────────────────────────────────────┐  │
│  │  🎨 Featured Control of the Day       │  │
│  │  ┌─────────────────────────────────┐  │  │
│  │  │                                 │  │  │
│  │  │   [Animated Control Preview]    │  │  │
│  │  │                                 │  │  │
│  │  └─────────────────────────────────┘  │  │
│  │  Gradient Pill Button                 │  │
│  │  Create beautiful gradient buttons... │  │
│  │                      [Explore →]      │  │
│  └───────────────────────────────────────┘  │
│                                             │
│  Popular Controls                           │
│  ┌─────────┐ ┌─────────┐ ┌─────────┐       │
│  │ ToggleB │ │ CalView │ │ Confett │       │
│  │   [✓]   │ │  [📅]   │ │   [🎉]  │       │
│  └─────────┘ └─────────┘ └─────────┘       │
│                                             │
│  Categories                                 │
│  ┌─────────────────────────────────────┐   │
│  │ 🔘 Buttons & Actions          (8) → │   │
│  ├─────────────────────────────────────┤   │
│  │ 📝 Input Controls            (5) → │   │
│  ├─────────────────────────────────────┤   │
│  │ 📊 Gauges & Progress         (4) → │   │
│  ├─────────────────────────────────────┤   │
│  │ 🎉 Animations & Effects      (3) → │   │
│  └─────────────────────────────────────┘   │
│                                             │
└─────────────────────────────────────────────┘
```

---

## ✅ Implementation Phases

### Phase 1: Foundation & Infrastructure ✅
- [x] **1.1** Create new folder structure for redesigned app
- [x] **1.2** Implement design system (colors, typography, spacing)
- [x] **1.3** Create base classes for demo pages
  - [x] `ControlDemoPageBase` - Common functionality
  - [x] `PropertyEditorFactory` - Reusable property editing UI
- [x] **1.4** ~~Implement AppShell with TabBar~~ → Implement TabbedPage (Shell incompatible)
- [x] **1.5** Create theme service (light/dark mode)

### Phase 2: Core Infrastructure Components ✅
- [x] **2.1** Create `ColorPickerEditor` for property editing
- [x] **2.2** Create `SliderEditor` for numeric properties
- [x] **2.3** Create `ExpandableSection` for property groups
- [x] **2.4** Create `PresetSelector` for quick configurations
- [x] **2.5** Create `CodeViewer` for code examples
- [x] **2.6** Create `ToggleEditor` for boolean properties
- [x] **2.7** Create `ValueConverters` (StringNotEmpty, InverseBool, etc.)

### Phase 3: Showcase & Navigation ✅
- [x] **3.1** Implement `ShowcasePage` with featured controls
- [x] **3.2** Implement `ControlsListPage` with categories
- [x] **3.3** Implement `EffectsPage` 
- [x] **3.4** Implement `SettingsPage`
- [x] **3.5** Add search functionality

### Phase 4: Button & Action Controls ✅
- [x] **4.1** Create `GradientPillButtonDemoPage`
- [x] **4.2** Create `GradientCircularButtonDemoPage`
- [x] **4.3** Refactor `CupertinoButtonTestPage`
- [x] **4.4** Refactor `TileTestPage`
- [x] **4.5** Refactor `SvgImageButtonTestPage`

### Phase 5: Input Controls ✅
- [x] **5.1** Refactor `StyledInputLayoutTestPage`
- [x] **5.2** Create `NumericEntryDemoPage`
- [x] **5.3** Refactor `ToggleBoxTestPage`
- [x] **5.4** Create `CupertinoToggleSwitchDemoPage`
- [x] **5.5** Create `SegmentedControlDemoPage`

### Phase 6: Calendar & Date Controls ✅
- [x] **6.1** Refactor `CalendarViewPage`
- [x] **6.2** Create `CalendarPickerDemoPage`

### Phase 7: Chips & Tags ✅
- [x] **7.1** Refactor `ChipGroupPage`

### Phase 8: Image & Graphics Controls
- [ ] **8.1** Refactor `SvgImageViewTestPage`
- [ ] **8.2** Refactor `TouchDrawLettersImagePage`
- [ ] **8.3** Refactor `SignaturePadPage`
- [ ] **8.4** Refactor `CutoutOverlayViewTestPage`

### Phase 9: Gauges & Progress
- [ ] **9.1** Create `GaugesDemoPage` (all 3 gauges)
- [ ] **9.2** Refactor `StepIndicatorTestPage`

### Phase 10: Loading Indicators
- [ ] **10.1** Create `LoadingIndicatorsDemoPage` (all 5 loaders)

### Phase 11: Animations & Effects
- [ ] **11.1** Refactor `ConfettiViewTestPage`
- [ ] **11.2** Create `VisualEffectsDemoPage`

### Phase 12: Layout Controls
- [ ] **12.1** Refactor `WrapLayoutTestPage`
- [ ] **12.2** Refactor `CardViewLayoutPage`

### Phase 13: Image Processing
- [ ] **13.1** Refactor `ImageProcessing` page

### Phase 14: Platform Effects
- [ ] **14.1** Create `PlatformEffectsDemoPage`
- [ ] **14.2** Consolidate keyboard effects
- [ ] **14.3** Create safe area demo

### Phase 15: Polish & Final Touches
- [ ] **15.1** Add animations and transitions
- [ ] **15.2** Implement haptic feedback
- [ ] **15.3** Final theme polish
- [ ] **15.4** Performance optimization
- [ ] **15.5** Documentation and code comments

---

## 📁 New Folder Structure

```
AuroraControls.TestApp/
├── App.xaml
├── App.xaml.cs
├── AppShell.xaml                    ← Shell with TabBar navigation
├── AppShell.xaml.cs                 ← Route registration here!
├── MauiProgram.cs
│
├── Themes/
│   ├── Colors.xaml
│   ├── Styles.xaml
│   ├── LightTheme.xaml
│   └── DarkTheme.xaml
│
├── Services/
│   ├── IThemeService.cs
│   ├── ThemeService.cs
│   └── INavigationService.cs
│
├── Controls/
│   ├── PropertyEditors/
│   │   ├── ColorPickerEditor.xaml
│   │   ├── SliderEditor.xaml
│   │   ├── ToggleEditor.xaml
│   │   ├── EnumPickerEditor.xaml
│   │   └── TextEditor.xaml
│   │
│   ├── ExpandableSection.xaml
│   ├── PresetSelector.xaml
│   ├── ControlCard.xaml
│   └── CategoryCard.xaml
│
├── Pages/
│   ├── Base/
│   │   └── ControlDemoPageBase.cs
│   │
│   ├── ShowcasePage.xaml            ← Route: "showcase" (in TabBar)
│   ├── ControlsListPage.xaml        ← Route: "controls" (in TabBar)
│   ├── EffectsPage.xaml             ← Route: "effects" (in TabBar)
│   ├── SettingsPage.xaml            ← Route: "settings" (in TabBar)
│   │
│   ├── Buttons/                     ← All routes registered in AppShell.xaml.cs
│   │   ├── GradientPillButtonDemoPage.xaml
│   │   ├── GradientCircularButtonDemoPage.xaml
│   │   ├── CupertinoButtonDemoPage.xaml
│   │   ├── TileDemoPage.xaml
│   │   └── SvgImageButtonDemoPage.xaml
│   │
│   ├── Inputs/
│   │   ├── StyledInputLayoutDemoPage.xaml
│   │   ├── NumericEntryDemoPage.xaml
│   │   ├── ToggleBoxDemoPage.xaml
│   │   ├── CupertinoToggleSwitchDemoPage.xaml
│   │   └── SegmentedControlDemoPage.xaml
│   │
│   ├── Calendar/
│   │   ├── CalendarViewDemoPage.xaml
│   │   └── CalendarPickerDemoPage.xaml
│   │
│   ├── Chips/
│   │   └── ChipGroupDemoPage.xaml
│   │
│   ├── Images/
│   │   ├── SvgImageViewDemoPage.xaml
│   │   ├── TouchDrawDemoPage.xaml
│   │   ├── SignaturePadDemoPage.xaml
│   │   └── CutoutOverlayDemoPage.xaml
│   │
│   ├── Progress/
│   │   ├── GaugesDemoPage.xaml
│   │   ├── StepIndicatorDemoPage.xaml
│   │   └── LoadingIndicatorsDemoPage.xaml
│   │
│   ├── Animations/
│   │   ├── ConfettiDemoPage.xaml
│   │   └── VisualEffectsDemoPage.xaml
│   │
│   ├── Layouts/
│   │   ├── WrapLayoutDemoPage.xaml
│   │   └── CardViewLayoutDemoPage.xaml
│   │
│   └── Effects/
│       ├── ImageProcessingDemoPage.xaml
│       └── PlatformEffectsDemoPage.xaml
│
├── ViewModels/
│   ├── Base/
│   │   └── ControlDemoViewModelBase.cs
│   ├── ShowcaseViewModel.cs
│   ├── ControlsListViewModel.cs
│   └── [ControlName]ViewModel.cs (for each control)
│
├── Models/
│   ├── ControlCategory.cs
│   ├── ControlInfo.cs
│   └── PropertyPreset.cs
│
├── Resources/
│   ├── Images/
│   │   ├── icon_home.png            ← Tab icons
│   │   ├── icon_controls.png
│   │   ├── icon_effects.png
│   │   └── icon_settings.png
│   ├── Fonts/
│   └── Raw/
│
└── Platforms/
    └── [existing platform folders]
```

---

## 🔄 Commit Strategy

Each phase should result in one or more commits following this pattern:

```
feat(testapp): [Phase X.Y] Brief description

- Detailed change 1
- Detailed change 2
- Detailed change 3
```

Example commits:
```
feat(testapp): [Phase 1.1] Create new folder structure
feat(testapp): [Phase 1.2] Implement design system resources
feat(testapp): [Phase 2.1] Add ColorPickerControl component
feat(testapp): [Phase 4.1] Create GradientPillButton demo page
```

---

## 📊 Progress Tracking

| Phase | Name | Status | Progress |
|-------|------|--------|----------|
| 1 | Foundation & Infrastructure | ✅ Complete | 5/5 |
| 2 | Core Infrastructure Components | ✅ Complete | 7/7 |
| 3 | Showcase & Navigation | ✅ Complete | 5/5 |
| 4 | Button & Action Controls | ✅ Complete | 5/5 |
| 5 | Input Controls | ✅ Complete | 5/5 |
| 6 | Calendar & Date Controls | ✅ Complete | 2/2 |
| 7 | Chips & Tags | ⬜ Not Started | 0/1 |
| 8 | Image & Graphics Controls | ⬜ Not Started | 0/4 |
| 9 | Gauges & Progress | ⬜ Not Started | 0/2 |
| 10 | Loading Indicators | ⬜ Not Started | 0/1 |
| 11 | Animations & Effects | ⬜ Not Started | 0/2 |
| 12 | Layout Controls | ⬜ Not Started | 0/2 |
| 13 | Image Processing | ⬜ Not Started | 0/1 |
| 14 | Platform Effects | ⬜ Not Started | 0/3 |
| 15 | Polish & Final Touches | ⬜ Not Started | 0/5 |

**Overall Progress: 29/50 tasks (58%)**

---

## 🚀 Getting Started

To begin implementation:

1. Ensure you're on the `feature/testapp-redesign` branch
2. Start with Phase 1: Foundation & Infrastructure
3. Update this document as you complete each task
4. Create commits after completing logical chunks of work
5. Use the design system consistently across all pages

---

## 📝 Notes & Decisions Log

| Date | Decision | Rationale |
|------|----------|-----------|
| 2025-12-05 | Use AppShell with TabBar | Modern MAUI pattern, better integration with platform navigation |
| 2025-12-05 | Register ALL routes in AppShell constructor | Shell.GoToAsync() requires explicit route registration |
| 2025-12-05 | Use relative routes for navigation | Simpler than absolute paths, works within Shell context |
| 2025-12-05 | Bottom sheet for properties | Maximizes preview space, follows M3 patterns |
| 2025-12-05 | Group loading indicators | Similar controls, reduces navigation depth |
| 2025-12-05 | Dark mode first | Matches premium app trends, easier on eyes |
| 2025-12-05 | Avoid Navigation.PushAsync | Use Shell.Current.GoToAsync() for consistent Shell navigation |

---

## ⚠️ Shell Navigation Gotchas

### Common Issues and Solutions

| Issue | Cause | Solution |
|-------|-------|----------|
| `RouteNotFoundException` | Route not registered | Add `Routing.RegisterRoute()` in AppShell constructor |
| Back button doesn't work | Using wrong navigation | Use `Shell.Current.GoToAsync("..")` instead of `Navigation.PopAsync()` |
| Tab bar disappears | Navigating with absolute path | Use relative routes or ensure path includes tab |
| Page appears twice | Mixing Shell and NavigationPage | Stick to Shell navigation only |
| Parameters not received | Missing `IQueryAttributable` | Implement interface on target page |

### Route Naming Convention

```
✅ Use lowercase, no spaces: "gradientpillbutton"
✅ Use simple names: "togglebox"
❌ Avoid slashes in route names: "buttons/gradientpill"
❌ Avoid special characters: "gradient-pill-button"
```

---

*Last Updated: December 5, 2025*
