using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace BabyationApp.Controls.Views
{
    public partial class RoundedBoxViewEx : ContentView
    {
        public RoundedBoxViewEx()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty RoundedViewColorProperty = BindableProperty.Create("RoundedViewColor", typeof(Color), typeof(RoundedBoxViewEx), Color.Gray);
        public Color RoundedViewColor
        {
            get { return (Color)GetValue(RoundedViewColorProperty); }
            set { SetValue(RoundedViewColorProperty, value); }
        }
    }
}
