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
using BabyationApp.Controls.Views;
using BabyationApp.Droid.Dependencies;
using BabyationApp.Droid.Helpers;
using BabyationApp.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using BabyationApp.Interfaces;

[assembly: ExportRenderer(typeof(ImageEx), typeof(ImageExRenderer))]
namespace BabyationApp.Droid.Renderers
{
    class ImageExRenderer : ViewRenderer<ImageEx, ImageView>
    {
        public ImageExRenderer(Context context) : base(context)
        {
        }

        private Bitmap UpdateBitmap()
        {
            if (this.Control != null && this.Element != null && this.Element.Source != null)
            {
                var source = this.Element.Source as FileImageSource;
                var cache = DependencyService.Get<IPictureCache>() as PictureCache;
                var bitmap = cache.GetBitmap(source.File);
                if (bitmap != null)
                {
                    this.Control.SetImageBitmap(bitmap);
                    if (Element.UseImageSize)
                    {
                        this.Element.WidthRequest = bitmap.Width;
                        this.Element.HeightRequest = bitmap.Height;
                    }
                    return bitmap;
                }
            }
            return null;
        }

        protected  override void OnElementChanged(ElementChangedEventArgs<ImageEx> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null)
            {
                var view = new ImageView(this.Context);
                SetNativeControl(view);                
            }

            if (e.NewElement != null)
            {
                UpdateBitmap();
            }
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == ImageEx.SourceProperty.PropertyName)
            {
                UpdateBitmap();
            }
        }
    }

    
}