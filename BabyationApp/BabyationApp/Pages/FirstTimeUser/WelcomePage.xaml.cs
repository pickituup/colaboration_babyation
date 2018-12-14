using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using BabyationApp;
using BabyationApp.Managers;
using BabyationApp.Pages.Settings;
using Xamarin.Forms;
using Plugin.Connectivity;
using Xamarin.Forms.Xaml;
using System.Diagnostics;
using BabyationApp.Common;
using BabyationApp.Resources;

namespace BabyationApp.Pages.FirstTimeUser
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    /// <summary>
    /// This class represents the wellcome page from the design
    /// </summary>
    public partial class WelcomePage : PageBase
    {
        public AboutModel aboutModel;

        /// <summary>
        /// Constructor -- Initialize the model and binds buttons events and other ui actions
        /// </summary>
        public WelcomePage()
        {
            InitializeComponent();

            BindingContext = this;

            BtnNext.Clicked += BtnNext_Clicked;

            //DataManager.Instance.InitialSyncCompleteEvent += HandleInitialSyncCompleteEvent;
            LeftPageType = typeof(DashboardTabPage);
        }

        /// <summary>
        /// Initializes this page to show whether at startup or from the settings page as about page
        /// </summary>
        /// <param name="asAbout"></param>
        public void Initialize(bool asAbout = false)
        {
            Titlebar.IsVisible = true;
            WelcomContent.IsVisible = !asAbout;

            if( asAbout )
            {
                aboutModel = new AboutModel();
                BindingContext = aboutModel;
            
                Titlebar.LeftButton.IsVisible = true;
                Titlebar.LeftButton.SetDynamicResource(StyleProperty, "BackButton");
                Titlebar.RightButton.IsVisible = false;
                LeftPageType = typeof(SettingsPage);

                RootLayout.RaiseChild(AboutContent);
            }
            else
            {
                Title = AppResource.AboutUpper;
                LblTitle.TextColor = Color.FromHex("#F8A752");
                BtnToS.IsVisible = true;
                BtnNext.IsVisible = true;
                BtnVisitPage.IsVisible = true;

                RootLayout.RaiseChild(WelcomContent);
            }
        }

        void BtnNext_Clicked(object sender, EventArgs e)
        {
            PageManager.Me.SetCurrentPage(typeof(SignUpPage));
        }
    }

    public class AboutModel : ObservableObject
    {
        public AboutModel()
        {
            Reset();
        }

        public void Reset()
        {
            ShowAboutPage = true;
        }

        #region Public UI properties

        private bool _showAboutPage = true;
        public bool ShowAboutPage
        {
            get => _showAboutPage;
            set => SetPropertyChanged(ref _showAboutPage, value);
        }

        public string PageTitle => AppResource.AboutBabyationUpper;
        public string AboutTitleText => AppResource.AboutTitle;
        public string AboutBodyText => AppResource.AboutBody;

        #endregion
    }
}
