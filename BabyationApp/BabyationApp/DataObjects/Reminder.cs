using AppServiceHelpers.Models;
using System;
using System.Collections.Generic;

namespace BabyationApp.DataObjects
{
    /// @class Profile
    /// @brief This class represents the Profile database table
    /// @req SRS_LLR_BCK_DBS_0036
    /// 
    /// More info...
    public class Reminder : EntityData
    {
        /// @req SRS_LLR_BCK_DBS_0011
        /// Id Inherated from EntityData
        /// @req SRS_LLR_BCK_DBS_0012
        public string ProfileId { get; set; }
        public Profile Profile { get; set; }
        public List<User> Users { get; set; }
        public string Name { get; set; }
        public byte Type { get; set; }
        public byte AutoStart { get; set; }
        public string ExperienceId { get; set; }
        public Experience Experience { get; set; }
        public byte Frequency { get; set; }
        public int TimeOffset { get; set; }
        public DateTime Time { get; set; }
        public string MediaId { get; set; }
        public Media Media { get; set; }


    }
}