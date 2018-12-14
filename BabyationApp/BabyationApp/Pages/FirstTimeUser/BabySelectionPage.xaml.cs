using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BabyationApp.Managers;
using BabyationApp.Models;
using Xamarin.Forms;

namespace BabyationApp.Pages.FirstTimeUser
{
    /// <summary>
    /// This class represents the Baby Selection page from the design
    /// </summary>
    public partial class BabySelectionPage : PageBase
    {
        /// <summary>
        /// Constructor -- Initialize the model and binds buttons events and other ui actions
        /// </summary>
        public BabySelectionPage()
        {
            InitializeComponent();

            Titlebar.IsVisible = false;
            
            BtnClose.Clicked += (sender, args) =>
            {
                PageManager.Me.SetCurrentPage(typeof(DashboardTabPage));
            };
        }

        /// <summary>
        /// Gets called when this page is about to show and performs the initialization
        /// </summary>
        public override void AboutToShow()
        {
            base.AboutToShow();

            if (ProfileManager.Instance.CurrentProfile != null)
            {
                listView.ItemsSource = ProfileManager.Instance.CurrentProfile.Babies;
            }
        }
    }
}
