using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using BabyationApp.Managers;
using System.Collections.ObjectModel;

using BabyationApp.Models;
using BabyationApp.Pages;

namespace BabyationApp.Views
{
    public partial class ProductSetupView : ContentView
    {
        public event EventHandler DeviceLoaded;
        public event EventHandler SkipClicked;
        public ObservableCollection<PumpModel> DiscoveredDevices { get; private set; }

        public ProductSetupView()
        {
            InitializeComponent();
            DiscoveredDevices = new ObservableCollection<PumpModel>();

            deviceListView.ItemsSource = DiscoveredDevices;// App.BluetoothService.DiscoveredDevices;
            deviceListView.ItemSelected += DeviceSelected;

            //App.BluetoothService.Initialize();
            //App.BluetoothService.DeviceDiscovered += DeviceDiscovered;
            //App.BluetoothService.DeviceConnected += DeviceConnected;
            //App.BluetoothService.StartScanningForDevices();

            skipBtn.Clicked += SkipBtn_Clicked;
        }

        private void SkipBtn_Clicked(object sender, EventArgs e)
        {
            EventHandler handler = SkipClicked;
            if (handler != null)
            {
                handler(this, e);
            }
            //testing
            DiscoveredDevices.Clear();
            //App.BluetoothService.StartScanningForDevices();
        }

        void DeviceSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var device = e.SelectedItem as PumpModel;
            if (device != null)
            {
                //App.BluetoothService.ConnectToDevice(device);
            }
        }
    }
}
