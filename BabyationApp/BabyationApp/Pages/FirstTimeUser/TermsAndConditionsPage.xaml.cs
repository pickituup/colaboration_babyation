using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BabyationApp.Managers;
using BabyationApp.Pages.Dashboard;
using Xamarin.Forms;

namespace BabyationApp.Pages.FirstTimeUser
{
    /// <summary>
    /// This class represents the Pump Adding page from the design
    /// </summary>
    public partial class TermsAndConditionsPage : PageBase
    {
        /// <summary>
        /// Constructor -- Initialize the model and binds buttons events and other ui actions
        /// </summary>
        public TermsAndConditionsPage()
        {
            InitializeComponent();

            BtnNext.Clicked += (s, e) =>
            {
                PageManager.Me.SetCurrentPage(typeof(BabyAdditionPage), (Controls.IRootView obj) =>
                {
                    BabyAdditionPage p = (BabyAdditionPage)obj;
                    p.UpdateParams(ProfileManager.Instance, true);
                });
            };

            BtnCancel.Clicked += (s, e) =>
            {
                PageManager.Me.SetCurrentPage(typeof(SignUpPage));
            };

            Titlebar.IsVisible = true;
        }

        public override void AboutToShow()
        {
            base.AboutToShow();
        }

    }
}
