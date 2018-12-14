using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BabyationApp.Controls.ListedSelector
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChildListedItem : Item
    {
        public ChildListedItem()
        {
            InitializeComponent();

            SelectedColor = Color.Transparent;
            DeselectedColor = Color.Transparent;
        }

        public override void OnTapped()
        {
            base.OnTapped();

            if (IsOnSelectionVisualChangesEnabled)
            {
                _tapFadeEffect_BoxView.Opacity = 1;
                _tapFadeEffect_BoxView.FadeTo(0, 250);
            }
        }

        public override void Selected()
        {
            base.Selected();

            if (IsOnSelectionVisualChangesEnabled)
            {
                _bottomIndicatorCachedImage.TranslationX = 0;
                _todo_BoxView.BackgroundColor = (Color)App.Current.Resources["MedBlue"];
            }
        }

        public override void Deselected()
        {
            base.Deselected();

            if (IsOnSelectionVisualChangesEnabled)
            {
                _bottomIndicatorCachedImage.TranslationX = short.MaxValue;
                _todo_BoxView.BackgroundColor = (Color)App.Current.Resources["Peach"];
            }
        }
    }
}