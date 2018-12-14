using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace BabyationApp.Controls.Views
{
    public partial class SaveView : Grid
    {
        public event EventHandler Tapped;

        public static readonly BindableProperty TappedCommandProperty =
            BindableProperty.Create(nameof(TappedCommand), typeof(ICommand), typeof(SaveView));

        public ICommand TappedCommand
        {
            get { return (ICommand)GetValue(TappedCommandProperty); }
            set { SetValue(TappedCommandProperty, value); }
        }

        public static readonly BindableProperty TextProperty
            = BindableProperty.Create(nameof(Text),
                                      typeof(string),
                                      typeof(SaveView),
                                      null,
                                      BindingMode.OneWay,
                                      propertyChanged: HandleTextPropertyChanged);

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        static void HandleTextPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is SaveView saveView)
            {
                saveView.SaveTextLabel.Text = (string)newValue;
            }
        }

        public SaveView()
        {
            InitializeComponent();
        }

        // Command put in here for manual checking of CanExecute
        void Handle_Tapped(object sender, EventArgs e)
        {
            if (TappedCommand != null && TappedCommand.CanExecute(e))
            {
                TappedCommand.Execute(null);
            }

            Tapped?.Invoke(sender, e);
        }
    }
}
