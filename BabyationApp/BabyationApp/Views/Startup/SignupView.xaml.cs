using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace BabyationApp.Views
{
    public partial class SignupView : ContentView
    {
        public event EventHandler SignupClicked;
        public event EventHandler HaveAccountClicked;

        public SignupView()
        {
            InitializeComponent();
            signupBtn.Clicked += SignupBtn_Clicked;
            haveAcctBtn.Clicked += HaveAcctBtn_Clicked;
        }

        private void HaveAcctBtn_Clicked(object sender, EventArgs e)
        {
            EventHandler handler = HaveAccountClicked;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void SignupBtn_Clicked(object sender, EventArgs e)
        {
            SignupClicked?.Invoke(this, e);
        }
    }
}
