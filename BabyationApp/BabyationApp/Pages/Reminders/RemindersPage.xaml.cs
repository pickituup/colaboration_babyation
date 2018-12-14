using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BabyationApp.Controls.Views;
using BabyationApp.Interfaces;
using BabyationApp.Models;
using Xamarin.Forms;
using BabyationApp.Common;
using System.Windows.Input;

namespace BabyationApp.Pages.Reminders
{
    /// <summary>
    /// This class represents the Reminders page from the design
    /// </summary>
    public partial class RemindersPage : RootViewBase
    {
        public RemindersViewModel ViewModel { get; set; }

        public RemindersPage()
        {
            InitializeComponent();

            ViewModel = new RemindersViewModel(CreateReminder, RequestDeleteReminder, ConfirmDeleteReminder);
            BindingContext = ViewModel;

            Titlebar.IsVisible = true;
        }

        public override void AboutToShow()
        {
            base.AboutToShow();

            // Restore style:
            Titlebar.IsVisible = true;
            RootLayout.Style = (Style)Application.Current.Resources["AbsoluteLayout_NavigationOnTop"];

            ViewModel.Reset();
        }

        #region Private

        private void CreateReminder()
        {
            PageManager.Me.SetCurrentPage(typeof(CreateReminderPage));
        }

        private void RequestDeleteReminder()
        {
            ViewModel.ShowDeletePopupCommand?.Execute(this);
            RootLayout.RaiseChild(ConfirmationView);
        }

        private void ConfirmDeleteReminder(bool isDelete)
        {
            RootLayout.RaiseChild(MainPage);
            if(isDelete )
            {
                //TODO:
            }
            else
            {
                //TODO:
            }
        }

        #endregion
    }


    public class RemindersViewModel : ObservableObject
    {
        private Action CreateReminderAction { get; set; }
        private Action RequestDeleteReminderAction { get; set; }
        private Action<bool> ConfirmDeleteReminderAction { get; set; }

        public RemindersViewModel(Action createReminder, Action requestDeleteReminder, Action<bool> confirmDeleteReminder)
        {
            CreateReminderAction = createReminder;
            RequestDeleteReminderAction = requestDeleteReminder;
            ConfirmDeleteReminderAction = confirmDeleteReminder;
        }

        public void Reset()
        {
            ShowDeletePopup = false;

            Refresh();
        }

        #region Public UI properties

        private bool _showDeletePopup;
        public bool ShowDeletePopup
        {
            get => _showDeletePopup;
            set => SetPropertyChanged(ref _showDeletePopup, value);
        }

        #endregion

        #region Data properties

        public List<AlarmItem> Datasource { get; private set; }

        private bool _refreshing;
        public bool Refreshing
        {
            get => _refreshing;
            set => SetPropertyChanged(ref _refreshing, value);
        }

        #endregion

        #region Commands

        private ICommand _refreshCommand;
        public ICommand RefreshCommand
        {
            get
            {
                _refreshCommand = _refreshCommand ?? new Command(Refresh);
                return _refreshCommand;
            }
        }

        ICommand _addReminderCommand;
        public ICommand AddReminderCommand
        {
            get
            {
                _addReminderCommand = _addReminderCommand ?? new Command(CreateReminderAction);
                return _addReminderCommand;
            }
        }

        ICommand _toggleReminderCommand;
        public ICommand ToggleReminderCommand
        {
            get
            {
                _toggleReminderCommand = _toggleReminderCommand ?? new Command((obj) =>
                {
                    if (!obj.GetType().Equals(typeof(AlarmItem)))
                        return;

                    AlarmItem model = (AlarmItem)obj;
                    model.IsOn = !model.IsOn;

                    Datasource.Where(x => x.Id.Equals(model.Id)).Select(x => x.IsOn = model.IsOn);
                    SetPropertyChanged(nameof(Datasource));
                });
                return _toggleReminderCommand;
            }
        }

        ICommand _toggleAutostartCommand;
        public ICommand ToggleAutostartCommand
        {
            get
            {
                _toggleAutostartCommand = _toggleAutostartCommand ?? new Command((obj) =>
                {
                    if (!obj.GetType().Equals(typeof(AlarmItem)))
                        return;

                    AlarmItem model = (AlarmItem)obj;
                    model.IsAutoStart = !model.IsAutoStart;

                    SetPropertyChanged(nameof(Datasource));
                });
                return _toggleAutostartCommand;
            }
        }

        ICommand _deleteReminderCommand;
        public ICommand DeleteReminderCommand
        {
            get
            {
                _deleteReminderCommand = _deleteReminderCommand ?? new Command((obj) =>
                {
                    RequestDeleteReminderAction?.Invoke();
                });
                return _deleteReminderCommand;
            }
        }

        ICommand _showDeletePopupCommand;
        public ICommand ShowDeletePopupCommand
        {
            get
            {
                _showDeletePopupCommand = _showDeletePopupCommand ?? new Command(() =>
                {
                    ShowDeletePopup = true;
                });
                return _showDeletePopupCommand;
            }
        }

        ICommand _rejectDeletionCommand;
        public ICommand RejectDeletionCommand
        {
            get
            {
                _rejectDeletionCommand = _rejectDeletionCommand ?? new Command(() =>
                {
                    ShowDeletePopup = false;
                    ConfirmDeleteReminderAction?.Invoke(false);
                });

                return _rejectDeletionCommand;
            }
        }

        ICommand _confirmDeletionCommand;
        public ICommand ConfirmDeletionCommand
        {
            get
            {
                _confirmDeletionCommand = _confirmDeletionCommand ?? new Command(() =>
                {
                    ShowDeletePopup = false;
                    ConfirmDeleteReminderAction?.Invoke(true);
                });

                return _confirmDeletionCommand;
            }
        }

        #endregion

        #region Private

        private void Refresh()
        {
            Refreshing = true;

            Datasource = this.TempList();
            SetPropertyChanged(nameof(Datasource));

            Refreshing = false;
        }

        private List<AlarmItem> TempList()
        {
            List<AlarmItem> alarms = new List<AlarmItem>();
            alarms.Add(new AlarmItem()
            {
                IsOn = true,
                Date = DateTime.Now,

                ModeName = "To Pump",
                Description = "My morning commute",
                IsAutoStart = true
            });
            alarms.Add(new AlarmItem()
            {
                IsOn = true,
                Date = DateTime.Now.Subtract(TimeSpan.FromDays(2)),

                ModeName = "To Bottle",
                Description = "Nighttime 2-5a",
                IsAutoStart = true
            });

            return alarms;
        }

        #endregion
    }


    /// <summary>
    /// This class is the UI model for an alarm to be used in the clas RemindersPage
    /// </summary>
    public class AlarmItem : ModelItemBase
    {
        public AlarmItem()
        {
#if DEBUG
            Id = Guid.NewGuid().ToString();
#endif
            Date = DateTime.MinValue;
            SetPropertyChanged(nameof(IsOn));
        }

        public string Id { get; set; }

        /// <summary>
        /// Gets/Sets whether the alarm is active or not
        /// </summary>
        private bool _isOn;
        public bool IsOn
        { 
            get => _isOn; 
            set => SetPropertyChanged(ref _isOn, value); 
        }

        /// <summary>
        /// Time of the alarm
        /// </summary>
        private DateTime _date;
        public DateTime Date
        {
            get => _date;
            set
            {
                SetPropertyChanged(ref _date, value);
                SetPropertyChanged(nameof(DateAbbr));
            }
        }

        /// <summary>
        /// Time Description of the alarm
        /// </summary>
        public String DateAbbr { get => Date.ToString("tt"); }

        /// <summary>
        /// Icon of the alarm
        /// </summary>
        public ImageSource Icon { get; set; }

        /// <summary>
        /// Description of the alarm
        /// </summary>
        private String _description;
        public String Description 
        { 
            get => _description; 
            set => SetPropertyChanged(ref _description, value); 
        }

        /// <summary>
        /// Gets or sets the name of the mode.
        /// </summary>
        /// <value>The name of the mode.</value>
        private String _modeName;
        public String ModeName 
        { 
            get => _modeName; 
            set => SetPropertyChanged(ref _modeName, value);
        }

        private String _modeId;
        public String ModeId 
        { 
            get => _modeId; 
            set => SetPropertyChanged(ref _modeId, value);
        }

        /// <summary>
        /// Gets/Sets whether the alarm is a auto-start type or manual
        /// </summary>
        private bool _isAutoStart;
        public bool IsAutoStart 
        { 
            get => _isAutoStart;
            set => SetPropertyChanged(ref _isAutoStart, value);
        }
    }
}
