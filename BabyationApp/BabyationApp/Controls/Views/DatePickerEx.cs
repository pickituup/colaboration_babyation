using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BabyationApp.Controls.Views
{
    /// <summary>
    /// Date Picker Extended used to customized the platform dependent control through renderers
    /// </summary>
    public class DatePickerEx : DatePicker
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
    }
}
