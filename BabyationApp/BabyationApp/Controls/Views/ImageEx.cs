using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFImageLoading.Forms;
using Xamarin.Forms;

namespace BabyationApp.Controls.Views
{
    /// <summary>
    /// Image View class use to customize through platform dependent renderers
    /// </summary>
    public class ImageEx : View
    {
        /// <summary>
        /// Constructor - initializes the default values
        /// </summary>
        public ImageEx()
        {
            UseImageSize = false;
        }

        /// <summary>
        /// Gets/Sets whether sync UI size to the Image's size
        /// </summary>
        public bool UseImageSize { get; set; }

        public static readonly BindableProperty SourceProperty = BindableProperty.Create("Source",
            typeof(Xamarin.Forms.ImageSource), typeof(ImageEx));

        /// <summary>
        /// Image url to show to this image viewer
        /// </summary>
        public ImageSource Source
        {
            get { return (ImageSource) GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }
    }
}