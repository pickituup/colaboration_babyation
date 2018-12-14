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

[assembly: ExportRendererAttribute(typeof(LabelEx), typeof(LabelExRenderer))]
namespace BabyationApp.Droid.Renderers
{
    public class LabelExRenderer : LabelRenderer
    {
        public LabelExRenderer(Context context) : base(context)
        {
        }

        protected LabelEx CustomLabel { get; private set; }


        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            UseLineHeightMultiplier();

            if (e.OldElement == null)
            {
                this.CustomLabel = (LabelEx)this.Element;
            }

            if (CustomLabel != null && CustomLabel.IsUnderlined)
            {
                this.Control.PaintFlags = this.Control.PaintFlags | PaintFlags.UnderlineText;
                this.UpdateLayout();
            }

            if (CustomLabel != null && CustomLabel.NumberOfLines > 1)
            {
                this.Control.SetLines(CustomLabel.NumberOfLines);
                this.UpdateLayout();
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == "LineHeightEx")
            {
                UseLineHeightMultiplier();
            }
        }

        private void UseLineHeightMultiplier()
        {
            try
            {
                Control.SetLineSpacing(0, ((LabelEx)Element).LineHeightEx);
            }
            catch (Exception ex)
            {                
            }
        }

    }
}


