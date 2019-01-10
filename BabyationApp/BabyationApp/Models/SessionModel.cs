using System;
using BabyationApp.Common;

namespace BabyationApp.Models
{
    public enum PumpPhase
    {
        Stimulation,
        Expression
    }

    public class SessionModel : ModelItemBase
    {
        private Guid _sessionId;
        private PumpState _desiredState = PumpState.Stop;
        private PumpState _actualState = PumpState.Stop;
        private ExperienceModel _currentExperience = null;
        private bool _experienceUpdated = false;
        private PumpPhase _pumpPhase = PumpPhase.Stimulation;
        private BreastType _breast = BreastType.Both;
        private int _desiredSuction = 0;
        private int _desiredSpeed = 0;
        private int _actualSuction = 0;
        private int _actualSpeed = 0;
        private double _leftBreastMilkLevel = 0.0;
        private double _rightBreastMilkLevel = 0.0;
        private double _totalMilkVolume = 0.0;
        private TimeSpan _duration = TimeSpan.Zero;
        private TimeSpan _maxDuration = TimeSpan.Zero;
        private DateTime _startTime;
        private DateTime _endTime;
        private SessionType _sessionType = SessionType.Pump;
        private DateTime _leftBreastStartTime = DateTime.MinValue;
        private DateTime _rightBreastStartTime = DateTime.MinValue;
        private DateTime _leftBreastEndTime = DateTime.MinValue;
        private DateTime _rightBreastEndTime = DateTime.MinValue;
        private TimeSpan _leftBreastduration = TimeSpan.Zero;
        private TimeSpan _rightBreastduration = TimeSpan.Zero;
        private TimeSpan _timerDuration = TimeSpan.Zero;
        private StorageType _storage = StorageType.Unspecified;
        private string _note;
        private MilkType _milk;
        private string _feedProfileId;

        public SessionModel()
        {
            _sessionId = Guid.NewGuid();
        }

        public Guid SessionId
        {
            get
            {
                return _sessionId;
            }
        }

        public PumpState DesiredState
        {
            get
            {
                return _desiredState;
            }
            internal set
            {
                SetPropertyChanged(ref _desiredState, value);
            }
        }

        public PumpState ActualState
        {
            get
            {
                return _actualState;
            }
            internal set
            {
                SetPropertyChanged(ref _actualState, value);
            }
        }

        public ExperienceModel CurrentExperience
        {
            get
            {
                return _currentExperience;
            }
            internal set
            {
                SetPropertyChanged(ref _currentExperience, value);
            }
        }

        public bool ExperienceUpdated
        {
            get
            {
                return _experienceUpdated;
            }
            internal set
            {
                SetPropertyChanged(ref _experienceUpdated, value);
            }
        }

        public PumpPhase PumpPhase
        {
            get
            {
                return _pumpPhase;
            }
            internal set
            {
                SetPropertyChanged(ref _pumpPhase, value);
            }
        }

        private void Update()
        {
            if (_currentExperience != null)
            {
                if (_pumpPhase == PumpPhase.Stimulation)
                {
                    if ((_desiredSuction != 0) && (_desiredSuction != _currentExperience.StimulationSuction))
                    {
                        _experienceUpdated = true;
                    }

                    if ((_desiredSpeed != 0) && (_desiredSpeed != _currentExperience.StimulationSpeed))
                    {
                        _experienceUpdated = true;
                    }
                }
                else
                {
                    if ((_desiredSuction != 0) && (_desiredSuction != _currentExperience.ExpressionSuction))
                    {
                        _experienceUpdated = true;
                    }

                    if ((_desiredSpeed != 0) && (_desiredSpeed != _currentExperience.ExpressionSpeed))
                    {
                        _experienceUpdated = true;
                    }
                }

                if (_breast != _currentExperience.Breast)
                {
                    _experienceUpdated = true;
                }
            }
        }

        public BreastType Breast
        {
            get
            {
                return _breast;
            }
            internal set
            {
                if (_breast != value)
                {
                    _breast = value;
                    Update();
                    SetPropertyChanged(nameof(Breast));
                }
            }
        }

        public int DesiredSuction
        {
            get
            {
                return _desiredSuction;
            }
            internal set
            {
                if (_desiredSuction != value)
                {
                    _desiredSuction = value;
                    Update();
                    SetPropertyChanged(nameof(DesiredSuction));
                }
            }
        }

        public int DesiredSpeed
        {
            get
            {
                return _desiredSpeed;
            }
            internal set
            {
                if (_desiredSpeed != value)
                {
                    _desiredSpeed = value;
                    Update();
                    SetPropertyChanged(nameof(DesiredSpeed));
                }
            }
        }

        public int ActualSuction
        {
            get
            {
                return _actualSuction;
            }
            internal set
            {
                SetPropertyChanged(ref _actualSuction, value);
            }
        }

        public int ActualSpeed
        {
            get
            {
                return _actualSpeed;
            }
            internal set
            {
                SetPropertyChanged(ref _actualSpeed, value);
            }
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
                    SetPropertyChanged("LeftBreastMilkLevelText");
                    SetPropertyChanged("TotalBreastMilkLevelText");
                }
            }
        }

        public string LeftBreastMilkLevelText
        {
            get
            {
                return _leftBreastMilkLevel.ToString("0.0");
            }
        }

        public double RightBreastMilkLevel
        {
            get
            {
                return _rightBreastMilkLevel;
            }
            internal set
            {
                if (SetPropertyChanged(ref _rightBreastMilkLevel, value))
                {
                    SetPropertyChanged("RightBreastMilkLevelText");
                    SetPropertyChanged("TotalBreastMilkLevelText");
                }
            }
        }

        public string RightBreastMilkLevelText
        {
            get
            {
                return _rightBreastMilkLevel.ToString("0.0");
            }
        }

        public string TotalBreastMilkLevelText
        {
            get
            {
                double temp = _leftBreastMilkLevel + _rightBreastMilkLevel;
                return temp.ToString("0.0");
            }
        }

        public TimeSpan Duration
        {
            get
            {
                return _duration;
            }
            set
            {
                SetPropertyChanged(ref _duration, value);
            }
        }

        public TimeSpan MaxDuration
        {
            get
            {
                return _maxDuration;
            }
            set
            {
                SetPropertyChanged(ref _maxDuration, value);
            }
        }

        public DateTime StartTime
        {
            get
            {
                return _startTime;
            }
            internal set
            {
                SetPropertyChanged(ref _startTime, value);
            }
        }

        public DateTime EndTime
        {
            get
            {
                return _endTime;
            }
            internal set
            {
                SetPropertyChanged(ref _endTime, value);
            }
        }

        public SessionType SessionType
        {
            get
            {
                return _sessionType;
            }
            internal set
            {
                SetPropertyChanged(ref _sessionType, value);
            }
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
            }
        }

        public DateTime LeftBreastEndTime
        {
            get
            {
                return _leftBreastEndTime;
            }
            internal set
            {
                SetPropertyChanged(ref _leftBreastEndTime, value);
            }
        }

        public DateTime RightBreastStartTime
        {
            get
            {
                return _rightBreastStartTime;
            }
            internal set
            {
                SetPropertyChanged(ref _rightBreastStartTime, value);
            }
        }

        public DateTime RightBreastEndTime
        {
            get
            {
                return _rightBreastEndTime;
            }
            internal set
            {
                SetPropertyChanged(ref _rightBreastEndTime, value);
            }
        }

        public TimeSpan TimerDuration
        {
            get
            {
                return _timerDuration;
            }
            internal set
            {
                SetPropertyChanged(ref _timerDuration, value);
            }
        }

        public MilkType Milk
        {
            get
            {
                return _milk;
            }
            internal set
            {
                SetPropertyChanged(ref _milk, value);
            }
        }

        public StorageType Storage
        {
            get
            {
                return _storage;
            }
            internal set
            {
                SetPropertyChanged(ref _storage, value);
            }
        }

        public string Note
        {
            get
            {
                return _note;
            }
            internal set
            {
                SetPropertyChanged(ref _note, value);
            }
        }

        public TimeSpan LeftBreastDuration
        {
            get
            {
                return _leftBreastduration;
            }
            internal set
            {
                if (SetPropertyChanged(ref _leftBreastduration, value))
                {
                    if (_sessionType == SessionType.Nurse)
                    {
                        Duration = LeftBreastDuration + RightBreastDuration;
                    }
                }
            }
        }

        public TimeSpan RightBreastDuration
        {
            get
            {
                return _rightBreastduration;
            }
            internal set
            {
                if(SetPropertyChanged(ref _rightBreastduration, value))
                {
                    if (_sessionType == SessionType.Nurse)
                    {
                        Duration = LeftBreastDuration + RightBreastDuration;
                    }
                }
            }
        }

        public double TotalMilkVolume
        {
            get
            {
                return _totalMilkVolume;
            }
            internal set
            {
                SetPropertyChanged(ref _totalMilkVolume, value);
            }
        }

        public TimeSpan CurrentBreastDuration
        {
            get
            {
                TimeSpan duration = TimeSpan.Zero;

                if (_sessionType == SessionType.Nurse)
                {
                    if (_breast == BreastType.Left)
                    {
                        duration = LeftBreastDuration;
                    }
                    else if (_breast == BreastType.Right)
                    {
                        duration = RightBreastDuration;
                    }
                }
                return duration;
            }
            internal set
            {
                if (_sessionType == SessionType.Nurse)
                {
                    if (_breast == BreastType.Left)
                    {
                        LeftBreastDuration = value;
                    }
                    else if (_breast == BreastType.Right)
                    {
                        RightBreastDuration = value;
                    }
                }
            }
        }

        public string FeedProfileId
        {
            get => _feedProfileId;
            internal set => SetPropertyChanged(ref _feedProfileId, value);
        }
    }
}
