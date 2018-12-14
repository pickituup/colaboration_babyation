using BabyationApp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BabyationApp.Controls.Buttons;
using BabyationApp.Converters;
using BabyationApp.Helpers;
using BabyationApp.Managers;
using BabyationApp.Models;
using Xamarin.Forms;
using BabyationApp.Resources;
using System.Windows.Input;

namespace BabyationApp.Pages.NurseSession
{
    /// <summary>
    /// This class represents Bottle Feeding Start page from the design
    /// </summary>
    public partial class NurseSessionStartPage : PageBase
    {
        ButtonExGroup _btnGroup = new ButtonExGroup();

        public NurseSessionModel ViewModel { get; set; }

        public NurseSessionStartPage()
        {
            InitializeComponent();

            UpdateParams();

            _btnGroup.AddButton(BtnLeft);
            _btnGroup.AddButton(BtnRight);

            _btnGroup.Toggled += _btnGroup_Toggled;
        }

        public void UpdateParams()
        {
            ViewModel = new NurseSessionModel(ShowNotepad, ShowSavedOverlay, FinishSession);
            BindingContext = ViewModel;

            Titlebar.IsVisible = true;

            //TODO: Update this approach
            BtnAddNote.IsPressed = true;
        }

        public override void AboutToShow()
        {
            base.AboutToShow();

            // Restore style:
            Titlebar.IsVisible = true;
            RootLayout.Style = (Style)Application.Current.Resources["AbsoluteLayout_NavigationOnTop"];
            //TODO: Update this approach
            BtnAddNote.IsPressed = false;

            var model = (NurseSessionModel)BindingContext;
            if (model != null)
            {
                model.Reset();
            }

            _btnGroup.UpdateCurrentButton(null);

            if (SessionManager.Instance.CurrentSession != null)
            {
                SessionManager.Instance.CurrentSession.PropertyChanged += CurrentSession_PropertyChanged;
            }
        }

        #region Private

        void _btnGroup_Toggled(ButtonExGroup sender, ButtonBase item, int index)
        {
            if (item != null)
            {
                SessionManager.Instance.CurrentBreast = item == BtnLeft ? BreastType.Left : BreastType.Right;
                if (ViewModel.IsFirstTimePresenting)
                {
                    ViewModel.IsFirstTimePresenting = false;
                    ViewModel.IsNursingRunning = true;
                    this.InvalidateMeasure();
                }

                //RootLayout.LowerChild(LabelTap2Begin);// Children.Remove(LabelTap2Begin);
            }
            else
            {
                SessionManager.Instance.CurrentBreast = BreastType.Unspecified;
            }
        }

        void CurrentSession_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            SessionModel currentSession = SessionManager.Instance.CurrentSession;

            switch (e.PropertyName)
            {
                case "Duration":
                    {
                        ViewModel.DurationTime = SessionManager.Instance.CurrentSession.Duration;
                    }
                    break;
            }
        }
        /// <summary>
        /// Enable/disable fullscreen UI to show notepad
        /// </summary>
        /// <param name="state">If set to <c>true</c> state.</param>
        private void ShowNotepad(bool state)
        {
            if (state)
            {
                Titlebar.IsVisible = false;
                RootLayout.Style = (Style)Application.Current.Resources["AbsoluteLayout_FullScreen"];
                Notepad.OnCloseNotepad += Notepad_OnCloseNotepad;
                Notepad.NoteText = SessionManager.Instance.CurrentSession.Note;
            }
            else
            {
                Titlebar.IsVisible = true;
                RootLayout.Style = (Style)Application.Current.Resources["AbsoluteLayout_NavigationOnTop"];
                Notepad.OnCloseNotepad -= Notepad_OnCloseNotepad;
            }
        }

        /// <summary>
        /// Handle text changes in notepad control
        /// </summary>
        /// <param name="save">If set to <c>true</c> save.</param>
        /// <param name="text">Text.</param>
        void Notepad_OnCloseNotepad(bool save, string text)
        {
            if (save)
            {
                SessionManager.Instance.CurrentSession.Note = text;
            }
            ViewModel?.CloseNoteCommand?.Execute(null);

            this.InvalidateMeasure();
        }

        /// <summary>
        /// Enable fullscreen UI besfore showing SAVED congrat
        /// </summary>
        private void ShowSavedOverlay()
        {
            Titlebar.IsVisible = false;
            RootLayout.Style = (Style)Application.Current.Resources["AbsoluteLayout_FullScreen"];
        }

        private void FinishSession()
        {
            ViewModel.IsFirstTimePresenting = true;
            //RootLayout.RaiseChild(LabelTap2Begin);

            PageManager.Me.SetCurrentPage(typeof(DashboardTabPage));
        }

        #endregion
    }


    public class NurseSessionModel : ObservableObject
    {
        private readonly DeviceTimer _timer = new DeviceTimer();
        private Action FinishSessionAction { get; set; }
        private Action<bool> ShowNotepadAction { get; set; }
        private Action SavedOverlayAction { get; set; }

        public NurseSessionModel(Action<bool> showNotepadAction, Action savedOverlay, Action finishSessionAction)
        {
            ShowNotepadAction = showNotepadAction;
            FinishSessionAction = finishSessionAction;
            SavedOverlayAction = savedOverlay;

            Reset();
        }

        public void Reset()
        {
            ShowNurseSessionPage = true;
            ShowAddNotePage = false;
            ShowSavedPopupPage = false;
            IsNursingPaused = false;
            IsNursingRunning = false;
            IsFirstTimePresenting = true;

            DurationTime = TimeSpan.MinValue;

            SetPropertyChanged(nameof(ChildName));
            SetPropertyChanged(nameof(ButtonNoteText));
            SetPropertyChanged(nameof(LabelNoteText));
            SetPropertyChanged(nameof(LeftDurationText));
            SetPropertyChanged(nameof(RightDurationText));
        }

        private bool _isFirstTime;
        public bool IsFirstTimePresenting
        {
            get => _isFirstTime;
            set
            {
                SetPropertyChanged(ref _isFirstTime, value);
                SetPropertyChanged(nameof(IsNursingRunning));
                SetPropertyChanged(nameof(IsNursingPaused));
            }
        }

        private bool _showNursePage;
        public bool ShowNurseSessionPage
        {
            get => _showNursePage;
            set => SetPropertyChanged(ref _showNursePage, value);
        }

        private bool _showAddNotePage;
        public bool ShowAddNotePage
        {
            get => _showAddNotePage;
            set => SetPropertyChanged(ref _showAddNotePage, value);
        }

        private bool _isNursinRunning;
        public bool IsNursingRunning
        {
            get => _isNursinRunning && !IsFirstTimePresenting;
            set => SetPropertyChanged(ref _isNursinRunning, value);
        }

        public bool IsNursingPaused
        {
            get => !IsNursingRunning && !IsFirstTimePresenting;
            set
            {
                IsNursingRunning = !value;
                SetPropertyChanged(nameof(IsNursingPaused));
            }
        }

        private bool _showSavedPopupPage;
        public bool ShowSavedPopupPage
        {
            get => _showSavedPopupPage;
            set => SetPropertyChanged(ref _showSavedPopupPage, value);
        }

        #region Public UI properties

        public string DurationText
        {
            get
            {
                //if (TimeSpan.MinValue == DurationTime)
                //{
                //    return AppResource.TimeDelimiter; // --:--
                //}
                return TimeSpanToStringConverter.TimeSpanToString(SessionManager.Instance.CurrentSession.Duration);
            }
        }
        private TimeSpan _durationTime;
        public TimeSpan DurationTime
        {
            get => _durationTime;
            set
            {
                if (SetPropertyChanged(ref _durationTime, value))
                {
                    SetPropertyChanged(nameof(DurationText));
                    SetPropertyChanged(nameof(LeftDurationText));
                    SetPropertyChanged(nameof(RightDurationText));
                }
            }
        }
        public string ChildName
        {
            get => ProfileManager.Instance.CurrentProfile?.CurrentBaby?.Name;
        }
        public string LabelNoteText
        {
            get => SessionManager.Instance.CurrentSession.Note;
        }
        public string ButtonNoteText
        {
            get => (String.IsNullOrEmpty(LabelNoteText) ? AppResource.AddANote : AppResource.EditNote);
        }
        public string LeftDurationText
        {
            get
            {
                if (TimeSpan.MinValue == DurationTime)
                {
                    return AppResource.TimeDelimiter; // --:--
                }
                return TimeSpanToStringConverter.TimeSpanToString(SessionManager.Instance.CurrentSession.LeftBreastDuration);
            }
        }
        public string RightDurationText
        {
            get
            {
                if (TimeSpan.MinValue == DurationTime)
                {
                    return AppResource.TimeDelimiter; // --:--
                }
                return TimeSpanToStringConverter.TimeSpanToString(SessionManager.Instance.CurrentSession.RightBreastDuration);
            }
        }

        #endregion

        #region Commands

        private ICommand _stopCommand;
        public ICommand StopCommand
        {
            get
            {
                _stopCommand = _stopCommand ?? new Command(() =>
                {
                    IsNursingPaused = true;

                    SessionManager.Instance.Pause();
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
                    IsNursingPaused = false;

                    SessionManager.Instance.Resume();
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
                    SavedOverlayAction?.Invoke();  // Ask codebehind to prepare fullscreen UI

                    ShowNurseSessionPage = false;
                    ShowSavedPopupPage = true;

                    if (SessionManager.Instance.CurrentSession != null)
                    {
                        SessionManager.Instance.Stop();
                        SessionManager.Instance.Save();
                    }

                    _timer.Enable = true;
                    _timer.Start(() =>
                    {
                        FinishSessionAction?.Invoke();  // Ask codebehind to switch to next page
                        return false;
                    });
                });
                return _finishCommand;
            }
        }

        private ICommand _addNoteCommand;
        public ICommand AddNoteCommand
        {
            get
            {
                _addNoteCommand = _addNoteCommand ?? new Command(() =>
                {
                    ShowNotepadAction?.Invoke(true); // Ask codebehind to prepare fullscreen UI

                    ShowAddNotePage = true;
                    ShowNurseSessionPage = false;
                });

                return _addNoteCommand;
            }
        }

        private ICommand _closeNoteCommand;
        public ICommand CloseNoteCommand
        {
            get
            {
                _closeNoteCommand = _closeNoteCommand ?? new Command(() =>
                {
                    ShowNotepadAction?.Invoke(false);  // Ask codebehind to restore regular UI

                    ShowAddNotePage = false;
                    ShowNurseSessionPage = true;

                    SetPropertyChanged(nameof(LabelNoteText));
                    SetPropertyChanged(nameof(ButtonNoteText));
                });

                return _closeNoteCommand;
            }
        }

        ICommand _closeSaveViewCommand;
        public ICommand CloseSaveViewCommand
        {
            get
            {
                // Short circuiting the timer
                _closeSaveViewCommand = _closeSaveViewCommand ?? new Command(() =>
                {
                    _timer.Enable = false;
                    FinishSessionAction?.Invoke();
                });

                return _closeSaveViewCommand;
            }
        }

        #endregion
    }
}
