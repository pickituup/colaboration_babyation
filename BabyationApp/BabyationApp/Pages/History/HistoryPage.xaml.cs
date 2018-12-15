

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BabyationApp.Controls.Buttons;
using BabyationApp.Controls.Views;
using BabyationApp.Models;
using Xamarin.Forms;
using BabyationApp.Managers;
using System.Windows.Input;
using BabyationApp.Common;
using Xamarin.Forms.Xaml;
using BabyationApp.Resources;
using System.Globalization;
using BabyationApp.Interfaces;
using BabyationApp.DataObjects;

namespace BabyationApp.Pages.History
{
    /// <summary>
    /// This class represets the history page from the design
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HistoryPage : RootViewBase, IDisposable
    {
        readonly ButtonExGroup _btnGroupTab = new ButtonExGroup();
        readonly ButtonExGroup _btnGroupMilkDays = new ButtonExGroup();

        DateTime? _dateMinimum;
        DateTime DateMinimum
        {
            get 
            {
                if (!_dateMinimum.HasValue)
                {
                    _dateMinimum = HistoryManager.Instance.GetMinDate();
                }

                return _dateMinimum.Value;
            }
        }

        DateTime? _dateMaximum;
        DateTime DateMaximum
        {
            get
            {
                if (!_dateMaximum.HasValue)
                {
                    _dateMaximum = HistoryManager.Instance.GetMaxDate();
                }

                return _dateMaximum.Value;
            }
        }

        //DateTime _currentDayDT = DateTime.Today;
        private bool _firstTimeShow = true;

        DateTime CurrentDay { get; set; } = DateTime.Today;

        public HistoryViewModel ViewModel { get; set; }

        public HistoryPage()
        {
            InitializeComponent();

            BindingContext = ViewModel = new HistoryViewModel(AddSession, ToggleSessionControl, ToggleChildsControl, ToggleBottleControl, UpdateUI);

            Titlebar.IsVisible = true;

            SetupGroupTab();

            UpdateRangeInfoText();

            BtnPrev.Clicked += BtnPrev_Clicked;
            BtnNext.Clicked += BtnNext_Clicked;

            ProfileManager.Instance.ProfilePropertyChanged += Instance_ProfilePropertyChanged;
            ProfileManager.Instance.CurrentBabyChanged += Instance_CurrentBabyChanged;
        }

        /// <summary>
        /// Gets called when this page is about to show and performs the initialization
        /// </summary>
        public override void AboutToShow()
        {
            base.AboutToShow();

            if (_firstTimeShow)
            {
                _firstTimeShow = false;
                ResetView();
            }
            else
            {
                UpdateView();
            }
        }

        void ResetView()
        {
            ViewModel.Reset();

            ViewModel.CurrentWeekDate = DateTime.Today;
            UpdateWeekModel();

            UpdateDayModel();

            ListDay.SelectedItem = null;
            ListWeek.SelectedItem = null;
        }

        void UpdateView()
        {
            DateTime tempDate = ViewModel.CurrentWeekDate;
            ViewModel.CurrentWeekDate = DateTime.MinValue;
            ViewModel.CurrentWeekDate = tempDate;

            UpdateWeekModel();
            UpdateDayModel();

            ListDay.SelectedItem = null;
            ListWeek.SelectedItem = null;
        }

        void SetupGroupTab()
        {
            _btnGroupTab.AddButton(BtnWeek);
            _btnGroupTab.AddButton(BtnDay);

            _btnGroupTab.UpdateCurrentButton(BtnWeek);
            _btnGroupTab.Toggled += GroupTab_Toggled;

            _btnGroupTab.CurrentIndex = 0;
        }

        void UpdateRangeInfoText()
        {
            LblRangeInfo.Text = String.Format("{0} {1}, {2}", CurrentDay.ToString("MMMM", CultureInfo.CurrentCulture), CurrentDay.Day.ToString(), CurrentDay.Year.ToString());
            if (_btnGroupTab.CurrentIndex == 0 && null != ViewModel.WeeklyHistoryRangeModel)
            {
                LblRangeInfo.Text = ViewModel.WeeklyHistoryRangeModel.RangeInfo;
            } 
        }

        void GroupTab_Toggled(ButtonExGroup sender, ButtonBase item, int index)
        {
            WeekContent.IsVisible = index == 0;
            DayContent.IsVisible = index == 1;

            ImgDatePicker.IsVisible = index == 1;

            CheckDateThresholds();
            UpdateRangeInfoText();
        }

        void ImgDatePicker_Tapped(object sender, EventArgs e)
        {
            DatePicker.Date = CurrentDay;
            DatePicker.Focus();
        }

        void Handle_DateSelected(object sender, Xamarin.Forms.DateChangedEventArgs e)
        {
            CurrentDay = e.NewDate.Date;
            UpdateDayModel();
            UpdateRangeInfoText();
        }

        void BtnPrev_Clicked(object sender, EventArgs e)
        {
            if (_btnGroupTab.CurrentIndex == 0) // Week
            {
                ViewModel.CurrentWeekDate = ViewModel.CurrentWeekDate.AddDays(-7);
                UpdateWeekModel();
            }
            else // Day
            {
                CurrentDay = CurrentDay.AddDays(-1);
                UpdateDayModel();
            }

            CheckDateThresholds();
            UpdateRangeInfoText();
        }

        void BtnNext_Clicked(object sender, EventArgs e)
        {
            if (_btnGroupTab.CurrentIndex == 0) // Week
            {
                ViewModel.CurrentWeekDate = ViewModel.CurrentWeekDate.AddDays(7);
                UpdateWeekModel();
            }
            else // Day
            {
                CurrentDay = CurrentDay.AddDays(1);
                UpdateDayModel();
            }

            CheckDateThresholds();
            UpdateRangeInfoText();
        }

        void CheckDateThresholds()
        {
            if (_btnGroupTab.CurrentIndex == 0)
            {
                if (DateMinimum > ViewModel.CurrentWeekDate)
                {
                    ViewModel.CurrentWeekDate = DateMinimum;
                }

                if (DateMaximum < ViewModel.CurrentWeekDate)
                {
                    ViewModel.CurrentWeekDate = DateMaximum;
                }

                BtnPrev.IsEnabled = (ViewModel.CurrentWeekDate != DateMinimum);
                BtnNext.IsEnabled = (ViewModel.CurrentWeekDate != DateMaximum);
            }
            else
            {
                //BtnPrev.IsEnabled = (CurrentDay.Date >= DateMinimum.Date);
                //BtnNext.IsEnabled = (CurrentDay.Date <= DateMaximum.Date);

                BtnPrev.IsEnabled = true;
                BtnNext.IsEnabled = true;
            }
        }

        void Instance_ProfilePropertyChanged(ProfileModel profile, string propertyName) => Device.BeginInvokeOnMainThread(ResetView);

        void Instance_CurrentBabyChanged(object sender, EventArgs e) => Device.BeginInvokeOnMainThread(ResetView);

        /// <summary>
        /// Fills HistoryRangeModelItem with the information from  HistoryRangeModel
        /// </summary>
        /// <param name="binCount">binCount in HistoryRangeModel</param>
        /// <param name="model">HistoryRangeModel itself</param>
        /// <param name="row">HistoryRangeModelItem to add to UI</param>
        /// <param name="type">Type of teh hisotry session</param>
        /// <param name="icon">icon to show for this row</param>
        /// <param name="titleFormat">format for the title</param>
        /// <param name="amountFormat">amount format</param>
        /// <returns></returns>
        HistoryRangeModelItem FillRow(int binCount, HistoryRangeModel model, 
                                      HistoryRangeModelItem row, SessionType type, 
                                      string icon, string titleFormat, string amountFormat, string amountText = null)
        {
            if (!String.IsNullOrEmpty(icon))
            {
                row.Icon = icon;
            }

            if (!String.IsNullOrEmpty(titleFormat))
            {
                double totalValue = model.Total[(int)type];
                if (SessionType.Nurse == type)
                {
                    //totalValue *= 60; // To minutes
                    totalValue /= (0 < binCount ? binCount : 7); // Per day in week

                    row.Title = String.Format(titleFormat, new object[] { totalValue });
                }
                else
                {
                    totalValue /= (0 < binCount ? binCount : 7);
                    row.Title = String.Format(titleFormat, new object[] { totalValue }); // Per day in week
                }
            }

            double maxValue = model.MaxRangeValue[(int)type];

            for (int i = 0; i < binCount; i++)
            {
                row.BinsInfo.Add(model.RangeNames[i]);
                double amount = model.RangeValues[(int)type, i];

                row.BinsAmountValue.Add(String.Format(amountFormat, new object[] { amount }));
                row.BinsAmountText.Add(String.IsNullOrEmpty(amountText) ? String.Empty : amountText);
                row.BarHeights.Add(Math.Max(0.0, row.MaxBarHeight * amount / Math.Max(1.0, maxValue)));
            }

            return row;
        }

        ObservableCollection<HistoryRangeModelItem> _historyListWeek = new ObservableCollection<HistoryRangeModelItem>();

        /// <summary>
        /// Updates the week Model for the Week tab
        /// </summary>
        void UpdateWeekModel()
        {
            var model = ViewModel.WeeklyHistoryRangeModel;

            if (null == ListWeek.Header && ListWeek.Header != ViewModel)
            {
                ListWeek.Header = ViewModel;
            }

            ViewModel.ListWeeklySource.Clear();

            if (null != model)
            {
                switch (ViewModel.SelectedSession)
                {
                    case SessionType.Pump:
                        ViewModel.ListWeeklySource.Add(FillRow(7, model, new HistoryRangeModelItem(), SessionType.Pump, "", String.Format("{0} {1}", AppResource.AverageOuncesPerDay, "{0:F1}\noz"), "{0:F1}\noz"));
                        break;
                    case SessionType.Nurse:
                        ViewModel.ListWeeklySource.Add(FillRow(7, model, new HistoryRangeModelItem(), SessionType.Nurse, "", String.Format("{0} {1} {2}", AppResource.AverageNursedPerDay, "{0:F1}", AppResource.MinutesLower), "{0:F2}", AppResource.MinutesLower));
                        break;
                    case SessionType.BottleFeed:
                        {
                            if (SessionType.Breastmilk == ViewModel.SelectedBottleType)
                            {
                                ViewModel.ListWeeklySource.Add(FillRow(7, model, new HistoryRangeModelItem(), SessionType.Breastmilk, "", String.Format("{0} {1}", AppResource.AverageBottlePerDay, "{0:F1}\noz"), "{0:F1}\noz"));
                            }
                            else if (SessionType.Formula == ViewModel.SelectedBottleType)
                            {
                                ViewModel.ListWeeklySource.Add(FillRow(7, model, new HistoryRangeModelItem(), SessionType.Formula, "", String.Format("{0} {1}", AppResource.AverageBottlePerDay, "{0:F1}\noz"), "{0:F1}\noz"));
                            }
                            else
                            {
                                ViewModel.ListWeeklySource.Add(FillRow(7, model, new HistoryRangeModelItem(), SessionType.BottleFeed, "", String.Format("{0} {1}", AppResource.AverageBottlePerDay, "{0:F1}\noz"), "{0:F1}\noz"));
                            }
                        }
                        break;

                        // default: TODO: Add No data cell
                }
                LblNoWeeklyRecords.IsVisible = false;
                ListWeek.IsVisible = true;

                if (null == ListWeek.ItemsSource)
                {
                    ListWeek.ItemsSource = ViewModel.ListWeeklySource;
                }
            }
            else
            {
                ListWeek.IsVisible = false;
                ListWeek.ItemsSource = null;
                LblNoWeeklyRecords.IsVisible = true;
            }
        }

        /// <summary>
        /// Updates the day Model for the Day tab
        /// </summary>
        void UpdateDayModel()
        {
            var sessions = HistoryManager.Instance.GetDay(CurrentDay)?.ToList();

            List<DailyHistorySessionItem> dailySessionItems = null;

            if (sessions?.Count() > 0)
            {
                dailySessionItems = new List<DailyHistorySessionItem>();
               
                for (int i = 0; i < sessions.Count(); i++) 
                {
                    var session = sessions[i];

                    dailySessionItems.Add(new DailyHistorySessionItem
                    {
                        SessionType = session.SessionType,
                        // TODO: Implement this 
                        //User = x.User,
                        ChildName = session.ChildName,
                        LeftSideVolume = session.LeftBreastMilkVolume,
                        RightSideVolume = session.RightBreastMilkVolume,
                        TotalVolume = session.TotalMilkVolume,
                        LeftSideDuration = (session.LeftBreastEndTime - session.LeftBreastStartTime),
                        RightSideDuration = (session.RightBreastEndTime - session.RightBreastStartTime),
                        TotalDuration = (session.EndTime - session.StartTime),
                        StartTime = session.StartTime,
                        BackgroundColor = i % 2 == 0 ? (Color)Application.Current.Resources["LightBlue"] 
                                                                         : (Color)Application.Current.Resources["LightBlue20"]
                    });
                }

                LblNoDailyRecords.IsVisible = false;
                ListDay.IsVisible = true;
                ListDay.ItemsSource = new ObservableCollection<DailyHistorySessionItem>(dailySessionItems);
            }
            else
            {
                ListDay.IsVisible = false;
                ListDay.ItemsSource = null;
                LblNoDailyRecords.IsVisible = true;
            }
        }

        void AddSession() => PageManager.Me.SetCurrentPage(typeof(AddSessionPage));

        // TODO: Move all toggles into ViewModel
        void ToggleSessionControl(SessionType type)
        {
            if (0 == _btnGroupTab.CurrentIndex) // Week
            {
                UpdateWeekModel();
            }
            else // Day
            {
                UpdateDayModel();
            }
        }

        // TODO: Move all toggles into ViewModel
        void ToggleChildsControl(int index)
        {
            if (0 == _btnGroupTab.CurrentIndex) // Week
            {
                UpdateWeekModel();
            }
            else // Day
            {
                UpdateDayModel();
            }
        }

        // TODO: Move all toggles into ViewModel
        void ToggleBottleControl(SessionType type)
        {
            if (0 == _btnGroupTab.CurrentIndex) // Week
            {
                UpdateWeekModel();
            }
            else // Day
            {
                UpdateDayModel();
            }
        }

        void UpdateUI() => InvalidateMeasure();

        public void Dispose() => throw new NotImplementedException();
    }

    public class DailyHistorySessionItem : ModelItemBase, ISessionItem
    {
        public SessionType SessionType { get; set; }

        public string Description 
        {
            get
            {
                string description = null;

                switch (SessionType)
                {
                    case SessionType.Nurse:
                        description = AppResource.Nursing;
                        break;
                    case SessionType.Pump:
                        description = AppResource.Pumping;
                        break;
                    case SessionType.BottleFeed:
                        description = AppResource.Bottle;
                        break;
                    case SessionType.Formula:
                        description = $"{AppResource.Bottle} - {AppResource.Formula.ToLower()}";
                        break;
                    case SessionType.Breastmilk:
                        description = $"{AppResource.Bottle} - {AppResource.BreastMilk.ToLower()}";
                        break;
                    default:
                        description = AppResource.Other;
                        break;
                }

                if (!string.IsNullOrEmpty(description) && !string.IsNullOrEmpty(ChildName))
                {
                    description += $" - {ChildName}";
                }

                return description;
            }
        }

        public string User { get; set; } = "Me";
        public string ChildName { get; set; }

        public DateTime StartTime { get; set; }

        public string StartTimeDescription => string.Format("{0:hh:mm}", StartTime);
        public string ClockSystemDescriptor => string.Format("{0:tt}", StartTime).ToLower();

        string _pumpingModeDescription;
        public string PumpingModeDescription
        {
            get
            {
                if (!string.IsNullOrEmpty(_pumpingModeDescription))
                {
                    _pumpingModeDescription = AppResource.NotApplicable;
                }

                return _pumpingModeDescription;
            }
            set
            {
                _pumpingModeDescription = value;
            }
        }

        public double LeftSideVolume { get; set; }
        public string LeftSideVolumeDescription => LeftSideVolume <= 0 ? AppResource.NotApplicable : LeftSideVolume.ToString();

        public double RightSideVolume { get; set; }
        public string RightSideVolumeDescription => RightSideVolume <= 0 ? AppResource.NotApplicable : RightSideVolume.ToString();

        public double TotalVolume { get; set; }
        public string TotalVolumeDescription => TotalVolume <= 0 ? AppResource.NotApplicable : TotalVolume.ToString();

        public TimeSpan LeftSideDuration { get; set; }
        public string LeftSideDurationDescription => LeftSideDuration != TimeSpan.Zero ? LeftSideDuration.ToString("mm':'ss") : AppResource.NotApplicable;

        public TimeSpan RightSideDuration { get; set; }
        public string RightSideDurationDescription => RightSideDuration != TimeSpan.Zero ? RightSideDuration.ToString("mm':'ss") : AppResource.NotApplicable;
        public TimeSpan TotalDuration { get; set; }
        public string TotalDurationDescription => TotalDuration != TimeSpan.Zero ? TotalDuration.ToString("mm':'ss") : AppResource.NotApplicable;

        // TODO: Remove this UI-dependent property when refactoring History functionality
        public Color BackgroundColor { get; set; }
    }

    public class HistoryViewModel : ObservableObject
    {
        Action AddSessionAction { get; set; }
        Action<SessionType> ToggleSessionAction { get; set; }
        Action<int> ToggleChildsAction { get; set; }
        Action<SessionType> ToggleBottleAction { get; set; }
        Action UpdateUIAction { get; set; }

        public HistoryViewModel(Action addSessionAction, Action<SessionType> toggleSession, Action<int> toggleChilds, Action<SessionType> toggleBottle, Action updateUI)
        {
            AddSessionAction = addSessionAction;
            ToggleSessionAction = toggleSession;
            ToggleChildsAction = toggleChilds;
            ToggleBottleAction = toggleBottle;
            UpdateUIAction = updateUI;

            Reset();
        }

        public void Reset()
        {
            _selectedSession = SessionType.Pump;
            _selectedChildIndex = -1;
            _selectedBottleType = SessionType.Max;

            //_weeklyChilds = null;
            WeeklyChilds = null;

            _currentWeekDate = DateTime.Today;
            CurrentWeekDate = HistoryManager.Instance.GetMinDate();

            ResetWeeklySessionControl = true;
        }

        #region Public UI properties

        private bool _resetWeeklySessionControl;
        public bool ResetWeeklySessionControl
        { 
            get => _resetWeeklySessionControl;
            set => SetPropertyChanged(ref _resetWeeklySessionControl, value);
        }

        #endregion

        #region Data properties

        private ObservableCollection<HistoryRangeModelItem> _listWeeklySource;
        public ObservableCollection<HistoryRangeModelItem> ListWeeklySource
        {
            get
            {
                if (null == _listWeeklySource)
                {
                    _listWeeklySource = new ObservableCollection<HistoryRangeModelItem>();
                }
                return _listWeeklySource;
            }
        }

        DateTime _currentWeekDate = DateTime.Now;
        public DateTime CurrentWeekDate
        {
            get => _currentWeekDate;
            set
            {
                if (SetPropertyChanged(ref _currentWeekDate, value))
                {
                    Histories = null;
                    SetPropertyChanged(nameof(Histories));

                    WeeklyHeaderModel = null;

                    WeeklyHistoryRangeModel = null;
                    SetPropertyChanged(nameof(WeeklyHistoryRangeModel));
                }
            }
        }

        List<HistoryRangeModel> _histories;
        public List<HistoryRangeModel> Histories
        {
            get
            {
                if (null == _histories)
                {
                    _histories = HistoryManager.Instance.GetWeek(CurrentWeekDate);
                }
                return _histories;
            }
            set => _histories = null;
        }

        HistoryRangeModel _weeklyHeaderModel;
        public HistoryRangeModel WeeklyHeaderModel
        {
            get => _weeklyHeaderModel;
            set
            {
                SetPropertyChanged(ref _weeklyHeaderModel, value);
            }
        }

        HistoryRangeModel _weeklyHistoryRangeModel;
        public HistoryRangeModel WeeklyHistoryRangeModel
        {
            get
            {
                if (null == _weeklyHistoryRangeModel)
                {
                    if (null != Histories)
                    {
                        WeeklyHeaderModel = Histories.ElementAt(0);

                        switch (SelectedSession)
                        {
                            case SessionType.Pump:
                                {
                                    _weeklyHistoryRangeModel = Histories.ElementAt(0);
                                }
                                break;
                            case SessionType.Nurse:
                                {
                                    if (0 <= SelectedChildIndex && (1 + SelectedChildIndex) < Histories.Count)
                                    {
                                        _weeklyHistoryRangeModel = Histories.ElementAt(1 + SelectedChildIndex);
                                    }
                                    else
                                    {
                                        _weeklyHistoryRangeModel = Histories.ElementAt(0);
                                    }
                                }
                                break;
                            case SessionType.BottleFeed:
                                {
                                    if (0 <= SelectedChildIndex && (1 + SelectedChildIndex) < Histories.Count)
                                    {
                                        _weeklyHistoryRangeModel = Histories.ElementAt(1 + SelectedChildIndex);
                                    }
                                    else
                                    {
                                        _weeklyHistoryRangeModel = Histories.ElementAt(0);
                                    }

                                    if (SessionType.Breastmilk == SelectedBottleType)
                                    {
                                        //???
                                    }
                                    else if (SessionType.Formula == SelectedBottleType)
                                    {
                                        //???
                                    }
                                }
                                break;
                        }
                    }
                }
                return _weeklyHistoryRangeModel;
            }
            set => _weeklyHistoryRangeModel = null;
        }

        SessionType _selectedSession;
        public SessionType SelectedSession
        {
            get => _selectedSession;
            set
            {
                if (SetPropertyChanged(ref _selectedSession, value))
                {
                    WeeklyHistoryRangeModel = null;
                    SetPropertyChanged(nameof(WeeklyHistoryRangeModel));
                }
            }
        }

        int _selectedChildIndex;
        public int SelectedChildIndex
        {
            get => _selectedChildIndex;
            set
            {
                if (SetPropertyChanged(ref _selectedChildIndex, value))
                {
                    WeeklyHistoryRangeModel = null;
                    SetPropertyChanged(nameof(WeeklyHistoryRangeModel));
                }
            }
        }

        SessionType _selectedBottleType;
        public SessionType SelectedBottleType
        {
            get => _selectedBottleType;
            set
            {
                if (SetPropertyChanged(ref _selectedBottleType, value))
                {
                    WeeklyHistoryRangeModel = null;
                    SetPropertyChanged(nameof(WeeklyHistoryRangeModel));
                }
            }
        }

        List<ChildItem> _weeklyChilds;
        public List<ChildItem> WeeklyChilds
        {
            get
            {
                if (null == _weeklyChilds)
                {
                    _weeklyChilds = new List<ChildItem>();
                }
                if (0 == _weeklyChilds.Count)
                {
                    if (0 < ProfileManager.Instance.CurrentProfile?.Babies?.Count)
                    {
                        foreach (BabyModel baby in ProfileManager.Instance.CurrentProfile.Babies)
                        {
                            ChildItem item = new ChildItem()
                            {
                                Id = baby.Id,
                                Name = baby.Name
                            };
                            _weeklyChilds.Add(item);
                        }
                    }
                }
                return _weeklyChilds;
            }
            set
            {
                SetPropertyChanged(ref _weeklyChilds, value);
            }
        }

        #endregion

        #region Commands

        ICommand _addSessionCommand;
        public ICommand AddSessionCommand
        {
            get
            {
                _addSessionCommand = _addSessionCommand ?? new Command(() =>
                {
                    AddSessionAction?.Invoke();
                });
                return _addSessionCommand;
            }
        }

        ICommand _sessionSelectorUpdateCommand;
        public ICommand WeeklySessionSelectorUpdateCommand
        {
            get
            {
                _sessionSelectorUpdateCommand = _sessionSelectorUpdateCommand ?? new Command((obj) =>
                {
                    UpdateUIAction?.Invoke();
                });
                return _sessionSelectorUpdateCommand;
            }
        }

        ICommand _sessionSelectorCommand;
        public ICommand WeeklySessionSelectorCommand
        {
            get
            {
                _sessionSelectorCommand = _sessionSelectorCommand ?? new Command<SessionType>(ToggleWeeklySessionSelector);
                return _sessionSelectorCommand;
            }
        }

        ICommand _childsSelectorCommand;
        public ICommand WeeklyChildsSelectorCommand
        {
            get
            {
                _childsSelectorCommand = _childsSelectorCommand ?? new Command<int>(ToggleWeeklyChildsSelector);
                return _childsSelectorCommand;
            }
        }

        ICommand _bottleContentSelectorCommand;
        public ICommand WeeklyBottleContentSelectorCommand
        {
            get
            {
                _bottleContentSelectorCommand = _bottleContentSelectorCommand ?? new Command<SessionType>(ToggleWeeklyBottleSelector);
                return _bottleContentSelectorCommand;
            }
        }

        #endregion

        #region Private

        void ToggleWeeklySessionSelector(SessionType type)
        {
            SelectedSession = type;

            SetPropertyChanged(nameof(SelectedSession));

            ToggleSessionAction?.Invoke(SelectedSession);
        }

        void ToggleWeeklyChildsSelector(int index)
        {
            SelectedChildIndex = index;

            SetPropertyChanged(nameof(SelectedChildIndex));

            ToggleChildsAction?.Invoke(SelectedChildIndex);
        }

        void ToggleWeeklyBottleSelector(SessionType type)
        {
            SelectedBottleType = type;

            SetPropertyChanged(nameof(SelectedBottleType));

            ToggleBottleAction?.Invoke(SelectedBottleType);
        }

        #endregion
    }

    /// <summary>
    /// Data model for a row for the listview for each of the Tab in this history model
    /// </summary>
    public class HistoryRangeModelItem : ModelItemBase
    {
        /// <summary>
        /// Constructor -- initialies the default values
        /// </summary>
        public HistoryRangeModelItem()
        {
            MaxBarHeight = 100;
            IsHeaderVisible = true;
            BinsInfo = new List<String>();
            BinsAmountValue = new List<String>();
            BinsAmountText = new List<String>();
            BarHeights = new List<double>();
        }

        /// <summary>
        /// Gets/Sets whether the header is visible in the row
        /// </summary>
        public bool IsHeaderVisible { get; set; }

        /// <summary>
        /// Icon to show for the row
        /// </summary>
        public ImageSource Icon { get; set; }

        /// <summary>
        /// Title of the row
        /// </summary>
        public String Title { get; set; }

        /// <summary>
        /// Bins information for the row
        /// </summary>
        public List<String> BinsInfo { get; private set; }

        /// <summary>
        /// Bins amount for the row
        /// </summary>
        public List<String> BinsAmountValue { get; private set; }
        public List<String> BinsAmountText { get; private set; }
        /// <summary>
        /// Max bar height for the row
        /// </summary>
        public double MaxBarHeight { get; set; }

        /// <summary>
        /// Height for each fo the bars in the row
        /// </summary>
        public List<double> BarHeights { get; set; }
    }
}
