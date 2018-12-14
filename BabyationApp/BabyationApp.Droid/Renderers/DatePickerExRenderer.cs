using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Xamarin.Forms.Platform.Android;
using BabyationApp.Droid.Renderers;
using BabyationApp.Droid.Gestures;
using System.ComponentModel;
using System.Reflection;
using BabyationApp.Controls.Views;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(DatePickerEx), typeof(DatePickerExRenderer))]
namespace BabyationApp.Droid.Renderers
{
    public class DatePickerExRenderer : ViewRenderer<DatePickerEx, Android.Widget.EditText>, DatePickerDialog.IOnDateSetListener, IJavaObject, IDisposable
    {
        public DatePickerExRenderer(Context context) : base(context)
        {
        }

        public class DatePickerDialogEx : DatePickerDialog
        {
            private DatePickerEx _datePicker;
            public DatePickerDialogEx(DatePickerEx picker, Context context, IOnDateSetListener callBack, int year,
                int monthOfYear, int dayOfMonth)
                : base (context, callBack, year, monthOfYear, dayOfMonth)
            {
                _datePicker = picker;
            }

            public override void OnClick(IDialogInterface dialog, int which)
            {
                base.OnClick(dialog, which);
                if (which == -1)
                {
                    _datePicker.FireOKEvent();
                }
                else if (which == -2)
                {
                    _datePicker.FireCancelEvent();
                }
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<DatePickerEx> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement == null)
            {
                this.SetNativeControl(new Android.Widget.EditText(Forms.Context));
                this.Control.Click += OnControlClick;
                this.Control.Text = DateTime.Now.ToString("HH:mm");
                this.Control.KeyListener = null;
                this.Control.FocusChange += OnControlFocusChanged;
            }
        }

        private DatePickerDialog _dialog;

        void OnControlFocusChanged(object sender, Android.Views.View.FocusChangeEventArgs e)
        {
            if (e.HasFocus)
            {
                ShowDatePicker();
            }
        }

        void OnControlClick(object sender, EventArgs e)
        {
            ShowDatePicker();
        }

        private void ShowDatePicker()
        {
            if (_dialog == null)
            {
                _dialog = new DatePickerDialogEx(this.Element, Forms.Context, this, DateTime.Now.Year, DateTime.Now.Month - 1, DateTime.Now.Day);
            }

            _dialog.Show();
        }

        public void OnDateSet(Android.Widget.DatePicker view, int year, int monthOfYear, int dayOfMonth)
        {            
            var date = new DateTime(year, monthOfYear+1, dayOfMonth);
            this.Element.Date = date;
            this.Control.Text = date.ToString("MM/dd/yyyy");
        }

        
    }
}