using Microsoft.WindowsAzure.MobileServices;
using System.Collections.Generic;

namespace BabyationApp.DataObjects
{
    /// @class Profile
    /// @brief This class represents the Profile database table
    /// @req SRS_LLR_BCK_DBS_0036
    /// 
    /// More info...
    public class User : EntityData
    {
        /// @req SRS_LLR_BCK_DBS_0001
        /// Id Inherated from EntityData
        /// @req SRS_LLR_BCK_DBS_0002
        public string ActiveDirectoryObjectId { get; set; }
        public List<Profile> UserProfiles { get; set; }
        public List<Reminder> Reminders { get; set; }
        public string DefaultProfileId { get; set; }
        public string Name { get; set; }

    }
}