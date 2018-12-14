using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace BabyationApp.Views
{
    public partial class BabyInfoView : ContentView
    {
        public event EventHandler Clicked;

        public BabyInfoView()
        {
            InitializeComponent();

            nextBtn.Clicked += NextBtn_Clicked;
        }

        private void NextBtn_Clicked(object sender, EventArgs e)
        {
            EventHandler handler = Clicked;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
