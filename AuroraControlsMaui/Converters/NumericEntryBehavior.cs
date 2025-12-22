// <copyright file="NumericEntryBehavior.cs" company="TheEightBot">
// Copyright (c) TheEightBot. All rights reserved.
// </copyright>

using System.Globalization;

namespace AuroraControls.Converters;

/// <summary>
/// A behavior that automatically formats numeric values in an Entry when the control loses focus.
/// Provides a seamless editing experience where users see raw numbers while editing
/// and formatted values when not editing.
/// </summary>
/// <example>
/// XAML usage:
/// <code>
/// &lt;Entry Text="{Binding Price}"&gt;
///     &lt;Entry.Behaviors&gt;
///         &lt;aurora:NumericEntryBehavior Format="C2" /&gt;
///     &lt;/Entry.Behaviors&gt;
/// &lt;/Entry&gt;
/// </code>
/// </example>
public class NumericEntryBehavior : Behavior<Entry>
{
    private Entry? _entry;
    private bool _isFormatting;
    private double? _currentValue;

    /// <summary>
    /// Bindable property for <see cref="Format"/>.
    /// </summary>
    public static readonly BindableProperty FormatProperty =
        BindableProperty.Create(
            nameof(Format),
            typeof(string),
            typeof(NumericEntryBehavior),
            "N2");

    /// <summary>
    /// Bindable property for <see cref="Culture"/>.
    /// </summary>
    public static readonly BindableProperty CultureProperty =
        BindableProperty.Create(
            nameof(Culture),
            typeof(CultureInfo),
            typeof(NumericEntryBehavior),
            null);

    /// <summary>
    /// Bindable property for <see cref="FormatOnUnfocus"/>.
    /// </summary>
    public static readonly BindableProperty FormatOnUnfocusProperty =
        BindableProperty.Create(
            nameof(FormatOnUnfocus),
            typeof(bool),
            typeof(NumericEntryBehavior),
            true);

    /// <summary>
    /// Bindable property for <see cref="AllowNegative"/>.
    /// </summary>
    public static readonly BindableProperty AllowNegativeProperty =
        BindableProperty.Create(
            nameof(AllowNegative),
            typeof(bool),
            typeof(NumericEntryBehavior),
            true);

    /// <summary>
    /// Bindable property for <see cref="AllowDecimal"/>.
    /// </summary>
    public static readonly BindableProperty AllowDecimalProperty =
        BindableProperty.Create(
            nameof(AllowDecimal),
            typeof(bool),
            typeof(NumericEntryBehavior),
            true);

    /// <summary>
    /// Bindable property for <see cref="MinValue"/>.
    /// </summary>
    public static readonly BindableProperty MinValueProperty =
        BindableProperty.Create(
            nameof(MinValue),
            typeof(double?),
            typeof(NumericEntryBehavior),
            null);

    /// <summary>
    /// Bindable property for <see cref="MaxValue"/>.
    /// </summary>
    public static readonly BindableProperty MaxValueProperty =
        BindableProperty.Create(
            nameof(MaxValue),
            typeof(double?),
            typeof(NumericEntryBehavior),
            null);

    /// <summary>
    /// Bindable property for <see cref="MaximumFractionDigits"/>.
    /// </summary>
    public static readonly BindableProperty MaximumFractionDigitsProperty =
        BindableProperty.Create(
            nameof(MaximumFractionDigits),
            typeof(int?),
            typeof(NumericEntryBehavior),
            null);

    /// <summary>
    /// Bindable property for <see cref="MinimumFractionDigits"/>.
    /// </summary>
    public static readonly BindableProperty MinimumFractionDigitsProperty =
        BindableProperty.Create(
            nameof(MinimumFractionDigits),
            typeof(int?),
            typeof(NumericEntryBehavior),
            null);

    /// <summary>
    /// Bindable property for <see cref="RoundingMode"/>.
    /// </summary>
    public static readonly BindableProperty RoundingModeProperty =
        BindableProperty.Create(
            nameof(RoundingMode),
            typeof(NumericRoundingMode),
            typeof(NumericEntryBehavior),
            NumericRoundingMode.ToEven);

    /// <summary>
    /// Bindable property for <see cref="EnforceMaxFractionDigitsDuringInput"/>.
    /// </summary>
    public static readonly BindableProperty EnforceMaxFractionDigitsDuringInputProperty =
        BindableProperty.Create(
            nameof(EnforceMaxFractionDigitsDuringInput),
            typeof(bool),
            typeof(NumericEntryBehavior),
            true);

    /// <summary>
    /// Gets or sets the format string to use when displaying the value.
    /// Supports standard .NET numeric format strings (e.g., "C2", "N0", "P1").
    /// Defaults to "N2".
    /// </summary>
    public string Format
    {
        get => (string)GetValue(FormatProperty);
        set => SetValue(FormatProperty, value);
    }

    /// <summary>
    /// Gets or sets the culture to use for formatting and parsing.
    /// When null, the current UI culture is used.
    /// </summary>
    public CultureInfo? Culture
    {
        get => (CultureInfo?)GetValue(CultureProperty);
        set => SetValue(CultureProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether to format the value when the entry loses focus.
    /// Defaults to true.
    /// </summary>
    public bool FormatOnUnfocus
    {
        get => (bool)GetValue(FormatOnUnfocusProperty);
        set => SetValue(FormatOnUnfocusProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether negative values are allowed.
    /// Defaults to true.
    /// </summary>
    public bool AllowNegative
    {
        get => (bool)GetValue(AllowNegativeProperty);
        set => SetValue(AllowNegativeProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether decimal values are allowed.
    /// Defaults to true.
    /// </summary>
    public bool AllowDecimal
    {
        get => (bool)GetValue(AllowDecimalProperty);
        set => SetValue(AllowDecimalProperty, value);
    }

    /// <summary>
    /// Gets or sets the minimum allowed value.
    /// When null, no minimum is enforced.
    /// </summary>
    public double? MinValue
    {
        get => (double?)GetValue(MinValueProperty);
        set => SetValue(MinValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum allowed value.
    /// When null, no maximum is enforced.
    /// </summary>
    public double? MaxValue
    {
        get => (double?)GetValue(MaxValueProperty);
        set => SetValue(MaxValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum number of fractional (decimal) digits allowed.
    /// When set, input and the stored value will be limited to this number of decimal places.
    /// When null, no limit is enforced (but formatting may still round the display).
    /// </summary>
    public int? MaximumFractionDigits
    {
        get => (int?)GetValue(MaximumFractionDigitsProperty);
        set => SetValue(MaximumFractionDigitsProperty, value);
    }

    /// <summary>
    /// Gets or sets the minimum number of fractional (decimal) digits to display.
    /// Values with fewer decimal digits will be zero-padded when formatting.
    /// When null, the format string determines the display.
    /// </summary>
    public int? MinimumFractionDigits
    {
        get => (int?)GetValue(MinimumFractionDigitsProperty);
        set => SetValue(MinimumFractionDigitsProperty, value);
    }

    /// <summary>
    /// Gets or sets the rounding mode to use when limiting decimal places.
    /// Defaults to <see cref="NumericRoundingMode.ToEven"/>.
    /// </summary>
    public NumericRoundingMode RoundingMode
    {
        get => (NumericRoundingMode)GetValue(RoundingModeProperty);
        set => SetValue(RoundingModeProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether to enforce the maximum fraction digits during input.
    /// When true (default), users cannot type more decimal places than allowed.
    /// When false, extra decimals can be entered but will be rounded on unfocus.
    /// </summary>
    public bool EnforceMaxFractionDigitsDuringInput
    {
        get => (bool)GetValue(EnforceMaxFractionDigitsDuringInputProperty);
        set => SetValue(EnforceMaxFractionDigitsDuringInputProperty, value);
    }

    /// <summary>
    /// Gets the effective culture for formatting and parsing.
    /// </summary>
    private CultureInfo EffectiveCulture => Culture ?? CultureInfo.CurrentUICulture;

    /// <summary>
    /// Gets the <see cref="MidpointRounding"/> equivalent for the current <see cref="RoundingMode"/>.
    /// </summary>
    private MidpointRounding MidpointRounding => RoundingMode switch
    {
        NumericRoundingMode.ToEven => MidpointRounding.ToEven,
        NumericRoundingMode.AwayFromZero => MidpointRounding.AwayFromZero,
        NumericRoundingMode.Ceiling => MidpointRounding.ToPositiveInfinity,
        NumericRoundingMode.Floor => MidpointRounding.ToNegativeInfinity,
        NumericRoundingMode.Truncate => MidpointRounding.ToZero,
        _ => MidpointRounding.ToEven,
    };

    /// <inheritdoc/>
    protected override void OnAttachedTo(Entry bindable)
    {
        base.OnAttachedTo(bindable);
        _entry = bindable;

        _entry.Focused += OnEntryFocused;
        _entry.Unfocused += OnEntryUnfocused;
        _entry.TextChanged += OnEntryTextChanged;

        // Format the initial value if present
        FormatCurrentValue();
    }

    /// <inheritdoc/>
    protected override void OnDetachingFrom(Entry bindable)
    {
        if (_entry is not null)
        {
            _entry.Focused -= OnEntryFocused;
            _entry.Unfocused -= OnEntryUnfocused;
            _entry.TextChanged -= OnEntryTextChanged;
        }

        _entry = null;
        base.OnDetachingFrom(bindable);
    }

    private void OnEntryFocused(object? sender, FocusEventArgs e)
    {
        if (_entry is null || _isFormatting)
        {
            return;
        }

        // When focused, show the raw numeric value for easier editing
        if (_currentValue.HasValue)
        {
            _isFormatting = true;
            try
            {
                _entry.Text = _currentValue.Value.ToString(EffectiveCulture);
            }
            finally
            {
                _isFormatting = false;
            }

            // Select all text for easy replacement
            _entry.CursorPosition = 0;
            _entry.SelectionLength = _entry.Text?.Length ?? 0;
        }
    }

    private void OnEntryUnfocused(object? sender, FocusEventArgs e)
    {
        if (_entry is null || _isFormatting || !FormatOnUnfocus)
        {
            return;
        }

        // Parse and validate the current text
        if (TryParseValue(_entry.Text, out var value))
        {
            // Apply min/max constraints
            value = ApplyConstraints(value);
            _currentValue = value;
        }

        // Format the value
        FormatCurrentValue();
    }

    private void OnEntryTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_entry is null || _isFormatting)
        {
            return;
        }

        var newText = e.NewTextValue;

        // Validate input characters
        if (!IsValidInput(newText))
        {
            _isFormatting = true;
            try
            {
                _entry.Text = e.OldTextValue;
            }
            finally
            {
                _isFormatting = false;
            }

            return;
        }

        // Try to parse the current value (for live validation)
        if (TryParseValue(newText, out var value))
        {
            _currentValue = value;
        }
    }

    private void FormatCurrentValue()
    {
        if (_entry is null)
        {
            return;
        }

        _isFormatting = true;
        try
        {
            // If we don't have a current value yet, try to parse from the entry text
            if (!_currentValue.HasValue && !string.IsNullOrWhiteSpace(_entry.Text))
            {
                if (TryParseValue(_entry.Text, out var parsedValue))
                {
                    _currentValue = ApplyConstraints(parsedValue);
                }
            }

            // Now format if we have a value
            if (_currentValue.HasValue)
            {
                _entry.Text = _currentValue.Value.ToString(Format, EffectiveCulture);
            }
        }
        finally
        {
            _isFormatting = false;
        }
    }

    private bool TryParseValue(string? text, out double value)
    {
        value = 0;

        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        var culture = EffectiveCulture;

        // Try standard parsing first
        if (double.TryParse(text, NumberStyles.Any, culture, out value))
        {
            return true;
        }

        // Try cleaning the text and parsing again
        var cleanedText = CleanNumericString(text, culture);
        return double.TryParse(cleanedText, NumberStyles.Any, culture, out value);
    }

    private string CleanNumericString(string value, CultureInfo culture)
    {
        var numberFormat = culture.NumberFormat;
        var result = value.Trim();

        // Remove currency symbol
        result = result.Replace(numberFormat.CurrencySymbol, string.Empty);

        // Remove percent symbol
        result = result.Replace(numberFormat.PercentSymbol, string.Empty);

        // Remove group separators
        result = result.Replace(numberFormat.NumberGroupSeparator, string.Empty);
        result = result.Replace(numberFormat.CurrencyGroupSeparator, string.Empty);

        return result.Trim();
    }

    private bool IsValidInput(string? text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return true;
        }

        var culture = EffectiveCulture;
        var decimalSeparator = culture.NumberFormat.NumberDecimalSeparator;
        var negativeSign = culture.NumberFormat.NegativeSign;

        // Check for max fraction digits enforcement
        if (EnforceMaxFractionDigitsDuringInput && MaximumFractionDigits.HasValue)
        {
            var decimalIndex = text.IndexOf(decimalSeparator, StringComparison.Ordinal);
            if (decimalIndex >= 0)
            {
                var fractionPart = text.Substring(decimalIndex + decimalSeparator.Length);

                // Count only digit characters in the fraction part
                var fractionDigitCount = 0;
                foreach (var c in fractionPart)
                {
                    if (char.IsDigit(c))
                    {
                        fractionDigitCount++;
                    }
                }

                if (fractionDigitCount > MaximumFractionDigits.Value)
                {
                    return false;
                }
            }
        }

        foreach (var c in text)
        {
            // Allow digits
            if (char.IsDigit(c))
            {
                continue;
            }

            // Allow decimal separator if decimals are allowed
            if (AllowDecimal && decimalSeparator.Contains(c))
            {
                continue;
            }

            // Allow negative sign if negatives are allowed
            if (AllowNegative && negativeSign.Contains(c))
            {
                continue;
            }

            // Allow whitespace (will be trimmed)
            if (char.IsWhiteSpace(c))
            {
                continue;
            }

            // Allow group separators during input
            if (culture.NumberFormat.NumberGroupSeparator.Contains(c))
            {
                continue;
            }

            // Invalid character
            return false;
        }

        return true;
    }

    private double ApplyConstraints(double value)
    {
        // Apply fraction digit rounding first
        if (MaximumFractionDigits.HasValue && MaximumFractionDigits.Value >= 0)
        {
            value = Math.Round(value, MaximumFractionDigits.Value, MidpointRounding);
        }

        if (!AllowNegative && value < 0)
        {
            value = 0;
        }

        if (MinValue.HasValue && value < MinValue.Value)
        {
            value = MinValue.Value;
        }

        if (MaxValue.HasValue && value > MaxValue.Value)
        {
            value = MaxValue.Value;
        }

        return value;
    }
}
