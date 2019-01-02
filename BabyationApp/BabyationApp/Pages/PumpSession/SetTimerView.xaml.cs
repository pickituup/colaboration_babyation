using System;
using System.Windows.Input;
using BabyationApp.Common;
using Xamarin.Forms;

namespace BabyationApp.Pages.PumpSession
{
    public delegate void AutoShutOffTimerHandler(TimeSpan time);

    public partial class SetTimerView : ContentView
    {
        public event AutoShutOffTimerHandler OnAutoShutOffTimerSet;

        public SetTimerView()
        {
            InitializeComponent();
        }

        public void Reset() => autoShutOffTimeEntry.Text = string.Empty;

        void Handle_Clicked(object sender, System.EventArgs e)
        {
            var timeSpan = ParseDurationTime(autoShutOffTimeEntry.Text);

            if (timeSpan.HasValue)
            { 
                OnAutoShutOffTimerSet?.Invoke(timeSpan.Value);
            }
        }

        private TimeSpan? ParseDurationTime(string timeStr)
        {
            var timeArray = timeStr.Split(':');

            if (int.TryParse(timeArray[0], out int minutes) && int.TryParse(timeArray[1], out int seconds))
            {
                return new TimeSpan(0, minutes, seconds);
            }

            return null;
        }
    }
}
