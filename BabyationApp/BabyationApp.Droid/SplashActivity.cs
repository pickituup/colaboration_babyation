using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BabyationApp.Managers;

namespace BabyationApp.Droid
{
    [Activity(Label = "@string/app_name", Icon = "@drawable/icon", Theme = "@style/SplashTheme", Immersive = true, MainLauncher = true, NoHistory = true, ConfigurationChanges = ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    
    public class SplashActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SplashScreen);   

            // Disable activity slide-in animation
            OverridePendingTransition(0, 0);
            Task startupWork = new Task(() =>
            {
                Task.Delay(500); // Simulate a bit of startup work.
            });

            startupWork.ContinueWith(t =>
            {
                StartActivity(typeof (MainActivity));

            }, TaskScheduler.FromCurrentSynchronizationContext());

            startupWork.Start();
        }
    }
}