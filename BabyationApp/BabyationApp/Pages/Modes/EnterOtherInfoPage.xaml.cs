using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BabyationApp.Common;
using Xamarin.Forms;
using BabyationApp.Controls.Buttons;
using BabyationApp.Managers;
using BabyationApp.Models;
using System.Diagnostics;
using BabyationApp.Resources;
using System.Windows.Input;
using System.Dynamic;

namespace BabyationApp.Pages.Modes
{
    /// <summary>
    /// This class represents Enter Other Info page from the design
    /// </summary>
    public partial class EnterOtherInfoPage : PageBase
    {
        private bool _isEditMode = false;

        DeviceTimer _timer = new DeviceTimer();

        EnterOtherInfoModel ViewModel;
        /// <summary>
        /// Constructor -- Initialize the model and binds buttons events and other ui actions
        /// </summary>
        public EnterOtherInfoPage()
        {
            InitializeComponent();

            ViewModel = new EnterOtherInfoModel(UpdateStorageType, FinishSession);
            BindingContext = ViewModel;

            Titlebar.IsVisible = true;
            LeftPageType = typeof(DashboardTabPage);
            Titlebar.LeftButton.IsVisible = true;
            Titlebar.LeftButton.SetDynamicResource(StyleProperty, "CancelButton");
            Titlebar.LeftButton.Clicked += CancelButton_Clicked;

            BtnBack.Clicked += BtnBack_Clicked;
            BtnSave.Clicked += BtnSave_Clicked;
            EntryName.TextChanged += EntryName_TextChanged;
        }

        public void Initialize(bool isEditMode = false)
        {
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
            RootLayout.Style = (Style)Application.Current.Resources["AbsoluteLayout_NavigationOnTop"];

            ViewModel.Reset();
            
            if (ExperienceManager.Instance.EditingExperience != null)
            {
                EntryName.Text = ExperienceManager.Instance.EditingExperience.Name;
                EntryDesc.Text = ExperienceManager.Instance.EditingExperience.Description;
                ViewModel.MilkType = ExperienceManager.Instance.EditingExperience.Storage;

                _circleSuctionStimulation.Value = ExperienceManager.Instance.EditingExperience.StimulationSuction;
                _circleSuctionExpression.Value = ExperienceManager.Instance.EditingExperience.ExpressionSuction;
                _circleSpeedStimulation.Value = ExperienceManager.Instance.EditingExperience.StimulationSpeed;
                _circleSpeedExpression.Value = ExperienceManager.Instance.EditingExperience.ExpressionSpeed;
            }

            BtnSave.IsEnabled = !String.IsNullOrEmpty(EntryName.Text);
        }

        void CancelButton_Clicked(object sender, EventArgs e)
        {
            // Cleanup
            ExperienceManager.Instance.EditingExperience = null;
        }

        #region Private

        private void UpdateStorageType(StorageType storageSource)
        {
            if (ExperienceManager.Instance.EditingExperience != null)
            {
                ExperienceManager.Instance.EditingExperience.Storage = storageSource;
            }
        }

        private void BtnBack_Clicked(object sender, EventArgs e)
        {
            ViewModel.Reset();
            PageManager.Me.SetCurrentPage(typeof(SelectSpeedPage), View => (View as SelectSpeedPage).Initialize(_isEditMode));
        }

        private void BtnSave_Clicked(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(EntryName.Text))
            {
                return;
            }

            Titlebar.IsVisible = false;
            RootLayout.Style = (Style)Application.Current.Resources["AbsoluteLayout_FullScreen"];

            ViewModel.ShowMainPage = false;
            ViewModel.ShowSavedPopupPage = true;
            
            if (ExperienceManager.Instance.EditingExperience != null)
            {
                ExperienceManager.Instance.EditingExperience.Name = EntryName.Text.Trim();
                if (EntryDesc.Text != null)
                {
                    ExperienceManager.Instance.EditingExperience.Description = EntryDesc.Text.Trim();
                }
                ExperienceManager.Instance.Save(ExperienceManager.Instance.EditingExperience);
            }

            _timer.Enable = true;
            _timer.Start(() =>
            {
                FinishSession();
                return false;
            });
        }

        private void EntryName_TextChanged(object sender, TextChangedEventArgs e)
        {
            BtnSave.IsEnabled = !String.IsNullOrEmpty(EntryName.Text);
        }

        private void FinishSession()
        {
            _timer.Enable = false;

            PageManager.Me.SetCurrentPage(typeof(DashboardTabPage));
        }

        #endregion
    }

    /// <summary>
    /// This class represets the UI model for this page
    /// </summary>
    class EnterOtherInfoModel : ObservableObject
    {
        private Action<StorageType> ToggleStorageType { get; set; }
        private Action FinishSessionAction { get; set; }

        public EnterOtherInfoModel(Action<StorageType> toggleStorageType, Action finishSessionAction)
        {
            ToggleStorageType = toggleStorageType;
            FinishSessionAction = finishSessionAction;

            Reset();
        }

        public void Reset()
        {
            ShowMainPage = true;
            ShowSavedPopupPage = false;

            ResetMilkTypeControl = true;
        }

        #region Public UI properties

        public bool _resetMilkTypeControl;
        public bool ResetMilkTypeControl
        {
            get => _resetMilkTypeControl;
            set
            {
                SetPropertyChanged(ref _resetMilkTypeControl, value);
                SetPropertyChanged(nameof(ResetMilkTypeControl));
            }
        }

        private StorageType _milkType;
        public StorageType MilkType
        {
            get => _milkType;
            set
            {
                SetPropertyChanged(ref _milkType, value);
            }
        }

        private bool _showMainPage;
        public bool ShowMainPage
        {
            get => _showMainPage;
            set => SetPropertyChanged(ref _showMainPage, value);
        }

        private bool _showSavedPopupPage;
        public bool ShowSavedPopupPage
        {
            get => _showSavedPopupPage;
            set => SetPropertyChanged(ref _showSavedPopupPage, value);
        }

        #endregion


        #region Data properties

        #endregion

        #region Commands

        ICommand _milkTypeCommand;
        public ICommand MilkTypeCommand
        {
            get
            {
                _milkTypeCommand = _milkTypeCommand ?? new Command<StorageType>(ToggleMilkType);
                return _milkTypeCommand;
            }
        }

        ICommand _closeSaveViewCommand;
        public ICommand CloseSaveViewCommand
        {
            get
            {
                // Short circuiting the timer
                _closeSaveViewCommand = _closeSaveViewCommand ?? new Command(() =>
                {
                    FinishSessionAction?.Invoke();
                });

                return _closeSaveViewCommand;
            }
        }

        #endregion


        #region Private

        void ToggleMilkType(StorageType type)
        {
            MilkType = type;

            SetPropertyChanged(nameof(MilkType));

            ToggleStorageType?.Invoke(type);
        }

        #endregion
    }
}
