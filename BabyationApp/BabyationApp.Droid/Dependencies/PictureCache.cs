using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BabyationApp.Interfaces;
using BabyationApp.Droid.Helpers;

namespace BabyationApp.Droid.Dependencies
{
    class PictureCache : IPictureCache
    {
        private  Dictionary<String, Bitmap> _store = new Dictionary<string, Bitmap>();


        public bool Contains(String key)
        {
            return _store.ContainsKey(key);
        }

        public Bitmap GetBitmap(String key, bool loadIfNotExist = true)
        {

            if (!Contains(key) && loadIfNotExist)
            {
                CacheFromFile(key);
            }

            if (Contains(key))
            {
                return _store[key];
            }
            return null;
        }

        public void CacheFromFile(String file)
        {
            if (!_store.ContainsKey(file))
            {
                Xamarin.Forms.ImageSource source = Xamarin.Forms.ImageSource.FromFile(file);
                var imageHandler = source.GetLoaderHandler();
                if (imageHandler != null)
                {
                    var nativeImage = imageHandler.LoadImageAsync(source, Android.App.Application.Context);
                    if (nativeImage != null && nativeImage.Status != TaskStatus.Faulted)
                    {
                        _store[file] = nativeImage.Result;
                        System.Diagnostics.Debug.WriteLine("PIC CACHED " + file);
                    }
                }
            }
        }

        public async void CacheFromFileAync(String file)
        {
            if (!_store.ContainsKey(file))
            {
                Xamarin.Forms.ImageSource source = Xamarin.Forms.ImageSource.FromFile(file);
                var imageHandler = source.GetLoaderHandler();
                if (imageHandler != null)
                {
                    var nativeImage = await imageHandler.LoadImageAsync(source, null);
                    if (nativeImage != null)
                    {
                        _store[file] = nativeImage;
                    }
                }
            }
        }
    }
}