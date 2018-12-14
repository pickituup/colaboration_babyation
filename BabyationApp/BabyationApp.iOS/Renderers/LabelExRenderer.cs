using System;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using BabyationApp.iOS.Renderers;
using BabyationApp.Controls.Views;
using System.Drawing;
using UIKit;
using BabyationApp.Controls.TextEditors;
using Foundation;

[assembly: ExportRendererAttribute(typeof(LabelEx), typeof(LabelExRenderer))]
namespace BabyationApp.iOS.Renderers
{
    public class LabelExRenderer : LabelRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);
            try
            {
                LabelEx control = Element as LabelEx;
                if (control == null || Control == null)
                {
                    return;
                }

                if (control.NumberOfLines > 1)
                {
                    Control.Lines = control.NumberOfLines;
                }

                if (control.LetterSpacing != 0.0f && !String.IsNullOrEmpty(control.Text))
                {
                    var text = control.Text;
                    var attributedString = new NSMutableAttributedString(text);

                    var nsKern = new NSString("NSKern");
                    var spacing = NSObject.FromObject(control.LetterSpacing * 10);
                    var range = new NSRange(0, text.Length);

                    attributedString.AddAttribute(nsKern, spacing, range);
                    if (control.IsUnderlined)
                    {
                        attributedString.Append(new NSAttributedString(control.Text, underlineStyle: NSUnderlineStyle.Single));
                    }
                    Control.AttributedText = attributedString;
                }
                else if (control.IsUnderlined)
                {
                    var attributedString = new NSMutableAttributedString();
                    attributedString.Append(new NSAttributedString(control.Text, underlineStyle: NSUnderlineStyle.Single));
                    Control.AttributedText = attributedString;
                }
                else
                {
                    var paragraphStyle = new NSMutableParagraphStyle()
                    {
                        LineSpacing = control.LineHeightEx
                    };
                    var attr = new NSMutableAttributedString(control.Text);
                    var style = UIStringAttributeKey.ParagraphStyle;
                    var range = new NSRange(0, attr.Length);

                    attr.AddAttribute(style, paragraphStyle, range);

                    this.Control.AttributedText = attr;
                }

            }
            catch (Exception exc)
            {
                System.Diagnostics.Debug.WriteLine("Exception " + exc.Message);
            }
        }
    }
}