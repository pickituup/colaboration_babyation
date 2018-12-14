using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Linq.Expressions;
using BabyationApp.Controls.Views;
using BabyationApp.Helpers;
using BabyationApp.Managers;
using BabyationApp.Models;
using BabyationApp.Pages.BottleSession;
using BabyationApp.Pages.FirstTimeUser;
using BabyationApp.Pages.PumpSession;
using BabyationApp.Pages.Modes;
using BabyationApp.Pages.NurseSession;
using BabyationApp.Pages.Settings.PumpSettings;
using BabyationApp.Resources;

namespace BabyationApp.Pages.Dashboard
{
    /// <summary>
    /// This class represents the Dashboard Details page from the design
    /// </summary>
    public partial class DashboardDetailView : RootViewBase
    {
        /// <summary>
        /// Constructor -- Initialize the model and binds buttons events and other ui actions
        /// </summary>
        public DashboardDetailView()
        {
            InitializeComponent();
            Title = "";
            Titlebar.IsVisible = true;
            
            _rl1.SizeChanged += ONRL1_SizeChanged;

            listView.ItemsSource = new ListViewList<ExperienceModel>(ExperienceManager.Instance.AllExperiences);

            listView.ItemSelected += (s, e) =>
            {
                if (e.SelectedItem != null)
                {
                    var selectedItem = (ExperienceModel)e.SelectedItem;
                    if (selectedItem != null)
                    {
                        ExperienceManager.Instance.CurrentExperience = selectedItem;
                    }
                }
            };

            BtnPumpSession.Clicked += (s, e) =>
            {

//#if MOCKEDPUMPING
                PumpManager.Instance.ConnectedPump = new PumpModel();
//#endif

                if (PumpManager.Instance.ConnectedPump == null)
                {
                    ModalAlertPage.ShowAlertWithClose(
                        "We couldn’t find your pump! Please make sure you are in range and that your pump is charged or plugged in.");
                    return;
                }

                PageManager.Me.SetCurrentPage(typeof(PumpSessionPage));
            };

            BtnFeedSession.Clicked += (s, e) =>
            {
                if (null == ProfileManager.Instance.CurrentProfile)
                    return;

                if (ProfileManager.Instance.CurrentProfile.Babies != null &&
                    ProfileManager.Instance.CurrentProfile.Babies.Count == 1)
                {
                    PageManager.Me.SetCurrentPage(typeof(BottleFeedSelectionPage));
                }
                else
                {
                    if (ProfileManager.Instance.CurrentProfile.Babies != null)
                    {
                        PageManager.Me.SetCurrentPage(typeof(SelectChildPage),
                            view => { (view as SelectChildPage).NextPageType = typeof(BottleFeedSelectionPage); });
                    }
                    else
                    {
                        ModalAlertPage.ShowAlertWithClose(AppResource.NoChildError);
                    }
                }
            };

            BtnNurseSession.Clicked += (s, e) =>
            {
                if (null == ProfileManager.Instance.CurrentProfile)
                    return;

                if (ProfileManager.Instance.CurrentProfile.Babies != null &&
                    ProfileManager.Instance.CurrentProfile.Babies.Count == 1)
                {
                    PageManager.Me.SetCurrentPage(typeof(NurseSessionSelectionPage));
                }
                else
                {
                    if (ProfileManager.Instance.CurrentProfile.Babies != null)
                    {
                        PageManager.Me.SetCurrentPage(typeof(SelectChildPage),
                            view => { (view as SelectChildPage).NextPageType = typeof(NurseSessionSelectionPage); });
                    }
                    else
                    {
                        ModalAlertPage.ShowAlertWithClose(AppResource.NoChildError);
                    }
                }
            };

            
            PumpConnect.Clicked += (s, e) =>
            {
                PageManager.Me.SetCurrentPage(typeof(PumpAdditionPage), view =>
                    {
                        (view as PumpAdditionPage).IsFirstTimeFlow = false;
                    });
            };

            PumpManager.Instance.NewPumps.CollectionChanged += (sender, args) =>
            {
                try
                {
                    Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                    {
                        RefreshTitlebar();
                    });
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exeption: " + e.Message);
                }
            };

            PumpManager.Instance.PumpConnectedEvent += (sender, args) =>
            {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    RefreshTitlebar();
                });
            };

            PumpManager.Instance.PumpDisconnectedEvent += (sender, args) =>
            {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    RefreshTitlebar();
                });
            };
        }

        /// <summary>
        /// Refresh the titlebar information
        /// </summary>
        private void RefreshTitlebar()
        {
            try
            {
                Title = AppResource.DashboardUpper;
                
                if (PumpManager.Instance.ConnectedPump != null)
                {
                    var pump = PumpManager.Instance.ConnectedPump;
                    pump.PropertyChanged -= Pump_PropertyChanged;
                    pump.PropertyChanged += Pump_PropertyChanged;
                    StackPumpInfo.IsVisible = true;
                    StackPumpConnect.IsVisible = false;
                    PumpName.Text = pump.Name;
                    PumpCharge.Text = pump.BatteryLevelText;
                }
                else
                {
                    StackPumpInfo.IsVisible = false;
                    StackPumpConnect.IsVisible = true;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exeption: " + e.Message);
            }          
        }

        /// <summary>
        /// Updates the baby information based on current profile
        /// </summary>
        private void Pump_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            PumpModel pump;

            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                if (PumpManager.Instance.ConnectedPump != null)
                {
                    pump = PumpManager.Instance.ConnectedPump;
                    switch (e.PropertyName)
                    {
                        case "BatteryLevelText":
                            Titlebar.LeftButton.Text = pump.BatteryLevelText;
                            break;
                        default:
                            break;
                    }
                }
            });
        }

        /// <summary>
        /// Gets called when this page is about to show and performs the initialization
        /// </summary>
        public override void AboutToShow()
        {
            base.AboutToShow();
            LblGreetings.Text = AppResource.HiMama;
            listView.SelectedItem = null;
            RefreshTitlebar();
        }

        /// <summary>
        /// Update the UI controls position/size when the page layout/size changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ONRL1_SizeChanged(object sender, EventArgs e)
        {
            var W = _rl1.Width * 2.93 / 10;
            var H = W;

            if (W <= 0) return;

            var x1 = (_rl1.Width - W) / 2;
            var y1 = 16;
            var c1 = BoundsConstraint.FromExpression((Expression<Func<Rectangle>>)(() => new Rectangle(x1, y1, W, H)));
            RelativeLayout.SetBoundsConstraint(BtnPumpSession, c1);

            var w = _rl1.Width * 2.21 / 10;
            var h = w;
            
            var x2 = x1 -  w - 14;
            var y2 = y1 + H / 2 + 12;
            var c2 = BoundsConstraint.FromExpression((Expression<Func<Rectangle>>)(() => new Rectangle(x2, y2, w, h)));
            RelativeLayout.SetBoundsConstraint(BtnFeedSession, c2);

            var x3 = x1 + W + 14;
            var y3 = y2;
            var c3 = BoundsConstraint.FromExpression((Expression<Func<Rectangle>>)(() => new Rectangle(x3, y3, w, h)));
            RelativeLayout.SetBoundsConstraint(BtnNurseSession, c3);
            _rl1.ForceLayout();
        }

        /// <summary>
        /// Baby picture change hanlder
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event args</param>
        private void OnBabyPickClicked(object sender, EventArgs e)
        {
            if (ProfileManager.Instance.CurrentProfile != null)
            {
                if (ProfileManager.Instance.CurrentProfile.Babies.Count > 1)
                {
                    PageManager.Me.SetCurrentPage(typeof(BabySelectionPage));
                }
            }
        }

    }
}
