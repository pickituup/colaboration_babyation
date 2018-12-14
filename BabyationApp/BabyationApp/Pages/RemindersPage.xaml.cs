using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace BabyationApp.Pages
{
    public partial class RemindersPage : ContentPage
    {
        BabyationApp.Views.ReminderListView _listView;
        BabyationApp.Views.NewReminderView _newView;

        public RemindersPage()
        {
            this.Title = "Reminders";
            InitializeComponent();

            _listView = new Views.ReminderListView();
            _newView = new Views.NewReminderView();

            Content = _listView;

            _listView.NewClicked += _listView_NewClicked;
            _newView.BackClicked += _newView_BackClicked;
            _newView.SaveClicked += _newView_SaveClicked;
        }

        private void _newView_SaveClicked(object sender, EventArgs e)
        {
            Content = _listView;
        }

        private void _newView_BackClicked(object sender, EventArgs e)
        {
            Content = _listView;
        }

        private void _listView_NewClicked(object sender, EventArgs e)
        {
            Content = _newView;
        }
    }
}
