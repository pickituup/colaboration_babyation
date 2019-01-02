using System;

namespace BabyationApp.Interfaces
{
    public interface ILocalNotificationService
    {
        void Schedule(string title, string body, string id, DateTime notifyTime);
        void Cancel(string id);
    }
}
