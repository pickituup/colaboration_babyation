using AppServiceHelpers.Models;
using System;
using System.Collections.Generic;

namespace BabyationApp.DataObjects
{
    /// @class Firmware
    /// @brief This class represents the Firmware database table

    public class Firmware : EntityData
    {
        public int FirmwareVersion { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime ReleaseDate { get; set; }
        public byte[] Binary { get; set; }
    }
}