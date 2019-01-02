using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BabyationApp.Models;
using System.Collections.ObjectModel;
using SQLite;
using System.IO;
using BabyationApp.DataObjects;
using Xamarin.Forms;
using BabyationApp.Interfaces;

namespace BabyationApp.Managers
{
    public class ScheduleManager
    {
        SQLiteAsyncConnection _connection;

        static readonly Lazy<ScheduleManager> lazy = new Lazy<ScheduleManager>(() => new ScheduleManager());

        public static ScheduleManager Instance
        { 
            get 
            { 
                return lazy.Value; 
            } 
        }

        ILocalNotificationService _localNotificationService;
        ILocalNotificationService LocalNotificationService
        {
            get
            {
                if (_localNotificationService == null)
                {
                    _localNotificationService = DependencyService.Get<ILocalNotificationService>();
                }

                return _localNotificationService;
            }
        }

        public ObservableCollection<ReminderModel> Reminders
        {
            get;
            private set;
        }

        ScheduleManager() { }

        public void Initialize()
        {
            Task.Run(() =>
            {
                var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Babyation.db3");

                _connection = new SQLiteAsyncConnection(dbPath);
                _connection.CreateTableAsync<ReminderModel>().Wait();
            });
        }

        public Task<List<ReminderModel>> GetRemindersAsync() => _connection.Table<ReminderModel>().ToListAsync();

        public async Task AddReminderAsync(ReminderModel reminder)
        {
            // TODO: Replace text with app resources
            LocalNotificationService.Schedule("Pumping Session Reminder", "Tap here to begin your pumping session!", reminder.Id.ToString(), reminder.Time.Value);

            await _connection.InsertAsync(reminder);
        }

        public async Task RemoveReminderAsync(ReminderModel reminder)
        {
            LocalNotificationService.Cancel(reminder.Id.ToString());

            await _connection.DeleteAsync(reminder);
        }
    }
}
