// <copyright file="CodeViewer.xaml.cs" company="TheEightBot">
// Copyright (c) TheEightBot. All rights reserved.
// </copyright>

namespace AuroraControls.TestApp.Controls;

/// <summary>
/// A code viewer control for displaying code examples in demo pages.
/// Features syntax-highlighted code display with copy functionality.
/// </summary>
public partial class CodeViewer : ContentView
{
    /// <summary>
    /// Bindable property for the title.
    /// </summary>
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(
            nameof(Title),
            typeof(string),
            typeof(CodeViewer),
            "Code Example");

    /// <summary>
    /// Bindable property for the programming language.
    /// </summary>
    public static readonly BindableProperty LanguageProperty =
        BindableProperty.Create(
            nameof(Language),
            typeof(string),
            typeof(CodeViewer),
            "XAML");

    /// <summary>
    /// Bindable property for the code content.
    /// </summary>
    public static readonly BindableProperty CodeProperty =
        BindableProperty.Create(
            nameof(Code),
            typeof(string),
            typeof(CodeViewer),
            string.Empty);

    /// <summary>
    /// Initializes a new instance of the <see cref="CodeViewer"/> class.
    /// </summary>
    public CodeViewer()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Gets or sets the title displayed in the header.
    /// </summary>
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>
    /// Gets or sets the programming language (shown as badge).
    /// </summary>
    public string Language
    {
        get => (string)GetValue(LanguageProperty);
        set => SetValue(LanguageProperty, value);
    }

    /// <summary>
    /// Gets or sets the code content to display.
    /// </summary>
    public string Code
    {
        get => (string)GetValue(CodeProperty);
        set => SetValue(CodeProperty, value);
    }

    private async void OnCopyClicked(object? sender, EventArgs e)
    {
        try
        {
            await Clipboard.Default.SetTextAsync(Code);
            CopyStatusLabel.Text = "✓ Copied to clipboard!";

            // Reset after 2 seconds
            await Task.Delay(2000);
            CopyStatusLabel.Text = string.Empty;
        }
        catch (Exception ex)
        {
            CopyStatusLabel.Text = $"Copy failed: {ex.Message}";
            CopyStatusLabel.TextColor = Color.FromArgb("#EF4444"); // Error color
        }
    }
}

/// <summary>
/// Static helper for generating code examples.
/// </summary>
public static class CodeExamples
{
    /// <summary>
    /// Generates XAML code for a control with specified properties.
    /// </summary>
    /// <param name="controlName">The control type name.</param>
    /// <param name="properties">Dictionary of property names and values.</param>
    /// <returns>Formatted XAML string.</returns>
    public static string GenerateXaml(string controlName, Dictionary<string, object> properties)
    {
        var lines = new List<string>
        {
            $"<aurora:{controlName}",
        };

        foreach (var prop in properties)
        {
            var value = FormatPropertyValue(prop.Value);
            lines.Add($"    {prop.Key}=\"{value}\"");
        }

        // Close the tag
        if (lines.Count > 1)
        {
            lines[^1] = lines[^1] + " />";
        }
        else
        {
            lines[0] = lines[0] + " />";
        }

        return string.Join(Environment.NewLine, lines);
    }

    /// <summary>
    /// Generates C# code for creating a control programmatically.
    /// </summary>
    /// <param name="controlName">The control type name.</param>
    /// <param name="properties">Dictionary of property names and values.</param>
    /// <returns>Formatted C# string.</returns>
    public static string GenerateCSharp(string controlName, Dictionary<string, object> properties)
    {
        var lines = new List<string>
        {
            $"var control = new {controlName}",
            "{",
        };

        foreach (var prop in properties)
        {
            var value = FormatCSharpValue(prop.Value);
            lines.Add($"    {prop.Key} = {value},");
        }

        lines.Add("};");

        return string.Join(Environment.NewLine, lines);
    }

    private static string FormatPropertyValue(object value)
    {
        return value switch
        {
            Color color => color.ToArgbHex(),
            bool b => b.ToString().ToLowerInvariant(),
            double d => d.ToString("F2"),
            float f => f.ToString("F2"),
            _ => value?.ToString() ?? string.Empty,
        };
    }

    private static string FormatCSharpValue(object value)
    {
        return value switch
        {
            Color color => $"Color.FromArgb(\"{color.ToArgbHex()}\")",
            bool b => b.ToString().ToLowerInvariant(),
            double d => $"{d:F2}d",
            float f => $"{f:F2}f",
            string s => $"\"{s}\"",
            _ => value?.ToString() ?? "null",
        };
    }
}
