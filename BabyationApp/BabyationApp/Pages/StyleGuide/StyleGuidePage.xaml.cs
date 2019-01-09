using System;
using System.Collections.Generic;
using BabyationApp.Pages.Settings;
using Xamarin.Forms;

namespace BabyationApp.Pages.StyleGuide
{
    public partial class StyleGuidePage : PageBase
    {
        public StyleGuidePage()
        {
            InitializeComponent();

            Titlebar.IsVisible = true;
            LeftPageType = typeof(MorePage);
            Titlebar.LeftButton.IsVisible = true;
            Titlebar.LeftButton.SetDynamicResource(StyleProperty, "BackButton");
        }
    }
}
