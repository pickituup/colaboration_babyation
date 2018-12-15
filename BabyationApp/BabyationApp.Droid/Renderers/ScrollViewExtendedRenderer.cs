using Android.Content;
using BabyationApp.Controls;
using BabyationApp.Droid.Renderers;
using System.ComponentModel;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRendererAttribute(typeof(ScrollViewExtended), typeof(ScrollViewExtendedRenderer))]
namespace BabyationApp.Droid.Renderers
{
    public class ScrollViewExtendedRenderer : ScrollViewRenderer
    {

        public ScrollViewExtendedRenderer(Context context)
            : base(context) { }

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                e.OldElement.PropertyChanged -= OnNewElementPropertyChanged;
            }

            if (e.NewElement != null)
            {
                e.NewElement.PropertyChanged += OnNewElementPropertyChanged;
            }

            if (Element != null)
            {
                ApplyScrollbarFading();
            }
        }

        private void OnNewElementPropertyChanged(object sender, PropertyChangedEventArgs e) => ApplyScrollbarFading();

        private void ApplyScrollbarFading()
        {
            try
            {
                ScrollbarFadingEnabled = ((ScrollViewExtended)Element).IsScrollbarFading;
            }
            catch (System.Exception exc)
            {
                System.Console.WriteLine(string.Format("ScrollViewExtendedRenderer.ApplyScrollbarFading - {0}", exc.Message));
                Debugger.Break();
            }
        }
    }
}