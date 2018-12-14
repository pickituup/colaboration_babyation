using System;
using Xamarin.Forms;

namespace BabyationApp.Controls.ListedSelector
{
    public abstract class ItemSelectableBase : ContentView
    {
        /// <summary>
        ///     ctor().
        /// </summary>
        public ItemSelectableBase()
        {
            ItemSelectCommand = new Command(() => {
                SelectionAction(this);
            });

            TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += OnTapGestureRecognizerTapped;

            GestureRecognizers.Add(tapGestureRecognizer);
        }

        public Command ItemSelectCommand { get; private set; }

        public Action<ItemSelectableBase> SelectionAction { get; set; }

        public bool IsSelectable { get; set; } = false;

        public bool IsOnSelectionVisualChangesEnabled { get; set; } = false;

        public abstract void Selected();

        public abstract void Deselected();

        public virtual void OnTapped() { }

        public virtual void Dispose()
        {
            ItemSelectCommand = null;
            SelectionAction = null;
        }

        private void OnTapGestureRecognizerTapped(object sender, EventArgs e)
        {
            if (IsSelectable)
            {
                OnTapped();
                SelectionAction?.Invoke(this);
            }
        }
    }
}
