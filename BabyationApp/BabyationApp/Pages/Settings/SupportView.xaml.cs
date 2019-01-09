using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using BabyationApp.Controls.Views;
using BabyationApp.Resources;

namespace BabyationApp.Pages.Settings
{
    public partial class SupportView : RootViewBase
    {
        public static readonly BindableProperty CaregiverFlowProperty = BindableProperty.Create(nameof(CaregiverFlow), typeof(bool), typeof(ProfileView), false);
        public bool CaregiverFlow
        {
            get => (bool)GetValue(CaregiverFlowProperty);
            set
            {
                SetValue(CaregiverFlowProperty, value);
            }
        }

        public SupportView()
        {
            InitializeComponent();

            if (CaregiverFlow)
            {
                Titlebar.IsVisible = true;
                Titlebar.Title = AppResource.FAQs;
                RootLayout.Style = (Style)Application.Current.Resources["Grid_NavigationOnTop"];
            }
            else
            {
                Titlebar.IsVisible = false;
                Padding = new Thickness(0);
            }
        }

        public override void AboutToShow()
        {
            base.AboutToShow();

            // Restore styles
            if (CaregiverFlow)
            {
                Titlebar.IsVisible = true;
                Titlebar.Title = AppResource.FAQs;
                RootLayout.Style = (Style)Application.Current.Resources["Grid_NavigationOnTop"];
                LeftPageType = null;
            }
            else
            {
                Titlebar.IsVisible = false;
                Padding = new Thickness(0);
                LeftPageType = typeof(MorePage);
            }
        }
    }
}
