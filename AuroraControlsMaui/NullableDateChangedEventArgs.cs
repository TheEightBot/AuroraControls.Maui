namespace AuroraControls;

public class NullableDateChangedEventArgs : EventArgs
{
    /// <summary>
    /// Gets the new date.
    /// </summary>
    /// <value>The new date.</value>
    public DateTime? NewDate
    {
        get;
        private set;
    }

    /// <summary>
    /// Gets the old date.
    /// </summary>
    /// <value>The old date.</value>
    public DateTime? OldDate
    {
        get;
        private set;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NullableDateChangedEventArgs"/> class.
    /// </summary>
    /// <param name="oldDate">Old date.</param>
    /// <param name="newDate">New date.</param>
    public NullableDateChangedEventArgs(DateTime? oldDate, DateTime? newDate)
    {
        this.OldDate = oldDate;
        this.NewDate = newDate;
    }
}
