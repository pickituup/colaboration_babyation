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
    public class Children : EntityData
    {
        /// @req SRS_LLR_BCK_DBS_0005
        /// Id Inherated from EntityData
        /// @req SRS_LLR_BCK_DBS_0006
        /// @req SRS_LLR_BCK_DBS_0007
        public string ProfileId { get; set; }
        public Profile Profile { get; set; }
        public string Name { get; set; }
        public DateTime Birthday { get; set; }
        public string MediaId { get; set; }
        public Media Media { get; set; }
    }
}