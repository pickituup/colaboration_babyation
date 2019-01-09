using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BabyationApp.Controls.Views;
using BabyationApp.Managers;
using BabyationApp.Models;
using Xamarin.Forms;
using BabyationApp.Common;
using System.Dynamic;
using System.Windows.Input;
using BabyationApp.Resources;

namespace BabyationApp.Pages.Reminders
{
    public partial class CreateReminderPage : PageBase
    {
        public CreateReminderModel ViewModel { get; set; }

        private AlarmItem alarmItem;

        public CreateReminderPage()
        {
            InitializeComponent();

            ViewModel = new CreateReminderModel(FinishSession);
            BindingContext = ViewModel;

            Titlebar.IsVisible = true;
            LeftPageType = typeof(DashboardTabPage);
            Titlebar.LeftButton.IsVisible = true;
            Titlebar.LeftButton.SetDynamicResource(StyleProperty, "CancelButton");
            Titlebar.LeftButton.Clicked += CancelButton_Clicked;
        }

        public void Initialize(AlarmItem item)
        {
            if (null != item)
            {
                alarmItem = item;
            }
        }

        public override void AboutToShow()
        {
            base.AboutToShow();

            if (null == alarmItem)
            {
                alarmItem = new AlarmItem();

                ViewModel.Reset();
//#if DEBUG
//                alarmItem.Description = "Megamode pumper";
//                alarmItem.Date = DateTime.Now;
//                alarmItem.IsAutoStart = true;
//#endif

                ViewModel.AlarmItem = alarmItem;
            }
            else
            {
                ViewModel.AlarmItem = alarmItem;
            }
        }

        void CancelButton_Clicked(object sender, EventArgs e)
        {
            // Cleanup
            alarmItem = null;
            ViewModel.AlarmItem = null;
        }

        #region Private

        private void FinishSession(AlarmItem item)
        {
            PageManager.Me.SetCurrentPage(typeof(ChooseModePage), View => (View as ChooseModePage).Initialize(item));
        }

        #endregion
    }



    public class CreateReminderModel : ObservableObject
    {
        private Action<AlarmItem> FinishSessionAction { get; set; }

        public DateTime MinimumDate => DateTime.Now.Subtract(TimeSpan.FromDays(14));

        public CreateReminderModel(Action<AlarmItem> finishSessionAction)
        {
            FinishSessionAction = finishSessionAction;

            Reset();
        }

        public void Reset()
        {
            Nickname = null;
            Date = DateTime.MinValue;
            Time = TimeSpan.Zero;

            SetPropertyChanged(nameof(MaxChars));
        }

        #region Data properties

        private AlarmItem _alarmItem;
        public AlarmItem AlarmItem
        {
            get => _alarmItem;
            set
            {
                _alarmItem = value;
                if (null != _alarmItem )
                {
                    Date = _alarmItem.Date;
                    Time = DateTime.MinValue == _alarmItem.Date ? TimeSpan.Zero : _alarmItem.Date.TimeOfDay;
                    Nickname = _alarmItem.Description;
                    AutoStart = _alarmItem.IsAutoStart;
                }
            }
        }

        private string _nickname;
        public string Nickname
        {
            get => _nickname;
            set
            {
                if (SetPropertyChanged(ref _nickname, value))
                {
                    SetPropertyChanged(nameof(CharsLeft));
                    SetPropertyChanged(nameof(IsReadyToGo));
                }
            }
        }

        public int MaxChars => 25;

        public string CharsLeft
        {
            get
            {
                int textLen = Nickname?.Length ?? 0;

                return int.MaxValue == MaxChars ? "" : String.Format("({0} {1})", (MaxChars - textLen), AppResource.CharactersLeft);
            }
        }

        public string DateFormat => "MM/dd/yyyy";
        public string DateValue
        {
            get
            {
                if (DateTime.MinValue == Date)
                {
                    return AppResource.CommonPlaceholderDelimiter; // __/__
                }
                return Date.ToString(DateFormat);
            }
        }
        private DateTime _date;
        public DateTime Date
        {
            get => _date;
            set
            {
                if (SetPropertyChanged(ref _date, value))
                {
                    SetPropertyChanged(nameof(IsReadyToGo));
                    SetPropertyChanged(nameof(DateValue));
                }
            }
        }

        public string TimeFormat => "h:mm";
        public string TimeValue
        {
            get
            {
                if (TimeSpan.Zero == Time)
                {
                    return AppResource.CommonPlaceholderDelimiter; //__/__
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
                    SetPropertyChanged(nameof(IsReadyToGo));
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

        private bool _autoStart;
        public bool AutoStart
        {
            get => _autoStart;
            set => SetPropertyChanged(ref _autoStart, value);
        }

        public bool IsReadyToGo
        {
            get => !String.IsNullOrEmpty(Nickname) && Date != DateTime.MinValue && Time != TimeSpan.Zero;
        }

        #endregion

        #region Commands

        ICommand _toggleAutostartCommand;
        public ICommand ToggleAutostartCommand
        {
            get
            {
                _toggleAutostartCommand = _toggleAutostartCommand ?? new Command(() =>
                {
                    AutoStart = !AutoStart;
                });
                return _toggleAutostartCommand;
            }
        }

        ICommand _chooseModeCommand;
        public ICommand ChooseModeCommand
        {
            get
            {
                // Short circuiting the timer
                _chooseModeCommand = _chooseModeCommand ?? new Command(() =>
                {
                    AlarmItem.Description = Nickname;

                    long tics = Date.Ticks + Time.Ticks;
                    AlarmItem.Date = new DateTime(tics);
                    AlarmItem.IsAutoStart = AutoStart;

                    FinishSessionAction?.Invoke(AlarmItem);
                });

                return _chooseModeCommand;
            }
        }

        #endregion
    }
}
