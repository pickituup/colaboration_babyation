using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using BabyationApp.Controls.Views;

namespace BabyationApp.Pages
{
    public partial class SupportView : RootViewBase
    {
        public SupportView()
        {
            InitializeComponent();

            Titlebar.IsVisible = true;
            Titlebar.Title = "FAQ";
            RootLayout.Style = (Style)Application.Current.Resources["Grid_NavigationOnTop"];
        }

        public override void AboutToShow()
        {
            base.AboutToShow();

            // Restore style:
            Titlebar.IsVisible = true;
            Titlebar.Title = "FAQ";
            RootLayout.Style = (Style)Application.Current.Resources["Grid_NavigationOnTop"];
        }
    }
}
