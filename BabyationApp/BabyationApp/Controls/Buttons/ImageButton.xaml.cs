using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BabyationApp.Controls.Views;
using BabyationApp.Helpers;
using FFImageLoading.Forms;
using Xamarin.Forms;

namespace BabyationApp.Controls.Buttons
{
    /// <summary>
    /// A button with images for states (normal, pressed, disabled)
    /// </summary>
    public partial class ImageButton : ButtonEx
    {
        /// <summary>
        /// ImageButton's constructor
        /// </summary>
        public ImageButton()
        {
            try
            {
                InitializeComponent();
                HandlePressedChanged();

                ImageTranslationY = Device.RuntimePlatform == Device.Android ? 1.4 : -2;

                OnTemplateStrategy();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                throw;
            }
        }

        public static readonly BindableProperty TemplateStrategyProperty =
            BindableProperty.Create(propertyName: nameof(TemplateStrategy),
                                    returnType: typeof(ImageButtonTemplateStrategies),
                                    declaringType: typeof(ImageButton),
                                    defaultValue: default(ImageButtonTemplateStrategies),
                                    propertyChanged: (BindableObject bindable, object oldValue, object newValue) => { if (bindable is ImageButton declarer) declarer.OnTemplateStrategy(); });

        public ImageButtonTemplateStrategies TemplateStrategy
        {
            get { return (ImageButtonTemplateStrategies)GetValue(TemplateStrategyProperty); }
            set { SetValue(TemplateStrategyProperty, value); }
        }

        private bool _isEnabledTemplateStrategy = false;
        public bool IsEnabledTemplateStrategy
        {
            get => _isEnabledTemplateStrategy;
            set
            {
                _isEnabledTemplateStrategy = value;
                OnTemplateStrategy();
            }
        }

        public bool IsSelectedActionAvailable { get; set; }

        public ImageButtonActionStrategies ActionStartegy { get; set; }

        private void OnTemplateStrategy()
        {
            try
            {
                if (IsEnabledTemplateStrategy)
                {
                    switch (TemplateStrategy)
                    {
                        case ImageButtonTemplateStrategies.Default:
                            _mainContent_ContentView.ControlTemplate = (ControlTemplate)Resources["ImageButtonTemplate"];
                            break;
                        case ImageButtonTemplateStrategies.Absolute:
                            _mainContent_ContentView.ControlTemplate = (ControlTemplate)Resources["AbsoluteTemplate"];
                            break;
                        default:
                            throw new InvalidOperationException("Unsuported ImageButtonTemplateStrategies");
                    }
                }
                else
                {
                    _mainContent_ContentView.ControlTemplate = (ControlTemplate)Resources["ImageButtonTemplate"];
                }
            }
            catch (Exception exc)
            {
                Debugger.Break();
                throw new InvalidOperationException(string.Format("ImageButton.OnTemplateStrategy. {0}", exc.Message), exc);
            }
        }

        protected override void OnIsPressed()
        {
            base.OnIsPressed();

            try
            {
                if (IsSelectedActionAvailable)
                {
                    switch (ActionStartegy)
                    {
                        case ImageButtonActionStrategies.RotateOn180:
                            if (IsPressed)
                            {

                                ((View)((Layout)_mainContent_ContentView).Children[0]).RotationX = 180;
                            }
                            else
                            {
                                ((View)((Layout)_mainContent_ContentView).Children[0]).RotationX = 0;
                            }
                            break;
                        default:
                            throw new InvalidOperationException("Unsuported ImageButtonActionStrategies");
                    }
                }
            }
            catch (Exception exc)
            {
                Debugger.Break();
                throw new InvalidOperationException(string.Format("ImageButton.OnIsPressed. {0}", exc.Message), exc);
            }
        }

        /// <summary>
        /// Handles button state change to update the view image
        /// </summary>
        protected override void HandlePressedChanged()
        {
            base.HandlePressedChanged();
            ImageCurrent = IsPressed ? ImagePressed : ImageNormal;
            TextCurrentColor = IsPressed ? TextPressedColor : TextColor;
        }

        public static readonly BindableProperty ImageTranslationYProperty =
            BindableProperty.Create(propertyName: "ImageTranslationY",
                                    returnType: typeof(double),
                                    declaringType: typeof(ImageButton),
                                    defaultValue: default(double));

        public double ImageTranslationY
        {
            get { return (double)GetValue(ImageTranslationYProperty); }
            set { SetValue(ImageTranslationYProperty, value); }
        }

        public static readonly BindableProperty ImageScaleProperty =
            BindableProperty.Create(propertyName: "ImageScale",
                                    returnType: typeof(double),
                                    declaringType: typeof(ImageButton),
                                    defaultValue: 1.0);

        public double ImageScale
        {
            get { return (double)GetValue(ImageScaleProperty); }
            set { SetValue(ImageScaleProperty, value); }
        }

        public static readonly BindableProperty ImageTextHorizontalOptionsProperty = BindableProperty.Create("ImageTextHorizontalOptions", typeof(LayoutOptions), typeof(ImageButton), LayoutOptions.Center);
        /// <summary>
        /// Horizontal layout options for the image and text
        /// </summary>
        public LayoutOptions ImageTextHorizontalOptions
        {
            get { return (LayoutOptions)GetValue(ImageTextHorizontalOptionsProperty); }
            set { SetValue(ImageTextHorizontalOptionsProperty, value); }
        }

        public static readonly BindableProperty ImageTextVerticalOptionsProperty = BindableProperty.Create("ImageTextVerticalOptions", typeof(LayoutOptions), typeof(ImageButton), LayoutOptions.Center);
        /// <summary>
        /// Vertical layout options for the image and text
        /// </summary>
        public LayoutOptions ImageTextVerticalOptions
        {
            get { return (LayoutOptions)GetValue(ImageTextVerticalOptionsProperty); }
            set { SetValue(ImageTextVerticalOptionsProperty, value); }
        }

        public static readonly BindableProperty ImageTextOrientationProperty = BindableProperty.Create("ImageTextOrientation", typeof(StackOrientation), typeof(ImageButton), StackOrientation.Horizontal);
        /// <summary>
        /// Orientation for the image and text
        /// </summary>
        public StackOrientation ImageTextOrientation
        {
            get { return (StackOrientation)GetValue(ImageTextOrientationProperty); }
            set { SetValue(ImageTextOrientationProperty, value); }
        }

        public static readonly BindableProperty ImageTextSpacingProperty = BindableProperty.Create("ImageTextSpacing", typeof(double), typeof(ImageButton), 8.0);
        /// <summary>
        /// Spacing between image and text
        /// </summary>
        public double ImageTextSpacing
        {
            get { return (double)GetValue(ImageTextSpacingProperty); }
            set { SetValue(ImageTextSpacingProperty, value); }
        }

        public static readonly BindableProperty ImageTextPaddingProperty = BindableProperty.Create("ImageTextPadding", typeof(Thickness), typeof(ImageButton), new Thickness(10));
        /// <summary>
        /// Padding for image/text content
        /// </summary>
        public Thickness ImageTextPadding
        {
            get { return (Thickness)GetValue(ImageTextPaddingProperty); }
            set { SetValue(ImageTextPaddingProperty, value); }
        }


        public static readonly BindableProperty ImageNormalProperty = BindableProperty.Create("ImageNormal", typeof(ImageSource), typeof(ImageButton), propertyChanged: OnImageNormalChanged);
        /// <summary>
        /// Normal state image
        /// </summary>
        public ImageSource ImageNormal
        {
            get { return (ImageSource)GetValue(ImageNormalProperty); }
            set { SetValue(ImageNormalProperty, value); }
        }

        static void OnImageNormalChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var self = bindable as ImageButton;
            if (self != null)
            {
                self.ImageCurrent = newValue as ImageSource;
            }
        }

        public static readonly BindableProperty ImagePressedProperty = BindableProperty.Create("ImagePressed", typeof(ImageSource), typeof(ImageButton));
        /// <summary>
        /// Pressed state image
        /// </summary>
        public ImageSource ImagePressed
        {
            get { return (ImageSource)GetValue(ImagePressedProperty); }
            set { SetValue(ImagePressedProperty, value); if (IsPressed) ImageCurrent = value; }
        }

        public static readonly BindableProperty ImageCurrentProperty = BindableProperty.Create("ImageCurrent", typeof(ImageSource), typeof(ImageButton));
        /// <summary>
        /// Current state (normal/pressed) image
        /// </summary>
        public ImageSource ImageCurrent
        {
            get { return (ImageSource)GetValue(ImageCurrentProperty); }
            set { SetValue(ImageCurrentProperty, value); }
        }

        public static readonly BindableProperty ImageDisabledProperty = BindableProperty.Create("ImageDisabled", typeof(ImageSource), typeof(ImageButton));
        /// <summary>
        /// Disabled state image
        /// </summary>
        public ImageSource ImageDisabled
        {
            get { return (ImageSource)GetValue(ImageDisabledProperty); }
            set { SetValue(ImageDisabledProperty, value); }
        }

        public static readonly BindableProperty TextProperty = BindableProperty.Create("Text", typeof(string), typeof(ImageButton), null);

        /// <summary>
        /// Button's text
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly BindableProperty TextColorProperty = BindableProperty.Create("TextColor", typeof(Color), typeof(ImageButton), Color.Black, propertyChanged: OnTextColorChanged);
        /// <summary>
        /// Normal state text color for the button
        /// </summary>
        public Color TextColor
        {
            get { return (Color)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }

        static void OnTextColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var self = bindable as ImageButton;
            if (self != null)
            {
                self.TextCurrentColor = (Color)newValue;
            }
        }

        public static readonly BindableProperty TextPressedColorProperty = BindableProperty.Create("TextPressedColor", typeof(Color), typeof(ImageButton), Color.White);
        /// <summary>
        /// Pressed state text color for the button
        /// </summary>
        public Color TextPressedColor
        {
            get { return (Color)GetValue(TextPressedColorProperty); }
            set { SetValue(TextPressedColorProperty, value); if (IsPressed) TextCurrentColor = value; }
        }


        public static readonly BindableProperty TextCurrentColorProperty = BindableProperty.Create("TextCurrentColor", typeof(Color), typeof(ImageButton), Color.White);
        /// <summary>
        /// Current state (normal/pressed) text color for the button
        /// </summary>
        public Color TextCurrentColor
        {
            get { return (Color)GetValue(TextCurrentColorProperty); }
            set { SetValue(TextCurrentColorProperty, value); }
        }


        public static readonly BindableProperty TextFontFamilyProperty = BindableProperty.Create("TextFontFamily", typeof(String), typeof(ImageButton), "fonts/HurmeGeometricSans3Bold.otf#HurmeGeometricSans3 Bold");
        /// <summary>
        /// Font family name for the button's text
        /// </summary>
        public String TextFontFamily
        {
            get { return (String)GetValue(TextFontFamilyProperty); }
            set { SetValue(TextFontFamilyProperty, value); }
        }


        public static readonly BindableProperty TextFontSizeProperty = BindableProperty.Create("TextFontSize", typeof(double), typeof(ImageButton), Device.GetNamedSize(NamedSize.Large, typeof(Label)));
        /// <summary>
        /// Font size for the button's text
        /// </summary>
        [TypeConverter(typeof(FontSizeConverter))]
        public double TextFontSize
        {
            get { return (double)GetValue(TextFontSizeProperty); }
            set { SetValue(TextFontSizeProperty, value); }
        }


        public static readonly BindableProperty TextFontAttributeProperty = BindableProperty.Create("TextFontAttributes", typeof(FontAttributes), typeof(ImageButton), FontAttributes.Bold);
        /// <summary>
        /// Font attributes for the button's text
        /// </summary>
        public FontAttributes TextFontAttributes
        {
            get { return (FontAttributes)GetValue(TextFontAttributeProperty); }
            set { SetValue(TextFontAttributeProperty, value); }
        }
    }

    public enum ImageButtonTemplateStrategies
    {
        Default,
        Absolute
    }

    public enum ImageButtonActionStrategies
    {
        RotateOn180
    }
}
