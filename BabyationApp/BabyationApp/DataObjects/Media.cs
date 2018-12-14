using AppServiceHelpers.Models;
using System.Collections.Generic;

namespace BabyationApp.DataObjects
{
    /// @class Profile
    /// @brief This class represents the Profile database table
    /// @req SRS_LLR_BCK_DBS_0036
    /// 
    /// More info...
    public class Media : EntityData
    {
        /// @req SRS_LLR_BCK_DBS_0008
        /// Id Inherated from EntityData
        /// @req SRS_LLR_BCK_DBS_0009
        /// @req SRS_LLR_BCK_DBS_0010
        public string ProfileId { get; set; }
        public Profile Profile { get; set; }
        public byte Type { get; set; }
        public byte[] Data { get; set; }
        //public string URL { get; set; }


    }
}