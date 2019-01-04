using System;
using System.Collections.Generic;
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
using BabyationApp.Resources;
using BabyationApp.Controls.Views;
using BabyationApp.Extensions;
using BabyationApp.Controls.TextEditors;
using System.Diagnostics;

namespace BabyationApp.Pages.BottleSession
{
    /// <summary>
    /// This class represents Bottle Feeding Log page from the design
    /// </summary>
    public partial class BottleFeedLogPage : PageBase
    {
        public BottleFeedLogModel ViewModel { get; set; }

        private readonly ButtonExGroup _btnGroupMilk = new ButtonExGroup();
        private readonly ButtonExGroup _btnGroupBreastMilk = new ButtonExGroup();

        /// <summary>
        /// Constructor -- Initialize the model and binds buttons events and other ui actions
        /// </summary>
        public BottleFeedLogPage()
        {
            InitializeComponent();

            // Moved from AddSession page
            //
            if (null == HistorySession)
            {
                HistorySession = HistoryManager.Instance.CreateSession(SessionType.BottleFeed);
            }

            ConfigureBottleAndInventory();

            UpdateParams();
        }

        public void UpdateParams()
        {
            ViewModel = new BottleFeedLogModel(ShowInventory, ShowNotepad, ShowSavedOverlay, FinishSession);
            BindingContext = ViewModel;

            _btnGroupMilk.Toggled += _btnGroupMilk_Toggled;

            Titlebar.IsVisible = true;
            Titlebar.LeftButton.Clicked += LeftButton_Clicked;

            //TODO: Update this approach
            BtnAddNote.IsPressed = true;
        }

        private void _btnGroupMilk_Toggled(ButtonExGroup sender, ButtonBase item, int index)
        {
            ViewModel.IsBottleTypeSelected = item.IsToggled;
        }

        public override void AboutToShow()
        {
            base.AboutToShow();

            // Restore style:
            Titlebar.IsVisible = true;
            Titlebar.LeftButton.IsVisible = true;
            Titlebar.LeftButton.SetDynamicResource(StyleProperty, "CancelButton");

            RootLayout.Style = (Style)Application.Current.Resources["AbsoluteLayout_NavigationOnTop"];
            //TODO: Update this approach
            BtnAddNote.IsPressed = false;

            var model = (BottleFeedLogModel)BindingContext;
            if (model != null)
            {
                model.Reset();
            }

            if (HistorySession != null)
            {
                HistorySession.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == "TotalBreastMilkLevelText")
                    {
                        ViewModel.TotalFedValue = SessionManager.Instance.CurrentSession.TotalBreastMilkLevelText;
                    }
                };
            }
        }
        #region Private

        private void ConfigureBottleAndInventory()
        {
            _btnGroupMilk.AddButton(_btnMilkFormula);
            _btnGroupMilk.AddButton(_btnBreastMilk);
            _btnGroupMilk.Toggled += (sender, item, index) =>
            {
                _gridMilkOptions.IsVisible = item == _btnBreastMilk;
                if (_gridMilkOptions.IsVisible == false)
                {
                    _btnGroupBreastMilk.UpdateCurrentButton(null);
                }
                this.InvalidateMeasure();
            };

            _btnGroupBreastMilk.AddButton(_btnMilkFridge);
            _btnGroupBreastMilk.AddButton(_btnMilkFreezer);
            _btnGroupBreastMilk.AddButton(_btnMilkOther);

            _btnGroupBreastMilk.Toggled += (sender, item, index) =>
            {
                if (!_updatingMilkStorageFromCode)
                {
                    if (item == _btnMilkFridge)
                    {
                        InventoryView.Initialize(InventoryFilter.Fridge);
                        ViewModel.ShowInventoryCommand?.Execute(true);
                        LeftPageType = null;
                    }
                    else if (item == _btnMilkFreezer)
                    {
                        InventoryView.Initialize(InventoryFilter.Freezer);
                        ViewModel.ShowInventoryCommand?.Execute(true);
                        LeftPageType = null;
                    }
                    else if (item == _btnMilkOther)
                    {
                        InventoryView.Initialize(InventoryFilter.Other);
                        ViewModel.ShowInventoryCommand?.Execute(true);
                        LeftPageType = null;
                    }
                    else
                    {
                        ViewModel.ShowInventoryCommand?.Execute(false);
                    }
                }
            };
        }

        private bool _updatingMilkStorageFromCode = false;
        private void UpdateStorageType(bool fromModelToGui = true)
        {
            _updatingMilkStorageFromCode = true;

            var currentSession = HistorySession;

            if (currentSession != null)
            {
                if (fromModelToGui)
                {
                    if (currentSession.Milk == MilkType.Formula)
                    {
                        _btnGroupMilk.UpdateCurrentButton(_btnMilkFormula);
                        _btnGroupBreastMilk.UpdateCurrentButton(null);
                    }
                    else if (currentSession.Milk == MilkType.BreastMilk)
                    {
                        _btnGroupMilk.UpdateCurrentButton(_btnBreastMilk);
                        switch (currentSession.Storage)
                        {
                            case StorageType.Fridge:
                                _btnGroupBreastMilk.UpdateCurrentButton(_btnMilkFridge);
                                break;
                            case StorageType.Freezer:
                                _btnGroupBreastMilk.UpdateCurrentButton(_btnMilkFreezer);
                                break;
                            case StorageType.Other:
                                _btnGroupBreastMilk.UpdateCurrentButton(_btnMilkOther);
                                break;
                            default:
                                _btnGroupBreastMilk.UpdateCurrentButton(null);
                                break;
                        }
                    }
                }
                else
                {
                    if (_btnGroupMilk.CurrentButton == _btnMilkFormula)
                    {
                        currentSession.Milk = MilkType.Formula;
                        currentSession.Storage = StorageType.Other;
                    }
                    else if (_btnGroupMilk.CurrentButton == _btnBreastMilk)
                    {
                        currentSession.Milk = MilkType.BreastMilk;

                        if (_btnGroupBreastMilk.CurrentButton == _btnMilkFridge)
                        {
                            currentSession.Storage = StorageType.Fridge;
                        }
                        else if (_btnGroupBreastMilk.CurrentButton == _btnMilkFreezer)
                        {
                            currentSession.Storage = StorageType.Freezer;
                        }
                        else
                        {
                            currentSession.Storage = StorageType.Other;
                        }
                    }
                }
            }

            _updatingMilkStorageFromCode = false;
        }

        private void ShowInventory(bool state)
        {
            if (state)
            {
                InventoryView.ItemUseNowEvent += InventoryView_ItemUseNowEvent;
            }
            else
            {
                InventoryView.ItemUseNowEvent -= InventoryView_ItemUseNowEvent;
            }
        }

        void LeftButton_Clicked(object sender, EventArgs e)
        {
            if (ViewModel.ShowInventory)
            {
                ViewModel.ShowInventoryCommand?.Execute(false);
            }
            else
            {
                LeftPageType = CurrentDashboard();
                PageManager.Me.SetCurrentPage(LeftPageType);
            }
        }

        void InventoryView_ItemUseNowEvent(HistoryModel model)
        {
            if (IsVisible && HistorySession != null)
            {
                HistorySession.Milk = model.Milk;

                // Other is the catch all for this page
                HistorySession.Storage = model.Storage != StorageType.Freezer || model.Storage != StorageType.Fridge
                                                                            ? StorageType.Other : model.Storage;
                HistoryManager.Instance.RemoveInventory(model);

                UpdateStorageType();

                ViewModel.ShowInventoryCommand?.Execute(false);
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
        /// Enable fullscreen UI besfore showing SAVED congrat
        /// </summary>
        private void ShowSavedOverlay()
        {
            Titlebar.IsVisible = false;
            RootLayout.Style = (Style)Application.Current.Resources["AbsoluteLayout_FullScreen"];
        }

        private void FinishSession()
        {
            SaveLogSession();
            PageManager.Me.SetCurrentPage(CurrentDashboard());
        }

        #endregion

        private HistoryModel _newHistoryModel;

        /// <summary>
        /// The history session for which we need the bottle feed log to
        /// </summary>
        public HistoryModel HistorySession
        {
            get { return _newHistoryModel; }
            set
            {
                if (value != _newHistoryModel)
                {
                    _newHistoryModel = value;
                    if (_newHistoryModel != null)
                    {
                        //UpdateStorageType();
                    }
                }
            }
        }

        private void SaveLogSession()
        {
            if (HistorySession != null)
            {
                HistorySession.StartTime = ViewModel.DateValue + ViewModel.StartTimeValue;

                string res = ParseDurationTime();

                if (TimeSpan.TryParseExact(res, @"mm\:ss", null, out TimeSpan result))
                {
                    //HistorySession.EndTime = HistorySession.StartTime + ViewModel.DurationValue; //_selectedDate + _selectedEndTime;
                    HistorySession.EndTime = HistorySession.StartTime + result; //_selectedDate + _selectedEndTime;
                }
                else
                {
                    HistorySession.EndTime = HistorySession.StartTime;
                }

                HistorySession.TotalMilkVolume = Convert.ToDouble(ViewModel.TotalFedValue);//Convert.ToDouble(EntryTotalOunces.Text);
                HistorySession.Description = ViewModel.NoteText;//LblNoteDesc.Text;

                UpdateStorageType(false);

                if (ProfileManager.Instance?.CurrentProfile?.CurrentBaby != null)
                {
                    HistorySession.ChildID = ProfileManager.Instance.CurrentProfile.CurrentBaby.Id;
                    HistorySession.ChildName = ProfileManager.Instance.CurrentProfile.CurrentBaby.Name;
                }

                HistoryManager.Instance.AddSession(HistorySession);
            }
        }

        private string ParseDurationTime()
        {
            string[] splitedString = ViewModel.DurationValue.Split(':');
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

        private void EntryEx_Unfocused(object sender, FocusEventArgs e)
        {
            ViewModel.DurationValue = ParseDurationTime();
        }
    }


    public class BottleFeedLogModel : ObservableObject
    {
        private readonly DeviceTimer _timer = new DeviceTimer();
        private Action<bool> ShowInventoryAction { get; set; }
        private Action<bool> ShowNotepadAction { get; set; }
        private Action SavedOverlayAction { get; set; }
        private Action FinishSessionAction { get; set; }

        public DateTime MinimumDate => DateTime.Now.Subtract(TimeSpan.FromDays(30)); // 30 days
        public DateTime MaximumDate => DateTime.Now;

        public BottleFeedLogModel(Action<bool> showInventoryAction, Action<bool> showNotepadAction, Action savedOverlay, Action finishSessionAction)
        {
            ShowInventoryAction = showInventoryAction;
            ShowNotepadAction = showNotepadAction;
            SavedOverlayAction = savedOverlay;
            FinishSessionAction = finishSessionAction;

            Reset();
        }

        public void Reset()
        {
            ShowBottleFeedSessionPage = true;
            ShowInventory = false;
            ShowSavedPopupPage = false;
            ShowAddNotePage = false;

            TotalFedValue = "";
            DateValue = DateTime.MinValue;
            StartTimeValue = TimeSpan.Zero;
            DurationValue = "";
            NoteText = "";

            SetPropertyChanged(nameof(ChildName));
            SetPropertyChanged(nameof(DateText));
            SetPropertyChanged(nameof(TotalFedValue));
            SetPropertyChanged(nameof(StartTimeText));
            //SetPropertyChanged(nameof(DurationText));
            SetPropertyChanged(nameof(NoteText));
        }

        #region Public UI properties

        public string ChildName
        {
            get => ProfileManager.Instance.CurrentProfile?.CurrentBaby?.Name;
        }

        private bool _showBottleFeedSessionPage = true;
        public bool ShowBottleFeedSessionPage
        {
            get => _showBottleFeedSessionPage;
            set => SetPropertyChanged(ref _showBottleFeedSessionPage, value);
        }

        private bool _showInventory = false;
        public bool ShowInventory
        {
            get => _showInventory;
            set => SetPropertyChanged(ref _showInventory, value);
        }

        private bool _showAddNotePage;
        public bool ShowAddNotePage
        {
            get => _showAddNotePage;
            set => SetPropertyChanged(ref _showAddNotePage, value);
        }

        private bool _showSavedPopupPage;
        public bool ShowSavedPopupPage
        {
            get => _showSavedPopupPage;
            set => SetPropertyChanged(ref _showSavedPopupPage, value);
        }

        public string DateText
        {
            get
            {
                if (DateTime.MinValue == DateValue)
                {
                    return AppResource.DateDelimiter2; // __/__/____
                }
                string date = DateValue.ToString("MM/dd/yyyy");

                return date;
            }
        }

        private string _nurseStartTimeFormat;
        public string NurseStartTimeFormat
        {
            get => _nurseStartTimeFormat;
            private set
            {
                SetPropertyChanged(ref _nurseStartTimeFormat, value);
            }
        }

        private string _totalFedValue;
        public string TotalFedValue
        {
            get => _totalFedValue;
            set
            {
                //value = InputValidator.ValidateInput(value);

                if (SetPropertyChanged(ref _totalFedValue, value))
                {
                    SetPropertyChanged(nameof(IsDataValid));
                }
            }
        }

        public string StartTimeText
        {
            get
            {
                if (TimeSpan.Zero == StartTimeValue)
                {
                    NurseStartTimeFormat = null;
                    return AppResource.TimeDelimiter2; // __:__
                }

                NurseStartTimeFormat = new DateTime(StartTimeValue.Ticks).ToString("tt").ToLower(); //AM or PM

                //return StartTimeValue.ToTimeString(false);
                string time = Convert.ToDateTime(StartTimeValue.ToString()).ToString("h:mm");
                return time;
            }
        }

        //public string DurationText
        //{
        //    get
        //    {
        //        if (TimeSpan.Zero == DurationValue)
        //        {
        //            return AppResource.TimeDelimiter2; // __:__
        //        }
        //        return DurationValue.ToDurationString();
        //    }
        //}

        public string NoteText { get; set; }

        public string ButtonNoteText
        {
            get => (String.IsNullOrEmpty(NoteText) ? AppResource.AddANote : AppResource.EditNote);
        }

        #endregion

        #region Data propeties

        public SessionModel CurrentSession
        {
            get => SessionManager.Instance.CurrentSession;
        }

        private SessionModel _data;
        public SessionModel Data
        {
            get { return _data; }
            set => SetPropertyChanged(ref _data, value);
        }

        private DateTime _dateValue;
        public DateTime DateValue
        {
            get => _dateValue;
            set
            {
                if (SetPropertyChanged(ref _dateValue, value))
                {
                    SetPropertyChanged(nameof(DateText));
                    SetPropertyChanged(nameof(IsDataValid));
                }
            }
        }

        private TimeSpan _startTimeValue;
        public TimeSpan StartTimeValue
        {
            get => _startTimeValue;
            set
            {
                if (SetPropertyChanged(ref _startTimeValue, value))
                {
                    SetPropertyChanged(nameof(IsDataValid));
                }
                    SetPropertyChanged(nameof(StartTimeText));
            }
        }

        private string _durationValue;
        public string DurationValue
        {
            get => _durationValue;
            set
            {
                if (SetPropertyChanged(ref _durationValue, value))
                {
                    //SetPropertyChanged(nameof(DurationText));
                    SetPropertyChanged(nameof(IsDataValid));
                }
            }
        }

        bool _isBottleTypeSelected;
        public bool IsBottleTypeSelected
        {
            get { return _isBottleTypeSelected; }
            set
            {
                SetPropertyChanged(ref _isBottleTypeSelected, value);
                SetPropertyChanged(nameof(IsDataValid));
            }
        }

        public bool IsDataValid
        {
            get
            {
                return (DateValue != DateTime.MinValue) &&
                    (StartTimeValue != TimeSpan.Zero) &&
                    !string.IsNullOrEmpty(DurationValue) &&
                    !string.IsNullOrEmpty(TotalFedValue) &&
                    IsBottleTypeSelected;
            }
        }

        #endregion

        #region Commands

        private ICommand _fedTtxtChangedCommand;
        public ICommand FedTextChangedCommand
        {
            get
            {
                return _fedTtxtChangedCommand ?? (_fedTtxtChangedCommand = new Command((obj) =>
                {
                    if (obj.GetType().Equals(typeof(EntryEx)))
                    {
                        EntryEx entry = (EntryEx)obj;
                        entry.Text = InputValidator.ValidateInput(entry.Text);
                    }
                }));
            }
        }

        // This logic will be updated with refactoring
        //
        private ICommand _pickerFocusCommand;
        public ICommand PickerFocusCommand
        {
            get
            {
                _pickerFocusCommand = _pickerFocusCommand ?? new Command((obj) =>
                {
                    if (obj.GetType().Equals(typeof(DatePickerEx)))
                    {
                        ((DatePickerEx)obj).Focus();
                    }
                    else if (obj.GetType().Equals(typeof(TimePickerEx)))
                    {
                        ((TimePickerEx)obj).Focus();
                    }
                    else
                    {
                        //Do something with it :)
                    }
                });
                return _pickerFocusCommand;
            }
        }

        private ICommand _showInventoryCommand;
        public ICommand ShowInventoryCommand
        {
            get
            {
                _showInventoryCommand = _showInventoryCommand ?? new Command((obj) =>
                {
                    if (!obj.GetType().Equals(typeof(bool)))
                        return;

                    bool flag = (bool)obj;

                    ShowInventory = flag;
                    ShowBottleFeedSessionPage = !flag;

                    ShowInventoryAction?.Invoke(ShowInventory); // Ask codebehind to prepare UI
                });

                return _showInventoryCommand;
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

                    SetPropertyChanged(nameof(NoteText));
                    SetPropertyChanged(nameof(ButtonNoteText));
                });

                return _closeNoteCommand;
            }
        }

        private ICommand _saveCommand;
        public ICommand SaveCommand
        {
            get
            {
                _saveCommand = _saveCommand ?? new Command(() =>
                {
                    SavedOverlayAction?.Invoke();  // Ask codebehind to prepare fullscreen UI

                    ShowBottleFeedSessionPage = false;
                    ShowSavedPopupPage = true;

                    _timer.Enable = true;
                    _timer.Start(() =>
                    {
                        FinishSessionAction?.Invoke();  // Ask codebehind to switch to next page
                        return false;
                    });
                });
                return _saveCommand;
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

        #region Methods



        #endregion
    }
}

