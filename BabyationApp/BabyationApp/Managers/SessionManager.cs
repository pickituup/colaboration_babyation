using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BabyationApp.Models;
using BabyationApp.Managers;
using Xamarin.Forms;

namespace BabyationApp.Managers
{
    /// <summary>
    /// The SessionManager class
    /// </summary>
    public class SessionManager
    {
        private static SessionManager _instance = null;
        private SessionModel _sessionModel = null;
        private PumpManager _pumpManager;
        private PumpModel _pumpModel;
        private ExperienceModel _currentExperience;
        private bool _sessionActive = false;
        private int _durationSeconds;
        private bool _isPaused = false;
        private BreastType _currentBreast = BreastType.Unspecified;
        private int _leftBreastDurationSeconds;
        private int _rightBreastDurationSeconds;

        /// <summary>
        /// Get the SessionManager singleton
        /// </summary>
        /// <returns>The SessionManager singleton</returns>
        public static SessionManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SessionManager();
                }
                return _instance;
            }
        }

        /// <summary>
        /// The SessionManagerconstructor
        /// </summary>
        private SessionManager()
        {
            _pumpManager = PumpManager.Instance;
            _pumpManager.PumpDisconnectedEvent += _pumpManager_PumpDisconnectedEvent;
        }

        private void _pumpManager_PumpDisconnectedEvent(object sender, EventArgs e)
        {
            if (_sessionModel != null)
            {
                _sessionModel.ActualState = PumpState.Error;
            }
        }

        public void StartBottleFeeding()
        {
            if (_sessionModel == null)
            {
                DateTime startTime = DateTime.Now;

                _sessionActive = true;
                _isPaused = false;
                _durationSeconds = 0;

                _sessionModel = new SessionModel()
                {
                    StartTime = startTime,
                    LeftBreastStartTime = startTime,
                    RightBreastStartTime = startTime,
                    SessionType = SessionType.BottleFeed
                };

                Xamarin.Forms.Device.StartTimer(TimeSpan.FromMilliseconds(1000), () =>
                {
                    if ((_sessionModel != null) && !_isPaused)
                    {
                        _durationSeconds++;
                        _sessionModel.Duration = new TimeSpan(0, 0, _durationSeconds);
                    }
                    return _sessionActive;
                });
            }
        }

        public void StartNursing()
        {
            if (_sessionModel == null)
            {
                _currentBreast = BreastType.Unspecified;
                _sessionActive = true;
                _isPaused = false;
                _durationSeconds = 0;
                _leftBreastDurationSeconds = 0;
                _rightBreastDurationSeconds = 0;

                _sessionModel = new SessionModel()
                {
                    SessionType = SessionType.Nurse
                };

                Xamarin.Forms.Device.StartTimer(TimeSpan.FromMilliseconds(1000), () =>
                {
                    if ((_sessionModel != null) && !_isPaused && (_currentBreast != BreastType.Unspecified))
                    {
                        if (_currentBreast == BreastType.Left)
                        {
                            _leftBreastDurationSeconds++;
                            _sessionModel.LeftBreastDuration = new TimeSpan(0, 0, _leftBreastDurationSeconds);
                        }
                        else if (_currentBreast == BreastType.Right)
                        {
                            _rightBreastDurationSeconds++;
                            _sessionModel.RightBreastDuration = new TimeSpan(0, 0, _rightBreastDurationSeconds);
                        }
                        _durationSeconds++;
                        _sessionModel.Duration = new TimeSpan(0, 0, _durationSeconds);
                    }
                    return _sessionActive;
                });
            }
        }

        public BreastType CurrentBreast
        {
            get
            {
                return _currentBreast;
            }
            set
            {
                if (_currentBreast != value)
                {
                    if (_sessionModel.SessionType == SessionType.Nurse)
                    {
                        if (value == BreastType.Left)
                        {
                            _sessionModel.LeftBreastStartTime = DateTime.UtcNow;
                        }
                        else if (value == BreastType.Right)
                        {
                            _sessionModel.RightBreastStartTime = DateTime.UtcNow;
                        }

                        if (_currentBreast == BreastType.Left)
                        {
                            _sessionModel.LeftBreastEndTime = DateTime.UtcNow;
                        }
                        else if (_currentBreast == BreastType.Right)
                        {
                            _sessionModel.RightBreastEndTime = DateTime.UtcNow;
                        }
                        else if (_currentBreast == BreastType.Unspecified)
                        {
                            _sessionModel.StartTime = DateTime.UtcNow;
                        }
                    }

                    _currentBreast = value;
                }
            }
        }

        public void StartPumping()
        {
            // If we have not started a session and a pump is connected
            if ((_sessionModel == null) && (_pumpManager.ConnectedPump != null))
            {
                DateTime startTime = DateTime.UtcNow;

                _isPaused = false;
                _durationSeconds = 0;
                _timerActive = false;

                _sessionModel = new SessionModel()
                {
                    StartTime = startTime,
                    LeftBreastStartTime = startTime,
                    RightBreastStartTime = startTime,
                    SessionType = SessionType.Pump,
                    CurrentExperience = _currentExperience,
                    PumpPhase = PumpPhase.Stimulation,
                    Storage = _currentExperience.Storage
                };

                _pumpModel = _pumpManager.ConnectedPump;
                _pumpModel.Storage = StorageType.Unspecified;
                _sessionModel.PropertyChanged -= _sessionModel_PropertyChanged;
                _pumpModel.PropertyChanged -= _pumpModel_PropertyChanged;
                _sessionModel.PropertyChanged += _sessionModel_PropertyChanged;
                _pumpModel.PropertyChanged += _pumpModel_PropertyChanged;
                _pumpModel.DesiredExperience = _currentExperience.ExperienceId;
                _pumpModel.DesiredPumpingMode = PumpMode.ExperienceControlled;
                _sessionModel.DesiredSpeed = _currentExperience.StimulationSpeed;
                _sessionModel.DesiredSuction = _currentExperience.StimulationSuction;
                _sessionModel.ActualSpeed = _currentExperience.StimulationSpeed;
                _sessionModel.ActualSuction = _currentExperience.StimulationSuction;
                _pumpModel.DesiredState = PumpState.Start;
                _sessionActive = true;
            }
        }

        public void ContinuePumping()
        {
            ExperienceModel experience;

            // If we have not started a session and a pump is connected
            if ((_sessionModel == null) && (_pumpManager.ConnectedPump != null))
            {
                DateTime startTime = DateTime.UtcNow;

                _pumpModel = _pumpManager.ConnectedPump;

                _isPaused = (_pumpModel.ActualState == PumpState.Pause) ? true : false;
                _durationSeconds = Convert.ToInt32(_pumpModel.CurrentDuration.TotalSeconds);
                _timerActive = false;

                experience = ExperienceManager.Instance.GetFromExperienceId(_pumpModel.DesiredExperience);

                if (experience != null)
                {
                    _currentExperience = experience;
                }
                else
                {
                    _currentExperience = ExperienceManager.Instance.CurrentExperience;
                }

                _sessionModel = new SessionModel()
                {
                    StartTime = startTime,
                    LeftBreastStartTime = startTime,
                    RightBreastStartTime = startTime,
                    SessionType = SessionType.Pump,
                    CurrentExperience = _currentExperience,
                    PumpPhase = (_pumpModel.ActualPumpingMode == PumpMode.Stimulation) ? PumpPhase.Stimulation : PumpPhase.Expression,
                    Storage = _currentExperience.Storage
                };

                _pumpModel.Storage = StorageType.Unspecified;
                _sessionModel.PropertyChanged -= _sessionModel_PropertyChanged;
                _pumpModel.PropertyChanged -= _pumpModel_PropertyChanged;
                _sessionModel.PropertyChanged += _sessionModel_PropertyChanged;
                _pumpModel.PropertyChanged += _pumpModel_PropertyChanged;
                //_pumpModel.DesiredExperience = _currentExperience.ExperienceId;
                //_pumpModel.DesiredPumpingMode = PumpMode.ExperienceControlled;
                _sessionModel.DesiredSpeed = _pumpModel.DesiredSpeed;
                _sessionModel.DesiredSuction = _pumpModel.DesiredSuction;
                _sessionModel.ActualSpeed = _pumpModel.ActualSpeed;
                _sessionModel.ActualSuction = _pumpModel.ActualSuction;
                //_sessionModel.Duration = _pumpModel.CurrentDuration;
                _sessionModel.ActualState = _pumpModel.ActualState;
                //_pumpModel.DesiredState = PumpState.Start;
                _sessionActive = true;
            }
        }

        private bool _timerActive = false;
        private int _timerSeconds = 0;
        private TimeSpan _timerDuration = TimeSpan.MinValue;
        public event EventHandler TimerFired;

        private void TimerExpired()
        {
            // Stop pumping session
            TimerFired?.Invoke(this, EventArgs.Empty);
        }

        public void StartTimer()
        {
            if (_sessionModel != null)
            {
                if (!_timerActive && (_timerDuration != TimeSpan.MinValue))
                {
                    _timerActive = true;
                    _timerSeconds = Convert.ToInt32(_timerDuration.TotalSeconds);

                    _sessionModel.TimerDuration = _timerDuration;

                    Xamarin.Forms.Device.StartTimer(TimeSpan.FromMilliseconds(1000), () =>
                    {
                        if (_timerSeconds > 0)
                        {
                            _timerSeconds--;

                            _sessionModel.Duration = new TimeSpan(0, 0, _timerSeconds);
                        }
                        else
                        {
                            TimerExpired();
                            _timerActive = false;
                        }

                        return _timerActive;
                    });
                }
            }
        }

        public void StopTimer()
        {
            _timerActive = false;
        }

        public void Pause()
        {
            if (_sessionModel != null)
            {
                if (_sessionModel.SessionType == SessionType.Pump)
                {
                    if (_pumpModel != null)
                    {
                        _pumpModel.DesiredState = PumpState.Pause;
                    }
                }
                _isPaused = true;
            }
        }

        public void Resume()
        {
            if (_sessionModel != null)
            {
                if (_sessionModel.SessionType == SessionType.Pump)
                {
                    if (_pumpModel != null)
                    {
                        _pumpModel.DesiredState = PumpState.Resume;
                    }
                }
                _isPaused = false;
            }
        }

        private void _pumpModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                if (_sessionModel != null)
                {
                    switch (e.PropertyName)
                    {
                        case "ActualState":
                            _sessionModel.ActualState = _pumpModel.ActualState;
                            break;
                        case "LeftBreastMilkLevel":
                            _sessionModel.LeftBreastMilkLevel = _pumpModel.LeftBreastMilkLevel;
                            break;
                        case "RightBreastMilkLevel":
                            _sessionModel.RightBreastMilkLevel = _pumpModel.RightBreastMilkLevel;
                            break;
                        case "CurrentDuration":
                            if (!_timerActive)
                            {
                                _sessionModel.Duration = _pumpModel.CurrentDuration;
                            }
                            break;
                        case "ActualPumpingMode":
                            _sessionModel.PumpPhase = (_pumpModel.ActualPumpingMode == PumpMode.Stimulation) ? PumpPhase.Stimulation : PumpPhase.Expression;
                            break;
                        case "ActualSuction":
                            _sessionModel.ActualSuction = _pumpModel.ActualSuction;
                            break;
                        case "ActualSpeed":
                            _sessionModel.ActualSpeed = _pumpModel.ActualSpeed;
                            break;
                        case "DesiredSuction":
                            _sessionModel.DesiredSuction = _pumpModel.DesiredSuction;
                            break;
                        case "DesiredSpeed":
                            _sessionModel.DesiredSpeed = _pumpModel.DesiredSpeed;
                            break;
                    }
                }
            });
        }

        private void _sessionModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_pumpModel == null) return;

            if (_sessionActive)
            {
                switch (e.PropertyName)
                {
                    case "DesiredState":
                        _pumpModel.DesiredState = _sessionModel.DesiredState;
                        break;
                    case "DesiredSuction":
                        _pumpModel.DesiredSuction = _sessionModel.DesiredSuction;
                        break;
                    case "DesiredSpeed":
                        _pumpModel.DesiredSpeed = _sessionModel.DesiredSpeed;
                        break;
                    case "PumpPhase":
                        if (_sessionModel.PumpPhase == PumpPhase.Stimulation)
                        {
                            _pumpModel.DesiredPumpingMode = PumpMode.Stimulation;
                        }
                        else
                        {
                            _pumpModel.DesiredPumpingMode = PumpMode.Expression;
                        }
                        break;
                    case "CurrentExperience":
                        _pumpModel.DesiredExperience = _sessionModel.CurrentExperience.ExperienceId;
                        break;
                    case "TimerDuration":
                        _timerDuration = _sessionModel.TimerDuration;
                        break;                       
                    case "Storage":
                        _pumpModel.Storage = _sessionModel.Storage;
                        break;
                    default:
                        break;
                }
            }
        }

        public void Stop()
        {
            if (_sessionModel.SessionType == SessionType.Pump)
            {
                if (_pumpModel != null)
                {
                    if ((_pumpModel.ActualState == PumpState.Start) ||
                        (_pumpModel.ActualState == PumpState.Resume) ||
                        (_pumpModel.ActualState == PumpState.Pause))
                    {
                        _pumpModel.DesiredState = PumpState.Stop;
                    }
                }
            }
            else if (_sessionModel.SessionType == SessionType.Nurse)
            {
                if (_currentBreast == BreastType.Left)
                {
                    _sessionModel.LeftBreastEndTime = DateTime.UtcNow;
                }
                else if (_currentBreast == BreastType.Right)
                {
                    _sessionModel.RightBreastEndTime = DateTime.UtcNow;
                }
            }
            _sessionModel.EndTime = DateTime.UtcNow;
        }

        public void Save()
        {
            HistoryManager historyManager = HistoryManager.Instance;
            HistoryModel historyModel;

            if (_sessionModel != null)
            {
                // Write to history manager
                if (_sessionModel.SessionType == SessionType.BottleFeed)
                {
                    historyModel = historyManager.CreateSession(SessionType.BottleFeed);

                    historyModel.StartTime = _sessionModel.StartTime;
                    historyModel.EndTime = _sessionModel.EndTime;
                    historyModel.TotalMilkVolume = _sessionModel.TotalMilkVolume;
                    historyModel.Description = _sessionModel.Note;
                    historyModel.Milk = _sessionModel.Milk;
                    historyModel.Storage = _sessionModel.Storage;

                    historyManager.AddSession(historyModel);
                }
                else if (_sessionModel.SessionType == SessionType.Nurse)
                {
                    historyModel = historyManager.CreateSession(SessionType.Nurse);

                    historyModel.StartTime = _sessionModel.StartTime;
                    historyModel.EndTime = _sessionModel.EndTime;
                    historyModel.LeftBreastStartTime = _sessionModel.LeftBreastStartTime;
                    historyModel.LeftBreastEndTime = _sessionModel.LeftBreastEndTime;
                    historyModel.RightBreastStartTime = _sessionModel.RightBreastStartTime;
                    historyModel.RightBreastEndTime = _sessionModel.RightBreastEndTime;
                    historyModel.TotalMilkVolume = _sessionModel.TotalMilkVolume;
                    historyModel.Description = _sessionModel.Note;
                    historyModel.Milk = _sessionModel.Milk;
                    historyModel.Storage = _sessionModel.Storage;

                    historyManager.AddSession(historyModel);
                }

                _sessionModel = null;
                _sessionActive = false;
            }
        }

        public void Finished()
        {
            Save();
        }

        public ExperienceModel CurrentExperience
        {
            get
            {
                return _currentExperience;
            }
            set
            {
                _currentExperience = value;
            }
        }

        public SessionModel CurrentSession
        {
            get
            {
                return _sessionModel;
            }
            internal set
            {
                _sessionModel = value;
            }
        }
    }
}
