using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using BabyationApp.Controls;
using BabyationApp.iOS.Renderers;
using Foundation;
using UIKit;
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
            try
            {
                
            }
            catch (System.Exception exc)
            {
                System.Console.WriteLine(string.Format("ScrollViewExtendedRenderer.ApplyScrollbarFading - {0}", exc.Message));
                Debugger.Break();
            }
        }
    }
}