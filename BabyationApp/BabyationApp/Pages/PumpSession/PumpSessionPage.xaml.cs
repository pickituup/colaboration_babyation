using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BabyationApp.Common;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.IO;
using BabyationApp.Controls.Views;
using BabyationApp.Controls.Buttons;
using BabyationApp.Helpers;
using BabyationApp.Models;
using BabyationApp.Managers;
using BabyationApp.Pages.Modes;
using System.Diagnostics;
using System.Windows.Input;
using BabyationApp.Resources;

namespace BabyationApp.Pages.PumpSession
{
    public partial class PumpSessionPage : PageBase
    {
        private static bool _isFirstTime = true;
        private PumpSessionModel ViewModel;

        public PumpSessionPage()
        {
            InitializeComponent();

            ViewModel = new PumpSessionModel(FinishSession);
            BindingContext = ViewModel;

            RestorePageDefaults();
            SetupModeControls();

            /*
            ViewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Picture" && ViewModel.IsPictureChangedByUser) {
                    if (ProfileManager.Instance.CurrentProfile != null && ProfileManager.Instance.CurrentProfile.CurrentBaby != null) {
                        ProfileManager.Instance.CurrentProfile.CurrentBaby.Picture = ViewModel.Picture;
                    }
                }
            };

                BtnLetdown.Toggled += (sender, args) =>
                {
                    ViewModel.Data.PumpPhase = BtnLetdown.IsToggled ? PumpPhase.Expression : PumpPhase.Stimulation;
                    ViewModel.PumpPhasePic = null;
                    ViewModel.PumpPhaseText = "";
                };

                BtnPicture.Clicked += (sender, args) =>
                {
                    myMediaView.AboutToShow();
                    ViewModel.IsMyMediaMode = true;
                };

                BtnSetTimer.Clicked += (sender, args) =>
                {
                    UpdateTitlebarInfo(false, Color.FromHex("#F8EBE3"));
                    ViewModel.IsSetTimerMode = true;
                };

                EntrySetTimeMinute.TextChanged += (sender, args) =>
                {
                    BtnTimerSet.IsEnabled = InputValidator.IsValidInput(EntrySetTimeMinute.Text);
                };

                BtnTimerSet.Clicked += (sender, args) =>
                {
                    UpdateTitlebarInfo(true, Color.FromHex("#F9DCD9"));
                    ViewModel.IsSetTimerMode = false;
                    SessionManager.Instance.CurrentSession.TimerDuration = TimeSpan.FromMinutes(Convert.ToDouble(EntrySetTimeMinute.Text));
                    SessionManager.Instance.StartTimer();
                };

                BtnCloseTimer.Clicked += (s, e) =>
                {
                    UpdateTitlebarInfo(true, Color.FromHex("#F9DCD9"));
                    ViewModel.IsSetTimerMode = false;
                };


                BtnNoThanks.Clicked += (sender, args) =>
                {
                    SessionManager.Instance.Finished();
                    ViewModel.IsSaveMode = false;
                    PageManager.Me.SetCurrentPage(typeof(DashboardTabPage));
                };
                BtnYesSave.Clicked += (sender, args) =>
                {
                    SessionManager.Instance.Save();
                    ViewModel.IsSaveMode = false;
                    PageManager.Me.SetCurrentPage(typeof(DashboardTabPage));
                };

                BtnClosePopup.Clicked += (s, e) =>
                {
                    UpdateTitlebarInfo(true, Color.FromHex("#F9DCD9"));
                    ViewModel.IsSaveMode = false;
                };

                SessionManager.Instance.TimerFired += (sender, args) => Finished();

                listView.ItemsSource = ExperienceManager.Instance.AllExperiences;
                listView.ItemSelected += (s, e) =>
                {
                    if (e.SelectedItem != null) {
                        var selectedItem = (ExperienceModel)e.SelectedItem;
                        if (selectedItem != null) {
                            SessionManager.Instance.CurrentSession.CurrentExperience = selectedItem;
                            ExperienceManager.Instance.CurrentExperience = selectedItem;
                        }
                    }
                };

                BtnTimerSet.MiddleCirclePadding = 7;
                BtnTimerSet.InnerCirclePadding = 10;
                BtnTimerSet.FontFamilyTop = "fonts/HurmeGeometricSans3Bold.otf#HurmeGeometricSans3 Bold";
                BtnTimerSet.FontFamilyBottom = "fonts/HurmeGeometricSans3Bold.otf#HurmeGeometricSans3 Bold";
                BtnTimerSet.FontAttributesTop = FontAttributes.Bold;
                BtnTimerSet.FontAttributesBottom = FontAttributes.Bold;

                BtnSuction.UpdateToBigUpDown();
                BtnSpeed.UpdateToBigUpDown();

                ExperienceManager.Instance.CurrentExperienceChanged += (sender, args) =>
                {
                    ViewModel.CurrentModeName = ExperienceManager.Instance.CurrentExperience.Name;
                    UpdateStorageType();
                };

                MediaManager.Instance.CurrentPumpPictureChanged += (media) => UpdateBabyInfo();

*/
        }

        public override void AboutToShow()
        {
            base.AboutToShow();

            // Restore style:
            Titlebar.IsVisible = true;
            Titlebar.LeftButton.IsVisible = false;

            RootLayout.Style = (Style)Application.Current.Resources["AbsoluteLayout_NavigationOnTop"];

            if (_isFirstTime)
            {
                _isFirstTime = false;

                ViewModel.Reset();
            }

            ExperienceManager.Instance.CurrentExperienceChanged -= Instance_CurrentExperienceChanged;
            ViewModel.PropertyChanged -= ViewModel_PropertyChanged;

            if (SessionManager.Instance.CurrentSession == null)
            {
                SessionManager.Instance.CurrentExperience = ExperienceManager.Instance.CurrentExperience;
                SessionManager.Instance.StartPumping();

                ViewModel.CurrentMode = SessionManager.Instance.CurrentExperience;
                ViewModel.Data = SessionManager.Instance.CurrentSession;
            }

            ExperienceManager.Instance.CurrentExperienceChanged += Instance_CurrentExperienceChanged;
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;

            ViewModel.Refresh();

            ViewModel.Data.PropertyChanged -= Data_PropertyChanged;
            ViewModel.Data.PropertyChanged += Data_PropertyChanged;

            UpdateBabyInfo();
            UpdateStorageType(ViewModel.MilkType);
            UpdatePumpLevels();
            UpdatePumpLevelControls();
        }

        public void ShowMediaView()
        {
            LeftPageType = typeof(PumpSessionPage);

            myMediaView.Initialize(this);
            myMediaView.AboutToShow();
        }

        /// <summary>
        /// Invokes from Media view to hide itself.
        /// </summary>
        public void HideMediaView()
        {
            if( ViewModel.IsMediaMode )
            {
                if (myMediaView.IsVisible)
                {
                    myMediaView.AboutToHide();
                }
                ViewModel.IsMediaMode = false;
            }
            RestorePageDefaults();
            UpdateBabyInfo();
            ViewModel.Refresh();
        }

        #region Property changes

        private void Data_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ActualState")
            {
                if (ViewModel.Data.ActualState == PumpState.Pause)
                {
                    if (!ViewModel.IsPausedMode)
                    {
                        ViewModel.IsPausedMode = true;
                    }
                }
                else if (ViewModel.Data.ActualState == PumpState.Resume)
                {
                    if (ViewModel.IsPausedMode)
                    {
                        ViewModel.IsPausedMode = false;
                    }
                }
                else if ((ViewModel.Data.ActualState == PumpState.Stop) || (ViewModel.Data.ActualState == PumpState.End) || (ViewModel.Data.ActualState == PumpState.Error))
                {
                    Device.BeginInvokeOnMainThread(FinishSession);
                }
            }
            else if (e.PropertyName == "ActualSpeed")
            {
                UpdatePumpLevelControls();
            }
            else if (e.PropertyName == "ActualSuction")
            {
                UpdatePumpLevelControls();
            }
            else if (e.PropertyName == "PumpPhase")
            {
                Device.BeginInvokeOnMainThread(ViewModel.UpdatePumpData);
            }
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentMode")
            {
                UpdateStorageType(ViewModel.MilkType);
            }
            else if (e.PropertyName == "CurrentSpeed")
            {
                //UpdatePumpLevelControls();
            }
            else if (e.PropertyName == "CurrentSuction")
            {
                //UpdatePumpLevelControls();
            }
            else if (e.PropertyName == "MilkType")
            {
                UpdateStorageType(ViewModel.MilkType);
            }
            else if (e.PropertyName == "Picture")
            {
                if(ViewModel.IsPictureChangedByUser && ProfileManager.Instance.CurrentProfile != null && ProfileManager.Instance.CurrentProfile.CurrentBaby != null)
                {
                    ProfileManager.Instance.CurrentProfile.CurrentBaby.Picture = ViewModel.Picture;
                }
            }
            else if (e.PropertyName == "IsMediaMode")
            {
                if( ViewModel.IsMediaMode )
                {
                    ShowMediaView();
                }
                else
                {
                    //NOP
                }
            }
        }

        void Instance_CurrentExperienceChanged(object sender, EventArgs e)
        {
            ViewModel.CurrentMode = ExperienceManager.Instance.CurrentExperience;
        }

        #endregion

        #region Private

        private void RestorePageDefaults()
        {
            Titlebar.IsVisible = true;
            Titlebar.LeftButton.IsVisible = false;
            Titlebar.Title = AppResource.PumpingSession_Upper;

            RootLayout.Style = (Style)Application.Current.Resources["AbsoluteLayout_NavigationOnTop"];

            LeftPageType = typeof(DashboardTabPage);
        }

        private void SetupModeControls()
        {
            _circleSuctionStimulation.RatioSmall = 6.0 / 20.0;
            _circleSuctionStimulation.OverlapSize = 10.0;
            _circleSpeedStimulation.RatioSmall = 6.0 / 20.0;
            _circleSpeedStimulation.OverlapSize = 10.0;

            _circleSuctionStimulation.ValueUpdated += value => { UpdatePumpLevels(); };
            _circleSpeedStimulation.ValueUpdated += value => { UpdatePumpLevels(); };
        }

        private void UpdateBabyInfo()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                if (MediaManager.Instance.CurrentPumpPicture != null)
                {
                    try
                    {
                        ViewModel.Picture = MediaManager.Instance.CurrentPumpPicture.Image;
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine("Exception at PSP:UpdateBabyInfo- " + e.Message);
                    }
                }
                else
                {
                    ViewModel.Picture = null;
                }
            });
        }

        private void UpdateStorageType(StorageType storageSource)
        {
            if ((ExperienceManager.Instance != null) && (ExperienceManager.Instance.CurrentExperience != null))
            {
                ExperienceManager.Instance.CurrentExperience.Storage = storageSource;
            }
            if ((SessionManager.Instance != null) && (SessionManager.Instance.CurrentSession != null))
            {
                SessionManager.Instance.CurrentSession.Storage = storageSource;
            }
        }

        private void UpdatePumpLevels()
        {
            ViewModel.CurrentSpeed = _circleSpeedStimulation.Value;
            ViewModel.CurrentSuction = _circleSuctionStimulation.Value;
        }

        private void UpdatePumpLevelControls()
        {
            _circleSpeedStimulation.Value = ViewModel.CurrentSpeed;
            _circleSuctionStimulation.Value = ViewModel.CurrentSuction;
        }

        private void FinishSession()
        {
            SessionManager.Instance.Stop();

            if (SessionManager.Instance.CurrentSession != null)
            {
                ViewModel.Data.PropertyChanged -= Data_PropertyChanged;

                //if (SessionManager.Instance.CurrentSession.ExperienceUpdated)
                //{
                //    ViewModel.IsSaveMode = true;
                //}
                //else
                {
                    SessionManager.Instance.Finished();
                    PageManager.Me.SetCurrentPage(typeof(DashboardTabPage));
                }
            }
            else
            {
                SessionManager.Instance.Finished();
                PageManager.Me.SetCurrentPage(typeof(DashboardTabPage));
            }
        }

        #endregion
    }


    public class PumpSessionModel : PictureModel
    {
        private Action FinishSessionAction { get; set; }

        public PumpSessionModel(Action finishAction)
        {
            FinishSessionAction = finishAction;
        }

        #region Public methods

        public void Reset()
        {
            Data = null;
            Picture = null;

            IsModesMode = false;
            IsPausedMode = false;
            IsMediaMode = false;
            IsSetTimerMode = false;
            IsSaveMode = false;
            IsRunningMode = true;
            IsPumpingMode = true;

            ResetMilkTypeControl = true;

            UpdatePumpData();
            UpdateModes();
        }

        public void Refresh()
        {
            IsModesMode = false;
            IsPausedMode = CurrentState == PumpState.Pause;
            IsMediaMode = false;
            IsSetTimerMode = false;
            IsSaveMode = false;
            IsRunningMode = true;
            IsPumpingMode = true;
            
            ResetMilkTypeControl = true;

            UpdatePumpData();
            UpdateModes();

            SetPropertyChanged(nameof(CurrentState));
            SetPropertyChanged(nameof(CurrentMode));
            SetPropertyChanged(nameof(CurrentModeName));
            SetPropertyChanged(nameof(CurrentSpeed));
            SetPropertyChanged(nameof(CurrentSuction));
            SetPropertyChanged(nameof(MilkType));
            SetPropertyChanged(nameof(Picture));
        }

        public void UpdatePumpData()
        {
            SetPropertyChanged(nameof(CurrentPumpPhase));
        }

        #endregion

        #region Public UI properties

        // Storage control
        public bool _resetMilkTypeControl;
        public bool ResetMilkTypeControl
        {
            get => _resetMilkTypeControl;
            set
            {
                SetPropertyChanged(ref _resetMilkTypeControl, value);
                SetPropertyChanged(nameof(ResetMilkTypeControl));
            }
        }

        public StorageType MilkType
        {
            get => CurrentMode?.Storage ?? StorageType.Unspecified;
            set
            {
                if (CurrentMode?.Storage != value)
                {
                    CurrentMode.Storage = value;
                    SetPropertyChanged(nameof(MilkType));
                }
            }
        }

        private bool _runningMode;
        public bool IsRunningMode
        {
            get => _runningMode;
            set => SetPropertyChanged(ref _runningMode, value);
        }

        private bool _isPausedMode;
        public bool IsPausedMode
        {
            get => _isPausedMode;
            set
            {
                if (value != _isPausedMode)
                {
                    _isPausedMode = value;

                    if (_runningMode)
                    {
                        if (_isPausedMode)
                        {
                            SessionManager.Instance.Pause();
                        }
                        else
                        {
                            SessionManager.Instance.Resume();
                        }
                    }
                    SetPropertyChanged(nameof(IsPausedMode));
                }
            }
        }

        private bool _isPumpingMode;
        public bool IsPumpingMode
        {
            get => _isPumpingMode;
            set => SetPropertyChanged(ref _isPumpingMode, value);
        }

        private bool _isModesMode;
        public bool IsModesMode
        {
            get { return _isModesMode; }
            set => SetPropertyChanged(ref _isModesMode, value);
        }

        private bool _isSetTimerMode;
        public bool IsSetTimerMode
        {
            get => _isSetTimerMode;
            set => SetPropertyChanged(ref _isSetTimerMode, value);
        }

        private bool _isSaveMode;
        public bool IsSaveMode
        {
            get => _isSaveMode;
            set => SetPropertyChanged(ref _isSaveMode, value);
        }

        private bool _isMyMediaMode;
        public bool IsMediaMode
        {
            get => _isMyMediaMode;
            set
            {
                if( SetPropertyChanged(ref _isMyMediaMode, value))
                {
                    //Show/hide pumping view 
                    IsPumpingMode = !value;
                }
            }
        }

        #endregion

        #region Data properties

        // Modes
        public List<ModeItem> ModesDatasource { get; private set; }

        public ExperienceModel CurrentMode
        {
            get => Data?.CurrentExperience ?? null;
            set
            {
                if(null != Data && Data.CurrentExperience != value)
                {
                    Data.CurrentExperience = value;
                    SetPropertyChanged(nameof(CurrentState));
                    SetPropertyChanged(nameof(CurrentMode));
                    SetPropertyChanged(nameof(CurrentModeName));
                    SetPropertyChanged(nameof(CurrentSpeed));
                    SetPropertyChanged(nameof(CurrentSuction));
                    SetPropertyChanged(nameof(MilkType));
                }
            }
        }

        public String CurrentModeName => CurrentMode?.Name;

        //Pump State
        public PumpState CurrentState => Data?.ActualState ?? PumpState.Stop;

        // Speed/Suction
        public int CurrentSpeed
        {
            get => Data?.ActualSpeed ?? 0;
            set
            {
                if(null != Data && Data.DesiredSpeed != value)
                {
                    Data.DesiredSpeed = value;
                    SetPropertyChanged(nameof(CurrentSpeed));
                }
            }
        }

        public int CurrentSuction
        {
            get => Data?.ActualSuction ?? 0;
            set
            {
                if(null != Data && Data.DesiredSuction != value)
                {
                    Data.DesiredSuction = value;
                    SetPropertyChanged(nameof(CurrentSuction));
                }
            }
        }

        // Pump
        public PumpPhase CurrentPumpPhase
        {
            get => Data?.PumpPhase ?? PumpPhase.Stimulation;
            set
            {
                if( null != Data && Data.PumpPhase != value)
                {
                    Data.PumpPhase = value;
                    SetPropertyChanged(nameof(CurrentPumpPhase));
                }
            }
        }

        // Session
        private SessionModel _data;
        public SessionModel Data
        {
            get 
            { 
                if( null == _data )
                {
                    _data = SessionManager.Instance.CurrentSession ?? new SessionModel();
                }
                return _data; 
            }
            set
            {
                SetPropertyChanged(ref _data, value);
            }
        }

        // Duration
        public TimeSpan Duration
        {
            get
            {
                if(null != Data && null != PumpManager.Instance.ConnectedPump )
                {
                    Data.Duration = PumpManager.Instance.ConnectedPump.CurrentDuration;
                }
                return Data?.Duration ?? TimeSpan.Zero;
            }
        }
        #endregion

        #region Commands

        private ICommand _toggleModesCommand;
        public ICommand ToggleModesCommand
        {
            get
            {
                _toggleModesCommand = _toggleModesCommand ?? new Command((obj) =>
                {
                    IsModesMode = !IsModesMode;
                });
                return _toggleModesCommand;
            }
        }

        private ICommand _selectModeCommand;
        public ICommand SelectModeCommand
        {
            get
            {
                _selectModeCommand = _selectModeCommand ?? new Command((obj) =>
                {
                    if (!obj.GetType().Equals(typeof(ModeItem)))
                        return;

                    ModeItem model = (ModeItem)obj;

                    foreach (var item in ModesDatasource)
                    {
                        if (item.Id.Equals(model.Id))
                        {
                            item.IsSelected = true;
                            model.IsSelected = true;
                            continue;
                        }
                        item.IsSelected = false;
                    }

                    CurrentMode = ExperienceManager.Instance.AllExperiences.FirstOrDefault(item => item.Id == model.Id);

                    SessionManager.Instance.CurrentSession.CurrentExperience = CurrentMode;
                    ExperienceManager.Instance.CurrentExperience = CurrentMode;

                    SetPropertyChanged(nameof(ModesDatasource));
                });
                return _selectModeCommand;
            }
        }

        private ICommand _selectMedia;
        public ICommand SelectMedia
        {
            get
            {
                _selectMedia = _selectMedia ?? new Command((obj) =>
                {
                    IsMediaMode = true;
                });
                return _selectMedia;
            }
        }

        private ICommand _togglePhaseCommand;
        public ICommand TogglePhaseCommand
        {
            get
            {
                _togglePhaseCommand = _togglePhaseCommand ?? new Command((obj) =>
                {
                    CurrentPumpPhase = CurrentPumpPhase == PumpPhase.Stimulation ? PumpPhase.Expression : PumpPhase.Stimulation;
                });
                return _togglePhaseCommand;
            }
        }

        private ICommand _stopCommand;
        public ICommand StopCommand
        {
            get
            {
                _stopCommand = _stopCommand ?? new Command(() =>
                {
                    IsPausedMode = true;
                });
                return _stopCommand;
            }
        }

        private ICommand _resumeCommand;
        public ICommand ResumeCommand
        {
            get
            {
                _resumeCommand = _resumeCommand ?? new Command(() =>
                {
                    IsPausedMode = false;
                });
                return _resumeCommand;
            }
        }

        private ICommand _finishCommand;
        public ICommand FinishCommand
        {
            get
            {
                _finishCommand = _finishCommand ?? new Command(() =>
                {
                    FinishSessionAction?.Invoke();  // Ask codebehind to switch to next page
                });
                return _finishCommand;
            }
        }

        private ICommand _milkTypeCommand;
        public ICommand MilkTypeCommand
        {
            get
            {
                _milkTypeCommand = _milkTypeCommand ?? new Command<StorageType>(ToggleMilkType);
                return _milkTypeCommand;
            }
        }

        #endregion

        #region Private

        private void UpdateModes()
        {
            if (null != ExperienceManager.Instance.AllExperiences)
            {
                CurrentMode = ExperienceManager.Instance.CurrentExperience;

                var items = (from item in ExperienceManager.Instance.AllExperiences
                             select new ModeItem()
                             {
                                 Id = item.Id,
                                 Title = item.Name,
                                 Description = item.Description,
                                 CreationDate = item.CreatedAt,
                                 IsPredefined = item.CreatedBy.Equals("babyation"),
                                 IsNew = (item.CreatedBy.Equals("babyation") && item.IsNew),
                                 IsSelected = (CurrentMode?.Id == item.Id),
                                 SelectModeCommand = SelectModeCommand
                             }).ToList();

                items = items.OrderByDescending(x => x.CreationDate).ToList();

                if (null == ModesDatasource)
                {
                    ModesDatasource = new List<ModeItem>(items);
                }
                else
                {
                    ModesDatasource.ForEach(item => item.IsSelected = (CurrentMode?.Id == item.Id));
                }
            }
            else
            {
                ModesDatasource = null;
            }

            SetPropertyChanged(nameof(ModesDatasource));
        }

        private void ToggleMilkType(StorageType type)
        {
            MilkType = type;

            SetPropertyChanged(nameof(MilkType));
        }

        #endregion
    }
}
