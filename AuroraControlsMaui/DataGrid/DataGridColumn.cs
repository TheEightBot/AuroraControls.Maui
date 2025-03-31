using System.ComponentModel;

namespace AuroraControls.DataGrid;

/// <summary>
/// Base class for all DataGrid column types.
/// </summary>
public abstract class DataGridColumn : BindableObject, INotifyPropertyChanged
{
    /// <summary>
    /// Property for column header text.
    /// </summary>
    public static readonly BindableProperty HeaderTextProperty =
        BindableProperty.Create(nameof(HeaderText), typeof(string), typeof(DataGridColumn), default(string));

    /// <summary>
    /// Property for column width.
    /// </summary>
    public static readonly BindableProperty WidthProperty =
        BindableProperty.Create(nameof(Width), typeof(double), typeof(DataGridColumn), -1d);

    /// <summary>
    /// Property for the property path to bind to.
    /// </summary>
    public static readonly BindableProperty PropertyPathProperty =
        BindableProperty.Create(nameof(PropertyPath), typeof(string), typeof(DataGridColumn), default(string));

    /// <summary>
    /// Property for determining if the column is visible.
    /// </summary>
    public static readonly BindableProperty IsVisibleProperty =
        BindableProperty.Create(nameof(IsVisible), typeof(bool), typeof(DataGridColumn), true);

    /// <summary>
    /// Gets or sets the header text.
    /// </summary>
    public string HeaderText
    {
        get => (string)GetValue(HeaderTextProperty);
        set => SetValue(HeaderTextProperty, value);
    }

    /// <summary>
    /// Gets or sets the column width.
    /// </summary>
    public double Width
    {
        get => (double)GetValue(WidthProperty);
        set => SetValue(WidthProperty, value);
    }

    /// <summary>
    /// Gets or sets the property path for data binding.
    /// </summary>
    public string PropertyPath
    {
        get => (string)GetValue(PropertyPathProperty);
        set => SetValue(PropertyPathProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether gets or sets whether the column is visible.
    /// </summary>
    public bool IsVisible
    {
        get => (bool)GetValue(IsVisibleProperty);
        set => SetValue(IsVisibleProperty, value);
    }

    /// <summary>
    /// Gets or sets the actual width of the column after layout.
    /// </summary>
    internal double ActualWidth { get; set; }

    /// <summary>
    /// Gets or sets the X position of the column in the grid.
    /// </summary>
    internal double X { get; set; }

    /// <summary>
    /// Gets the cell value for the specified data item.
    /// </summary>
    /// <param name="item">The data item.</param>
    /// <returns>The cell value.</returns>
    public abstract object GetCellValue(object item);

    /// <summary>
    /// Draws a cell in the grid.
    /// </summary>
    /// <param name="canvas">The canvas to draw on.</param>
    /// <param name="rect">The rectangle to draw within.</param>
    /// <param name="value">The value to draw.</param>
    /// <param name="isSelected">Whether the cell is selected.</param>
    public abstract void DrawCell(SKCanvas canvas, SKRect rect, object value, bool isSelected);

    /// <summary>
    /// Draws the column header.
    /// </summary>
    /// <param name="canvas">The canvas to draw on.</param>
    /// <param name="rect">The rectangle to draw within.</param>
    /// <param name="isSelected">Whether the header is selected.</param>
    public abstract void DrawHeader(SKCanvas canvas, SKRect rect, bool isSelected);
}
