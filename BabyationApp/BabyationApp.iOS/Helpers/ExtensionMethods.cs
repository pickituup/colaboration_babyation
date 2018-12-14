using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace BabyationApp.iOS.Helpers
{
	static class ExtensionMethods
	{
		public static IImageSourceHandler GetLoaderHandler(this ImageSource source)
		{
			//Image source handler to return
			IImageSourceHandler returnValue = null;
			//check the specific source type and return the correct image source handler
			if (source is UriImageSource)
			{
				returnValue = new ImageLoaderSourceHandler();
			}
			else if (source is FileImageSource)
			{
				returnValue = new FileImageSourceHandler();
			}
			else if (source is StreamImageSource)
			{
				returnValue = new StreamImagesourceHandler();
			}
			return returnValue;
		}
	}
}
