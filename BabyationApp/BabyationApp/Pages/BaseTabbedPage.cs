using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

using Xamarin.Forms;

namespace BabyationApp.Pages
{
    public class BaseTabbedPage : TabbedPage
    {
        BabyationApp.Pages.SessionPage _sessionPage;
        BabyationApp.Pages.PresetsPage _presetsPage;
        BabyationApp.Pages.InventoryPage _inventoryPage;
        BabyationApp.Pages.SupportView _supportPage;
        ContentPage _settingsPage;
        //BabyationApp.Pages.RemindersPage _remindersPage;

        public BaseTabbedPage()
        {
            //this.Title = "Dashboard";
            
            _sessionPage = new SessionPage();
            _presetsPage = new PresetsPage();
            _inventoryPage = new InventoryPage();
            _supportPage = new SupportView();
            _settingsPage = new ContentPage();
           // _remindersPage = new RemindersPage();
            
            this.Children.Add(_sessionPage);
            this.Children.Add(_presetsPage);
            this.Children.Add(_inventoryPage);
            //this.Children.Add(_supportPage);
            //this.Children.Add(_settingsPage);
            //this.Children.Add(_remindersPage);

            this.CurrentPageChanged += BaseTabbedPage_CurrentPageChanged;
        }

        private void BaseTabbedPage_CurrentPageChanged(object sender, EventArgs e)
        {
            this.Title = this.CurrentPage.Title;
        }
    }
}
