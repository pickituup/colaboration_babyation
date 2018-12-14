using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabyationApp.Common
{
    /// <summary>
    /// XF Device.StartTimer wrapped into this class
    /// </summary>
    public class DeviceTimer
    {
        public DeviceTimer()
        {
            Enable = true;
            Duration = TimeSpan.FromMilliseconds(3000);
            IsRunning = false;
        }

        /// <summary>
        /// Timer will be running if Enabled
        /// </summary>
        public  bool Enable { get; set; }

        /// <summary>
        /// Timer's interval
        /// </summary>
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Timer callback handler
        /// </summary>
        public Func<bool> Callback;

        /// <summary>
        /// Indicates whether the timer is currently running or not
        /// </summary>
        public  bool IsRunning { get; private set; }

        /// <summary>
        /// Start the specified callback. Timer defaults to 3 seconds.
        /// </summary>
        /// <param name="callback">Callback.</param>
        public void Start(Func<bool> callback = null) => Start(Duration.Seconds, callback);

        /// <summary>
        /// Executes a hanlder in every x seconds
        /// </summary>
        /// <param name="seconds">Timer's interval. Will default to 3 seconds for values 0 or under passed in.</param>
        /// <param name="callback">callback handler to execute when the timer elapses</param>
        public void Start(int seconds, Func<bool> callback = null )
        {
            if (IsRunning)
            {
                return;
            }

            if (seconds > 0)
            {
                Duration = TimeSpan.FromSeconds(seconds);
            }

            if (callback != null)
            {
                Callback = callback;
            }

            Xamarin.Forms.Device.StartTimer(Duration, () =>
            {                
                if (Enable)
                {
                    IsRunning = Callback();                    
                }
                else
                {
                    IsRunning = false;
                }
                return IsRunning;
            });
        }
    }
}
