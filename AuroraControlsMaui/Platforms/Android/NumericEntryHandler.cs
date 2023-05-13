using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Android.Text;
using AndroidX.AppCompat.Widget;
using AuroraControls.Platforms.Android;
using Java.Lang;
using Microsoft.Maui.Handlers;

namespace AuroraControls;

public partial class NumericEntryHandler : EntryHandler
{
#nullable enable
    private IInputFilter[]? _startingInputFilters;
#nullable disable

    private IInputFilter _numericInputFilter;

    protected override void ConnectHandler(AppCompatEditText platformView)
    {
        base.ConnectHandler(platformView);

        platformView.InputType = Android.Text.InputTypes.NumberFlagDecimal;

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
        private static readonly Java.Lang.String _emptyJavaString = new Java.Lang.String(string.Empty);

        private NumericEntry _numericEntry;

        public NumericInputFilter(NumericEntry numericEntry)
        {
            _numericEntry = numericEntry;
        }

        public ICharSequence FilterFormatted(ICharSequence source, int start, int end, ISpanned dest, int dstart, int dend)
        {
            var originalSource = source.ToString();
            var originalDest = dest.ToString();

            if (string.IsNullOrEmpty(originalDest) && string.IsNullOrEmpty(originalSource))
            {
                return null;
            }

            if (dend > dstart)
            {
                originalDest = originalDest.Remove(dstart, dend - dstart);
            }

            var final = originalDest.Insert(dstart, originalSource);

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
}