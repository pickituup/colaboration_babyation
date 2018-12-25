﻿using BabyationApp.Controls.Views;
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
using BabyationApp.Resources;

namespace BabyationApp.Pages.Settings.PumpSettings {
    /// <summary>
    /// This class represents the My Pumps page from the design
    /// </summary>
    public partial class MyPumpsPage : PageBase {
        private ObservableCollection<PumpModel> _groups;

        /// <summary>
        /// Constructor -- Initialize the model and binds buttons events and other ui actions
        /// </summary>
        public MyPumpsPage() {
            InitializeComponent();

            Title = "MY PUMPS";
            Titlebar.IsVisible = true;
            Titlebar.LeftButton.IsVisible = true;
            Titlebar.LeftButton.SetDynamicResource(StyleProperty, "BackButton");
            LeftPageType = typeof(DashboardTabPage);

            //BtnAddPump.Clicked += (s, e) =>
            //{
            //    if (PumpManager.Instance.NewPumps.Count == 1)
            //    {
            //        PumpManager.Instance.SelectedPump = PumpManager.Instance.NewPumps[0];
            //        PageManager.Me.SetCurrentPage(typeof(BluetoothPumpSetupPage));
            //    }
            //    else
            //    {
            //        // If no pumps or multiple pumps go to Pump Detected Page
            //        PageManager.Me.SetCurrentPage(typeof(BluetoothPumpDetectedPage));
            //    }
            //};

            BtnAddPump.CommandEx = new Command(() =>
            {
                PumpManager.Instance.NewPumps.Add(new PumpModel()
                {
                    ActualPumpingMode = PumpMode.Expression,
                    ActualSpeed = 9,
                    ActualState = PumpState.Start,
                    ActualSuction = 9,
                    AdvertisedName = "Pump A",
                    AlertMessages = new ObservableCollection<string>() { "Hello", "world" },
                    BatteryLevel = 9,
                    ChargeState = true,
                    CurrentDuration = TimeSpan.FromSeconds(9),
                    DesiredExperience = 8,
                    DesiredPumpingMode = PumpMode.Expression,
                    DesiredSpeed = 8,
                    DesiredState = PumpState.Pause,
                    DesiredSuction = 8,
                    Device = null,
                    FirmwareRevision = "Hello world version",
                    HardwareRevision = "Hello world hardware version",
                    HasAlerts = false,
                    Id = Guid.NewGuid(),
                    Index = 0,
                    InUse = true,
                    IsConnected = true,
                    IsFocused = true,
                    IsSelected = false,
                    IsUpdateAvailable = true,
                    IsUpdating = true,
                    LeftBreastMilkLevel = 90,
                    Logs = new List<LogEntryModel>(),
                    ModelNumber = "Duck model number",
                    Name = "Cat dog",
                    PresetExperiences = new ObservableCollection<ExperienceModel>(),
                    PumpingSessions = new List<HistoryModel>(),
                    RightBreastMilkLevel = 80,
                    SerialNumber = "Cat dog duck serial number",
                    SoftwareRevision = "Duck software version",
                    Storage = StorageType.Fridge,
                    TagBool1 = true,
                    TagDouble1 = 32,
                    UpdatePercent = 4,
                    UserExperiences = new ObservableCollection<ExperienceModel>()
                });

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
            });

            _groups = new ObservableCollection<PumpModel>();
            PumpGroupItemItem myPumps = new PumpGroupItemItem(PumpManager.Instance.PairedPumps) { GroupTitle = "MY PUMP", GroupKey = "me" };
            //_groups.Add(myPumps);
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
        public override void AboutToShow() {
            base.AboutToShow();
            _groups.Clear();
            PumpGroupItemItem myPumps = new PumpGroupItemItem(PumpManager.Instance.PairedPumps) { GroupTitle = "MY PUMP", GroupKey = "me" };
            //_groups.Add(myPumps);





            foreach (PumpModel pumpModel in TODO_HARDCODED_BUIDLER())
            {
                _groups.Add(pumpModel);
            }






            //Titlebar.TitleTextColor = Color.FromHex("#11442b");
            listView.SelectedItem = null;
        }


        private ObservableCollection<PumpModel> TODO_HARDCODED_BUIDLER() {
            ObservableCollection<PumpModel> pumps = new ObservableCollection<PumpModel>();

            PumpModel pumpA = new PumpModel()
            {
                ActualPumpingMode = PumpMode.Expression,
                ActualSpeed = 9,
                ActualState = PumpState.Start,
                ActualSuction = 9,
                AdvertisedName = "Pump A",
                AlertMessages = new ObservableCollection<string>() { "Hello", "world" },
                BatteryLevel = 9,
                ChargeState = true,
                CurrentDuration = TimeSpan.FromSeconds(9),
                DesiredExperience = 8,
                DesiredPumpingMode = PumpMode.Expression,
                DesiredSpeed = 8,
                DesiredState = PumpState.Pause,
                DesiredSuction = 8,
                Device = null,
                FirmwareRevision = "Hello world version",
                HardwareRevision = "Hello world hardware version",
                HasAlerts = false,
                Id = Guid.NewGuid(),
                Index = 0,
                InUse = true,
                IsConnected = true,
                IsFocused = true,
                IsSelected = false,
                IsUpdateAvailable = true,
                IsUpdating = true,
                LeftBreastMilkLevel = 90,
                Logs = new List<LogEntryModel>(),
                ModelNumber = "Duck model number",
                Name = "Cat dog",
                PresetExperiences = new ObservableCollection<ExperienceModel>(),
                PumpingSessions = new List<HistoryModel>(),
                RightBreastMilkLevel = 80,
                SerialNumber = "Cat dog duck serial number",
                SoftwareRevision = "Duck software version",
                Storage = StorageType.Fridge,
                TagBool1 = true,
                TagDouble1 = 32,
                UpdatePercent = 4,
                UserExperiences = new ObservableCollection<ExperienceModel>()
            };

            PumpModel pumpB = new PumpModel()
            {
                ActualPumpingMode = PumpMode.Expression,
                ActualSpeed = 9,
                ActualState = PumpState.Start,
                ActualSuction = 9,
                AdvertisedName = "Pump A",
                AlertMessages = new ObservableCollection<string>() { "Hello", "world" },
                BatteryLevel = 9,
                ChargeState = false,
                CurrentDuration = TimeSpan.FromSeconds(9),
                DesiredExperience = 8,
                DesiredPumpingMode = PumpMode.Expression,
                DesiredSpeed = 8,
                DesiredState = PumpState.Pause,
                DesiredSuction = 8,
                Device = null,
                FirmwareRevision = "Hello world version",
                HardwareRevision = "Hello world hardware version",
                HasAlerts = false,
                Id = Guid.NewGuid(),
                Index = 1,
                InUse = false,
                IsConnected = true,
                IsFocused = false,
                IsSelected = false,
                IsUpdateAvailable = false,
                IsUpdating = false,
                LeftBreastMilkLevel = 90,
                Logs = new List<LogEntryModel>(),
                ModelNumber = "Duck model number",
                Name = "Cat dog",
                PresetExperiences = new ObservableCollection<ExperienceModel>(),
                PumpingSessions = new List<HistoryModel>(),
                RightBreastMilkLevel = 80,
                SerialNumber = "Cat dog duck serial number",
                SoftwareRevision = "Duck software version",
                Storage = StorageType.Fridge,
                TagBool1 = false,
                TagDouble1 = 32,
                UpdatePercent = 4,
                UserExperiences = new ObservableCollection<ExperienceModel>()
            };

            PumpModel pumpC = new PumpModel()
            {
                ActualPumpingMode = PumpMode.Expression,
                ActualSpeed = 9,
                ActualState = PumpState.Start,
                ActualSuction = 9,
                AdvertisedName = "Pump A",
                AlertMessages = new ObservableCollection<string>() { "Hello", "world" },
                BatteryLevel = 9,
                ChargeState = false,
                CurrentDuration = TimeSpan.FromSeconds(9),
                DesiredExperience = 8,
                DesiredPumpingMode = PumpMode.Expression,
                DesiredSpeed = 8,
                DesiredState = PumpState.Pause,
                DesiredSuction = 8,
                Device = null,
                FirmwareRevision = "Hello world version",
                HardwareRevision = "Hello world hardware version",
                HasAlerts = false,
                Id = Guid.NewGuid(),
                Index = 2,
                InUse = false,
                IsConnected = false,
                IsFocused = true,
                IsSelected = false,
                IsUpdateAvailable = false,
                IsUpdating = false,
                LeftBreastMilkLevel = 90,
                Logs = new List<LogEntryModel>(),
                ModelNumber = "Duck model number",
                Name = "Cat dog",
                PresetExperiences = new ObservableCollection<ExperienceModel>(),
                PumpingSessions = new List<HistoryModel>(),
                RightBreastMilkLevel = 80,
                SerialNumber = "Cat dog duck serial number",
                SoftwareRevision = "Duck software version",
                Storage = StorageType.Fridge,
                TagBool1 = false,
                TagDouble1 = 32,
                UpdatePercent = 4,
                UserExperiences = new ObservableCollection<ExperienceModel>()
            };

            PumpModel pumpD = new PumpModel()
            {
                ActualPumpingMode = PumpMode.Expression,
                ActualSpeed = 9,
                ActualState = PumpState.Start,
                ActualSuction = 9,
                AdvertisedName = "Pump A",
                AlertMessages = new ObservableCollection<string>() { "Hello", "world" },
                BatteryLevel = 9,
                ChargeState = false,
                CurrentDuration = TimeSpan.FromSeconds(9),
                DesiredExperience = 8,
                DesiredPumpingMode = PumpMode.Expression,
                DesiredSpeed = 8,
                DesiredState = PumpState.Pause,
                DesiredSuction = 8,
                Device = null,
                FirmwareRevision = "Hello world version",
                HardwareRevision = "Hello world hardware version",
                HasAlerts = false,
                Id = Guid.NewGuid(),
                Index = 3,
                InUse = false,
                IsConnected = false,
                IsFocused = false,
                IsSelected = false,
                IsUpdateAvailable = false,
                IsUpdating = false,
                LeftBreastMilkLevel = 90,
                Logs = new List<LogEntryModel>(),
                ModelNumber = "Duck model number",
                Name = "Cat dog",
                PresetExperiences = new ObservableCollection<ExperienceModel>(),
                PumpingSessions = new List<HistoryModel>(),
                RightBreastMilkLevel = 80,
                SerialNumber = "Cat dog duck serial number",
                SoftwareRevision = "Duck software version",
                Storage = StorageType.Fridge,
                TagBool1 = false,
                TagDouble1 = 32,
                UpdatePercent = 4,
                UserExperiences = new ObservableCollection<ExperienceModel>()
            };

            pumps.Add(pumpA);
            pumps.Add(pumpB);
            pumps.Add(pumpC);
            pumps.Add(pumpD);

            return pumps;
        }
    }

    /// <summary>
    /// This class is the model of a group of pumps to the MyPumpsPage listview
    /// </summary>
    class PumpGroupItemItem : ListViewList<PumpModel> {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="source">The list of pumps to show on a group on the my pumps page listview</param>
        public PumpGroupItemItem(ObservableCollection<PumpModel> source) : base(source) {
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
