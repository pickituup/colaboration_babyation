using BabyationApp.Controls.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace BabyationApp.Controls.Buttons
{
    /// <summary>
    /// Circle button which has 3 background views to show 3 cicles
    /// </summary>
    public partial class CircleButton : ButtonBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <remarks>Find and initializes the background views</remarks>
        public CircleButton()
        {
            try
            {
                InitializeComponent();
                IsCircle = true;
                Helpers.VisualTreeHelper.GetTemplateChild<RoundedBoxView>(this, "_outerCircle").IsCircle = true;
                Helpers.VisualTreeHelper.GetTemplateChild<RoundedBoxView>(this, "_middleCircle").IsCircle = true;
                Helpers.VisualTreeHelper.GetTemplateChild<RoundedBoxView>(this, "_innerCircle").IsCircle = true;
            }
            catch (Exception exc)
            {
                System.Diagnostics.Debug.WriteLine("EXCEPTION-" + this.ToString() + "#" + exc.Message);
            }

            BackgroundView = Helpers.VisualTreeHelper.GetTemplateChild<RoundedBoxView>(this, "_innerCircle");
        }

        /// <summary>
        /// Handles button state change to update the views color and image
        /// </summary>
        protected  override  void HandlePressedChanged()
        {
            OuterCircleCurrentColor =  (IsPressed ? OuterCirclePressedColor : OuterCircleColor);
            MiddleCircleCurrentColor = (IsPressed ? MiddleCirclePressedColor : MiddleCircleColor);
            InnerCircleCurrentColor =  (IsPressed ? InnerCirclePressedColor : InnerCircleColor);
            ImageCurrent = IsPressed ? ImagePressed : ImageNormal;
            TextTopCurrentColor = IsPressed ? TextTopPressedColor : TextTopColor;
            TextBottomCurrentColor = IsPressed ? TextBottomPressedColor : TextBottomColor;
        }


        public static readonly BindableProperty TextTopProperty = BindableProperty.Create("TextTop", typeof(string), typeof(CircleButton), "");
        /// <summary>
        /// Top text of the circle button
        /// </summary>
        public string TextTop
        {
            get { return (string)GetValue(TextTopProperty); }
            set { SetValue(TextTopProperty, value); }
        }

        public static readonly BindableProperty TextBottomProperty = BindableProperty.Create("TextBottom", typeof(string), typeof(CircleButton), "");
        /// <summary>
        /// Bottom text of the circle button
        /// </summary>
        public string TextBottom
        {
            get { return (string)GetValue(TextBottomProperty); }
            set { SetValue(TextBottomProperty, value); }
        }

        public static readonly BindableProperty TextTopColorProperty = BindableProperty.Create("TextTopColor", typeof(Color), typeof(CircleButton), Color.Black, propertyChanged: OnTextTopCurrentColorChanged);
        /// <summary>
        /// Top text normal-state color of the circle button
        /// </summary>
        public Color TextTopColor
        {
            get { return (Color)GetValue(TextTopColorProperty); }
            set { SetValue(TextTopColorProperty, value); }
        }

        public static readonly BindableProperty TextTopPressedColorProperty = BindableProperty.Create("TextTopPressedColor", typeof(Color), typeof(CircleButton), Color.White);
        /// <summary>
        /// Top text pressed-state color of the circle button
        /// </summary>
        public Color TextTopPressedColor
        {
            get { return (Color)GetValue(TextTopPressedColorProperty); }
            set { SetValue(TextTopPressedColorProperty, value); }
        }

        static void OnTextTopCurrentColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var self = bindable as CircleButton;
            if (self != null)
            {
                self.TextTopCurrentColor = (Color)newValue;
            }
        }

        public static readonly BindableProperty TextTopCurrentColorProperty = BindableProperty.Create("TextTopCurrentColor", typeof(Color), typeof(CircleButton), Color.Black);
        /// <summary>
        /// Top text's current state (normal or pressed) color
        /// </summary>
        public Color TextTopCurrentColor
        {
            get { return (Color)GetValue(TextTopCurrentColorProperty); }
            set { SetValue(TextTopCurrentColorProperty, value); }
        }


        public static readonly BindableProperty TextBottomColorProperty = BindableProperty.Create("TextBottomColor", typeof(Color), typeof(CircleButton), Color.Black, propertyChanged: OnTextBottomCurrentColorChanged);
        /// <summary>
        /// Bottom text normal-state color of the circle button
        /// </summary>
        public Color TextBottomColor
        {
            get { return (Color)GetValue(TextBottomColorProperty); }
            set { SetValue(TextBottomColorProperty, value); }
        }


        public static readonly BindableProperty TextBottomPressedColorProperty = BindableProperty.Create("TextBottomPressedColor", typeof(Color), typeof(CircleButton), Color.White);
        /// <summary>
        /// Bottom text pressed-state color of the circle button
        /// </summary>
        public Color TextBottomPressedColor
        {
            get { return (Color)GetValue(TextBottomPressedColorProperty); }
            set { SetValue(TextBottomPressedColorProperty, value); }
        }
        static void OnTextBottomCurrentColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var self = bindable as CircleButton;
            if (self != null)
            {
                self.TextBottomCurrentColor = (Color)newValue;
            }
        }

        public static readonly BindableProperty TextBottomCurrentColorProperty = BindableProperty.Create("TextBottomCurrentColor", typeof(Color), typeof(CircleButton), Color.Black);
        /// <summary>
        /// Bottom text's current state (normal or pressed) color
        /// </summary>
        public Color TextBottomCurrentColor
        {
            get { return (Color)GetValue(TextBottomCurrentColorProperty); }
            set { SetValue(TextBottomCurrentColorProperty, value); }
        }

        public static readonly BindableProperty TextTopBottomSpacingProperty = BindableProperty.Create("TextTopBottomSpacing", typeof(double), typeof(CircleButton), 0.0);
        /// <summary>
        /// Spacing between top-bottom texts
        /// </summary>
        public double TextTopBottomSpacing
        {
            get { return (double)GetValue(TextTopBottomSpacingProperty); }
            set { SetValue(TextTopBottomSpacingProperty, value); }
        }

        public static readonly BindableProperty TextTopFontSizeProperty = BindableProperty.Create("TextTopFontSize", typeof(double), typeof(CircleButton), Device.GetNamedSize(NamedSize.Large, typeof(Label)));
        /// <summary>
        /// Top text font size
        /// </summary>
        [TypeConverter(typeof(FontSizeConverter))]
        public double TextTopFontSize
        {
            get { return (double)GetValue(TextTopFontSizeProperty); }
            set { SetValue(TextTopFontSizeProperty, value); }
        }

        public static readonly BindableProperty TextBottomFontSizeProperty = BindableProperty.Create("TextBottomFontSize", typeof(double), typeof(CircleButton), Device.GetNamedSize(NamedSize.Small, typeof(Label)));

        /// <summary>
        /// Text bottom font size
        /// </summary>
        [TypeConverter(typeof(FontSizeConverter))]
        public double TextBottomFontSize
        {
            get { return (double)GetValue(TextBottomFontSizeProperty); }
            set { SetValue(TextBottomFontSizeProperty, value); }
        }


        public static readonly BindableProperty FontFamilyTopProperty = BindableProperty.Create("FontFamilyTop", typeof(String), typeof(CircleButton), "fonts/HurmeGeometricSans3Bold.otf#HurmeGeometricSans3 Bold");
        /// <summary>
        /// Top text font family name
        /// </summary>
        public String FontFamilyTop
        {
            get { return (String)GetValue(FontFamilyTopProperty); }
            set { SetValue(FontFamilyTopProperty, value); }
        }


        public static readonly BindableProperty FontFamilyBottomProperty = BindableProperty.Create("FontFamilyBottom", typeof(String), typeof(CircleButton), "fonts/HurmeGeometricSans3Bold.otf#HurmeGeometricSans3 Regular");

        /// <summary>
        /// Text bottom font family name
        /// </summary>
        public String FontFamilyBottom
        {
            get { return (String)GetValue(FontFamilyBottomProperty); }
            set { SetValue(FontFamilyBottomProperty, value); }
        }

		public static readonly BindableProperty FontAttributesTopProperty = BindableProperty.Create("FontAttributesTop", typeof(FontAttributes), typeof(CircleButton), FontAttributes.Bold);
        /// <summary>
        /// Top text font-attributes
        /// </summary>
        public FontAttributes FontAttributesTop
		{
			get { return (FontAttributes)GetValue(FontAttributesTopProperty); }
			set { SetValue(FontAttributesTopProperty, value); }
		}

		public static readonly BindableProperty FontAttributesBottomProperty = BindableProperty.Create("FontAttributesBottom", typeof(FontAttributes), typeof(CircleButton), FontAttributes.None);
        /// <summary>
        /// Botton text font-attributes
        /// </summary>
        public FontAttributes FontAttributesBottom
		{
			get { return (FontAttributes)GetValue(FontAttributesBottomProperty); }
			set { SetValue(FontAttributesBottomProperty, value); }
		}


        public static readonly BindableProperty OuterCircleColorProperty = BindableProperty.Create("OuterCircleColor", typeof(Color), typeof(CircleButton), Color.FromHex("#11719C"), propertyChanged: OnOuterCircleColorChanged);
        /// <summary>
        /// Outer view normal-state color
        /// </summary>
        public Color OuterCircleColor
        {
            get { return (Color)GetValue(OuterCircleColorProperty); }
            set { SetValue(OuterCircleColorProperty, value); }
        }

        static void OnOuterCircleColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var self = bindable as CircleButton;
            if (self != null)
            {
                self.OuterCircleCurrentColor = (Color)newValue;
            }
        }

        public static readonly BindableProperty MiddleCircleColorProperty = BindableProperty.Create("MiddleCircleColor", typeof(Color), typeof(CircleButton), Color.FromHex("#11719C"), propertyChanged: OnMiddleCircleColorChanged);
        /// <summary>
        /// Middle view normal-state color
        /// </summary>
        public Color MiddleCircleColor
        {
            get { return (Color)GetValue(MiddleCircleColorProperty); }
            set { SetValue(MiddleCircleColorProperty, value); }
        }

        static void OnMiddleCircleColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var self = bindable as CircleButton;
            if (self != null)
            {
                self.MiddleCircleCurrentColor = (Color)newValue;
            }
        }

        public static readonly BindableProperty InnerCircleColorProperty = BindableProperty.Create("InnerCircleColor", typeof(Color), typeof(CircleButton), Color.FromHex("#093F5B"), propertyChanged: OnInnerCircleColorChanged);
        /// <summary>
        /// Inner view normal-state color
        /// </summary>
        public Color InnerCircleColor
        {
            get { return (Color)GetValue(InnerCircleColorProperty); }
            set { SetValue(InnerCircleColorProperty, value);}
        }

        static void OnInnerCircleColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var self = bindable as CircleButton;
            if (self != null)
            {
                self.InnerCircleCurrentColor = (Color)newValue;
            }
        }


        public static readonly BindableProperty OuterCirclePressedColorProperty = BindableProperty.Create("OuterCirclePressedColor", typeof(Color), typeof(CircleButton), Color.FromHex("#EE4041"));
        /// <summary>
        /// Outer view pressed-state color
        /// </summary>
        public Color OuterCirclePressedColor
        {
            get { return (Color)GetValue(OuterCirclePressedColorProperty); }
            set { SetValue(OuterCirclePressedColorProperty, value); if (IsToggled) OuterCircleCurrentColor = value; }
        }

        public static readonly BindableProperty MiddleCirclePressedColorProperty = BindableProperty.Create("MiddleCirclePressedColor", typeof(Color), typeof(CircleButton), Color.White);
        /// <summary>
        /// Middle view pressed-state color
        /// </summary>
        public Color MiddleCirclePressedColor
        {
            get { return (Color)GetValue(MiddleCirclePressedColorProperty); }
            set { SetValue(MiddleCirclePressedColorProperty, value); if (IsToggled) MiddleCircleCurrentColor = value; }
        }

        public static readonly BindableProperty InnerCirclePressedColorProperty = BindableProperty.Create("InnerCirclePressedColor", typeof(Color), typeof(CircleButton), Color.FromHex("#EE4041"));
        /// <summary>
        /// Inner view pressed-state color
        /// </summary>
        public Color InnerCirclePressedColor
        {
            get { return (Color)GetValue(InnerCirclePressedColorProperty); }
            set { SetValue(InnerCirclePressedColorProperty, value); if (IsToggled) InnerCircleCurrentColor = value; }
        }


        public static readonly BindableProperty OuterCircleCurrentColorProperty = BindableProperty.Create("OuterCircleCurrentColor", typeof(Color), typeof(CircleButton), Color.FromHex("#11719C"));
        /// <summary>
        /// Outer view current-state (normal/pressed) color
        /// </summary>
        public Color OuterCircleCurrentColor
        {
            get { return (Color)GetValue(OuterCircleCurrentColorProperty); }
            set { SetValue(OuterCircleCurrentColorProperty, value); }
        }

        public static readonly BindableProperty MiddleCircleCurrentColorProperty = BindableProperty.Create("MiddleCircleCurrentColor", typeof(Color), typeof(CircleButton), Color.FromHex("#11719C"));
        /// <summary>
        /// Middle view current-state (normal/pressed) color
        /// </summary>
        public Color MiddleCircleCurrentColor
        {
            get { return (Color)GetValue(MiddleCircleCurrentColorProperty); }
            set { SetValue(MiddleCircleCurrentColorProperty, value); }
        }

        public static readonly BindableProperty InnerCircleCurrentColorProperty = BindableProperty.Create("InnerCircleCurrentColor", typeof(Color), typeof(CircleButton), Color.FromHex("#093F5B"));
        /// <summary>
        /// Inner view current-state (normal/pressed) color
        /// </summary>
        public Color InnerCircleCurrentColor
        {
            get { return (Color)GetValue(InnerCircleCurrentColorProperty); }
            set { SetValue(InnerCircleCurrentColorProperty, value); }
        }


        public static readonly BindableProperty MiddleCirclePaddingProperty = BindableProperty.Create("MiddleCirclePadding", typeof(Thickness), typeof(CircleButton), new Thickness(3));
        /// <summary>
        /// Padding between outer & middle views
        /// </summary>
        public Thickness MiddleCirclePadding
        {
            get { return (Thickness)GetValue(MiddleCirclePaddingProperty); }
            set { SetValue(MiddleCirclePaddingProperty, value); }
        }


        public static readonly BindableProperty InnerCirclePaddingProperty = BindableProperty.Create("InnerCirclePadding", typeof(Thickness), typeof(CircleButton), new Thickness(6));
        /// <summary>
        /// Padding between inner & middle views
        /// </summary>
        public Thickness InnerCirclePadding
        {
            get { return (Thickness)GetValue(InnerCirclePaddingProperty); }
            set { SetValue(InnerCirclePaddingProperty, value); }
        }

		public static readonly BindableProperty ContentPaddingProperty = BindableProperty.Create("ContentPadding", typeof(Thickness), typeof(CircleButton), new Thickness(0));
        /// <summary>
        /// Button's content padding
        /// </summary>
        public Thickness ContentPadding
		{
			get { return (Thickness)GetValue(ContentPaddingProperty); }
			set { SetValue(ContentPaddingProperty, value); }
		}

        public static readonly BindableProperty ImageNormalProperty = BindableProperty.Create("ImageNormal", typeof(ImageSource), typeof(CircleButton), propertyChanged: OnImageNormalChanged);
        /// <summary>
        /// Normal state image for the button
        /// </summary>
        public ImageSource ImageNormal
        {
            get { return (ImageSource)GetValue(ImageNormalProperty); }
            set { SetValue(ImageNormalProperty, value); }
        }

        static void OnImageNormalChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var self = bindable as CircleButton;
            if (self != null)
            {
                self.ImageCurrent = (ImageSource)newValue;
            }
        }

        public static readonly BindableProperty ImagePressedProperty = BindableProperty.Create("ImagePressed", typeof(ImageSource), typeof(CircleButton), null);
        /// <summary>
        /// Pressed state image for the button
        /// </summary>
        public ImageSource ImagePressed
        {
            get { return (ImageSource)GetValue(ImagePressedProperty); }
            set { SetValue(ImagePressedProperty, value); }
        }

        public static readonly BindableProperty ImageCurrentProperty = BindableProperty.Create("ImageCurrent", typeof(ImageSource), typeof(CircleButton), null);
        /// <summary>
        /// Current state (normal/pressed) image for the button
        /// </summary>
        public ImageSource ImageCurrent
        {
            get { return (ImageSource)GetValue(ImageCurrentProperty); }
            set { SetValue(ImageCurrentProperty, value); }
        }
    }
}
