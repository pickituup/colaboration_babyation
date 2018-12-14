using System;
using Xamarin.Forms;
using BabyationApp.Controls.Views;
using Xamarin.Forms.Platform.Android;
using Android.Graphics;
using BabyationApp.Droid.Renderers;
using BabyationApp.Droid.Gestures;
using Android.Views;
using System.ComponentModel;

[assembly: ExportRendererAttribute(typeof(RoundedBoxView), typeof(RoundedBoxViewRenderer))]

namespace BabyationApp.Droid.Renderers
{    
    public class RoundedBoxViewRenderer : BoxRenderer
    {
        private readonly GeneralGestureListener _listener;
        private readonly GestureDetector _detector;
        public RoundedBoxViewRenderer()
        {
            _listener = new GeneralGestureListener();
            _detector = new GestureDetector(_listener);
            this.SetWillNotDraw(false);
            _listener.TapStarted += (s, e) => { RoundedBoxView view = this.Element as RoundedBoxView; if (view != null) view.RaiseTapStarted(); };
            _listener.Tapped += (s, e) => { RoundedBoxView view = this.Element as RoundedBoxView; if (view != null) view.RaiseTapped(); };
            _listener.TapEnded += (s, e) => { RoundedBoxView view = this.Element as RoundedBoxView; if (view != null) view.RaiseTapEnded(); };
        }

        private void _listener_TapStarted(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        protected override void OnElementChanged(ElementChangedEventArgs<BoxView> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                this.GenericMotion += HandleGenericMotion;
                this.Touch += HandleTouch;
            }
        }
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == RoundedBoxView.StrokeProperty.PropertyName)
            {
                Invalidate();
            }
        }

        void HandleTouch(object sender, TouchEventArgs e)
        {
            _detector.OnTouchEvent(e.Event);
            if (e.Event.Action == MotionEventActions.Up)
            {
                RoundedBoxView view = this.Element as RoundedBoxView;
                if (view != null) view.RaiseTapEnded();
            }
        }

        void HandleGenericMotion(object sender, GenericMotionEventArgs e)
        {
            _detector.OnTouchEvent(e.Event);
        }

        public override void Draw(Canvas canvas)
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
            if (rbv.IsCircle)
            {
                var size = Math.Min(drawRect.Width(), drawRect.Height());
                var x = (drawRect.Width() - size) / 2;
                var y = (drawRect.Height() - size) / 2;
                drawRect = new RectF(x, y, x + size, y + size);
            }
            float radius = (float)(rbv.IsCircle ? Math.Min(drawRect.Width(), drawRect.Height()) / 2 : rbv.CornerRadius);
            canvas.DrawRoundRect(drawRect, radius, radius, p);
        }

    }
}