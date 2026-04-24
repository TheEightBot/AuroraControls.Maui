# Numeric Value Converters Implementation Plan

## Overview

This document outlines the plan for creating a flexible, resilient, and modular set of value converters for formatting numeric values in .NET MAUI applications. These converters will work with standard entry controls (Entry, Editor, etc.) and provide formatting capabilities similar to Syncfusion's NumericEntry control.

---

## Feature Set (Based on Syncfusion NumericEntry Formatting)

The converters will support the following formatting capabilities:

### Core Formatting Features
- **Currency Format** (C, C0, C2, etc.) - Format values as currency with culture support
- **Percent Format** (P, P0, P2, etc.) - Format values as percentages
- **Decimal/Numeric Format** (N, N0, N2, etc.) - Format values with thousand separators
- **Custom Format Strings** - Support for standard .NET numeric format strings
- **Integer Digit Formatting** - Control minimum number of integer digits (e.g., `00000.00`)
- **Fractional Digit Formatting** - Control minimum/maximum decimal places
- **Culture Support** - Full localization support via CultureInfo

### Advanced Features
- **Percent Display Mode** - `Value` (show actual value) vs `Compute` (multiply by 100)
- **Maximum Decimal Digits** - Limit the number of decimal digits displayed
- **Null Value Handling** - Configurable placeholder for null/empty values
- **Format Specifiers** - Support `0` (zero placeholder) and `#` (digit placeholder)

---

## Architecture

### Design Principles
1. **Single Responsibility** - Each converter handles one specific formatting concern
2. **Composable** - Converters can be chained using `MultiBinding` or wrapper converters
3. **Configurable** - Properties for customization without subclassing
4. **Culture-Aware** - All converters respect CultureInfo for localization
5. **Bidirectional** - Support both `Convert` and `ConvertBack` for two-way binding
6. **Type-Safe** - Handle multiple numeric types (double, decimal, float, int, long)
7. **Resilient** - Graceful error handling with sensible defaults

### Converter Classes

```
AuroraControls/
└── Converters/
    ├── NumericFormattingConverterBase.cs    # Abstract base class
    ├── CurrencyConverter.cs                  # Currency formatting (C format)
    ├── PercentConverter.cs                   # Percentage formatting (P format)
    ├── DecimalFormatConverter.cs             # Decimal/Numeric formatting (N format)
    ├── CustomNumericFormatConverter.cs       # Custom format strings
    ├── NumericPrecisionConverter.cs          # Control decimal precision
    ├── NumericToStringConverter.cs           # Simple number-to-string conversion
    └── NumericConverterOptions.cs            # Shared options/configuration
```

---

## Implementation Phases

### Phase 1: Foundation & Base Infrastructure
**Status:** ✅ Completed

#### Tasks:
- [x] Create new branch `feature/numeric-value-converters`
- [x] Create `Converters` folder in `AuroraControlsMaui` project
- [x] Implement `NumericConverterOptions` class with shared configuration
- [x] Implement `NumericFormattingConverterBase` abstract class
  - [x] Common type conversion logic (double, decimal, float, int, long)
  - [x] Null value handling
  - [x] CultureInfo support
  - [x] Error handling with fallback values
- [ ] Add unit test project or test file scaffolding (deferred to Phase 5)

#### Deliverables:
- ✅ `NumericConverterOptions.cs`
- ✅ `NumericFormattingConverterBase.cs`
- ✅ Basic project structure

---

### Phase 2: Core Format Converters
**Status:** ✅ Completed

#### Tasks:
- [x] Implement `CurrencyConverter`
  - [x] Support format strings: C, C0, C1, C2, etc.
  - [x] Currency symbol customization
  - [x] Culture-based currency formatting
  - [x] ConvertBack parsing with currency symbol stripping
- [x] Implement `PercentConverter`
  - [x] Support format strings: P, P0, P1, P2, etc.
  - [x] `PercentDisplayMode` enum: `Value` vs `Compute`
  - [x] ConvertBack with percent handling
- [x] Implement `DecimalFormatConverter`
  - [x] Support format strings: N, N0, N1, N2, etc.
  - [x] Thousand separator support
  - [x] Culture-based decimal separator

#### Deliverables:
- ✅ `CurrencyConverter.cs`
- ✅ `PercentConverter.cs`
- ✅ `DecimalFormatConverter.cs`

---

### Phase 3: Advanced Format Converters
**Status:** ✅ Completed

#### Tasks:
- [x] Implement `CustomNumericFormatConverter`
  - [x] Support custom format patterns (e.g., `$00.00##`, `00.000%`)
  - [x] Support `0` (zero placeholder) format specifier
  - [x] Support `#` (digit placeholder) format specifier
  - [x] Prefix/suffix support
- [x] Implement `NumericPrecisionConverter`
  - [x] `MinimumIntegerDigits` property
  - [x] `MinimumFractionDigits` property
  - [x] `MaximumFractionDigits` property
  - [x] Rounding mode options
- [x] Implement `NumericToStringConverter`
  - [x] Lightweight conversion with minimal formatting
  - [x] Optional format string via parameter

#### Deliverables:
- ✅ `CustomNumericFormatConverter.cs`
- ✅ `NumericPrecisionConverter.cs`
- ✅ `NumericToStringConverter.cs`

---

### Phase 4: Behaviors & Extensions (Optional Enhancement)
**Status:** ✅ Completed

#### Tasks:
- [x] Create `NumericEntryBehavior` for automatic formatting on focus lost
- [x] Create attached properties for easy XAML configuration
- [x] Create markup extensions for common formats
- [ ] Integration with existing `StyledInputLayout` (deferred - can be added later)

#### Deliverables:
- ✅ `NumericEntryBehavior.cs`
- ✅ `NumericFormatting.cs` (attached properties)
- ✅ `NumericFormatExtension.cs` (markup extension)

---

### Phase 5: Testing & Documentation
**Status:** ✅ Completed

#### Tasks:
- [x] Create test page in TestApp demonstrating all converters
- [x] Add comprehensive XML documentation comments
- [x] Create usage examples in documentation
- [x] Edge case testing:
  - [x] Null/empty values
  - [x] Invalid input strings
  - [x] Overflow/underflow values
  - [x] Different cultures
  - [x] Different numeric types
- [ ] Performance testing with large lists (deferred - manual testing recommended)

#### Deliverables:
- ✅ `NumericConvertersTestPage.xaml`
- ✅ `NumericConvertersTestPage.xaml.cs`
- ✅ Updated plan documentation

---

## Detailed Class Specifications

### NumericConverterOptions
```csharp
public class NumericConverterOptions
{
    public CultureInfo? Culture { get; set; }
    public string? NullPlaceholder { get; set; }
    public object? FallbackValue { get; set; }
    public bool ThrowOnError { get; set; }
}
```

### NumericFormattingConverterBase
```csharp
public abstract class NumericFormattingConverterBase : IValueConverter
{
    public CultureInfo Culture { get; set; }
    public string NullPlaceholder { get; set; }
    public object FallbackValue { get; set; }
    
    protected double? ToDouble(object? value);
    protected decimal? ToDecimal(object? value);
    protected bool TryParseNumeric(string? value, out double result);
}
```

### CurrencyConverter
```csharp
public class CurrencyConverter : NumericFormattingConverterBase
{
    public int DecimalDigits { get; set; } = 2;
    public string? CurrencySymbol { get; set; }  // Override culture default
}
```

### PercentConverter
```csharp
public enum PercentDisplayMode
{
    Value,    // Display actual value with % symbol (e.g., 1000 -> "1000%")
    Compute   // Multiply by 100 (e.g., 0.5 -> "50%") [Default]
}

public class PercentConverter : NumericFormattingConverterBase
{
    public int DecimalDigits { get; set; } = 0;
    public PercentDisplayMode DisplayMode { get; set; } = PercentDisplayMode.Compute;
}
```

### CustomNumericFormatConverter
```csharp
public class CustomNumericFormatConverter : NumericFormattingConverterBase
{
    public string Format { get; set; }  // e.g., "$00.00##", "00.000%", "#,##0.00"
}
```

---

## Usage Examples

### XAML Usage

```xml
<!-- Currency formatting -->
<Entry Text="{Binding Price, Converter={StaticResource CurrencyConverter}}" />

<!-- Percentage formatting -->
<Entry Text="{Binding Discount, Converter={StaticResource PercentConverter}}" />

<!-- Custom format -->
<Entry Text="{Binding Value, Converter={StaticResource CustomNumericFormat}, 
              ConverterParameter='$#,##0.00'}" />

<!-- Resource definitions -->
<ContentPage.Resources>
    <aurora:CurrencyConverter x:Key="CurrencyConverter" DecimalDigits="2" />
    <aurora:PercentConverter x:Key="PercentConverter" DisplayMode="Compute" />
    <aurora:CustomNumericFormatConverter x:Key="CustomNumericFormat" />
</ContentPage.Resources>
```

### Integration with StyledInputLayout

```xml
<aurora:StyledInputLayout Placeholder="Price">
    <Entry Text="{Binding Price, Converter={StaticResource CurrencyConverter}}" />
</aurora:StyledInputLayout>
```

---

## Risk Considerations

| Risk | Mitigation |
|------|------------|
| Culture parsing differences | Extensive testing with multiple cultures; use invariant parsing with fallback |
| Precision loss with decimals | Use `decimal` type internally for financial calculations |
| Invalid input during ConvertBack | Graceful fallback to previous value or default |
| Performance with large lists | Lazy initialization, avoid allocations in hot paths |
| XAML parsing errors | Sensible defaults for all properties |

---

## Success Criteria

- [ ] All converters support two-way binding
- [ ] All converters handle null values gracefully
- [ ] All converters respect CultureInfo settings
- [ ] All standard .NET numeric format strings are supported
- [ ] ConvertBack correctly parses formatted strings
- [ ] No exceptions thrown during normal use
- [ ] XML documentation for all public members
- [ ] Working demo in TestApp

---

## Approval Checkpoints

Please review and approve each phase before proceeding:

| Phase | Description | Status |
|-------|-------------|--------|
| Phase 1 | Foundation & Base Infrastructure | ✅ Completed |
| Phase 2 | Core Format Converters | ✅ Completed |
| Phase 3 | Advanced Format Converters | ✅ Completed |
| Phase 4 | Behaviors & Extensions | ✅ Completed |
| Phase 5 | Testing & Documentation | ✅ Completed |

---

## Next Steps

1. **Review this plan** and provide feedback on the proposed architecture
2. **Approve Phase 1** to begin implementation
3. I will create the feature branch and start with the foundation code

---

*Document created: December 20, 2025*
*Project: AuroraControls.Maui*

