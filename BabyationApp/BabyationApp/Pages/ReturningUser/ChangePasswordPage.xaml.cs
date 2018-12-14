using System;
using BabyationApp.Common;
using BabyationApp.Controls.TextEditors;
using BabyationApp.Controls.Views;
using BabyationApp.Helpers;
using BabyationApp.Managers;
using BabyationApp.Models;
using BabyationApp.Pages.FirstTimeUser;
using BabyationApp.Pages.Settings;
using BabyationApp.Resources;
using Xamarin.Forms;

namespace BabyationApp.Pages.ReturningUser
{
    /// <summary>
    /// This class represents the Change Password page from the design
    /// </summary>
    public partial class ChangePasswordPage : PageBase
    {
        ChangePasswordModel _model;
        PasswordConditionsModel _passModel;

        /// <summary>
        /// Constructor -- Initialize the model and binds buttons events and other ui actions
        /// </summary>
        public ChangePasswordPage()
        {
            InitializeComponent();

            Titlebar.IsVisible = true;
            LeftPageType = typeof(ProfilePage);
            Titlebar.LeftButton.IsVisible = true;
            Titlebar.LeftButton.SetDynamicResource(StyleProperty, "CancelButton");

            BindingContext = _model = new ChangePasswordModel();

            GridPassConditions.BindingContext = _passModel = new PasswordConditionsModel();

            EntryPasswordCurrent.Focused += PasswordEntry_Focused;
            EntryPasswordCurrent.Unfocused += PasswordEntry_Unfocused;
            EntryPasswordCurrent.TextChanged += (s, e) => ValidatePassword(EntryPasswordCurrent, IconPasswordCheckedCurrent);

            EntryPasswordNew.Focused += PasswordEntry_Focused;
            EntryPasswordNew.Unfocused += PasswordEntry_Unfocused;
            EntryPasswordNew.TextChanged += (s, e) => ValidatePassword(EntryPasswordNew, IconPasswordCheckedNew);

            BtnSave.Clicked += async (sender, args) => 
            {
                if (!_model.IsPasswordInvalid)
                {
                    BusyIndicator.IsRunning = true;

                    if (await LoginManager.Instance.ChangePassword(EntryPasswordCurrent.Text, EntryPasswordNew.Text))
                    {
                        PageManager.Me.SetCurrentPage(typeof(SignUpPage));
                    }
                    else
                    {
                        ModalAlertPage.ShowAlertWithClose(AppResource.CurrentPasswordIncorrect);
                        _model.IsPasswordInvalid = true;
                    }

                    BusyIndicator.IsRunning = false;
                }
            };
        }

        void PasswordEntry_Focused(object sender, FocusEventArgs e)
        {
            _passModel.Validate(EntryPasswordCurrent.Text);
            GridPassConditions.IsVisible = e.IsFocused && !_passModel.AllCriteriaMet;
        }

        void PasswordEntry_Unfocused(object sender, FocusEventArgs e) => GridPassConditions.IsVisible = e.IsFocused;

        void ValidatePassword(EntryEx targetEntry, ImageEx targetImage)
        {
            var pass = targetEntry.Text;

            if (string.IsNullOrEmpty(pass))
            {
                _passModel.Reset();
            }
            else
            {
                _passModel.Validate(pass);
            }

            targetImage.IsVisible = _passModel.AllCriteriaMet;

            if (EntryPasswordNew.IsFocused)
            {
                GridPassConditions.IsVisible = !_passModel.AllCriteriaMet;
            }

            if (InputValidator.IsValidPassword(EntryPasswordCurrent.Text) && InputValidator.IsValidPassword(EntryPasswordNew.Text))
            {
                _model.NotReadyToSave = false;
            }
        }

        /// <summary>
        /// Gets called when this page is about to show and performs the initialization
        /// </summary>
        public override void AboutToShow()
        {
            base.AboutToShow();
            EntryPasswordCurrent.Text = "";
            EntryPasswordNew.Text = "";
            (BindingContext as ChangePasswordModel).IsPasswordInvalid = false;
            (BindingContext as ChangePasswordModel).IsPasswordChanged = false;
        }
    }

    /// <summary>
    /// This class is the UI model for the ChangePasswordPage
    /// </summary>
    class ChangePasswordModel : ModelItemBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ChangePasswordModel()
        {
            _isPasswordChanged = false;
            _isPasswordInvalid = false;
        }

        bool _notReadyToSave = true;
        public bool NotReadyToSave
        {
            get { return _notReadyToSave; }
            set { SetPropertyChanged(ref _notReadyToSave, value); }
        }

        /// <summary>
        /// Gets/Sets whether the password is changed or not
        /// </summary>
        bool _isPasswordChanged;
        public bool IsPasswordChanged
        {
            get { return _isPasswordChanged; }
            set => SetPropertyChanged(ref _isPasswordChanged, value);
        }

        /// <summary>
        /// Gets/Sets whether the given password is invalid or not
        /// </summary>
        bool _isPasswordInvalid;
        public bool IsPasswordInvalid
        {
            get { return _isPasswordInvalid; }
            set => SetPropertyChanged(ref _isPasswordInvalid, value);
        }
    }
}
