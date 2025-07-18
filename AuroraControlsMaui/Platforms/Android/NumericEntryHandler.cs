using Android.Text;
using AndroidX.AppCompat.Widget;
using Java.Lang;
using Microsoft.Maui.Handlers;

namespace AuroraControls;

public partial class NumericEntryHandler : EntryHandler, IDisposable
{
#nullable enable
    private IInputFilter[]? _startingInputFilters;
#nullable disable

    private IInputFilter _numericInputFilter;

    protected override void ConnectHandler(AppCompatEditText platformView)
    {
        base.ConnectHandler(platformView);

        platformView.InputType = InputTypes.NumberFlagDecimal;

        _startingInputFilters = platformView.GetFilters();

        _numericInputFilter = new NumericInputFilter(NumericEntryVirtualView);

        var allInputFilters = new List<IInputFilter>();
        allInputFilters.AddRange(_startingInputFilters ?? Enumerable.Empty<IInputFilter>());
        allInputFilters.Add(_numericInputFilter);

        platformView.SetFilters(allInputFilters.ToArray());
    }

    protected override void DisconnectHandler(AppCompatEditText platformView)
    {
        platformView.SetFilters(_startingInputFilters);

        _numericInputFilter?.Dispose();

        base.DisconnectHandler(platformView);
    }

    public class NumericInputFilter : Java.Lang.Object, IInputFilter
    {
        private static readonly Java.Lang.String _emptyJavaString = new(string.Empty);

        private NumericEntry _numericEntry;

        public NumericInputFilter(NumericEntry numericEntry) => _numericEntry = numericEntry;

        public ICharSequence FilterFormatted(ICharSequence source, int start, int end, ISpanned dest, int dstart, int dend)
        {
            string originalSource = source.ToString();
            string originalDest = dest.ToString();

            if (string.IsNullOrEmpty(originalDest) && string.IsNullOrEmpty(originalSource))
            {
                return null;
            }

            if (dend > dstart)
            {
                originalDest = originalDest.Remove(dstart, dend - dstart);
            }

            string final = originalDest.Insert(dstart, originalSource);

            return IsValid(final, _numericEntry.CultureInfo, _numericEntry.ValueType)
                ? null
                : _emptyJavaString;
        }

        protected override void Dispose(bool disposing)
        {
            _numericEntry = null;

            base.Dispose(disposing);
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _numericInputFilter?.Dispose();
        }
    }

    ~NumericEntryHandler() => Dispose(false);
}
