using BabyationApp.Controls.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BabyationApp.Models;
using Xamarin.Forms;
using BabyationApp.Managers;

namespace BabyationApp.Pages.Settings.PumpSettings
{
    /// <summary>
    /// This class represents the My Pumps page from the design
    /// </summary>
    public partial class MyPumpsPage : PageBase
    {
        private ObservableCollection<PumpGroupItemItem> _groups;

        /// <summary>
        /// Constructor -- Initialize the model and binds buttons events and other ui actions
        /// </summary>
        public MyPumpsPage()
        {
            InitializeComponent();

            Title = "MY PUMPS";
            Titlebar.IsVisible = true;
            Titlebar.LeftButton.IsVisible = true;
            Titlebar.LeftButton.SetDynamicResource(StyleProperty, "CloseButton");
            LeftPageType = typeof(DashboardTabPage);
            Titlebar.RightButton.IsVisible = true;
            Titlebar.RightButton.Text = "HELP";

            BtnAddPump.Clicked += (s, e) =>
            {
                if (PumpManager.Instance.NewPumps.Count == 1)
                {
                    PumpManager.Instance.SelectedPump = PumpManager.Instance.NewPumps[0];
                    PageManager.Me.SetCurrentPage(typeof(BluetoothPumpSetupPage));
                }
                else
                {
                    // If no pumps or multiple pumps go to Pump Detected Page
                    PageManager.Me.SetCurrentPage(typeof(BluetoothPumpDetectedPage));
                }
            };

            _groups = new ObservableCollection<PumpGroupItemItem>();
            PumpGroupItemItem myPumps = new PumpGroupItemItem(PumpManager.Instance.PairedPumps) { GroupTitle = "MY PUMP", GroupKey = "me" };
            _groups.Add(myPumps);
            listView.ItemsSource = _groups;
            listView.ItemSelected += (s, e) =>
            {
                if (e.SelectedItem != null)
                {
                    PumpManager.Instance.SelectedPump = (PumpModel)e.SelectedItem;
                    PageManager.Me.SetCurrentPage(typeof(PumpDetailPage));
                }
            };

            //PumpManager.Instance.PairedPumps.CollectionChanged += (sender, args) =>
            //{
            //    // TODO: Is this the correct way to manage the list of pumps?
            //    Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            //    {
            //        _groups.Remove(_myPumps);
            //        _myPumps = new PumpGroupItemItem(PumpManager.Instance.PairedPumps) { GroupTitle = "MY PUMP", GroupKey = "me" };
            //        _groups.Add(_myPumps);
            //    });
            //};
        }

        /// <summary>
        /// Gets called when this page is about to show and performs the initialization
        /// </summary>
        public override void AboutToShow()
        {
            base.AboutToShow();
            _groups.Clear();
            PumpGroupItemItem myPumps = new PumpGroupItemItem(PumpManager.Instance.PairedPumps) { GroupTitle = "MY PUMP", GroupKey = "me" };
            _groups.Add(myPumps);
            //Titlebar.TitleTextColor = Color.FromHex("#11442b");
            listView.SelectedItem = null;
        }        
    }

    /// <summary>
    /// This class is the model of a group of pumps to the MyPumpsPage listview
    /// </summary>
    class PumpGroupItemItem : ListViewList<PumpModel>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="source">The list of pumps to show on a group on the my pumps page listview</param>
        public PumpGroupItemItem(ObservableCollection<PumpModel> source) : base(source)
        {
        }

        /// <summary>
        /// This pumps group title
        /// </summary>
        public String GroupTitle { get; set; }

        /// <summary>
        /// Group key of this pumps group
        /// </summary>
        public String GroupKey { get; set; }
    }
}
