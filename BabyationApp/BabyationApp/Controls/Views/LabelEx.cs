using System;
using Xamarin.Forms;


namespace BabyationApp.Controls.Views
{
    /// <summary>
    /// Extented Label
    /// </summary>
    /// <remarks>Its mainly to let user set platform dependent font size in xaml through properties rather than Device attribute</remarks>
	public class LabelEx : Label
	{
        public static readonly BindableProperty LineHeightExProperty =
            BindableProperty.Create("LineHeightEx",
                typeof(float),
                typeof(LabelEx),
                defaultValue: 1F);

        public float LineHeightEx
        {
            get => (float)GetValue(LineHeightExProperty);
            set => SetValue(LineHeightExProperty, value);
        }

        /// <summary>
        /// Constructor
        /// </summary>
		public LabelEx()
		{
			//Device.OnPlatform (iOS: () => label.Font = Font.OfSize ("HelveticaNeue-UltraLight", NamedSize.Large));

			this.PropertyChanged += (object sender, System.ComponentModel.PropertyChangedEventArgs e) => {
				switch (e.PropertyName)
				{
					case "FSiOS": if (Device.RuntimePlatform == Device.iOS) FontSize = FSiOS; break;
					case "FSDroid": if (Device.RuntimePlatform == Device.Android) FontSize = FSDroid; break;
					//case "FSWinPhone": if (Device.RuntimePlatform == Device.WinPhone) FontSize = FSWinPhone; break;
				}
            }; 
		}

		public static readonly BindableProperty FSiOSProperty = BindableProperty.Create("FSiOS", typeof(double), typeof(LabelEx), 10.0);
        /// <summary>
        /// iOS platform font-size for this label
        /// </summary>
		[TypeConverter(typeof(FontSizeConverter))]
		public double FSiOS
		{
			get { return (double)GetValue(FSiOSProperty); }
			set { SetValue(FSiOSProperty, value); }
		}

		public static readonly BindableProperty FSAndroidProperty = BindableProperty.Create("FSDroid", typeof(double), typeof(LabelEx), 10.0);
        /// <summary>
        /// Android platform font-size for this label
        /// </summary>
		[TypeConverter(typeof(FontSizeConverter))]
		public double FSDroid
		{
			get { return (double)GetValue(FSAndroidProperty); }
			set { SetValue(FSAndroidProperty, value); }
		}

		public static readonly BindableProperty FSWinPhoneProperty = BindableProperty.Create("FSWinPhone", typeof(double), typeof(LabelEx), 10.0);
        /// <summary>
        /// WinPhone platform font-size for this label
        /// </summary>
		[TypeConverter(typeof(FontSizeConverter))]
		public double FSWinPhone
		{
			get { return (double)GetValue(FSWinPhoneProperty); }
			set { SetValue(FSWinPhoneProperty, value); }
		}


        public static readonly BindableProperty LetterSpacingProperty = BindableProperty.Create("LetterSpacing", typeof(float), typeof(LabelEx), 0.0f);
        /// <summary>
        /// Gets/Sets letter spacing for the text for this label
        /// </summary>
        public float LetterSpacing
        {
            get { return (float)GetValue(LetterSpacingProperty); }
            set { SetValue(LetterSpacingProperty, value); }
        }

        public static readonly BindableProperty IsUnderlinedProperty = BindableProperty.Create("IsUnderlined", typeof(bool), typeof(LabelEx), false);
        /// <summary>
        /// Gets/Sets whether the text for this label should be underlined
        /// </summary>
        public bool IsUnderlined
        {
            get { return (bool)GetValue(IsUnderlinedProperty); }
            set { SetValue(IsUnderlinedProperty, value); }
        }

        public static readonly BindableProperty NumberOfLinesProperty = BindableProperty.Create("NumberOfLines", typeof(int), typeof(LabelEx), 1);
        /// <summary>
        /// Gets/Sets number of lines of text to show
        /// </summary>
        public int NumberOfLines
        {
            get { return (int)GetValue(NumberOfLinesProperty); }
            set { SetValue(NumberOfLinesProperty, value); }
        }
    }
}
