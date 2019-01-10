using AppServiceHelpers.Models;
using System;
using System.Collections.Generic;
using BabyationApp.Helpers;

namespace BabyationApp.DataObjects
{
    public class HistoricalSession :EntityData
    {   
        public string ProfileId { get; set; }
        public Profile Profile { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public byte Type { get; set; }
        public DateTime SessionStart { get; set; } = ExtensionMethods.DefaultDateTime;
        public DateTime SessionEnd { get; set; } = ExtensionMethods.DefaultDateTime;
        public double TotalVolume { get; set; }
        public DateTime Expires { get; set; } = ExtensionMethods.DefaultDateTime;
        public byte Available { get; set; }
        public DateTime LeftBreastStart { get; set; } = ExtensionMethods.DefaultDateTime;
        public DateTime LeftBreastEnd { get; set; } = ExtensionMethods.DefaultDateTime;
        public double LeftBreastVolume { get; set; }
        public DateTime RightBreastStart { get; set; } = ExtensionMethods.DefaultDateTime;
        public DateTime RightBreastEnd { get; set; } = ExtensionMethods.DefaultDateTime;
        public double RightBreastVolume { get; set; }
        public string Notes { get; set; }
        public byte Preferred { get; set; }
        public byte StorageType { get; set; }
        public byte MilkType { get; set; }
        public string ChildId { get; set; }
        public string FeedByProfileId { get; set; }
    }
}