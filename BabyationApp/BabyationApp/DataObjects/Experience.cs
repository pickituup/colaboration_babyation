using AppServiceHelpers.Models;
using System.Collections.Generic;

namespace BabyationApp.DataObjects
{
    public class Experience : EntityData
    {
        public string ProfileId { get; set; }
        public Profile Profile { get; set; }
        public int Duration { get; set; }
        public int DelayStart { get; set; }
        public byte Breast { get; set; }
        public byte StimulationSpeed { get; set; }
        public byte StimulationSuction { get; set; }
        public byte ExpressionSpeed { get; set; }
        public byte ExpressionSuction { get; set; }
        public byte TransistionType { get; set; }
        public byte Storage { get; set; }
        public string MediaId { get; set; }
        public Media Media { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int TransistionTime { get; set; }
        public byte ExperienceId { get; set; }
    }
}