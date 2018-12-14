using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BabyationApp.Common;
using BabyationApp.Managers;
using BabyationApp.Models;
using Xamarin.Forms;

namespace BabyationApp.Pages.Settings.PumpSettings
{
    /// <summary>
    /// This class represents the Pump Test page from the design
    /// </summary>
    public partial class PumpTestPage : PageBase
    {
        /// <summary>
        /// Constructor -- Initialize the model and binds buttons events and other ui actions
        /// </summary> 
        public PumpTestPage()
        {
            InitializeComponent();

            Title = "MY PUMPS";
            Titlebar.IsVisible = true;
            Titlebar.LeftButton.IsVisible = true;
            Titlebar.LeftButton.SetDynamicResource(StyleProperty, "CloseButton");
            LeftPageType = typeof(MyPumpsPage);
            Titlebar.RightButton.IsVisible = true;
            Titlebar.RightButton.Text = "HELP";

            this.SizeChanged += (s, e) =>
            {
                UpdateTagSize();
            };

            _model = new PumpTestModel();
            BindingContext = _model;

            BtnCalibrateLevelSensors.Clicked += (sender, args) =>
            {
                BtnCalibPt1.IsInteractable = true;
                BtnCalibPt1.BackgroundColorNormal = Color.FromHex("#EE4041");
                BtnCalibPt2.IsInteractable = true;
                BtnCalibPt2.BackgroundColorNormal = Color.FromHex("#EE4041");
                _model.ShowLevelCalibration = true;
            };

            BtnCalibPt1.Clicked += (sender, args) =>
            {
                BtnCalibPt1.IsInteractable = false;
                BtnCalibPt1.BackgroundColorNormal = Color.Gray;
                _model.Model.SetcalibrationPoint1();
                ShowSavedPopup();
            };

            BtnCalibPt2.Clicked += (sender, args) =>
            {
                BtnCalibPt2.IsInteractable = false;
                BtnCalibPt2.BackgroundColorNormal = Color.Gray;
                _model.Model.SetcalibrationPoint2();
                ShowSavedPopup();
            };
        }

        DeviceTimer _timer = new DeviceTimer();
        /// <summary>
        /// Shows the saved popup
        /// </summary>
        private void ShowSavedPopup()
        {
            if (!BtnCalibPt1.IsInteractable && !BtnCalibPt2.IsInteractable)
            {
                _model.ShowSavedPopup = true;
                UpdateTitlebarInfo(false, Color.FromHex("#11442B"));

                _timer.Enable = true;
                _timer.Start(() =>
                {
                    PageManager.Me.SetCurrentPage(typeof(PumpDetailPage));
                    return false;
                });
            }
        }


        private PumpTestModel _model;
        /// <summary>
        /// Updates the tag size
        /// </summary>
        private void UpdateTagSize()
        {
            if (_model != null)
            {
                _model.BtnHeight = this.Height / 9;
            }
        }

        /// <summary>
        /// Gets called when this page is about to show and performs the initialization
        /// </summary>
        public override void AboutToShow()
        {
            _model.Reset();
            base.AboutToShow();
            _model.Model = PumpManager.Instance.SelectedPump;     
        }
    }

    /// <summary>
    /// This class is the UI model to the PumpTestPage
    /// </summary>
    class PumpTestModel : ObservableObject
    {
        /// <summary>
        /// Resets the model to its initial state
        /// </summary>
        public void Reset()
        {
            ShowLevelCalibration = false;
            ShowSavedPopup = false;
        }


        private double _btnHeight;
        public double BtnHeight
        {
            get { return _btnHeight; }
            set => SetPropertyChanged(ref _btnHeight, value);
        }

        private PumpModel _model;
        /// <summary>
        /// The pump model to test on the PumpTestPage
        /// </summary>
        public PumpModel Model
        {
            get { return _model;}
            set => SetPropertyChanged(ref _model, value);
        }

        private bool _showLevelCalibration = false;
        /// <summary>
        /// Gets/Sets whether to show level calibaration ui part of the page PumpTestPage
        /// </summary>
        public bool ShowLevelCalibration
        {
            get
            {
                return _showLevelCalibration;
            }
            set => SetPropertyChanged(ref _showLevelCalibration, value);
        }

        private bool _showSavedPopup = false;
        /// <summary>
        /// Shows/Hides the saved popup on PumpTestPage
        /// </summary>
        public bool ShowSavedPopup
        {
            get
            {
                return _showSavedPopup;
            }
            set => SetPropertyChanged(ref _showSavedPopup, value);
        }
    }
}
