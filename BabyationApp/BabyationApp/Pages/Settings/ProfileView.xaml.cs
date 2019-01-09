using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BabyationApp.Controls.Buttons;
using BabyationApp.Controls.Views;
using BabyationApp.Converters;
using BabyationApp.Models;
using Xamarin.Forms;
using BabyationApp.Managers;
using BabyationApp.Pages.FirstTimeUser;
using BabyationApp.Pages.ReturningUser;
using System.Windows.Input;
using BabyationApp.Extensions;
using BabyationApp.Common;
using System.Resources;
using BabyationApp.Resources;
using Xamarin.Forms.Internals;

namespace BabyationApp.Pages.Settings
{
    /// <summary>
    /// This class represents the Profile page from the design
    /// </summary>
    public partial class ProfileView : RootViewBase
    {
        public MyInfoViewModel ContextMyInfo { get; private set; }
        public MyChildrenViewModel ContextMyChildren { get; private set; }
        public CaregiversViewModel ContextMyCaregivers { get; private set; }

        private readonly ButtonExGroup _btnGroupTab = new ButtonExGroup();
        private BabyModel _babyDeleteRequested;
        private Type oldLeftPageType = null;

        #region Bindable properties

        public static readonly BindableProperty CaregiverFlowProperty = BindableProperty.Create(nameof(CaregiverFlow), typeof(bool), typeof(ProfileView), false);
        public bool CaregiverFlow
        {
            get => (bool)GetValue(CaregiverFlowProperty);
            set
            {
                SetValue(CaregiverFlowProperty, value);
            }
        }

        #endregion

        public ProfileView()
        {
            InitializeComponent();

            BindingContext = this;

            if (CaregiverFlow)
            {
                Titlebar.IsVisible = true;
                RootLayout.Style = (Style)Application.Current.Resources["AbsoluteLayout_NavigationOnTop"];
            }
            else
            {
                Titlebar.IsVisible = false;
                this.Padding = new Thickness(0);
            }
            Titlebar.RightButton.IsVisible = false;

            // My Info
            ContextMyInfo = new MyInfoViewModel(ChangePassword, AddAuthCode);
            ContextMyInfo.PropertyChanged += ContextMyInfo_PropertyChanged;
            RLMyInfo.BindingContext = ContextMyInfo;
            RemoveYourselfFromCaregiversContainer.BindingContext = ContextMyInfo;
            ContextMyInfo.Reset();

            // My children
            ContextMyChildren = new MyChildrenViewModel(AddBaby, RemoveBaby);
            RLMyBabies.BindingContext = ContextMyChildren;
            RemoveChildContainer.BindingContext = ContextMyChildren;
            ContextMyChildren.Reset();

            // Caregivers
            ContextMyCaregivers = new CaregiversViewModel(AddCaregiver);
            ContextMyCaregivers.PropertyChanged += ContextMyCaregivers_PropertyChanged;
            RLMyCaregivers.BindingContext = ContextMyCaregivers;
            RemoveCaregiverContainer.BindingContext = ContextMyCaregivers;
            ContextMyCaregivers.Reset();

            _btnGroupTab.AddButton(BtnMyInfo);
            _btnGroupTab.AddButton(BtnMyBabies);
            _btnGroupTab.AddButton(BtnMyPeople);

            _btnGroupTab.Toggled += (sender, item, index) =>
            {
                RLMyInfo.IsVisible = index == 0;
                RLMyBabies.IsVisible = index == 1;
                RLMyCaregivers.IsVisible = index == 2;

                UpdateVisibleContext();
            };

            BtnMyInfo.IsToggled = true;
        }

        public override void AboutToShow()
        {
            base.AboutToShow();

            // Restore styles
            if (CaregiverFlow)
            {
                Titlebar.IsVisible = true;
                Titlebar.Title = AppResource.MyProfile;
                RootLayout.Style = (Style)Application.Current.Resources["AbsoluteLayout_NavigationOnTop"];
                LeftPageType = null;
            }
            else
            {
                Titlebar.IsVisible = false;
                this.Padding = new Thickness(0);
                LeftPageType = typeof(SettingsPage);
            }

            UpdateVisibleContext();

            _babyDeleteRequested = null;
        }

        #region Private - Common

        void UpdateVisibleContext()
        {
            ProfileManager.Instance.CurrentBabyChanged -= Instance_CurrentBabyChanged;
            ProfileManager.Instance.BabyModelPropertyChanged -= Instance_BabyModelPropertyChanged;
            ProfileManager.Instance.ProfilePropertyChanged -= Instance_ProfilePropertyChanged;

            if (RLMyInfo.IsVisible)
            {
                ProfileManager.Instance.ProfilePropertyChanged += Instance_ProfilePropertyChanged;
                ContextMyInfo.Reset();
            }
            else if (RLMyBabies.IsVisible)
            {
                ProfileManager.Instance.CurrentBabyChanged += Instance_CurrentBabyChanged;
                ProfileManager.Instance.BabyModelPropertyChanged += Instance_BabyModelPropertyChanged;

                ContextMyChildren.Reset();
            }
            else if (RLMyCaregivers.IsVisible)
            {
                ProfileManager.Instance.ProfilePropertyChanged += Instance_ProfilePropertyChanged;
                ContextMyCaregivers.Reset();
            }
        }

        void Instance_ProfilePropertyChanged(ProfileModel profile, string propertyName)
        {
            if (propertyName == "Caregivers")
            {
                if (RLMyInfo.IsVisible)
                {
                    ContextMyInfo.Reset();
                }
                else if (RLMyCaregivers.IsVisible)
                {
                    ContextMyCaregivers.Refresh(true);
                }
            }
        }

        #endregion

        #region Private - My Info

        void ChangePassword()
        {
            PageManager.Me.SetCurrentPage(typeof(ChangePasswordPage));
        }

        void AddAuthCode()
        {
            PageManager.Me.SetCurrentPage(typeof(AddAuthCodePage));
        }

        void ContextMyInfo_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if( e.PropertyName == "IsPrimarySelected" )
            {
                LeftPageType = ContextMyInfo.IsPrimarySelected ? typeof(SettingsPage) : typeof(CaregiverTabbedPage);

                if( CaregiverFlow )
                {
                    PageManager.Me.SetCurrentPage(LeftPageType);
                }
            }
        }

        #endregion

        #region Private - Children

        private ICommand _pickerFocusCommand;
        public ICommand PickerFocusCommand
        {
            get
            {
                _pickerFocusCommand = _pickerFocusCommand ?? new Command((obj) =>
                {
                    if (!obj.GetType().Equals(typeof(DatePicker)))
                    {
                        return;
                    }

                    ((DatePicker)obj).Focus();
                });
                return _pickerFocusCommand;
            }
        }

        private void AddBaby()
        {
            PageManager.Me.SetCurrentPage(typeof(BabyAdditionPage), View => 
            {
                (View as BabyAdditionPage).UpdateParams(ProfileManager.Instance, false, true);
            });
        }

        private void RemoveBaby(bool flag)
        {
            if( null != _babyDeleteRequested )
            {
                if( flag )
                {
                    ProfileManager.Instance.RemoveBaby(_babyDeleteRequested);
                    _babyDeleteRequested = null;

                    ContextMyChildren.UpdateBabiesList();
                }
                else
                {
                    _babyDeleteRequested.IsDeleteRequested = false;
                    _babyDeleteRequested = null;
                }

                Titlebar.LeftButton.IsVisible = true;
                ContextMyChildren.IsBabyDeleteRequested = false;
            }
        }

        private void Instance_CurrentBabyChanged(object sender, EventArgs e)
        {
            ContextMyChildren.UpdateBabiesList();
        }

        private void Instance_BabyModelPropertyChanged(BabyModel baby, string propertyName)
        {
            if(propertyName == "IsSelected")
            {
                ContextMyChildren.UpdateBabiesList();
            }
            else if (propertyName == "IsDeleteRequested")
            {
                _babyDeleteRequested = baby;

                Titlebar.LeftButton.IsVisible = false;
                ContextMyChildren.IsBabyDeleteRequested = true;            }
        }

        #endregion

        #region Private - Caregivers

        void AddCaregiver()
        {
            PageManager.Me.SetCurrentPage(typeof(InviteCaregiverPage));
        }

        void ContextMyCaregivers_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsCaregiverDeleteRequested")
            {
                if (ContextMyCaregivers.IsCaregiverDeleteRequested)
                {
                    oldLeftPageType = LeftPageType;
                    LeftPageType = null;
                }
                else
                {
                    LeftPageType = oldLeftPageType;
                    oldLeftPageType = null;
                }
            }
        }

        #endregion
    }

    public class MyInfoViewModel : ObservableObject
    {
        private Action ChangePasswordAction { get; set; }
        private Action AddAuthCodeAction { get; set; }

        public MyInfoViewModel(Action changePassword, Action addAuthCode)
        {
            ChangePasswordAction = changePassword;
            AddAuthCodeAction = addAuthCode;
        }

        public void Reset()
        {
            IsYourselfAsCaregiverDeleteRequested = false;

            SetPropertyChanged(nameof(UserName));
            SetPropertyChanged(nameof(UserEmail));
            SetPropertyChanged(nameof(CaregiverEmail));
            SetPropertyChanged(nameof(IsPrimarySelected));
            SetPropertyChanged(nameof(IsCaregiverExist));
        }

        #region Public UI properties

        private bool _isShowMainPage;
        public bool IsShowMainPage
        {
            get => _isShowMainPage;
            set
            {
                SetPropertyChanged(ref _isShowMainPage, value);
            }
        }

        private bool _isYourselfAsCaregiverDeleteRequested;
        public bool IsYourselfAsCaregiverDeleteRequested
        {
            get => _isYourselfAsCaregiverDeleteRequested;
            set
            {
                SetPropertyChanged(ref _isYourselfAsCaregiverDeleteRequested, value);
                IsShowMainPage = !value;
            }
        }

        #endregion

        #region Public UI properties - User

        public string UserName
        {
            get => ProfileManager.Instance.CurrentProfile?.Name ?? String.Empty;
            set
            {
                if (null != ProfileManager.Instance.CurrentProfile && !String.IsNullOrEmpty(value))
                {
                    ProfileManager.Instance.CurrentProfile.Name = value;

                    SetPropertyChanged(nameof(UserName));
                }
            }
        }

        public string UserEmail
        {
            get => ProfileManager.Instance.CurrentProfile?.Email ?? String.Empty;
        }

        public bool IsPrimarySelected
        {
            get => !(ProfileManager.Instance.CurrentProfile?.CaregiverAccountSelected ?? false);
        }

        #endregion

        #region Public UI properties - Caregiver

        public bool IsCaregiverExist => null != (ProfileManager.Instance.CurrentProfile?.CurrentCaregiver ?? null);

        public string CaregiverEmail
        {
            get => ProfileManager.Instance.CurrentProfile?.CurrentCaregiver?.CaregiverEmail ?? String.Empty;
        }

        #endregion

        #region Commands

        private ICommand _selectPrimaryAccountCommand;
        public ICommand SelectPrimaryAccountCommand
        {
            get
            {
                _selectPrimaryAccountCommand = _selectPrimaryAccountCommand ?? new Command(() =>
                {
                    ProfileManager.Instance.CurrentProfile.CaregiverAccountSelected = false;

                    SetPropertyChanged(nameof(IsPrimarySelected));
                });
                return _selectPrimaryAccountCommand;
            }
        }

        private ICommand _selectCaregiversAccountCommand;
        public ICommand SelectCaregiversAccountCommand
        {
            get
            {
                _selectCaregiversAccountCommand = _selectCaregiversAccountCommand ?? new Command(() =>
                {
                    ProfileManager.Instance.CurrentProfile.CaregiverAccountSelected = true;

                    SetPropertyChanged(nameof(IsPrimarySelected));
                });
                return _selectCaregiversAccountCommand;
            }
        }

        private ICommand _changePasswordCommand;
        public ICommand ChangePasswordCommand
        {
            get
            {
                _changePasswordCommand = _changePasswordCommand ?? new Command(() =>
                {
                    ChangePasswordAction?.Invoke();
                });
                return _changePasswordCommand;
            }
        }

        private ICommand _addCodeCommand;
        public ICommand AddCodeCommand
        {
            get
            {
                _addCodeCommand = _addCodeCommand ?? new Command(() =>
                {
                    AddAuthCodeAction?.Invoke();
                });
                return _addCodeCommand;
            }
        }

        private ICommand _removeFromAccountCommand;
        public ICommand RemoveFromAccountCommand
        {
            get
            {
                _removeFromAccountCommand = _removeFromAccountCommand ?? new Command(() =>
                {
                    IsYourselfAsCaregiverDeleteRequested = true;
                });
                return _removeFromAccountCommand;
            }
        }

        private ICommand _keepYourselfAsCaregiverCommand;
        public ICommand KeepYourselfAsCaregiverCommand
        {
            get
            {
                _keepYourselfAsCaregiverCommand = _keepYourselfAsCaregiverCommand ?? new Command(() =>
                {
                    IsYourselfAsCaregiverDeleteRequested = false;
                });
                return _keepYourselfAsCaregiverCommand;
            }
        }

        private ICommand _confirmDeleteYourselfAsCaregiverCommand;
        public ICommand ConfirmDeleteYourselfAsCaregiverCommand
        {
            get
            {
                _confirmDeleteYourselfAsCaregiverCommand = _confirmDeleteYourselfAsCaregiverCommand ?? new Command(async () =>
                {
                    await ProfileManager.Instance.RemoveCaregiver(ProfileManager.Instance.CurrentProfile.CurrentCaregiver, true);

                    ProfileManager.Instance.CurrentProfile.CurrentCaregiver = null;

                    SetPropertyChanged(nameof(CaregiverEmail));
                    SetPropertyChanged(nameof(IsPrimarySelected));
                    SetPropertyChanged(nameof(IsCaregiverExist));

                    IsYourselfAsCaregiverDeleteRequested = false;
                });

                return _confirmDeleteYourselfAsCaregiverCommand;
            }
        }

        #endregion

        #region Private
        #endregion
    }

    public class MyChildrenViewModel : ObservableObject
    {
        private Action AddBabyAction { get; set; }
        private Action<bool> RemoveBabyAction { get; set; }

        public MyChildrenViewModel(Action addBaby, Action<bool> removeBaby)
        {
            AddBabyAction = addBaby;
            RemoveBabyAction = removeBaby;
        }

        public void Reset()
        {
            IsBabyDeleteRequested = false;

            _babies = null;

            Refresh();
        }

        public void UpdateBabiesList()
        {
            _babies = null;
            Refresh();
        }

        #region Public UI properties

        public bool IsPrimarySelected
        {
            get => !(ProfileManager.Instance.CurrentProfile?.CaregiverAccountSelected ?? false);
        }

        private bool _isShowMainPage;
        public bool IsShowMainPage
        {
            get => _isShowMainPage;
            set
            {
                SetPropertyChanged(ref _isShowMainPage, value);
            }
        }

        public DateTime MinimumDate => DateTime.Now.FirstDayOfYear().Subtract(TimeSpan.FromDays(1 + (5 * 365))); // 5 years
        public DateTime MaximumDate => DateTime.Today;

        private bool _isBabyDeleteRequested;
        public bool IsBabyDeleteRequested 
        {
            get =>_isBabyDeleteRequested;
            set
            {
                SetPropertyChanged(ref _isBabyDeleteRequested, value);
                IsShowMainPage = !value;
            }
        }

        private ObservableCollection<BabyModel> _babies;
        public ObservableCollection<BabyModel> BabiesList
        {
            get => _babies ?? null;
        }

        #endregion

        #region Commands

        private ICommand _addBabyCommand;
        public ICommand AddBabyCommand
        {
            get
            {
                _addBabyCommand = _addBabyCommand ?? new Command(() =>
                {
                    AddBabyAction?.Invoke();
                });
                return _addBabyCommand;
            }
        }

        private ICommand _keepBabyCommand;
        public ICommand KeepBabyCommand
        {
            get
            {
                _keepBabyCommand = _keepBabyCommand ?? new Command(() =>
                {
                    RemoveBabyAction?.Invoke(false);
                });
                return _keepBabyCommand;
            }
        }

        private ICommand _removeBabyCommand;
        public ICommand RemoveBabyCommand
        {
            get
            {
                _removeBabyCommand = _removeBabyCommand ?? new Command(() =>
                {
                    RemoveBabyAction?.Invoke(true);
                });
                return _removeBabyCommand;
            }
        }

        #endregion

        #region Private

        private void Refresh()
        {
            if (null == BabiesList)
            {
                if (0 < (ProfileManager.Instance.CurrentProfile?.Babies?.Count ?? 0))
                {
                    _babies = new ObservableCollection<BabyModel>(ProfileManager.Instance.CurrentProfile.Babies);
                    _babies.ForEach((obj) => obj.IsDeleteRequested = false );
                }
            }

            SetPropertyChanged(nameof(BabiesList));
        }

        #endregion
    }

    public class CaregiversViewModel : ObservableObject
    {
        private Action AddCaregiverAction { get; set; }

        private CaregiverModel _caregiverDeleteRequested = null;

        public CaregiversViewModel(Action addCaregiver)
        {
            AddCaregiverAction = addCaregiver;
        }

        public void Reset()
        {
            IsCaregiverDeleteRequested = false;

            _caregivers = null;

            UpdateCaregivers();
        }

        public void Refresh(bool force = false)
        {
            UpdateCaregivers(force);
        }

        private bool _isShowMainPage;
        public bool IsShowMainPage
        {
            get => _isShowMainPage;
            set
            {
                SetPropertyChanged(ref _isShowMainPage, value);
            }
        }

        #region Public UI properties - User

        public bool IsPrimarySelected
        {
            get => !(ProfileManager.Instance.CurrentProfile?.CaregiverAccountSelected ?? false);
        }

        private bool _isCaregiverDeleteRequested;
        public bool IsCaregiverDeleteRequested
        { 
            get => _isCaregiverDeleteRequested; 
            set
            {
                SetPropertyChanged(ref _isCaregiverDeleteRequested, value);
                IsShowMainPage = !value;
            }
        }

        private ObservableCollection<CaregiverModel> _caregivers;
        public ObservableCollection<CaregiverModel> CaregiversList
        {
            get => _caregivers ?? null;
        }

        #endregion


        #region Commands

        private ICommand _addCaregiverCommand;
        public ICommand AddCaregiverCommand
        {
            get
            {
                _addCaregiverCommand = _addCaregiverCommand ?? new Command(() =>
                {
                    AddCaregiverAction?.Invoke();
                });
                return _addCaregiverCommand;
            }
        }

        private ICommand _removeCaregiverCommand;
        public ICommand RemoveCaregiverCommand
        {
            get
            {
                _removeCaregiverCommand = _removeCaregiverCommand ?? new Command((obj) =>
                {
                    if (!obj.GetType().Equals(typeof(CaregiverModel)))
                    {
                        return;
                    }

                    IsCaregiverDeleteRequested = true;

                    _caregiverDeleteRequested = (CaregiverModel)obj;
                });
                return _removeCaregiverCommand;
            }
        }

        private ICommand _keepCaregiverCommand;
        public ICommand KeepCaregiverCommand
        {
            get
            {
                _keepCaregiverCommand = _keepCaregiverCommand ?? new Command(() =>
                {
                    _caregiverDeleteRequested = null;
                    IsCaregiverDeleteRequested = false;
                });
                return _keepCaregiverCommand;
            }
        }

        private ICommand _confirmDeleteCaregiverCommand;
        public ICommand ConfirmDeleteCaregiverCommand
        {
            get
            {
                _confirmDeleteCaregiverCommand = _confirmDeleteCaregiverCommand ?? new Command( async () =>
                {
                    await ProfileManager.Instance.RemoveCaregiver(_caregiverDeleteRequested);

                    _caregiverDeleteRequested = null;

                    SetPropertyChanged(nameof(CaregiversList));

                    IsCaregiverDeleteRequested = false;
                });
                return _confirmDeleteCaregiverCommand;
            }
        }

        #endregion


        #region Private

        private void UpdateCaregivers(bool force = false)
        {
            ProfileModel profile = ProfileManager.Instance.CurrentProfile;
            if( null != profile )
            {
                if (null == CaregiversList && 0 < profile.Caregivers.Count)
                {
                    _caregivers = new ObservableCollection<CaregiverModel>(profile.Caregivers);
                    CaregiverModel caregiver = _caregivers.FirstOrDefault(c => c.CaregiverId == profile.ProfileId);
                    if (null != caregiver)
                    {
                        _caregivers.Remove(caregiver);
                    }
                }

                if (force)
                {
                    CaregiversList.Clear();

                    foreach (var item in profile.Caregivers)
                    {
                        if (profile.ProfileId == item.CaregiverId)
                            continue;

                        CaregiversList.Add(item);
                    }
                }
                SetPropertyChanged(nameof(CaregiversList));
            }
        }

        #endregion
    }
}
