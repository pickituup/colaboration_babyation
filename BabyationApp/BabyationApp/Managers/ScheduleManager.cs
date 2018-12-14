using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BabyationApp.Models;
using System.Collections.ObjectModel;

namespace BabyationApp.Managers
{
    public class ScheduleManager
    {
        private ObservableCollection<ReminderModel> _reminders = new ObservableCollection<ReminderModel>();

        public void Initialize()
        {
            // Sync with backend
        }

        public void Shutdown()
        {
            // Make sure everything is synced with backend
        }

        public void AddReminder(ReminderModel reminder)
        {

        }

        public void RemoveReminder(ReminderModel reminder)
        {

        }

        public ObservableCollection<ReminderModel> Reminders
        {
            get
            {
                return _reminders;
            }
        }
    }
}
