using System;
using BabyationApp.Common;
using BabyationApp.Helpers;
using Xamarin.Forms;
using BabyationApp.Managers;
using System.Windows.Input;
using BabyationApp.Resources;
using BabyationApp.Pages.Settings;

namespace BabyationApp.Pages.Settings
{
    public partial class AddAuthCodePage : PageBase
    {
        private AddAuthCodeModel ViewModel { get; set; }

        public AddAuthCodePage()
        {
            InitializeComponent();

            Titlebar.IsVisible = true;
            LeftPageType = typeof(ProfilePage);
            Titlebar.LeftButton.IsVisible = true;
            Titlebar.LeftButton.SetDynamicResource(StyleProperty, "CancelButton");

            ViewModel = new AddAuthCodeModel(SaveCodeAsync, FinishSession);
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

        private async void SaveCodeAsync()
        {
            if (ViewModel.IsCodeSending)
                return;

            ViewModel.IsCodeSending = true;
            bool success = true;//await LoginManager.Instance.ForgotPassword(ViewModel.Text);
            ViewModel.IsCodeSending = false;

            if (success)
            {
                ShowSavedOverlay();
            }
            else
            {
                ViewModel.IsCodeValid = false;
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
            PageManager.Me.SetCurrentPage(typeof(ProfilePage));
        }

        #endregion
    }

    class AddAuthCodeModel : ObservableObject
    {
        private readonly DeviceTimer _timer = new DeviceTimer();

        private Action SaveCodeAction { get; set; }
        private Action FinishSessionAction { get; set; }


        public AddAuthCodeModel(Action saveCodeAction, Action finishSessionAction)
        {
            SaveCodeAction = saveCodeAction;
            FinishSessionAction = finishSessionAction;

            Reset();
        }

        public void Reset()
        {
            ShowSendPopupPage = false;
            IsCodeValid = false;
            IsCodeSending = false;
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
                    IsCodeValid = !String.IsNullOrEmpty(value);
                }
            }
        }

        private bool _isCodeValid;
        public bool IsCodeValid
        {
            get => _isCodeValid;
            set => SetPropertyChanged(ref _isCodeValid, value);
        }

        private bool _isSending;
        public bool IsCodeSending
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

        ICommand _sendCodeCommand;
        public ICommand SendCodeCommand
        {
            get
            {
                _sendCodeCommand = _sendCodeCommand ?? new Command(SaveCodeAction);
                return _sendCodeCommand;
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
