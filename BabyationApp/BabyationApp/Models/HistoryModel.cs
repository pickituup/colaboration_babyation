using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BabyationApp.Helpers;
using BabyationApp.Interfaces;
using Xamarin.Forms;

namespace BabyationApp.Models
{
    public delegate void HistoryItemUseNowEvent(HistoryModel model);
    public class HistoryModel : ModelItemBase, ISessionItem
    {
        private string _id;
        private SessionType _sessionType;
        private DateTime _startTime = ExtensionMethods.DefaultDateTime;
        private DateTime _endTime = ExtensionMethods.DefaultDateTime;
        private double _totalMilkVolume = 0.0;
        private DateTime _expirationTime = ExtensionMethods.DefaultDateTime;
        private bool _isUsed;
        private Guid _userId;
        private DateTime _leftBreastStartTime = ExtensionMethods.DefaultDateTime;
        private DateTime _leftBreastEndTime = ExtensionMethods.DefaultDateTime;
        private double _leftBreastMilkVolume = 0.0;
        private DateTime _rightBreastStartTime = ExtensionMethods.DefaultDateTime;
        private DateTime _rightBreastEndTime = ExtensionMethods.DefaultDateTime;
        private double _rightBreastMilkVolume = 0.0;
        private bool _isPreferred = false;
        private string _description;
        private MilkType _milk;
        private StorageType _storage;
        private string _user = "";
        private string _childID;
        private string _childName;
        private string _feedByProfileId;

        public event HistoryItemUseNowEvent UseNowEvent;

        public HistoryModel()
        {
            UseNowCommand = new Command(() => UseNowEvent?.Invoke(this));
            PreferredCommand = new Command(() => IsPreferred = !IsPreferred);
        }

        public string Id
        {
            get
            {
                return _id;
            }
            internal set => SetPropertyChanged(ref _id, value);
        }

        public SessionType SessionType
        {
            get
            {
                return _sessionType;
            }
            set => SetPropertyChanged(ref _sessionType, value);
        }

        public DateTime StartTime
        {
            get
            {
                return _startTime;
            }
            internal set => SetPropertyChanged(ref _startTime, value);
        }

        public DateTime EndTime
        {
            get
            {
                return _endTime;
            }
            internal set => SetPropertyChanged(ref _endTime, value);
        }

        public double TotalMilkVolume
        {
            get
            {
                return _totalMilkVolume;
            }
            internal set => SetPropertyChanged(ref _totalMilkVolume, value);
        }

        private void UpdateTotalMilkVolume()
        {
            TotalMilkVolume = _leftBreastMilkVolume + _rightBreastMilkVolume;
        }

        public DateTime ExpirationTime
        {
            get
            {
                return _expirationTime;
            }
            internal set => SetPropertyChanged(ref _expirationTime, value);
        }

        public bool IsUsed
        {
            get
            {
                return _isUsed;
            }
            set => SetPropertyChanged(ref _isUsed, value);
        }

        public Guid UserId
        {
            get
            {
                return _userId;
            }
            internal set => SetPropertyChanged(ref _userId, value);
        }

        public DateTime LeftBreastStartTime
        {
            get
            {
                return _leftBreastStartTime;
            }
            internal set
            {
                SetPropertyChanged(ref _leftBreastStartTime, value);
                UpdateTotalMilkVolume();
            }
        }

        public DateTime LeftBreastEndTime
        {
            get
            {
                return _leftBreastEndTime;
            }
            internal set => SetPropertyChanged(ref _leftBreastEndTime, value);
        }

        public double LeftBreastMilkVolume
        {
            get
            {
                return _leftBreastMilkVolume;
            }
            internal set
            {
                SetPropertyChanged(ref _leftBreastMilkVolume, value);
                UpdateTotalMilkVolume();
            }
        }

        public DateTime RightBreastStartTime
        {
            get
            {
                return _rightBreastStartTime;
            }
            internal set => SetPropertyChanged(ref _rightBreastStartTime, value);
        }

        public DateTime RightBreastEndTime
        {
            get
            {
                return _rightBreastEndTime;
            }
            internal set => SetPropertyChanged(ref _rightBreastEndTime, value);
        }

        public double RightBreastMilkVolume
        {
            get
            {
                return _rightBreastMilkVolume;
            }
            internal set
            {
                SetPropertyChanged(ref _rightBreastMilkVolume, value);
                UpdateTotalMilkVolume();
            }
        }

        public bool IsPreferred
        {
            get
            {
                return _isPreferred;
            }
            set => SetPropertyChanged(ref _isPreferred, value);
        }

        public string Description
        {
            get
            {
                return _description;
            }
            internal set => SetPropertyChanged(ref _description, value);
        }

        public MilkType Milk
        {
            get
            {
                return _milk;
            }
            internal set => SetPropertyChanged(ref _milk, value);
        }

        public StorageType Storage
        {
            get
            {
                return _storage;
            }
            internal set => SetPropertyChanged(ref _storage, value);
        }

        public string User
        {
            get
            {
                return _user;
            }
            internal set => SetPropertyChanged(ref _user, value);
        }


        public String HistoryViewData
        {
            get
            {
                if (SessionType == SessionType.Nurse)
                {
                    double leftMinutes = (LeftBreastEndTime - LeftBreastStartTime).TotalMinutes;
                    if (leftMinutes < 0.0) leftMinutes = 0.0;
                    double rightMinutes = (RightBreastEndTime - RightBreastStartTime).TotalMinutes;
                    if (rightMinutes < 0.0) rightMinutes = 0.0;
                    return String.Format("{0:F1}m", new object[] { leftMinutes + rightMinutes });
                }
                else
                {
                    return String.Format("{0:F1}oz", new object[] { TotalMilkVolume });
                }
            }
        }


        public ICommand UseNowCommand { get; set; }
        public ICommand PreferredCommand { get; set; }


        public string ChildID
        {
            get => _childID;
            internal set => SetPropertyChanged(ref _childID, value);
        }
        public string ChildName
        {
            get => _childName;
            internal set => SetPropertyChanged(ref _childName, value);
        }

        public string FeedByProfileId
        {
            get => _feedByProfileId;
            set => SetPropertyChanged(ref _feedByProfileId, value);
        }
    }
}
