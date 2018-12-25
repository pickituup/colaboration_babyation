using BabyationApp.Controls;
using BabyationApp.iOS.Renderers;
using System;
using System.ComponentModel;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ContentViewExtended), typeof(ContentViewExtendedRenderer))]
namespace BabyationApp.iOS.Renderers
{
    public class ContentViewExtendedRenderer : VisualElementRenderer<ContentViewExtended>
    {

        private ContentViewExtended _element;

        protected override void OnElementChanged(ElementChangedEventArgs<ContentViewExtended> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {

                _element = e.NewElement as ContentViewExtended;
                SetupLayer(_element.BorderThickness, _element.CornerRadius);
            }
        }

        private void SetupLayer(float borderWidth, nfloat borderRadius)
        {

            Layer.CornerRadius = borderRadius;

            if (Element.BackgroundColor != Color.Default)
            {
                Layer.BackgroundColor = Element.BackgroundColor.ToUIColor().CGColor;
            }
            else
            {
                Layer.BackgroundColor = UIColor.White.CGColor;
            }

            if (Element.BorderColor != Color.Default)
            {
                Layer.BorderColor = Element.BorderColor.ToCGColor();
                Layer.BorderWidth = borderWidth;
            }

            Layer.RasterizationScale = UIScreen.MainScreen.Scale;
            Layer.ShouldRasterize = true;
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (ContentViewExtended.BorderColorProperty.PropertyName == e.PropertyName)
            {
                Layer.BorderColor = Element.BorderColor.ToCGColor();
            }
        }
    }
}