using BabyationApp.Controls.ListedSelector;
using BabyationApp.Controls.Views;
using BabyationApp.Managers;
using BabyationApp.Models;
using System;
using Xamarin.Forms.Internals;

namespace BabyationApp.Pages.BottleSession
{
    /// <summary>
    /// This class represents Bottle Feed Selection page from the design
    /// </summary>
    public partial class SelectChildPage : PageBase
    {
        private BabyModel _selectedbabyModel;

        /// <summary>
        /// Constructor -- Initialize the model and binds buttons events and other ui actions
        /// </summary>
        public SelectChildPage()
        {
            InitializeComponent();

            Titlebar.IsVisible = true;
            Titlebar.LeftButton.IsVisible = true;
            Titlebar.LeftButton.SetDynamicResource(StyleProperty, "CancelButton");

            BtnNext.Clicked += (s, e) =>
            {
                ProfileManager.Instance.CurrentProfile.CurrentBaby = _selectedbabyModel;
                PageManager.Me.SetCurrentPage(NextPageType);
            };
        }

        public Type NextPageType { get; set; }
        /// <summary>
        /// Gets called when this page is about to show and performs the initialization
        /// </summary>
        public override void AboutToShow()
        {
            base.AboutToShow();

            LeftPageType = CurrentDashboard();

            if (ProfileManager.Instance.CurrentProfile != null)
            {
                ProfileManager.Instance.CurrentProfile.CurrentBaby = null;
                BtnNext.IsEnabled = false;
                _selectedbabyModel = null;
                _childFeedingList_ListedSelector.ItemsSource = ProfileManager.Instance.CurrentProfile.Babies;
                _childFeedingList_ListedSelector.SelectedItem = null;
                ProfileManager.Instance.CurrentProfile.Babies.ForEach(baby => baby.IsSelected = false);
            }
        }

        private void ChildFeedingList_ListedSelector_ItemSelected(object sender, EventArgs e)
        {
            if (sender is ChildListedItem childFeedingListedItem)
            {
                if (childFeedingListedItem.BindingContext is BabyModel babyModel)
                {
                    _selectedbabyModel = babyModel;

                    BtnNext.IsEnabled = true;
                }
            }
        }
    }
}
