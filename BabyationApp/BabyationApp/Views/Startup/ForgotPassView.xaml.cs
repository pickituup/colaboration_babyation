using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace BabyationApp.Views
{
    public partial class ForgotPassView : ContentView
    {
        public event EventHandler RequestPasswordClicked;

        public ForgotPassView()
        {
            InitializeComponent();

            requestBtn.Clicked += RequestBtn_Clicked;
        }

        public void RequestStatus(bool success)
        {
            if (success)
            {
                statusLbl.Text = "Request sent.  Check your email!";
                // Are there view groups?
                emailLbl.IsVisible = false;
                requestBtn.IsVisible = false;
                statusLbl.IsVisible = true;
            }
            else
            {
                statusLbl.Text = "Email not recognized.";
                // Are there view groups?
                emailLbl.IsVisible = true;
                requestBtn.IsVisible = true;
                statusLbl.IsVisible = true;
            }
        }

        private void RequestBtn_Clicked(object sender, EventArgs e)
        {
            EventHandler handler = RequestPasswordClicked;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
