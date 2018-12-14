using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BabyationApp.Managers;
using Plugin.Connectivity;

using Xamarin.Forms;

namespace BabyationApp.Pages.FirstTimeUser
{
    public partial class StartupPage : ContentPage
    {
        BabyationApp.Views.SignupView _signupView;
        BabyationApp.Views.LoginView _loginView;
        BabyationApp.Views.ForgotPassView _passwordView;
        BabyationApp.Views.BabyInfoView _babyView;
        BabyationApp.Views.ProductSetupView _setupView;

        // Temporary
        public event EventHandler SkipClicked;

        public StartupPage()
        {
            InitializeComponent();

            _signupView = new Views.SignupView();
            _loginView = new Views.LoginView();
            _passwordView = new Views.ForgotPassView();
            _babyView = new Views.BabyInfoView();
            _setupView = new Views.ProductSetupView();

            //TODO: Appropriate view selection logic/auth flow
            //Content = _setupView;

            _signupView.SignupClicked += _signupView_Clicked;
            _signupView.HaveAccountClicked += _signupView_HaveAccountClicked;
            _loginView.LoginClicked += _loginView_LoginClicked;
            _loginView.NeedAccountClicked += _loginView_NeedAccountClicked;
            _loginView.ForgotPasswordClicked += _loginView_ForgotPasswordClicked;
            _passwordView.RequestPasswordClicked += _passwordView_RequestPasswordClicked;
            _babyView.Clicked += _babyView_Clicked;
            _setupView.SkipClicked += _setupView_SkipClicked;
        }

        private void _setupView_SkipClicked(object sender, EventArgs e)
        {
            EventHandler handler = SkipClicked;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void _passwordView_RequestPasswordClicked(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            // TODO: call services
            _passwordView.RequestStatus(true);
        }

        private void _loginView_ForgotPasswordClicked(object sender, EventArgs e)
        {
            Content = _passwordView;
        }

        private void _loginView_NeedAccountClicked(object sender, EventArgs e)
        {
            Content = _signupView;
        }

        private void _loginView_LoginClicked(object sender, EventArgs e)
        {
            Content = _loginView;
        }

        private void _signupView_HaveAccountClicked(object sender, EventArgs e)
        {
            Content = _loginView;
        }

        private void _babyView_Clicked(object sender, EventArgs e)
        {
            Content = _setupView;
        }

        private void _signupView_Clicked(object sender, EventArgs e)
        {
            Content = _babyView;
        }

        protected override void OnAppearing()
        {
        }
    }
}
