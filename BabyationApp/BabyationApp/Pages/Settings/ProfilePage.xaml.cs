using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BabyationApp.Controls.Buttons;
using BabyationApp.Controls.Views;
using BabyationApp.Converters;
using BabyationApp.Models;
using Xamarin.Forms;
using BabyationApp.Managers;
using BabyationApp.Pages.FirstTimeUser;
using BabyationApp.Pages.ReturningUser;
using System.Windows.Input;
using BabyationApp.Extensions;

namespace BabyationApp.Pages.Settings
{
    /// <summary>
    /// This class represents the Profile page from the design
    /// </summary>
    public partial class ProfilePage : PageBase
    {
        private readonly ButtonExGroup _btnGroupTab = new ButtonExGroup();
        private BabyModel _babyDeleteRequested;

        public DateTime MinimumDate => DateTime.Now.FirstDayOfYear().Subtract(TimeSpan.FromDays(1 + (5 * 365))); // 5 years
        public DateTime MaximumDate => DateTime.Today;

        /// <summary>
        /// Constructor -- Initialize the model and binds buttons events and other ui actions
        /// </summary> 
        public ProfilePage()
        {
            InitializeComponent();

            Titlebar.IsVisible = true;
            LeftPageType = typeof(DashboardTabPage);
            Titlebar.LeftButton.IsVisible = true;
            Titlebar.LeftButton.SetDynamicResource(StyleProperty, "CloseButton");
            Titlebar.RightButton.IsVisible = false;


            _btnGroupTab.AddButton(BtnMyInfo);
            _btnGroupTab.AddButton(BtnMyBabies);
            _btnGroupTab.AddButton(BtnMyPeople);
            BtnMyInfo.IsToggled = true;

            _btnGroupTab.Toggled += (sender, item, index) =>
            {
                RLMyInfo.IsVisible = index == 0;
                RLMyBabies.IsVisible = index == 1;
                RLMyPeople.IsVisible = index == 2;
            };

            EntryName.Completed += (sender, args) =>
            {
                if (ProfileManager.Instance.CurrentProfile != null)
                {
                    if (!String.IsNullOrEmpty(EntryName.Text))
                    {
                        ProfileManager.Instance.CurrentProfile.Name = EntryName.Text;
                    }
                }
            };

            BtnAddAnotherChild.CommandEx = new Command((Object sender) =>
            {
                PageManager.Me.SetCurrentPage(typeof(BabyAdditionPage), (Controls.IRootView obj) =>
                {
                    BabyAdditionPage p = (BabyAdditionPage)obj;
                    p.UpdateParams(ProfileManager.Instance, isProfilePage: true);
                });
            });

            BtnChangePassword.Clicked += (sender, args) =>
            {
                PageManager.Me.SetCurrentPage(typeof(ChangePasswordPage));
            };


            ProfileManager.Instance.CurrentBabyChanged += (sender, args) => UpdateBabyInfo();

            ProfileManager.Instance.BabyModelPropertyChanged += (baby, propertyName) =>
            {
                if (propertyName == "IsDeleteRequested")
                {
                    _babyDeleteRequested = baby;
                }

            };
        }


        private ICommand _pickerFocusCommand;
        public ICommand PickerFocusCommand
        {
            get
            {
                _pickerFocusCommand = _pickerFocusCommand ?? new Command((obj) =>
                {
                    if (!obj.GetType().Equals(typeof(DatePicker)))
                    {
                        return;
                    }

                    ((DatePicker)obj).Focus();
                });
                return _pickerFocusCommand;
            }
        }

        /// <summary>
        /// Updates the baby information based on current profile selected
        /// </summary>
        private void UpdateBabyInfo()
        {
            if (ProfileManager.Instance.CurrentProfile != null)
            {
                this.BindingContext = ProfileManager.Instance.CurrentProfile;

                Titlebar.LeftButton.SetBinding(IsVisibleProperty,
                                               new Binding("ShowBabyDeleteAlert", BindingMode.Default, new BooleanInverseConverter(), null, null, ProfileManager.Instance.CurrentProfile));

                ListBabies.ItemsSource = new ListViewList<BabyModel>(ProfileManager.Instance.CurrentProfile.Babies);
            }
        }

        /// <summary>
        /// Gets called when this page is about to show and performs the initialization
        /// </summary>
        public override void AboutToShow()
        {
            base.AboutToShow();
            UpdateBabyInfo();
            //ListPeople.ItemsSource = profile.Peoples;

            ListBabies.SelectedItem = null;
            _babyDeleteRequested = null;
            //ListPeople.SelectedItem = null;
        }

        private void BtnYesDeleteBaby_OnClicked(object sender, EventArgs e)
        {
            var profile = BindingContext as ProfileModel;
            if (profile != null)
            {
                if (_babyDeleteRequested != null)
                {
                    ProfileManager.Instance.RemoveBaby(_babyDeleteRequested);
                    _babyDeleteRequested = null;
                }

                profile.ShowBabyDeleteAlert = false;
                UpdateBabyInfo();
            }
        }

        private void BtnDontDeleteBaby_OnClicked(object sender, EventArgs e)
        {
            var profile = BindingContext as ProfileModel;
            if (profile != null)
            {
                _babyDeleteRequested.IsDeleteRequested = false;
                _babyDeleteRequested = null;
                profile.ShowBabyDeleteAlert = false;
            }
        }
    }
}
