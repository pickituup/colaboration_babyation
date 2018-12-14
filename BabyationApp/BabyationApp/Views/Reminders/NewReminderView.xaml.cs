using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace BabyationApp.Views
{
    public partial class NewReminderView : ContentView
    {
        public event EventHandler BackClicked;
        public event EventHandler SaveClicked;

        public NewReminderView()
        {
            InitializeComponent();

            backBtn.Clicked += BackBtn_Clicked;
            saveBtn.Clicked += SaveBtn_Clicked;
        }

        private void SaveBtn_Clicked(object sender, EventArgs e)
        {
            EventHandler handler = SaveClicked;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void BackBtn_Clicked(object sender, EventArgs e)
        {
            EventHandler handler = BackClicked;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
