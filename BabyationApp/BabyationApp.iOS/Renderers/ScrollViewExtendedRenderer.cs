using BabyationApp.Controls;
using BabyationApp.iOS.Renderers;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRendererAttribute(typeof(ScrollViewExtended), typeof(ScrollViewExtendedRenderer))]
namespace BabyationApp.iOS.Renderers {
    public class ScrollViewExtendedRenderer : ScrollViewRenderer {

        protected override void OnElementChanged(VisualElementChangedEventArgs e) {
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

        private void ApplyScrollbarFading() {
            ///
            /// TODO: iOS - guidelines prevent ability to make scrollbar always visible
            /// 
        }
    }
}