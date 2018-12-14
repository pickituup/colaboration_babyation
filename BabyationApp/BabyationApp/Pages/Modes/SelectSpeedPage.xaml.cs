using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BabyationApp.Controls.Buttons;
using BabyationApp.Managers;
using Xamarin.Forms;
using BabyationApp.Resources;

namespace BabyationApp.Pages.Modes
{
    /// <summary>
    /// This class represents the Log a Select Speed page from the design
    /// </summary>
    public partial class SelectSpeedPage : PageBase
    {
        private bool _isEditMode = false;

        /// <summary>
        /// Constructor -- Initialize the model and binds buttons events and other ui actions
        /// </summary>
        public SelectSpeedPage()
        {
            InitializeComponent();

            Titlebar.IsVisible = true;
            LeftPageType = typeof(DashboardTabPage);
            Titlebar.LeftButton.IsVisible = true;
            Titlebar.LeftButton.SetDynamicResource(StyleProperty, "CancelButton");
            Titlebar.LeftButton.Clicked += CancelButton_Clicked;

            _circleSuctionStimulation.RatioSmall = 4.5 / 20.0;
            _circleSuctionStimulation.OverlapSize = 6.0;
            _circleSpeedStimulation.RatioSmall = 4.5 / 20.0;
            _circleSpeedStimulation.OverlapSize = 6.0;
            _circleSuctionExpression.RatioSmall = 4.5 / 20.0;
            _circleSuctionExpression.OverlapSize = 6.0;
            _circleSpeedExpression.RatioSmall = 4.5 / 20.0;
            _circleSpeedExpression.OverlapSize = 6.0;

            BtnNext.Clicked += BtnNext_Clicked;

            _circleSuctionStimulation.ValueUpdated += value => { UpdateNextButtonState(); };
            _circleSuctionExpression.ValueUpdated += value => { UpdateNextButtonState(); };
            _circleSpeedStimulation.ValueUpdated += value => { UpdateNextButtonState(); };
            _circleSpeedExpression.ValueUpdated += value => { UpdateNextButtonState(); };
        }

        public void Initialize(bool isEditMode = false)
        {
            _isEditMode = isEditMode;

            String modeName = ExperienceManager.Instance.EditingExperience?.Name ?? null;

            Title = (isEditMode && !String.IsNullOrEmpty(modeName) ? String.Format("{0} {1}", AppResource.EditUpper, modeName) : AppResource.CreateNewModeUpper);
        }
        /// <summary>
        /// Gets called when this page is about to show and performs the initialization
        /// </summary>
        public override void AboutToShow()
        {
            base.AboutToShow();

            // Restore style:
            Titlebar.IsVisible = true;
            RootLayout.Style = (Style)Application.Current.Resources["Grid_NavigationOnTop"];

            if (ExperienceManager.Instance.EditingExperience != null)
            {
                _circleSuctionStimulation.Value = ExperienceManager.Instance.EditingExperience.StimulationSuction;
                _circleSuctionExpression.Value = ExperienceManager.Instance.EditingExperience.ExpressionSuction;
                _circleSpeedStimulation.Value = ExperienceManager.Instance.EditingExperience.StimulationSpeed;
                _circleSpeedExpression.Value = ExperienceManager.Instance.EditingExperience.ExpressionSpeed;
            }
        }

        void CancelButton_Clicked(object sender, EventArgs e)
        {
            // Cleanup
            ExperienceManager.Instance.EditingExperience = null;
        }

        void BtnNext_Clicked(object sender, EventArgs e)
        {
            if (ExperienceManager.Instance.EditingExperience != null)
            {
                ExperienceManager.Instance.EditingExperience.StimulationSuction = _circleSuctionStimulation.Value;
                ExperienceManager.Instance.EditingExperience.ExpressionSuction = _circleSuctionExpression.Value;
                ExperienceManager.Instance.EditingExperience.StimulationSpeed = _circleSpeedStimulation.Value;
                ExperienceManager.Instance.EditingExperience.ExpressionSpeed = _circleSpeedExpression.Value;
            }
            PageManager.Me.SetCurrentPage(typeof(EnterOtherInfoPage), View => (View as EnterOtherInfoPage).Initialize(_isEditMode));
        }


        /// <summary>
        /// Update the next button state based on the user input has given input or not
        /// </summary>
        private void UpdateNextButtonState()
        {
            BtnNext.IsEnabled = _circleSuctionExpression.IsValid() 
                && _circleSpeedExpression.IsValid()
                && _circleSuctionStimulation.IsValid() 
                && _circleSpeedStimulation.IsValid();
        }
    }
}
