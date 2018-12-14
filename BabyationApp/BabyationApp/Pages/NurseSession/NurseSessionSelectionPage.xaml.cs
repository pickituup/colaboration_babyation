using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BabyationApp.Managers;
using BabyationApp.Models;
using Xamarin.Forms;

namespace BabyationApp.Pages.NurseSession
{
    /// <summary>
    /// This class represents the Nurse Session Selection page from the design
    /// </summary>
    public partial class NurseSessionSelectionPage : PageBase
    {
        /// <summary>
        /// Constructor -- Initialize the model and binds buttons events and other ui actions
        /// </summary>
        public NurseSessionSelectionPage()
        {
            try
            {
                InitializeComponent();
                //RLRoot.SizeChanged += BottleFeedPage_SizeChanged;

                BtnStartNursing.Clicked += (s, e) =>
                {
                    SessionManager.Instance.StartNursing();
                    PageManager.Me.SetCurrentPage(typeof(NurseSessionStartPage), view =>
                    {

                    });
                };

                BtnPastNursing.Clicked += (s, e) =>
                {
                    PageManager.Me.SetCurrentPage(typeof(NurseSessionLogPage), view =>
                    {
                        (view as NurseSessionLogPage).HistorySession =
                            HistoryManager.Instance.CreateSession(SessionType.Nurse);
                    });
                };

                Titlebar.IsVisible = true;
                LeftPageType = typeof(DashboardTabPage);
                Titlebar.LeftButton.IsVisible = true;
                Titlebar.LeftButton.SetDynamicResource(StyleProperty, "CancelButton");

                //BtnStartNursing.MiddleCirclePadding = 7;
                //BtnStartNursing.InnerCirclePadding = Device.RuntimePlatform == Device.Android ? 8.5 : 5;
                //BtnStartNursing.FontFamilyTop = "fonts/HurmeGeometricSans3Bold.otf#HurmeGeometricSans3 Bold";
                //BtnStartNursing.FontFamilyBottom = "fonts/LarsseitMedium.otf#Larsseit Medium";
                //BtnStartNursing.FontAttributesTop = FontAttributes.Bold;
                //BtnStartNursing.FontAttributesBottom = FontAttributes.None;

                //BtnPastNursing.MiddleCirclePadding = 7;
                //BtnPastNursing.InnerCirclePadding = Device.RuntimePlatform == Device.Android ? 8.5 : 5;
                //BtnPastNursing.FontFamilyTop = "fonts/HurmeGeometricSans3Bold.otf#HurmeGeometricSans3 Bold";
                //BtnPastNursing.FontFamilyBottom = "fonts/LarsseitMedium.otf#Larsseit Medium";
                //BtnPastNursing.FontAttributesTop = FontAttributes.Bold;
                //BtnPastNursing.FontAttributesBottom = FontAttributes.None;
            }
            catch (Exception ex)
            {
                Debugger.Break();
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
        /// Update the UI controls position/size when the page relative layout/size changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void BottleFeedPage_SizeChanged(object sender, EventArgs e)
        //{
        //    var w = RLRoot.Width * 3 / 10;
        //    var h = w;

        //    if (w <= 0) return;

        //    var x2 = RLRoot.Width / 2 - w / 2;
        //    var y2 = RLRoot.Height / 2 - h - 25;
        //    var c2 = BoundsConstraint.FromExpression((Expression<Func<Rectangle>>)(() => new Rectangle(x2, y2, w, h)));
        //    RelativeLayout.SetBoundsConstraint(BtnStartNursing, c2);

        //    var x3 = RLRoot.Width / 2 - w / 2;
        //    var y3 = RLRoot.Height / 2 + 25;
        //    var c3 = BoundsConstraint.FromExpression((Expression<Func<Rectangle>>)(() => new Rectangle(x3, y3, w, h)));
        //    RelativeLayout.SetBoundsConstraint(BtnPastNursing, c3);
        //    RLRoot.ForceLayout();
        //}
    }
}
