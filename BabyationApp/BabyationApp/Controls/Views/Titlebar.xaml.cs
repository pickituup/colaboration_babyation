using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BabyationApp.Controls.Buttons;
using Xamarin.Forms;

namespace BabyationApp.Controls.Views
{
    /// <summary>
    /// Composite titlebar for the views/pages of this application
    /// </summary>
    public partial class Titlebar : ContentView
    {
        private ImageButton _btnLeft, _btnRight;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <remarks>
        /// Finds left/right button from the visual tree view
        /// </remarks>
        public Titlebar()
        {
            InitializeComponent();

            HeightRequest = Double.Parse(Application.Current.Resources["NavBarHeight"].ToString());

            _btnLeft = Helpers.VisualTreeHelper.GetTemplateChild<ImageButton>(this, "BtnLeft");
            _btnRight = Helpers.VisualTreeHelper.GetTemplateChild<ImageButton>(this, "BtnRight");
        }

        public static readonly BindableProperty TitleBackColorProperty = BindableProperty.Create("TitleBackColor", typeof(Color), typeof(Titlebar), (Color)Application.Current.Resources["MedPink"]); //Color.FromHex("#F9DCD9"));
        /// <summary>
        /// Gets/Sets titlebar background color
        /// </summary>
        public Color TitleBackColor
        {
            get { return (Color)GetValue(TitleBackColorProperty); }
            set { SetValue(TitleBackColorProperty, value); }
        }

        public static readonly BindableProperty TitleTextColorProperty = BindableProperty.Create("TitleTextColor", typeof(Color), typeof(Titlebar), (Color)Application.Current.Resources["Green"]);//Color.FromHex("#093954"));
        /// <summary>
        /// Gets/Sets titlebar text color
        /// </summary>
        public Color TitleTextColor
        {
            get { return (Color)GetValue(TitleTextColorProperty); }
            set { SetValue(TitleTextColorProperty, value); }
        }

        public static readonly BindableProperty TitleProperty = BindableProperty.Create("Title", typeof(String), typeof(Titlebar), "TITLE");
        /// <summary>
        /// Gets/Sets title of the titlebar
        /// </summary>
        public String Title
        {
            get { return (String)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly BindableProperty TitleFontFamilyProperty = BindableProperty.Create("TitleFontFamily", typeof(String), typeof(Titlebar), "fonts/HurmeGeometricSans3Bold.otf#HurmeGeometricSans3 Bold");
        /// <summary>
        /// Gets/Sets titlebar text font family
        /// </summary>
        public String TitleFontFamily
        {
            get { return (String)GetValue(TitleFontFamilyProperty); }
            set { SetValue(TitleFontFamilyProperty, value); }
        }

        public static readonly BindableProperty TitleFontSizeProperty = BindableProperty.Create("TitleFontSize", typeof(double), typeof(Titlebar), 14.0);
        /// <summary>
        /// Gets/Sets titlebar text font size
        /// </summary>
        public double TitleFontSize
        {
            get { return (double)GetValue(TitleFontSizeProperty); }
            set { SetValue(TitleFontSizeProperty, value); }
        }

        /// <summary>
        /// Gets the title left button control
        /// </summary>
        public  ImageButton LeftButton { get { return _btnLeft; } }

        /// <summary>
        /// Gets the title right button control
        /// </summary>
        public ImageButton RightButton { get { return _btnRight; } }

    }
}
