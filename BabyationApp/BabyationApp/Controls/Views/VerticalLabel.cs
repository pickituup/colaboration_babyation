using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

using Xamarin.Forms;

namespace BabyationApp.Controls.Views
{
    public class VerticalLabel : ContentView
    {
        public VerticalLabel()
        {
            this.Content = new StackLayout()
            {
                Spacing = 0,
                Padding = new Thickness(0),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.End
            };
            VText = "EXT_V_FAIL";            
        }


        public static readonly BindableProperty VTextProperty = BindableProperty.Create("VText", typeof(String), typeof(VerticalLabel), "");
        public String VText
        {
            get { return (String)GetValue(VTextProperty); }
            set
            {                
                //if (value != VText)
                {
                    var sl = this.Content as StackLayout;
                    sl.Children.Clear();
                    foreach (Char c in value)
                    {
                        Label lbl = new Label()
                        {
                            Text = c.ToString(),
                            Rotation = -90,
                            WidthRequest = 12,
                            HeightRequest = 12,
                            VerticalOptions = LayoutOptions.End,
                            FontFamily = "fonts/LarsseitMedium.otf#Larsseit Medium",
                            FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                            TextColor = Color.FromHex("#272822"),
                            HorizontalTextAlignment = TextAlignment.Start,
                            VerticalTextAlignment = TextAlignment.Start
                        };
                        sl.Children.Insert(0, lbl);
                    }            
                }
                SetValue(VTextProperty, value);
            }
        
        }
    }
}
