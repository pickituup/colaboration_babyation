using System;
using System.Collections.Generic;
using System.Text;
using BabyationApp.Interfaces;
using BabyationApp.iOS.Dependencies;
using BabyationApp.iOS.Helpers;
using Xamarin.Forms;
using UIKit;
using Foundation;
using Xamarin.Forms.Platform.iOS;
using BabyationApp.iOS.Core;
using System.Diagnostics;


[assembly: Dependency(typeof(PlatformAPI))]
namespace BabyationApp.iOS.Dependencies
{
    public class PlatformAPI : IPlatformAPI
    {
        iOSDevice _device = null;
        public void UpdateStatusBar(String color, bool visible)
        {
            try
            {
                UIApplication.SharedApplication.StatusBarHidden = !visible;
                UIView statusBar = UIApplication.SharedApplication.ValueForKey(new NSString("statusBar")) as UIView;
                if (statusBar != null && visible)
                {
                    statusBar.BackgroundColor = Xamarin.Forms.Color.FromHex(color).ToUIColor();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("UpdateStatusBar exception: {0}", ex);
            }
        }

        public bool HasTopNotch()
        {
            if( null == _device)
            {
                _device = new iOSDevice();
            }

            if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
            {
                UIEdgeInsets sArea = UIApplication.SharedApplication.Delegate.GetWindow().SafeAreaInsets;
                Debug.WriteLine("Safe areas: {0}", sArea.ToString());
            }
            return _device.deviceHasNotch();
        }

        public Thickness SafeArea()
        {
            UIEdgeInsets sArea = new UIEdgeInsets();
            if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
            {
                sArea = UIApplication.SharedApplication.Delegate.GetWindow().SafeAreaInsets;
                Debug.WriteLine("Safe areas: {0}", sArea.ToString());
            }
            return new Thickness(sArea.Left, sArea.Top, sArea.Right, sArea.Bottom);
        }
    }
}
