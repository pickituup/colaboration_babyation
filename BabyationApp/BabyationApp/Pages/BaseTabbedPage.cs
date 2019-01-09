using System;
using Xamarin.Forms;

namespace BabyationApp.Pages
{
    public class BaseTabbedPage : TabbedPage
    {
        SessionPage _sessionPage;
        PresetsPage _presetsPage;
        InventoryPage _inventoryPage;
        Settings.SupportView _supportPage;

        ContentPage _settingsPage;

        public BaseTabbedPage()
        {
            _sessionPage = new SessionPage();
            _presetsPage = new PresetsPage();
            _inventoryPage = new InventoryPage();
            _supportPage = new Settings.SupportView();
            _settingsPage = new ContentPage();
            
            Children.Add(_sessionPage);
            Children.Add(_presetsPage);
            Children.Add(_inventoryPage);

            CurrentPageChanged += BaseTabbedPage_CurrentPageChanged;
        }

        private void BaseTabbedPage_CurrentPageChanged(object sender, EventArgs e)
        {
            Title = CurrentPage.Title;
        }
    }
}
