using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BabyationApp.Controls.Buttons;
using BabyationApp.Managers;
using BabyationApp.Pages.BottleSession;
using BabyationApp.Pages.FirstTimeUser;
using BabyationApp.Pages.Settings.PumpSettings;
using BabyationApp.Resources;
using Xamarin.Forms;

namespace BabyationApp.Pages.Settings
{
    /// <summary>
    /// This class represents the Extra Settings page from the design
    /// </summary>
    public partial class SettingsPage : PageBase
    {
        /// <summary>
        /// Constructor -- Initialize the model and binds buttons events and other ui actions
        /// </summary> 
        public SettingsPage()
        {
            InitializeComponent();

            Titlebar.IsVisible = true;
            LeftPageType = typeof(MorePage);
            Titlebar.LeftButton.IsVisible = false;
            Titlebar.RightButton.IsVisible = false;

            var items = new List<SettingItemModel>();
            items.Add(new SettingItemModel()
            {
                BackColorNormal = Color.FromHex("#EEF8FD"),
                Text = AppResource.MyProfile,
                Command = new Command(() =>
                {
                    PageManager.Me.SetCurrentPage(typeof(ProfilePage));
                })
            });

            items.Add(new SettingItemModel()
            {
                BackColorNormal = Color.FromHex("#E6F2F8"),
                Text = AppResource.MyPumps,
                Command = new Command(() =>
                {
                    PageManager.Me.SetCurrentPage(typeof(MyPumpsPage));
                })
            });

            items.Add(new SettingItemModel()
            {
                BackColorNormal = Color.FromHex("#EEF8FD"),
                Text = AppResource.AboutBabyation,
                Command = new Command(() =>
                {
                    PageManager.Me.SetCurrentPage(typeof(WelcomePage), view => (view as WelcomePage).Initialize(true));
                })
            });

            BtnSignout.Clicked += (sender, args) =>
            {
                PumpManager.Instance.Reset();
                ProfileManager.Instance.Reset();
                HistoryManager.Instance.Reset();
                ExperienceManager.Instance.Reset();
                LoginManager.Instance.SignOut();
                PageManager.Me.SetCurrentPage(typeof(SignUpPage));
            };

            //items.Add(new SettingItemModel()
            //{
            //    BackColorNormal = Color.FromHex("#EEF8FD"),
            //    Text = "SIGN OUT",
            //    Image = "icon_signout.png",
            //    Command = new Command(() =>
            //    {
            //        LoginManager.Instance.SignOut();
            //        PageManager.Me.SetCurrentPage(typeof(SignUpPage));
            //    })
            //});

            CreateButtons(items);
        }

        private void CreateButtons(List<SettingItemModel> items)
        {
            foreach (SettingItemModel model in items)
            {
                var btn = new SettingsButton();
                btn.BindingContext = model;
                StackButtons.Children.Add(btn);
            }
        }
    }
}
