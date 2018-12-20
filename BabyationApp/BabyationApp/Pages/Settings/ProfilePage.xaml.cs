using System;
using System.Collections.Generic;

using Xamarin.Forms;
using BabyationApp.Resources;

namespace BabyationApp.Pages.Settings
{
    public partial class ProfilePage : PageBase
    {
        public ProfilePage()
        {
            InitializeComponent();

            Titlebar.IsVisible = true;
            Titlebar.Title = AppResource.MyProfile;
            LeftPageType = typeof(DashboardTabPage);
            Titlebar.LeftButton.IsVisible = true;
            Titlebar.LeftButton.SetDynamicResource(StyleProperty, "CloseButton");
            Titlebar.RightButton.IsVisible = false;
            RootLayout.Style = (Style)Application.Current.Resources["StackLayout_NavigationOnTop"];

            ProfileView.PropertyChanged += ProfileView_PropertyChanged;
        }

        public override void AboutToShow()
        {
            base.AboutToShow();

            // Restore styles
            Titlebar.IsVisible = true;
            RootLayout.Style = (Style)Application.Current.Resources["StackLayout_NavigationOnTop"];

            ProfileView.AboutToShow();
        }

        void ProfileView_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if( e.PropertyName == "LeftPageType")
            {
                LeftPageType = ((ProfileView)sender).LeftPageType;
            }
        }
    }
}
