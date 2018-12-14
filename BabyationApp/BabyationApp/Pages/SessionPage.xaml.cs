using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace BabyationApp.Pages
{
    public partial class SessionPage : ContentPage
    {
        BabyationApp.Views.StartSessionView _startView;
        BabyationApp.Views.PumpSessionView _pumpView;
        BabyationApp.Views.ManualSessionView _nurseView;
        BabyationApp.Views.BottleSessionView _bottleView;
        BabyationApp.Pages.StopwatchPage _stopwatchView;

        public SessionPage()
        {
            this.Title = "Session";
            _startView = new Views.StartSessionView();
            _pumpView = new Views.PumpSessionView();
            _nurseView = new Views.ManualSessionView();
            _bottleView = new Views.BottleSessionView();
            _stopwatchView = new Pages.StopwatchPage();

            Content = _startView;

            _startView.PumpClicked += _startView_PumpClicked;
            _startView.NurseClicked += _startView_NurseClicked;
            _startView.BottleClicked += _startView_BottleClicked;

            _pumpView.CancelClicked += _pumpView_CancelClicked;
            _nurseView.CancelClicked += _nurseView_CancelClicked;
            _bottleView.CancelClicked += _bottleView_CancelClicked;
            _bottleView.MilkClicked += _bottleFView_MilkClicked;
            _bottleView.FormulaClicked += _bottleMView_FormulaClicked;
            _bottleView.StopwatchClicked += _bottleView_StopwatchClicked;
        }

        async private void _bottleView_StopwatchClicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(_stopwatchView);
        }

        private void _bottleMView_FormulaClicked(object sender, EventArgs e)
        {
            //Content = _bottleView;
        }

        private void _bottleFView_MilkClicked(object sender, EventArgs e)
        {
            //Content = _bottleView;
        }

        private void _bottleView_CancelClicked(object sender, EventArgs e)
        {
            this.Title = "Session";
            Content = _startView;
        }

        private void _nurseView_CancelClicked(object sender, EventArgs e)
        {
            this.Title = "Session";
            Content = _startView;
        }

        private void _pumpView_CancelClicked(object sender, EventArgs e)
        {
            this.Title = "Session";
            Content = _startView;
        }

        private void _startView_BottleClicked(object sender, EventArgs e)
        {
            this.Title = "Bottle Session";
            Content = _bottleView;
        }

        private void _startView_NurseClicked(object sender, EventArgs e)
        {
            this.Title = "Nursing Session";
            Content = _nurseView;
        }

        private void _startView_PumpClicked(object sender, EventArgs e)
        {
            this.Title = "Pumping Session";
            Content = _pumpView;
        }
    }
}
