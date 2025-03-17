using System.Collections.Specialized;
using System.Globalization;

namespace AuroraControls;

public enum CalendarDayOfWeekDisplayType
{
    Full,
    Abbreviated,
    AbbreviatedUppercase,
    Shortest,
}

public enum CalendarDayDisplayLocationType
{
    Centered,
    UpperRight,
}

/// <summary>
/// Calendar selection type enum. Used to specify the selection type of the calendar.
/// </summary>
public enum CalendarSelectionType
{
    Single,
    Span,
    Multiple,
}

public class CalendarView : AuroraViewBase
{
    private const int Columns = 7, Rows = 6, TotalCells = Columns * Rows;

    private readonly List<CellInfo> _dateContainers;
    private readonly List<DayOfWeek> _daysOfWeekNames;
    private readonly ObservableUniqueCollection<DateTime> _selectedDates = new();
    private readonly ObservableUniqueCollection<CalendarEvent> _events = new();
    private readonly object _lock = new();

    private float _rowSize = -1f, _columnSize = -1f;

    private bool _currentCalendarNeedsRebuild = true;

    private int? _pressedIndex;
    private bool _wasSelected;

    public event EventHandler<CalendarSelectedDatesChangedEventArgs> CalendarSelectedDatesChanged;

    /// <summary>
    /// The current year property. Updating this property causes the surface to redraw.
    /// </summary>
    public static readonly BindableProperty CurrentYearProperty =
        BindableProperty.Create(nameof(CurrentYear), typeof(int), typeof(CalendarView), DateTime.Now.Year,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                if (bindable is not CalendarView control)
                {
                    return;
                }

                control._currentCalendarNeedsRebuild = true;
                control.InvalidateSurface();
            },
            coerceValue: (bindable, value) =>
            {
                if (bindable is not CalendarView control)
                {
                    return value;
                }

                var val = (int)value;

                var maxDate = control.MaximumDate;

                if (val > maxDate.Year)
                {
                    return maxDate.Year;
                }

                return value;
            });

    /// <summary>
    /// Gets or sets the current year.
    /// </summary>
    /// <value>The current year.</value>
    public int CurrentYear
    {
        get { return (int)GetValue(CurrentYearProperty); }
        set { SetValue(CurrentYearProperty, value); }
    }

    /// <summary>
    /// The current month property. Updating this property causes the surface to redraw.
    /// </summary>
    public static readonly BindableProperty CurrentMonthProperty =
        BindableProperty.Create(nameof(CurrentMonth), typeof(int), typeof(CalendarView), DateTime.Now.Month,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                if (bindable is not CalendarView control)
                {
                    return;
                }

                control._currentCalendarNeedsRebuild = true;
                control.InvalidateSurface();
            },
            coerceValue: (bindable, value) =>
            {
                if (bindable is not CalendarView control)
                {
                    return value;
                }

                var val = (int)value;

                var maxDate = control.MaximumDate;

                if (maxDate.Year >= control.CurrentYear && val > maxDate.Month)
                {
                    return maxDate.Month;
                }

                return value;
            });

    /// <summary>
    /// Gets or sets the current month.
    /// </summary>
    /// <value>int value that represents the current month.</value>
    public int CurrentMonth
    {
        get { return (int)GetValue(CurrentMonthProperty); }
        set { SetValue(CurrentMonthProperty, value); }
    }

    /// <summary>
    /// The maximum date property used to define the maximum date.
    /// </summary>
    public static readonly BindableProperty MaximumDateProperty =
        BindableProperty.Create(nameof(MaximumDate), typeof(DateTime), typeof(CalendarView), DateTime.MaxValue);

    /// <summary>
    /// Gets or sets the maximum date.
    /// </summary>
    /// <value>The Maximum DateTime.</value>
    public DateTime MaximumDate
    {
        get { return (DateTime)GetValue(MaximumDateProperty); }
        set { SetValue(MaximumDateProperty, value); }
    }

    /// <summary>
    /// The minimum date property used to define the minimum date.
    /// </summary>
    public static readonly BindableProperty MinimumDateProperty =
        BindableProperty.Create(nameof(MinimumDate), typeof(DateTime), typeof(CalendarView), DateTime.MinValue);

    /// <summary>
    /// Gets or sets the minimum date.
    /// </summary>
    /// <value>The Minimum DateTime.</value>
    public DateTime MinimumDate
    {
        get { return (DateTime)GetValue(MinimumDateProperty); }
        set { SetValue(MinimumDateProperty, value); }
    }

    public static readonly BindableProperty HeaderTextColorProperty =
        BindableProperty.Create(nameof(HeaderTextColor), typeof(Color), typeof(CalendarView), Colors.White,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    public Color HeaderTextColor
    {
        get => (Color)GetValue(HeaderTextColorProperty);
        set => SetValue(HeaderTextColorProperty, value);
    }

    /// <summary>
    /// The separator color property.
    /// </summary>
    public static readonly BindableProperty SeparatorColorProperty =
        BindableProperty.Create(nameof(SeparatorColor), typeof(Color), typeof(CalendarView), Colors.LightGray,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the color of the separator.
    /// </summary>
    /// <value>The Color.</value>
    public Color SeparatorColor
    {
        get { return (Color)GetValue(SeparatorColorProperty); }
        set { SetValue(SeparatorColorProperty, value); }
    }

    public static readonly BindableProperty UnavailableDateColorProperty =
        BindableProperty.Create(nameof(UnavailableDateColor), typeof(Color), typeof(CalendarView), Colors.LightGray,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the color of the unavailable date.
    /// </summary>
    /// <value>The Color.</value>
    public Color UnavailableDateColor
    {
        get { return (Color)GetValue(UnavailableDateColorProperty); }
        set { SetValue(UnavailableDateColorProperty, value); }
    }

    /// <summary>
    /// The available date color property. Represents the dates that are available.
    /// </summary>
    public static readonly BindableProperty AvailableDateColorProperty =
        BindableProperty.Create(nameof(AvailableDateColor), typeof(Color), typeof(CalendarView), Colors.White,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the color of the available date.
    /// </summary>
    /// <value>The Color.</value>
    public Color AvailableDateColor
    {
        get { return (Color)GetValue(AvailableDateColorProperty); }
        set { SetValue(AvailableDateColorProperty, value); }
    }

    /// <summary>
    /// The date color property.
    /// </summary>
    public static readonly BindableProperty DateColorProperty =
        BindableProperty.Create(nameof(DateColor), typeof(Color), typeof(CalendarView), Colors.DarkGray,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the color of the date.
    /// </summary>
    /// <value>The Color.</value>
    public Color DateColor
    {
        get { return (Color)GetValue(DateColorProperty); }
        set { SetValue(DateColorProperty, value); }
    }

    public static readonly BindableProperty SelectedDateColorProperty =
        BindableProperty.Create(nameof(SelectedDateColor), typeof(Color), typeof(CalendarView), Colors.MediumBlue,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the color of the selected date.
    /// </summary>
    /// <value>The Color.</value>
    public Color SelectedDateColor
    {
        get { return (Color)GetValue(SelectedDateColorProperty); }
        set { SetValue(SelectedDateColorProperty, value); }
    }

    /// <summary>
    /// The date text color property.
    /// </summary>
    public static readonly BindableProperty DateTextColorProperty =
        BindableProperty.Create(nameof(DateTextColor), typeof(Color), typeof(CalendarView), Colors.DarkGray,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the color of the date text.
    /// </summary>
    /// <value>The Color.</value>
    public Color DateTextColor
    {
        get { return (Color)GetValue(DateTextColorProperty); }
        set { SetValue(DateTextColorProperty, value); }
    }

    /// <summary>
    /// The selected date text color property.
    /// </summary>
    public static readonly BindableProperty SelectedDateTextColorProperty =
        BindableProperty.Create(nameof(SelectedDateTextColor), typeof(Color), typeof(CalendarView), Colors.White,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the color of the selected date text.
    /// </summary>
    /// <value>The Color.</value>
    public Color SelectedDateTextColor
    {
        get { return (Color)GetValue(SelectedDateTextColorProperty); }
        set { SetValue(SelectedDateTextColorProperty, value); }
    }

    /// <summary>
    /// The date background color property.
    /// </summary>
    public static readonly BindableProperty DateBackgroundColorProperty =
        BindableProperty.Create(nameof(DateBackgroundColor), typeof(Color), typeof(CalendarView), Colors.DarkGray,
            propertyChanged: IAuroraView.PropertyChangedInvalidateSurface);

    /// <summary>
    /// Gets or sets the color of the date background.
    /// </summary>
    /// <value>The Color.</value>
    public Color DateBackgroundColor
    {
        get { return (Color)GetValue(DateBackgroundColorProperty); }
        set { SetValue(DateBackgroundColorProperty, value); }
    }

    /// <summary>
    /// The selection type property. Sets the selection mode based on the provided type.
    /// </summary>
    public static readonly BindableProperty SelectionTypeProperty =
        BindableProperty.Create(nameof(SelectionType), typeof(CalendarSelectionType), typeof(CalendarView),
            CalendarSelectionType.Single,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                var control = bindable as CalendarView;
                var selectionType = (CalendarSelectionType)newValue;
                if (control == null)
                {
                    return;
                }

                var startingDayOfWeek = (int)new DateTime(control.CurrentYear, control.CurrentMonth, 1).DayOfWeek;

                if (startingDayOfWeek == 7)
                {
                    startingDayOfWeek = 1;
                }
                else
                {
                    startingDayOfWeek++;
                }

                switch (selectionType)
                {
                    case CalendarSelectionType.Single:
                        foreach (var item in control._dateContainers
                                     .Select((x, index) => new { CellInfo = x, Index = index })
                                     .Where(x => x.CellInfo.Selected).Skip(1))
                        {
                            var removableItem = control._selectedDates.FirstOrDefault(x =>
                                x.Date == new DateTime(control.CurrentYear, control.CurrentMonth,
                                    item.Index - startingDayOfWeek + 2).Date);
                            control._selectedDates.Remove(removableItem);
                            item.CellInfo.Selected = false;
                        }

                        break;
                    case CalendarSelectionType.Span:
                        var firstSelectedIndex = control._dateContainers.FindIndex(x => x.Selected);
                        var lastSelectedIndex = control._dateContainers.FindLastIndex(x => x.Selected);

                        if (firstSelectedIndex >= 0 && lastSelectedIndex >= firstSelectedIndex)
                        {
                            for (int i = 0; i < control._dateContainers.Count; i++)
                            {
                                if (!control._dateContainers?.ElementAtOrDefault(i)?.Available ?? true)
                                {
                                    continue;
                                }

                                var date = new DateTime(control.CurrentYear, control.CurrentMonth, i - startingDayOfWeek + 2).Date;
                                var foundDate = control._selectedDates.FirstOrDefault(x =>
                                    x.Date == new DateTime(control.CurrentYear, control.CurrentMonth, i - startingDayOfWeek + 2).Date);

                                var isWithinSelection = i >= firstSelectedIndex && i <= lastSelectedIndex;
                                control._dateContainers.ElementAt(i).Selected = isWithinSelection;

                                if (foundDate.Date != date.Date && isWithinSelection)
                                {
                                    control._selectedDates.Add(date);
                                }

                                if (foundDate.Date != date.Date && !isWithinSelection)
                                {
                                    control._selectedDates.Remove(date);
                                }
                            }
                        }

                        break;
                    default:
                        break;
                }

                control.InvalidateSurface();
            });

    /// <summary>
    /// Gets or sets the type of the selection.
    /// </summary>
    /// <value>The type of the selection.</value>
    public CalendarSelectionType SelectionType
    {
        get { return (CalendarSelectionType)GetValue(SelectionTypeProperty); }
        set { SetValue(SelectionTypeProperty, value); }
    }

    /// <summary>
    /// The selected dates property key.
    /// </summary>
    public static readonly BindablePropertyKey SelectedDatesPropertyKey = BindableProperty.CreateReadOnly(
        nameof(SelectedDates),
        typeof(List<DateTime>), typeof(CalendarView), new List<DateTime>());

    public static readonly BindableProperty DayOfWeekDisplayTypeProperty =
        BindableProperty.Create(nameof(DayOfWeekDisplayType), typeof(CalendarDayOfWeekDisplayType),
            typeof(CalendarView), CalendarDayOfWeekDisplayType.Abbreviated,
            propertyChanged:
            (bindable, oldValue, newValue) =>
            {
                (bindable as CalendarView)._currentCalendarNeedsRebuild = true;
                (bindable as IAuroraView)?.InvalidateSurface();
            });

    public CalendarDayOfWeekDisplayType DayOfWeekDisplayType
    {
        get => (CalendarDayOfWeekDisplayType)GetValue(DayOfWeekDisplayTypeProperty);
        set => SetValue(DayOfWeekDisplayTypeProperty, value);
    }

    public static readonly BindableProperty CalendarDayDisplayLocationProperty =
        BindableProperty.Create(nameof(CalendarDayDisplayLocation), typeof(CalendarDayDisplayLocationType),
            typeof(CalendarView), CalendarDayDisplayLocationType.Centered,
            propertyChanged:
            (bindable, oldValue, newValue) =>
            {
                (bindable as CalendarView)._currentCalendarNeedsRebuild = true;
                (bindable as IAuroraView)?.InvalidateSurface();
            });

    public CalendarDayDisplayLocationType CalendarDayDisplayLocation
    {
        get => (CalendarDayDisplayLocationType)GetValue(CalendarDayDisplayLocationProperty);
        set => SetValue(CalendarDayDisplayLocationProperty, value);
    }

    /// <summary>
    /// The selected date's property.
    /// </summary>
    public static readonly BindableProperty SelectedDatesProperty = SelectedDatesPropertyKey.BindableProperty;

    /// <summary>
    /// Gets the user-selected dates.
    /// </summary>
    /// <value>the collection of selected dates.</value>
    public IList<DateTime> SelectedDates => _selectedDates;

    /// <summary>
    /// Gets the event dates.
    /// </summary>
    /// <value>the collection of event dates.</value>
    public IList<CalendarEvent> Events => _events;

    /// <summary>
    /// Initializes a new instance of the <see cref="CalendarView"/> class.
    /// </summary>
    public CalendarView()
    {
        _dateContainers = new List<CellInfo>(Enumerable.Range(0, TotalCells).Select(_ => new CellInfo()));

        // TODO: Make this not trash
        _daysOfWeekNames = new List<DayOfWeek>();
        _daysOfWeekNames.Add(DayOfWeek.Sunday);
        _daysOfWeekNames.Add(DayOfWeek.Monday);
        _daysOfWeekNames.Add(DayOfWeek.Tuesday);
        _daysOfWeekNames.Add(DayOfWeek.Wednesday);
        _daysOfWeekNames.Add(DayOfWeek.Thursday);
        _daysOfWeekNames.Add(DayOfWeek.Friday);
        _daysOfWeekNames.Add(DayOfWeek.Saturday);
    }

    protected override void Attached()
    {
        this.EnableTouchEvents = true;

        _selectedDates.CollectionChanged -= SelectedDates_CollectionChanged;
        _selectedDates.CollectionChanged += SelectedDates_CollectionChanged;

        _events.CollectionChanged -= Events_CollectionChanged;
        _events.CollectionChanged += Events_CollectionChanged;

        base.Attached();
    }

    protected override void Detached()
    {
        _events.CollectionChanged -= Events_CollectionChanged;
        _selectedDates.CollectionChanged -= SelectedDates_CollectionChanged;
        base.Detached();
    }

    private void Events_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        this.InvalidateSurface();
    }

    protected override void OnPropertyChanged(string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (!propertyName.Equals(HeightProperty.PropertyName) &&
            !propertyName.Equals(WidthProperty.PropertyName))
        {
            return;
        }

        this._currentCalendarNeedsRebuild = true;
        this?.InvalidateSurface();
    }

    /// <summary>
    /// This is the method used to draw our calenar. This method is fired every time <c>this.InvalidateSurface();</c> is called, resulting in a "redrawing" of the entire calendar.
    /// </summary>
    /// <param name="surface">The skia surface to paint on the controls.</param>
    /// <param name="info">Information about the skia image.</param>
    protected override void PaintControl(SKSurface surface, SKImageInfo info)
    {
        var canvas = surface.Canvas;

        using var dateFontPaint = new SKPaint();
        var columnSize = info.Width / (float)Columns;

        var dateFontPaintColor = this.DateTextColor.ToSKColor();
        var selectedDateFontPaintColor = this.SelectedDateTextColor.ToSKColor();

        dateFontPaint.Typeface = PlatformInfo.DefaultTypeface;
        dateFontPaint.IsAntialias = true;
        dateFontPaint.TextAlign = SKTextAlign.Center;
        dateFontPaint.LcdRenderText = true;
        dateFontPaint.SubpixelText = true;

        var currentCultureDateFormat = CultureInfo.CurrentUICulture.DateTimeFormat;

        var daysOfWeekNames =
            this._daysOfWeekNames
                .Select(
                    dayOfWeek =>
                    {
                        switch (this.DayOfWeekDisplayType)
                        {
                            case CalendarDayOfWeekDisplayType.Abbreviated:
                                return currentCultureDateFormat.GetAbbreviatedDayName(dayOfWeek);
                            case CalendarDayOfWeekDisplayType.AbbreviatedUppercase:
                                return currentCultureDateFormat.GetAbbreviatedDayName(dayOfWeek).ToUpperInvariant();
                            case CalendarDayOfWeekDisplayType.Shortest:
                                return currentCultureDateFormat.GetShortestDayName(dayOfWeek);
                            case CalendarDayOfWeekDisplayType.Full:
                            default:
                                return currentCultureDateFormat.GetDayName(dayOfWeek);
                        }
                    })
                .ToArray();

        var longestName =
            daysOfWeekNames
                .OrderByDescending(x => x.Length)
                .FirstOrDefault();

        dateFontPaint.TextSize = 49f;
        var foundFontSize = false;

        while (!foundFontSize)
        {
            var measuredText = canvas.TextSize(longestName, dateFontPaint);

            if (measuredText.Width > columnSize)
            {
                dateFontPaint.TextSize -= .5f;
            }
            else
            {
                foundFontSize = true;
            }
        }

        var dayHeaderHeight = (float)canvas.TextSize("Xy", dateFontPaint).Height + 8f;

        var rowSize = (info.Height - dayHeaderHeight) / (float)Rows;

        var minSize = (float)Math.Min(rowSize, columnSize);

        // TODO: Make this more configurable
        var selectionSize = minSize * .8f;
        var halfSelectionSize = selectionSize * .5f;

        var currentYear = this.CurrentYear;
        var currentMonth = this.CurrentMonth;

        var startingDayOfWeek = (int)new DateTime(currentYear, currentMonth, 1).DayOfWeek;

        if (startingDayOfWeek == 7)
        {
            startingDayOfWeek = 1;
        }
        else
        {
            startingDayOfWeek++;
        }

        var daysInMonth = DateTime.DaysInMonth(currentYear, currentMonth);

        int dayOfMonth = 1,
            currentPosition = startingDayOfWeek,
            columnPosition = startingDayOfWeek,
            rowPosition = 1;

        if (this._currentCalendarNeedsRebuild)
        {
            this._currentCalendarNeedsRebuild = false;

            this._rowSize = rowSize;
            this._columnSize = columnSize;

            var rowIncrementer = 0;
            var columnIncrementer = 0;
            for (int currIndex = 0; currIndex < TotalCells; currIndex++)
            {
                var rowLocation = dayHeaderHeight + (rowIncrementer * rowSize);
                var columnLocation = columnIncrementer * columnSize;

                var cellInfo = this._dateContainers?.ElementAtOrDefault(currIndex);

                if (cellInfo != null)
                {
                    cellInfo.Available = currIndex >= startingDayOfWeek - 1 &&
                                         currIndex < daysInMonth + startingDayOfWeek - 1;
                    cellInfo.Location = new SKRect(columnLocation, rowLocation, columnLocation + columnSize,
                        rowLocation + rowSize);
                    cellInfo.Selected = this._dateContainers?.ElementAt(currIndex)?.Selected ?? false;

                    if (cellInfo.Available)
                    {
                        var date = new DateTime(currentYear, currentMonth, currIndex - startingDayOfWeek + 2).Date;
                        cellInfo.Selected = this._selectedDates.Contains(date);
                    }
                    else
                    {
                        cellInfo.Selected = false;
                    }
                }

                columnIncrementer++;

                if (columnIncrementer >= Columns)
                {
                    columnIncrementer = 0;
                    rowIncrementer++;
                }
            }
        }

        canvas.Clear();

        using (var unavailablePaint = new SKPaint())
        using (var availablePaint = new SKPaint())
        using (var selectedPaint = new SKPaint())
        using (var separatorPaint = new SKPaint())
        using (var fontPaint = new SKPaint())
        using (var calendarEventPaint = new SKPaint())
        using (var dateBackgroundPaint = new SKPaint())
        using (var eventTextPaint = new SKPaint())
        {
            fontPaint.Color = this.DateColor.ToSKColor();
            fontPaint.TextSize = minSize * .5f;
            fontPaint.Typeface = PlatformInfo.DefaultTypeface;
            fontPaint.IsAntialias = true;
            fontPaint.LcdRenderText = true;
            fontPaint.SubpixelText = true;

            eventTextPaint.Typeface = PlatformInfo.DefaultTypeface;
            eventTextPaint.IsAntialias = true;
            eventTextPaint.TextAlign = SKTextAlign.Center;
            eventTextPaint.LcdRenderText = true;
            eventTextPaint.SubpixelText = true;

            availablePaint.Color = this.AvailableDateColor.ToSKColor();
            unavailablePaint.Color = this.UnavailableDateColor.ToSKColor();

            selectedPaint.Color = this.SelectedDateColor.ToSKColor();
            selectedPaint.IsAntialias = true;

            calendarEventPaint.IsAntialias = true;

            separatorPaint.Color = this.SeparatorColor.ToSKColor();

            dateBackgroundPaint.Color = this.DateBackgroundColor.ToSKColor();

            dateFontPaint.Color = this.HeaderTextColor.ToSKColor();

            for (int index = 0; index < this._daysOfWeekNames.Count; index++)
            {
                canvas.DrawRect(
                    new SKRect(index * columnSize, 0f, (index * columnSize) + columnSize, dayHeaderHeight),
                    dateBackgroundPaint);
                canvas.DrawText(
                    daysOfWeekNames[index],
                    (index * columnSize) + (columnSize * .5f),
                    dateFontPaint.FontMetrics.CapHeight + ((dayHeaderHeight - dateFontPaint.FontMetrics.CapHeight) * .5f),
                    dateFontPaint);
            }

            dateFontPaint.Color = dateFontPaintColor;

            for (int i = 0; i < this._dateContainers.Count; i++)
            {
                var dateContainer = this._dateContainers?.ElementAtOrDefault(i);

                if (dateContainer != null)
                {
                    if (!dateContainer.Available)
                    {
                        canvas.DrawRect(dateContainer.Location, unavailablePaint);
                    }
                    else if (dateContainer.Available && !dateContainer.Selected)
                    {
                        canvas.DrawRect(dateContainer.Location, availablePaint);
                    }
                    else
                    {
                        canvas.DrawRect(dateContainer.Location, availablePaint);

                        var previousContainerSelected =
                            this._dateContainers?.ElementAtOrDefault(i - 1)?.Selected ?? false;
                        var nextContainerSelected = this._dateContainers?.ElementAtOrDefault(i + 1)?.Selected ?? false;

                        if (previousContainerSelected && nextContainerSelected)
                        {
                            canvas.DrawRect(
                                new SKRect(
                                    dateContainer.Location.Left,
                                    dateContainer.Location.MidY - halfSelectionSize,
                                    dateContainer.Location.Right,
                                    dateContainer.Location.MidY + halfSelectionSize),
                                selectedPaint);
                        }
                        else if (previousContainerSelected)
                        {
                            using (var selectionCircle = new SKPath())
                            using (var selectionRect = new SKPath())
                            {
                                selectionCircle.AddCircle(dateContainer.Location.MidX, dateContainer.Location.MidY,
                                    halfSelectionSize);
                                selectionRect.AddRect(new SKRect(
                                    dateContainer.Location.Left,
                                    dateContainer.Location.MidY - halfSelectionSize,
                                    dateContainer.Location.Right - (dateContainer.Location.Width * .5f),
                                    dateContainer.Location.MidY + halfSelectionSize));

                                using (var selectionBackground = selectionRect.Op(selectionCircle, SKPathOp.Union))
                                {
                                    canvas.DrawPath(selectionBackground, selectedPaint);
                                }
                            }
                        }
                        else if (nextContainerSelected)
                        {
                            using (var selectionCircle = new SKPath())
                            using (var selectionRect = new SKPath())
                            {
                                selectionCircle.AddCircle(dateContainer.Location.MidX, dateContainer.Location.MidY,
                                    halfSelectionSize);
                                selectionRect.AddRect(new SKRect(
                                    dateContainer.Location.Left + (dateContainer.Location.Width * .5f),
                                    dateContainer.Location.MidY - halfSelectionSize,
                                    dateContainer.Location.Right,
                                    dateContainer.Location.MidY + halfSelectionSize));

                                using (var selectionBackground = selectionRect.Op(selectionCircle, SKPathOp.Union))
                                {
                                    canvas.DrawPath(selectionBackground, selectedPaint);
                                }
                            }
                        }
                        else
                        {
                            canvas.DrawCircle(dateContainer.Location.MidX, dateContainer.Location.MidY,
                                halfSelectionSize, selectedPaint);
                        }
                    }
                }
            }

            for (int i = 1; i < Columns; i++)
            {
                var columnLocation = i * columnSize;
                canvas.DrawLine(columnLocation, dayHeaderHeight, columnLocation, info.Height, separatorPaint);
            }

            for (int i = 1; i < Rows; i++)
            {
                var rowLocation = dayHeaderHeight + (i * rowSize);
                canvas.DrawLine(0, rowLocation, info.Width, rowLocation, separatorPaint);
            }

            var verticalPadding = 8f;
            var horizontalPadding = 10f;

            while (dayOfMonth <= daysInMonth)
            {
                var dayOfMonthText = dayOfMonth.ToString();

                var columnPlacement = columnPosition * columnSize;
                var rowPlacement = rowPosition * rowSize;

                switch (this.CalendarDayDisplayLocation)
                {
                    case CalendarDayDisplayLocationType.UpperRight:
                        fontPaint.TextSize = minSize * .25f;
                        fontPaint.TextAlign = SKTextAlign.Left;
                        fontPaint.Color = dateFontPaintColor;
                        var measured = canvas.TextSize(dayOfMonthText, fontPaint);
                        var fontXUpperRightPlacement = columnPlacement - (float)measured.Width - horizontalPadding;
                        var fontYUpperRightPlacement = -rowSize - (float)fontPaint.FontMetrics.Ascent + verticalPadding;
                        canvas.DrawText(dayOfMonthText, fontXUpperRightPlacement, dayHeaderHeight + rowPlacement + fontYUpperRightPlacement, fontPaint);
                        break;
                    case CalendarDayDisplayLocationType.Centered:
                    default:
                        fontPaint.TextSize = minSize * .5f;
                        fontPaint.TextAlign = SKTextAlign.Center;
                        fontPaint.Color = this._dateContainers[currentPosition - 1].Selected
                            ? selectedDateFontPaintColor
                            : dateFontPaintColor;
                        var fontXCenteredPlacement = columnSize * .5f;
                        var fontYCenteredPlacement = (rowSize - fontPaint.FontMetrics.CapHeight) * .5f;
                        canvas.DrawText(dayOfMonthText, columnPlacement - fontXCenteredPlacement,
                            dayHeaderHeight + rowPlacement - fontYCenteredPlacement, fontPaint);
                        break;
                }

                var currentDate = new DateTime(currentYear, currentMonth, dayOfMonth);
                var calendarEventsForDay = this.Events.Where(x => x.EventDate.Date.Equals(currentDate)).ToList();

                if (calendarEventsForDay?.Any() ?? false)
                {
                    var largeEvent = calendarEventsForDay.FirstOrDefault(x =>
                        x.CalendarEventDisplay == CalendarEventDisplayType.LargeEvent);

                    if (largeEvent != null)
                    {
                        calendarEventPaint.Color = largeEvent.Color != default(Color)
                            ? largeEvent.Color.ToSKColor()
                            : SKColors.Transparent;

                        // We basically have to force it to this size to make it work right
                        dateFontPaint.TextSize = minSize * .25f;
                        var circleSize = Math.Abs(dateFontPaint.FontMetrics.Ascent) * .5f;
                        var halfCircleSize = circleSize * .5f;

                        canvas.DrawCircle(
                            columnPlacement - columnSize + circleSize + verticalPadding,
                            dayHeaderHeight + rowPlacement - rowSize + circleSize + horizontalPadding,
                            circleSize, calendarEventPaint);

                        if (!string.IsNullOrEmpty(largeEvent.DisplayText))
                        {
                            eventTextPaint.Color = largeEvent.TextColor.ToSKColor();
                            eventTextPaint.TextSize = dateFontPaint.TextSize * .8f;

                            var fontSizeGood = false;

                            var x = columnPlacement - columnSize + (columnSize * .5f);
                            var y = dayHeaderHeight + rowPlacement - rowSize + (circleSize * 2f) + verticalPadding;
                            var yAvailable = rowSize - (circleSize * 2f) - (verticalPadding * 2f);

                            Size textSize = Size.Zero;

                            while (!fontSizeGood)
                            {
                                textSize = canvas.TextSize(largeEvent.DisplayText, eventTextPaint);

                                if (textSize.Width <= columnSize - (horizontalPadding * 2f) &&
                                    textSize.Height <= yAvailable)
                                {
                                    fontSizeGood = true;
                                    break;
                                }

                                eventTextPaint.TextSize = eventTextPaint.TextSize - .5f;
                            }

                            canvas.DrawMultiLineText(
                                largeEvent.DisplayText,
                                x, y + verticalPadding + ((yAvailable - (float)textSize.Height) * .5f),
                                eventTextPaint);
                        }
                    }
                    else
                    {
                        var count = calendarEventsForDay.Count;

                        var fontYPlacement = (rowSize - fontPaint.FontMetrics.CapHeight) * .5f;

                        var standardEventSize = fontYPlacement * .25f;

                        for (int i = 0; i < count; i++)
                        {
                            var calendarEvent = calendarEventsForDay[i];
                            calendarEventPaint.Color = calendarEvent.Color != default(Color)
                                ? calendarEvent.Color.ToSKColor()
                                : SKColors.OrangeRed;
                            var columnOffset = columnSize * ((i + 1f) / (float)(count + 1));
                            canvas.DrawCircle(
                                columnPlacement - columnOffset,
                                dayHeaderHeight + rowPlacement - (fontYPlacement * .5f), standardEventSize,
                                calendarEventPaint);

                            if (!string.IsNullOrEmpty(calendarEvent.DisplayText))
                            {
                                eventTextPaint.Color = calendarEvent.TextColor.ToSKColor();
                                eventTextPaint.TextSize = standardEventSize;
                                canvas
                                    .DrawText(
                                        calendarEvent.DisplayText,
                                        columnPlacement - columnOffset,
                                        dayHeaderHeight + rowPlacement + (eventTextPaint.FontMetrics.XHeight * .5f) - (fontYPlacement * .5f),
                                        eventTextPaint);
                            }
                        }
                    }
                }

                dayOfMonth++;
                currentPosition++;
                columnPosition++;

                if (columnPosition > 7)
                {
                    columnPosition = 1;
                    rowPosition++;
                }
            }
        }
    }

    /// <summary>
    /// Handles touch events on the canvas.
    /// </summary>
    /// <param name="e">The event arguments that contain the touch information.</param>
    protected override void OnTouch(SKTouchEventArgs e)
    {
        e.Handled = true;

        var updatedContainers = false;

        var startingDayOfWeek = (int)new DateTime(this.CurrentYear, this.CurrentMonth, 1).DayOfWeek;

        if (startingDayOfWeek == 7)
        {
            startingDayOfWeek = 1;
        }
        else
        {
            startingDayOfWeek++;
        }

        if (e.ActionType == SKTouchAction.Pressed && e.InContact)
        {
            for (int i = 0; i < _dateContainers.Count; i++)
            {
                if (_dateContainers[i] != null &&
                    (_dateContainers[i]?.Location.Contains(e.Location) ?? false) &&
                    (_dateContainers[i]?.Available ?? false))
                {
                    if (SelectionType == CalendarSelectionType.Single || SelectionType == CalendarSelectionType.Span)
                    {
                        _selectedDates?.Clear();

                        foreach (var dateContainer in _dateContainers)
                        {
                            dateContainer.Selected = false;
                        }
                    }

                    _wasSelected = _dateContainers[i].Selected = !_dateContainers[i].Selected;

                    var comparisonDate = new DateTime(CurrentYear, CurrentMonth, i - startingDayOfWeek + 2).Date;
                    var foundItem = _selectedDates.FirstOrDefault(x => x.Date == comparisonDate);

                    if (foundItem != default(DateTime))
                    {
                        _selectedDates.Remove(foundItem);
                    }
                    else
                    {
                        _selectedDates.Add(comparisonDate);
                    }

                    _pressedIndex = i;

                    updatedContainers = true;
                    break;
                }
            }
        }
        else if (_pressedIndex.HasValue && SelectionType != CalendarSelectionType.Single &&
                 e.ActionType == SKTouchAction.Moved && e.InContact)
        {
            for (int i = 0; i < _dateContainers.Count; i++)
            {
                if (_dateContainers[i] != null &&
                    (_dateContainers[i]?.Location.Contains(e.Location) ?? false) &&
                    (_dateContainers[i]?.Available ?? false))
                {
                    switch (SelectionType)
                    {
                        case CalendarSelectionType.Span:
                            var firstSelectedIndex = Math.Min(_pressedIndex.Value, i);
                            var lastSelectedIndex = Math.Max(_pressedIndex.Value, i);

                            for (int index = 0; index < _dateContainers.Count; index++)
                            {
                                if (!(_dateContainers?.ElementAtOrDefault(index)?.Available ?? true))
                                {
                                    continue;
                                }

                                var date = new DateTime(CurrentYear, CurrentMonth, index - startingDayOfWeek + 2).Date;
                                var foundDate = _selectedDates.FirstOrDefault(x =>
                                    x.Date == new DateTime(CurrentYear, CurrentMonth, index - startingDayOfWeek + 2)
                                        .Date);

                                var isWithinSelection = index >= firstSelectedIndex && index <= lastSelectedIndex;
                                _dateContainers.ElementAt(index).Selected = isWithinSelection;

                                if (foundDate.Date != date.Date && isWithinSelection)
                                {
                                    _selectedDates.Add(date);
                                }

                                if (foundDate.Date != date.Date && !isWithinSelection)
                                {
                                    _selectedDates.Remove(date);
                                }
                            }

                            updatedContainers = true;
                            break;
                        case CalendarSelectionType.Multiple:
                            var minIndex = Math.Min(i, _pressedIndex ?? -1);
                            var maxIndex = Math.Max(i, _pressedIndex ?? -1);

                            if (minIndex >= 0 && maxIndex >= 0)
                            {
                                var currIndex = minIndex;
                                foreach (var dateContainer in _dateContainers?.GetRange(minIndex, maxIndex - minIndex + 1))
                                {
                                    dateContainer.Selected = _wasSelected;

                                    var comparisonDate = new DateTime(CurrentYear, CurrentMonth,
                                        currIndex - startingDayOfWeek + 2).Date;
                                    var foundItem = _selectedDates.FirstOrDefault(x => x.Date == comparisonDate);

                                    if (foundItem != default(DateTime) && !_wasSelected)
                                    {
                                        _selectedDates.Remove(foundItem);
                                    }
                                    else if (foundItem == default(DateTime) && _wasSelected)
                                    {
                                        _selectedDates.Add(comparisonDate);
                                    }

                                    currIndex++;
                                }
                            }

                            updatedContainers = true;
                            break;
                    }
                }
            }
        }
        else
        {
            _pressedIndex = null;
        }

        if (updatedContainers)
        {
            base.OnPropertyChanged(SelectedDatesProperty.PropertyName);
            this.InvalidateSurface();
        }
    }

    /// <summary>
    /// Selecteds the dates collection changed.
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="e">NotifyCollectionChangedEventArgs.</param>
    private void SelectedDates_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        lock (_lock)
        {
            _currentCalendarNeedsRebuild = true;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (SelectionType == CalendarSelectionType.Single)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            for (int i = SelectedDates.Count - 1; i >= 0; i--)
                            {
                                if (i != e.NewStartingIndex)
                                {
                                    this._selectedDates.RemoveAt(i);
                                }
                            }
                        });
                    }

                    break;
                default:
                    break;
            }

            InvalidateSurface();

            CalendarSelectedDatesChanged?.Invoke(this, new CalendarSelectedDatesChangedEventArgs(SelectedDates?.ToList()));
        }
    }

    /// <summary>
    /// Class that represents the date cell information.
    /// </summary>
    public class CellInfo
    {
        /// <summary>
        /// Gets or sets the location of the date cell.
        /// </summary>
        /// <value>The location.</value>
        public SKRect Location { get; set; } = SKRect.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:Aurora.Controls.CalendarView.CellInfo"/> is selected.
        /// </summary>
        /// <value><c>true</c> if selected; otherwise, <c>false</c>.</value>
        public bool Selected { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:Aurora.Controls.CalendarView.CellInfo"/> is available.
        /// </summary>
        /// <value><c>true</c> if available; otherwise, <c>false</c>.</value>
        public bool Available { get; set; }
    }
}

public enum CalendarEventDisplayType
{
    NotificationDot,
    LargeEvent,
}

public class CalendarEvent : BindableObject
{
    public static readonly BindableProperty EventDateProperty =
        BindableProperty.Create(nameof(EventDate), typeof(DateTime), typeof(CalendarEvent), default(DateTime));

    public DateTime EventDate
    {
        get => (DateTime)GetValue(EventDateProperty);
        set => SetValue(EventDateProperty, value);
    }

    public static readonly BindableProperty DisplayTextProperty =
        BindableProperty.Create(nameof(DisplayText), typeof(string), typeof(CalendarEvent), default(string));

    public string DisplayText
    {
        get => (string)GetValue(DisplayTextProperty);
        set => SetValue(DisplayTextProperty, value);
    }

    public static readonly BindableProperty ColorProperty =
        BindableProperty.Create(nameof(Color), typeof(Color), typeof(CalendarEvent), Colors.Red);

    public Color Color
    {
        get => (Color)GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    public static readonly BindableProperty TextColorProperty =
        BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(CalendarEvent), Colors.White);

    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    public static readonly BindableProperty CalendarEventDisplayProperty =
        BindableProperty.Create(nameof(CalendarEventDisplay), typeof(CalendarEventDisplayType), typeof(CalendarEvent),
            CalendarEventDisplayType.NotificationDot);

    public CalendarEventDisplayType CalendarEventDisplay
    {
        get => (CalendarEventDisplayType)GetValue(CalendarEventDisplayProperty);
        set => SetValue(CalendarEventDisplayProperty, value);
    }
}

public class CalendarSelectedDatesChangedEventArgs : EventArgs
{
    public IEnumerable<DateTime> SelectedDates { get; private set; }

    public CalendarSelectedDatesChangedEventArgs(IEnumerable<DateTime> selectedDates)
    {
        SelectedDates = selectedDates;
    }
}
