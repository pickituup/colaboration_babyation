using System;
using Xamarin.Forms;
using BabyationApp.Controls.TextEditors;
using Xamarin.Forms.Platform.Android;
using Android.Graphics;
using BabyationApp.Droid.Renderers;
using BabyationApp.Droid.Gestures;
using Android.Views;
using System.ComponentModel;
using Android.Content;
using BabyationApp.Controls.Views;

[assembly: ExportRendererAttribute(typeof(ViewCell), typeof(ViewCellRendererEx))]
namespace BabyationApp.Droid.Renderers
{
    public class ViewCellRendererEx : ViewCellRenderer
    {
        protected override Android.Views.View GetCellCore(Cell item, Android.Views.View convertView, ViewGroup parent, Context context)
        {
            Android.Views.View view = base.GetCellCore(item, convertView, parent, context);
            if (view != null)
            {
                view.SetBackgroundColor(Xamarin.Forms.Color.Transparent.ToAndroid());
            } 
            return view;
        }
    }
}


