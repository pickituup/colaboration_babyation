using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using System.Threading.Tasks;

using Xamarin.Forms.Platform.iOS;
using UIKit;
using CoreGraphics;
using Foundation;
using BabyationApp.iOS.Dependencies;

using BabyationApp.Interfaces;
using BabyationApp.iOS.Helpers;

[assembly: Dependency(typeof(PictureCache))]

namespace BabyationApp.iOS.Dependencies
{

	class PictureCache : IPictureCache
	{
		private Dictionary<String, UIImage> _store = new Dictionary<string, UIImage>();


		public bool Contains(String key)
		{
			return _store.ContainsKey(key);
		}

		public UIImage GetBitmap(String key, bool loadIfNotExist = true)
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
					var nativeImage = imageHandler.LoadImageAsync(source);
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
					var nativeImage = await imageHandler.LoadImageAsync(source);
					if (nativeImage != null)
					{
						_store[file] = nativeImage;
					}
				}
			}
		}
	}
}
