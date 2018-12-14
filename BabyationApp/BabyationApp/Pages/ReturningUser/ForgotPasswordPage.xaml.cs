using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BabyationApp.Common;
using BabyationApp.Helpers;
using BabyationApp.Models;
using BabyationApp.Pages.FirstTimeUser;
using Xamarin.Forms;
using BabyationApp.Managers;
using System.Windows.Input;
using BabyationApp.Resources;

namespace BabyationApp.Pages.ReturningUser
{
    /// <summary>
    /// This class represents the Forgot Password page from the design
    /// </summary>
    public partial class ForgotPasswordPage : PageBase
    {
        private ForgotPasswordModel ViewModel { get; set; }

        public ForgotPasswordPage()
        {
            InitializeComponent();

            Titlebar.IsVisible = true;
            LeftPageType = typeof(SignUpPage);
            Titlebar.LeftButton.IsVisible = true;
            Titlebar.LeftButton.SetDynamicResource(StyleProperty, "CancelButton");

            ViewModel = new ForgotPasswordModel(SendEmailAsync, FinishSession);
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
            bool success = await LoginManager.Instance.ForgotPassword(ViewModel.Text);
            ViewModel.IsEmailSending = false;

            if (success)
            {
                ShowSavedOverlay();
            }
            else
            {
                ViewModel.IsEmailValid = false;
                ModalAlertPage.ShowAlertWithClose(AppResource.InvalidEntry);
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
            PageManager.Me.SetCurrentPage(typeof(SignUpPage));
        }

        #endregion
    }

    class ForgotPasswordModel : ObservableObject
    {
        private readonly DeviceTimer _timer = new DeviceTimer();

        private Action SendEmailAction { get; set; }
        private Action FinishSessionAction { get; set; }


        public ForgotPasswordModel(Action sendEmailAction, Action finishSessionAction)
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
                if( SetPropertyChanged(ref _text, value))
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

        ICommand _sendEmailCommand;
        public ICommand SendEmailCommand
        {
            get
            {
                _sendEmailCommand = _sendEmailCommand ?? new Command(SendEmailAction);
                return _sendEmailCommand;
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

        ICommand _closeSaveViewCommand;
        public ICommand CloseSaveViewCommand
        {
            get
            {
                // Short circuiting the timer
                _closeSaveViewCommand = _closeSaveViewCommand ?? new Command(() =>
                {
                    _timer.Enable = false;
                    FinishSessionAction?.Invoke();
                });

                return _closeSaveViewCommand;
            }
        }

        #endregion
    }
}
