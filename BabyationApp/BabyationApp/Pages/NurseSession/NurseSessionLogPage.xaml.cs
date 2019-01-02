using BabyationApp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BabyationApp.Controls.Buttons;
using BabyationApp.Helpers;
using BabyationApp.Managers;
using BabyationApp.Models;
using Xamarin.Forms;
using System.Windows.Input;
using BabyationApp.Resources;
using System.Diagnostics;

namespace BabyationApp.Pages.NurseSession
{
    /// <summary>
    /// This class represents the Nurse Session Log page from the design
    /// </summary>
    public partial class NurseSessionLogPage : PageBase
    {
        DeviceTimer _timer = new DeviceTimer();
        ButtonExGroup _btnGroup = new ButtonExGroup();
        private DateTime _selectedDate = new DateTime();
        private TimeSpan _selectedStartTime = new TimeSpan();
        private Action _updateTime;

        public NurseSessionLogModel ViewModel { get; set; }

        /// <summary>
        /// Constructor -- Initialize the model and binds buttons events and other ui actions
        /// </summary>
        public NurseSessionLogPage()
        {
            InitializeComponent();

            // Moved from AddSession page.
            if (null == HistorySession)
            {
                HistorySession = HistoryManager.Instance.CreateSession(SessionType.Nurse);
            }

            BtnAddNote.CornerRadius = 1;

            _btnGroup.AddButton(BtnLeft);
            _btnGroup.AddButton(BtnRight);

            _btnGroup.Toggled += (sender, item, index) =>
            {
                PullDataFromModel(item);
            };

            ViewModel = new NurseSessionLogModel(ShowNotepad)
            {
                NotReadyToSave = true,
                ShowSavedPopupPage = false
            };

            BindingContext = ViewModel;

            _updateTime = new Action(() =>
            {
                if (HistorySession != null)
                {
                    try
                    {
                        if (NurseStartTime.Time.HasValue)
                        {
                            HistorySession.LeftBreastStartTime = _selectedDate + _selectedStartTime;
                            HistorySession.RightBreastStartTime = _selectedDate + _selectedStartTime;

                            TotalCount();

                            ViewModel.NotReadyToSave = false;
                        }
                        else
                        {
                            TotalCount();
                        }

                        if( NurseTotalTime.Time.HasValue)
                        {
                            if( !String.IsNullOrEmpty(ParseTime(ViewModel.FirstTime)))
                            {
                                if(TimeSpan.TryParseExact(ViewModel.FirstTime, @"mm\:ss", null, out TimeSpan resultTime))
                                {
                                    HistorySession.LeftBreastEndTime = HistorySession.LeftBreastStartTime + resultTime;
                                }
                            }
                            if (!String.IsNullOrEmpty(ParseTime(ViewModel.LastTime)))
                            {
                                if (TimeSpan.TryParseExact(ViewModel.LastTime, @"mm\:ss", null, out TimeSpan resultTime))
                                {
                                    HistorySession.RightBreastEndTime = HistorySession.RightBreastStartTime + resultTime;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debugger.Break();
                        NurseTotalTime.Time = TimeSpan.FromMinutes(0);
                    }

                    //if (_btnGroup.CurrentButton != null)
                    //{
                    //    if (NurseStartTime.Time.HasValue)
                    //    {
                    //        HistorySession.LeftBreastStartTime = _selectedDate + _selectedStartTime;
                    //        HistorySession.RightBreastStartTime = _selectedDate + _selectedStartTime;
                    //    }

                    //    if (NurseTotalTime.Time.HasValue)
                    //    {
                    //        try
                    //        {
                    //            //string[] parts = LblDuraPicker.Text.Split(new string[] {":"},
                    //            //    StringSplitOptions.RemoveEmptyEntries);
                    //            //if (parts.Length >= 2)
                    //            //{
                    //            //    if (_btnGroup.CurrentButton == BtnLeft)
                    //            //    {
                    //            //        HistorySession.LeftBreastEndTime =
                    //            //        HistorySession.LeftBreastStartTime.AddHours(Convert.ToInt32(parts[0]));
                    //            //        HistorySession.LeftBreastEndTime = HistorySession.LeftBreastEndTime.AddMinutes(Convert.ToInt32(parts[1]));
                    //            //        model.NotReadyToSave = false;
                    //            //    }
                    //            //    else if (_btnGroup.CurrentButton == BtnRight)
                    //            //    {
                    //            //        HistorySession.RightBreastEndTime =
                    //            //        HistorySession.RightBreastStartTime.AddHours(Convert.ToInt32(parts[0]));
                    //            //        HistorySession.RightBreastEndTime = HistorySession.RightBreastEndTime.AddMinutes(Convert.ToInt32(parts[1]));
                    //            //        model.NotReadyToSave = false;
                    //            //    }
                    //            //}
                    //        }
                    //        catch
                    //        {
                    //        }
                    //    }
                    //}
                }
            });

            NurseDate.CalendarDate = DateTime.Now;
            NurseDate.ValidationFunc = (dateToValidate) =>
            {
                if (dateToValidate > DateTime.Now.Date)
                {
                    ModalAlertPage.ShowAlertWithClose(AppResource.PickingFutureTime);
                    return false;
                }

                if (dateToValidate + _selectedStartTime >= DateTime.Now)
                {
                    ModalAlertPage.ShowAlertWithClose(AppResource.PickingFutureTime);
                    return false;
                }

                if (NurseTotalTime.Time.HasValue)
                {
                    try
                    {
                        var date = dateToValidate + _selectedStartTime;
                        date = date.AddHours(NurseTotalTime.Time.Value.Hours);
                        date = date.AddMinutes(NurseTotalTime.Time.Value.Minutes);
                        if (date >= DateTime.Now)
                        {
                            ModalAlertPage.ShowAlertWithClose(AppResource.PickingFutureTime);
                            return false;
                        }
                    }
                    catch
                    {
                        return false;
                    }
                }

                return true;
            };

            NurseDate.AfterAction = () =>
            {
                _selectedDate = NurseDate.Date.Value;
                _updateTime();
            };

            NurseStartTime.ValidationFunc = timeValidate =>
            {
                if (_selectedDate + timeValidate >= DateTime.Now)
                {
                    ModalAlertPage.ShowAlertWithClose(AppResource.PickingFutureTime);
                    return false;
                }

                if (NurseTotalTime.Time.HasValue)
                {
                    try
                    {
                        var date = NurseDate.Date.Value + timeValidate;
                        date = date.AddHours(NurseTotalTime.Time.Value.Hours);
                        date = date.AddMinutes(NurseTotalTime.Time.Value.Minutes);
                        if (date >= DateTime.Now)
                        {
                            ModalAlertPage.ShowAlertWithClose(AppResource.PickingFutureTime);
                            return false;
                        }
                    }
                    catch
                    {
                        return false;
                    }
                }

                return true;
            };

            NurseStartTime.AfterAction = () =>
            {
                _selectedStartTime = NurseStartTime.Time.Value;
                _updateTime();
            };

            NurseTotalTime.ValidationFunc = timeValidate =>
            {
                if (NurseStartTime.Time.HasValue)
                {
                    var date = _selectedDate + _selectedStartTime;
                    date = date.AddHours(timeValidate.Hours);
                    date = date.AddMinutes(timeValidate.Minutes);
                    if (date >= DateTime.Now)
                    {
                        ModalAlertPage.ShowAlertWithClose(AppResource.PickingFutureTime);
                        return false;
                    }
                }

                return true;
            };

            NurseTotalTime.AfterAction = () =>
            {
                _updateTime();
            };

            BtnSave.Clicked += (s, e) =>
            {
                if (_historyModel != null)
                {
                    if (null != ProfileManager.Instance?.CurrentProfile?.CurrentBaby)
                    {

                        _historyModel.ChildID = ProfileManager.Instance.CurrentProfile.CurrentBaby.Id;
                        _historyModel.ChildName = ProfileManager.Instance.CurrentProfile.CurrentBaby.Name;

                        HistoryManager.Instance.AddSession(_historyModel);
                    }
                    else
                    {
                        ModalAlertPage.ShowAlertWithClose(AppResource.NoChildError);
                    }
                }
                ViewModel.ShowSavedPopupPage = true;
                //UpdateTitlebarInfo(false, Color.FromHex("#11442B"));

                _timer.Enable = true;
                _timer.Start(() =>
                {
                    PageManager.Me.SetCurrentPage(typeof(DashboardTabPage));
                    return false;
                });
            };

            Titlebar.IsVisible = true;
            LeftPageType = typeof(DashboardTabPage);
            Titlebar.LeftButton.IsVisible = true;
            Titlebar.LeftButton.SetDynamicResource(StyleProperty, "CancelButton");

            NurseTotalTime.Time = TimeSpan.FromMinutes(0);
        }

        public override void AboutToShow()
        {
            base.AboutToShow();

            Titlebar.IsVisible = true;

            ViewModel.Reset();

            NurseDate.CalendarDate = DateTime.Now;
            NurseDate.Date = null;
            NurseStartTime.Time = null;
            NurseTotalTime.Time = TimeSpan.FromMinutes(0);

            _btnGroup.UpdateCurrentButton(null);
            BtnAddNote.IsPressed = false;
        }

        private void TotalCount()
        {
            if (!string.IsNullOrEmpty(ViewModel.FirstTime))
            {
                string firstTimeStr = ParseTime(ViewModel.FirstTime);
                ViewModel.FirstTime = firstTimeStr;
            }

            if (!string.IsNullOrEmpty(ViewModel.LastTime))
            {
                string lastTimeStr = ParseTime(ViewModel.LastTime);
                ViewModel.LastTime = lastTimeStr;
            }

            if (!string.IsNullOrEmpty(ViewModel.FirstTime) && !string.IsNullOrEmpty(ViewModel.LastTime))
            {
                if (TimeSpan.TryParseExact(ViewModel.FirstTime, @"mm\:ss", null, out TimeSpan firstTime) && TimeSpan.TryParseExact(ViewModel.LastTime, @"mm\:ss", null, out TimeSpan lastTime))
                {
                    NurseTotalTime.Time = firstTime + lastTime;
                }
            }
        }

        private string ParseTime(string value)
        {
            string[] splitedString = value.Split(':');
            string res = string.Empty;
            if (splitedString.Length > 1)
            {
                bool hasMinutes = int.TryParse(splitedString[0], out int minutes);
                res += hasMinutes ? string.Format($"{minutes:D2}") : "00";

                res += ":";

                bool hasSeconds = int.TryParse(splitedString[1], out int seconds);
                res += hasSeconds ? string.Format($"{seconds:D2}") : "00";
            }
            else
            {
                if (int.TryParse(splitedString[0], out int minutes))
                {
                    res = string.Format($"{minutes:D2}:00");
                }
            }

            return res;
        }

        private void ShowNotepad(bool state)
        {
            if (state)
            {
                Titlebar.IsVisible = false;
                RootLayout.Style = (Style)Application.Current.Resources["AbsoluteLayout_FullScreen"];
                Notepad.OnCloseNotepad += Notepad_OnCloseNotepad;
                Notepad.NoteText = ViewModel.NoteText;
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
                ViewModel.NoteText = text;
            }
            ViewModel?.CloseNoteCommand?.Execute(null);
        }

        /// <summary>
        /// HistoryModel for which the nurse sessions needs to be logged on this page
        /// </summary>
        HistoryModel _historyModel;
        public HistoryModel HistorySession
        {
            get { return _historyModel; }
            set
            {
                if (value != _historyModel)
                {
                    _historyModel = value;
                    if (_historyModel != null)
                    {
                        _historyModel.PropertyChanged += (sender, args) =>
                        {
                            if (args.PropertyName == "TotalMilkVolume")
                            {
                                //LblTotalOunces.Text = String.Format("{0:F1}oz", new object[] { _newHistoryModel.TotalMilkVolume });
                            }
                        };
                    }
                }
            }
        }

        /// <summary>
        /// Pulls data from Model and updates UI
        /// </summary>
        /// <param name="btn">Left or right button for which the needs to pulled and update the UI</param>
        private void PullDataFromModel(ButtonBase btn)
        {
            try
            {
                if (btn == BtnLeft)
                {
                    if (!_historyModel.LeftBreastStartTime.Date.IsEmpty())
                    {
                        NurseDate.Date = _historyModel.LeftBreastStartTime.Date;
                    }

                    if (!_historyModel.LeftBreastStartTime.TimeOfDay.IsEmpty())
                    {
                        NurseStartTime.Time = _historyModel.LeftBreastStartTime.TimeOfDay;
                    }

                    if (!_historyModel.LeftBreastEndTime.IsEmpty() && !_historyModel.LeftBreastStartTime.IsEmpty() &&
                        _historyModel.LeftBreastEndTime > _historyModel.LeftBreastStartTime)
                    {
                        NurseTotalTime.Time = _historyModel.LeftBreastEndTime - _historyModel.LeftBreastStartTime;
                    }

                }
                else if (btn == BtnRight)
                {
                    if (!_historyModel.RightBreastStartTime.Date.IsEmpty())
                    {
                        NurseDate.Date = _historyModel.RightBreastStartTime.Date;
                    }

                    if (!_historyModel.RightBreastStartTime.TimeOfDay.IsEmpty())
                    {
                        NurseStartTime.Time = _historyModel.RightBreastStartTime.TimeOfDay;
                    }

                    if (!_historyModel.RightBreastEndTime.IsEmpty() && !_historyModel.RightBreastStartTime.IsEmpty() &&
                        _historyModel.RightBreastEndTime > _historyModel.RightBreastStartTime)
                    {
                        NurseTotalTime.Time = _historyModel.RightBreastEndTime - _historyModel.RightBreastStartTime;
                    }
                }
            }
            catch (Exception exc)
            {
                System.Diagnostics.Debug.WriteLine("Exception: " + exc.Message);
            }
        }

        // Short circuiting the timer
        void Handle_SaveView_Tapped(object sender, System.EventArgs e)
        {
            _timer.Enable = false;
            PageManager.Me.SetCurrentPage(typeof(DashboardTabPage));
        }

        private void EntryEx_Completed(object sender, EventArgs e)
        {
            _updateTime();
        }

        private void EntryEx_Unfocused(object sender, FocusEventArgs e)
        {
            _updateTime();
        }
    }

    /// <summary>
    /// This class is the UI model for this page
    /// </summary>
    public class NurseSessionLogModel : ObservableObject
    {
        private Action<bool> ShowNotepadAction { get; set; }

        public NurseSessionLogModel(Action<bool> showNotepadAction)
        {
            ShowNotepadAction = showNotepadAction;
        }

        public void Reset()
        {
            ShowSavedPopupPage = false;
            NotReadyToSave = true;

            FirstTime = string.Empty;
            LastTime = string.Empty;
            ChildName = ProfileManager.Instance.CurrentProfile?.CurrentBaby?.Name;
            NoteText = null;
        }

        #region Data properties

        string _firstTime;
        public string FirstTime
        {
            get { return _firstTime; }
            set { SetPropertyChanged(ref _firstTime, value); }
        }

        string _lastTime;
        public string LastTime
        {
            get { return _lastTime; }
            set { SetPropertyChanged(ref _lastTime, value); }
        }

        TimeSpan _totalTime;
        public TimeSpan TotalTime
        {
            get { return _totalTime; }
            set { SetPropertyChanged(ref _totalTime, value); }
        }

        bool _showAddNotePage;
        public bool ShowAddNotePage
        {
            get => _showAddNotePage;
            set => SetPropertyChanged(ref _showAddNotePage, value);
        }

        private string _noteText;
        public string NoteText 
        { 
            get => _noteText; 
            set
            {
                SetPropertyChanged(ref _noteText, value);
                SetPropertyChanged(nameof(ButtonNoteText));
            }
        }

        public string ButtonNoteText
        {
            get => (String.IsNullOrEmpty(NoteText) ? AppResource.AddANote : AppResource.EditNote);
        }

        private bool _notReadyToSave;
        public bool NotReadyToSave
        {
            get { return _notReadyToSave; }
            set => SetPropertyChanged(ref _notReadyToSave, value);
        }

        private bool _showSavedPopupPage;
        public bool ShowSavedPopupPage
        {
            get { return _showSavedPopupPage; }
            set => SetPropertyChanged(ref _showSavedPopupPage, value);
        }

        string _childName = ProfileManager.Instance.CurrentProfile?.CurrentBaby?.Name;
        public string ChildName
        {
            get { return _childName; }
            set { SetPropertyChanged(ref _childName, value); }
        }

        #endregion

        #region Commands

        private ICommand _addNoteCommand;
        public ICommand AddNoteCommand
        {
            get
            {
                _addNoteCommand = _addNoteCommand ?? new Command(() =>
                {
                    ShowNotepadAction?.Invoke(true); // Ask codebehind to prepare fullscreen UI

                    ShowAddNotePage = true;
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

                    SetPropertyChanged(nameof(NoteText));
                    SetPropertyChanged(nameof(ButtonNoteText));
                });

                return _closeNoteCommand;
            }
        }

        #endregion
    }
}
