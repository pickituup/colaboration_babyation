using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using Xamarin.Forms;

namespace BabyationApp.Pages
{
    public partial class StopwatchPage : ContentPage
    {
        public event EventHandler TimeSaved;
        public event EventHandler Dismissed;

        private Stopwatch _watch;

        public StopwatchPage()
        {
            InitializeComponent();

            _watch = new Stopwatch();

            Device.StartTimer(TimeSpan.FromMilliseconds(100), () =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    stopWatchTime.Text = _watch.Elapsed.ToString();
                });
                return true;
            });


            startBtn.Clicked += StartBtn_Clicked;
            stopBtn.Clicked += StopBtn_Clicked;
            saveBtn.Clicked += SaveBtn_Clicked;
        }

        private void SaveBtn_Clicked(object sender, EventArgs e)
        {
            EventHandler handler = TimeSaved;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void StopBtn_Clicked(object sender, EventArgs e)
        {
            _watch.Stop();
        }

        private void StartBtn_Clicked(object sender, EventArgs e)
        {
            _watch.Start();
        }
    }
}
