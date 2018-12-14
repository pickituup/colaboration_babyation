using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BabyationApp.Controls.TextEditors
{
    /// <summary>
    /// Text Input control
    /// </summary>
    public class EntryEx : Entry
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public EntryEx()
        {
        }

        public static readonly BindableProperty IsTimeDurationInputProperty = BindableProperty.Create("IsTimeDurationInput", typeof(bool), typeof(EntryEx), false);
        /// <summary>
        /// Get/Sets whether this input control is for time duration
        /// </summary>
        public bool IsTimeDurationInput
        {
            get { return (bool)GetValue(IsTimeDurationInputProperty); }
            set { SetValue(IsTimeDurationInputProperty, value); }
        }

        public static readonly BindableProperty IsSeparateKbProperty = BindableProperty.Create("IsSeparateKb", typeof(bool), typeof(EntryEx), false);
        /// <summary>
        /// Get/Sets to control the show of a separate keyboard or not when the control is focused
        /// </summary>
        public bool IsSeparateKb
        {
            get { return (bool)GetValue(IsSeparateKbProperty); }
            set { SetValue(IsSeparateKbProperty, value); }
        }

        public static readonly BindableProperty CustomCursorColorProperty = BindableProperty.Create("CustomCursorColor", typeof(Color), typeof(EntryEx), null);
        /// <summary>
        /// Get/Sets to control the show of a separate keyboard or not when the control is focused
        /// </summary>
        public Color CustomCursorColor
        {
            get { return (Color)GetValue(CustomCursorColorProperty); }
            set { SetValue(CustomCursorColorProperty, value); }
        }
    }
}
