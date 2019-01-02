using System;
using System.Collections.Generic;

using Xamarin.Forms;
using BabyationApp.Resources;
using BabyationApp.Managers;

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
            Titlebar.LeftButton.SetDynamicResource(StyleProperty, "BackButton");
            Titlebar.RightButton.IsVisible = false;
            RootLayout.Style = (Style)Application.Current.Resources["StackLayout_NavigationOnTop"];

            ProfileView.PropertyChanged += ProfileView_PropertyChanged;
        }

        public override async void AboutToShow()
        {
            base.AboutToShow();

            // Restore styles
            Titlebar.IsVisible = true;
            RootLayout.Style = (Style)Application.Current.Resources["StackLayout_NavigationOnTop"];

            ProfileView.AboutToShow();

            //var result = await ProfileManager.Instance.AddCaregiver("test@test.com");

            //var result = await ProfileManager.Instance.VerifyCaregiverCode("abc");

            //string test = "";
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
