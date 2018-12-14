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

                BtnNext.Clicked += (s, e) => {
                    PageManager.Me.SetCurrentPage(typeof(DashboardTabPage));
                };

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
            Title = IsFirstTimeFlow ? AppResource.PairYouPumpOptional : AppResource.PairYouPump;
            BtnNext.Text = IsFirstTimeFlow ? AppResource.Skip : AppResource.Back;
            BtnNext.ImageNormal = IsFirstTimeFlow ? "icon_next2.png" : "icon_next2.png";
            BtnNext.ImagePressed = IsFirstTimeFlow ? "icon_next2.png" : "icon_next2.png";

        }

    }
}
