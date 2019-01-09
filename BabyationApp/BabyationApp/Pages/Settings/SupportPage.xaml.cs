
using BabyationApp.Resources;
using Xamarin.Forms;

namespace BabyationApp.Pages.Settings
{
    public partial class SupportPage : PageBase
    {
        public SupportPage()
        {
            InitializeComponent();

            Titlebar.IsVisible = true;
            Titlebar.Title = AppResource.FAQs;
            LeftPageType = typeof(MorePage);
            Titlebar.LeftButton.IsVisible = true;
            Titlebar.LeftButton.SetDynamicResource(StyleProperty, "BackButton");
            Titlebar.RightButton.IsVisible = false;
            RootLayout.Style = (Style)Application.Current.Resources["StackLayout_NavigationOnTop"];
        }

        public override void AboutToShow()
        {
            base.AboutToShow();

            // Restore styles
            Titlebar.IsVisible = true;
            RootLayout.Style = (Style)Application.Current.Resources["StackLayout_NavigationOnTop"];
        }

        void ProfileView_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "LeftPageType")
            {
                LeftPageType = ((SupportView)sender).LeftPageType;
                Titlebar.LeftButton.IsVisible = (null != LeftPageType);
            }
        }
    }
}
