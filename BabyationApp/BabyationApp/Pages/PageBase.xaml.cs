using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BabyationApp.Controls;
using Xamarin.Forms;

using BabyationApp.Controls.Views;
using BabyationApp.Helpers;
using BabyationApp.Interfaces;
using System.Diagnostics;
using BabyationApp.Managers;
using BabyationApp.Models;

namespace BabyationApp.Pages
{
    /// <summary>
    /// This class is the base page of all the pages in this application
    /// </summary>
    public partial class PageBase : ContentPage, IRootView
    {
        private Titlebar _titleBar;
        /// <summary>
        /// Constructor
        /// </summary>
        public PageBase()
        {
            try {
            InitializeComponent();

            ShouldExitOnBackButton = false;
            HandleBackPressIfBackButtonIsMissing = false;
            _titleBar = this.FindTemplateElementByName<BabyationApp.Controls.Views.Titlebar>("MyTitlebar");

            } catch (Exception ex) {
                Debugger.Break();
                throw;
            }
        }

        /// <summary>
        /// Gets the titlebar of the page
        /// </summary>
        public Titlebar Titlebar { get { return _titleBar; } }

        /// <summary>
        /// Gets/Sets the left page type to show when left button clicks on titlebar
        /// </summary>
        public Type LeftPageType { get; set; }

        /// <summary>
        /// Gets/Sets the right page type to show when right button clicks on titlebar
        /// </summary>
        public Type RightPageType { get; set; }

        /// <summary>
        /// Gets/Sets whether the current page should exit on left/back button click of this page
        /// </summary>
        public  bool ShouldExitOnBackButton { get; set; }

        public bool HandleBackPressIfBackButtonIsMissing { get; set; }

        /// <summary>
        /// Gets called when a page is instantiated
        /// </summary>
        public virtual void PageCreationDone()
        {
            PageManager.Me.SetNavPagesForPage(this);
        }

        /// <summary>
        /// Handles the back/left button press
        /// </summary>
        /// <returns>Returns true if ShouldExitOnBackButton is false; otherwise false</returns>
        protected override bool OnBackButtonPressed()
        {
            if (Titlebar.LeftButton.IsVisible)
            {
                Titlebar.LeftButton.AnimateClicked();
            }
            else if (HandleBackPressIfBackButtonIsMissing && LeftPageType != null)
            {
                PageManager.Me.SetCurrentPage(LeftPageType);
            }
            base.OnBackButtonPressed();
            return !ShouldExitOnBackButton;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            //AboutToShow();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            //AboutToHide();
        }

        /// <summary>
        /// Gets called when a page is about to show
        /// </summary>
        public virtual void AboutToShow()
        {
            if (null != Titlebar)
            {
                App.Instance.PlatformAPI?.UpdateStatusBar(Titlebar.TitleBackColor.ToHexString(), Titlebar.IsVisible);

                if (Device.RuntimePlatform == Device.iOS)
                {
                    this.Padding = new Thickness(0, Titlebar.IsVisible ? Double.Parse(Application.Current.Resources["StatusBarHeight"].ToString()) : 0, 0, 0);
                }
            }
        }

        /// <summary>
        /// Gets called when a page is about to hide
        /// </summary>
        public virtual void AboutToHide()
        {

        }

        /// <summary>
        /// Updates titlbar's title, text/background colors
        /// </summary>
        public void UpdateTitlebarInfo(bool visible, Color backColor)
        {
            if (null != Titlebar)
            {
                Titlebar.IsVisible = visible;
                Titlebar.TitleBackColor = backColor;

                App.Instance.PlatformAPI?.UpdateStatusBar(Titlebar.TitleBackColor.ToHexString(), visible);
            }
        }

        /// <summary>
        /// Not sure that this is the best place to this logic
        /// </summary>
        /// <returns>The dashboard.</returns>
        public Type CurrentDashboard()
        {
            ProfileModel profileModel = ProfileManager.Instance.CurrentProfile;
            if( null != profileModel && null != profileModel.CurrentCaregiver && profileModel.CaregiverAccountSelected )
            {
                return typeof(CaregiverTabbedPage);
            }

            return typeof(DashboardTabPage);
        }
    }
}
