using System;
using BabyationApp.Helpers;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Microsoft.WindowsAzure.MobileServices;
using BabyationApp.Managers;
using FFImageLoading;
using FFImageLoading.Forms.Touch;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using System.Threading.Tasks;
using KeyboardOverlap.Forms.Plugin.iOSUnified;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using UserNotifications;
using ObjCRuntime;

namespace BabyationApp.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate, IAuthenticate
    {
        public async Task<bool> Authenticate(Provider provider, string username = "", string password = "")
        {
            var success = false;
            var message = string.Empty;
            try
            {
                switch (provider)
                {
                    case Provider.Google:
                        user = await LoginManager.Instance.CurrentClient.LoginAsync(UIApplication.SharedApplication.KeyWindow.RootViewController,
                            MobileServiceAuthenticationProvider.Google, "babyation");
                        break;
                    case Provider.Facebook:
                        user = await LoginManager.Instance.CurrentClient.LoginAsync(UIApplication.SharedApplication.KeyWindow.RootViewController,
                            MobileServiceAuthenticationProvider.Facebook, "babyation");
                        break;
                    case Provider.Custom:
                        user = await LoginManager.Instance.CurrentClient.LoginAsync("custom", new JObject { ["Username"] = username, ["Password"] = password });
                        break;
                    default:
                        break;

                }
                // Sign in with Facebook login using a server-managed flow.

                if (user != null)
                {
                    success = true;
                    LoginManager.Instance.Token = user.MobileServiceAuthenticationToken;
                    string[] useridParts = user.UserId.Split(':');
                    if (useridParts.Length > 1)
                    {
                        LoginManager.Instance.UserId = useridParts[1];
                    }
                    else
                    {
                        LoginManager.Instance.UserId = useridParts[0];
                    }

                    LoginManager.Instance.Provider = provider;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            return success;

        }

        private MobileServiceUser user;

        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            // BLE service must be instantiated from proper platform
            BluetoothManager bluetoothManager = BluetoothManager.Instance;
            IBluetoothLE bluetoothLE = CrossBluetoothLE.Current;
            bluetoothLE.Adapter.ScanMode = ScanMode.Balanced;
            bluetoothManager.BluetoothLE = bluetoothLE;

            Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();
            SQLitePCL.CurrentPlatform.Init();
            KeyboardOverlapRenderer.Init();

            DependencyService.Register<BabyationApp.Interfaces.IPictureCache, BabyationApp.iOS.Dependencies.PictureCache>();
            DependencyService.Register<BabyationApp.Interfaces.IPlatformAPI, BabyationApp.iOS.Dependencies.PlatformAPI>();
            DependencyService.Register<BabyationApp.Interfaces.ILocalNotificationService, BabyationApp.iOS.Dependencies.LocalNotificationService>();

            global::Xamarin.Forms.Forms.Init();

            var config = new FFImageLoading.Config.Configuration()
            {
                VerboseLogging = true,
                VerbosePerformanceLogging = true,
                Logger = new CustomMiniLogger(),
            };

            ImageService.Instance.Initialize(config);
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init();
            Rg.Plugins.Popup.Popup.Init();

            LoginManager loginInstance = LoginManager.Instance;
            loginInstance.Init(this);

            ScheduleManager.Instance.Initialize();
            RequestNotificationPermission();

            App myapp = new App();
            LoadApplication(myapp);

            // check for a notification

            /*
            if (options != null)
            {
                // check for a local notification
                if (options.ContainsKey(UIApplication.LaunchOptionsLocalNotificationKey))
                {
                    if (options[UIApplication.LaunchOptionsLocalNotificationKey] is UILocalNotification localNotification)
                    {
                        UIAlertController okayAlertController = UIAlertController.Create(localNotification.AlertAction, 
                                                                                         localNotification.AlertBody, 
                                                                                         UIAlertControllerStyle.Alert);

                        okayAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));

                        Window.RootViewController.PresentViewController(okayAlertController, true, null);

                        // reset our badge
                        UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
                    }
                }
            }
            */

            bool result = base.FinishedLaunching(app, options);
            myapp.AfterStart();
            return result;
        }

        void RequestNotificationPermission()
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                // Ask the user for permission to get notifications on iOS 10.0+
                UNUserNotificationCenter.Current.RequestAuthorization(
                    UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound,
                    (approved, error) => { });

                // Watch for notifications while app is active
                UNUserNotificationCenter.Current.Delegate = new UserNotificationCenterDelegate();
            }
            else if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                // Ask the user for permission to get notifications on iOS 8.0+
                var settings = UIUserNotificationSettings.GetSettingsForTypes(
                    UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound,
                    new NSSet());

                UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
            }
        }

        private static async void InitDataManager()
        {
            await DataManager.Instance.Initialize(LoginManager.Instance.CurrentClient);
        }

        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            bool result = LoginManager.Instance.CurrentClient.ResumeWithURL(url);

            return (result ? result : base.OpenUrl(app, url, options));
        }

        [Export("application:didReceiveLocalNotification:")]
        public override void ReceivedLocalNotification(UIApplication application, UILocalNotification notification)
        {
            string test = "";
        }

    }

    public class UserNotificationCenterDelegate : UNUserNotificationCenterDelegate
    {
        public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification,
                                                     Action<UNNotificationPresentationOptions> completionHandler)
        {
            // Tell system to display the notification anyway or use
            // `None` to say we have handled the display locally.
            completionHandler(UNNotificationPresentationOptions.Alert);
        }
    }
}
