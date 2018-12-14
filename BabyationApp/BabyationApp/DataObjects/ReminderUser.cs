using AppServiceHelpers.Models;
using System.Collections.Generic;

namespace BabyationApp.DataObjects
{
    /// @class Profile
    /// @brief This class represents the Profile database table
    /// @req SRS_LLR_BCK_DBS_0036
    /// 
    /// More info...
    public class ReminderUser : EntityData
    {
        public string ReminderId { get; set; }
        public Reminder Reminder { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
    }
}