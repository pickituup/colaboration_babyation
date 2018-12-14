using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace BabyationApp.Views
{
    public partial class StartSessionView : ContentView
    {
        public event EventHandler PumpClicked;
        public event EventHandler NurseClicked;
        public event EventHandler BottleClicked;

        public StartSessionView()
        {
            InitializeComponent();
            pumpBtn.Clicked += PumpBtn_Clicked;
            nurseBtn.Clicked += NurseBtn_Clicked;
            bottleBtn.Clicked += BottleBtn_Clicked;
        }

        private void BottleBtn_Clicked(object sender, EventArgs e)
        {
            EventHandler handler = BottleClicked;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void NurseBtn_Clicked(object sender, EventArgs e)
        {
            EventHandler handler = NurseClicked;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void PumpBtn_Clicked(object sender, EventArgs e)
        {
            EventHandler handler = PumpClicked;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
