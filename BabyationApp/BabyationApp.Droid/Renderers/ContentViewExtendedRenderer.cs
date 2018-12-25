using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using BabyationApp.Controls;
using BabyationApp.Droid.Renderers;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ContentViewExtended), typeof(ContentViewExtendedRenderer))]
namespace BabyationApp.Droid.Renderers {
    public sealed class ContentViewExtendedRenderer : ViewRenderer {

        private static readonly PorterDuff.Mode _MASK_DSTIN_PORTER_DUFF = PorterDuff.Mode.DstIn;
        private static readonly float _DEFAULT_BORDER_WIDTH = 4;
        private static readonly Android.Graphics.Color _DEFAULT_BORDER_COLOR = Android.Graphics.Color.Red;

        private Paint _maskPaint;
        private Paint _borderPaint;

        private Path _maskPath;
        private Path _borderPath;

        /// <summary>
        /// Public ctor
        /// </summary>
        /// <param name="context"></param>
        public ContentViewExtendedRenderer(Context context) : base(context) {
            ///
            /// Allows to draw itself (in other case Draw(Canvas canvas) will not be occured).
            /// 
            SetWillNotDraw(false);

            ///
            /// Resolves 'black masked' area (for Xfermode consuming).
            /// 
            if (((int)Android.OS.Build.VERSION.SdkInt) >= 11)
            {
                SetLayerType(LayerType.Hardware, null);
            }

            Init();
        }

        private float _resolvedCornerRadius;
        public float ResolvedCornerRadius {
            get => _resolvedCornerRadius;
            private set
            {
                _resolvedCornerRadius = TypedValue.ApplyDimension(ComplexUnitType.Dip, value, Context.Resources.DisplayMetrics);
            }
        }

        private float _resolvedBorderThickness;
        public float ResolvedBorderThickness {
            get => _resolvedBorderThickness;
            private set
            {
                _resolvedBorderThickness = TypedValue.ApplyDimension(ComplexUnitType.Dip, value, Context.Resources.DisplayMetrics);

                _borderPaint.StrokeWidth = _resolvedBorderThickness;
                _maskPaint.StrokeWidth = _resolvedBorderThickness;
            }
        }

        private Android.Graphics.Color _resolvedBorderColor;
        public Android.Graphics.Color ResolvedBorderColor {
            get => _resolvedBorderColor;
            private set
            {
                _resolvedBorderColor = value;

                _borderPaint.Color = _resolvedBorderColor;
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.View> e) {
            base.OnElementChanged(e);

            if (Element != null)
            {
                ResolvedCornerRadius = ((ContentViewExtended)Element).CornerRadius;

                ResolvedBorderThickness = ((ContentViewExtended)Element).BorderThickness;
                ResolvedBorderColor = ResolveNativeColor(((ContentViewExtended)Element).BorderColor);
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e) {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == ContentViewExtended.BorderColorProperty.PropertyName)
            {
                ResolvedBorderColor = ResolveNativeColor(((ContentViewExtended)Element).BorderColor);

                /// 
                /// Invalidate() call, will force view, to redraw itself
                /// 
                Invalidate();
            }
            else if (e.PropertyName == ContentViewExtended.BorderThicknessProperty.PropertyName)
            {
                ResolvedBorderThickness = ((ContentViewExtended)Element).BorderThickness;

                /// 
                /// Invalidate() call, will force view, to redraw itself
                /// 
                Invalidate();
            }
            else if (e.PropertyName == ContentViewExtended.CornerRadiusProperty.PropertyName)
            {
                ResolvedCornerRadius = ((ContentViewExtended)Element).CornerRadius;

                /// 
                /// Invalidate() call, will force view, to redraw itself
                /// 
                Invalidate();
            }
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec) {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);

            _maskPath = BuildRoundedPath(MeasuredWidth, MeasuredHeight, ResolvedCornerRadius, ResolvedBorderThickness);
            _borderPath = BuildRoundedPath(MeasuredWidth, MeasuredHeight, ResolvedCornerRadius, ResolvedBorderThickness);
        }

        public override void Draw(Canvas canvas) {
            try
            {
                canvas.Save();

                base.Draw(canvas);

                canvas.DrawPath(_maskPath, _maskPaint);
                canvas.DrawPath(_borderPath, _borderPaint);
                canvas.Restore();

            }
            catch (System.Exception exc)
            {
            }
        }

        private void Init() {
            _borderPaint = new Paint(PaintFlags.AntiAlias)
            {
                StrokeWidth = _DEFAULT_BORDER_WIDTH,
                Color = _DEFAULT_BORDER_COLOR,
            };
            _borderPaint.SetStyle(Paint.Style.Stroke);

            _maskPaint = new Paint(PaintFlags.AntiAlias);
            _maskPaint.SetStyle(Paint.Style.FillAndStroke);
            _maskPaint.SetXfermode(new PorterDuffXfermode(_MASK_DSTIN_PORTER_DUFF));
        }

        private Path BuildRoundedPath(float width, float height, float radius, float offset = 0) {
            Path path = new Path();

            try
            {
                path.AddRoundRect(offset, offset, width - offset, height - offset, radius, radius, Path.Direction.Cw);
            }
            catch (System.Exception) { }

            return path;
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