using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BabyationApp.Interfaces;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace BabyationApp.Droid.Dependencies
{
    public class PlatformAPI : IPlatformAPI
    {
        public void UpdateStatusBar(String color, bool visible)
        {
            try
            {
                var window = ((Activity)Forms.Context).Window;
                window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                window.SetStatusBarColor(Color.FromHex(color).ToAndroid());
                if (visible)
                {
                    window.ClearFlags(WindowManagerFlags.Fullscreen);
                }
                else
                {
                    window.AddFlags(WindowManagerFlags.Fullscreen);
                }
            }
            catch (Exception exc)
            {
                System.Diagnostics.Debug.WriteLine("Exception in setting status bar color in android: " + exc.Message);
            }
        }

        public bool HasTopNotch()
        {
            return false;
        }

        public Thickness SafeArea()
        {
            return new Thickness();
        }
    }
}