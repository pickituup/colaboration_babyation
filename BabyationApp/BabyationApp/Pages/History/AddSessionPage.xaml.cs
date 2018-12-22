using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BabyationApp.Managers;
using BabyationApp.Models;
using BabyationApp.Pages.BottleSession;
using BabyationApp.Pages.NurseSession;
using BabyationApp.Resources;
using Xamarin.Forms;

namespace BabyationApp.Pages.History
{
    /// <summary>
    /// This class represents the Add Session Page from the design
    /// </summary>
    public partial class AddSessionPage : PageBase
    {
        /// <summary>
        /// Constructor -- Initialize the model and binds buttons events and other ui actions
        /// </summary>
        public AddSessionPage()
        {
            InitializeComponent();
            RLRoot.SizeChanged += OnRootViewSizeChanged;

            BtnPumpSession.Clicked += (s, e) =>
            {
                PageManager.Me.SetCurrentPage(typeof(LogAPastPumpPage));
            };
            BtnNurseSession.Clicked += (s, e) =>
            {
                SelectChildToFeedAction(typeof(NurseSessionLogPage));
            };
            BtnFeedSession.Clicked += (s, e) =>
            {
                SelectChildToFeedAction(typeof(BottleFeedLogPage));
            };
            LeftPageType = typeof(DashboardTabPage);

            Titlebar.IsVisible = true;
            Titlebar.LeftButton.IsVisible = true;
            Titlebar.LeftButton.SetDynamicResource(StyleProperty, "CancelButton");
        }

        private void SelectChildToFeedAction(Type nextPage)
        {
            if (ProfileManager.Instance.CurrentProfile.Babies != null)
            {
                PageManager.Me.SetCurrentPage(typeof(SelectChildPage), view => 
                { 
                    (view as SelectChildPage).NextPageType = nextPage;
                });
            }
            else
            {
                ModalAlertPage.ShowAlertWithClose(AppResource.NoChildError);
            }
        }
        /// <summary>
        /// Update the UI controls position/size when the page layout/size changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRootViewSizeChanged(object sender, EventArgs e)
        {
            var w = RLRoot.Width * 3 / 10;
            var h = w;

            if (w <= 0) return;

            var x1 = (RLRoot.Width - w) / 2;
            var y1 = RLRoot.Height / 3 - h;
            var c1 = BoundsConstraint.FromExpression((Expression<Func<Rectangle>>)(() => new Rectangle(x1, y1, w, h)));
            RelativeLayout.SetBoundsConstraint(BtnPumpSession, c1);

            var x2 = RLRoot.Width / 1.7 - w - w / 2;
            var y2 = RLRoot.Height / 2 - h / 1.1;
            var c2 = BoundsConstraint.FromExpression((Expression<Func<Rectangle>>)(() => new Rectangle(x2, y2, w, h)));
            RelativeLayout.SetBoundsConstraint(BtnFeedSession, c2);

            var x3 = RLRoot.Width / 2.3 + w / 2;
            var y3 = RLRoot.Height / 2 - h / 1.1;
            var c3 = BoundsConstraint.FromExpression((Expression<Func<Rectangle>>)(() => new Rectangle(x3, y3, w, h)));
            RelativeLayout.SetBoundsConstraint(BtnNurseSession, c3);
            RLRoot.ForceLayout();
        }
    }
}
