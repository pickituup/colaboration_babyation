using System;
using BabyationApp.Common;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Xamarin.Forms;
using BabyationApp.Managers;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace BabyationApp.Models
{
    public enum PumpState
    {
        Stop,
        Start,
        Pause,
        Resume,
        Error,
        End
    }

    public enum PumpMode
    {
        ExperienceControlled,
        Stimulation,
        Expression
    }

    //public enum PumpStatus
    //{
    //    Idle,
    //    Pumping,
    //    Tipped
    //}

    public class PumpModel : ModelItemBase
    {
        private Guid _id;
        private string _advertisedName = string.Empty;
        private string _name = string.Empty;
        private IDevice _device;
        private int _batteryLevel = 0;
        private string _modelNumber = string.Empty;
        private string _serialNumber = string.Empty;
        private string _hardwareRevision = string.Empty;
        private string _firmwareRevision = string.Empty;
        private string _softwareRevision = string.Empty;
        private IEnumerable<LogEntryModel> _logs = new List<LogEntryModel>();
        private double _leftBreastMilkLevel = 0.0;
        private double _rightBreastMilkLevel = 0.0;
        private ObservableCollection<ExperienceModel> _presetExperiences = new ObservableCollection<ExperienceModel>();
        private ObservableCollection<ExperienceModel> _userExperiences = new ObservableCollection<ExperienceModel>();
        private int _actualSuction = 0;
        private int _actualSpeed = 0;
        private PumpMode _actualPumpingMode = PumpMode.ExperienceControlled;
        private TimeSpan _currentDuration = TimeSpan.Zero;
        private PumpMode _desiredPumpingMode = PumpMode.ExperienceControlled;
        private int _desiredSuction = 0;
        private int _desiredSpeed;
        private bool _chargeState = false;
        //private PumpStatus _status = PumpStatus.Idle;
        private PumpState _desiredState;
        private PumpState _actualState;
        private bool _inUse;
        private bool _isConnected = false;
        //private int _chargedPercent = 0;
        private ObservableCollection<string> _alertMessages = new ObservableCollection<string>();
        private IEnumerable<HistoryModel> _pumpingSessions = new List<HistoryModel>();
        private bool _hasAlerts = false;
        private int _desiredExperience = -1;
        private int _updatePercent = 0;
        private bool _isUpdateAvailable = false;
        private bool _isUpdating = false;
        private StorageType _storage = StorageType.Unspecified;

        public PumpModel()
        {

        }

        public Guid Id
        {
            get
            {
                return _id;
            }

            internal set => SetPropertyChanged(ref _id, value);
        }

        public string AdvertisedName
        {
            get
            {
                return _advertisedName;
            }

            internal set => SetPropertyChanged(ref _advertisedName, value);
        }

        public string Name
        {
            get
            {
                if (_name == string.Empty)
                {
                    byte[] id = _id.ToByteArray();
                    // Build a temporary name to differentiate the pumps
                    return _id.ToString();
                }

                return _name;
            }

            internal set => SetPropertyChanged(ref _name, value);
        }

        public IDevice Device
        {
            get
            {
                return _device;
            }

            internal set
            {
                _device = value;             
            }
        }

        public int BatteryLevel
        {
            get
            {
                return _batteryLevel;
            }

            internal set
            {
                if (SetPropertyChanged(ref _batteryLevel, value))
                {
                    SetPropertyChanged("BatteryLevelText");
                }
            }
        }

        public string BatteryLevelText
        {
            get
            {
                string level = string.Empty;

                if (_isConnected)
                {
                    level = string.Format($"{_batteryLevel}% charged");
                }

                return level;
            }
        }

        public string ModelNumber
        {
            get
            {
                return _modelNumber;
            }

            internal set => SetPropertyChanged(ref _modelNumber, value);
        }

        public string SerialNumber
        {
            get
            {
                return _serialNumber;
            }

            internal set => SetPropertyChanged(ref _serialNumber, value);
        }

        public string HardwareRevision
        {
            get
            {
                return _hardwareRevision;
            }

            internal set => SetPropertyChanged(ref _hardwareRevision, value);
        }

        public string FirmwareRevision
        {
            get
            {
                return _firmwareRevision;
            }

            internal set => SetPropertyChanged(ref _firmwareRevision, value);
        }

        public string SoftwareRevision
        {
            get
            {
                return _softwareRevision;
            }

            internal set => SetPropertyChanged(ref _softwareRevision, value);
        }

        public IEnumerable<LogEntryModel> Logs
        {
            get
            {
                return _logs;
            }

            internal set => SetPropertyChanged(ref _logs, value);
        }

        public double LeftBreastMilkLevel
        {
            get
            {
                return _leftBreastMilkLevel;
            }

            internal set
            {
                if (SetPropertyChanged(ref _leftBreastMilkLevel, value))
                {
                    SetPropertyChanged("LeftBreastMilkLevel");
                }
            }
        }

        public double RightBreastMilkLevel
        {
            get
            {
                return _rightBreastMilkLevel;
            }

            internal set => SetPropertyChanged(ref _rightBreastMilkLevel, value);
        }

        public ObservableCollection<ExperienceModel> PresetExperiences
        {
            get
            {
                return _presetExperiences;
            }

            internal set => SetPropertyChanged(ref _presetExperiences, value);
        }

        public ObservableCollection<ExperienceModel> UserExperiences
        {
            get
            {
                return _userExperiences;
            }

            internal set => SetPropertyChanged(ref _userExperiences, value);
        }

        //Experience PresetExperience;
        //Experience CurrentExperience;
        public int ActualSuction
        {
            get
            {
                return _actualSuction;
            }

            internal set => SetPropertyChanged(ref _actualSuction, value);
        }

        public int ActualSpeed
        {
            get
            {
                return _actualSpeed;
            }

            internal set => SetPropertyChanged(ref _actualSpeed, value);
        }

        public PumpMode ActualPumpingMode
        {
            get
            {
                return _actualPumpingMode;
            }

            internal set => SetPropertyChanged(ref _actualPumpingMode, value);
        }

        public TimeSpan CurrentDuration
        {
            get
            {
                return _currentDuration;
            }

            internal set => SetPropertyChanged(ref _currentDuration, value);
        }

        public PumpState DesiredState
        {
            get
            {
                return _desiredState;
            }

            internal set
            {
                if (_desiredState != value)
                {
                    if (_desiredState == PumpState.Start)
                    {
                        _actualState = PumpState.Stop;
                    }

                    SetPropertyChanged(ref _desiredState, value);
                }
            }
        }

        public PumpState ActualState
        {
            get
            {
                return _actualState;
            }

            internal set => SetPropertyChanged(ref _actualState, value);
        }

        public PumpMode DesiredPumpingMode
        {
            get
            {
                return _desiredPumpingMode;
            }

            internal set => SetPropertyChanged(ref _desiredPumpingMode, value);
        }

        public int DesiredSuction
        {
            get
            {
                return _desiredSuction;
            }

            internal set => SetPropertyChanged(ref _desiredSuction, value);
        }

        public int DesiredSpeed
        {
            get
            {
                return _desiredSpeed;
            }

            internal set => SetPropertyChanged(ref _desiredSpeed, value);
        }

        public bool ChargeState
        {
            get
            {
                return _chargeState;
            }

            internal set => SetPropertyChanged(ref _chargeState, value);
        }

        public bool InUse
        {
            get
            {
                return _inUse;
            }

            internal set
            {
                if (SetPropertyChanged(ref _inUse, value))
                {
                    SetPropertyChanged("IsConnectedText");
                }
            }
        }

        public String IsConnectedText
        {
            get
            {
                return _isConnected ? "CONNECTED" : "DISCONNECTED";
            }
        }

        public bool IsConnected
        {
            get
            {
                return _isConnected;
            }

            internal set => SetPropertyChanged(ref _isConnected, value);
        }

        public ObservableCollection<string> AlertMessages
        {
            get
            {
                return _alertMessages;
            }

            internal set => SetPropertyChanged(ref _alertMessages, value);
        }

        public bool HasAlerts
        {
            get
            {
                return _hasAlerts;
            }

            internal set => SetPropertyChanged(ref _hasAlerts, value);
        }

        public int DesiredExperience
        {
            get
            {
                return _desiredExperience;
            }

            internal set => SetPropertyChanged(ref _desiredExperience, value);
        }

        public IEnumerable<HistoryModel> PumpingSessions
        {
            get
            {
                return _pumpingSessions;
            }

            internal set => SetPropertyChanged(ref _pumpingSessions, value);
        }

        public bool IsUpdateAvailable
        {
            get
            {
                return _isUpdateAvailable;
            }

            internal set => SetPropertyChanged(ref _isUpdateAvailable, value);
        }

        public bool IsUpdating
        {
            get
            {
                return _isUpdating;
            }

            internal set => SetPropertyChanged(ref _isUpdating, value);
        }

        public int UpdatePercent
        {
            get
            {
                return _updatePercent;
            }

            internal set
            {
                if (SetPropertyChanged(ref _updatePercent, value))
                {
                    SetPropertyChanged("UpdatePercentString");
                }
            }
        }

        public string UpdatePercentString
        {
            get
            {
                return string.Format("{0}%", _updatePercent);
            }
        }

        public void UpdateFirmware()
        {
            BluetoothManager.Instance.UpdateFirmware();
        }

        public void SetcalibrationPoint1()
        {
            BluetoothManager.Instance.SetcalibrationPoint1();
        }

        public void SetcalibrationPoint2()
        {
            BluetoothManager.Instance.SetcalibrationPoint2();
        }

        public StorageType Storage
        {
            get
            {
                return _storage;
            }
            set
            {
                _storage = value;
            }
        }
    }
}
