using System;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using BabyationApp.iOS.Renderers;
using BabyationApp.Controls.Views;
using System.Drawing;
using UIKit;
using BabyationApp.Controls.TextEditors;

[assembly: ExportRendererAttribute(typeof(EntryEx), typeof(EntryExRenderer))]
namespace BabyationApp.iOS.Renderers
{
    public class EntryExRenderer : EntryRenderer
    {
        private UIColor _originalTintColor = null;

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            var control = Element as EntryEx;
            if (control == null || Control == null)
            {
                return;
            }

            Control.BackgroundColor = Xamarin.Forms.Color.Transparent.ToUIColor();
            Control.BorderStyle = UITextBorderStyle.None;

            if( 0.0 < control.CustomCursorColor.A ) // Check for alpha channel since we have can't check for null
            {
                if(null == _originalTintColor )
                {
                    _originalTintColor = Control.TintColor;
                }
                Control.TintColor = control.CustomCursorColor.ToUIColor();
            }
            else
            {
                Control.TintColor = _originalTintColor;
            }
        }
    }
}