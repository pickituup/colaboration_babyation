using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabyationApp.Models
{
    public enum LogEntryType
    {
        PumpSessionStart = 0,
        PumpSessionEnd,
        PumpSessionPause,
        PumpSessionResume,
        BluetoothConnected,
        BluetoothDisconnected,
        TimeSet
    };

    public class LogEntryModel
    {       
        public uint Uptime { get; set; }
        public LogEntryType Type { get; set; }

        // Type == PumpSessionStart
        public int Mode { get; set; }

        // Type == PumpSessionEnd
        public double LeftVolume { get; set; }
        public double RightVolume { get; set; }

        // Type == BluetoothConnected
        public byte[] Address { get; set; }

        // Type == TimeSet
        public DateTime UtcTime { get; set; }

        public DateTime Timestamp { get; set; }

        public void DetermineDateTime(uint uptime, DateTime utcTime)
        {
            long diff;

            diff = (long)Uptime - (long)uptime;

            Timestamp = utcTime + TimeSpan.FromSeconds(diff);
        }
    }
}
