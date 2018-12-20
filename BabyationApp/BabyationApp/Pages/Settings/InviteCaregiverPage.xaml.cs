using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BabyationApp.Common;
using BabyationApp.Helpers;
using BabyationApp.Pages.FirstTimeUser;
using Xamarin.Forms;
using BabyationApp.Managers;
using System.Windows.Input;
using BabyationApp.Resources;
using BabyationApp.Models;

namespace BabyationApp.Pages.Settings
{
    /// <summary>
    /// This class represents the Forgot Password page from the design
    /// </summary>
    public partial class InviteCaregiverPage : PageBase
    {
        private InviteCaregiverModel ViewModel { get; set; }

        public InviteCaregiverPage()
        {
            InitializeComponent();

            Titlebar.IsVisible = true;
            LeftPageType = typeof(ProfilePage);
            Titlebar.LeftButton.IsVisible = true;
            Titlebar.LeftButton.SetDynamicResource(StyleProperty, "CancelButton");

            ViewModel = new InviteCaregiverModel(SendEmailAsync, FinishSession);
            BindingContext = ViewModel;
        }

        public override void AboutToShow()
        {
            base.AboutToShow();

            ViewModel.Reset();

            // Restore style:
            Titlebar.IsVisible = true;
            RootLayout.Style = (Style)Application.Current.Resources["AbsoluteLayout_NavigationOnTop"];
        }

        #region Private

        private async void SendEmailAsync()
        {
            if (ViewModel.IsEmailSending)
                return;

            ViewModel.IsEmailSending = true;
            //bool success = //await LoginManager.Instance.ForgotPassword(ViewModel.Text);

            PeopleModel caregiver = new PeopleModel(null) { Email = ViewModel.Text };
            bool success = ProfileManager.Instance.CurrentProfile.AddCaregiver(caregiver);

            ViewModel.IsEmailSending = false;

            if (success)
            {
                ShowSavedOverlay();
            }
            else
            {
                ViewModel.IsEmailValid = false;
                ModalAlertPage.ShowAlertWithClose(ProfileManager.Instance.CurrentProfile.ErrorMessage ?? AppResource.InvalidEntry);
            }
        }

        private void ShowSavedOverlay()
        {
            Titlebar.IsVisible = false;
            RootLayout.Style = (Style)Application.Current.Resources["AbsoluteLayout_FullScreen"];
            ViewModel.ShowSavePageCommand?.Execute(this);
        }

        private void FinishSession()
        {
            PageManager.Me.SetCurrentPage(LeftPageType);
        }

        #endregion
    }

    class InviteCaregiverModel : ObservableObject
    {
        private readonly DeviceTimer _timer = new DeviceTimer();

        private Action SendEmailAction { get; set; }
        private Action FinishSessionAction { get; set; }


        public InviteCaregiverModel(Action sendEmailAction, Action finishSessionAction)
        {
            SendEmailAction = sendEmailAction;
            FinishSessionAction = finishSessionAction;

            Reset();
        }

        public void Reset()
        {
            ShowSendPopupPage = false;
            IsEmailValid = false;
            IsEmailSending = false;
            Text = null;
        }

        #region Public UI properties

        private string _text;
        public string Text
        {
            get => _text;
            set
            {
                if (SetPropertyChanged(ref _text, value))
                {
                    IsEmailValid = !String.IsNullOrEmpty(value) && InputValidator.IsValidEmail(value);
                }
            }
        }

        private bool _isEmailValid;
        public bool IsEmailValid
        {
            get => _isEmailValid;
            set => SetPropertyChanged(ref _isEmailValid, value);
        }

        private bool _isSending;
        public bool IsEmailSending
        {
            get => _isSending;
            set => SetPropertyChanged(ref _isSending, value);
        }

        private bool _showSendPopup;
        public bool ShowSendPopupPage
        {
            get => _showSendPopup;
            set => SetPropertyChanged(ref _showSendPopup, value);
        }

        #endregion

        #region Commands

        ICommand _addCaregiverCommand;
        public ICommand AddCaregiverCommand
        {
            get
            {
                _addCaregiverCommand = _addCaregiverCommand ?? new Command(SendEmailAction);
                return _addCaregiverCommand;
            }
        }

        ICommand _showSavePageCommand;
        public ICommand ShowSavePageCommand
        {
            get
            {
                _showSavePageCommand = _showSavePageCommand ?? new Command(() =>
                {
                    ShowSendPopupPage = true;

                    _timer.Enable = true;
                    _timer.Start(() =>
                    {
                        FinishSessionAction?.Invoke();  // Ask codebehind to switch to next page
                        return false;
                    });
                });
                return _showSavePageCommand;
            }
        }

        ICommand _closeViewCommand;
        public ICommand CloseViewCommand
        {
            get
            {
                // Short circuiting the timer
                _closeViewCommand = _closeViewCommand ?? new Command(() =>
                {
                    _timer.Enable = false;
                    FinishSessionAction?.Invoke();
                });

                return _closeViewCommand;
            }
        }

        #endregion
    }
}
