using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace BabyationApp.Views
{
    public partial class ManualSessionView : ContentView
    {
        public event EventHandler CancelClicked;

        public ManualSessionView()
        {
            InitializeComponent();
            cancelBtn.Clicked += CancelBtn_Clicked; ;
        }

        private void CancelBtn_Clicked(object sender, EventArgs e)
        {
            EventHandler handler = CancelClicked;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
