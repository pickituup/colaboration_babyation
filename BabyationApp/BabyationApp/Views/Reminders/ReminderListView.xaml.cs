using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace BabyationApp.Views
{
    public partial class ReminderListView : ContentView
    {
        public event EventHandler NewClicked;

        public ReminderListView()
        {
            InitializeComponent();

            addNewBtn.Clicked += AddNewBtn_Clicked;
        }

        private void AddNewBtn_Clicked(object sender, EventArgs e)
        {
            EventHandler handler = NewClicked;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
