using System;
using Xamarin.Forms;
using BabyationApp.Controls.Views;
using Xamarin.Forms.Platform.Android;
using Android.Graphics;
using BabyationApp.Droid.Renderers;
using BabyationApp.Droid.Gestures;
using Android.Views;
using System.ComponentModel;
using Android.Content;

[assembly: ExportRendererAttribute(typeof(RoundedBoxView), typeof(RoundedBoxViewRenderer))]

namespace BabyationApp.Droid.Renderers
{   
    public class RoundedBoxViewRenderer : BoxRenderer
    {
        private readonly GeneralGestureListener _listener;
        private readonly GestureDetector _detector;
        private DateTime _timeCalc;
        private bool _tapEnded = false;
        public RoundedBoxViewRenderer(Context context) : base(context)
        {
            _listener = new GeneralGestureListener();
            _detector = new GestureDetector(context, _listener);
            this.SetWillNotDraw(false);

            _listener.TapStarted += (s, e) =>
            {
                _tapEnded = false;
                this._timeCalc = DateTime.Now; RoundedBoxView view = this.Element as RoundedBoxView; if (view != null) view.RaiseTapStarted();
            };

            _listener.Tapped += (s, e) =>
            {
                RoundedBoxView view = this.Element as RoundedBoxView; if (view != null) view.RaiseTapped();
            };          

            _listener.LongPressed += (s, e) =>
            {
                RoundedBoxView view = this.Element as RoundedBoxView;

                if (view != null)
                {
                    view.RaiseLongPress((DateTime.Now - this._timeCalc).TotalMilliseconds);                    
                }

                if (!_tapEnded)
                {
                    _tapEnded = true;
                    view.RaiseTapEnded((DateTime.Now - this._timeCalc).TotalMilliseconds);
                }
            };

            //_listener.TapEnded += (s, e) => {RoundedBoxView view = this.Element as RoundedBoxView; if (view != null) view.RaiseTapEnded((DateTime.Now - this._timeCalc).TotalMilliseconds); };
        }

        protected override void OnElementChanged(ElementChangedEventArgs<BoxView> e)
        { 
            base.OnElementChanged(e);

            if (e.NewElement == null)
            {
                this.GenericMotion -= HandleGenericMotion;
                this.Touch -= HandleTouch;
            }
            if (e.OldElement == null)
            {
                this.GenericMotion += HandleGenericMotion;
                this.Touch += HandleTouch;
            }
        }
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == RoundedBoxView.CornerRadiusProperty.PropertyName
                || e.PropertyName == RoundedBoxView.RadiusBasedOnSizeProperty.PropertyName
                || e.PropertyName == RoundedBoxView.RadiusSizeRatioProperty.PropertyName
               || e.PropertyName == RoundedBoxView.StrokeProperty.PropertyName
               || e.PropertyName == RoundedBoxView.StrokeThicknessProperty.PropertyName
               || e.PropertyName == RoundedBoxView.IsCircleProperty.PropertyName
               || e.PropertyName == RoundedBoxView.WidthProperty.PropertyName
               || e.PropertyName == RoundedBoxView.HeightProperty.PropertyName
               || e.PropertyName == RoundedBoxView.WidthRequestProperty.PropertyName
               || e.PropertyName == RoundedBoxView.HeightRequestProperty.PropertyName)
            {
                Invalidate();
            }
        }

        void HandleTouch(object sender, TouchEventArgs e)
        {            
            RoundedBoxView view = this.Element as RoundedBoxView;
            if (view != null)
            {
                if (view.IsInteractable == false)
                {
                    e.Handled = false;
                }
                else
                {                    
                    if (e.Event.Action == MotionEventActions.Up)
                    {
                        _tapEnded = true;
                        Rect rc = new Rect();
                        GetDrawingRect(rc);
                        int x = (int)e.Event.GetX(), y = (int)e.Event.GetY();                        
                        view.RaiseTapEnded((DateTime.Now - this._timeCalc).TotalMilliseconds, rc.Contains(x, y) == false);   
                        view.RaiseTapUp((DateTime.Now - this._timeCalc).TotalMilliseconds, rc.Contains(x, y) == false);                   
                    }

                    _detector.OnTouchEvent(e.Event);
                }
            }
        }

        void HandleGenericMotion(object sender, GenericMotionEventArgs e)
        {
            RoundedBoxView view = this.Element as RoundedBoxView;
            if (view != null && view.IsInteractable == false)
            {
                e.Handled = false;
            }
            else
            {
                _detector.OnTouchEvent(e.Event);
            }
        }

        public override void Draw(Canvas canvas)
        {
            try
            {
                RoundedBoxView rbv = (RoundedBoxView)this.Element;

                Rect rc = new Rect();
                GetDrawingRect(rc);

                Rect interior = rc;
                //interior.Inset((int)rbv.StrokeThickness, (int)rbv.StrokeThickness);

                Paint p = new Paint()
                {
                    Color = rbv.Stroke.ToAndroid(),
                    AntiAlias = true,
                    Dither = true
                };
                RectF drawRect = new RectF(interior);
                float radius = (float)(rbv.CornerRadius);
                if (rbv.IsCircle)
                {
                    var size = Math.Min(drawRect.Width(), drawRect.Height());
                    var x = (drawRect.Width() - size) / 2;
                    var y = (drawRect.Height() - size) / 2;
                    drawRect = new RectF(x, y, x + size, y + size);
                    radius = Math.Min(drawRect.Width(), drawRect.Height()) / 2;
                }
                else if (rbv.RadiusBasedOnSize)
                {
                    radius = Math.Min(drawRect.Width(), drawRect.Height()) * (float) (rbv.RadiusSizeRatio);
                }
                canvas.DrawRoundRect(drawRect, radius, radius, p);
            }
            catch (Exception e)
            {
                base.Draw(canvas);
            }
            
        }

    }
}