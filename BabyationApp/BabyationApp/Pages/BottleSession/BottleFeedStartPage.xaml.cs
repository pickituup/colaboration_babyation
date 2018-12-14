using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

using BabyationApp.Controls.Buttons;
using BabyationApp.Common;
using BabyationApp.Converters;
using BabyationApp.Managers;
using BabyationApp.Models;
using BabyationApp.Helpers;
using System.Windows.Input;
using System.Dynamic;
using BabyationApp.Controls.Views;
using BabyationApp.Resources;

namespace BabyationApp.Pages.BottleSession
{
    /// <summary>
    /// This class represents Bottle Feeding Start page from the design
    /// </summary>
    public partial class BottleFeedStartPage : PageBase
    {
        public BottleFeedInSessionModel ViewModel { get; set; }

        public BottleFeedStartPage()
        {
            try {

            InitializeComponent();

            UpdateParams();
            } catch (Exception ex) {
                Debugger.Break();
                throw;
            }
        }

        public void UpdateParams()
        {
            ViewModel = new BottleFeedInSessionModel(ShowNotepad, FinishSession);
            BindingContext = ViewModel;

            Titlebar.IsVisible = true;

            //TODO: Update this approach
            BtnAddNote.IsPressed = true;
        }

        public override void PageCreationDone()
        {
            base.PageCreationDone();
        }

        public override void AboutToShow()
        {
            base.AboutToShow();

            // Restore style:
            Titlebar.IsVisible = true;
            RootLayout.Style = (Style)Application.Current.Resources["AbsoluteLayout_NavigationOnTop"];
            //TODO: Update this approach
            BtnAddNote.IsPressed = false;

            var model = (BottleFeedInSessionModel)BindingContext;
            if (model != null)
            {
                model.Reset();
            }

            if (SessionManager.Instance.CurrentSession != null)
            {
                SessionManager.Instance.CurrentSession.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == "Duration")
                    {
                        ViewModel.DurationTime = SessionManager.Instance.CurrentSession.Duration;
                    }
                };
            }
        }

        #region Private

        /// <summary>
        /// Enable fullscreen UI besfore showing SAVED congrat
        /// </summary>
        private void ShowSavedOverlay()
        {
            Titlebar.IsVisible = false;
            RootLayout.Style = (Style)Application.Current.Resources["AbsoluteLayout_FullScreen"];
        }

        /// <summary>
        /// Enable/disable fullscreen UI to show notepad
        /// </summary>
        /// <param name="state">If set to <c>true</c> state.</param>
        private void ShowNotepad(bool state)
        {
            if( state )
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
            if( save )
            {
                SessionManager.Instance.CurrentSession.Note = text;
            }
            ViewModel?.CloseNoteCommand?.Execute(null);
        }


        private void FinishSession()
        {
            PageManager.Me.SetCurrentPage(typeof(BottleFeedAmountPage));
        }

        #endregion
    }


    public class BottleFeedInSessionModel : ObservableObject
    {
        private Action FinishSessionAction { get; set; }
        private Action<bool> ShowNotepadAction { get; set; }

        public BottleFeedInSessionModel(Action<bool> showNotepadAction, Action finishSessionAction)
        {
            ShowNotepadAction = showNotepadAction;
            FinishSessionAction = finishSessionAction;

            Reset();
        }

        public void Reset()
        {
            ShowBottleFeedSessionPage = true;
            ShowAddNotePage = false;
            IsFeedingPaused = false;

            DurationTime = TimeSpan.MinValue;

            SetPropertyChanged(nameof(ChildName));
            SetPropertyChanged(nameof(ButtonNoteText));
            SetPropertyChanged(nameof(LabelNoteText));
        }

        private bool _showBottleFeedPage;
        public bool ShowBottleFeedSessionPage
        {
            get => _showBottleFeedPage;
            set => SetPropertyChanged(ref _showBottleFeedPage, value);
        }

        private bool _showAddNotePage;
        public bool ShowAddNotePage
        {
            get => _showAddNotePage;
            set => SetPropertyChanged(ref _showAddNotePage, value);
        }

        private bool _isFeedingRunning;
        public bool IsFeedingRunning
        {
            get => _isFeedingRunning;
            set => SetPropertyChanged(ref _isFeedingRunning, value);
        }

        public bool IsFeedingPaused
        {
            get => !IsFeedingRunning;
            set
            {
                IsFeedingRunning = !value;
                SetPropertyChanged(nameof(IsFeedingPaused));
            }
        }

        #region Public UI properties

        public string DurationText
        {
            get
            {
                if (TimeSpan.MinValue == DurationTime)
                {
                    return AppResource.TimeDelimiter; // --:--
                }
                return TimeSpanToStringConverter.TimeSpanToString(SessionManager.Instance.CurrentSession.Duration);
            }
        }
        private TimeSpan _durationTime;
        public TimeSpan DurationTime 
        { 
            get => _durationTime;
            set
            {
                if( SetPropertyChanged(ref _durationTime, value ))
                {
                    SetPropertyChanged(nameof(DurationText));
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

        #endregion

        #region Commands

        private ICommand _stopCommand;
        public ICommand StopCommand
        {
            get
            {
                _stopCommand = _stopCommand ?? new Command(() =>
                {
                    IsFeedingPaused = true;

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
                    IsFeedingPaused = false;

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
                    ShowAddNotePage = false;

                    if (SessionManager.Instance.CurrentSession != null )
                    {
                        SessionManager.Instance.Stop();
                        // Should be saved on the BottleFeedAmountPage!
                    }

                    FinishSessionAction?.Invoke();  // Ask codebehind to switch to next page
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
                    ShowBottleFeedSessionPage = false;
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
                    ShowBottleFeedSessionPage = true;

                    SetPropertyChanged(nameof(LabelNoteText));
                    SetPropertyChanged(nameof(ButtonNoteText));
                });

                return _closeNoteCommand;
            }
        }

        #endregion
    }
}
