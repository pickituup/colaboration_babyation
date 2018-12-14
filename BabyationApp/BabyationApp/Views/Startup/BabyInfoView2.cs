using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

using Xamarin.Forms;

namespace BabyationApp.Views.Registration
{
    public class BabyInfoView : ContentView
    {
        public BabyInfoView()
        {
            Grid grid = new Grid
            {
                VerticalOptions = LayoutOptions.Center
            };

            StackLayout logo = new StackLayout
            {
                VerticalOptions = LayoutOptions.Center,
                Padding = new Thickness(24, 0),
                Children =
                {
                    new Label
                    {
                        Text = "LOGO",
                        HorizontalOptions = LayoutOptions.Center
                    }
                }
            };
            grid.Children.Add(logo, 0, 0);

            StackLayout header = new StackLayout
            {
                VerticalOptions = LayoutOptions.Center,
                Padding = new Thickness(24, 0),
                Children =
                {
                    new Label
                    {
                        Text = "Bacon ipsum dolor amet alcatra pork tenderloin strip steak porchetta t-bone. Ball tip tri-tip shank tail ground round jowl pastrami capicola corned beef turkey chicken cupim andouille. Swine ham hock hamburger andouille pig, brisket shoulder pork loin t-bone.",
                        HorizontalTextAlignment = TextAlignment.Center
                    }
                }
            };
            grid.Children.Add(header, 0, 1);

            StackLayout babyForm = new StackLayout
            {
                VerticalOptions = LayoutOptions.Center,
                Padding = new Thickness(24, 0),
                Children =
                {
                    new Entry
                    {
                        Placeholder = "Baby Name",
                        HorizontalTextAlignment = TextAlignment.Center
                    },
                    new Button
                    {
                        Text = "Upload Picture"
                    }
                }
            };
            grid.Children.Add(babyForm, 0, 2);

            StackLayout options = new StackLayout
            {
                VerticalOptions = LayoutOptions.Center,
                Padding = new Thickness(24, 0),
                Children =
                {
                    new Label
                    {
                        Text = "Next/Skip",
                        HorizontalOptions = LayoutOptions.Center
                    },
                    new Label
                    {
                        Text = "Add another child",
                        HorizontalOptions = LayoutOptions.Center
                    }
                }
            };
            grid.Children.Add(options, 0, 3);

            Content = grid;
        }
    }
}
