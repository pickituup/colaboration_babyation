using System;
using System.Collections.Generic;
using System.Windows.Input;
using BabyationApp.Common;
using Xamarin.Forms;

namespace BabyationApp.Controls.Views {
    public partial class AddControlView : ContentView {
        public static readonly BindableProperty ShowOutlinedButtonProperty = BindableProperty.Create(nameof(ShowOutlinedButton), typeof(bool), typeof(AddControlView), false, propertyChanged: OnShowOutlinedButtonPropertyChanged);
        public bool ShowOutlinedButton {
            get => (bool)GetValue(ShowOutlinedButtonProperty);
            set
            {
                SetValue(ShowOutlinedButtonProperty, value);
            }
        }

        static void OnShowOutlinedButtonPropertyChanged(BindableObject bindable, object oldValue, object newValue) {
            var self = bindable as AddControlView;
            if (null != self && null != newValue && true == (bool)newValue)
            {
                self.UpdateButtons((bool)newValue);
            }
        }


        public static readonly BindableProperty ImageProperty = BindableProperty.Create(nameof(Image), typeof(ImageSource), typeof(AddControlView), ImageSource.FromFile("icon_plus_pink"));
        public ImageSource Image {
            get => (ImageSource)GetValue(ImageProperty);
            set
            {
                SetValue(ImageProperty, value);
            }
        }

        public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(AddControlView), String.Empty);
        public string Text {
            get => (string)GetValue(TextProperty);
            set
            {
                SetValue(TextProperty, value);
            }
        }

        public static readonly BindableProperty CommandExProperty = BindableProperty.Create(nameof(CommandEx), typeof(ICommand), typeof(AddControlView), null);
        public ICommand CommandEx {
            get => (ICommand)GetValue(CommandExProperty);
            set => SetValue(CommandExProperty, value);
        }

        public AddControlView() {
            InitializeComponent();
        }

        protected void UpdateButtons(bool showReversed) {
            //NOP
        }
    }
}
