using System;
using System.Windows.Input;
using BabyationApp.Pages;
using Xamarin.Forms;

namespace BabyationApp.Controls.Views
{
    public partial class ConfirmationView : ContentView, IDialog
    {
        #region Body

        public static readonly BindableProperty BodyTextProperty = BindableProperty.Create(nameof(BodyText), typeof(string), typeof(ConfirmationView), String.Empty);
        public string BodyText
        {
            get => (string)GetValue(BodyTextProperty);
            set
            {
                SetValue(BodyTextProperty, value);
            }
        }

        public static readonly BindableProperty BodyTextStyleProperty = BindableProperty.Create(nameof(BodyTextStyle), typeof(Style), typeof(ConfirmationView), null);
        public Style BodyTextStyle
        {
            get => (Style)GetValue(BodyTextStyleProperty);
            set
            {
                SetValue(BodyTextStyleProperty, value);
            }
        }

        public static readonly BindableProperty BodyTextColorProperty = BindableProperty.Create(nameof(BodyTextColor), typeof(Color), typeof(ConfirmationView), Color.Black);
        public Color BodyTextColor
        {
            get => (Color)GetValue(BodyTextColorProperty);
            set
            {
                SetValue(BodyTextColorProperty, value);
            }
        }

        #endregion

        #region Destructive 

        public static readonly BindableProperty DestructiveTextProperty = BindableProperty.Create(nameof(DestructiveText), typeof(string), typeof(ConfirmationView), String.Empty);
        public string DestructiveText
        {
            get => (string)GetValue(DestructiveTextProperty);
            set
            {
                SetValue(DestructiveTextProperty, value);
            }
        }

        public static readonly BindableProperty DestructiveImageProperty = BindableProperty.Create(nameof(DestructiveImage), typeof(ImageSource), typeof(ConfirmationView), null);
        public ImageSource DestructiveImage
        {
            get => (ImageSource)GetValue(DestructiveImageProperty);
            set
            {
                SetValue(DestructiveImageProperty, value);
            }
        }

        public static readonly BindableProperty DestructiveImagePressedProperty = BindableProperty.Create(nameof(DestructiveImagePressed), typeof(ImageSource), typeof(ConfirmationView), null);
        public ImageSource DestructiveImagePressed
        {
            get => (ImageSource)GetValue(DestructiveImagePressedProperty);
            set
            {
                SetValue(DestructiveImagePressedProperty, value);
            }
        }

        public static readonly BindableProperty DestructiveCommandProperty = BindableProperty.Create(nameof(DestructiveCommand), typeof(ICommand), typeof(ConfirmationView), default(ICommand));
        public ICommand DestructiveCommand
        {
            get => (ICommand)GetValue(DestructiveCommandProperty);
            set => SetValue(DestructiveCommandProperty, value);
        }

        #endregion

        #region Positive

        public static readonly BindableProperty PositiveTextProperty = BindableProperty.Create(nameof(PositiveText), typeof(string), typeof(ConfirmationView), String.Empty);
        public string PositiveText
        {
            get => (string)GetValue(PositiveTextProperty);
            set
            {
                SetValue(PositiveTextProperty, value);
            }
        }

        public static readonly BindableProperty PositiveImageProperty = BindableProperty.Create(nameof(PositiveImage), typeof(ImageSource), typeof(ConfirmationView), null);
        public ImageSource PositiveImage
        {
            get => (ImageSource)GetValue(PositiveImageProperty);
            set
            {
                SetValue(PositiveImageProperty, value);
            }
        }

        public static readonly BindableProperty PositiveImagePressedProperty = BindableProperty.Create(nameof(PositiveImagePressed), typeof(ImageSource), typeof(ConfirmationView), null);
        public ImageSource PositiveImagePressed
        {
            get => (ImageSource)GetValue(PositiveImagePressedProperty);
            set
            {
                SetValue(PositiveImagePressedProperty, value);
            }
        }

        public static readonly BindableProperty PositiveCommandProperty = BindableProperty.Create(nameof(PositiveCommand), typeof(ICommand), typeof(ConfirmationView), default(ICommand));
        public ICommand PositiveCommand
        {
            get => (ICommand)GetValue(PositiveCommandProperty);
            set => SetValue(PositiveCommandProperty, value);
        }

        #endregion

        public DashboardTabPage RelativeDashboardTabPage { get; set; }

        public ConfirmationView()
        {
            InitializeComponent();

            BtnDestructive.Clicked += BtnDestructive_Clicked;
            BtnPositive.Clicked += BtnPositive_Clicked;
        }

        void BtnDestructive_Clicked(object sender, EventArgs e)
        {
            DestructiveCommand?.Execute(this);

            if (RelativeDashboardTabPage != null)
            {
                RelativeDashboardTabPage.HideDialog();
            }
        }

        void BtnPositive_Clicked(object sender, EventArgs e)
        {
            PositiveCommand.Execute(this);

            if (RelativeDashboardTabPage != null)
            {
                RelativeDashboardTabPage.HideDialog();
            }
        }
    }
}
