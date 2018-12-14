using System;
using System.Collections.Generic;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace BabyationApp.Managers
{
    public class AnalyticsManager
    {
        //const string Android_Api_Key = "bc68205f-33a4-4b00-87a4-b71ec266d1af";
        //const string iOS_Api_Key = "ee866bef-d455-4355-9c05-ae94514f250a";

        const string Android_Api_Key = "143bb148-12a4-4e34-b8e1-93a155755957";
        const string iOS_Api_Key = "20890a8d-8e8e-4322-91f6-c9e11a0c53b6";

        static readonly Lazy<AnalyticsManager> lazy = new Lazy<AnalyticsManager>(() => new AnalyticsManager());
        public static AnalyticsManager Instance { get { return lazy.Value; } }

        AnalyticsManager() { }

        public void Start()
        {
            AppCenter.Start($"android={Android_Api_Key};" +
                            $"ios={iOS_Api_Key}",
                            typeof(Analytics), typeof(Crashes));
            AppCenter.LogLevel = LogLevel.Verbose;
        }

        public void TrackError(Exception ex) => Crashes.TrackError(ex);

        public void TrackError(Exception ex, IDictionary<string, string> properties) => Crashes.TrackError(ex, properties);

        public void TrackEvent(string description) => Analytics.TrackEvent(description);
    }
}
