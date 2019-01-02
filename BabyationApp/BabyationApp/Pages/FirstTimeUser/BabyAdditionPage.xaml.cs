
using Xamarin.Forms.Xaml;
using BabyationApp.Managers;
using BabyationApp.ViewModels;
using System.Diagnostics;
using System;
using BabyationApp.Pages.Settings;
using BabyationApp.Resources;

namespace BabyationApp.Pages.FirstTimeUser
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BabyAdditionPage : PageBase
    {
        public BabyAdditionViewModel ViewModel { get; set; }

        public BabyAdditionPage()
        {
            try
            {
                InitializeComponent();

                Titlebar.IsVisible = true;
            }
            catch (System.Exception ex)
            {
                Debugger.Break();
            }
        }

        public void UpdateParams(ProfileManager profileManager, bool skippable = false, bool isProfilePage = false)
        {
            this.ViewModel = new BabyAdditionViewModel(profileManager, skippable, isProfilePage);
            this.BindingContext = this.ViewModel;
        }

        public override void AboutToShow()
        {
            base.AboutToShow();

            Titlebar.IsVisible = true;
            Titlebar.LeftButton.IsVisible = ViewModel.IsProfileSession;
            if( ViewModel.IsProfileSession )
            {
                Titlebar.Title = AppResource.AddYourChild;
                Titlebar.LeftButton.SetDynamicResource(StyleProperty, "BackButton");
                LeftPageType = typeof(ProfilePage);
            }
            else
            {
                Titlebar.Title = AppResource.AddYourChildOpt;
                Titlebar.LeftButton.SetDynamicResource(StyleProperty, "CloseButton");
                LeftPageType = typeof(DashboardTabPage);
            }

            ViewModel.OnAppearing();
        }

        public override void AboutToHide()
        {
            base.AboutToHide();

            ViewModel.OnDisappearing();
        }
    }
}
