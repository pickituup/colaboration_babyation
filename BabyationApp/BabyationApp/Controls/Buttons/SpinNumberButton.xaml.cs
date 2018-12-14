using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace BabyationApp.Controls.Buttons
{
    /// <summary>
    /// Delete for Spin's value update
    /// </summary>
    /// <param name="value"></param>
    public delegate void SpinValueUpdated(int value);

    /// <summary>
    /// The up/down number button
    /// </summary>
    public partial class SpinNumberButton : ContentView
    {
        private const String DefaultValue = "--";

        /// <summary>
        /// Event to be fired on Up click
        /// </summary>
        public EventHandler UpClicked;

        /// <summary>
        /// Event to be fired on Down click
        /// </summary>
        public EventHandler DownClicked;

        /// <summary>
        /// Event to be fired on value update
        /// </summary>
        public event SpinValueUpdated ValueUpdated;


        /// <summary>
        /// Constructor -- initializes the default values
        /// </summary>
        public SpinNumberButton()
        {
            BindingContext = this;
            InitializeComponent();
            RatioSmall = 6.0 / 20.0;
            OverlapSize = 12;
            Step = 1;
            _rl1.SizeChanged += RelativeLayout1_SizeChanged;
            _circleUp.Clicked += OnUpButtonClicked;
            _circleDown.Clicked += OnDownButtonClicked;
			_circleView.MiddleCirclePadding = 6;
			_circleView.InnerCirclePadding = 9;
			_circleView.FontFamilyTop = "fonts/LarsseitMedium.otf#Larsseit Medium";
			_circleView.FontFamilyBottom = "fonts/HurmeGeometricSans3Bold.otf#HurmeGeometricSans3 Bold";
			_circleView.FontAttributesTop = FontAttributes.None;
			_circleView.FontAttributesBottom = FontAttributes.Bold;
			_circleView.ContentPadding = new Thickness(0, 10, 0, 0);

            MinValue = 1;
            MaxValue = 10;
            Value = 0;
        }

        /// <summary>
        /// Update images to bigger version
        /// </summary>
        public void UpdateToBigUpDown()
        {
            _circleUp.ImageNormal = "icon_plus.png";
            _circleUp.ImagePressed = "icon_plus_pressed.png";
            _circleDown.ImageNormal = "icon_minus.png";
            _circleDown.ImagePressed = "icon_minus_pressed.png";
        }

        /// <summary>
        /// Constant ratio between the big and small circles
        /// </summary>
		public static readonly double RatioBig = 8.0 / 20.0;

        /// <summary>
        /// Smaller cicle ratio
        /// </summary>
        public double RatioSmall { get; set; } 

        /// <summary>
        /// Big and small cicles overlap size
        /// </summary>
        public double OverlapSize { get; set; }

        /// <summary>
        /// Gets/Sets Spin button's min value
        /// </summary>
        public  int MinValue { get; set; }

        /// <summary>
        /// Gets/Sets Spin buttons max value
        /// </summary>
        public int MaxValue { get; set; }        

        /// <summary>
        /// Recalculates/Reposition the big/small cicles on resizing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RelativeLayout1_SizeChanged(object sender, EventArgs e)
        {            
            if (_rl1.Width <=0 || _rl1.Height <= 0) return;

            var minRadi = Math.Min(_rl1.Width * 2, _rl1.Height);
			var L = minRadi * RatioBig;
			var l = minRadi * RatioSmall;

			var x1 = (_rl1.Width - L) / 2;
			var y1 = (_rl1.Height - L) / 2;
            var c1 = BoundsConstraint.FromExpression((Expression<Func<Rectangle>>)(() => new Rectangle(x1, y1, L, L)));
            RelativeLayout.SetBoundsConstraint(_circleView, c1);

			var x2 = (_rl1.Width - l) / 2;
			var y2 = (_rl1.Height - L) / 2 - l + OverlapSize;
            var c2 = BoundsConstraint.FromExpression((Expression<Func<Rectangle>>)(() => new Rectangle(x2, y2, l, l)));
            RelativeLayout.SetBoundsConstraint(_circleUp, c2);

            var x3 = (_rl1.Width - l) / 2;
            var y3 = (_rl1.Height + L ) / 2 - OverlapSize;
            var c3 = BoundsConstraint.FromExpression((Expression<Func<Rectangle>>)(() => new Rectangle(x3, y3, l, l)));
            RelativeLayout.SetBoundsConstraint(_circleDown, c3);

            _rl1.ForceLayout();
        }

        /// <summary>
        /// Spin incrementing/decrementing Step
        /// </summary>
        public int Step { get; set; }
        
        /// <summary>
        /// Description of the spin button
        /// </summary>
        public string DescriptionText
        {
            get { return _circleView.TextTop; }
            set { _circleView.TextTop = value; }
        }

        private int _value;

        /// <summary>
        /// Gets/Sets current value for the spin button
        /// </summary>
        public int Value
        {
            get { return _value; }
            set
            {
                _value = value;

                if (value >= MinValue && value <= MaxValue)
                {                    
                    ValueText = value.ToString();
                }
                else
                {
                    ValueText = DefaultValue;
                }

                ValueUpdated?.Invoke(_value);
            }
        }

        /// <summary>
        /// Checks if the current value is in min/max range
        /// </summary>
        /// <returns>True if current value is valid; otherwise returns false</returns>
        public bool IsValid()
        {
            return _value >= MinValue && _value <= MaxValue;
        }

        public static readonly BindableProperty ValueTextProperty = BindableProperty.Create("ValueText", typeof(string), typeof(SpinNumberButton), "0");
        /// <summary>
        /// Gets/Sets Text version of the value
        /// </summary>
        public string ValueText
        {
            get { return (string)GetValue(ValueTextProperty); }
            set
            {
                SetValue(ValueTextProperty, value);
                _circleView.TextBottom = value;
            }
        }


        /// <summary>
        /// Up circle button click handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnUpButtonClicked(object sender, EventArgs e)
        {
            try
            {
                if (UpClicked != null)
                {
                    UpClicked(this, e);
                }
                else
                {
                    if (_value < MinValue)
                    {
                        Value = MinValue;
                    }
                    else if (_value > MaxValue)
                    {
                        Value = MaxValue;
                    }
                    else
                    {
                        Value = Math.Min(_value + Step, MaxValue);
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// Down circle button click handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDownButtonClicked(object sender, EventArgs e)
        {
            try
            {
                if (DownClicked != null)
                {
                    DownClicked(this, e);
                }
                else
                {
                    if (_value < MinValue)
                    {
                        Value = MinValue;
                    }
                    else if (_value > MaxValue)
                    {
                        Value = MaxValue;
                    }
                    else
                    {
                        Value = Math.Max(_value - Step, MinValue);
                    }
                }
            }
            catch (Exception exc)
            {
                System.Diagnostics.Debug.WriteLine(exc.Message);
            }
        }

        
    }
}
