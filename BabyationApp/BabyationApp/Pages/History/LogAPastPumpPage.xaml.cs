using BabyationApp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BabyationApp.Controls.Buttons;
using Xamarin.Forms;
using BabyationApp.Helpers;
using BabyationApp.Managers;
using BabyationApp.Models;
using System.Diagnostics;
using System.Windows.Input;
using BabyationApp.Resources;

namespace BabyationApp.Pages.History
{

    /// <summary>
    /// This class represents the Log a Past Pump page from the design
    /// </summary>
    public partial class LogAPastPumpPage : PageBase
    {
        DeviceTimer _timer = new DeviceTimer();

        private LogPastPumpModel _logPastPumpModel;

        ButtonExGroup _btnGroup = new ButtonExGroup();

        private DateTime _selectedDate = new DateTime();

        private TimeSpan _selectedStartTime = new TimeSpan();

        private Action _checkInputStatus;

        /// <summary>
        /// History model for which this page needs to log a past pump
        /// </summary>
        HistoryModel _historyModel = null;
        public HistoryModel HistorySession
        {
            get => _historyModel;
            set { _historyModel = value; }
        }

        /// <summary>
        /// Constructor -- Initialize the model and binds buttons events and other ui actions
        /// </summary>
        public LogAPastPumpPage()
        {
            InitializeComponent();

            // Moved from AddSession page
            //
            if( null == HistorySession )
            {
                HistorySession = HistoryManager.Instance.CreateSession(SessionType.Pump);
            }

            BindingContext = _logPastPumpModel = new LogPastPumpModel();

            var updateTime = new Action(() =>
            {
                if (HistorySession != null)
                {
                    if (_selectedDate + _selectedStartTime >= DateTime.Now)
                    {
                        ModalAlertPage.ShowAlertWithClose("You are picking a future date");
                        return;
                    }

                    HistorySession.StartTime = _selectedDate + _selectedStartTime;
                }
            });

            LogPumpingDate.CalendarDate = DateTime.Now;
            LogPumpingDate.AfterAction = () =>
            {
                updateTime();
                _checkInputStatus();
            };
            LogPumpingDate.ValidationFunc = (dateToValidate) =>
            {
                if (dateToValidate > DateTime.Now.Date)
                {
                    ModalAlertPage.ShowAlertWithClose("You are picking a future date");
                    return false;
                }

                _selectedDate = dateToValidate;

                return true;
            };

            LogPumpingStartTime.AfterAction = () =>
            {
                updateTime();
                _checkInputStatus();
            };
            LogPumpingStartTime.ValidationFunc = (timeValidate) =>
            {
                if (_selectedDate + timeValidate >= DateTime.Now)
                {
                    ModalAlertPage.ShowAlertWithClose("You are picking a future time");
                    return false;
                }

                _selectedStartTime = timeValidate;

                return true;
            };

            _checkInputStatus = new Action(() =>
            {
                bool isValid = LogPumpingStartTime.Time.HasValue && LogPumpingDate.Date.HasValue && _logPastPumpModel.ValidateForm();
                _logPastPumpModel.NotReadyToSave = !isValid;
            });

            BtnSave.Clicked += (s, e) =>
            {
                if (HistorySession != null)
                {
                    HistorySession.EndTime = GetEndTime();
                    HistorySession.RightBreastMilkVolume = Convert.ToDouble(_logPastPumpModel.AmountRight);
                    HistorySession.LeftBreastMilkVolume = Convert.ToDouble(_logPastPumpModel.AmountLeft);
                    HistorySession.TotalMilkVolume = Convert.ToDouble(_logPastPumpModel.TotalAmount);
                    HistorySession.Storage = _logPastPumpModel.MilkType;

                    HistoryManager.Instance.AddSession(HistorySession);
                }

                _logPastPumpModel.ShowSavedPopupPage = true;

                UpdateTitlebarInfo(false, Color.FromHex("#11442B"));

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
        }

        DateTime GetEndTime()
        {
            string[] parts = _logPastPumpModel.TotalDuration.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length >= 2)
            {
                return HistorySession.StartTime.AddMinutes(Convert.ToInt32(parts[0])).AddSeconds(Convert.ToInt32(parts[1]));
            }

            return default(DateTime);
        }

        /// <summary>
        /// Gets called when this page is about to show and performs the initialization
        /// </summary>
        public override void AboutToShow()
        {
            if (BindingContext is LogPastPumpModel model)
            {
                model.MilkType = StorageType.Fridge;
                _selectedDate = new DateTime();
                _selectedStartTime = new TimeSpan();
                LogPumpingDate.Date = null;
                LogPumpingStartTime.Time = null;
                model.ShowSavedPopupPage = false;
                model.NotReadyToSave = true;
                model.TotalAmount = string.Empty;
                model.AmountRight = string.Empty;
                model.AmountLeft = string.Empty;
                model.TotalDuration = string.Empty;
            }
            else
            {
                model = new LogPastPumpModel();
            }


            Titlebar.TitleBackColor = Color.FromHex("#F9DCD9");
            Titlebar.IsVisible = true;

            base.AboutToShow();
        }

        private void CheckValidationForm(object sender, EventArgs e)
        {
            _checkInputStatus();
        }
    }

    /// <summary>
    /// This class is the UI model for the LogAPastPumpPage
    /// </summary>
    public class LogPastPumpModel : ObservableObject
    {
        public string TimeFormat => "h:mm";
        public string TimeValue
        {
            get
            {
                if (TimeSpan.Zero == Time)
                {
                    return AppResource.TimeDelimiter2; //__:__
                }
                return new DateTime(Time.Ticks).ToString(TimeFormat);
            }
        }

        private TimeSpan _time;
        public TimeSpan Time
        {
            get => _time;
            set
            {
                if (SetPropertyChanged(ref _time, value))
                {
                    SetPropertyChanged(nameof(TimeAbbr));
                    SetPropertyChanged(nameof(TimeValue));
                }
            }
        }

        public string TimeAbbr
        {
            get
            {
                return TimeSpan.Zero == Time ? null : new DateTime(Time.Ticks).ToString("tt").ToLower(); //AM or PM
            }
        }

        bool _resetMilkTypeControl;
        public bool ResetMilkTypeControl
        {
            get => _resetMilkTypeControl;
            set
            {
                SetPropertyChanged(ref _resetMilkTypeControl, value);
                SetPropertyChanged(nameof(ResetMilkTypeControl));
            }
        }

        ICommand _milkTypeCommand;
        public ICommand MilkTypeCommand
        {
            get
            {
                _milkTypeCommand = _milkTypeCommand ?? new Command<StorageType>(ToggleMilkType);
                return _milkTypeCommand;
            }
        }

        private void ToggleMilkType(StorageType type)
        {
            MilkType = type;

            SetPropertyChanged(nameof(MilkType));
        }

        private StorageType _milkType;
        public StorageType MilkType
        {
            get => _milkType;
            set
            {
                SetPropertyChanged(ref _milkType, value);
            }
        }

        string _amountRight;
        public string AmountRight
        {
            get { return _amountRight; }
            set
            {
                if (SetPropertyChanged(ref _amountRight, value))
                {
                    UpdateTotalAmount();
                }
            }
        }

        string _amountLeft;
        public string AmountLeft
        {
            get { return _amountLeft; }
            set
            {
                if (SetPropertyChanged(ref _amountLeft, value))
                {
                    UpdateTotalAmount();
                }
            }
        }

        string _totalAmount;
        public string TotalAmount
        {
            get { return _totalAmount; }
            set { SetPropertyChanged(ref _totalAmount, value); }
        }

        /// <summary>
        /// Gets/Sets whether all input is done and the page is ready to save
        /// </summary>
        bool _notReadyToSave = true;
        public bool NotReadyToSave
        {
            get => _notReadyToSave;
            set => SetPropertyChanged(ref _notReadyToSave, value);
        }

        /// <summary>
        /// Gets/Sets whether to show the saved popup
        /// </summary>
        bool _showSavedPopupPage = false;
        public bool ShowSavedPopupPage
        {
            get => _showSavedPopupPage;
            set => SetPropertyChanged(ref _showSavedPopupPage, value);
        }

        string _totalDuration;
        public string TotalDuration
        {
            get { return _totalDuration; }
            set
            {
                SetPropertyChanged(ref _totalDuration, value);
            }
        }

        public ICommand ParseTotalDurationCommand => new Command(() => ParseTotalDuration());

        /// <summary>
        ///     ctor().
        /// </summary>
        public LogPastPumpModel()
        {

        }

        ~LogPastPumpModel()
        {

        }

        //private void UpdateTotalAmount()
        //{
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(AmountLeft) && !string.IsNullOrEmpty(AmountRight))
        //        {
        //            double amountLeft = Convert.ToDouble(AmountLeft);
        //            double amountRight = Convert.ToDouble(AmountRight);

        //            TotalAmount = (amountLeft + amountRight).ToString();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine($"ERROR:{ex.Message}");
        //        Debugger.Break();
        //    }
        //}

        private void UpdateTotalAmount()
        {
            try
            {
                double leftAmmount = 0;
                double rightAmmount = 0;

                if (!double.TryParse(AmountLeft, out leftAmmount))
                {
                    leftAmmount = 0;
                }

                if (!double.TryParse(AmountRight, out rightAmmount))
                {
                    rightAmmount = 0;
                }

                double totalAmount = leftAmmount + rightAmmount;

                TotalAmount = totalAmount == 0 ? "00.00" : totalAmount.ToString();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR:{ex.Message}");
                Debugger.Break();
            }
        }

        private void ParseTotalDuration()
        {
            string[] splitedString = TotalDuration.Split(':');
            string res = string.Empty;
            if (splitedString.Length > 1)
            {
                if (int.TryParse(splitedString[0], out int ounces) && int.TryParse(splitedString[1], out int parts))
                {
                    res = string.Format($"{ounces:D2}:{parts:D2}");
                }
            }
            else
            {
                if (int.TryParse(splitedString[0], out int ounces))
                {
                    res = string.Format($"{ounces:D2}:00");
                }
            }

            TotalDuration = res;
        }

        public bool ValidateForm()
        {
            bool isValid = !string.IsNullOrEmpty(TotalAmount) &&
                           !string.IsNullOrEmpty(TotalDuration) &&
                           !string.IsNullOrEmpty(AmountLeft) &&
                           !string.IsNullOrEmpty(AmountRight);

            return isValid;
        }
    }
}
































//using BabyationApp.Common;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Text;
//using System.Threading.Tasks;
//using BabyationApp.Controls.Buttons;
//using Xamarin.Forms;
//using BabyationApp.Helpers;
//using BabyationApp.Managers;
//using BabyationApp.Models;


//namespace BabyationApp.Pages.History
//{

//    /// <summary>
//    /// This class represents the Log a Past Pump page from the design
//    /// </summary>
//    public partial class LogAPastPumpPage : PageBase
//    {

//        DeviceTimer _timer = new DeviceTimer();

//        private bool _isEditMode;
//        private LogPastPumpModel _logPastPumpModel;

//        ButtonExGroup _btnGroup = new ButtonExGroup();

//        private DateTime _selectedDate = new DateTime();

//        private TimeSpan _selectedStartTime = new TimeSpan();

//        /// <summary>
//        /// Constructor -- Initialize the model and binds buttons events and other ui actions
//        /// </summary>
//        public LogAPastPumpPage()
//        {
//            InitializeComponent();

//            BindingContext = _logPastPumpModel = new LogPastPumpModel();

//            if (Device.RuntimePlatform == Device.iOS)
//            {
//                EntryOuncesPumped.WidthRequest = 150;
//            }

//            _rlButtons.SizeChanged += OnLOgPumpRL_SizeChanged;

//            //var _logPastPumpModel = new LogPastPumpModel()
//            //{
//            //    NotReadyToSave = true,
//            //    ShowSavedPopupPage = false
//            //};


//            var updateTime = new Action(() =>
//            {
//                if (HistorySession != null)
//                {
//                    if (InputValidator.IsValidInput(LblDtPicker.Text) && InputValidator.IsValidInput(LblTmPickerStart.Text))
//                    {
//                        HistorySession.StartTime = _selectedDate + _selectedStartTime;
//                    }

//                    if (InputValidator.IsValidInput(LblDtPicker.Text) && InputValidator.IsValidInput(LblTmPickerStart.Text) &&
//                        InputValidator.IsValidInput(LblDuraPicker.Text))
//                    {
//                        try
//                        {
//                            string[] parts = LblDuraPicker.Text.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
//                            if (parts.Length >= 2)
//                            {
//                                HistorySession.EndTime = HistorySession.StartTime.AddHours(Convert.ToInt32(parts[0]));
//                                HistorySession.EndTime = HistorySession.EndTime.AddMinutes(Convert.ToInt32(parts[1]));
//                            }
//                        }
//                        catch { }
//                    }
//                }
//            });


//            var checkInputStatus = new Action(() =>
//            {
//                _logPastPumpModel.NotReadyToSave = !InputValidator.IsValidInput(LblDtPicker.Text) ||
//                                         !InputValidator.IsValidInput(LblTmPickerStart.Text) ||
//                                         !InputValidator.IsValidInput(LblDuraPicker.Text) ||
//                                         !InputValidator.IsValidInput(LblTotalOunces.Text);
//            });

//            BtnDtPicker.Clicked += (sender, args) => DtPicker.Focus();
//            DtPicker.OKClicked += (sender, args) =>
//            {
//                if (DtPicker.Date.Date > DateTime.Now.Date)
//                {
//                    ModalAlertPage.ShowAlertWithClose("You are picking a future date");
//                    return;
//                }

//                if (InputValidator.IsValidInput(LblTmPickerStart.Text) && DtPicker.Date.Date + _selectedStartTime >= DateTime.Now)
//                {
//                    ModalAlertPage.ShowAlertWithClose("You are picking a future time");
//                    return;
//                }

//                if (InputValidator.IsValidInput(LblTmPickerStart.Text) && InputValidator.IsValidInput(LblDuraPicker.Text))
//                {
//                    try
//                    {
//                        string[] parts = LblDuraPicker.Text.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
//                        if (parts.Length >= 2)
//                        {
//                            var date = DtPicker.Date.Date + _selectedStartTime;
//                            date = date.AddHours(Convert.ToInt32(parts[0]));
//                            date = date.AddMinutes(Convert.ToInt32(parts[1]));
//                            if (date >= DateTime.Now)
//                            {
//                                ModalAlertPage.ShowAlertWithClose("You are picking a future time");
//                                return;
//                            }
//                        }
//                    }
//                    catch { }
//                }

//                LblDtPicker.Text = DtPicker.Date.ToDateString();
//                _selectedDate = DtPicker.Date.Date;
//                updateTime();
//                checkInputStatus();
//            };

//            BtnTmPicker.Clicked += (sender, args) => TmPickerStart.Focus();
//            TmPickerStart.OKClicked += (sender, e) =>
//            {
//                if (InputValidator.IsValidInput(LblDtPicker.Text) && _selectedDate + TmPickerStart.Time >= DateTime.Now)
//                {
//                    ModalAlertPage.ShowAlertWithClose("You are picking a future time");
//                    return;
//                }

//                if (InputValidator.IsValidInput(LblDtPicker.Text) && InputValidator.IsValidInput(LblDuraPicker.Text))
//                {
//                    try
//                    {
//                        string[] parts = LblDuraPicker.Text.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
//                        if (parts.Length >= 2)
//                        {
//                            var date = DtPicker.Date.Date + TmPickerStart.Time;
//                            date = date.AddHours(Convert.ToInt32(parts[0]));
//                            date = date.AddMinutes(Convert.ToInt32(parts[1]));
//                            if (date >= DateTime.Now)
//                            {
//                                ModalAlertPage.ShowAlertWithClose("You are picking a future time");
//                                return;
//                            }
//                        }
//                    }
//                    catch { }
//                }

//                LblTmPickerStart.Text = TmPickerStart.Time.ToTimeString(false);
//                _selectedStartTime = TmPickerStart.Time;
//                updateTime();
//                checkInputStatus();
//            };

//            BtnDuraPicker.Clicked += (sender, args) => DuraPicker.Focus();
//            DuraPicker.OKClicked += (sender, e) =>
//            {
//                if (InputValidator.IsValidInput(LblDtPicker.Text) && InputValidator.IsValidInput(LblTmPickerStart.Text))
//                {
//                    var date = _selectedDate + _selectedStartTime;
//                    date = date.AddHours(DuraPicker.Time.Hours);
//                    date = date.AddMinutes(DuraPicker.Time.Minutes);
//                    if (date >= DateTime.Now)
//                    {
//                        ModalAlertPage.ShowAlertWithClose("You are picking a future time");
//                        return;
//                    }
//                }
//                LblDuraPicker.Text = DuraPicker.Time.ToDurationString();
//                updateTime();
//                checkInputStatus();
//            };

//            _btnGroup.AddButton(BtnBoth);
//            _btnGroup.AddButton(BtnLeft);
//            _btnGroup.AddButton(BtnRight);

//            _btnGroup.Toggled += (sender, item, index) =>
//            {
//                String ounce = String.Empty;
//                EntryOuncesPumped.IsEnabled = item != null;
//                if (item == BtnBoth && HistorySession.TotalMilkVolume > 0.0)
//                {
//                    ounce = String.Format("{0:F1}", new object[] { HistorySession.TotalMilkVolume });
//                }
//                else if (item == BtnLeft && HistorySession.LeftBreastMilkVolume > 0.0)
//                {
//                    ounce = String.Format("{0:F1}", new object[] { HistorySession.LeftBreastMilkVolume });
//                }
//                else if (item == BtnRight && HistorySession.RightBreastMilkVolume > 0.0)
//                {
//                    ounce = String.Format("{0:F1}", new object[] { HistorySession.RightBreastMilkVolume });
//                }

//                EntryOuncesPumped.Text = InputValidator.ValidateInput(ounce);
//                checkInputStatus();
//            };

//            EntryOuncesPumped.Completed += (sender, args) =>
//            {
//                if (BtnLeft.IsToggled)
//                {
//                    if (HistorySession != null && InputValidator.IsValidInput(EntryOuncesPumped.Text))
//                    {
//                        try
//                        {
//                            HistorySession.LeftBreastMilkVolume = Convert.ToDouble(EntryOuncesPumped.Text);
//                        }
//                        catch
//                        {
//                        }
//                    }
//                }

//                if (BtnRight.IsToggled)
//                {
//                    if (HistorySession != null && InputValidator.IsValidInput(EntryOuncesPumped.Text))
//                    {
//                        try
//                        {
//                            HistorySession.RightBreastMilkVolume = Convert.ToDouble(EntryOuncesPumped.Text);
//                        }
//                        catch
//                        {
//                        }
//                    }
//                }

//                if (BtnBoth.IsToggled)
//                {
//                    if (HistorySession != null && InputValidator.IsValidInput(EntryOuncesPumped.Text))
//                    {
//                        try
//                        {
//                            HistorySession.TotalMilkVolume = Convert.ToDouble(EntryOuncesPumped.Text);
//                        }
//                        catch
//                        {
//                        }
//                    }
//                }
//                checkInputStatus();
//            };

//            BtnSave.Clicked += (s, e) =>
//            {
//                if (HistorySession != null)
//                {
//                    HistoryManager.Instance.AddSession(HistorySession);
//                }

//                _logPastPumpModel.ShowSavedPopupPage = true;
//                UpdateTitlebarInfo(false, Color.FromHex("#11442B"));

//                _timer.Enable = true;
//                _timer.Start(() =>
//                {
//                    PageManager.Me.SetCurrentPage(typeof(DashboardTabPage));
//                    return false;
//                });
//            };

//            Titlebar.IsVisible = true;
//            LeftPageType = typeof(DashboardTabPage);
//            Titlebar.LeftButton.IsVisible = true;
//            Titlebar.LeftButton.SetDynamicResource(StyleProperty, "CancelButton");
//            Titlebar.RightButton.IsVisible = true;
//            Titlebar.RightButton.Text = "HELP";
//        }

//        private HistoryModel _historyModel = null;

//        /// <summary>
//        /// History model for which this page needs to log a past pump
//        /// </summary>
//        public HistoryModel HistorySession
//        {
//            get
//            {
//                return _historyModel;
//            }
//            set
//            {
//                if (_historyModel != null)
//                {
//                    _historyModel.PropertyChanged -= _historyModel_PropertyChanged;
//                }
//                _historyModel = value;
//                if (_historyModel != null)
//                {
//                    _historyModel.PropertyChanged += _historyModel_PropertyChanged;
//                }
//            }
//        }

//        /// <summary>
//        /// Gets/Sets whether the page is edit mode (if true) or creation mode
//        /// </summary>
//        public bool IsEditMode
//        {
//            get { return _isEditMode; }
//            set
//            {
//                if (value != _isEditMode)
//                {
//                    _isEditMode = value;
//                    Title = "LOG A PAST PUMP";
//                }
//            }
//        }

//        private void _historyModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
//        {
//            if (e.PropertyName == "TotalMilkVolume")
//            {
//                if (HistorySession.TotalMilkVolume > 0.0)
//                {
//                    LblTotalOunces.Text = InputValidator.ValidateInput(_historyModel.TotalMilkVolume.ToString());
//                }
//                else
//                {
//                    LblTotalOunces.Text = "--.--";
//                }
//            }
//        }

//        /// <summary>
//        /// Gets called when this page is about to show and performs the initialization
//        /// </summary>
//        public override void AboutToShow()
//        {
//            //switch (HistorySession.SessionType)
//            //{
//            //    case SessionType.Pump:
//            //        Title = "LOG A PAST PUMP";
//            //        break;
//            //    case SessionType.Breastmilk:
//            //        Title = "LOG A PAST FEED";
//            //        break;
//            //    case SessionType.Nurse:
//            //        Title = "LOG A PAST NURSE";
//            //        break;
//            //}

//            if (BindingContext is LogPastPumpModel model)
//            {
//                model.ShowSavedPopupPage = false;
//                model.NotReadyToSave = true;
//            }
//            else
//            {
//                model = new LogPastPumpModel();
//            }

//            //Titlebar.TitleBackColor = Color.FromHex("#F9DCD9");
//            Titlebar.IsVisible = true;
//            BtnLeft.IsToggled = false;
//            BtnRight.IsToggled = false;
//            BtnBoth.IsToggled = false;
//            EntryOuncesPumped.IsEnabled = false;
//            base.AboutToShow();

//            DtPicker.Date = DateTime.Now;
//            LblDtPicker.Text = "--/--";
//            LblTmPickerStart.Text = "--:--";
//            LblTotalOunces.Text = "--.--";
//            LblDuraPicker.Text = "--:--";
//            EntryOuncesPumped.Text = String.Empty;
//        }

//        /// <summary>
//        /// Update the UI controls position/size when the page layout/size changes
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        private void OnLOgPumpRL_SizeChanged(object sender, EventArgs e)
//        {
//            var W = _rlButtons.Width * 3.2 / 10;
//            var H = W;

//            if (W <= 0) return;

//            var w = W * 3.2 / 4;
//            var h = H * 3.2 / 4;

//            var x1 = (_rlButtons.Width - w) / 2;
//            var y1 = 10;
//            var c1 = BoundsConstraint.FromExpression((Expression<Func<Rectangle>>)(() => new Rectangle(x1, y1, w, h)));
//            RelativeLayout.SetBoundsConstraint(BtnBoth, c1);

//            var x2 = x1 - w - 10;
//            var y2 = y1 + h / 2 + 10;
//            var c2 = BoundsConstraint.FromExpression((Expression<Func<Rectangle>>)(() => new Rectangle(x2, y2, w, h)));
//            RelativeLayout.SetBoundsConstraint(BtnLeft, c2);

//            var x3 = x1 + w + 10;
//            var y3 = y2;
//            var c3 = BoundsConstraint.FromExpression((Expression<Func<Rectangle>>)(() => new Rectangle(x3, y3, w, h)));
//            RelativeLayout.SetBoundsConstraint(BtnRight, c3);
//            _rlButtons.ForceLayout();
//        }
//    }

//    /// <summary>
//    /// This class is the UI model for the LogAPastPumpPage
//    /// </summary>
//    public class LogPastPumpModel : ObservableObject
//    {
//        /// <summary>
//        /// Gets/Sets whether all input is done and the page is ready to save
//        /// </summary>
//        bool _notReadyToSave = true;
//        public bool NotReadyToSave
//        {
//            get => _notReadyToSave;
//            set => SetPropertyChanged(ref _notReadyToSave, value);
//        }

//        /// <summary>
//        /// Gets/Sets whether to show the saved popup
//        /// </summary>
//        bool _showSavedPopupPage = false;
//        public bool ShowSavedPopupPage
//        {
//            get => _showSavedPopupPage;
//            set => SetPropertyChanged(ref _showSavedPopupPage, value);
//        }

//        /// <summary>
//        ///     ctor().
//        /// </summary>
//        public LogPastPumpModel()
//        {

//        }
//    }
//}

