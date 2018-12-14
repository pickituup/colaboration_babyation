using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace BabyationApp.Controls.Views
{    
    /// <summary>
    /// Event argument object for Long Press event
    /// </summary>
    public class EventArgsLongPress : EventArgs
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public EventArgsLongPress()
        {
            Cancelled = false;
        }

        /// <summary>
        /// Indicates whether the event was cancelled or not
        /// </summary>
        public  bool Cancelled { get; set; }

        /// <summary>
        /// The time that long presses was held for
        /// </summary>
        public double Duration { get; set; }
    }

    /// <summary>
    /// Delegate for the long press event handler
    /// </summary>
    /// <param name="sender">event sender</param>
    /// <param name="e">event args</param>
    public delegate void LongPressEventHandler(Object sender, EventArgsLongPress e);


    /// <summary>
    /// Rounded/circled view is used to show rounded/circle buttons
    /// </summary>
    /// <remarks> 
    /// We are using platform depended renderers to draw rounded/circle as well as gesturers to detect tap events
    /// </remarks>
    public class RoundedBoxView : BoxView
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public RoundedBoxView()
        {
            IsCircle = true;
            IsRadioMode = false;
        }

        /// <summary>
        /// Event that fires when tap starts
        /// </summary>
        public event EventHandler TapStarted;

        /// <summary>
        /// Event that fires when tapped occured
        /// </summary>
        public event EventHandler Tapped;

        /// <summary>
        /// Event that fires when tap ended
        /// </summary>
        public event LongPressEventHandler TapEnded;

        /// <summary>
        /// Event that fires when the box-view is toggled if IsRadioMode is true
        /// </summary>
        public event EventHandler Toggled;

        /// <summary>
        /// Event that fires on long press
        /// </summary>
        public event LongPressEventHandler LongPressed;

        /// <summary>
        /// Event that fires when IsPressed property changes
        /// </summary>
        public event EventHandler IsPressedChanged;

        /// <summary>
        /// Event that fires when log press even ended
        /// </summary>
        public event LongPressEventHandler TapUp;


        /// <summary>
        /// Platform depended renderers call this to raise TapStarted event
        /// </summary>
        public void RaiseTapStarted()
        {
            if (!IsInteractable) { return; }

            IsPressed = IsRadioMode? !IsPressed : true;

            if (TapStarted != null)
            {
                TapStarted(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Platform depended renderers call this to raise Tapped event
        /// </summary>
        public void RaiseTapped()
        {
            if (!IsInteractable) { return; }

            //if (Tapped != null)
            //    Tapped(this, EventArgs.Empty);

            //if (IsRadioMode == false)
            //{
            //    IsPressed = false;
            //}
        }

        /// <summary>
        /// Platform depended renderers call this to raise TapEnded event
        /// </summary>
        public void RaiseTapEnded(double durationInMS= 0, bool cancelled = false)
        {
            if (!IsInteractable) { return; }

            if (!cancelled)
            {
                if (Tapped != null)
                {
                    Tapped(this, EventArgs.Empty);
                }

                var oldPressstate = IsPressed;

                if (TapEnded != null && oldPressstate)
                    TapEnded(this, new EventArgsLongPress() {Duration = durationInMS, Cancelled = cancelled});

                if (IsRadioMode)
                {
                    if (Toggled != null)
                    {
                        Toggled(this, EventArgs.Empty);
                    }
                }

                if (IsRadioMode == false)
                {
                    IsPressed = false;
                }
            }            
            else
            {
                IsPressed = IsRadioMode ? !IsPressed : false;
            }
        }

        /// <summary>
        /// Platform depended renderers call this to raise LongPressed event
        /// </summary>
        public void RaiseLongPress(double durationInMS = 0)
        {
            if (!IsInteractable) { return; }

            if (LongPressed!= null)
            {
                LongPressed(this, new EventArgsLongPress() { Duration = durationInMS});
            }
        }

        /// <summary>
        /// Platform depended renderers call this to raise TapUp event
        /// </summary>
        public void RaiseTapUp(double durationInMS = 0, bool cancelled = false)
        {
            if (!IsInteractable) { return; }

            if (TapUp != null)
            {
                TapUp(this, new EventArgsLongPress() { Duration = durationInMS, Cancelled = cancelled});
            }
        }

        public static readonly BindableProperty IsInteractableProperty = BindableProperty.Create("IsInteractable", typeof(bool), typeof(RoundedBoxView), true);
        /// <summary>
        /// Gets/Sets whether this control should allow user interaction
        /// </summary>
        public bool IsInteractable
        {
            get { return (bool)GetValue(IsInteractableProperty); }
            set { SetValue(IsInteractableProperty, value); }
        }


        public static readonly BindableProperty IsRadioModeProperty = BindableProperty.Create("IsRadioMode", typeof(bool), typeof(RoundedBoxView), false);
        /// <summary>
        /// Gets/Sets whether the view should be radio button type
        /// </summary>
        /// <remarks>
        /// If true, fires Toggled event
        /// </remarks>
        public bool IsRadioMode
        {
            get { return (bool)GetValue(IsRadioModeProperty); }
            set { SetValue(IsRadioModeProperty, value); }
        }

        public static readonly BindableProperty IsPressedProperty = BindableProperty.Create("IsPressed", typeof(bool), typeof(RoundedBoxView), false, BindingMode.TwoWay);
        /// <summary>
        /// Gets/Sets whether the view is pressed right now or not
        /// </summary>
        public bool IsPressed
        {
            get { return (bool)GetValue(IsPressedProperty); }
            set
            {
                SetValue(IsPressedProperty, value);
                IsPressedChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public static readonly BindableProperty CornerRadiusProperty =
            BindableProperty.Create("CornerRadius", typeof(double), typeof(RoundedBoxView), default(double));
        /// <summary>
        /// Cornder radius property for the rounded views
        /// </summary>
        public double CornerRadius
        {
            get { return (double)base.GetValue(CornerRadiusProperty); }
            set { base.SetValue(CornerRadiusProperty, value); }
        }

        public static readonly BindableProperty StrokeProperty = BindableProperty.Create("Stroke", typeof(Color), typeof(RoundedBoxView), Color.Transparent);

        /// <summary>
        /// Background color for the view
        /// </summary>
        public Color Stroke
        {
            get { return (Color)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        public static readonly BindableProperty StrokeThicknessProperty = BindableProperty.Create("StrokeThickness", typeof(double), typeof(RoundedBoxView), 5.0);
        /// <summary>
        /// The line width of the strokes to draw the view. 
        /// </summary>
        /// <remarks>
        /// Uses by platform dependent renderers
        /// </remarks>
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public static readonly BindableProperty HasShadowProperty = BindableProperty.Create("HasShadow", typeof(bool), typeof(RoundedBoxView), default(bool));
        /// <summary>
        /// Gets/Sest whether the view have shadows
        /// </summary>
        /// <remarks>
        /// Uses by platform dependent renderers
        /// </remarks>
        public bool HasShadow
        {
            get { return (bool)GetValue(HasShadowProperty); }
            set { SetValue(HasShadowProperty, value); }
        }

        public static readonly BindableProperty IsCircleProperty = BindableProperty.Create("IsCircle", typeof(bool), typeof(RoundedBoxView), false);
        /// <summary>
        /// Gets/Sest whether the view should be circled
        /// </summary>
        /// <remarks>
        /// Uses by platform dependent renderers to draw the view
        /// </remarks>
        public bool IsCircle
        {
            get { return (bool)GetValue(IsCircleProperty); }
            set { SetValue(IsCircleProperty, value); }
        }

        public static readonly BindableProperty RadiusBasedOnSizeProperty = BindableProperty.Create("RadiusBasedOnSize", typeof(bool), typeof(RoundedBoxView), false);
        /// <summary>
        /// Enable/Disable the decision whether the button's corner radius should be calculated based on button's size or not
        /// </summary>
        public bool RadiusBasedOnSize
        {
            get { return (bool)GetValue(RadiusBasedOnSizeProperty); }
            set { SetValue(RadiusBasedOnSizeProperty, value); }
        }

        public static readonly BindableProperty RadiusSizeRatioProperty =
            BindableProperty.Create("RadiusSizeRatio", typeof(double), typeof(RoundedBoxView), 0.5);
        /// <summary>
        /// If RadiusBasedOnSize is true, this ratio is used to calculate the corner radius based on button width/height
        /// </summary>
        public double RadiusSizeRatio
        {
            get { return (double)base.GetValue(RadiusSizeRatioProperty); }
            set { base.SetValue(RadiusSizeRatioProperty, value); }
        }
    }
}