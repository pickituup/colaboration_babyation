using BabyationApp.Controls.Buttons;
using BabyationApp.Controls.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BabyationApp.Common;
using BabyationApp.Interfaces;
using BabyationApp.Models;
using Xamarin.Forms;
using BabyationApp.Managers;

namespace BabyationApp.Pages.BottleSession
{
    /// <summary>
    /// This class represents Inventory page from the design
    /// </summary>
    public partial class MyInventoryPage : PageBase
    {
        /// <summary>
        /// Constructor -- Initialize the model and binds buttons events and other ui actions
        /// </summary>
        public MyInventoryPage()
        {
            InitializeComponent();

            Titlebar.IsVisible = true;

            LeftPageType = typeof(DashboardTabPage);

            Titlebar.LeftButton.IsVisible = true;
            Titlebar.LeftButton.SetDynamicResource(StyleProperty, "CloseButton");

            InventoryView.ItemUseNowEvent += OnItemUseNowHandler;
        }

        void OnItemUseNowHandler(HistoryModel model)
        {
            if (IsVisible)
            {
                PageManager.Me.SetCurrentPage(typeof(InventoryUseNowPopupPage), view =>
                {
                    (view as InventoryUseNowPopupPage).UseNowHistoryModelItem = model;
                    (view as InventoryUseNowPopupPage).SourcePageType = typeof(MyInventoryPage);
                });
            }
        }

        /// <summary>
        /// Gets called when this page is about to show and performs the initialization
        /// </summary>
        public override void AboutToShow() 
        {
            base.AboutToShow();
            InventoryView.Initialize();
        }                    
    }
}
