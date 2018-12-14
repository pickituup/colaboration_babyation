using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BabyationApp.Managers;
using BabyationApp.Models;
using Xamarin.Forms;

namespace BabyationApp.Pages.Settings.PumpSettings
{
    /// <summary>
    /// This class represents the Bluetooth Pump Setup page from the design
    /// </summary>
    public partial class BluetoothPumpSetupPage : PageBase
    {
        /// <summary>
        /// Constructor -- Initialize the model and binds buttons events and other ui actions
        /// </summary>
        public BluetoothPumpSetupPage()
        {
            InitializeComponent();

            Title = "ADD A PUMP";
            Titlebar.IsVisible = true;
            Titlebar.LeftButton.IsVisible = true;
            Titlebar.LeftButton.SetDynamicResource(StyleProperty, "CloseButton");
            LeftPageType = typeof(MyPumpsPage);
            Titlebar.RightButton.IsVisible = true;
            Titlebar.RightButton.Text = "HELP";

            BtnAdd.Clicked += (s, e) =>
            {
                if (_pumpModel != null)
                {
                    _pumpModel.Name = EntryPumpName.Text;
                    try
                    {
                        PumpManager.Instance.Pair(_pumpModel);
                    }
                    catch
                    {
                    }
                    PageManager.Me.SetCurrentPage(typeof(MyPumpsPage));
                }
            };
        }

        private PumpModel _pumpModel;
        /// <summary>
        /// Sets the BL pump model to setup in this page
        /// </summary>
        /// <param name="model"></param>
        public void SetPump(PumpModel model)
        {
            //if (_pumpModel != null)
            //{
            //}

            _pumpModel = model;

            //if (_pumpModel != null)
            //{
            //}

            BindingContext = model;
        }

        /// <summary>
        /// Gets called when this page is about to show and performs the initialization
        /// </summary>
        public override void AboutToShow()
        {
            base.AboutToShow();

            EntryPumpName.Text = "";

            if (PumpManager.Instance.SelectedPump != null)
            {
                SetPump(PumpManager.Instance.SelectedPump);
            }  
        }
    }
}
