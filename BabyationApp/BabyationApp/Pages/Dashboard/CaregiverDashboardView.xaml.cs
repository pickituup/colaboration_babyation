using System;
using System.Collections.Generic;

using Xamarin.Forms;
using BabyationApp.Controls.Views;
using BabyationApp.Resources;
using BabyationApp.Common;
using System.Windows.Input;
using BabyationApp.Managers;
using BabyationApp.Pages.BottleSession;
using BabyationApp.Models;

namespace BabyationApp.Pages.Dashboard
{
    public partial class CaregiverDashboardView : RootViewBase
    {
        private CaregiverDashboardViewModel ViewModel;

        public CaregiverDashboardView()
        {
            InitializeComponent();

            Titlebar.Title = AppResource.DashboardUpper;

            ViewModel = new CaregiverDashboardViewModel(SwapAccount, Feed);
            BindingContext = ViewModel;
        }

        public override void AboutToShow()
        {
            base.AboutToShow();

            // Restore style:
            Titlebar.IsVisible = true;
            RootLayout.Style = (Style)Application.Current.Resources["Grid_NavigationOnTop"];
        }

        #region Private

        private void SwapAccount()
        {
            ProfileManager.Instance.CurrentProfile.CaregiverAccountSelected = false;
            PageManager.Me.SetPage(typeof(DashboardTabPage));
        }

        private void Feed()
        {
            ProfileModel profile = ProfileManager.Instance.CurrentProfile;
            if (null == profile)
                return;
                
            if( profile.HasBabies )
            {
                if( 1 == profile.Babies.Count )
                {
                    PageManager.Me.SetCurrentPage(typeof(BottleFeedSelectionPage));
                }
                else
                {
                    PageManager.Me.SetCurrentPage(typeof(SelectChildPage), view =>
                    {
                        (view as SelectChildPage).NextPageType = typeof(BottleFeedSelectionPage);
                    });
                }
            }
            else
            {
                ModalAlertPage.ShowAlertWithClose(AppResource.NoChildError);
            }
        }
        #endregion
    }

    public class CaregiverDashboardViewModel : ObservableObject
    {
        private Action SwapAccountAction;
        private Action FeedAction;

        public CaregiverDashboardViewModel(Action swap, Action feed)
        {
            SwapAccountAction = swap;
            FeedAction = feed;
        }

        #region Commands

        private ICommand _swapAccountCommand;
        public ICommand SwapAccountCommand
        {
            get
            {
                _swapAccountCommand = _swapAccountCommand ?? new Command(() =>
                {
                    SwapAccountAction?.Invoke();
                });
                return _swapAccountCommand;
            }
        }

        private ICommand _feedCommand;
        public ICommand FeedCommand
        {
            get
            {
                _feedCommand = _feedCommand ?? new Command(() =>
                {
                    FeedAction?.Invoke();
                });
                return _feedCommand;
            }
        }
        #endregion
    }
}
