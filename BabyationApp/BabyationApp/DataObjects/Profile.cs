using AppServiceHelpers.Models;
using System.Collections.Generic;

namespace BabyationApp.DataObjects
{
    /// @class Profile
    /// @brief This class represents the Profile database table
    /// @req SRS_LLR_BCK_DBS_0036
    /// 
    /// More info...
    public class Profile : EntityData
    {
        /// @req SRS_LLR_BCK_DBS_0001
        /// Id Inherated from EntityData
        /// @req SRS_LLR_BCK_DBS_0002
        public User PrimaryUser { get; set; }
        public string PrimaryUserId { get; set; }
        public List<User> ProfileUsers { get; set; }
        public List<Experience> Experiences { get; set; }
        public List<Children> Children { get; set; }
        public List<Reminder> Reminders { get; set; }
        public List<Pump> Pumps { get; set; }
        public List<HistoricalSession> HistoricalSessions { get; set; }
        public string SelectedPumpId { get; set; }
        public string SelectedChildId { get; set; }
    }
}