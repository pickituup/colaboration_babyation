using BabyationApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BabyationApp.Common;
using BabyationApp.Managers;

using Xamarin.Forms;

namespace BabyationApp.Pages.Settings.PumpSettings
{
    /// <summary>
    /// This class represents the Pump Details page from the design
    /// </summary>
    public partial class PumpDetailPage : PageBase
    {
        private DeviceTimer _timer = new DeviceTimer();

        /// <summary>
        /// Constructor -- Initialize the model and binds buttons events and other ui actions
        /// </summary>    
        public PumpDetailPage()
        {
            InitializeComponent();            

            this.SizeChanged += (s, e) =>
            {
                UpdateTagSize();
            };

            BtnForgetPump.Clicked += (s, e) =>
            {
                PumpManager pumpManager = PumpManager.Instance;
                pumpManager.Remove(_pumpModel);
                PageManager.Me.SetCurrentPage(typeof(MyPumpsPage));
            };

            BtnUpdateFW.Clicked += (sender, args) =>
            {
                if (_pumpModel != null)
                {
                    _pumpModel.UpdateFirmware();
                }
            };

            int pumpNameTapListCounter = 0;
            bool isBtnNameReleased = true;
            BtnPumpName.BackgroundView.TapStarted += (sender, args) =>
            {
                pumpNameTapListCounter++;
                isBtnNameReleased = false;
                _timer.Enable = true;
                _timer.Start(10, () =>
                {
                    pumpNameTapListCounter--;
                    if (!isBtnNameReleased && pumpNameTapListCounter == 0)
                    {
                        PageManager.Me.SetCurrentPage(typeof(PumpTestPage));
                    }
                    return false;
                });
            };
            BtnPumpName.BackgroundView.TapUp += (sender, press) =>
            {
                if (press.Cancelled) {
                    isBtnNameReleased = true;
                }
            };

            Title = "MY PUMPS";
            Titlebar.IsVisible = true;
            Titlebar.LeftButton.IsVisible = true;
            Titlebar.LeftButton.SetDynamicResource(StyleProperty, "CloseButton");
            LeftPageType = typeof(MyPumpsPage);
            Titlebar.RightButton.IsVisible = true;
            Titlebar.RightButton.Text = "HELP";
        }

        private PumpModel _pumpModel = null;
        /// <summary>
        /// Sets the current pumpt to show details about on this page
        /// </summary>
        /// <param name="model"></param>
        public void SetPump(PumpModel model)
        {
            if (_pumpModel != null)
            {
                _pumpModel.AlertMessages.CollectionChanged -= AlertMessages_CollectionChanged;
                _pumpModel.PropertyChanged -= OnPumpModelPropertyChanged;
            }

            _pumpModel = model;
            BindingContext = _pumpModel;            

            if (_pumpModel != null)
            {
                _pumpModel.AlertMessages.CollectionChanged += AlertMessages_CollectionChanged;
                _pumpModel.PropertyChanged += OnPumpModelPropertyChanged;
            }

            SyncUpdateUIPart();
        }

        /// <summary>
        /// Handles the pump model's properties changes
        /// </summary>
        /// <param name="sender">sender of the event -- usually the model itself</param>
        /// <param name="args">event argument </param>
        private void OnPumpModelPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "IsUpdateAvailable" || args.PropertyName == "IsUpdating" || args.PropertyName == "UpdatePercent")
            {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    SyncUpdateUIPart();
                });
            }
        }

        /// <summary>
        /// Sync the UI part based on the pump model properties
        /// </summary>
        private void SyncUpdateUIPart()
        {
            if (_pumpModel != null)
            {
                LblNoUpdate.IsVisible = !_pumpModel.IsUpdateAvailable;
                BtnUpdateFW.IsVisible = _pumpModel.IsUpdateAvailable && !_pumpModel.IsUpdating;
                LblUpdatePercent.IsVisible = _pumpModel.IsUpdating;
            }
        }

        private void AlertMessages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                HandleAlerts();
            });
        }

        private void HandleAlerts()
        {
            try
            {
				if (_pumpModel != null && _pumpModel.AlertMessages != null)
                {
                    LblProblemCounter.Text = String.Format("{0} Problem{1} Identified", new Object[]
                    {
                        _pumpModel.AlertMessages.Count,
                        _pumpModel.AlertMessages.Count > 1 ? "s" : ""
                    });
					/*
                    String msg = "";
                    for (int i = 1; i <= _pumpModel.AlertMessages.Count; i++)
                    {
                        msg += (i.ToString() + ". " + _pumpModel.AlertMessages[i - 1]);
                        if (i < _pumpModel.AlertMessages.Count)
                        {
                            msg += "\n";
                        }
                    }
                    LblErrors.Text = msg;*/
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exeption: " + e.Message);
            }
        } 

        private void UpdateTagSize()
        {
            if (_pumpModel != null)
            {
                _pumpModel.TagDouble1 = this.Height / 9;
            }
        }

        /// <summary>
        /// Gets called when this page is about to show and performs the initialization
        /// </summary>
        public override void AboutToShow()
        {
            SetPump(PumpManager.Instance.SelectedPump);
            HandleAlerts();
            UpdateTagSize();
            base.AboutToShow();
        }
    }
}
