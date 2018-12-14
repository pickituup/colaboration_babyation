using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BabyationApp.Controls.Views;
using Xamarin.Forms;

namespace BabyationApp.Controls.Buttons
{
    /// <summary>
    /// All of the buttons (circle/rounded, flat) base class
    /// </summary>
    public class ButtonBase : ContentView
    {
        /// <summary>
        /// Button clicked event
        /// </summary>
        public event EventHandler Clicked;

        /// <summary>
        /// Toggle even if the button is radio or checkbox type
        /// </summary>
        public event EventHandler Toggled;

        public ButtonBase()
        {
            BackgroundColor = Color.Transparent;

            this.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "IsToggleOverride")
                {
                    IsToggled = IsToggleOverride;
                    //HandlePressedChanged();
                }
                else if (e.PropertyName == IsEnabledProperty.PropertyName)
                {
					EnabledPropertyChanged();
                }
                //
                // This is hack because only touch events changes variable behind this property. 
                // Changes from xaml like "IsPressed="{Binding IsSelected}" does not changes variable for unknown reason
                //
                else if (e.PropertyName == "IsPressed")
                {
                    IsPressed = !IsPressed;
                }
            };
        }      

		protected virtual void EnabledPropertyChanged()
		{
            Opacity = IsEnabled ? 1.0 : 0.6;
		}

        /// <summary>
        /// Called from the platform dependent renderers & interaction handlers
        /// to fire the Clicked event to the button's user
        /// </summary>
        /// <param name="e"></param>
        protected /*async Task*/ void FireClicked(EventArgs e)
        {
            //await Task.Run(() =>
            //{
            //    Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            //    {
                    Clicked?.Invoke(this, e);
                    Command?.Execute(this);
            //    });
            //});
            
        }

        /// <summary>
        /// Called from the platform dependent renderers & interaction handlers
        /// to fire the toggle event to the button's user
        /// </summary>
        /// <param name="e"></param>
        protected /*async Task*/ void FireToggled(EventArgs e)
        {
            //await Task.Run(() =>
            //{
            //    Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            //    {
                    Toggled?.Invoke(this, e);
                    ToggleCommand?.Execute(this);
            //    });
            //});
        }

        protected virtual void HandlePressedChanged()
        {
        }

        public static readonly BindableProperty BackgroundViewProperty = BindableProperty.Create("BackgroundView", typeof(RoundedBoxView), typeof(ButtonBase), null);

        /// <summary>
        /// Buttons background view
        /// </summary>
        /// <remarks>
        /// We need to style this view to make the button circle/rounded/flat
        /// </remarks>
        public RoundedBoxView BackgroundView
        {
            get { return (RoundedBoxView)GetValue(BackgroundViewProperty); }
            protected set
            {
                SetValue(BackgroundViewProperty, value);
                if (value != null)
                {
                    BackgroundView.Tapped += (s, e) =>
                    {
                            FireClicked(e);
                            
                    };
                    BackgroundView.Toggled += (s, e) =>
                    { 
                            FireToggled(e);
                            
                    };
                    BackgroundView.IsPressedChanged += (s, e) =>
                    {
                        HandlePressedChanged();
                    };

                }
            }
        }


        public static readonly BindableProperty IsInteractableProperty = BindableProperty.Create("IsInteractable", typeof(bool), typeof(ButtonBase), true);

        /// <summary>
        /// Enable/disable the user interaction on this control
        /// </summary>
        public bool IsInteractable
        {
            get { return (bool)GetValue(IsInteractableProperty); }
            set { SetValue(IsInteractableProperty, value); }
        }

        public static readonly BindableProperty IsTogglableProperty = BindableProperty.Create("IsTogglable", typeof(bool), typeof(ButtonBase), false);

        /// <summary>
        /// Gets/Sets whether this button is togglable(radio/checkbox) or not
        /// </summary>
        public bool IsTogglable
        {
            get { return (bool)GetValue(IsTogglableProperty); }
            set { SetValue(IsTogglableProperty, value); }
        }

        public static readonly BindableProperty IsToggledProperty = BindableProperty.Create("IsToggled", typeof(bool), typeof(ButtonBase), false, defaultBindingMode: BindingMode.TwoWay);

        /// <summary>
        /// If IsTogglable is true, this property get/set the button's toggle-state 
        /// </summary>
        public bool IsToggled
        {
            get { return IsPressed; }
            set {
                SetValue(IsToggledProperty, value);
                IsPressed = value;
            }
        }

        public static readonly BindableProperty IsPressedProperty = BindableProperty.Create("IsPressed", typeof(bool), typeof(ButtonBase), false);

        /// <summary>
        /// Gets/Sets whether the button is pressed or not
        /// </summary>
        public bool IsPressed
        {
            get { return BackgroundView != null ? this.BackgroundView.IsPressed : false; }
            set { if (BackgroundView != null) this.BackgroundView.IsPressed = value;  }
        }

        public static readonly BindableProperty CornerRadiusProperty =
            BindableProperty.Create("CornerRadius", typeof(double), typeof(ButtonBase), default(double));

        /// <summary>
        /// Gets/Sets button's corner radius
        /// </summary>
        public double CornerRadius
        {
            get { return (double)base.GetValue(CornerRadiusProperty); }
            set { base.SetValue(CornerRadiusProperty, value); }
        }


        public static readonly BindableProperty RadiusBasedOnSizeProperty = BindableProperty.Create("RadiusBasedOnSize", typeof(bool), typeof(ButtonBase), false);

        /// <summary>
        /// Enable/Disable the decision whether the button's corner radius should be calculated based on button's size or not
        /// </summary>
        public bool RadiusBasedOnSize
        {
            get { return (bool)GetValue(RadiusBasedOnSizeProperty); }
            set { SetValue(RadiusBasedOnSizeProperty, value); }
        }

        public static readonly BindableProperty RadiusSizeRatioProperty =
            BindableProperty.Create("RadiusSizeRatio", typeof(double), typeof(ButtonBase), .5);

        /// <summary>
        /// If RadiusBasedOnSize is true, this ratio is used to calculate the corner radius based on button width/height
        /// </summary>
        public double RadiusSizeRatio
        {
            get { return (double)base.GetValue(RadiusSizeRatioProperty); }
            set { base.SetValue(RadiusSizeRatioProperty, value); }
        }

        public static readonly BindableProperty IsCircleProperty = BindableProperty.Create("IsCircle", typeof(bool), typeof(ButtonBase), false);

        /// <summary>
        /// Gets/Sets whether the button should be a circle or not
        /// </summary>
        public bool IsCircle
        {
            get { return (bool)GetValue(IsCircleProperty); }
            set { SetValue(IsCircleProperty, value); }
        }


        public static readonly BindableProperty CommandProperty = BindableProperty.Create("Command", typeof(ICommand), typeof(ButtonBase), null);
        /// <summary>
        /// Gets/Sets the command to execute on button's click event
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly BindableProperty ToggleCommandProperty = BindableProperty.Create("ToggleCommand", typeof(ICommand), typeof(ButtonBase), null);

        /// <summary>
        /// Gets/Sets the command to execute on button's toggle event
        /// </summary>
        public ICommand ToggleCommand
        {
            get { return (ICommand)GetValue(ToggleCommandProperty); }
            set { SetValue(ToggleCommandProperty, value); }
        }

        /// <summary>
        /// Programatically generates the ClickedEvent for user
        /// </summary>
        public void AnimateClicked()
        {
            if (IsTogglable == false)
            {
                IsPressed = true;
                FireClicked(EventArgs.Empty);
                IsPressed = false;
            }
        }


        public static readonly BindableProperty IsToggleOverrideProperty = BindableProperty.Create("IsToggleOverride", typeof(bool), typeof(ButtonBase), false);
        public bool IsToggleOverride
        {
            get { return (bool)GetValue(IsToggleOverrideProperty); }
            set {
                SetValue(IsToggleOverrideProperty, value);
            }
        }

        public static readonly BindableProperty ContentPaddingProperty = BindableProperty.Create("ContentPadding", typeof(Thickness), typeof(FlatButton), new Thickness(0));
        /// <summary>
        /// Padding to the content of this button
        /// </summary>
        public Thickness ContentPadding
        {
            get { return (Thickness)GetValue(ContentPaddingProperty); }
            set { SetValue(ContentPaddingProperty, value); }
        }

        /// <summary>
        /// User tag to tag sometihng to this button
        /// </summary>
        public object Tag { get; set; }
    }


    /// <summary>
    /// Manages the mutually exclusive button group (radio buttons like state) so that
    /// only of them gets checked in a group
    /// </summary>
    public class ButtonExGroup
    {
        /// <summary>
        /// Delegate to use when a button toggle state change among the buttons group
        /// </summary>
        /// <param name="sender">This group</param>
        /// <param name="item">The button that's toggle state changed</param>
        /// <param name="index">Index of the button in this group</param>
        public delegate void ButtonToggled(ButtonExGroup sender, ButtonBase item, int index);

        /// <summary>
        /// List of the buttons
        /// </summary>
        private List<ButtonBase> _buttons;

        /// <summary>
        /// Toggle event fired by this class once one of the buttons state changes.
        /// </summary>
        public event ButtonToggled Toggled;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <remarks>Initialize the buttons list</remarks>
        public ButtonExGroup()
        {
            _buttons = new List<ButtonBase>();
        }

        /// <summary>
        /// Get the list of the buttons
        /// </summary>
        public List<ButtonBase> Buttons { get { return _buttons; } }


        private ButtonBase _currentButton;

        /// <summary>
        /// Gets/Sets currently checked button
        /// </summary>
        public ButtonBase CurrentButton
        {
            get { return _currentButton; }
            set
            {
                if (_currentButton != value)
                {
                    _currentButton = value;
                    UpdateCurrentButton(value);
                }
            }
        }

        /// <summary>
        /// Gets/Sets the currently checked button index
        /// </summary>
        public int CurrentIndex
        {
            get
            {
                return _currentButton != null ? _buttons.IndexOf(_currentButton) : -1;
            }

            set
            {
                if (value != CurrentIndex)
                {
                    CurrentButton = _buttons[value];
                }
            }
        }

        /// <summary>
        /// Gets or sets the first button.
        /// </summary>
        /// <value>The first button.</value>
        public ButtonBase FirstButton
        {
            get 
            {
                if (0 < Buttons.Count)
                {
                    return Buttons.ElementAt(0);
                }
                return null; 
            }
        }

        /// <summary>
        /// Add a button to the group
        /// </summary>
        /// <param name="btn">The button to add to the group</param>
        public void AddButton(ButtonBase btn)
        {
            if (!_buttons.Contains(btn))
            {
                btn.IsTogglable = true;
                _buttons.Add(btn);

                btn.Toggled += (s, e) =>
                {
                    UpdateCurrentButton(s as ButtonBase);
                };

            }
        }

        public void RemoveAllButtons()
        {
            if( 0 < _buttons?.Count )
            {
                _buttons.Clear();
                _currentButton = null;
            }
        }

        public void Reset()
        {
            if (0 < _buttons?.Count)
            {
                foreach (ButtonBase b in _buttons)
                {
                    b.IsToggled = false;
                }
                _currentButton = null;
            }
        }

        /// <summary>
        /// Change the current active button
        /// </summary>
        /// <param name="btn">The button to make active/checked in the group</param>
        public void UpdateCurrentButton(ButtonBase btn)
        {
            _currentButton = btn;
            if (btn != null)
            {
                btn.IsToggled = true;
            }

            foreach (ButtonBase b in _buttons)
            {
                if (btn != b) b.IsToggled = false;
            }

            if (btn != null)
            {
                Toggled?.Invoke(this, btn, _buttons.IndexOf(btn));
            }
            else
            {
                Toggled?.Invoke(this, null, -1);
            }
        }

    }
}
