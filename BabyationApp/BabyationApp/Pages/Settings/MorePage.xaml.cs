using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BabyationApp.Managers;
using BabyationApp.Controls.Views;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Windows.Input;
using BabyationApp.Controls.Buttons;
using BabyationApp.Models;
using BabyationApp.Pages.FirstTimeUser;
using BabyationApp.Pages.BottleSession;
using BabyationApp.Pages.Settings.PumpSettings;
using BabyationApp.Resources;
using Plugin.Connectivity;
using BabyationApp.Pages.StyleGuide;


namespace BabyationApp.Pages.Settings {
    /// <summary>
    /// This class represents the More Tab on the dashboard tab page from design
    /// </summary>
    public partial class MorePage : PageBase {
        private List<SettingItemModel> _modelItems = null;
        /// <summary>
        /// Constructor -- Initialize the model and binds buttons events and other ui actions
        /// </summary>
        public MorePage() {
            InitializeComponent();
            Titlebar.IsVisible = false;
            Titlebar.BackgroundColor = (Color)Application.Current.Resources["MedPink"];
            Titlebar.LeftButton.IsVisible = false;
            Titlebar.RightButton.IsVisible = false;
            HandleBackPressIfBackButtonIsMissing = true;
            LeftPageType = typeof(DashboardTabPage);

            BtnClose.Clicked += (sender, args) => {
                PageManager.Me.SetCurrentPage(typeof(DashboardTabPage));
            };

            var items = new List<SettingItemModel>();
            items.Add(new SettingItemModel() {
                BackColorNormal = (Color)Application.Current.Resources["MedPink50"],
                Text = AppResource.Dash,
                Image = "dashboard_med_blue2.png",
                ImageSelected = "dashboard_navy2.png",
                Command = new Command(() => {
                    PageManager.Me.SetCurrentPage(typeof(DashboardTabPage), view => (view as DashboardTabPage).SetCurrentTab(DashboardTabPage.TabType.Dashboard));
                })
            });

            items.Add(new SettingItemModel() {
                BackColorNormal = (Color)Application.Current.Resources["MedPink50"],
                Text =  AppResource.History,
                Image = "history_med_blue2.png",
                ImageSelected = "history_navy2.png",
                Command = new Command(() => {
                    PageManager.Me.SetCurrentPage(typeof(DashboardTabPage), view => (view as DashboardTabPage).SetCurrentTab(DashboardTabPage.TabType.History));
                })
            });

            items.Add(new SettingItemModel() {
                BackColorNormal = (Color)Application.Current.Resources["MedPink50"],
                Text = AppResource.Modes,
                Image = "modes_med_blue2.png",
                ImageSelected = "modes_navy2.png",
                Command = new Command(() => {
                    PageManager.Me.SetCurrentPage(typeof(DashboardTabPage), view => (view as DashboardTabPage).SetCurrentTab(DashboardTabPage.TabType.Modes));
                })
            });

            items.Add(new SettingItemModel() {
                BackColorNormal = (Color)Application.Current.Resources["MedPink50"],
                Text = AppResource.Reminders,
                Image = "timer_med_blue2.png",
                ImageSelected = "timer_navy2.png",
                Command = new Command(() => {
                    PageManager.Me.SetCurrentPage(typeof(DashboardTabPage), view => (view as DashboardTabPage).SetCurrentTab(DashboardTabPage.TabType.Alarms));
                })
            });

            items.Add(new SettingItemModel() {
                BackColorNormal = (Color)Application.Current.Resources["MedPink50"],
                Text = AppResource.Inventory,
                Image = "inventory_med_blue2.png",
                ImageSelected = "inventory_navy2.png",
                Command = new Command(() => {
                    PageManager.Me.SetCurrentPage(typeof(MyInventoryPage));
                })
            });

            items.Add(new SettingItemModel() {
                BackColorNormal = (Color)Application.Current.Resources["MedPink50"],
                Text = AppResource.Settings,
                Image = "settings_med_blue2.png",
                ImageSelected = "settings_navy2.png",
                Command = new Command(() => {
                    PageManager.Me.SetCurrentPage(typeof(SettingsPage));
                })
            });

            items.Add(new SettingItemModel() {
                IsEnabled = false,
                BackColorNormal = (Color)Application.Current.Resources["MedPink50"],
                Text = AppResource.FAQs,
                Image = "faqs_med_blue2.png",
                ImageSelected = "faqs_navy2.png",
                Command = new Command(() => {
                    PageManager.Me.SetCurrentPage(typeof(SettingsPage));
                })
            });

            items.Add(new SettingItemModel()
            {
                IsEnabled = true,
                BackColorNormal = (Color)Application.Current.Resources["MedPink50"],
                Text = "Style Guide",
                Image = "faqs_med_blue2.png",
                ImageSelected = "faqs_navy2.png",
                Command = new Command(() => {
                    PageManager.Me.SetCurrentPage(typeof(StyleGuidePage));
                })
            });

            _modelItems = items;
            CreateButtons(items);
        }

        private void CreateButtons(List<SettingItemModel> items) {
            foreach (SettingItemModel model in items) {
                var btn = new SettingsButton();
                model.Button = btn;
                btn.BindingContext = model;
                StackButtons.Children.Add(btn);
            }
        }

        /// <summary>
        /// Gets called when this page is about to show and performs the initialization
        /// </summary>
        public override void AboutToShow() {
            try {
                Titlebar.IsVisible = false;
                base.AboutToShow();

                foreach (SettingItemModel model in _modelItems) {
                    if (model.IsSelected) {
                        model.IsSelected = false;
                        model.SyncUI();
                    }
                }

                int type = (int)PageManager.Me.GetDashboardTabPage().GetCurrentTabType();
                if (type < _modelItems.Count) {
                    _modelItems[type].IsSelected = true;
                    _modelItems[type].SyncUI();
                }
            }
            catch (Exception ex) {

                throw;
            }
        }
    }

    /// <summary>
    /// This class represents the UI model for a settings button on the MorePage
    /// </summary>
    class SettingItemModel : ModelItemBase {
        /// <summary>
        /// Constructor
        /// </summary>
        public SettingItemModel() {
            IsEnabled = true;
            TextColor = (Color)App.Instance.Resources["MedBlue"];
            TextColorSelected = (Color)App.Instance.Resources["Navy"];
        }

        /// <summary>
        /// Gets/Sets the settings button tied to this model item
        /// </summary>
        public SettingsButton Button { get; set; }

        /// <summary>
        /// Sync the model text/color to the UI
        /// </summary>
        public void SyncUI() {
            if (Button != null) {
                Button.LablePart.TextColor = IsSelected ? TextColorSelected : TextColor;
                Button.ImagePart.Source = IsSelected ? ImageSelected : Image;
            }
        }

        private bool _isEnabled;
        /// <summary>
        /// Gets/Sests whether the Settings button is enabled or not
        /// </summary>
        public bool IsEnabled {
            get { return _isEnabled; }
            set => SetPropertyChanged(ref _isEnabled, value);
        }

        private Color _backColorNormal;
        /// <summary>
        /// Gets/Sets Background color for the normal state of the settings button
        /// </summary>
        public Color BackColorNormal {
            get { return _backColorNormal; }
            set => SetPropertyChanged(ref _backColorNormal, value);
        }

        private Color _backColorPressed;
        /// <summary>
        /// Gets/Sets Background color for the pressed state of the settings button
        /// </summary>
        public Color BackColorPressed {
            get { return _backColorPressed; }
            set => SetPropertyChanged(ref _backColorPressed, value);
        }

        private Color _textColor;
        /// <summary>
        /// Gets/Sets Text color for the normal state of the settings button
        /// </summary>
        public Color TextColor {
            get { return _textColor; }
            set => SetPropertyChanged(ref _textColor, value);
        }

        private Color _textColorSelected;
        /// <summary>
        /// Gets/Sets Text color for the pressed/selected state of the settings button
        /// </summary>
        public Color TextColorSelected {
            get { return _textColorSelected; }
            set => SetPropertyChanged(ref _textColorSelected, value);
        }

        private string _text;
        /// <summary>
        /// Gets/Sets Text of the settings button
        /// </summary>
        public String Text {
            get { return _text; }
            set => SetPropertyChanged(ref _text, value);
        }

        private ImageSource _image;
        /// <summary>
        /// Gets/Sets normal state Image for the settings button
        /// </summary>
        public ImageSource Image {
            get { return _image; }
            set => SetPropertyChanged(ref _image, value);
        }

        private ImageSource _imageSelected;
        /// <summary>
        /// Gets/Sets pressed state Image for the settings button
        /// </summary>
        public ImageSource ImageSelected {
            get { return _imageSelected; }
            set => SetPropertyChanged(ref _imageSelected, value);
        }

        /// <summary>
        /// The command to execute when the settings button is clicked
        /// </summary>
        public ICommand Command { get; set; }
    }
}
