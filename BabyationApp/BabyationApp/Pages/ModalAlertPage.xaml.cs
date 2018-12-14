using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Interfaces.Animations;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace BabyationApp.Pages
{
    public partial class ModalAlertPage : PopupPage
    {
        public ModalAlertPage()
        {
            InitializeComponent();
            BtnInfo1.Clicked += (sender, args) => PopupNavigation.Instance.PopAsync();
            BtnClose1.Clicked += (sender, args) => PopupNavigation.Instance.PopAsync();
        }

        public static ModalAlertPage ShowAlertWithClose(String msg)
        {
            ModalAlertPage popup = new ModalAlertPage();
            popup.LblMsg.Text = msg;
            popup.BtnInfo1.IsVisible = false;
            popup.BtnClose1.IsVisible = true;
            PopupNavigation.Instance.PushAsync(popup);
            return popup;
        }

        public static ModalAlertPage ShowAlertWithInfoBtn(String msg, String btnText, Action actBtnClicked)
        {
            ModalAlertPage popup = new ModalAlertPage();
            popup.LblMsg.Text = msg;
            popup.BtnInfo1.Text = btnText;
            popup.BtnClose1.IsVisible = false;
            popup.BtnInfo1.IsVisible = true;
            popup.BtnInfo1.Clicked += (sender, args) => actBtnClicked?.Invoke();
            PopupNavigation.Instance.PushAsync(popup);
            return popup;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        // Method for animation child in PopupPage
        // Invoced after custom animation end
        protected override async void OnAppearingAnimationEnd()
        {
            await Content.FadeTo(1);
        }

        // Method for animation child in PopupPage
        // Invoked before custom animation begin
        protected override async void OnDisappearingAnimationBegin()
        {
            await Content.FadeTo(1);
        }

        protected override bool OnBackButtonPressed()
        {
            // Prevent hide popup
            //return base.OnBackButtonPressed();
            return true;
        }

        // Invoced when background is clicked
        protected override bool OnBackgroundClicked()
        {
            // Return default value - CloseWhenBackgroundIsClicked
            return base.OnBackgroundClicked();
        }
    }

    class UserAnimation : IPopupAnimation
    {
        // Call Before OnAppering
        public void Preparing(View content, PopupPage page)
        {
            // Preparing content and page
            content.Opacity = 0;
        }

        // Call After OnDisappering
        public void Disposing(View content, PopupPage page)
        {
            // Dispose Unmanaged Code
        }

        // Call After OnAppering
        public async Task Appearing(View content, PopupPage page)
        {
            // Show animation
            await content.FadeTo(1);
        }

        // Call Before OnDisappering
        public async Task Disappearing(View content, PopupPage page)
        {
            // Hide animation
            await content.FadeTo(0);
        }
    }
}
