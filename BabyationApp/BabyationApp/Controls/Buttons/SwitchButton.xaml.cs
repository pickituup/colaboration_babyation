using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using BabyationApp.Resources;

namespace BabyationApp.Controls.Buttons
{
    /// <summary>
    ///  ON/OFF, CHECK/UNCHECK button
    /// </summary>
    public partial class SwitchButton : ButtonEx
    {
        private Style CurrentStyle;
        private static Style TextOnStyleValue
        {
            get
            {
                object styleValue = null;
                return Application.Current?.Resources.TryGetValue("SwitchOnStyle", out styleValue) ?? false ? (Style)styleValue : null;
            }
        }
        private static Style TextOffStyleValue
        {
            get
            {
                object styleValue = null;
                return Application.Current?.Resources.TryGetValue("SwitchOffStyle", out styleValue) ?? false ? (Style)styleValue : null;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public SwitchButton()
        {
            InitializeComponent();

            this.SizeChanged += (sender, args) =>
            {
                BtnHandle.WidthRequest = Math.Min(this.Height, this.Width) - 3;
                BtnHandle.HeightRequest = Math.Min(this.Height, this.Width) - 3;
            };

            this.BackgroundView.IsPressedChanged += (sender, e) => 
            {
                if (this.IsPressed != BtnHandle.IsPressed)
                {
                    SyncUIState();
                    BtnHandle.IsToggled = this.IsPressed;
                }
            };

            this.BackgroundView.Toggled += (sender, args) =>
            {
                if (this.IsToggled != BtnHandle.IsToggled)
                {
                    SyncUIState();
                    BtnHandle.IsToggled = this.IsToggled;
                }
            };

            BtnHandle.Toggled += (sender, args) =>
            {
                if (this.IsToggled != BtnHandle.IsToggled)
                {
                    this.IsToggled = BtnHandle.IsToggled;
                    SyncUIState();
                    this.FireToggled(EventArgs.Empty);
                }
            };

            SyncUIState();
        }

        /// <summary>
        /// Sync the UI state to properties
        /// </summary>
        private void SyncUIState()
        {
            BtnHandle.HorizontalOptions = IsToggled ? LayoutOptions.End : LayoutOptions.Start;
            CurrentStyle = IsToggled ? TextOnStyle : TextOffStyle;
            Lbl.Text = IsToggled ? TextOn : TextOff;
            Lbl.Style = CurrentStyle;
        }

        public static readonly BindableProperty HandleColorNormalProperty = BindableProperty.Create("HandleColorNormal", typeof(Color), typeof(SwitchButton), Color.FromHex("#093954")); //Navy
        /// <summary>
        /// Switch hanlde's normal state color
        /// </summary>
        public Color HandleColorNormal
        {
            get { return (Color)GetValue(HandleColorNormalProperty); }
            set { SetValue(HandleColorNormalProperty, value); }
        }

        public static readonly BindableProperty HandleColorPressedProperty = BindableProperty.Create("HandleColorPressed", typeof(Color), typeof(SwitchButton), Color.FromHex("#EE4041")); //Red
        /// <summary>
        /// Gets/Sets Switch hanlde's pressed state color
        /// </summary>
        public Color HandleColorPressed
        {
            get { return (Color)GetValue(HandleColorPressedProperty); }
            set { SetValue(HandleColorPressedProperty, value); }
        }

        public static readonly BindableProperty TextOffProperty = BindableProperty.Create("TextOff", typeof(string), typeof(SwitchButton), AppResource.OffUpper);
        /// <summary>
        /// Gets/Sets text to show when switch is off
        /// </summary>
        public string TextOff
        {
            get { return (string)GetValue(TextOffProperty); }
            set { SetValue(TextOffProperty, value); }
        }

        public static readonly BindableProperty TextOnProperty = BindableProperty.Create("TextOn", typeof(string), typeof(SwitchButton), AppResource.OnUpper);
        /// <summary>
        /// Gets/Sets text to show when switch is on
        /// </summary>
        public string TextOn
        {
            get { return (string)GetValue(TextOnProperty); }
            set { SetValue(TextOnProperty, value); }
        }

        public static readonly BindableProperty TextOnStyleProperty = BindableProperty.Create("TextOnStyle", typeof(Style), typeof(SwitchButton), TextOnStyleValue);
        /// <summary>
        /// Gets/Sets color for text to show when switch is on
        /// </summary>
        public Style TextOnStyle
        {
            get { return (Style)GetValue(TextOnStyleProperty); }
            set { SetValue(TextOnStyleProperty, value); }
        }

        public static readonly BindableProperty TextOffStyleProperty = BindableProperty.Create("TextOffStyle", typeof(Style), typeof(SwitchButton), TextOffStyleValue);
        /// <summary>
        /// Gets/Sets color for text to show when switch is on
        /// </summary>
        public Style TextOffStyle
        {
            get { return (Style)GetValue(TextOffStyleProperty); }
            set { SetValue(TextOffStyleProperty, value); }
        }
    }
}
