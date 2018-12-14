using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Foundation;

namespace BabyationApp.iOS.Core
{
    //
    // Source: https://baglabs.com/2018/01/09/handling-iphone-x-devices-xamarin-forms/
    //

    /// <summary>
    /// List of iOS devices 
    /// </summary>
    public class iOSDevice
    {
        List<string> iphonesWithNotch = new List<string>();

        public iOSDevice()
        {
            // Hardware identificators grabbed from https://gist.github.com/adamawolf/3048717
            // iPhone resoulutions: https://www.paintcodeapp.com/news/ultimate-guide-to-iphone-resolutions
            //
            iphonesWithNotch.Add("iPhone10,3"); //iPhone X
            iphonesWithNotch.Add("iPhone10,6"); //iPhone X GSM
            iphonesWithNotch.Add("iPhone11,2"); //iPhone XS
            iphonesWithNotch.Add("iPhone11,4"); //iPhone XS Max
            iphonesWithNotch.Add("iPhone11,6"); //iPhone XS Max China
            iphonesWithNotch.Add("iPhone11,8"); //iPhone XR
        }

        public bool deviceHasNotch()
        {
            var device = CheckDeviceHardware("hw.machine");

            //Simulator
            if (device == "i386" || device == "x86_64")
            {
                var simulatorDevice = NSProcessInfo.ProcessInfo.Environment["SIMULATOR_MODEL_IDENTIFIER"].Description;
                if (iphonesWithNotch.Contains(simulatorDevice))
                {
                    return true;
                }
            }
            //Actual Device
            else if (iphonesWithNotch.Contains(device))
            {
                return true;
            }

            return false;
        }

        /*
         * From XLabs
         * https://github.com/XLabs/Xamarin-Forms-Labs/blob/master/src/Platform/XLabs.Platform.iOS/Device/AppleDevice.cs#L152
         */

        [DllImport("/usr/lib/libSystem.dylib")]
        internal static extern int sysctlbyname(
           [MarshalAs(UnmanagedType.LPStr)] string property,
           IntPtr output,
           IntPtr oldLen,
           IntPtr newp,
           uint newlen);

        public string CheckDeviceHardware(string property)
        {
            var pLen = Marshal.AllocHGlobal(sizeof(int));
            sysctlbyname(property, IntPtr.Zero, pLen, IntPtr.Zero, 0);
            var length = Marshal.ReadInt32(pLen);
            var pStr = Marshal.AllocHGlobal(length);
            sysctlbyname(property, pStr, pLen, IntPtr.Zero, 0);

            // prevent memory leak
            //
            //return Marshal.PtrToStringAnsi(pStr);
            var result = Marshal.PtrToStringAnsi(pStr);
            Marshal.FreeHGlobal(pLen);
            Marshal.FreeHGlobal(pStr);
            return result;
        }
    }
}