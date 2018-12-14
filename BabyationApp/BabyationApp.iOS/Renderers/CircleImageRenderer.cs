using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using BabyationApp.Controls.Views;
using BabyationApp.iOS.Renderers;
using CoreGraphics;

[assembly: ExportRenderer(typeof(CircleImage), typeof(CircleImageRenderer))]

namespace BabyationApp.iOS.Renderers
{
    public class CircleImageRenderer : ImageRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || Element == null)
                return;
            try
            {
                double min = Math.Min(Element.Width, Element.Height);
                Control.Layer.CornerRadius = (float)(min / 2.0);
                Control.Layer.MasksToBounds = false;
                Control.Layer.BorderColor = new CGColor(1, 1, 1);
                Control.Layer.BorderWidth = 3;
                Control.ClipsToBounds = true;
            }
            catch (Exception ex)
            {
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == VisualElement.HeightProperty.PropertyName ||
                e.PropertyName == VisualElement.WidthProperty.PropertyName)
            {
                try
                {
                    double min = Math.Min(Element.Width, Element.Height);
                    Control.Layer.CornerRadius = (float)(min / 2.0);
                    Control.Layer.MasksToBounds = false;
                    Control.ClipsToBounds = true;
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}
