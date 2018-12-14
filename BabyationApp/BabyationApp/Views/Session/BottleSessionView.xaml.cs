using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace BabyationApp.Views
{
    //Temporary, should be a full class under Models
    class Bottle
    {
        public Bottle(string date, string time, int ounces)
        {
            this.Date = date;
            this.Time = time;
            this.Ounces = ounces;
        }

        public string Date { private set; get; }
        public string Time { private set; get; }
        public int Ounces { private set; get; }
    };

    public partial class BottleSessionView : ContentView
    {
        public event EventHandler CancelClicked;
        public event EventHandler MilkClicked;
        public event EventHandler FormulaClicked;
        public event EventHandler StopwatchClicked;

        public BottleSessionView()
        {
            InitializeComponent();

            // Define some data.
            List<Bottle> bottles = new List<Bottle>
            {
                new Bottle("Yesterday", "noon", 16),
                new Bottle("Yesterday", "noonish", 15),
                new Bottle("Yesterday", "afternoon", 14),
                new Bottle("Yesterday", "prenoon", 13)
            };
            listFridge.ItemsSource = bottles;

            cancelBtn.Clicked += CancelBtn_Clicked;
            milkBtn.Clicked += MilkBtn_Clicked;
            formulaBtn.Clicked += FormulaBtn_Clicked;
            fridgeBtn.Clicked += FridgeBtn_Clicked;
            freezerBtn.Clicked += FreezerBtn_Clicked;
            manualBtn.Clicked += ManualBtn_Clicked;
            stopWatchBtn.Clicked += StopWatchBtn_Clicked;
        }

        private void StopWatchBtn_Clicked(object sender, EventArgs e)
        {
            EventHandler handler = StopwatchClicked;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void FreezerBtn_Clicked(object sender, EventArgs e)
        {
            bottleGrp.IsVisible = false;
            invGrp.IsVisible = true;
        }

        private void ManualBtn_Clicked(object sender, EventArgs e)
        {
            bottleGrp.IsVisible = true;
            invGrp.IsVisible = false;
        }

        private void FridgeBtn_Clicked(object sender, EventArgs e)
        {
            bottleGrp.IsVisible = false;
            invGrp.IsVisible = true;
        }

        private void FormulaBtn_Clicked(object sender, EventArgs e)
        {
            milkStorage.IsVisible = false;
            EventHandler handler = FormulaClicked;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void MilkBtn_Clicked(object sender, EventArgs e)
        {
            milkStorage.IsVisible = true;
            EventHandler handler = MilkClicked;
            if (handler != null)
            {
                handler(this, e);
            }
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
