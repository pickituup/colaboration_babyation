using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BabyationApp.Pages.Dashboard;
using Xamarin.Forms;

namespace BabyationApp.Pages.FirstTimeUser
{
    /// <summary>
    /// This class represents the Pump Adding page from the design
    /// </summary>
    public partial class PumpNamePage : PageBase
    {
        /// <summary>
        /// Constructor -- Initialize the model and binds buttons events and other ui actions
        /// </summary>
        public PumpNamePage()
        {
            InitializeComponent();

            BtnNext.Clicked += (s, e) =>
            {
                PageManager.Me.SetCurrentPage(typeof(DashboardTabPage));
            };

            Titlebar.IsVisible = true;
        }

        public override void AboutToShow()
        {
            base.AboutToShow();
        }

    }
}
