using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Plugin.BLE;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using System.Collections.ObjectModel;

using BabyationApp.Models;
using System.Diagnostics;
using System.Threading;

namespace BabyationApp.Managers
{
    /// <summary>
    /// Event arguments sent when a new pump is discovered
    /// </summary>
    public class PumpDiscoveredEventArgs : EventArgs
    {
        public PumpModel Pump { get; internal set; }
    }

    /// <summary>
    /// Event arguments sent when a pump is connected
    /// </summary>
    public class PumpConnectedEventArgs : EventArgs
    {
        public PumpModel Pump { get; internal set; }
    }

    /// <summary>
    /// Event arguments sent when a pump is disconnected
    /// </summary>
    public class PumpDisconnectedEventArgs : EventArgs
    {
        public PumpModel Pump { get; internal set; }
    }

    /// <summary>
    /// The Bluetooth Manager
    /// </summary>
    public class BluetoothManager
    {
        static private BluetoothManager _instance = null;

        private IBluetoothLE _bluetoothLE = null;
        private bool _initialized = false;
        private PumpModel _connectingPump = null;
        private PumpModel _connectedPump = null;
        private Dictionary<Guid, PumpModel> _discoveredPumps;
        private Dictionary<Int32, float> _reservedValues;
        private Dictionary<Guid, Dictionary<Guid, ICharacteristic>> _characteristics;
        private bool _isScanningPaused = false;

        public event EventHandler<PumpConnectedEventArgs> PumpConnected;
        public event EventHandler<PumpDiscoveredEventArgs> PumpDiscovered;
        public event EventHandler<PumpDisconnectedEventArgs> PumpDisconnected;
        public event EventHandler LogUpdated;

        /// <summary>
        /// Get the singleton to the Bluetooth Manager
        /// </summary>
        /// <returns>The singleton to the Bluetooth Manager</returns>
        static public BluetoothManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new BluetoothManager();
                }

                return _instance;
            }
        }

        /// <summary>
        /// Set the environment specific BLE implementation
        /// </summary>
        public IBluetoothLE BluetoothLE
        {
            set
            {
                _bluetoothLE = value;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        private BluetoothManager()
        {
            _discoveredPumps = new Dictionary<Guid, PumpModel>();

            _reservedValues = new Dictionary<Int32, float> {
              { 0x07FE, float.PositiveInfinity },
              { 0x07FF, float.NaN },
              { 0x0800, float.NaN },
              { 0x0801, float.NaN },
              { 0x0802, float.NegativeInfinity }
            };

            _characteristics = new Dictionary<Guid, Dictionary<Guid, ICharacteristic>>();

            //_breastPumpControl = new byte[4];
            //ResetBreastPumpControl();
        }

        /// <summary>
        /// Get the current paired state
        /// </summary>
        /// <returns>The current paired state</returns>
        public bool IsPaired
        {
            get
            {
                return (_connectedPump != null) ? true : false;
            }
        }

        private async void _connectedPump_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // TODO: Add Desired Experience
            await Task.Run(() =>
            {
                if (_connectedPump != null)
                {
                    switch (e.PropertyName)
                    {
                        case "DesiredState":
                            DesiredState = _connectedPump.DesiredState;
                            break;
                        case "DesiredPumpingMode":
                            DesiredPumpingMode = _connectedPump.DesiredPumpingMode;
                            break;
                        case "DesiredSuction":
                            DesiredSuction = _connectedPump.DesiredSuction;
                            break;
                        case "DesiredSpeed":
                            DesiredSpeed = _connectedPump.DesiredSpeed;
                            break;
                        case "DesiredExperience":
                            DesiredExperience = _connectedPump.DesiredExperience;
                            break;
                        case "PresetExperiences":
                            PresetExperiences = _connectedPump.PresetExperiences;
                            break;
                        case "UserExperiences":
                            UserExperiences = _connectedPump.UserExperiences;
                            break;
                        default:
                            break;
                    }
                }
            });
        }

        /// <summary>
        /// Reset the state of the Bluetooth Manager
        /// </summary>
        private void Reset()
        {
            _desiredExperience = -1;
            _desiredSpeed = -1;
            _desiredSuction = -1;
            _mtuSize = -1;
            _isUpdatingFirmware = false;
            _isUpdateFirmwareInitialized = false;
            _blockSize = 0;
            _bytesSent = 0;
            _firmware = null;
            _version = -1;

            _characteristics.Clear();

            if (_connectedPump != null)
            {
                _connectedPump.PropertyChanged -= _connectedPump_PropertyChanged;
                _discoveredPumps.Remove(_connectedPump.Id);
            }

            _connectedPump = null;
            _connectingPump = null;
        }

        int _mtuSize = -1;
        bool _isUpdatingFirmware = false;
        bool _isUpdateFirmwareInitialized = false;
        const int OadDefaultBlockSize = 20;
        private int _blockSize = 0;
        private int _bytesSent = 0;
        private byte[] _firmware = null;
        private int _version = -1;

        /// <summary>
        /// Get the version number of the current firmware on the pump
        /// </summary>
        /// <returns>The version number of the current firmware</returns>
        public int Version
        {
            get
            {
                return _version;
            }
        }

        /// <summary>
        /// Set the firmware update byte array
        /// </summary>
        /// <returns>The firmware update byte array</returns>
        public byte[] Firmware
        {
            set
            {
                _firmware = value;
            }
        }

        /// <summary>
        /// Update the firmware on the device
        /// </summary>
        public async void UpdateFirmware()
        {
            if (!_isUpdatingFirmware && (_firmware != null))
            {
                _isUpdatingFirmware = true;
                _connectedPump.IsUpdating = true;

                await Task.Run(async () =>
                {
                    ICharacteristic characteristic;

                    if (!_isUpdateFirmwareInitialized)
                    {
                        characteristic = _characteristics[Service.OADService][Characteristic.ImageIdentity];
                        characteristic.WriteType = CharacteristicWriteType.WithoutResponse;
                        characteristic.ValueUpdated += OadCharacteristicUpdated;
                        await characteristic.StartUpdatesAsync();

                        characteristic = _characteristics[Service.OADService][Characteristic.ImageBlock];
                        characteristic.WriteType = CharacteristicWriteType.WithoutResponse;

                        // Negotiate a larger MTU
                        _mtuSize = await _connectedPump.Device.RequestMtuAsync(251);

                        Debug.WriteLine($"MTU Size Updated To: {_mtuSize}");

                        _isUpdateFirmwareInitialized = true;
                    }

                    _blockSize = OadDefaultBlockSize;

                    // Send 'Get Block Size' command
                    await OadSendCommand(OadCommandBlockSize);
                });
            }
       }

        const byte OadCommandBlockSize = 0x01;
        const byte OadCommandSetImageCount = 0x02;
        const byte OadCommandStart = 0x03;
        const byte OadCommandEnableImage = 0x04;
        const byte OadCommandGetSoftwareVersion = 0x07;
        const byte OadCommandImageBlockWriteResponse = 0x12;

        const byte OadSuccess = 0;
        const byte OadCRCError = 1;
        const byte OadFlashError = 2;
        const byte OadBufferOverflow = 3;
        const byte OadAlreadyStarted = 4;
        const byte OadNotStarted = 5;
        const byte OadDownloadNotComplete = 6;
        const byte OadNoResources = 7;
        const byte OadImageTooBig = 8;
        const byte OadIncompatibleImage = 9;
        const byte OadInvalidFile = 10;
        const byte OadIncompatibleFile = 11;
        const byte OadAuthorizationFailed = 12;
        const byte OadCommandNotSupported = 13;
        const byte OadDownloadComplete = 14;
        const byte OadNotificationsNotEnabled = 15;
        const byte OadImageIdentityTimedOut = 16;

        private readonly string[] OadStatusToString =
        {
            "Firmware Update Success",
            "Firmware Update CRC Error",
            "Firmware Update Flash Error",
            "Firmware Update Buffer Overflow Error",
            "Firmware Update Already Started Error",
            "Firmware Update Not Started Error",
            "Firmware Update Download Not Complete Error",
            "Firmware Update No Resources Error",
            "Firmware Update Image Too Big Error",
            "Firmware Update Incompatible Image Error",
            "Firmware Update Invalid File Error",
            "Firmware Update Incompatible File Error",
            "Firmware Update Authorization Failed Error",
            "Firmware Update Command Not Supported Error",
            "Firmware Update Download Complete",
            "Firmware Update Notifications Not Enabled Error",
            "Firmware Update Image Identity Timed Out Error",
        };

        /// <summary>
        /// Send an OAD command
        /// </summary>
        /// <param name="command">The command to send</param>
        private async Task OadSendCommand(byte command)
        {
            byte[] value = null;

            switch (command)
            {
                case OadCommandBlockSize:
                    Debug.WriteLine($"Sending Block Size Command");
                    value = new byte[1];
                    value[0] = command;
                    break;
                case OadCommandStart:
                    Debug.WriteLine($"Sending Start Command");
                    value = new byte[1];
                    value[0] = command;                    
                    break;
                case OadCommandGetSoftwareVersion:
                    Debug.WriteLine($"Sending Get Software Version Command");
                    value = new byte[1];
                    value[0] = command;
                    break;
                case OadCommandEnableImage:
                    Debug.WriteLine($"Sending Enable Image Command");
                    value = new byte[1];
                    value[0] = command;
                    break;
                default:
                    break;
            }

            if (value != null)
            {
                await WriteCharacteristic(Service.OADService, Characteristic.OADControl, value);
            }
        }

        /// <summary>
        /// Send an OAD image identity to the device
        /// </summary>
        private async Task OadSendImageIdentity()
        {
            byte[] value = new byte[22];
            int offset = 0;

            if (_firmware != null)
            {
                // OAD Image Identifiaction Value
                Array.Copy(_firmware, offset, value, 0, 8);
                offset += 8;

                // BIM Version
                value[offset] = _firmware[12];
                offset++;

                // Image Header Version
                value[offset] = _firmware[13];
                offset++;

                // Image Header Information
                value[offset] = _firmware[16];
                offset++;
                value[offset] = _firmware[17];
                offset++;
                value[offset] = _firmware[18];
                offset++;
                value[offset] = _firmware[19];
                offset++;

                //Image Length
                Array.Copy(_firmware, 24, value, offset, 4);
                offset += 4;

                // Software Version
                Array.Copy(_firmware, 28, value, offset, 4);
                offset += 4;

                await WriteCharacteristic(Service.OADService, Characteristic.ImageIdentity, value);
            }
        }

        private async Task OadSendBlock(uint blockNumber)
        {
            byte[] value;
            int offset;
            int payloadSize;
            int updatePercent;

            if (_firmware != null)
            {
                // Remove bytes needed for header
                payloadSize = _blockSize - 4;
                offset = (int)(blockNumber * payloadSize);

                if (offset + payloadSize > _firmware.Length)
                {
                    payloadSize = _firmware.Length - offset;
                }

                value = new byte[payloadSize + 4];

                SetUint32(blockNumber, value, 0);

                Array.Copy(_firmware, offset, value, 4, payloadSize);

                await WriteCharacteristic(Service.OADService, Characteristic.ImageBlock, value);

                _bytesSent += payloadSize;
                updatePercent = ((_bytesSent * 100) / _firmware.Length);

                Debug.WriteLine($"Update percent: {updatePercent}%");

                _connectedPump.UpdatePercent = updatePercent;
            }
        }

        private async void OadCharacteristicUpdated(object sender, Plugin.BLE.Abstractions.EventArgs.CharacteristicUpdatedEventArgs e)
        {
            uint blockNumber;

            await Task.Run(async () =>
            {
                if (e.Characteristic.Id == Characteristic.ImageIdentity)
                {
                    byte status = e.Characteristic.Value[0];

                    Debug.WriteLine($"Image Identity Response: {status}");

                    if (status == OadSuccess)
                    {
                        await OadSendCommand(OadCommandStart);
                    }
                }
                else if (e.Characteristic.Id == Characteristic.OADControl)
                {
                    byte command = e.Characteristic.Value[0];

                    if (command == OadCommandBlockSize)
                    {
                        _blockSize = (int)GetUint16(e.Characteristic.Value, 1);

                        Debug.WriteLine($"Block Size Updated To: {_blockSize}");

                        // Start writing blocks
                        await OadSendImageIdentity();
                    }
                    else if (command == OadCommandEnableImage)
                    {
                        byte status = e.Characteristic.Value[1];

                        if (status == OadSuccess)
                        {
                            Debug.WriteLine($"Update complete...");
                        }
                    }
                    else if (command == OadCommandGetSoftwareVersion)
                    {
                        // Just use the app version
                        _version = (int)GetUint16(e.Characteristic.Value, 3);

                        Debug.WriteLine($"Software Version: {_version}");
                    }
                    else if (command == OadCommandImageBlockWriteResponse)
                    {
                        byte status = e.Characteristic.Value[1];

                        if (status == OadSuccess)
                        {
                            blockNumber = GetUint32(e.Characteristic.Value, 2);

                            Debug.WriteLine($"Block Write Response: {blockNumber}");

                            // Send next block
                            await OadSendBlock(blockNumber);
                        }
                        else if (status == OadDownloadComplete)
                        {
                            Debug.WriteLine($"Download Complete");
                            _connectedPump.IsUpdating = false;

                            // Enable the completed image
                            await OadSendCommand(OadCommandEnableImage);
                        }
                        else
                        {
                            string statusString;

                            if (status <= OadImageIdentityTimedOut)
                            {
                                statusString = OadStatusToString[status];
                            }
                            else
                            {
                                statusString = "Unknown Status";
                            }

                            // Add it to the device alert list
                            _connectedPump.AlertMessages.Add(statusString);
                            _connectedPump.IsUpdating = false;

                            Debug.WriteLine(statusString);
                        }
                    }
                }
            });
        }

        public void Initialize()
        {
            if (!_initialized)
            {
                IAdapter adapter = _bluetoothLE.Adapter;

                _bluetoothLE.StateChanged += _bluetoothLE_StateChanged;

                adapter.DeviceDiscovered += Adapter_DeviceDiscovered;
                adapter.DeviceConnected += Adapter_DeviceConnected;
                adapter.DeviceDisconnected += Adapter_DeviceDisconnected;
                adapter.DeviceConnectionLost += Adapter_DeviceConnectionLost;
                adapter.ScanTimeoutElapsed += Adapter_ScanTimeoutElapsed;

                _initialized = true;
            }
        }

        private byte[] _breastPumpHistoryControl = new byte[] { 0x01 };
        public async void ClearLogs()
        {
            _logEntries.Clear();
            await WriteCharacteristic(Service.BreastPumpHistory, Characteristic.BreastPumpLogControl, _breastPumpHistoryControl);
            UpdateCurrentTime();
            await WriteCharacteristic(Service.CurrentTimeService, Characteristic.CurrentTime, _currentTime);
        }

        private void Adapter_ScanTimeoutElapsed(object sender, EventArgs e)
        {
            if (!IsPaired)
            {
                Scan();
            }
        }

        private void ForceDisconnect()
        {
            PumpModel disconnectedDevice = _connectedPump;

            Debug.WriteLine($"Forcing disconnect,  id: {disconnectedDevice.Device.Id}");

            // Cleanly disconnect
            Disconnect();

            //Clean up
            Reset();

            PumpDisconnected?.Invoke(this, new PumpDisconnectedEventArgs() { Pump = disconnectedDevice });
        }

        private void Adapter_DeviceConnectionLost(object sender, Plugin.BLE.Abstractions.EventArgs.DeviceErrorEventArgs e)
        {
            PumpModel disconnectedDevice = _connectedPump;

            Debug.WriteLine($"Pump connection lost, id: {e.Device.Id}");

            // Cleanly disconnect
            Disconnect();

            //Clean up
            Reset();

            PumpDisconnected?.Invoke(this, new PumpDisconnectedEventArgs() { Pump = disconnectedDevice });

            // Begin scannning for devices again
            Scan();
        }

        private void Adapter_DeviceDisconnected(object sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs e)
        {
            PumpModel disconnectedDevice = _connectedPump;

            Debug.WriteLine($"Pump disconnected, id: {e.Device.Id}");

            // Cleanly disconnect
            Disconnect();

            Reset();

            PumpDisconnected?.Invoke(this, new PumpDisconnectedEventArgs() { Pump = disconnectedDevice });

            // Begin scannning for devices again
            Scan();
        }

        private async void Adapter_DeviceConnected(object sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs e)
        {
            IDevice device;
            byte[] value;
            bool logsCorrupted = false;

            // TODO: Move this to a background thread
            try
            {
                if (_connectingPump != null)
                {
                    device = e.Device;
                    _connectedPump = _connectingPump;

                    IList<IService> services = await device.GetServicesAsync();

                    foreach (IService service in services)
                    {
                        IList<ICharacteristic> characteristics = await service.GetCharacteristicsAsync();
                        _characteristics.Add(service.Id, new Dictionary<Guid, ICharacteristic>());

                        foreach (ICharacteristic characteristic in characteristics)
                        {
                            try
                            {
                                _characteristics[service.Id].Add(characteristic.Id, characteristic);
                            }
                            catch
                            {

                            }

                            if (characteristic.Id != null)
                            {
                                if (characteristic.Id == Characteristic.ManufacturerNameString)
                                {
                                    value = await characteristic.ReadAsync();
                                }
                                if (characteristic.Id == Characteristic.BatteryLevel)
                                {
                                    // Seed initial value
                                    value = await characteristic.ReadAsync();
                                    UpdateBatteryLevel(value);

                                    // Register for notifies
                                    characteristic.ValueUpdated += Characteristic_ValueUpdated;
                                    await characteristic.StartUpdatesAsync();
                                }
                                else if (characteristic.Id == Characteristic.BreastPumpStatus)
                                {
                                    // Seed initial value
                                    value = await characteristic.ReadAsync();
                                    UpdateStatus(value);

                                    // Register for notifies
                                    characteristic.ValueUpdated += Characteristic_ValueUpdated;
                                    await characteristic.StartUpdatesAsync();
                                }
                                else if (characteristic.Id == Characteristic.BreastPumpMilkLevel)
                                {
                                    // Seed initial value
                                    value = await characteristic.ReadAsync();
                                    UpdateBreastPumpMilkLevel(value);

                                    // Register for notifies
                                    characteristic.ValueUpdated += Characteristic_ValueUpdated;
                                    await characteristic.StartUpdatesAsync();
                                }
                                else if (characteristic.Id == Characteristic.BreastPumpLog)
                                {
                                    LogEntryModel logEntryModel;

                                    _timeSetEntry = null;

                                    // Pull logs
                                    do
                                    {
                                        value = await _characteristics[Service.BreastPumpHistory][Characteristic.BreastPumpLog].ReadAsync();

                                        if (value.Length > 0)
                                        {
                                            // Parse it
                                            logEntryModel = ParseLogEntry(value);

                                            if (logEntryModel != null)
                                            {
                                                // Special entry, save it off to recreate other timestamps
                                                if (logEntryModel.Type == LogEntryType.TimeSet)
                                                {
                                                    if (_timeSetEntry == null)
                                                    {
                                                        _timeSetEntry = logEntryModel;
                                                    }
                                                }
                                                else
                                                {
                                                    // Save off the entry
                                                    _logEntries.Add(logEntryModel);
                                                }
                                            }


                                        }
                                    } while (value.Length > 0);

                                    // Fix all the timestamps
                                    if (_timeSetEntry != null)
                                    {
                                        try
                                        {
                                            foreach (LogEntryModel logEntry in _logEntries)
                                            {
                                                logEntry.DetermineDateTime(_timeSetEntry.Uptime, _timeSetEntry.UtcTime);
                                            }

                                            _connectedPump.Logs = _logEntries;
                                        }
                                        catch
                                        {
                                            logsCorrupted = true;
                                        }
                                    }     
                                }
                                else if (characteristic.Id == Characteristic.BreastPumpLogStatus)
                                {
                                    // Register for notifies
                                    characteristic.ValueUpdated += Characteristic_ValueUpdated;
                                    await characteristic.StartUpdatesAsync();
                                }
                                else if (characteristic.Id == Characteristic.CurrentTime)
                                {
                                    UpdateCurrentTime();
                                    await characteristic.WriteAsync(_currentTime);
                                }
                                else if (characteristic.Id == Characteristic.BreastPumpPresetExperiences)
                                {
                                    ExperienceModel experienceModel;

                                    do
                                    {
                                        value = await _characteristics[Service.BreastPumpStatus][Characteristic.BreastPumpPresetExperiences].ReadAsync();

                                        if (value.Length > 0)
                                        {
                                            // Parse it
                                            experienceModel = ParseExperience(value);

                                            if (experienceModel != null)
                                            {
                                                if (!_presetExperiences.ContainsKey(experienceModel.ExperienceId))
                                                {
                                                    _presetExperiences.Add(experienceModel.ExperienceId, experienceModel);
                                                }
                                            }
                                        }
                                    } while (value.Length > 0);
                                }
                                else if (characteristic.Id == Characteristic.BreastPumpUserExperiences)
                                {
                                    ExperienceModel experienceModel;

                                    do
                                    {
                                        value = await _characteristics[Service.BreastPumpStatus][Characteristic.BreastPumpUserExperiences].ReadAsync();

                                        if (value.Length > 0)
                                        {
                                            // Parse it
                                            experienceModel = ParseExperience(value);

                                            if (experienceModel != null)
                                            {
                                                if (!_userExperiences.ContainsKey(experienceModel.ExperienceId))
                                                {
                                                    _userExperiences.Add(experienceModel.ExperienceId, experienceModel);
                                                }
                                            }
                                        }
                                    } while (value.Length > 0);
                                }
                                else if (characteristic.Id == Characteristic.OADControl)
                                {
                                    characteristic.WriteType = CharacteristicWriteType.WithoutResponse;
                                    characteristic.ValueUpdated += OadCharacteristicUpdated;
                                    await characteristic.StartUpdatesAsync();

                                    // Request the softare version
                                    await OadSendCommand(OadCommandGetSoftwareVersion);
                                }
                            }
                        }
                    }

                    if (logsCorrupted)
                    {
                        ClearLogs();
                    }

                    _connectedPump.PropertyChanged += _connectedPump_PropertyChanged;
                    _connectingPump = null;
                    PumpConnected?.Invoke(this, new PumpConnectedEventArgs() { Pump = _connectedPump });

                    _sync = true;
                }
            }
            catch (Exception ex)
            {
                Disconnect();
                Reset();
                Scan();
            }
        }

        private byte[] _currentTime = new byte[9];
        private void UpdateCurrentTime()
        {
            DateTime now = DateTime.UtcNow;
            SetUint16((ushort)now.Year, _currentTime, 0);
            _currentTime[2] = (byte)now.Month;
            _currentTime[3] = (byte)now.Day;
            _currentTime[4] = (byte)now.Hour;
            _currentTime[5] = (byte)now.Minute;
            _currentTime[6] = (byte)now.Second;
            _currentTime[7] = (byte)now.DayOfWeek;
            _currentTime[8] = (byte)0;
        }

        private async Task SyncLogs()
        {
            byte[] value;
            LogEntryModel logEntryModel;

            _timeSetEntry = null;

            // Pull logs
            do
            {
                //value = await _characteristics[Service.BreastPumpHistory][Characteristic.BreastPumpLog].ReadAsync();
                value = await ReadCharacteristic(Service.BreastPumpHistory, Characteristic.BreastPumpLog);

                if (value != null)
                {
                    if (value.Length > 0)
                    {
                        // Parse it
                        logEntryModel = ParseLogEntry(value);

                        if (logEntryModel != null)
                        {
                            // Special entry, save it off to recreate other timestamps
                            if (logEntryModel.Type == LogEntryType.TimeSet)
                            {
                                if (_timeSetEntry == null)
                                {
                                    _timeSetEntry = logEntryModel;
                                }
                            }
                            else
                            {
                                // Save off the entry
                                _logEntries.Add(logEntryModel);
                            }
                        }


                    }
                }
            } while ((value != null) && (value.Length > 0));

            // Fix all the timestamps
            if (_timeSetEntry != null)
            {
                foreach (LogEntryModel logEntry in _logEntries)
                {
                    logEntry.DetermineDateTime(_timeSetEntry.Uptime, _timeSetEntry.UtcTime);
                }
            }
        }

        private bool _sync = false;

        private void Adapter_DeviceDiscovered(object sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs e)
        {
            EventHandler<PumpDiscoveredEventArgs> pumpDiscovered = PumpDiscovered;
            PumpModel pump;

            if ((e.Device.Name != null) && (e.Device.Name.ToLower().Contains("babyation")))
            {
                if (!_discoveredPumps.ContainsKey(e.Device.Id))
                {
                    Debug.WriteLine($"Pump found, id: {e.Device.Id}");

                    pump = new PumpModel()
                    {
                        Id = e.Device.Id,
                        AdvertisedName = e.Device.Name,
                        Device = e.Device
                    };

                    _discoveredPumps.Add(e.Device.Id, pump);
                }
                else
                {
                    pump = _discoveredPumps[e.Device.Id];
                }

                if (pumpDiscovered != null)
                {
                    PumpDiscoveredEventArgs args = new PumpDiscoveredEventArgs()
                    {
                        Pump = pump
                    };
                    pumpDiscovered(this, args);
                }
            }
        }

        private void _bluetoothLE_StateChanged(object sender, Plugin.BLE.Abstractions.EventArgs.BluetoothStateChangedArgs e)
        {
            Debug.WriteLine($"The bluetooth state changed to {e.NewState}");
        }

#region NumericConversions
        private float ToSFloat(byte[] value, int start)
        {
            byte b0 = value[start];
            byte b1 = value[start + 1];

            var mantissa = unsignedToSigned(ToInt(b1) + ((ToInt(b0) & 0x0F) << 8), 12);
            var exponent = unsignedToSigned(ToInt(b0) >> 4, 4);

            if (_reservedValues.ContainsKey(mantissa))
                return _reservedValues[mantissa];

            return (float)(mantissa * Math.Pow(10d, exponent));
        }

        public int ToInt(byte value)
        {
            return value & 0xFF;
        }

        private int unsignedToSigned(int unsigned, int size)
        {
            if ((unsigned & (1 << size - 1)) != 0)
            {
                unsigned = -1 * ((1 << size - 1) - (unsigned & ((1 << size - 1) - 1)));
            }
            return unsigned;
        }

        public Int16 ToSInt(byte[] value, int offset)
        {
            byte b0 = value[offset + 1];
            byte b1 = value[offset];
            return (Int16)(b1 | (b0 << 8));
        }

        private UInt16 SwapEndianness(UInt16 value)
        {
            UInt16 b0 = (byte)(value & 0xFF);
            UInt16 b1 = (byte)((value & 0xFF00) >> 8);
            return (UInt16)(b1 | (b0 << 8));
        }

        private Int16 SwapEndianness(Int16 value)
        {
            UInt16 b0 = (byte)(value & 0xFF);
            UInt16 b1 = (byte)((value & 0xFF00) >> 8);
            return (Int16)(b1 | (b0 << 8));
        }

        private Int32 SwapEndianness(Int32 value)
        {
            Int32 b0 = (byte)(value & 0x000000FF);
            Int32 b1 = (byte)((value & 0x0000FF00) >> 8);
            Int32 b2 = (byte)((value & 0x00FF0000) >> 16);
            Int32 b3 = (byte)((value & 0xFF000000) >> 24);
            return (b3 | (b2 << 8) | (b1 << 16) | (b0 << 24));
        }

        private UInt32 SwapEndianness(UInt32 value)
        {
            UInt32 b0 = (byte)(value & 0x000000FF);
            UInt32 b1 = (byte)((value & 0x0000FF00) >> 8);
            UInt32 b2 = (byte)((value & 0x00FF0000) >> 16);
            UInt32 b3 = (byte)((value & 0xFF000000) >> 24);
            return (b3 | (b2 << 8) | (b1 << 16) | (b0 << 24));
        }

        private UInt16 GetUint16(byte[] array, int offset)
        {
            UInt16 temp;

            temp = (UInt16)(((int)array[offset + 1] << 8) | ((int)array[offset]));

            return temp;
        }

        private UInt32 GetUint32(byte[] array, int offset)
        {
            UInt32 temp;

            temp = (UInt32)(((int)array[offset + 3] << 24) | ((int)array[offset + 2] << 16) | ((int)array[offset + 1] << 8) | ((int)array[offset]));

            return temp;
        }

        private void SetUint16(UInt16 value, byte[] array, int offset)
        {
            array[offset] = (byte)((value >> 8) & 0xFF);
            array[offset + 1] = (byte)(value & 0xFF);
        }

        private void SetUint16(UInt16 value, List<byte> array)
        {
            array.Add((byte)((value >> 8) & 0xFF));
            array.Add((byte)(value & 0xFF));
        }

        private void SetUint32(UInt32 value, byte[] array, int offset)
        {
            array[offset + 3] = (byte)((value >> 24) & 0xFF);
            array[offset + 2] = (byte)((value >> 16) & 0xFF);
            array[offset + 1] = (byte)((value >> 8) & 0xFF);
            array[offset] = (byte)(value & 0xFF);
        }
#endregion

        private void UpdateBatteryLevel(byte[] value)
        {
            int batteryLevel = (int)value[0];

            if (_connectedPump != null)
            {
                _connectedPump.BatteryLevel = batteryLevel;
            }
        }

        private void UpdateBreastPumpMilkLevel(byte[] value)
        {
            byte flags = value[0];
            int offset = 1;
            float leftBreastMilkLevel = 0;
            float rightBreastMilkLevel = 0;

            if ((flags & 0x02) == 0x02)
            {
                // Left breast level included
                leftBreastMilkLevel = ToSFloat(value, offset);
                offset += 2;
            }

            if ((flags & 0x04) == 0x04)
            {
                // Right breast level included
                rightBreastMilkLevel = ToSFloat(value, offset);
            }

            if (_connectedPump != null)
            {
                if ((flags & 0x02) == 0x02)
                {
                    _connectedPump.LeftBreastMilkLevel = leftBreastMilkLevel;
                }

                if ((flags & 0x04) == 0x04)
                {
                    _connectedPump.RightBreastMilkLevel = rightBreastMilkLevel;
                }
            }
        }

        private void UpdateStatus(byte[] value)
        {
            PumpState actualState = (PumpState)(((int)value[0] >> 0) & 0x07);
            bool chargeState = ((((int)value[0] >> 3) & 0x01) == 0x01);
            //PumpStatus status = (PumpStatus)(((int)value[0] >> 2) & 0x03);
            PumpMode actualPumpingMode = ((((int)value[0] >> 4) & 0x01) == 0x01) ? PumpMode.Stimulation : PumpMode.Expression;

            int currentExperience = (int)value[1];
            int actualSuction = (int)value[2] / 10;
            int actualSpeed = (int)value[3] / 10;
            TimeSpan currentDuration = new TimeSpan(0, 0, SwapEndianness(BitConverter.ToUInt16(value, 4)));

            if (_connectedPump != null)
            {
                if ((actualState == PumpState.End) || (actualState == PumpState.Error) || (actualState == PumpState.Stop))
                {
                    _desiredState = PumpState.Stop;
                    _connectedPump.DesiredState = PumpState.Stop;
                }

                _connectedPump.ActualState = actualState;
                _connectedPump.ChargeState = chargeState;
                _connectedPump.ActualPumpingMode = actualPumpingMode;
                _connectedPump.ActualSuction = actualSuction;
                _connectedPump.ActualSpeed = actualSpeed;
                _connectedPump.CurrentDuration = currentDuration;
            }
        }

        private async void UpdateBreastPumpLogStatus(byte[] value)
        {
            // New log entries exist
            await SyncLogs();
            LogUpdated?.Invoke(this, EventArgs.Empty);
        }

        private void Characteristic_ValueUpdated(object sender, Plugin.BLE.Abstractions.EventArgs.CharacteristicUpdatedEventArgs e)
        {
            if (e.Characteristic.Id == Characteristic.BatteryLevel)
            {
                UpdateBatteryLevel(e.Characteristic.Value);
            }
            else if (e.Characteristic.Id == Characteristic.BreastPumpStatus)
            {
                UpdateStatus(e.Characteristic.Value);
            }
            else if (e.Characteristic.Id == Characteristic.BreastPumpMilkLevel)
            {
                UpdateBreastPumpMilkLevel(e.Characteristic.Value);
            }
            else if (e.Characteristic.Id == Characteristic.BreastPumpLogStatus)
            {
                UpdateBreastPumpLogStatus(e.Characteristic.Value);
            }
        }

        public class Service
        {
            public static Guid CurrentTimeService = Guid.Parse("00001805-0000-1000-8000-00805F9B34FB");
            public static Guid DeviceInformation = Guid.Parse("0000180A-0000-1000-8000-00805F9B34FB");
            public static Guid BatteryService = Guid.Parse("0000180F-0000-1000-8000-00805F9B34FB");
            public static Guid BreastPumpMilkLevel = Guid.Parse("752B4056-EEC6-4BD0-BC55-BFA85317F3A4");
            public static Guid BreastPumpHistory = Guid.Parse("E38B90D5-32B5-42BC-880C-5158C736E460");
            public static Guid BreastPumpStatus = Guid.Parse("E38B1D6F-32B5-42BC-880C-5158C736E460");
            public static Guid OADService = Guid.Parse("F000FFC0-0451-4000-B000-000000000000");                                                                                
        }

        public class Characteristic
        {
            // Current Time Service
            public static Guid CurrentTime = Guid.Parse("00002A2B-0000-1000-8000-00805F9B34FB");
            public static Guid LocalTimeInformation = Guid.Parse("00002A0F-0000-1000-8000-00805F9B34FB");

            // Device Information
            public static Guid ManufacturerNameString = Guid.Parse("00002A29-0000-1000-8000-00805F9B34FB");

            // Battery Service
            public static Guid BatteryLevel = Guid.Parse("00002A19-0000-1000-8000-00805F9B34FB");

            // Breast Pump Status Service
            public static Guid BreastPumpControl = Guid.Parse("FB4B13AF-3C53-462B-953B-864F321D0796");
            public static Guid BreastPumpUserExperiencesControl = Guid.Parse("781489F8-8D27-4DDC-BF97-A6F941E36684");
            public static Guid BreastPumpUserExperiencesStatus = Guid.Parse("CCBD3B3F-4A65-40B9-8940-10EE45CA512D");
            public static Guid BreastPumpStatus = Guid.Parse("DA224A48-4E52-4758-88CF-E6B72657347E");
            public static Guid BreastPumpTest = Guid.Parse  ("781489F8-8D27-4DDC-BF97-A6F941E36684");
            public static Guid BreastPumpUserExperiences = Guid.Parse("5A596ED4-C09D-449E-AF12-9225303F0E7B");
            public static Guid BreastPumpPresetExperiences = Guid.Parse("AE3F2D66-CDFA-4D23-9FB1-B107AA852A0E");
            public static Guid BreastPumpPresetExperiencesStatus = Guid.Parse("8FBF564F-DF98-41CD-AA0A-F500A937024D");

            // Breast Pump Milk Level Service
            public static Guid BreastPumpMilkLevel = Guid.Parse("6EBB6B67-5D9C-44BD-93C8-58809481999C");

            // Breast Pump History Service
            public static Guid BreastPumpLogStatus = Guid.Parse("65D6CE40-BE8D-11E4-852A-0002A5D5C51B");
            public static Guid BreastPumpLog = Guid.Parse("0036E03C-0178-4315-AB14-3FE640C7C5CE");
            public static Guid BreastPumpLogControl = Guid.Parse("E38B8D9D-32B5-42BC-880C-5158C736E460");

            // OAD Service
            public static Guid ImageIdentity = Guid.Parse("F000FFC1-0451-4000-B000-000000000000");
            public static Guid ImageBlock = Guid.Parse("F000FFC2-0451-4000-B000-000000000000");
            public static Guid OADControl = Guid.Parse("F000FFC5-0451-4000-B000-000000000000");
        }

        private async Task WriteCharacteristic(Guid service, Guid characteristic, byte[] data)
        {
            int retry = 5;

            while (retry > 0)
            {
                try
                {
                    await _characteristics[service][characteristic].WriteAsync(data);
                    break;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"WriteCharacteristic, exception: {ex.Message}");
                    await Task.Delay(100);
                }
                retry--;
            }
        }

        private async Task<byte[]> ReadCharacteristic(Guid service, Guid characteristic)
        {
            int retry = 5;
            byte[] value = null;

            while (retry > 0)
            {
                try
                {
                    value = await _characteristics[service][characteristic].ReadAsync();
                    break;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"ReadCharacteristic, exception: {ex.Message}");
                    await Task.Delay(100);
                }
                retry--;
            }

            return value;
        }

        private Dictionary<int, ExperienceModel> _presetExperiences = new Dictionary<int, ExperienceModel>();
        private Dictionary<int, ExperienceModel> _userExperiences = new Dictionary<int, ExperienceModel>();
        private List<LogEntryModel> _logEntries = new List<LogEntryModel>();

        private ExperienceModel ParseExperience(byte[] value)
        {
            ExperienceModel experienceModel = null;
            int offset = 0;
            int nameLength;

            try
            {
                // Parse it
                experienceModel = new ExperienceModel();

                offset = 0;
                experienceModel.ExperienceId = value[offset++];

                nameLength = value[offset++];
                experienceModel.Name = Encoding.UTF8.GetString(value, offset, nameLength);
                offset += nameLength;

                experienceModel.TransitionType = (value[offset++] == 0) ? TransitionType.Letdown : TransitionType.Timed;
                experienceModel.StimulationSpeed = value[offset++];
                experienceModel.StimulationSuction = value[offset++];
                experienceModel.ExpressionSpeed = value[offset++];
                experienceModel.ExpressionSuction = value[offset++];
                experienceModel.Duration = new TimeSpan(0, 0, SwapEndianness(BitConverter.ToUInt16(value, offset)));
                offset += 2;

                if (experienceModel.TransitionType == TransitionType.Timed)
                {
                    experienceModel.TransitionTime = new TimeSpan(0, 0, SwapEndianness(BitConverter.ToUInt16(value, offset)));
                }
            }
            catch (Exception ex)
            {
                // If device does not send the proper payload length here parsing can fail. For now
                // catch it and move to the next experience
            }

            return experienceModel;
        }        

        private bool ExperienceChanged(ExperienceModel a, ExperienceModel b)
        {
            bool changed = false;

            if ((a.ExperienceId != b.ExperienceId) ||
                (a.TransitionType != b.TransitionType) ||
                (a.StimulationSpeed != b.StimulationSpeed) ||
                (a.StimulationSuction != b.StimulationSuction) ||
                (a.ExpressionSpeed != b.ExpressionSpeed) ||
                (a.ExpressionSuction != b.ExpressionSuction) ||
                (a.Duration != b.Duration) ||
                (a.TransitionTime != b.TransitionTime))
            {
                changed = true;
            }

            return changed;
        }

        private byte[] PackExperience(ExperienceModel experienceModel)
        {
            List<byte> packedExperience = new List<byte>();
            int nameLength;

            // Add id
            packedExperience.Add((byte)experienceModel.ExperienceId);

            // For now, don't send the name, it is not used and wastes bandwidth
            nameLength = 0;
            packedExperience.Add((byte)nameLength);

            // Flags
            if (experienceModel.TransitionType == TransitionType.Timed)
            {
                packedExperience.Add(1);
            }
            else
            {
                packedExperience.Add(0);
            }

            // Add simulation speed
            packedExperience.Add((byte)experienceModel.StimulationSpeed);

            // Add simulation suction
            packedExperience.Add((byte)experienceModel.StimulationSuction);

            // Add expression speed
            packedExperience.Add((byte)experienceModel.ExpressionSpeed);

            // Add expression suction
            packedExperience.Add((byte)experienceModel.ExpressionSuction);

            // Add duration
            SetUint16((ushort)experienceModel.Duration.TotalSeconds, packedExperience);

            // Add transtion time (if required)
            if (experienceModel.TransitionType == TransitionType.Timed)
            {
                SetUint16((ushort)experienceModel.TransitionTime.TotalSeconds, packedExperience);
            }

            return packedExperience.ToArray();
        }

        public IEnumerable<ExperienceModel> PresetExperiences
        {      
            get
            {
                return _presetExperiences.Values;
            }
            set
            {
                ExperienceModel tempExperience;
                byte[] packedExperience;

                // See if anything changed
                foreach (ExperienceModel experience in value)
                {
                    if (_presetExperiences.ContainsKey(experience.ExperienceId))
                    {
                        tempExperience = _presetExperiences[experience.ExperienceId];

                        if (ExperienceChanged(experience, tempExperience))
                        {
                            packedExperience = PackExperience(experience);

                            UpdateCharacteristic(Service.BreastPumpStatus, Characteristic.BreastPumpPresetExperiences, packedExperience);
                        }
                    }
                    else
                    {
                        // Current firmware allows 10 preset experiences to be stored
                        if (experience.ExperienceId < 128 + 10)
                        {
                            _presetExperiences.Add(experience.ExperienceId, experience);
                            packedExperience = PackExperience(experience);
                            UpdateCharacteristic(Service.BreastPumpStatus, Characteristic.BreastPumpPresetExperiences, packedExperience);
                        }
                    }
                }
            }
        }

        public IEnumerable<ExperienceModel> UserExperiences
        {
            get
            {
                return _userExperiences.Values;
            }
            set
            {
                ExperienceModel tempExperience;
                byte[] packedExperience;

                // See if anything changed
                foreach (ExperienceModel experience in value)
                {
                    if (_userExperiences.ContainsKey(experience.ExperienceId))
                    {
                        tempExperience = _userExperiences[experience.ExperienceId];

                        if (ExperienceChanged(experience, tempExperience))
                        {
                            tempExperience.Copy(experience);
                            packedExperience = PackExperience(experience);
                            UpdateCharacteristic(Service.BreastPumpStatus, Characteristic.BreastPumpUserExperiences, packedExperience);
                        }
                    }
                    else
                    {
                        // Current firmware allows 10 user experiences to be stored
                        if (experience.ExperienceId < 10)
                        {
                            _userExperiences.Add(experience.ExperienceId, experience);
                            packedExperience = PackExperience(experience);
                            UpdateCharacteristic(Service.BreastPumpStatus, Characteristic.BreastPumpUserExperiences, packedExperience);
                        }
                    }
                }
            }
        }

        private LogEntryModel _timeSetEntry = null;

        private int YearLength(int year)
        {
            int yearLength = 365;

            if (DateTime.IsLeapYear(year))
            {
                yearLength = 366;
            }

            return yearLength;
        }

        private int MonthLength(bool isLeapYear, int month)
        {
            int days = 31;

            if (month == 1) // feb
            {
                days = 28;

                if (isLeapYear)
                {
                    days++;
                }
            }
            else
            {
                if (month > 6) // aug-dec
                {
                    month--;
                }

                if ((month & 1) != 0)
                {
                    days = 30;
                }
            }

            return days;
        }

        private DateTime ConvertUtcTime(uint utcTime)
        {
            DateTime dateTime;

            dateTime = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(utcTime);

            return dateTime;
        }

        private LogEntryModel ParseLogEntry(byte[] value)
        {
            LogEntryModel logEntryModel = null;
            int offset = 0;
            uint utcTime;
            int size;

            try
            {
                // Parse it
                logEntryModel = new LogEntryModel();

                logEntryModel.Uptime = SwapEndianness(BitConverter.ToUInt32(value, offset));
                offset += 4;

                logEntryModel.Type = (LogEntryType)value[offset++];

                // Use for sanity checking
                size = value[offset++];

                switch (logEntryModel.Type)
                {
                    case LogEntryType.PumpSessionStart:
                        logEntryModel.Mode = value[offset++];
                        break;
                    case LogEntryType.PumpSessionEnd:
                        logEntryModel.LeftVolume = (double)SwapEndianness(BitConverter.ToUInt16(value, offset)) / 10;
                        offset += 2;
                        logEntryModel.RightVolume = (double)SwapEndianness(BitConverter.ToUInt16(value, offset)) / 10;
                        offset += 2;
                        break;
                    case LogEntryType.BluetoothConnected:
                        logEntryModel.Address = new byte[6];
                        Array.Copy(value, offset, logEntryModel.Address, 0, 6);
                        offset += 6;
                        break;
                    case LogEntryType.TimeSet:
                        utcTime = SwapEndianness(BitConverter.ToUInt32(value, offset));
                        logEntryModel.UtcTime = ConvertUtcTime(utcTime);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                // If device does not send the proper payload length here parsing can fail. For now
                // catch it and move to the next experience
                logEntryModel = null;
            }

            return logEntryModel;
        }

        public IEnumerable<LogEntryModel> LogEntries
        {
            get
            {
                return _logEntries;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <param name="characteristic"></param>
        /// <param name="data"></param>
        private async void UpdateCharacteristic(Guid service, Guid characteristic, byte[] data)
        {
            await WriteCharacteristic(service, characteristic, data);
        }

        private Guid _presetExperiencesId = Guid.Empty;

        public Guid PresetExperiencesId
        {
            get
            {
                return _presetExperiencesId;
            }
            set
            {
                if (_presetExperiencesId != value)
                {

                }
            }
        }

        private byte CreateBreastPumpTestFlags(bool setCalibration1, bool setCalibration2)
        {
            byte flags = 0;

            // Verify only one calibration flag is set
            if (setCalibration1 && !setCalibration2)
            {
                flags = 0x01;
            }

            // Verify only one calibration flag is set
            if (setCalibration2 && !setCalibration1)
            {
                flags = 0x02;
            }

            return flags;
        }

        private byte CreateBreastPumpControlFlags(bool hasMode = false, bool hasSuction = false, bool hasSpeed = false)
        {
            byte flags = 0;

            switch (_desiredState)
            {
                case PumpState.Start:
                    flags |= 0x01;
                    break;
                case PumpState.Pause:
                    flags |= 0x02;
                    break;
                case PumpState.Resume:
                    flags |= 0x03;
                    break;
                default:
                    break;
            }

            switch (_desiredPumpingMode)
            {
                case PumpMode.Expression:
                    flags |= 0x08;
                    break;
                case PumpMode.Stimulation:
                    flags |= 0x10;
                    break;
                default:
                    break;
            }

            if (hasMode)
            {
                flags |= 0x20;
            }

            if (hasSuction)
            {
                flags |= 0x40;
            }

            if (hasSpeed)
            {
                flags |= 0x80;
            }

            return flags;
        }

        private async void UpdateDesiredState()
        {
            byte[] data;

            if (_desiredState == PumpState.Start)
            {
                data = new byte[2];
                data[0] = CreateBreastPumpControlFlags(true, false, false);
                data[1] = (byte)_desiredExperience;
            }
            else
            {
                data = new byte[1];
                data[0] = CreateBreastPumpControlFlags();
            }

            await WriteCharacteristic(Service.BreastPumpStatus, Characteristic.BreastPumpControl, data);
        }

        private PumpState _desiredState = PumpState.Stop;
        public PumpState DesiredState
        {
            set
            {
                if (_connectedPump != null)
                {
                    if (value != _desiredState)
                    {
                        _desiredState = value;
                        UpdateDesiredState();
                    }
                }
            }
        }

        /// <summary>
        /// Set calibration point 1. This is currently with 1 oz in each bottle.
        /// </summary>
        public async void SetcalibrationPoint1()
        {
            byte[] data = new byte[1];

            data[0] = CreateBreastPumpTestFlags(true, false);

            await WriteCharacteristic(Service.BreastPumpStatus, Characteristic.BreastPumpTest, data);
        }

        /// <summary>
        /// Set calibration point 2. This is currently with 5 oz in each bottle.
        /// </summary>
        public async void SetcalibrationPoint2()
        {
            byte[] data = new byte[1];

            data[0] = CreateBreastPumpTestFlags(false, true);

            await WriteCharacteristic(Service.BreastPumpStatus, Characteristic.BreastPumpTest, data);
        }

        private async void UpdateDesiredPumpingMode()
        {
            byte[] data = new byte[1];
            data[0] = CreateBreastPumpControlFlags();
            await WriteCharacteristic(Service.BreastPumpStatus, Characteristic.BreastPumpControl, data);
        }

        private PumpMode _desiredPumpingMode = PumpMode.ExperienceControlled;
        public PumpMode DesiredPumpingMode
        {
            set
            {
                if (_connectedPump != null)
                {
                    if (_desiredPumpingMode != value)
                    {
                        _desiredPumpingMode = value;
                        UpdateDesiredPumpingMode();
                    }
                }
            }
        }

        private async void UpdateDesiredExperience()
        {
            byte[] data = new byte[2];
            data[0] = CreateBreastPumpControlFlags(true, false, false);
            data[1] = (byte)_desiredExperience;
            await WriteCharacteristic(Service.BreastPumpStatus, Characteristic.BreastPumpControl, data);
        }

        private int _desiredExperience = -1;
        public int DesiredExperience
        {
            set
            {
                if (_connectedPump != null)
                {
                    if (_desiredExperience != value)
                    {
                        _desiredExperience = value;
                        UpdateDesiredExperience();
                    }
                }
            }
        }

        private async void UpdateDesiredSuction()
        {
            byte[] data = new byte[2];
            data[0] = CreateBreastPumpControlFlags(false, true, false);
            data[1] = (byte)(_desiredSuction * 10);
            await WriteCharacteristic(Service.BreastPumpStatus, Characteristic.BreastPumpControl, data);
        }

        private int _desiredSuction = -1;
        public int DesiredSuction
        {
            set
            {
                if (_connectedPump != null)
                {
                    if (_desiredSuction != value)
                    {
                        _desiredSuction = value;
                        UpdateDesiredSuction();
                    }
                }
            }
        }

        private async void UpdateDesiredSpeed()
        {
            byte[] data = new byte[2];
            data[0] = CreateBreastPumpControlFlags(false, false, true);
            data[1] = (byte)(_desiredSpeed * 10);
            await WriteCharacteristic(Service.BreastPumpStatus, Characteristic.BreastPumpControl, data);
        }

        private int _desiredSpeed = -1;
        public int DesiredSpeed
        {
            set
            {
                if (_connectedPump != null)
                {
                    if (_desiredSpeed != value)
                    {
                        _desiredSpeed = value;
                        UpdateDesiredSpeed();
                    }
                }
            }
        }

        //TODO: this is tied to connect...
        public void Pair(PumpModel device)
        {
            //TODO: Generate RSA
            // Send request
            // get response
            // build auth request
            // send auth request to azure
            // recieve response
            // if known
            //   send auth token to device
            //   complete
            // Generate code
            // Display code
            // send code and token from azure to device
            // get accepted code from device
            // request security token from azure
            // get response token
            // send token to device
            // get response valid
        }

#region BluetoothAdapter control

        public void Connect(PumpModel device)
        {

            try
            {
                IAdapter adapter = _bluetoothLE.Adapter;

                // We can only connect to one device at a time
                if (_connectedPump == null)
                {
                    _connectingPump = device;

                    adapter.ConnectToDeviceAsync(device.Device);
                }
            }
            catch
            {
            }
        }

        public async void Disconnect()
        {
            try
            {
                IAdapter adapter = _bluetoothLE.Adapter;

                // We can only connect to one device at a time
                if (_connectedPump != null)
                {
                    await adapter.DisconnectDeviceAsync(_connectedPump.Device);
                }
            }
            catch
            {
            }

        }

        public async void Scan()
        {
            if (!_isScanningPaused)
            {
                IAdapter adapter = _bluetoothLE.Adapter;
                await adapter.StartScanningForDevicesAsync();
            }
        }
#endregion

        // Firmware, temp for testing until moved to database

    }

}
