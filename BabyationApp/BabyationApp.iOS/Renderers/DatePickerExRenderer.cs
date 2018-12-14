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

[assembly: ExportRenderer(typeof(DatePickerEx), typeof(DatePickerExRenderer))]
namespace BabyationApp.iOS.Renderers
{
    public class DatePickerExRenderer : DatePickerRenderer
    {
        private DatePickerEx _datePicker = null;
        protected override void OnElementChanged(ElementChangedEventArgs<DatePicker> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                var toolbar = (UIToolbar)Control.InputAccessoryView;
                if (toolbar != null && toolbar.Items.Length > 1)
                {
                    _datePicker = null;
                    var doneBtn = toolbar.Items[1];                    
                    doneBtn.Clicked -= OnDoneClicked;
                }
            }

            if (e.NewElement != null)
            {
                _datePicker = this.Element as DatePickerEx;
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
            if (_datePicker != null)
            {
                _datePicker.FireOKEvent();
            }
        }
    }
}

