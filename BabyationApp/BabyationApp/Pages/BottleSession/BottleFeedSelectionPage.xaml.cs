using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BabyationApp.Managers;
using BabyationApp.Models;
using BabyationApp.Pages.NurseSession;
using Xamarin.Forms;

namespace BabyationApp.Pages.BottleSession
{
    /// <summary>
    /// This class represents Bottle Feed Selection page from the design
    /// </summary>
    public partial class BottleFeedSelectionPage : PageBase
    {
        /// <summary>
        /// Constructor -- Initialize the model and binds buttons events and other ui actions
        /// </summary>
        public BottleFeedSelectionPage()
        {
            try
            {

                InitializeComponent();
                //RLRoot.SizeChanged += BottleFeedPage_SizeChanged;
                BtnStartFeeding.Clicked += (s, e) =>
                {
                    SessionManager.Instance.StartBottleFeeding();
                    PageManager.Me.SetCurrentPage(typeof(BottleFeedStartPage), view =>
                    {
                        //(view as BottleFeedStartPage).SourcePageType = typeof(DashboardTabPage);
                    });
                };

                BtnFeedingLog.Clicked += (s, e) =>
                {
                    PageManager.Me.SetCurrentPage(typeof(BottleFeedLogPage), view =>
                    {
                        (view as BottleFeedLogPage).HistorySession =
                            HistoryManager.Instance.CreateSession(SessionType.BottleFeed);
                    });
                };

                Titlebar.IsVisible = true;
                LeftPageType = CurrentDashboard();
                Titlebar.LeftButton.IsVisible = true;
                Titlebar.LeftButton.SetDynamicResource(StyleProperty, "CancelButton");

                //BtnStartFeeding.MiddleCirclePadding = 7;
                //BtnStartFeeding.InnerCirclePadding = Device.RuntimePlatform == Device.Android ? 8.5 : 5;
                //BtnStartFeeding.FontFamilyTop = "fonts/HurmeGeometricSans3Bold.otf#HurmeGeometricSans3 Bold";
                //BtnStartFeeding.FontFamilyBottom = "fonts/LarsseitMedium.otf#Larsseit Medium";
                //BtnStartFeeding.FontAttributesTop = FontAttributes.Bold;
                //BtnStartFeeding.FontAttributesBottom = FontAttributes.None;

                //BtnFeedingLog.MiddleCirclePadding = 7;
                //BtnFeedingLog.InnerCirclePadding = Device.RuntimePlatform == Device.Android ? 8.5 : 5;
                //BtnFeedingLog.FontFamilyTop = "fonts/HurmeGeometricSans3Bold.otf#HurmeGeometricSans3 Bold";
                //BtnFeedingLog.FontFamilyBottom = "fonts/LarsseitMedium.otf#Larsseit Medium";
                //BtnFeedingLog.FontAttributesTop = FontAttributes.Bold;
                //BtnFeedingLog.FontAttributesBottom = FontAttributes.None;
            }
            catch (Exception ex)
            {
                Debugger.Break();
                throw;
            }
        }

        /// <summary>
        /// Gets called when this page is about to show and performs the initialization
        /// </summary>
        public override void AboutToShow()
        {
            base.AboutToShow();
        }

        /// <summary>
        /// Reposition the controls when the page size changes
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event args</param>
        //private void BottleFeedPage_SizeChanged(object sender, EventArgs e)
        //{
        //    var w = RLRoot.Width * 3 / 10;
        //    var h = w;

        //    if (w <= 0) return;

        //    var x2 = RLRoot.Width / 2 - w / 2;
        //    var y2 = RLRoot.Height / 2 - h - 25;
        //    var c2 = BoundsConstraint.FromExpression((Expression<Func<Rectangle>>)(() => new Rectangle(x2, y2, w, h)));
        //    RelativeLayout.SetBoundsConstraint(BtnStartFeeding, c2);

        //    var x3 = RLRoot.Width / 2 - w / 2;
        //    var y3 = RLRoot.Height / 2 + 25;
        //    var c3 = BoundsConstraint.FromExpression((Expression<Func<Rectangle>>)(() => new Rectangle(x3, y3, w, h)));
        //    RelativeLayout.SetBoundsConstraint(BtnFeedingLog, c3);
        //    RLRoot.ForceLayout();
        //}

    }
}
