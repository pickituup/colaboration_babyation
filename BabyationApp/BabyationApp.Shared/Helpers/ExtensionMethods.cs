using System;
using Xamarin.Forms;
#if__IOS__
using Xamarin.Forms.Platform.iOS;
#elif __ANDROID__
using Xamarin.Forms.Platform.Android;
#endif


namespace BabyationApp.Shared.Helpers
{
	public static class ImageSourceExtensions
	{
		public static IImageSourceHandler GetHandler (this ImageSource source)
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
