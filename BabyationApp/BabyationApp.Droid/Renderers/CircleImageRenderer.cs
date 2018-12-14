
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms.Platform.Android;
using Android.Graphics;
using Android.Views;
using System.ComponentModel;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Graphics;
using BabyationApp.Controls.Views;
using BabyationApp.Droid.Renderers;
using BabyationApp.Droid.Gestures;

[assembly: ExportRenderer(typeof(CircleImage), typeof(CircleImageRenderer))]
namespace BabyationApp.Droid.Renderers
{

    public class CircleImageRenderer : ImageRenderer
    {

        public CircleImageRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null)
            {

                if ((int)Android.OS.Build.VERSION.SdkInt < 18)
                    SetLayerType(LayerType.Software, null);
            }
        }

        protected override bool DrawChild(Canvas canvas, global::Android.Views.View child, long drawingTime)
        {
            try
            {
                var radius = Math.Min(Width, Height) / 2;
                var strokeWidth = 4;
                radius -= strokeWidth / 2;

                Path path = new Path();
                path.AddCircle(Width / 2, Height / 2, radius, Path.Direction.Ccw);
                canvas.Save();
                canvas.ClipPath(path);

                var result = base.DrawChild(canvas, child, drawingTime);

                canvas.Restore();

                path = new Path();
                path.AddCircle(Width / 2, Height / 2, radius, Path.Direction.Ccw);

                var paint = new Paint();
                paint.AntiAlias = true;
                paint.StrokeWidth = strokeWidth/2;
                paint.SetStyle(Paint.Style.Stroke);
                paint.Color = global::Android.Graphics.Color.Transparent;

                canvas.DrawPath(path, paint);

                paint.Dispose();
                path.Dispose();
                return result;
            }
            catch (Exception ex)
            {
            }

            return base.DrawChild(canvas, child, drawingTime);
        }

    }
}