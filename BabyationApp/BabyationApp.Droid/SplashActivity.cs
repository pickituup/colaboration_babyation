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
using Java.Lang;

namespace BabyationApp.Droid
{
    [Activity(Label = "@string/app_name", Icon = "@drawable/icon", Theme = "@style/SplashTheme", LaunchMode = LaunchMode.SingleTask, Immersive = true, MainLauncher = true, NoHistory = true, ConfigurationChanges = ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]

    [IntentFilter(new[] { Intent.ActionView },
              Categories = new[] { Intent.ActionView, Intent.CategoryBrowsable, Intent.CategoryDefault },
              DataScheme = "https",
              DataHost = "babyation.azurewebsites.net",
              DataPathPrefix = "/",
              AutoVerify = true)]
    public class SplashActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            //SetContentView(Resource.Layout.SplashScreen);   

            // Disable activity slide-in animation
            OverridePendingTransition(0, 0);
            //Task startupWork = new Task(() =>
            //{
            //    Task.Delay(500); // Simulate a bit of startup work.
            //});

            //startupWork.ContinueWith(t =>
            //{
                string action = Intent.Action;
                string strLink = Intent.DataString;
                Intent intent = new Intent(Application.Context, typeof(MainActivity));
                if (Android.Content.Intent.ActionView == action && !string.IsNullOrWhiteSpace(strLink))
                {
                    intent.SetAction(Intent.ActionView);
                    intent.SetData(Intent.Data);
                }
                StartActivity(intent);

            //}, TaskScheduler.FromCurrentSynchronizationContext());

            //startupWork.Start();
        }
    }
}