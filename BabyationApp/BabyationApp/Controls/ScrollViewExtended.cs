using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace BabyationApp.Controls
{
    public class ScrollViewExtended : ScrollView
    {

        public static readonly BindableProperty IsScrollbarFadingProperty = BindableProperty.Create(
            nameof(IsScrollbarFading),
            typeof(bool),
            typeof(ScrollViewExtended),
            defaultValue: default(bool));

        public ScrollViewExtended() { }

        public bool IsScrollbarFading
        {
            get => (bool)GetValue(IsScrollbarFadingProperty);
            set => SetValue(IsScrollbarFadingProperty, value);
        }
    }
}
