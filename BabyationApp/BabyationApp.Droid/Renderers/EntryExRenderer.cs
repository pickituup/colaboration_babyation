using System;
using Xamarin.Forms;
using BabyationApp.Controls.TextEditors;
using Xamarin.Forms.Platform.Android;
using Android.Graphics;
using BabyationApp.Droid.Renderers;
using BabyationApp.Droid.Gestures;
using Android.Views;
using System.ComponentModel;
using Android.App;
using Android.Content;
using Android.Text;
using Android.Runtime;

[assembly: ExportRendererAttribute(typeof(EntryEx), typeof(EntryExRenderer))]
namespace BabyationApp.Droid.Renderers
{
    public class EntryExRenderer : EntryRenderer
    {
        public EntryExRenderer(Context context) : base(context)
        {
        }

        private EntryEx _entry;
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null)
            {
                _entry = (EntryEx)this.Element;
            }

            if (Control != null)
            {
                Control.SetBackgroundColor(global::Android.Graphics.Color.Transparent);
                Control.SetPadding(0, 0, 0, 0);
            }

            if (Control != null && _entry != null && _entry.IsTimeDurationInput)
            {
                //Control.SetRawInputType(InputTypes.DatetimeVariationTime | InputTypes.ClassNumber);
            }

            if (Control != null && _entry != null && _entry.IsSeparateKb)
            {
                Control.FocusChange += OnControlFocusChange;
            }

            if( Control != null && _entry != null && _entry.CustomCursorColor != null )
            {
                // No validway except read value from android styles
            }
        }


        void OnControlFocusChange(object sender, FocusChangeEventArgs e)
        {
            if (e.HasFocus)
                (Forms.Context as Activity).Window.SetSoftInputMode(SoftInput.AdjustResize);
            else
                (Forms.Context as Activity).Window.SetSoftInputMode(SoftInput.AdjustNothing);
        }
    }
}