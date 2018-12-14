using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BabyationApp.Controls.Views
{
    /// <summary>
    /// Time picker extended view to customize and control the behaviors through platform dependent renderers
    /// </summary>
    public class TimePickerEx : TimePicker
    {
        /// <summary>
        /// Event fires when clicks on cancel or outside of the picker
        /// </summary>
        public event EventHandler CancelClicked;

        /// <summary>
        /// Event fires when selects a date and press on OK
        /// </summary>
        public event EventHandler OKClicked;

        /// <summary>
        /// Helper functions to let renderers to fire the CancelClicked event
        /// </summary>
        public void FireCancelEvent()
        {
            CancelClicked?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Helper functions to let renderers to fire the OKClicked event
        /// </summary>
        public void FireOKEvent()
        {
            OKClicked?.Invoke(this, EventArgs.Empty);
        }

        public static readonly BindableProperty Is24HourViewProperty = BindableProperty.Create("Is24HourView", typeof(bool), typeof(TimePickerEx), false);
        /// <summary>
        /// Gets/Sets whether the time picker should show 24 hour mode
        /// </summary>
        public bool Is24HourView
        {
            get { return (bool)GetValue(Is24HourViewProperty); }
            set { SetValue(Is24HourViewProperty, value); }
        }
    }
}
