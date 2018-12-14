using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFImageLoading.Forms;
using Xamarin.Forms;

namespace BabyationApp.Controls.Views
{
    /// <summary>
    /// Cached Image Extented. It just keeps ui size sync with the image.
    /// </summary>
    class CachedImageEx : CachedImage
    {
        public CachedImageEx()
        {
            RetryCount = 0;
            //WidthRequest = 1;
            //HeightRequest = 1;
            DownsampleToViewSize = true;
            CacheDuration = TimeSpan.FromDays(30);
            // DownsampleUseDipUnits = true;
            LoadingPriority = FFImageLoading.Work.LoadingPriority.Highest;
            CacheType = FFImageLoading.Cache.CacheType.Memory;
            Aspect = Aspect.AspectFit;
            Success += ImageEx_Success;
            Error += ImageEx_Error;
            PropertyChanged += ImageEx_PropertyChanged;
        }

        /// <summary>
        /// Just logs the image loading error in case
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void ImageEx_Error(object sender, CachedImageEvents.ErrorEventArgs e)
        {
            Debug.WriteLine((e.Exception.Message));
        }

        /// <summary>
        /// Handles the Source and IsVisible property changes to sync the image size to UI size
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void ImageEx_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Source":
                    if (Source == null)
                    {
                        WidthRequest = 0;
                        HeightRequest = 0;
                    }
                    break;
                case "IsVisible":
                    if (IsVisible == false)
                    {
                        WidthRequest = 0;
                        HeightRequest = 0;
                    }
                    else
                    {
                        WidthRequest = _lastWidth;
                        HeightRequest = _lastHeight;
                    }
                    break;
            }
        }

        private double _lastWidth, _lastHeight;
        /// <summary>
        /// Tracks the last successfull loaded image size
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void ImageEx_Success(object sender, CachedImageEvents.SuccessEventArgs e)
        {
            WidthRequest = e.ImageInformation.OriginalWidth;
            HeightRequest = e.ImageInformation.OriginalHeight;
            _lastWidth = e.ImageInformation.OriginalWidth;
            _lastHeight = e.ImageInformation.OriginalHeight;
        }
    }
}
