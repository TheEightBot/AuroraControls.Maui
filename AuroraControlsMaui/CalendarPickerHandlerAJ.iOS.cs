using System;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using UIKit;

namespace AuroraControls
{
#if !MACCATALYST
    public partial class CalendarPickerHandlerAJ : ViewHandler<IDatePicker, MauiDatePicker>
    {
        protected override MauiDatePicker CreatePlatformView()
        {
            MauiDatePicker platformDatePicker = new MauiDatePicker();
            return platformDatePicker;
        }

        internal UIDatePicker? DatePickerDialog
        {
            get { return PlatformView?.InputView as UIDatePicker; }
        }

        internal bool UpdateImmediately { get; set; }

        protected override void ConnectHandler(MauiDatePicker platformView)
        {
            ////platformView.MauiDatePickerDelegate = new DatePickerDelegate(this);

            if (DatePickerDialog is UIDatePicker picker)
            {
                var date = VirtualView?.Date;
                if (date is DateTime dt)
                {
                    picker.Date = dt.ToNSDate();
                }
            }

            base.ConnectHandler(platformView);
        }

        protected override void DisconnectHandler(MauiDatePicker platformView)
        {
            //// platformView.MauiDatePickerDelegate = null;

            //// base.DisconnectHandler(platformView);
        }

        public static partial void MapFormat(IDatePickerHandler handler, IDatePicker datePicker)
        {
            var picker = (handler as CalendarPickerHandlerAJ)?.DatePickerDialog;
            handler.PlatformView?.UpdateFormat(datePicker, picker);
        }

        public static partial void MapDate(IDatePickerHandler handler, IDatePicker datePicker)
        {
            var picker = (handler as CalendarPickerHandlerAJ)?.DatePickerDialog;
            handler.PlatformView?.UpdateDate(datePicker, picker);
        }

        public static partial void MapMinimumDate(IDatePickerHandler handler, IDatePicker datePicker)
        {
            if (handler is CalendarPickerHandlerAJ platformHandler)
            {
                handler.PlatformView?.UpdateMinimumDate(datePicker, platformHandler.DatePickerDialog);
            }
        }

        public static partial void MapMaximumDate(IDatePickerHandler handler, IDatePicker datePicker)
        {
            if (handler is CalendarPickerHandlerAJ platformHandler)
            {
                handler.PlatformView?.UpdateMaximumDate(datePicker, platformHandler.DatePickerDialog);
            }
        }

        public static partial void MapCharacterSpacing(IDatePickerHandler handler, IDatePicker datePicker)
        {
            handler.PlatformView?.UpdateCharacterSpacing(datePicker);
        }

        public static partial void MapFont(IDatePickerHandler handler, IDatePicker datePicker)
        {
            //// var fontManager = handler.GetRequiredService<IFontManager>();

            //// handler.PlatformView?.UpdateFont(datePicker, fontManager);
        }

        public static partial void MapTextColor(IDatePickerHandler handler, IDatePicker datePicker)
        {
            handler.PlatformView?.UpdateTextColor(datePicker);
        }

        public static partial void MapFlowDirection(CalendarPickerHandlerAJ handler, IDatePicker datePicker)
        {
            handler.PlatformView?.UpdateFlowDirection(datePicker);
            handler.PlatformView?.UpdateTextAlignment(datePicker);
        }

        private static void OnValueChanged(object? sender)
        {
            if (sender is CalendarPickerHandlerAJ datePickerHandler)
            {
                // Platform Specific
                if (datePickerHandler.UpdateImmediately)
                {
                    datePickerHandler.SetVirtualViewDate();
                }

                if (datePickerHandler.VirtualView != null)
                {
                    datePickerHandler.VirtualView.IsFocused = true;
                }
            }
        }

        private static void OnStarted(object? sender)
        {
            if (sender is IDatePickerHandler datePickerHandler && datePickerHandler.VirtualView != null)
            {
                datePickerHandler.VirtualView.IsFocused = true;
            }
        }

        private static void OnEnded(object? sender)
        {
            if (sender is IDatePickerHandler datePickerHandler && datePickerHandler.VirtualView != null)
            {
                datePickerHandler.VirtualView.IsFocused = false;
            }
        }

        private static void OnDoneClicked(object? sender)
        {
            if (sender is CalendarPickerHandlerAJ handler)
            {
                handler.SetVirtualViewDate();
                handler.PlatformView.ResignFirstResponder();
            }
        }

        private void SetVirtualViewDate()
        {
            if (VirtualView == null || DatePickerDialog == null)
            {
                return;
            }

            VirtualView.Date = DatePickerDialog.Date.ToDateTime().Date;
        }

        internal class DatePickerDelegate : CalendarPickerDelegate
        {
            private readonly WeakReference<IDatePickerHandler> _handler;

            public DatePickerDelegate(IDatePickerHandler handler) =>
                _handler = new WeakReference<IDatePickerHandler>(handler);

            private IDatePickerHandler? Handler
            {
                get
                {
                    if (_handler?.TryGetTarget(out IDatePickerHandler? target) == true)
                    {
                        return target;
                    }

                    return null;
                }
            }

            public override void DatePickerEditingDidBegin()
            {
                CalendarPickerHandlerAJ.OnStarted(Handler);
            }

            public override void DatePickerEditingDidEnd()
            {
                CalendarPickerHandlerAJ.OnEnded(Handler);
            }

            public override void DatePickerValueChanged()
            {
                CalendarPickerHandlerAJ.OnValueChanged(Handler);
            }

            public override void DoneClicked()
            {
                CalendarPickerHandlerAJ.OnDoneClicked(Handler);
            }
        }
    }
#endif
}
