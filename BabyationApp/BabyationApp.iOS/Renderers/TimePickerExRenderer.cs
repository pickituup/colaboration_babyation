using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BabyationApp.iOS.Renderers;
using BabyationApp.Controls.Views;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(TimePickerEx), typeof(TimePickerExRenderer))]
namespace BabyationApp.iOS.Renderers
{
    public class TimePickerExRenderer : TimePickerRenderer
    {
        private TimePickerEx _timePicker;
        protected override void OnElementChanged(ElementChangedEventArgs<TimePicker> e)
        {
            base.OnElementChanged(e);
            var timePicker = (UIDatePicker)Control.InputView;            

            if (e.OldElement != null)
            {
                var toolbar = (UIToolbar)Control.InputAccessoryView;
                if (toolbar != null && toolbar.Items.Length > 1)
                {
                    _timePicker = null;
                    var doneBtn = toolbar.Items[1];
                    doneBtn.Clicked -= OnDoneClicked;
                }
            }

            if (e.NewElement != null)
            {
                _timePicker = this.Element as TimePickerEx;
                timePicker.Locale = _timePicker.Is24HourView ? new NSLocale("no_nb") : timePicker.Locale;
                var toolbar = (UIToolbar)Control.InputAccessoryView;
                if (toolbar != null && toolbar.Items.Length > 1)
                {
                    var doneBtn = toolbar.Items[1];
                    doneBtn.Clicked += OnDoneClicked;
                }
            }
        }

        private void OnDoneClicked(object sender, EventArgs e)
        {
            if (_timePicker != null)
            {
                _timePicker.FireOKEvent();
            }
        }
    }
}
