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

[assembly: ExportRenderer(typeof(TimePickerEx), typeof(TimePickerExRenderer))]
namespace BabyationApp.Droid.Renderers
{
    public class TimePickerExRenderer : ViewRenderer<TimePickerEx, Android.Widget.EditText>, TimePickerDialog.IOnTimeSetListener, IJavaObject, IDisposable
    {
        public TimePickerExRenderer(Context context) : base(context)
        {
        }

        public class TimePickerDialogEx : TimePickerDialog
        {
            private TimePickerEx _TimePicker;
            public TimePickerDialogEx(TimePickerEx picker, Context context, IOnTimeSetListener callBack, int hour,
                int minute)
                : base(context, callBack, hour, minute, picker.Is24HourView)
            {
                _TimePicker = picker;
            }

            public override void OnClick(IDialogInterface dialog, int which)
            {
                base.OnClick(dialog, which);
                if (which == -1)
                {
                    _TimePicker.FireOKEvent();
                }
                else if (which == -2)
                {
                    _TimePicker.FireCancelEvent();
                }
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<TimePickerEx> e)
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

        private TimePickerDialog _dialog;

        void OnControlFocusChanged(object sender, Android.Views.View.FocusChangeEventArgs e)
        {
            if (e.HasFocus)
            {
                ShowTimePicker();
            }
        }

        void OnControlClick(object sender, EventArgs e)
        {
            ShowTimePicker();
        }

        private void ShowTimePicker()
        {
            if (_dialog == null)
            {
                _dialog = new TimePickerDialogEx(this.Element, Forms.Context, this, DateTime.Now.Hour, DateTime.Now.Minute);
            }

            _dialog.Show();
        }

        public void OnTimeSet(Android.Widget.TimePicker view, int hourOfDay, int minute)
        {
            var time = new TimeSpan(0, hourOfDay, minute, 0);
            this.Element.Time = time;
            this.Control.Text = time.ToString();
        }


    }
}