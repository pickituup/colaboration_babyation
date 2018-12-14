using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using BabyationApp.Helpers;
using BabyationApp.Managers;
using BabyationApp.Models;
using Xamarin.Forms;
using BabyationApp.Controls.Buttons;
using BabyationApp.Resources;

namespace BabyationApp.Pages.FirstTimeUser
{
    /// <summary>
    /// This class represents the Local Signup (through email/pass) page from the design
    /// </summary>
    public partial class SignUpViaEmailPage : PageBase
    {
        PasswordConditionsModel _passModel = new PasswordConditionsModel();

        /// <summary>
        /// Constructor -- Initialize the model and binds buttons events and other ui actions
        /// </summary>
        public SignUpViaEmailPage()
        {
            InitializeComponent();

            Title = AppResource.CreateAccount.ToUpper();

            Resources = Application.Current.Resources;

            var model = new SignupViaEmailModel();

            BindingContext = model;

            var checkInputStatus = new Action(() =>
            {
                model.ReadyToSignup = IconEmailChecked.IsVisible && IconPasswordChecked.IsVisible;
            });

            EntryEmail.TextChanged += (s, e) =>
            {
                IconEmailChecked.IsVisible = InputValidator.IsValidEmail(EntryEmail.Text.Trim());
                checkInputStatus();
            };

            _passModel.Reset();

            GridPassConditions.BindingContext = _passModel;

            EntryPassword.Focused += (sender, args) =>
            {
                GridPassConditions.IsVisible = args.IsFocused && !_passModel.AllCriteriaMet;
            };

            EntryPassword.Unfocused += (sender, args) =>
            {
                GridPassConditions.IsVisible = args.IsFocused;
            };

            EntryPassword.TextChanged += (s, e) =>
            {
                String pass = EntryPassword.Text;

                if (String.IsNullOrEmpty(pass))
                {
                    _passModel.Reset();
                }
                else
                {
                    _passModel.FlagChars = pass.ContainsChar(2);
                    _passModel.FlagDigits = pass.ContainsDigit(2);
                    _passModel.FlagSymbols = pass.ContainsSymbol();
                    _passModel.FlagLength = pass.Length >= 8;
                }

                IconPasswordChecked.IsVisible = _passModel.AllCriteriaMet;

                if (EntryPassword.IsFocused)
                {
                    GridPassConditions.IsVisible = !_passModel.AllCriteriaMet;
                }

                checkInputStatus();
            };            
           

            BtnSignup.Clicked += async (s, e) =>
            {
                if (model.ReadyToSignup)
                {
                    BusyIndicator.IsRunning = true;

                    bool signupSuccess = await LoginManager.Instance.SignUpPage(EntryEmail.Text, EntryPassword.Text);

                    if ( signupSuccess )
                    {
                        await LoginManager.Instance.Authenticator.Authenticate( Provider.Custom, EntryEmail.Text, EntryPassword.Text);

                        if (!string.IsNullOrEmpty(LoginManager.Instance.UserId))
                        {
                            DataManager.Instance.SetNewUser(LoginManager.Instance.UserId);

                            PageManager.Me.SetCurrentPage(typeof(TermsAndConditionsPage));
                        }
                        else
                        {
                            ModalAlertPage.ShowAlertWithClose("Sign up success but authentication failed.");
                        }
                    }
                    else
                    {
                        ModalAlertPage.ShowAlertWithClose("User already exists.");
                    }

                    BusyIndicator.IsRunning = false;
                }
            };
            
            Titlebar.IsVisible = true;
            Titlebar.LeftButton.IsVisible = true;
            Titlebar.LeftButton.SetDynamicResource(StyleProperty, "CancelButton");

            LeftPageType = typeof(SignUpPage);
        }

        /// <summary>
        /// Gets called when this page is about to show and performs the initialization
        /// </summary>
        public override void AboutToShow()
        {
            base.AboutToShow();

            _passModel.Reset();

            GridPassConditions.IsVisible = false;
            IconEmailChecked.IsVisible = !String.IsNullOrEmpty(EntryEmail.Text) && Helpers.InputValidator.IsValidEmail(EntryEmail.Text.Trim());
            EntryPassword.Text = "";

            (BindingContext as SignupViaEmailModel).Reset();
        }
    }

    /// <summary>
    /// UI Model class for the SignUpViaEmailPage page
    /// </summary>
    class SignupViaEmailModel : ModelItemBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public SignupViaEmailModel ()
        {
            Reset();
            EULA = "THE SOFTWARE IS PROVIDED ON AN \"AS IS\" BASIS, AND NO WARRANTY, EITHER EXPRESS OR IMPLIED, IS GIVEN. " +
                "YOUR USE OF THE SOFTWARE IS AT YOUR SOLE RISK. GitHub does not warrant that (i) the Software will meet your " +
                "specific requirements; (ii) the Software is fully compatible with any particular platform; (iii) your use of " +
                "the Software will be uninterrupted, timely, secure, or error-free; (iv) the results that may be obtained from " +
                "the use of the Software will be accurate or reliable; (v) the quality of any products, services, information, or " +
                "other material purchased or obtained by you through the Software will meet your expectations; or (vi) any errors in " +
                "the Software will be corrected.\r\n\r\nYOU EXPRESSLY UNDERSTAND AND AGREE THAT GITHUB SHALL NOT BE LIABLE FOR ANY DIRECT, " +
                "INDIRECT, INCIDENTAL, SPECIAL, CONSEQUENTIAL OR EXEMPLARY DAMAGES, INCLUDING BUT NOT LIMITED TO, DAMAGES FOR LOSS OF PROFITS, " +
                "GOODWILL, USE, DATA OR OTHER INTANGIBLE LOSSES (EVEN IF GITHUB HAS BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGES) RELATED " +
                "TO THE SOFTWARE, including, for example: (i) the use or the inability to use the Software; (ii) the cost of procurement of " +
                "substitute goods and services resulting from any goods, data, information or services purchased or obtained or messages " +
                "received or transactions entered into through or from the Software; (iii) unauthorized access to or alteration of your transmissions " +
                "or data; (iv) statements or conduct of any third-party on the Software; (v) or any other matter relating to the Software.\r\n\r\nGitHub " +
                "reserves the right at any time and from time to time to modify or discontinue, temporarily or permanently, the Software " +
                "(or any part thereof) with or without notice. GitHub shall not be liable to you or to any third-party for any modification, " +
                "price change, suspension or discontinuance of the Software.";
        }

        /// <summary>
        /// Resets the model
        /// </summary>
        public void Reset()
        {
            ReadyToSignup = false;
            ShowTerms = false;
        }

        private bool _readyToSignup;

        /// <summary>
        /// Gets/Sets whether user input is done and ready to perform signup
        /// </summary>
        public bool ReadyToSignup
        {
            get
            {
                return _readyToSignup;
            }

            set => SetPropertyChanged(ref _readyToSignup, value);
        }

        private bool _showTerms;
        /// <summary>
        /// Gets/Sets to show/hide terms and conditions
        /// </summary>
        public bool ShowTerms
        {
            get
            {
                return _showTerms;
            }

            set => SetPropertyChanged(ref _showTerms, value);
        }

        private string _eula;
        /// <summary>
        /// Gets/Sets to show/hide EULA
        /// </summary>
        public String EULA
        {
            get
            {
                return _eula;
            }

            set => SetPropertyChanged(ref _eula, value);
        }
    }

    /// <summary>
    /// Password Conditions validation model for the password input for this page
    /// </summary>
    class PasswordConditionsModel : ModelItemBase
    {
        /// <summary>
        /// Resets this model to its initial state
        /// </summary>
        public void Reset()
        {
            FlagChars = false;
            FlagDigits = false;
            FlagSymbols = false;
            FlagLength = false;
        }

        private bool _flagChars;
        /// <summary>
        /// Gets/Sets whetehr the character input is valid
        /// </summary>
        public bool FlagChars
        {
            get { return _flagChars; }
            set => SetPropertyChanged(ref _flagChars, value);
        }

        private bool _flagDigits;
        /// <summary>
        /// Gets/Sets whetehr the digits input is valid
        /// </summary>
        public bool FlagDigits
        {
            get { return _flagDigits; }
            set => SetPropertyChanged(ref _flagDigits, value);
        }

        private bool _flagSymbols;
        /// <summary>
        /// Gets/Sets whetehr the symbols input is valid
        /// </summary>
        public bool FlagSymbols
        {
            get { return _flagSymbols; }
            set => SetPropertyChanged(ref _flagSymbols, value);
        }

        private bool _flagLength;
        /// <summary>
        /// Gets/Sets whetehr the password length is valid
        /// </summary>
        public bool FlagLength
        {
            get { return _flagLength; }
            set => SetPropertyChanged(ref _flagLength, value);
        }

        /// <summary>
        /// Gets whether all password conditiosn are met
        /// </summary>
        public bool AllCriteriaMet
        {
            get { return FlagChars && FlagDigits && FlagSymbols && FlagLength; }
        }

        public void Validate(string pass)
        {
            FlagChars = pass.ContainsChar(2);
            FlagDigits = pass.ContainsDigit(2);
            FlagSymbols = pass.ContainsSymbol();
            FlagLength = pass.Length >= 8;
        }
    }
}
