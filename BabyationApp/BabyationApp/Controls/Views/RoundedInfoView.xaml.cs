using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace BabyationApp.Controls.Views
{
    /// <summary>
    /// Composite views to show information in rounded views. 
    /// </summary>
    /// <remarks>
    /// Used in SelectSpeedPage and EnterOtherInfoPage
    /// </remarks>
    public partial class RoundedInfoView : ContentView
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public RoundedInfoView()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty TextTopProperty = BindableProperty.Create("TextTop", typeof(string), typeof(RoundedInfoView), "Text Top");
        /// <summary>
        /// Gets/Sets Top text of the rounded view
        /// </summary>
        public string TextTop
        {
            get { return (string)GetValue(TextTopProperty); }
            set { SetValue(TextTopProperty, value); }
        }

        public static readonly BindableProperty TextBottomProperty = BindableProperty.Create("TextBottom", typeof(string), typeof(RoundedInfoView), "Text Bottom");
        /// <summary>
        /// Gets/Sets bottom text of the rounded view
        /// </summary>
        public string TextBottom
        {
            get { return (string)GetValue(TextBottomProperty); }
            set { SetValue(TextBottomProperty, value); }
        }

        public static readonly BindableProperty TextTopColorProperty = BindableProperty.Create("TextTopColor", typeof(Color), typeof(RoundedInfoView), Color.Black);
        /// <summary>
        /// Gets/Sets Top text color of the rounded view
        /// </summary>
        public Color TextTopColor
        {
            get { return (Color)GetValue(TextTopColorProperty); }
            set { SetValue(TextTopColorProperty, value); }
        }

        public static readonly BindableProperty TextBottomColorProperty = BindableProperty.Create("TextBottomColor", typeof(Color), typeof(RoundedInfoView), Color.Black);
        /// <summary>
        /// Gets/Sets Bottom text color of the rounded view
        /// </summary>
        public Color TextBottomColor
        {
            get { return (Color)GetValue(TextBottomColorProperty); }
            set { SetValue(TextBottomColorProperty, value); }
        }


        public static readonly BindableProperty CircleColorProperty = BindableProperty.Create("CircleColor", typeof(Color), typeof(RoundedInfoView), Color.Gray);
        /// <summary>
        /// Gets/Sets Circle sub-view color for this view
        /// </summary>
        public Color CircleColor
        {
            get { return (Color)GetValue(CircleColorProperty); }
            set { SetValue(CircleColorProperty, value);}
        }

        public static readonly BindableProperty ImageProperty = BindableProperty.Create("Image", typeof(ImageSource), typeof(RoundedInfoView));
        /// <summary>
        /// Gets/Sets Image show to for this view
        /// </summary>
        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }
    }
}
