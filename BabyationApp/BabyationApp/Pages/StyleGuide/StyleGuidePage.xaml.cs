using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace BabyationApp.Pages.StyleGuide
{
    public partial class StyleGuidePage : PageBase
    {
        public StyleGuidePage()
        {
            InitializeComponent();

            Titlebar.IsVisible = true;
            LeftPageType = typeof(DashboardTabPage);
            Titlebar.LeftButton.IsVisible = true;
            Titlebar.LeftButton.SetDynamicResource(StyleProperty, "CancelButton");
        }
    }
}
