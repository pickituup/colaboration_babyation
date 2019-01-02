using System;

namespace BabyationApp.Droid.Helpers
{
    public class LocalNotification
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string Id { get; set; }
        public int IconId { get; set; }
        public DateTime NotifyTime { get; set; }
    }
}
