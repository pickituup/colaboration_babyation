using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BabyationApp.Pages.Dashboard;
using BabyationApp.Resources;
using Xamarin.Forms;

namespace BabyationApp.Pages.FirstTimeUser
{
    /// <summary>
    /// This class represents the Pump Adding page from the design
    /// </summary>
    public partial class PumpAdditionPage : PageBase
    {
        public bool IsFirstTimeFlow { get; set; } = true;

        /// <summary>
        /// Constructor -- Initialize the model and binds buttons events and other ui actions
        /// </summary>
        public PumpAdditionPage()
        {
            try {
                InitializeComponent();

                BtnPair.Clicked += (s, e) => {
                    PageManager.Me.SetCurrentPage(typeof(PumpNamePage));
                };

                Titlebar.IsVisible = true;

            } catch (Exception ex) {
                Debugger.Break();
                throw;
            }
        }

        public override void AboutToShow()
        {
            base.AboutToShow();

            Titlebar.Title = AppResource.PairYouPump;
            Titlebar.LeftButton.IsVisible = true;
            Titlebar.LeftButton.SetDynamicResource(StyleProperty, "CancelButton");
            LeftPageType = LeftPageType ?? typeof(DashboardTabPage);
        }
    }
}
