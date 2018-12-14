using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BabyationApp.Controls.Views;
using BabyationApp.Helpers;
using Xamarin.Forms;

namespace BabyationApp.Controls.Buttons
{
    /// <summary>
    /// Button with no rounding
    /// </summary>
    public partial class FlatButton : ButtonEx
    {
        public FlatButton()
        {
            try
            {
                InitializeComponent();
                InitViewBackground = true;
            }
            catch (Exception exc)
            {
                System.Diagnostics.Debug.WriteLine("EXCEPTION-" + this.ToString() + "#" + exc.Message);
            }
        }

        /// <summary>
        /// Handles button state change to update the view color
        /// </summary>
        protected override void HandlePressedChanged()
        {
            base.HandlePressedChanged();
            TextCurrentColor = IsPressed ? TextPressedColor : TextColor;
        }


        public static readonly BindableProperty TextHorizontalOptionsProperty = BindableProperty.Create("TextHorizontalOptions", typeof(LayoutOptions), typeof(FlatButton), LayoutOptions.Center);
        /// <summary>
        /// Horizontal layout option for the button's text
        /// </summary>
        public LayoutOptions TextHorizontalOptions
        {
            get { return (LayoutOptions)GetValue(TextHorizontalOptionsProperty); }
            set { SetValue(TextHorizontalOptionsProperty, value); }
        }

        public static readonly BindableProperty TextVerticalOptionsProperty = BindableProperty.Create("TextVerticalOptions", typeof(LayoutOptions), typeof(FlatButton), LayoutOptions.Center);
        /// <summary>
        /// Vertical layout option for the button's text
        /// </summary>
        public LayoutOptions TextVerticalOptions
        {
            get { return (LayoutOptions)GetValue(TextVerticalOptionsProperty); }
            set { SetValue(TextVerticalOptionsProperty, value); }
        }


        public static readonly BindableProperty TextPaddingProperty = BindableProperty.Create("TextPadding", typeof(Thickness), typeof(FlatButton), new Thickness(0));
        /// <summary>
        /// Padding for the button text 
        /// </summary>
        public Thickness TextPadding
        {
            get { return (Thickness)GetValue(TextPaddingProperty); }
            set { SetValue(TextPaddingProperty, value); }
        }

        public static readonly BindableProperty TextProperty = BindableProperty.Create("Text", typeof(string), typeof(FlatButton), "");
        /// <summary>
        /// Button's text
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly BindableProperty TextColorProperty = BindableProperty.Create("TextColor", typeof(Color), typeof(FlatButton), Color.Black, propertyChanged: OnTextColorChanged);
        /// <summary>
        /// Normal state text color for this button
        /// </summary>
        public Color TextColor
        {
            get { return (Color)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }

        static void OnTextColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var self = bindable as FlatButton;
            if (self != null)
            {
                self.TextCurrentColor = (Color)newValue;
            }
        }

        public static readonly BindableProperty TextPressedColorProperty = BindableProperty.Create("TextPressedColor", typeof(Color), typeof(FlatButton), Color.White);
        /// <summary>
        /// Pressed state text color for this button
        /// </summary>
        public Color TextPressedColor
        {
            get { return (Color)GetValue(TextPressedColorProperty); }
            set { SetValue(TextPressedColorProperty, value); if (IsPressed) TextCurrentColor = value; }
        }


        public static readonly BindableProperty TextCurrentColorProperty = BindableProperty.Create("TextCurrentColor", typeof(Color), typeof(FlatButton), Color.White);
        /// <summary>
        /// Current state (normal/pressed) text color for this button
        /// </summary>
        public Color TextCurrentColor
        {
            get { return (Color)GetValue(TextCurrentColorProperty); }
            set { SetValue(TextCurrentColorProperty, value); }
        }


        public static readonly BindableProperty TextFontFamilyProperty = BindableProperty.Create("TextFontFamily", typeof(String), typeof(FlatButton), "fonts/HurmeGeometricSans3Bold.otf#HurmeGeometricSans3 Bold");
        /// <summary>
        /// Font family name for the button's text
        /// </summary>
        public String TextFontFamily
        {
            get { return (String)GetValue(TextFontFamilyProperty); }
            set { SetValue(TextFontFamilyProperty, value); }
        }


        public static readonly BindableProperty TextFontSizeProperty = BindableProperty.Create("TextFontSize", typeof(double), typeof(FlatButton), Device.GetNamedSize(NamedSize.Large, typeof(Label)));
        /// <summary>
        /// Font size for the button's text
        /// </summary>
        [TypeConverter(typeof(FontSizeConverter))]
        public double TextFontSize
        {
            get { return (double)GetValue(TextFontSizeProperty); }
            set { SetValue(TextFontSizeProperty, value); }
        }


        public static readonly BindableProperty TextFontAttributeProperty = BindableProperty.Create("TextFontAttributes", typeof(FontAttributes), typeof(FlatButton), FontAttributes.Bold);
        /// <summary>
        /// Font attributes for the button's text
        /// </summary>
        public FontAttributes TextFontAttributes
        {
            get { return (FontAttributes)GetValue(TextFontAttributeProperty); }
            set { SetValue(TextFontAttributeProperty, value); }
        }
    }
}
