
using System;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using BabyationApp.iOS.Renderers;
using BabyationApp.Controls.Views;
using System.Drawing;
using CoreGraphics;
using UIKit;
using Foundation;

[assembly: ExportRendererAttribute(typeof(RoundedBoxView), typeof(RoundedBoxViewRenderer))]
namespace BabyationApp.iOS.Renderers
{
    public class RoundedBoxViewRenderer : Xamarin.Forms.Platform.iOS.VisualElementRenderer<RoundedBoxView>
    {

        private RoundedBoxView _view;
        private DateTime _timeCalc;
        private UILongPressGestureRecognizer _longPressGestureRecognizer;

        public RoundedBoxViewRenderer()
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<RoundedBoxView> e)
        {
            base.OnElementChanged(e);

            var rbv = e.NewElement as RoundedBoxView;

            if (e.NewElement == null)
            {

                if (_longPressGestureRecognizer != null)
                {
                    this.RemoveGestureRecognizer(_longPressGestureRecognizer);
                }

            }
            if (e.OldElement == null)
            {
                if (rbv != null)
                {
                    this.UserInteractionEnabled = true;
                    _view = rbv;

                    _longPressGestureRecognizer = new UILongPressGestureRecognizer(() =>
                    {
                        rbv.RaiseLongPress((DateTime.Now - this._timeCalc).TotalMilliseconds);
                    });
                    this.AddGestureRecognizer(_longPressGestureRecognizer);
                }
            }

        }

        public override void Draw(CGRect rect)
        {
            try
            {
                RoundedBoxView rbv = (RoundedBoxView)this.Element;

                if (rbv != null)
                {
                    using (var context = UIGraphics.GetCurrentContext())
                    {
                        var rc = rect;
                        float radius = (float)(rbv.CornerRadius);
                        if (rbv.IsCircle)
                        {
                            var size = Math.Min(rect.Width, rect.Height);
                            var x = (rect.Width - size) / 2;
                            var y = (rect.Height - size) / 2;
                            rc = new CGRect(x, y, size, size);
                            radius = (float)(Math.Min(rc.Width, rc.Height) / 2);
                        }
                        else if (rbv.RadiusBasedOnSize)
                        {
                            radius = (float)(Math.Min(rc.Width, rc.Height) * rbv.RadiusSizeRatio);
                        }

						if (radius > Math.Min(rc.Width, rc.Height) / 2)
						{
							radius = (float)Math.Min(rc.Width, rc.Height) / 2;
						}

                        try
						{
							CGPath path = CGPath.FromRoundedRect(rc, radius, radius);
							context.AddPath(path);
	                        context.SetFillColor(rbv.Stroke.ToCGColor());
	                        context.SetStrokeColor(rbv.Stroke.ToCGColor());
	                        context.SetLineWidth((float)rbv.StrokeThickness);                        
	                        context.FillPath();
						}
						catch
						{
							var rectanglePath = UIBezierPath.FromRoundedRect(rc, radius);
							rbv.Stroke.ToUIColor().SetFill();
							rectanglePath.Fill();
						}
                    }
                }
            }
            catch (Exception e)
            {
                base.Draw(rect);
            }
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == RoundedBoxView.CornerRadiusProperty.PropertyName
                || e.PropertyName == RoundedBoxView.RadiusBasedOnSizeProperty.PropertyName
                || e.PropertyName == RoundedBoxView.RadiusSizeRatioProperty.PropertyName
                || e.PropertyName == RoundedBoxView.StrokeProperty.PropertyName
			    || e.PropertyName == RoundedBoxView.StrokeThicknessProperty.PropertyName
                || e.PropertyName == RoundedBoxView.IsCircleProperty.PropertyName
			    || e.PropertyName == RoundedBoxView.HeightProperty.PropertyName
				|| e.PropertyName == RoundedBoxView.WidthProperty.PropertyName)
                this.SetNeedsDisplay();
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);
            var touch = touches.AnyObject as UITouch;

            if (_view != null && touch != null)
            {
                this._timeCalc = DateTime.Now;
                _view.RaiseTapStarted();
            }
        }

        public override void TouchesMoved(NSSet touches, UIEvent evt)
        {
            base.TouchesMoved(touches, evt);
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            base.TouchesCancelled(touches, evt);
            var touch = touches.AnyObject as UITouch;

            if (_view != null && touch != null)
            {
                _view.RaiseTapEnded((DateTime.Now - this._timeCalc).TotalMilliseconds, false);
                _view.RaiseTapUp((DateTime.Now - this._timeCalc).TotalMilliseconds, false);
            }
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);
            var touch = touches.AnyObject as UITouch;

            if (_view != null && touch != null)
            {
                _view.RaiseTapEnded((DateTime.Now - this._timeCalc).TotalMilliseconds);
                _view.RaiseTapUp((DateTime.Now - this._timeCalc).TotalMilliseconds, true);
            }
        }
    }


    class RoundedBoxViewGesureRecognizer : UITapGestureRecognizer
    {
        private RoundedBoxView _view;
        private DateTime _timeCalc;

        public RoundedBoxViewGesureRecognizer(RoundedBoxView view) 
        {
            _view = view;
        }
        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);
            var touch = touches.AnyObject as UITouch;

            if (_view != null && touch != null)
            {
                this._timeCalc = DateTime.Now;
                _view.RaiseTapStarted();
            }
        }

        public override void TouchesMoved(NSSet touches, UIEvent evt)
        {
            base.TouchesMoved(touches, evt);
            //var touch = touches.AnyObject as UITouch;

            //if (touch != null)
            //{
            //    var target = GetTouchTarget(touch);

            //    if (_last != target)
            //    {
            //        if (target != null)
            //            target.RaiseEntered();
            //        if (_last != null)
            //            _last.RaiseExited();
            //        _last = target;
            //    }
            //}
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            base.TouchesCancelled(touches, evt);
            var touch = touches.AnyObject as UITouch;

            if (_view != null && touch != null)
            {
                _view.RaiseTapEnded((DateTime.Now - this._timeCalc).TotalMilliseconds);
            }
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);
            var touch = touches.AnyObject as UITouch;

            if (_view != null && touch != null)
            {
                _view.RaiseTapEnded((DateTime.Now - this._timeCalc).TotalMilliseconds);
            }
        }
    }
}
