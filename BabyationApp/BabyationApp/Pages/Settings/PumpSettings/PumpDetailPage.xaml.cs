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
using BabyationApp.Resources;

namespace BabyationApp.Pages.Settings.PumpSettings
{
    /// <summary>
    /// This class represents the Pump Details page from the design
    /// </summary>
    public partial class PumpDetailPage : PageBase
    {
        private DeviceTimer _timer = new DeviceTimer();
        private string _pumpName;
        private PumpModel _pumpModel = null;

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

            _keepPumpButton_ImageButton.Command = new Command(() => IsForgetPumbDialogActive = false);

            _removePumpButton_ImageButton.Command = new Command(() =>
            {
                if (_pumpModel != null)
                {
                    PumpManager pumpManager = PumpManager.Instance;
                    pumpManager.Remove(_pumpModel);
                    PageManager.Me.SetCurrentPage(typeof(MyPumpsPage));

                    IsForgetPumbDialogActive = false;
                }
            });

            _nixFactoryResetButton_ImageButton.Command = new Command(() => IsFactoryResetDialogActive = false);

            _applyFactoryResetButton_ImageButton.Command = new Command(() =>
            {
                if (_pumpModel != null)
                {
                    ///
                    /// TODO: factory reset logic. Current behavior simply navigates back to the `my pumps page`
                    /// 
                    PageManager.Me.SetCurrentPage(typeof(MyPumpsPage));
                    IsFactoryResetDialogActive = false;
                }
            });

            //BtnUpdateFW.Clicked += (sender, args) =>
            //{
            //    if (_pumpModel != null)
            //    {
            //        _pumpModel.UpdateFirmware();
            //    }
            //};

            //int pumpNameTapListCounter = 0;
            //bool isBtnNameReleased = true;

            //BtnPumpName.BackgroundView.TapStarted += (sender, args) =>
            //{
            //    pumpNameTapListCounter++;
            //    isBtnNameReleased = false;
            //    _timer.Enable = true;
            //    _timer.Start(10, () =>
            //    {
            //        pumpNameTapListCounter--;
            //        if (!isBtnNameReleased && pumpNameTapListCounter == 0)
            //        {
            //            PageManager.Me.SetCurrentPage(typeof(PumpTestPage));
            //        }
            //        return false;
            //    });
            //};
            //BtnPumpName.BackgroundView.TapUp += (sender, press) =>
            //{
            //    if (press.Cancelled) {
            //        isBtnNameReleased = true;
            //    }
            //};

            Title = AppResource.EditMyPumpUppercase;
            Titlebar.IsVisible = true;
            Titlebar.LeftButton.IsVisible = true;
            Titlebar.LeftButton.SetDynamicResource(StyleProperty, "BackButton");
            LeftPageType = typeof(MyPumpsPage);
        }

        private bool _isForgetPumbDialogActive;
        public bool IsForgetPumbDialogActive
        {
            get => _isForgetPumbDialogActive;
            private set
            {
                _isForgetPumbDialogActive = value;

                OnIsForgetPumbDialogActive();
            }
        }

        private bool _isFactoryResetDialogActive;
        public bool IsFactoryResetDialogActive
        {
            get => _isFactoryResetDialogActive;
            private set
            {
                _isFactoryResetDialogActive = value;

                OnIsFactoryResetDialogActive();
            }
        }


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
                _pumpNameInput_EntryEx.Text = _pumpModel.Name;
                _modelNumber_Label.Text = _pumpModel.ModelNumber;
                _serialNumber_Label.Text = _pumpModel.SerialNumber;
                _hardwareRevision_Label.Text = _pumpModel.HardwareRevision;
                _firmwareRevision_Label.Text = string.Format("{0} / {1}", _pumpModel.SoftwareRevision, _pumpModel.FirmwareRevision);
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
            OnIsForgetPumbDialogActive();
            OnIsFactoryResetDialogActive();

            base.AboutToShow();
        }

        private void OnPumpNameTextChanged(object sender, TextChangedEventArgs e) => _pumpName = e.NewTextValue;

        private void OnForgetDeviceTapped(object sender, EventArgs e) => IsForgetPumbDialogActive = true;

        private void OnFactoryResetTapped(object sender, EventArgs e) => IsFactoryResetDialogActive = true;

        private void OnSaveTapped(object sender, EventArgs e)
        {
            ///
            /// TODO: pump save logic
            /// 
            if (!string.IsNullOrEmpty(_pumpName))
            {
                if (_pumpModel != null)
                {
                    _pumpModel.Name = _pumpName;
                    TogglePupmSaveOutput(true);
                }
            }
        }

        private void OnIsForgetPumbDialogActive()
        {
            if (IsForgetPumbDialogActive)
            {
                Title = AppResource.ForgetDeviceUppercase;
                Titlebar.LeftButton.IsVisible = false;

                _forgetPumpDialog_Grid.TranslationX = 0;
                _forgetPumpDialog_Grid.InputTransparent = false;
            }
            else
            {
                Title = AppResource.EditMyPumpUppercase;
                Titlebar.LeftButton.IsVisible = true;

                _forgetPumpDialog_Grid.TranslationX = long.MaxValue;
                _forgetPumpDialog_Grid.InputTransparent = true;
            }
        }

        private void OnIsFactoryResetDialogActive()
        {
            if (IsFactoryResetDialogActive)
            {
                Title = AppResource.FactoryReset.ToUpper();
                Titlebar.LeftButton.IsVisible = false;

                _factoryResetDialog_Grid.TranslationX = 0;
                _factoryResetDialog_Grid.InputTransparent = false;
            }
            else
            {
                Title = AppResource.EditMyPumpUppercase;
                Titlebar.LeftButton.IsVisible = true;

                _factoryResetDialog_Grid.TranslationX = long.MaxValue;
                _factoryResetDialog_Grid.InputTransparent = true;
            }
        }

        private void TogglePupmSaveOutput(bool isVisible)
        {
            _pumpSavedInfoOutput_AbsoluteLayout.IsVisible = isVisible;
            Titlebar.IsVisible = !isVisible;
        }

        private void OnPumpSavedCloseTapped(object sender, EventArgs e)
        {
            PageManager.Me.SetCurrentPage(typeof(MyPumpsPage));
            TogglePupmSaveOutput(false);
        }
    }
}
