using System;
using System.Collections.Generic;

using Xamarin.Forms;
using BabyationApp.Common;
using System.Windows.Input;

namespace BabyationApp.Controls.Views
{
    public partial class PhotoBoxControl : ContentView
    {
        static double MinWidth = 74.0;
        static double MinHeight = 74.0;

        public static readonly BindableProperty BoxWidthProperty = BindableProperty.Create(nameof(BoxWidth), typeof(double), typeof(PhotoBoxControl), MinWidth);
        public double BoxWidth
        {
            get => (double)GetValue(BoxWidthProperty);
            set
            {
                if (value >= MinWidth)
                {
                    SetValue(BoxWidthProperty, value);
                }
            }
        }

        public static readonly BindableProperty BoxHeightProperty = BindableProperty.Create(nameof(BoxHeight), typeof(double), typeof(PhotoBoxControl), MinHeight);
        public double BoxHeight
        {
            get => (double)GetValue(BoxHeightProperty);
            set
            {
                if (value >= MinHeight)
                {
                    SetValue(BoxHeightProperty, value);
                }
            }
        }

        public static readonly BindableProperty SourceProperty = BindableProperty.Create(nameof(Source), typeof(ImageSource), typeof(PhotoBoxControl), null);
        public ImageSource Source
        {
            get => (ImageSource)GetValue(SourceProperty);
            set
            {
                SetValue(SourceProperty, value);
            }
        }

        public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(PhotoBoxControl), null);
        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public PhotoBoxControl()
        {
            InitializeComponent();
        }
    }

    public class PhotoBoxModel : ObservableObject
    {
        private ImageSource _source;
        public ImageSource Source 
        { 
            get => _source;
            set => SetPropertyChanged(ref _source, value);
        }
        public ICommand Command { get; set; }
    }
}
