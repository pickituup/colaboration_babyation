using System;
using System.Collections.Generic;
using Xamarin.Forms;
using BabyationApp.Controls.Buttons;
using BabyationApp.Controls.Views;
using BabyationApp.Pages.Settings;

namespace BabyationApp.Pages {
    public interface IDialog {
        DashboardTabPage RelativeDashboardTabPage { get; set; }
    }

    /// <summary>
    /// This class is the container of the 5 tab pages (see enum TabType)
    /// </summary>
    public partial class DashboardTabPage : PageBase {
        public enum TabType {
            Dashboard, History, Modes, Alarms, Settings
        }

        private ButtonExGroup _btnGroup = new ButtonExGroup();
        private List<RootViewBase> _tabViews;
        private ButtonBase _lastSelectedButton;

        private IDialog _lastAddedDialog;

        public void ShowDialog(IDialog dialog) {
            if (dialog != null && dialog is View viewDialog)
            {
                if (_lastAddedDialog != null)
                {
                    _dialogSpot_AbsoluteLayout.Children.Remove((View)_lastAddedDialog);
                }

                _lastAddedDialog = dialog;
                _lastAddedDialog.RelativeDashboardTabPage = this;

                AbsoluteLayout.SetLayoutBounds(viewDialog, new Rectangle(0, 0, 1, 1));
                AbsoluteLayout.SetLayoutFlags(viewDialog, AbsoluteLayoutFlags.All);

                _dialogSpot_AbsoluteLayout.Children.Add(viewDialog);
                _dialogSpot_AbsoluteLayout.IsVisible = true;
                _dialogSpot_AbsoluteLayout.InputTransparent = false;
            }
        }

        public void HideDialog() {
            _dialogSpot_AbsoluteLayout.Children.Remove((View)_lastAddedDialog);
            _dialogSpot_AbsoluteLayout.IsVisible = false;
            _dialogSpot_AbsoluteLayout.InputTransparent = true;

            _lastAddedDialog.RelativeDashboardTabPage = null;
            _lastAddedDialog = null;
        }

        /// <summary>
        /// Constructor -- Initialize the model and binds buttons events and other ui actions
        /// </summary> 
        public DashboardTabPage() {
            try
            {
                InitializeComponent();

                _tabViews = new List<RootViewBase>() { PageDashboardDetail, PageHistory, PageModesDashboard, PageReminder, PageSettings };

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
        public void SetCurrentTab(TabType type) {
            switch (type)
            {
                case TabType.Dashboard:
                    _btnGroup.UpdateCurrentButton(BtnDash);
                    break;
                case TabType.History:
                    _btnGroup.UpdateCurrentButton(BtnHistory);
                    break;
                case TabType.Modes:
                    _btnGroup.UpdateCurrentButton(BtnModes);
                    break;
                case TabType.Alarms:
                    _btnGroup.UpdateCurrentButton(BtnAlarms);
                    break;
            }
        }

        /// <summary>
        /// Gets the currently selected tab type
        /// </summary> 
        public TabType GetCurrentTabType() {
            if (_lastSelectedButton == BtnDash)
            {
                return TabType.Dashboard;
            }
            else if (_lastSelectedButton == BtnHistory)
            {
                return TabType.History;
            }
            else if (_lastSelectedButton == BtnModes)
            {
                return TabType.Modes;
            }
            else if (_lastSelectedButton == BtnAlarms)
            {
                return TabType.Alarms;
            }
            else
            {
                return TabType.Settings;
            }
        }

        /// <summary>
        /// Sets the current tab based on index
        /// </summary>
        /// <param name="index"></param>
        private void SetCurrentIndex(int index) {
            for (int i = 0; i < _gridTabContents.Children.Count; i++)
            {
                if (i == index)
                {
                    _tabViews[i].AboutToShow();
                }
                _gridTabContents.Children[i].IsVisible = i == index;
            }

            if (_btnGroup.CurrentButton == BtnSettings)
            {
                PageManager.Me.SetCurrentPage(typeof(MorePage));
            }
        }

        private bool _isFirstShow = true;
        /// <summary>
        /// Gets called when this page is about to show and performs the initialization
        /// </summary>
        public override void AboutToShow() {
            base.AboutToShow();

            IsFirstTime = "no";

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
        public override void PageCreationDone() {
            base.PageCreationDone();
            foreach (RootViewBase rvb in _tabViews)
            {
                rvb.PageCreationDone();
            }
        }


        protected override void OnAppearing() {
            base.OnAppearing();
            AboutToShow();
        }

        /// <summary>
        /// Gets sets whether this is the first time showing of this page
        /// </summary>
        protected string IsFirstTime {
            get
            {
                return Helpers.Settings.IsFirstTimeUser;
            }
            set
            {
                if (Helpers.Settings.IsFirstTimeUser != value)
                {
                    Helpers.Settings.IsFirstTimeUser = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
