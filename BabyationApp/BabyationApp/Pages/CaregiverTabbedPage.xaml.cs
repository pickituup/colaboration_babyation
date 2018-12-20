using System;
using System.Collections.Generic;
using BabyationApp.Controls;
using BabyationApp.Controls.Buttons;
using BabyationApp.Controls.Views;
using Xamarin.Forms;

namespace BabyationApp.Pages
{
    public partial class CaregiverTabbedPage : PageBase
    {
        public enum TabType
        {
            Dashboard, Inventory, Settings, FAQ
        }

        private ButtonExGroup _btnGroup = new ButtonExGroup();
        private List<IRootView> _tabViews;
        private ButtonBase _lastSelectedButton;

        /// <summary>
        /// Constructor -- Initialize the model and binds buttons events and other ui actions
        /// </summary> 
        public CaregiverTabbedPage()
        {
            try
            {
                InitializeComponent();

                _tabViews = new List<IRootView>() { PageCaregiverDashboard, PageInventory, PageSettings, PageFAQ };

                _btnGroup.Toggled += (sender, btn, index) =>
                {
                    if (btn != BtnSettings)
                    {
                        _lastSelectedButton = btn;
                    }
                    SetCurrentIndex(index);
                };

                foreach (View v in _gridTabBtns.Children)
                {
                    var btn = v as ButtonEx;
                    if (btn != null)
                    {
                        _btnGroup.AddButton(btn);
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        /// <summary>
        /// Selects a current tab and shows it
        /// </summary>
        /// <param name="type"></param>
        public void SetCurrentTab(TabType type)
        {
            switch (type)
            {
                case TabType.Dashboard:
                    _btnGroup.UpdateCurrentButton(BtnDash);
                    break;
                case TabType.Inventory:
                    _btnGroup.UpdateCurrentButton(BtnInventory);
                    break;
                case TabType.Settings:
                    _btnGroup.UpdateCurrentButton(BtnSettings);
                    break;
                case TabType.FAQ:
                    _btnGroup.UpdateCurrentButton(BtnFAQ);
                    break;
            }
        }

        /// <summary>
        /// Gets the currently selected tab type
        /// </summary> 
        public TabType GetCurrentTabType()
        {
            if (_lastSelectedButton == BtnDash)
            {
                return TabType.Dashboard;
            }
            else if (_lastSelectedButton == BtnInventory)
            {
                return TabType.Inventory;
            }
            else if (_lastSelectedButton == BtnSettings)
            {
                return TabType.Settings;
            }
            else if (_lastSelectedButton == BtnFAQ)
            {
                return TabType.FAQ;
            }
            else
            {
                return TabType.FAQ;
            }
        }

        /// <summary>
        /// Sets the current tab based on index
        /// </summary>
        /// <param name="index"></param>
        private void SetCurrentIndex(int index)
        {
            for (int i = 0; i < _gridTabContents.Children.Count; i++)
            {
                if (i == index)
                {
                    _tabViews[i].AboutToShow();
                }
                _gridTabContents.Children[i].IsVisible = i == index;
            }
        }

        private bool _isFirstShow = true;
        /// <summary>
        /// Gets called when this page is about to show and performs the initialization
        /// </summary>
        public override void AboutToShow()
        {
            base.AboutToShow();
            
            ButtonBase currentBtn = _lastSelectedButton;
            if (_isFirstShow)
            {
                _isFirstShow = false;
                currentBtn = BtnDash;
            }

            if (currentBtn != null)
            {
                _btnGroup.UpdateCurrentButton(currentBtn);
            }
        }

        /// <summary>
        /// Gets called when a page is instantiated
        /// </summary>
        public override void PageCreationDone()
        {
            base.PageCreationDone();
            foreach (IRootView rvb in _tabViews)
            {
                rvb.PageCreationDone();
            }
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();
            AboutToShow();
        }
    }
}
