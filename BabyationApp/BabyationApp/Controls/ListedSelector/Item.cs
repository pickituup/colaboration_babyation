using Xamarin.Forms;

namespace BabyationApp.Controls.ListedSelector
{
    public class Item : ItemSelectableBase
    {
        private static readonly Color DEFAULT_SELECTED_COLOR = Color.LightGray;

        private static readonly Color DEFAULT_DESELECTED_COLOR = Color.Transparent;

        public Color SelectedColor { get; set; } = DEFAULT_SELECTED_COLOR;

        public Color DeselectedColor { get; set; } = DEFAULT_DESELECTED_COLOR;

        public override void Deselected()
        {
            if (IsOnSelectionVisualChangesEnabled)
            {
                BackgroundColor = DeselectedColor;
            }
        }

        public override void Selected()
        {
            if (IsOnSelectionVisualChangesEnabled)
            {
                BackgroundColor = SelectedColor;
            }
        }
    }
}
