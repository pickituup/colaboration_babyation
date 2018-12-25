using BabyationApp.Controls.TextEditors;
using BabyationApp.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(EditorExtended), typeof(EditorExtendedRenderer))]
namespace BabyationApp.iOS.Renderers {
    public class EditorExtendedRenderer : EditorRenderer { }
}