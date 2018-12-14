using System;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using Android.Widget;
using Android.OS;
using Xamarin.Forms;
using Android.Content;
using Android.Graphics;
using BabyationApp.Droid.Dependencies;
using BabyationApp.Helpers;
using Microsoft.WindowsAzure.MobileServices;
using BabyationApp.Managers;
using FFImageLoading;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Android;
using System.Net;
using Plugin.Permissions;
using Plugin.CurrentActivity;

namespace BabyationApp.Droid
{
    [Activity(Theme = "@style/MainTheme", ConfigurationChanges = ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, IAuthenticate
    {

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            CurrentPlatform.Init();
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            CrossCurrentActivity.Current.Init(this, savedInstanceState);

            OverridePendingTransition(0, 0);

            DependencyService.Register<BabyationApp.Interfaces.IPictureCache, BabyationApp.Droid.Dependencies.PictureCache>();
            DependencyService.Register<BabyationApp.Interfaces.IPlatformAPI, BabyationApp.Droid.Dependencies.PlatformAPI>();

            ThreadHelper.Initialize(System.Environment.CurrentManagedThreadId);

            // BLE service must be instantiated from proper platform
            BluetoothManager bluetoothManager = BluetoothManager.Instance;
			ServicePointManager.ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => true;

            IBluetoothLE bluetoothLE = CrossBluetoothLE.Current;            
            bluetoothLE.Adapter.ScanMode = ScanMode.Balanced;
            bluetoothManager.BluetoothLE = bluetoothLE;

            Rg.Plugins.Popup.Popup.Init(this, savedInstanceState);

            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            var config = new FFImageLoading.Config.Configuration()
            {
                VerboseLogging = true,
                VerbosePerformanceLogging = true,
                Logger = new CustomMiniLogger(),
            };

            ImageService.Instance.Initialize(config);

            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(null);

            App myapp = new App();
            LoadApplication(myapp);

            //this.RequestPermissions(new[]
            //{
            //    Manifest.Permission.AccessCoarseLocation,
            //    Manifest.Permission.BluetoothPrivileged,
            //    Manifest.Permission.ReadExternalStorage
            //}, 0);


            LoginManager loginInstance = LoginManager.Instance;
            loginInstance.Init((IAuthenticate)this);
            await DataManager.Instance.Initialize(LoginManager.Instance.CurrentClient);
            myapp.AfterStart();
            /*}
            catch (Exception ex) {

                throw;
            }*/
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    

        #region Exceptions

        /*
        private static void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs unobservedTaskExceptionEventArgs) {
            throw new Exception("TaskSchedulerOnUnobservedTaskException", unobservedTaskExceptionEventArgs.Exception);

        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs) {
            throw new Exception("CurrentDomainOnUnhandledException", unhandledExceptionEventArgs.ExceptionObject as Exception);
        }
        void MyApp_UnhandledExceptionHandler(object sender, RaiseThrowableEventArgs e) {
            throw e.Exception;
        }*/


        #endregion

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
        }

        MobileServiceUser user;

        public async Task<bool> Authenticate(Provider provider, string username = "", string password = "")
        {
            var success = false;
            var message = string.Empty;

            try
            {
                switch (provider)
                {
                    case Provider.Google:
                        user = await LoginManager.Instance.CurrentClient.LoginAsync(this,
                            MobileServiceAuthenticationProvider.Google, "babyation");
                        break;
                    case Provider.Facebook:
                        user = await LoginManager.Instance.CurrentClient.LoginAsync(this,
                            MobileServiceAuthenticationProvider.Facebook, "babyation");
                        break;
                    case Provider.Custom:
                        user = await LoginManager.Instance.CurrentClient.LoginAsync("custom",
                            new JObject { ["Username"] = username, ["Password"] = password });
                        break;
                    default:
                        break;
                }

                if (user != null)
                {
                    success = true;

                    LoginManager.Instance.Token = user.MobileServiceAuthenticationToken;
                    string[] useridParts = user.UserId.Split(':');

                    LoginManager.Instance.UserId = useridParts.Length > 1 ? useridParts[1] : useridParts[0];

                    LoginManager.Instance.Provider = provider;
                }
            }
            catch (Exception ex)
            {
                AnalyticsManager.Instance.TrackError(ex);
            }

            return success;
        }
    }


    public class AndroidBug5497WorkaroundForXamarinAndroid
    {

        // For more information, see https://code.google.com/p/android/issues/detail?id=5497
        // To use this class, simply invoke assistActivity() on an Activity that already has its content view set.

        // CREDIT TO Joseph Johnson (http://stackoverflow.com/users/341631/joseph-johnson) for publishing the original Android solution on stackoverflow.com

        public static void assistActivity(Activity activity)
        {
            new AndroidBug5497WorkaroundForXamarinAndroid(activity);
        }

        private Android.Views.View mChildOfContent;
        private int usableHeightPrevious;
        private FrameLayout.LayoutParams frameLayoutParams;

        private AndroidBug5497WorkaroundForXamarinAndroid(Activity activity)
        {
            var content = (FrameLayout)activity.FindViewById(Android.Resource.Id.Content);
            mChildOfContent = content.GetChildAt(0);

            var vto = mChildOfContent.ViewTreeObserver;
            vto.GlobalLayout += (object sender, EventArgs e) =>
            {
                possiblyResizeChildOfContent();
            };

            frameLayoutParams = (FrameLayout.LayoutParams)mChildOfContent.LayoutParameters;
        }

        private void possiblyResizeChildOfContent()
        {
            int usableHeightNow = computeUsableHeight();

            if (usableHeightNow != usableHeightPrevious)
            {
                int usableHeightSansKeyboard = mChildOfContent.RootView.Height;
                int heightDifference = usableHeightSansKeyboard - usableHeightNow;

                frameLayoutParams.Height = usableHeightSansKeyboard - heightDifference;
                mChildOfContent.RequestLayout();
                usableHeightPrevious = usableHeightNow;
            }
        }

        private int computeUsableHeight()
        {
            var r = new Rect();

            mChildOfContent.GetWindowVisibleDisplayFrame(r);
            
            return Build.VERSION.SdkInt < BuildVersionCodes.Lollipop ? r.Bottom - r.Top : r.Bottom;
        }
    }
}

