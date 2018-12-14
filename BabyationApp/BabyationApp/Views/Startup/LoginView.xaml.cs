using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace BabyationApp.Views
{
    public partial class LoginView : ContentView
    {
        public event EventHandler LoginClicked;
        public event EventHandler NeedAccountClicked;
        public event EventHandler ForgotPasswordClicked;

        public LoginView()
        {
            InitializeComponent();

            loginBtn.Clicked += LoginBtn_Clicked;
            noAcctBtn.Clicked += NoAcctBtn_Clicked;
            forgotPassBtn.Clicked += ForgotPassBtn_Clicked;
        }

        private void ForgotPassBtn_Clicked(object sender, EventArgs e)
        {
            EventHandler handler = ForgotPasswordClicked;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void NoAcctBtn_Clicked(object sender, EventArgs e)
        {
            EventHandler handler = NeedAccountClicked;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void LoginBtn_Clicked(object sender, EventArgs e)
        {
            EventHandler handler = LoginClicked;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
