using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using BabyationApp.Models;
using BabyationApp.DataObjects;
using Xamarin.Forms;
using BabyationApp.Helpers;

namespace BabyationApp.Managers
{
    public class PumpManager
    {
        private static PumpManager _instance = null;
        private BluetoothManager _bluetoothManager;
        private PumpModel _connectedPump = null;
        private PumpModel _selectedPump = null;
        private ObservableCollection<PumpModel> _pairedPumps = new ObservableCollection<PumpModel>();
        private ObservableCollection<PumpModel> _newPumps = new ObservableCollection<PumpModel>();
        private bool _isStarted = false;
        public event EventHandler<PumpConnectedEventArgs> PumpConnectedEvent;
        public event EventHandler PumpDisconnectedEvent;
        public event EventHandler FirmwareAvailableEvent;
        //private bool _hasPumpsInUse = false;
        private Firmware _firmware = null;

        public static PumpManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PumpManager();
                }
                return _instance;
            }
        }

        private PumpManager()
        {
            _bluetoothManager = BluetoothManager.Instance;

            _bluetoothManager.PumpDiscovered += _bluetoothManager_PumpDiscovered;
            _bluetoothManager.PumpConnected += _bluetoothManager_PumpConnected;
            _bluetoothManager.PumpDisconnected += _bluetoothManager_PumpDisconnected;
            _bluetoothManager.LogUpdated += _bluetoothManager_LogUpdated;
        }

        public void Reset()
        {
            _connectedPump = null;
            _selectedPump = null;
            _pairedPumps = new ObservableCollection<PumpModel>();
            _newPumps = new ObservableCollection<PumpModel>();
        }

        public async Task Initialize()
        {
            DataManager dataManager = DataManager.Instance;
            PumpModel pumpModel;

            // Build the paired list from those pumps
            _knownPumps = (await dataManager.GetAllPumps()).ToList();

            foreach (Pump pump in _knownPumps)
            {
                //if (pump.InUse)
                //{
                //    _hasPumpsInUse = true;
                //}

                pumpModel = new PumpModel()
                {
                    Name = pump.Name,
                    Id = Guid.Parse(pump.Identifier),
                    InUse = false
                };
                _pairedPumps.Add(pumpModel);
            }
        }

        private void CheckForUpdates()
        {
            BluetoothManager bluetoothManager = BluetoothManager.Instance;

            if (_firmware != null)
            {
                // Let the connected pump know
                foreach (PumpModel pump in _pairedPumps)
                {
                    if (pump.IsConnected)
                    {
                        // Check if the firmware is newer
                        if (_firmware.FirmwareVersion > bluetoothManager.Version)
                        {
                            pump.IsUpdateAvailable = true;
                            bluetoothManager.Firmware = _firmware.Binary;
                        }
                        else
                        {
                            pump.IsUpdateAvailable = false;
                            bluetoothManager.Firmware = null;
                        }
                    }
                }
            }
        }

        public async Task Sync()
        {
            DataManager dataManager = DataManager.Instance;

            // Check if new firmware exist for the pump
            await dataManager.SyncFirmware();

            try
            {
                _firmware = (await dataManager.Firmware.OrderByDescending(f => f.FirmwareVersion).ToEnumerableAsync()).FirstOrDefault();
            }
            catch (Exception ex)
            {

            }

            if (_firmware != null)
            {
                CheckForUpdates();

                FirmwareAvailableEvent?.Invoke(this, EventArgs.Empty);
            }
        }

        public Firmware Firmware
        {
            get
            {
                return _firmware;
            }
        }

        private StorageType _storageType = StorageType.Unspecified;

        private void _bluetoothManager_LogUpdated(object sender, EventArgs e)
        {
            if (_connectedPump != null)
            {
                _storageType = _connectedPump.Storage;
            }
            ConvertLogEntriesToPumpingHistory();
        }

        private void _bluetoothManager_PumpDisconnected(object sender, PumpDisconnectedEventArgs e)
        {
            if (_connectedPump != null)
            {
                _connectedPump.IsConnected = false;
                _connectedPump.InUse = false;
            }
            _connectedPump = null;
            PumpDisconnectedEvent?.Invoke(this, EventArgs.Empty);
        }

        private async void _bluetoothManager_PumpConnected(object sender, PumpConnectedEventArgs e)
        {
            _connectedPump = e.Pump;
            _connectedPump.IsConnected = true;
            _connectedPump.InUse = true;

            PumpModel proxyPump = _pairedPumps.Where(p => p.Id == e.Pump.Id).FirstOrDefault();

            // We have not saved this pump to our paired list yet
            if (proxyPump == null)
            {
                _pairedPumps.Add(_connectedPump);

                DataManager dataManager = DataManager.Instance;
                User user = (await dataManager.User.ToEnumerableAsync()).FirstOrDefault();

                if (user != null)
                {
                    Pump pump = new Pump()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = _connectedPump.Name,
                        Identifier = _connectedPump.Id.ToString(),
                        ProfileId = user.DefaultProfileId
                    };

                    await dataManager.AddUpdatePump(pump);
                }
            }
            else
            {
                // Replace the proxy pump
                _pairedPumps.Remove(proxyPump);
                _pairedPumps.Add(_connectedPump);
            }

            // Convert log entries to history entries
            ConvertLogEntriesToPumpingHistory();

            // Check if update is available
            CheckForUpdates();

            PumpConnectedEvent?.Invoke(this, new PumpConnectedEventArgs() { Pump = _connectedPump });
        }

        private void ConvertLogEntriesToPumpingHistory()
        {
            List<HistoryModel> pumpingHistory = new List<HistoryModel>();
            HistoryModel historyModel;
            DateTime sessionStartTimestamp = DateTime.MinValue;
            LogEntryModel sessionStart = null;
            long duration;

            // See if we had an existing session from the last logs
            if (Settings.SessionUptime != -1L)
            {
                if (Settings.SessionStart != string.Empty)
                {
                    sessionStartTimestamp = DateTime.Parse(Settings.SessionStart).ToUniversalTime();
                    Settings.SessionStart = string.Empty;
                    sessionStart = new LogEntryModel()
                    {
                        Timestamp = sessionStartTimestamp,
                        Uptime = (uint)Settings.SessionUptime
                    };
                }

                Settings.SessionUptime = -1L;
            }

            // If we find a session start and a session end, save the pump history. Otherwise we will sync once the session is complete
            foreach(LogEntryModel logEntry in _connectedPump.Logs)
            {
                if (logEntry.Type == LogEntryType.PumpSessionStart)
                {
                    if (sessionStart == null)
                    {
                        sessionStart = logEntry;
                        sessionStartTimestamp = logEntry.Timestamp;
                    }
                }
                else if (logEntry.Type == LogEntryType.PumpSessionEnd)
                {
                    if (sessionStart != null)
                    {
                        duration = (long)logEntry.Uptime - (long)sessionStart.Uptime;

                        // Sanity checking
                        if ((duration > 0) &&
                            (sessionStart.Timestamp != DateTime.MinValue) &&
                            (logEntry.Timestamp != DateTime.MinValue) &&
                            (logEntry.Timestamp > sessionStart.Timestamp))
                        {
                            historyModel = HistoryManager.Instance.CreateSession(SessionType.Pump);

                            historyModel.StartTime = DateTime.SpecifyKind(sessionStart.Timestamp, DateTimeKind.Utc);
                            historyModel.EndTime = DateTime.SpecifyKind(logEntry.Timestamp, DateTimeKind.Utc);
                            historyModel.LeftBreastStartTime = historyModel.StartTime;
                            historyModel.LeftBreastEndTime = historyModel.EndTime;
                            historyModel.RightBreastStartTime = historyModel.StartTime;
                            historyModel.RightBreastEndTime = historyModel.EndTime;
                            historyModel.LeftBreastMilkVolume = logEntry.LeftVolume;
                            historyModel.RightBreastMilkVolume = logEntry.RightVolume;
                            historyModel.TotalMilkVolume = logEntry.LeftVolume + logEntry.RightVolume;
                            historyModel.Storage = _storageType;

                            pumpingHistory.Add(historyModel);
                        }

                        if (Settings.SessionStart != string.Empty)
                        {
                            Settings.SessionStart = string.Empty;
                        }

                        sessionStart = null;
                    }
                }
            }

            if (sessionStart != null)
            {
                // If a pumping session is in progress, save the start time so we can recreate the full session
                // next time we pull the logs
                Settings.SessionStart = sessionStart.Timestamp.ToString("u");
                Settings.SessionUptime = (long)sessionStart.Uptime;
            }

            // Wipe the existing logs
            _bluetoothManager.ClearLogs();

            _connectedPump.PumpingSessions = pumpingHistory;
        }

        private void _bluetoothManager_PumpDiscovered(object sender, PumpDiscoveredEventArgs e)
        {
            // Do we know about this pump?
            PumpModel pump = _pairedPumps.Where(p => p.Id == e.Pump.Id).FirstOrDefault();

            if (pump != null)
            {
                // Set the name if this pump was found before
                e.Pump.Name = pump.Name;

                // If this is a pump we know about and have paired with, pair automatically
                if (!_bluetoothManager.IsPaired)
                {
                    _bluetoothManager.Connect(e.Pump);
                }
            }
            else
            {
                // Otherwise, add it as a new pump
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    if (!_newPumps.Contains(e.Pump))
                    {
                        _newPumps.Add(e.Pump);
                    }
                });
            }
        }

        List<Pump> _knownPumps;

        public void Start()
        {
            if (!_isStarted)
            {
                _bluetoothManager.Scan();
            }
        }

        public void Pair(PumpModel pump)
        {
            // TODO: Implement pairing
            if (_connectedPump == null)
            {
                // Remove the pump from the new and not pairde list
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    if (_newPumps.Contains(pump))
                    {
                        _newPumps.Remove(pump);
                    }
                });

                _bluetoothManager.Connect(pump);
            }
        }

        public void Remove(PumpModel pumpModel)
        {
            Pump pump = _knownPumps.Where(p => Guid.Parse(p.Identifier) == pumpModel.Id).FirstOrDefault();

            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                if (_pairedPumps.Contains(pumpModel))
                {
                    _pairedPumps.Remove(pumpModel);
                }
            });

            if (pump != null)
            {
                DataManager dataManager = DataManager.Instance;
                dataManager.DeletePump(pump);
                _knownPumps.Remove(pump);
            }

            _bluetoothManager.Disconnect();
        }

        public ObservableCollection<PumpModel> PairedPumps
        {
            get
            {
                return _pairedPumps;
            }
            internal set
            {
                 _pairedPumps = value;
            }
        }

        public ObservableCollection<PumpModel> NewPumps
        {
            get
            {
                return _newPumps;
            }
            internal set
            {
                _newPumps = value;
            }
        }

        public PumpModel ConnectedPump
        {
            get
            {
                return _connectedPump;
            }
            set
            {
                _connectedPump = value;
            }
        }

        public PumpModel SelectedPump
        {
            get
            {
                return _selectedPump;
            }
            set
            {
                _selectedPump = value;
            }
        }
    }
}
