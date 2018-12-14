using System;
using Xamarin.Forms;
using UIKit;
using Xamarin.Forms.Platform.iOS;
using System.Threading.Tasks;
using CoreGraphics;
using CoreAnimation;
using Foundation;
using BabyationApp.Controls.Views;
using BabyationApp.iOS.Helpers;
using BabyationApp.Interfaces;
using BabyationApp.iOS.Dependencies;
using BabyationApp.iOS.Renderers;


[assembly: ExportRendererAttribute(typeof(ImageEx), typeof(ImageExRenderer))]
namespace BabyationApp.iOS.Renderers
{ 
    public class ImageExRenderer : ViewRenderer<ImageEx, UIImageView>
	{
		public ImageExRenderer()
		{
		}

		private UIImage UpdateImage()
		{
			if (this.Control != null && this.Element != null && this.Element.Source != null)
			{
				var source = this.Element.Source as FileImageSource;
				var cache = DependencyService.Get<IPictureCache>() as PictureCache;
				var bitmap = cache.GetBitmap(source.File);
				if (bitmap != null)
				{
					this.Control.Image = bitmap;
					if (Element.UseImageSize)
					{
						this.Element.WidthRequest = bitmap.Size.Width  * bitmap.CurrentScale;
						this.Element.HeightRequest = bitmap.Size.Height * bitmap.CurrentScale;
					}
					return bitmap;
				}
			}
			return null;
		}

		protected override void OnElementChanged(ElementChangedEventArgs<ImageEx> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement == null)
			{
				SetNativeControl(new UIImageView(this.Frame));
				base.Control.ContentMode = UIViewContentMode.ScaleAspectFit;
			}

			if (e.NewElement != null)
			{
				UpdateImage();
			}
		}

		protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);
			if (e.PropertyName == ImageEx.SourceProperty.PropertyName)
			{
				 UpdateImage();
			}
		}
	}
}
