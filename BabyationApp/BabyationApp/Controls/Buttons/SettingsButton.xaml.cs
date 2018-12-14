using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BabyationApp.Controls.Views;
using Xamarin.Forms;

namespace BabyationApp.Controls.Buttons
{
    /// <summary>
    /// Specialized settings buttons for the settings/more screens
    /// </summary>
    public partial class SettingsButton : ButtonEx
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public SettingsButton()
        {
            InitializeComponent();

            BackgroundView.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "IsPressed")
                {
                    if (BackgroundView.IsPressed)
                    {
                        Xamarin.Forms.Device.StartTimer(TimeSpan.FromMilliseconds(500), () =>
                        {
                            BackgroundView.IsPressed = false;
                            return false;
                        });
                    }
                }
            };
        }

        /// <summary>
        /// Gets the image control of the settings button
        /// </summary>
        public ImageEx ImagePart { get { return Img; } }

        /// <summary>
        /// Gets the text control of the settings button
        /// </summary>
        public Label LablePart { get { return Lbl; } }
    }
}
