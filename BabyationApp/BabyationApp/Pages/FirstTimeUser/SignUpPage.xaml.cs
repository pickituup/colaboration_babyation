using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BabyationApp.Common;
using Xamarin.Forms;

using BabyationApp.Managers;
using BabyationApp.Pages.ReturningUser;
using BabyationApp.Helpers;
using BabyationApp.Controls.Views;
using BabyationApp.Controls.TextEditors;
using BabyationApp.Resources;
using System.Diagnostics;

namespace BabyationApp.Pages.FirstTimeUser
{
    /// <summary>
    /// This class represents the SingUp options page from the design
    /// </summary>
    /// <remarks>It lets user to choose from one of the signup of options (email/pass, google, facebook)</remarks>
    public partial class SignUpPage : PageBase
    {
        enum ValidationTarget { Email, Password };

        /// <summary>
        /// Constructor -- Initialize the model and binds buttons events and other ui actions
        /// </summary>
        public SignUpPage()
        {
            try {

            InitializeComponent();
            } catch (Exception ex) {
                Debugger.Break(); 
                throw;
            }
        }

        void Handle_EmailText_Changed(object sender, TextChangedEventArgs e) => Validate(ValidationTarget.Email);

        void Handle_PasswordText_Changed(object sender, TextChangedEventArgs e) => Validate(ValidationTarget.Password);

        void Validate(ValidationTarget target)
        {
            var isEmailValid = InputValidator.IsValidEmail(EntryEmail.Text);
            var isPasswordValid = InputValidator.IsValidPassword(EntryPassword.Text);

            if (target == ValidationTarget.Email)
            {
                IconEmailChecked.IsVisible = isEmailValid;
            }
            else
            {
                IconPasswordChecked.IsVisible = isPasswordValid;
            }

            _btnSignInLocal.IsEnabled = isEmailValid && isPasswordValid;
            _gridSignInOverlay.IsVisible = !isEmailValid || !isPasswordValid;
        }

        void Handle_CreateAccount_Tapped(object sender, EventArgs e) => PageManager.Me.SetCurrentPage(typeof(SignUpViaEmailPage));

        void Handle_ForgotPassword_Tapped(object sender, EventArgs e) => PageManager.Me.SetCurrentPage(typeof(ForgotPasswordPage));

        async void Handle_SignIn_Tapped(object sender, EventArgs e)
        {
            if (IconEmailChecked.IsVisible && IconPasswordChecked.IsVisible)
            {
                BusyIndicator.IsRunning = true;

                bool success = await OnSignIn();

                if (success)
                {
                    DataManager.Instance.SetNewUser(LoginManager.Instance.UserId);

                    if (IsFirstTime == "yes")
                    {
                        PageManager.Me.SetCurrentPage(typeof(TermsAndConditionsPage));
                    }
                    else
                    {
                        PageManager.Me.SetCurrentPage(typeof(DashboardTabPage));
                    }
                }

                BusyIndicator.IsRunning &= success;
            }
        }

        /// <summary>
        /// Performs the actual signin to the cloud
        /// </summary>
        /// <returns></returns>
        async Task<bool> OnSignIn()
        {
            bool signedIn = false;

            try
            {
                if (!await LoginManager.Instance.Authenticator.Authenticate(Provider.Custom, EntryEmail.Text, EntryPassword.Text))
                {
                    EntryPassword.Text = string.Empty;
                    IconPasswordChecked.IsVisible = false;
                    ModalAlertPage.ShowAlertWithClose(AppResource.AuthenticationFailure);
                }
                else
                {
                    await LoginManager.Instance.GetUserInfo();
                    signedIn = true;
                }
            }
            catch (Exception ex)
            {
                AnalyticsManager.Instance.TrackError(ex);
            }

            return signedIn;
        }

        async void Handle_SignIntoGoogle_Tapped(object sender, EventArgs e)
        {
            BusyIndicator.IsRunning = true;

            bool auth = false;

            try
            {
                auth = await LoginManager.Instance.Authenticator.Authenticate(Provider.Google);
                if (auth)
                {
                    await LoginManager.Instance.GetUserInfo();

                    DataManager.Instance.SetNewUser(LoginManager.Instance.UserId);

                    if (IsFirstTime == "yes")
                    {
                        PageManager.Me.SetCurrentPage(typeof(TermsAndConditionsPage));
                    }
                    else
                    {
                        PageManager.Me.SetCurrentPage(typeof(DashboardTabPage));
                    }
                }
            }
            catch (Exception ex)
            {
                AnalyticsManager.Instance.TrackError(ex);
            }

            BusyIndicator.IsRunning &= auth;
        }

        async void Handle_SignIntoFacebook_Tapped(object sender, EventArgs e)
        {
            BusyIndicator.IsRunning = true;

            bool auth = false;

            try
            {
                auth = await LoginManager.Instance.Authenticator.Authenticate(Provider.Facebook);
                if (auth)
                {
                    await LoginManager.Instance.GetUserInfo();

                    DataManager.Instance.SetNewUser(LoginManager.Instance.UserId);

                    if (IsFirstTime == "yes")
                    {
                        PageManager.Me.SetCurrentPage(typeof(TermsAndConditionsPage));
                    }
                    else
                    {
                        PageManager.Me.SetCurrentPage(typeof(DashboardTabPage));
                    }
                }
            }
            catch (Exception ex)
            {
                AnalyticsManager.Instance.TrackError(ex);
            }

            BusyIndicator.IsRunning &= auth;
        }

        /// <summary>
        /// Gets/Sets whether this is the very first time login
        /// </summary>
        protected string IsFirstTime
        {
            get
            {
                return BabyationApp.Helpers.Settings.IsFirstTimeUser;
            }
            set
            {
                if (BabyationApp.Helpers.Settings.IsFirstTimeUser != value)
                {
                    BabyationApp.Helpers.Settings.IsFirstTimeUser = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets called when this page is about to show and performs the initialization
        /// </summary>
        public override void AboutToShow()
        {
            BusyIndicator.IsRunning = false;

            base.AboutToShow();

            IconPasswordChecked.IsVisible = false;
            EntryPassword.Text = string.Empty;
        }
    }
}
