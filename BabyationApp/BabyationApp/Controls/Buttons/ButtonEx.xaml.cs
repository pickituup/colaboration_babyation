using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BabyationApp.Controls.Views;

using Xamarin.Forms;
using System.Windows.Input;

namespace BabyationApp.Controls.Buttons
{
    /// <summary>
    /// A button that can be styled with normal/pressed color for background and text color
    /// </summary>
    public partial class ButtonEx : ButtonBase
    {        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <remarks>Find and initializes the background view</remarks>
        public ButtonEx()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception exc)
            {
                System.Diagnostics.Debug.WriteLine("EXCEPTION-" + this.ToString() + "#" + exc.Message);
            }

            if (BackgroundView == null)
            {
                BackgroundView = Helpers.VisualTreeHelper.GetTemplateChild<RoundedBoxView>(this, "_backgroundView");
            }

            PropertyChanged += (s, e) =>
            {
                if (InitViewBackground && e.PropertyName == BackgroundColorNormalProperty.PropertyName)
                {
                    this.BackgroundColor = BackgroundColorNormal;
                }
            };
        }

        public static readonly BindableProperty InitViewBackgroundProperty = BindableProperty.Create("InitViewBackground", typeof(bool), typeof(ButtonEx),  false);
        /// <summary>
        /// Gets/Sets whether to initialize the background color at first (override what set to default)
        /// </summary>
        public bool InitViewBackground
        {
            get { return (bool)GetValue(InitViewBackgroundProperty); }
            set { SetValue(InitViewBackgroundProperty, value); }
        }

        /// <summary>
        /// Handles button state change to update the view color
        /// </summary>
        protected override void HandlePressedChanged()
        {
            BackgroundColorCurrent = IsPressed ? BackgroundColorPressed : BackgroundColorNormal;
        }
        
        public static readonly BindableProperty BackgroundColorNormalProperty = BindableProperty.Create("BackgroundColorNormal", typeof(Color), typeof(ButtonEx), Color.Silver, BindingMode.TwoWay, propertyChanged: OnBackgroundColorNormalChanged);
        /// <summary>
        /// Buttons normal-state color
        /// </summary>
        public Color BackgroundColorNormal
        {
            get { return (Color)GetValue(BackgroundColorNormalProperty); }
            set { SetValue(BackgroundColorNormalProperty, value); }
        }


        static void OnBackgroundColorNormalChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var self = bindable as ButtonEx;

            if (self != null)
            {
                self.BackgroundColorCurrent = (Color)newValue;
            }
        }

        public static readonly BindableProperty BackgroundColorPressedProperty = BindableProperty.Create("BackgroundColorPressed", typeof(Color), typeof(ButtonEx), Color.FromHex("#EE4041"), BindingMode.TwoWay);
        /// <summary>
        /// Buttons pressed-state color
        /// </summary>
        public Color BackgroundColorPressed
        {
            get { return (Color)GetValue(BackgroundColorPressedProperty); }
            set { SetValue(BackgroundColorPressedProperty, value); }
        }

        public static readonly BindableProperty BackgroundColorCurrentProperty = BindableProperty.Create("BackgroundColorCurrent", typeof(Color), typeof(ButtonEx), Color.Transparent, BindingMode.TwoWay);
        /// <summary>
        /// Buttons current-state(could be normal, pressed or disabled) color
        /// </summary>
        public Color BackgroundColorCurrent
        {
            get { return (Color)GetValue(BackgroundColorCurrentProperty); }
            set { SetValue(BackgroundColorCurrentProperty, value); }
        }

        public static readonly BindableProperty BackgroundColorDisabledProperty = BindableProperty.Create("BackgroundColorDisabled", typeof(Color), typeof(ButtonEx), Color.Gray, BindingMode.TwoWay);

        /// <summary>
        /// Buttons disabled-state color
        /// </summary>
        public Color BackgroundColorDisabled
        {
            get { return (Color)GetValue(BackgroundColorDisabledProperty); }
            set { SetValue(BackgroundColorDisabledProperty, value); }
        }
    }
}
