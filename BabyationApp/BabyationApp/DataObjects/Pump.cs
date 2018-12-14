using AppServiceHelpers.Models;
using System.Collections.Generic;

namespace BabyationApp.DataObjects
{
    /// @class Profile
    /// @brief This class represents the Profile database table
    /// @req SRS_LLR_BCK_DBS_0036
    /// 
    /// More info...
    public class Pump : EntityData
    {
        /// @req SRS_LLR_BCK_DBS_0003
        /// Id Inherated from EntityData
        /// @req SRS_LLR_BCK_DBS_0004
        public string ProfileId { get; set; }
        public Profile Profile { get; set; }
        public string Name { get; set; }
        public string AuthenticationToken { get; set; }
        public string Identifier { get; set; }
        public bool InUse { get; set; }

    }
}