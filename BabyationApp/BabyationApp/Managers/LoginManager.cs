using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using System;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using Plugin.Connectivity;
using BabyationApp.Pages.FirstTimeUser;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using BabyationApp.Helpers;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;
using System.Collections;
using BabyationApp.Pages.Modes;

namespace BabyationApp.Managers
{

    public enum Provider
    {
        None = -1,
        Custom,
        Google,
        Facebook
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }


    public interface IAuthenticate
    {
        Task<bool> Authenticate(Provider provider, string username = "", string password = "");
    }

    public class LoginManager
    {
        public IAuthenticate Authenticator { get; private set; }

        public void Init(IAuthenticate authenticator)
        {
            Authenticator = authenticator;
        }

        private MobileServiceClient client;

        public MobileServiceClient CurrentClient
        {
            get { return client; }
        }

        private Provider provider;

        static private LoginManager _instance = null;
        private const string _aPIbaseURL = @"https://babyation.azurewebsites.net/";
        //private const string _aPIbaseURL = @"https://192.168.1.12:4443/";
        private const string _accountRoute = @"api/Account/";
        private const string _logout = @"Logout/";
        private const string _manageInfo = @"ManageInfo?returnUrl=%2F&generateState=true";
        private const string _changePassword = @"ChangePassword/";
        private const string _resetPassword = @"ResetPassword/";
        private const string _register = @"Register/";

        private Uri _signUp = new Uri($"{_aPIbaseURL}{_accountRoute}{_register}");
        private Uri _userInfoURI = new Uri($"{_aPIbaseURL}{_accountRoute}{_manageInfo}");
        private Uri _signOut = new Uri($"{_aPIbaseURL}{_accountRoute}{_logout}");
        private Uri _change = new Uri($"{_aPIbaseURL}{_accountRoute}{_changePassword}");
        private Uri _forgot = new Uri($"{_aPIbaseURL}{_accountRoute}{_resetPassword}");


        /// <summary>
        /// Makes request to change password for the current user
        /// </summary>
        /// <param name="currentPassword">Current Password for user</param>
        /// <param name="newPassword">Desired New Password</param>
        /// <returns>bool indicating if password change occured</returns>

        public async Task<bool> ChangePassword(string currentPassword, string newPassword)
        {
            bool changed = false;
            try
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    //HttpClient client = new HttpClient();
                    var body = new ChangePasswordBindingModel()
                    {
                        UserName = UserEmail,
                        OldPassword = currentPassword,
                        NewPassword = newPassword,
                        ConfirmPassword = newPassword
                    };


                    string jsonString = JsonConvert.SerializeObject(body);
                    var registerresult = await CurrentClient.InvokeApiAsync("Account/ChangePassword", JToken.FromObject(body));
                    changed = true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("LoginManager ChangePassword Exception " + ex.Message);
            }

            return changed;
        }

        /// <summary>
        /// Sends Sign Up request to mobile api
        /// </summary>
        /// <param name="userName">New User Name</param>
        /// <param name="password">New User Password</param>
        /// <returns>bool where a successful sign up returns true</returns>
        public async Task<bool> SignUpPage(string userName, string password)
        {
            bool success = false;
            try
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    var body = new RegisterBindingModel() 
                    {
                        Email = userName,
                        Password = password,
                        ConfirmPassword = password
                    };
                    string jsonString = JsonConvert.SerializeObject(body);
                    var registerresult = await CurrentClient.InvokeApiAsync("Account/Register", JToken.FromObject( body));
                    success = true; 
                } 
            }
            catch (Exception ex)
            {
                Debug.WriteLine("LoginManager SignUpIn Exception " + ex.Message);
            }
            return success;
        }



        /// <summary>
        /// Sends Forgot Password Request to mobile api
        /// </summary>
        /// <param name="email">Email address of users forgotten password</param>
        /// <returns>boolean true if password reset email was sent</returns>
        public async Task<bool> ForgotPassword(string email)
        {
            bool sent = false;
            try
            {
                if(CrossConnectivity.Current.IsConnected)
                {
                    HttpClient client = new HttpClient();
                    var body = new
                    {
                        Email = email
                    };
                    string jsonString = JsonConvert.SerializeObject(body);
                    StringContent content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json");
                    try
                    {
                        HttpResponseMessage result = await client.PostAsync(_forgot, content);
                        if (result.IsSuccessStatusCode)
                        {
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        string debugBreak = ex.ToString();
                    }

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("LoginManager On Forgot Password Exception " + ex.Message);
            }
            return sent;
        }

        /// <summary>
        /// Gets a login manager instance
        /// </summary>
        static public LoginManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LoginManager();
                }

                return _instance;
            }
        }

        /// <summary>
        /// Makes constructor private so that only one login manager instance will exist
        /// </summary>
        private LoginManager():base()
        {
            client = new MobileServiceClient(new Uri(_aPIbaseURL));
//            client.AlternateLoginHost = new Uri("https://babyation.azurewebsites.net/");
            if(client.CurrentUser == null && !string.IsNullOrEmpty(Token) && !string.IsNullOrEmpty(UserId))
            {
                string userid = string.Empty;
                if(Provider == Provider.Facebook || Provider == Provider.Google)
                {
                    userid = "sid:";
                }
                userid += UserId;
                client.CurrentUser = new MobileServiceUser(userid);
                client.CurrentUser.MobileServiceAuthenticationToken = Token;
            }
        }

        public async Task<bool> Authenticated()
        {
            if(await GetUserInfo())
            {
                return true;
            }
            SignOut();
            return false;
        }

        /// <summary>
        /// Clears Current User Information Removing Access to the mobile API
        /// </summary>
        public void SignOut()
        {
            UserId = string.Empty;
            UserEmail = string.Empty;
            Token = string.Empty;
            Provider = Provider.None;
            Name = string.Empty;
            DataManager.Instance.SignOut();
            if (CrossConnectivity.Current.IsConnected)
            {
                CurrentClient.LogoutAsync();
            }

        }

        public Provider Provider
        {
            get
            {
                return (Provider)Settings.Provider;
            }
            set
            {
                if(value != (Provider) Settings.Provider)
                {
                    Settings.Provider = (int)value;
                }
            }
        }


        /// <summary>
        /// Current authorized user id or empty string
        /// </summary>
        public string UserId
        {
            get
            {
                return Settings.UserId;
            }
            set
            {
                if (Settings.UserId != value)
                {
                    Settings.UserId = value;
                }
            }

        }

        public string Token
        {
            get
            {
                return Settings.Token;
            }
            set
            {
                if (Settings.Token != value)
                {
                    Settings.Token = value;
                }
            }

        }


        //Current Authorized User Email Address or empty string
        public string UserEmail
        {
            get
            {
                return Settings.UserEmail;
            }
            set
            {
                if (Settings.UserEmail != value)
                {
                    Settings.UserEmail = value;
                }
            }
        }

        public string Name
        {
            get
            {
                return Settings.Name;
            }
            set
            {
                if (Settings.Name != value)
                {
                    Settings.Name = value;
                }
            }

        }

        /// <summary>
        /// Private task to get user information from mobile api
        /// </summary>
        /// <returns>boolean true if current authorization is valid when online or if offline and user info is cached</returns>
        public async Task<bool> GetUserInfo()
        {
            bool success = false;
            if (CrossConnectivity.Current.IsConnected) {
                try
                {
                    switch(Provider)
                    {
                        case Provider.Custom:
                            var customresult = await CurrentClient.InvokeApiAsync("Account/UserInfo", HttpMethod.Get, null);
                            UserEmail = customresult.SelectToken("Email").Value<string>();
                            Name = string.Empty;
                            success = true;
                            break;
                        case Provider.Google:
                        case Provider.Facebook:
                            var extResult = await CurrentClient.InvokeApiAsync("/.auth/me", HttpMethod.Get, null);
                            UserEmail = extResult[0].SelectToken("user_id").Value<string>();
                            var claims = extResult[0].SelectToken("user_claims");
                            foreach(var claim in claims)
                            {
                                string typ = claim.SelectToken("typ").Value<string>();
                                if (typ == "name" || typ == @"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")
                                {
                                    Name = claim.SelectToken("val").Value<string>();
                                    break;
                                }
                            }
                            //foreach( var claim in claims)
                            //{
                            //    if(claim.s)

 //                           }
                            success = true;
                            break;
                        default:
                            break;

                    }
                } catch (Exception ex)
                {
                    SignOut();
                }
            } else if (!string.IsNullOrEmpty(Token))
            {
                success = true;
            }

            return success;
        }


    }

    public class RegisterBindingModel
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }
    }

    public class ChangePasswordBindingModel
    {
        public string UserName { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }

}
