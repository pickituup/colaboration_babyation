using BabyationApp.Managers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BabyationApp.Common;
using BabyationApp.Models;
using Xamarin.Forms;
using BabyationApp.Controls.Views;

namespace BabyationApp.Pages.Settings.PumpSettings
{
    /// <summary>
    /// This class represents the Bluetooth Pump Detected page from the design
    /// </summary>
    public partial class BluetoothPumpDetectedPage : PageBase
    {
        DeviceTimer _timer = new DeviceTimer();

        /// <summary>
        /// Constructor -- Initialize the model and binds buttons events and other ui actions
        /// </summary>
        public BluetoothPumpDetectedPage()
        {
            InitializeComponent();            

            Titlebar.IsVisible = false;

            listView.ItemsSource = new ListViewList<PumpModel>(PumpManager.Instance.NewPumps);

            listView.ItemSelected += (s, e) =>
            {
                if (e.SelectedItem != null)
                {
                    PumpManager.Instance.SelectedPump = (PumpModel)e.SelectedItem;
                    PageManager.Me.SetCurrentPage(typeof(BluetoothPumpSetupPage));
                }
            };
            BtnClose.Clicked += (sender, args) =>
            {
                _timer.Enable = false;
                PageManager.Me.SetCurrentPage(typeof(MyPumpsPage));
            };
        }

        /// <summary>
        /// Gets called when this page is about to show and performs the initialization
        /// </summary>
        public override void AboutToShow()
        {
            listView.SelectedItem = null;            
            LblTitle.Text = PumpManager.Instance.NewPumps.Count == 0 ? "NO PUMPS DETECTED" : (PumpManager.Instance.NewPumps.Count == 1 ? "PUMP DETECTED" : "PUMPS DETECTED");
            LblInfo.Text = PumpManager.Instance.NewPumps.Count == 0 ? "Move within range of a pump to activate it."
                : "You are in range of more than one pump. Which one would you like to activate?";

            //Titlebar.TitleBackColor = Color.FromHex("#083954");
            base.AboutToShow();

            if (PumpManager.Instance.NewPumps.Count == 0)
            {
                _timer.Enable = true;
                _timer.Start(() =>
                {
                    PageManager.Me.SetCurrentPage(typeof(MyPumpsPage));
                    return false;
                });
            }
        }
    }
}
