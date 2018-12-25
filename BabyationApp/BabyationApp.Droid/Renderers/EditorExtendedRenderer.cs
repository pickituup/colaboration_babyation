using Android.Content;
using Android.Graphics.Drawables;
using BabyationApp.Controls.TextEditors;
using BabyationApp.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(EditorExtended), typeof(EditorExtendedRenderer))]
namespace BabyationApp.Droid.Renderers {
    public class EditorExtendedRenderer : EditorRenderer {

        public EditorExtendedRenderer(Context context)
            : base(context) { }

        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e) {
            base.OnElementChanged(e);

            if (Control != null && Element != null)
            {
                RemoveUnderscore();
            }
        }

        private void RemoveUnderscore() {
            if (Control != null && Element != null)
            {
                Control.Background = new ColorDrawable(ResolveNativeColor(Element.BackgroundColor));
            }
        }

        private Android.Graphics.Color ResolveNativeColor(Xamarin.Forms.Color color) {
            byte red = (byte)(color.R * 255);
            byte green = (byte)(color.G * 255);
            byte blue = (byte)(color.B * 255);
            byte alpha = (byte)(color.A * 255);

            return new Android.Graphics.Color(red, green, blue, alpha);
        }
    }
}