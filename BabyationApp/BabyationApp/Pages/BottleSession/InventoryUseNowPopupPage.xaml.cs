using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BabyationApp.Managers;
using BabyationApp.Models;
using BabyationApp.Resources;
using Xamarin.Forms;

namespace BabyationApp.Pages.BottleSession
{
    /// <summary>
    /// This class represents Inventory Use Now page from the design
    /// </summary>
    public partial class InventoryUseNowPopupPage : PageBase
    {
        /// <summary>
        /// Constructor -- Initialize the model and binds buttons events and other ui actions
        /// </summary>
        public InventoryUseNowPopupPage()
        {
            InitializeComponent();

            Titlebar.IsVisible = true;
            Title = AppResource.MyInventory;

            BtnNoKeepIt.Clicked += (s, e) => { PageManager.Me.SetCurrentPage(SourcePageType); };

            BtnYesUseNow.Clicked += (s, e) =>
            {
                if (UseNowHistoryModelItem != null)
                {
                    HistoryManager.Instance.RemoveInventory(UseNowHistoryModelItem);

                    SessionManager.Instance.StartBottleFeeding();

                    if (SessionManager.Instance.CurrentSession != null)
                    {
                        ProfileModel profile = ProfileManager.Instance.CurrentProfile;
                        SessionManager.Instance.CurrentSession.FeedProfileId = (profile.CaregiverAccountSelected ? null : profile.ProfileId);

                        SessionManager.Instance.CurrentSession.Milk = UseNowHistoryModelItem.Milk;
                        SessionManager.Instance.CurrentSession.Storage = UseNowHistoryModelItem.Storage;

                        PageManager.Me.SetCurrentPage(typeof(BottleFeedStartPage), view =>
                        {
                            //
                        });
                    }
                }
            };
        }

        /// <summary>
        /// Gets/Sets the source page type from which we landed on this page, so that when done, we can show that page
        /// </summary>
        public Type SourcePageType { get; set; }

        /// <summary>
        /// Gets/Sets the history model to use for the page
        /// </summary>
        public HistoryModel UseNowHistoryModelItem { get; set; }
    }
}
