#define CREATE_PAGE_ON_DEMAND
#undef CREATE_PAGE_ON_DEMAND
using System;
using System.Collections.Generic;
using BabyationApp.Pages;
using Xamarin.Forms;
using BabyationApp.Managers;
using BabyationApp.Models;
using System.Diagnostics;
using System.Threading.Tasks;
using BabyationApp.Controls;
using BabyationApp.Pages.BottleSession;
using BabyationApp.Pages.FirstTimeUser;
using BabyationApp.Pages.Modes;
using BabyationApp.Pages.NurseSession;
using BabyationApp.Helpers;
using BabyationApp.Interfaces;
using BabyationApp.Pages.PumpSession;
using Microsoft.AppCenter.Crashes;
using BabyationApp.Pages.Settings;

namespace BabyationApp
{
    /// <summary>
    /// The class Entry point in the Portable project
    /// </summary>
    public partial class App : Application
    {
        public static App Instance => (App)Application.Current;
        public IPlatformAPI PlatformAPI { get; protected set; }
        // app coordinates

        public App()
        {
            InitializeComponent();           
        }
    
        protected string IsFirstTime
        {
            get
            {
                return Settings.IsFirstTimeUser;
            }
            set
            {
                if (Settings.IsFirstTimeUser != value)
                {
                    Settings.IsFirstTimeUser = value;
                    OnPropertyChanged();
                }
            }
        }

        protected override void OnStart()
        {
            try
            {
                PlatformAPI = DependencyService.Get<IPlatformAPI>();
                UpdateGlobalUIStyle();

                // Handle when your app starts
                AnalyticsManager.Instance.Start();                

                DataManager dataManager = DataManager.Instance;
                dataManager.InitializeCompleteEvent += DataManager_InitializeCompleteEvent;
                dataManager.SetUserCompleteEvent += DataManager_SetUserCompleteEvent;

                if (IsFirstTime == "yes")
                {
                    // if this is the first time, set it to "No" and load the
                    // Main Page ,which will show at the first time use
                    PageManager.Me.AddCachePage(typeof(DashboardTabPage));
                    PageManager.Me.AddCachePage(typeof(BabyAdditionPage));
                    PageManager.Me.AddCachePage(typeof(PumpAdditionPage));
                }

                PageManager.Me.StartCachingPages();
            }
            catch (Exception ex)
            {
                Debugger.Break();
                AnalyticsManager.Instance.TrackError(ex);
                throw;
            }
        }

        protected override void OnSleep()
        {
            base.OnSleep();
        }
        protected override void OnResume()
        {
            base.OnResume();
        }

        private bool _cloudSyncComplete = false;
        private bool _pumpSyncComplete = false;

        private void DataManager_InitializeCompleteEvent(object sender, object e)
        {
            try
            {
                ExperienceManager.Instance.Initialize();
                HistoryManager.Instance.Initialize();

            }
            catch (Exception ex)
            {
                Debugger.Break();
                AnalyticsManager.Instance.TrackError(ex);
                throw;
            }
        }

        private void PumpManager_FirmwareAvailableEvent(object sender, EventArgs e)
        {
            // Notify user?
        }

        private void PumpManager_PumpDisconnectedEvent(object sender, EventArgs e)
        {
            _pumpSyncComplete = false;
        }

        private async void DataManager_SetUserCompleteEvent(object sender, object e)
        {
            try
            {
                // Start Bluetooth
                BluetoothManager bluetoothManager = BluetoothManager.Instance;
                bluetoothManager.Initialize();

                // Start looking for pumps
                PumpManager pumpManager = PumpManager.Instance;
                pumpManager.PumpConnectedEvent += PumpManager_PumpConnectedEvent;
                pumpManager.PumpDisconnectedEvent += PumpManager_PumpDisconnectedEvent;
                pumpManager.FirmwareAvailableEvent += PumpManager_FirmwareAvailableEvent;
                await pumpManager.Initialize();
                pumpManager.Start();
                pumpManager.Sync();

                ExperienceManager.Instance.Sync();
                // Make sure the media manager is synced before loading the profile
                await MediaManager.Instance.Sync();
                ProfileManager.Instance.Initialize();
                HistoryManager.Instance.Sync();

                _cloudSyncComplete = true;
                SyncPump();

                // Sync additional changes as they occur
                ExperienceManager.Instance.ExperienceAddedEvent -= Instance_ExperienceAddedEvent;
                ExperienceManager.Instance.ExperienceChangedEvent -= Instance_ExperienceChangedEvent;
                ExperienceManager.Instance.ExperienceAddedEvent += Instance_ExperienceAddedEvent;
                ExperienceManager.Instance.ExperienceChangedEvent += Instance_ExperienceChangedEvent;

            }
            catch (Exception ex)
            {
                Debugger.Break();
                AnalyticsManager.Instance.TrackError(ex);               
            }
        }

        private void Instance_ExperienceChangedEvent(object sender, ExperienceChangedEventArgs e)
        {
            try
            {

                PumpModel pump = PumpManager.Instance.ConnectedPump;

                if (pump != null)
                {
                    pump.UserExperiences = ExperienceManager.Instance.UserExperiences;
                }
            }
            catch (Exception ex)
            {
                Debugger.Break();
                AnalyticsManager.Instance.TrackError(ex);                
            }
        }

        private void Instance_ExperienceAddedEvent(object sender, ExperienceAddedEventArgs e)
        {
            try
            {
                PumpModel pump = PumpManager.Instance.ConnectedPump;

                if (pump != null)
                {
                    pump.UserExperiences = ExperienceManager.Instance.UserExperiences;
                }

            }
            catch (Exception ex)
            {
                Debugger.Break();
                AnalyticsManager.Instance.TrackError(ex);                
            }
        }

        public async void AfterStart()
        {
            try
            {
                PageManager.Me.StartPageType = typeof(WelcomePage);

                try
                {
                    Task[] tasks = new Task[] { DataManager.Instance.Initialize(LoginManager.Instance.CurrentClient) };
                    await Task.WhenAll(tasks);

                    if (IsFirstTime == "yes")
                    {
                        ProfileManager.Instance.IsFirstTimeUser = true;
                        // if this is the first time, set it to "No" and load the
                        // Main Page ,which will show at the first time use
                        //                IsFirstTime = "no";
                        PageManager.Me.StartPageType = typeof(SignUpPage);
                    }
                    else
                    {
                        ProfileManager.Instance.IsFirstTimeUser = false;
                        if (await LoginManager.Instance.Authenticated())
                        {
                            await DataManager.Instance.SetNewUser(LoginManager.Instance.UserId);
                            if (IsFirstTime == "yes")
                            {
                                PageManager.Me.StartPageType = typeof(BabyAdditionPage);
                            }
                            else
                            {
                                if (!String.IsNullOrEmpty(Settings.CaregiverCode))
                                {
                                    PageManager.Me.StartPageType = typeof(AddAuthCodePage);
                                }
                                else
                                {
                                    PageManager.Me.StartPageType = typeof(DashboardTabPage);
                                }
                            }
                        }
                        else
                        {
                            PageManager.Me.StartPageType = typeof(SignUpPage);
                        }
                    }
                    PageManager.Me.SetCurrentPage(PageManager.Me.StartPageType, view =>
                    {
                        var welcomePage = view as WelcomePage;
                        if (welcomePage != null)
                        {
                            welcomePage.Initialize(false);
                        }
                    });
                }
                catch (Exception ex)
                {
                    Debugger.Break();
                    AnalyticsManager.Instance.TrackError(ex);
                    string message = ex.Message;
                }

            }
            catch (Exception ex)
            {
                Debugger.Break();
                AnalyticsManager.Instance.TrackError(ex);
            }
            //PageManager.Me.StartCachingPages();
        }

        private void SyncPump()
        {
            try
            {
                if (!_pumpSyncComplete)
                {
                    HistoryManager historyManager = HistoryManager.Instance;
                    PumpModel pump = PumpManager.Instance.ConnectedPump;

                    if (pump != null)
                    {
                        // Sync preset experiences to the pump
                        pump.PresetExperiences = ExperienceManager.Instance.PresetExperiences;

                        // Sync user experiences to the pump
                        pump.UserExperiences = ExperienceManager.Instance.UserExperiences;

                        // Add these sessions to the History Manager to sync locally and with the cloud
                        foreach (HistoryModel pumpSession in pump.PumpingSessions)
                        {
                            historyManager.AddSession(pumpSession);
                        }

                        _pumpSyncComplete = true;
                    }
                }

            }
            catch (Exception ex)
            {
                Debugger.Break();
                AnalyticsManager.Instance.TrackError(ex);                
            }
        }

        private void PumpManager_PumpConnectedEvent(object sender, PumpConnectedEventArgs e)
        {
            try
            {

                PumpModel pump = PumpManager.Instance.ConnectedPump;

                // If a pump session is active go straight to the pumping screen
                if ((pump.ActualState == PumpState.Start) || (pump.ActualState == PumpState.Pause))
                {
                    if (SessionManager.Instance.CurrentSession == null)
                    {
                        SessionManager.Instance.ContinuePumping();
                        Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                        {
                            PageManager.Me.SetPage(typeof(PumpSessionPage));
                        });
                    }
                }

                pump.PropertyChanged += Pump_PropertyChanged;

                if (_cloudSyncComplete)
                {
                    SyncPump();
                }
            }
            catch (Exception ex)
            {
                Debugger.Break();
                AnalyticsManager.Instance.TrackError(ex);
                throw;
            }
        }

        private void Pump_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            try
            {
                HistoryManager historyManager = HistoryManager.Instance;
                PumpModel pump = PumpManager.Instance.ConnectedPump;

                if (e.PropertyName == "PumpingSessions")
                {
                    // Add these sessions to the History Manager to sync locally and with the cloud
                    foreach (HistoryModel pumpSession in pump.PumpingSessions)
                    {
                        historyManager.AddSession(pumpSession);
                    }
                }
                else if (e.PropertyName == "ActualState")
                {
                    if (pump.ActualState == PumpState.Start)
                    {
                        if (SessionManager.Instance.CurrentSession == null)
                        {
                            SessionManager.Instance.ContinuePumping();
                            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                            {
                                PageManager.Me.SetPage(typeof(PumpSessionPage));
                            });
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Debugger.Break();
                AnalyticsManager.Instance.TrackError(ex);
                throw;
            }
        }

        private void UpdateGlobalUIStyle()
        {
            try
            {

                switch (Device.RuntimePlatform)
                {
                    case Device.iOS:
                        {
                            //
                            // Longread: https://stackoverflow.com/questions/46192280/detect-if-the-device-is-iphone-x?page=1&tab=votes#tab-top
                            //
                            bool hasNotch = PlatformAPI.HasTopNotch();
                            Thickness safeArea = PlatformAPI.SafeArea();

                            double statusH = Double.Parse(Application.Current.Resources["StatusBarHeight"].ToString());
                            double navH = Double.Parse(Application.Current.Resources["NavBarHeight"].ToString());
                            double bottomH = Double.Parse(Application.Current.Resources["BottomBarHeight"].ToString());

                            Application.Current.Resources["StatusBarHeight"] = hasNotch ? safeArea.Top : statusH;
                            Application.Current.Resources["NavBarHeight"] = hasNotch ? 50 : navH;
                            Application.Current.Resources["BottomBarHeight"] = hasNotch ? bottomH + safeArea.Bottom : bottomH;

                            statusH = Double.Parse(Application.Current.Resources["StatusBarHeight"].ToString());
                            navH = Double.Parse(Application.Current.Resources["NavBarHeight"].ToString());

                            Application.Current.Resources["StatusBarInsets"] = new Thickness(0, statusH, 0, 0);
                            Application.Current.Resources["NavBarInsets"] = new Thickness(0, navH, 0, 0);
                        }
                        break;
                    default:
                        {
                            Application.Current.Resources["StatusBarInsets"] = new Thickness(0, 0, 0, 0);
                            Application.Current.Resources["NavBarInsets"] = new Thickness(0, 35, 0, 0);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Debugger.Break();
                AnalyticsManager.Instance.TrackError(ex);
                throw;
            }
        }

        protected override void OnAppLinkRequestReceived(Uri uri)
        {
            Debug.WriteLine($"AppLink: {uri}");

            const string associatedHost = "https://babyation.azurewebsites.net";

            if (!uri.ToString().ToLowerInvariant().StartsWith(associatedHost, StringComparison.Ordinal))
                return;

            string pageUrl = uri.ToString().Replace(associatedHost, string.Empty).Trim();
            var parts = pageUrl.Split('?');
            string parameter = parts[1].Replace("code=", string.Empty);

            if (!String.IsNullOrEmpty(parameter))
            {
                Settings.CaregiverCode = parameter;

                if (null != ProfileManager.Instance.CurrentProfile)
                {
                    PageManager.Me.SetCurrentPage(typeof(AddAuthCodePage));
                }
            }

            base.OnAppLinkRequestReceived(uri);
        }
    }

    /// <summary>
    /// This sealed class a the manager that chaches the pages, instantiates the pages on deman, shows the pages on titlebar actions
    /// and other classes use this class to show a page
    /// </summary>
    sealed class PageManager
    {
        public delegate void PageCreated(Type type, IRootView page);
        private static PageManager _me = new PageManager();
        public static PageManager Me
        {
            get { return _me; }
        }

        private Dictionary<Type, IRootView> _pages = new Dictionary<Type, IRootView>();
        private Queue<Type> _cachePages = new Queue<Type>();
        private object _cacheLock = new object();
        private StackPagesContainerPage _pageMain;
        private ContentPage _currentPage = null;
        SplashPage _splashPage;
        public Type StartPageType { get; set; }

        //public event PageCreated PageCreatedEvent;

        /// <summary>
        /// Constructor initialize the caching pages list
        /// </summary>
        private PageManager()
        {
            try
            {
                var api = DependencyService.Get<IPlatformAPI>();
                if (api != null)
                {
                    api.UpdateStatusBar("#F8EBE3", false);
                }

                _pageMain = new StackPagesContainerPage();
                Application.Current.MainPage = _pageMain;
                if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.iOS)
                {
                    _pageMain.BackgroundColor = Color.FromHex("#F8EBE3");
                    _splashPage = new SplashPage();
                    _pageMain.Children.Add(_splashPage);
                }

                //_cachePages.Enqueue(typeof(PumpSessionPage));
                //_cachePages.Enqueue(typeof(BottleFeedSelectionPage));
                //_cachePages.Enqueue(typeof(NurseSessionSelectionPage));
                //_cachePages.Enqueue(typeof(SelectSpeedPage));
                //_cachePages.Enqueue(typeof(EnterOtherInfoPage));
                //_cachePages.Enqueue(typeof(RemindersPage));
                //_cachePages.Enqueue(typeof(CreateReminderTypePage));
                //_cachePages.Enqueue(typeof(CreateReminderPage));
                //_cachePages.Enqueue(typeof(SettingsPage));
                //_cachePages.Enqueue(typeof(MyPumpsPage));
                //_cachePages.Enqueue(typeof(PumpDetailPage));
                //_cachePages.Enqueue(typeof(BottleFeedStartPage));
                //_cachePages.Enqueue(typeof(NurseSessionStartPage));
                //_cachePages.Enqueue(typeof(MyInventoryPage));
                //_cachePages.Enqueue(typeof(ProfilePage));
                //_cachePages.Enqueue(typeof(BluetoothPumpDetectedPage));
                //_cachePages.Enqueue(typeof(BluetoothPumpSetupPage));                                  

            }
            catch (Exception ex)
            {
                Debugger.Break();
                throw;
            }
        }

        Dictionary<Type, IRootView> _finishedPages = new Dictionary<Type, IRootView>();


        /// <summary>
        /// Adds a page in the to be caches pages list
        /// </summary>
        /// <param name="page"></param>
        public void AddCachePage(Type page)
        {
            try
            {
                _cachePages.Enqueue(page);

            }
            catch (Exception ex)
            {
                Debugger.Break();
                AnalyticsManager.Instance.TrackError(ex);
                throw;
            }
        }

        /// <summary>
        /// Stars the cacheing of the pages
        /// </summary>
        public void StartCachingPages()
        {
            try
            {
                //await Task.Run(() =>
                //{

                //});
                try
                    {
                        bool finished = false;
                        Type pageType;

                        while (!finished)
                        {
                            if (_cachePages.Count > 0)
                            {
                                pageType = _cachePages.Dequeue();
                                if (_finishedPages.ContainsKey(pageType)) continue;
                                try
                                {
                                    var page = Activator.CreateInstance(pageType) as IRootView;
                                    if (page == null)
                                    {
                                        return;
                                    }
                                    page.PageCreationDone();
                                    _finishedPages[pageType] = page;
                                }
                                catch (Exception e)
                                {
                                    throw e;
                                }
                            }
                            else
                            {
                                finished = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debugger.Break();
                        throw;
                    }

                try
                {
                    foreach (var kvp in _finishedPages)
                    {
                        IRootView finishedPage = kvp.Value;
                        var contentPage = finishedPage as ContentPage;
                        try
                        {
                            if (contentPage != null)
                            {
                                if (!_pageMain.Children.Contains(contentPage))
                                {
                                    _pageMain.Children.Add(contentPage);
                                }
                            }


                            if (!_pages.ContainsKey(finishedPage.GetType()))
                            {
                                (finishedPage as VisualElement).IsVisible = false;
                                _pages.Add(finishedPage.GetType(), finishedPage);
                                //PageCreatedEvent?.Invoke(finishedPage.GetType(), finishedPage);
                            }
                        }
                        catch (Exception exc)
                        {
                            AnalyticsManager.Instance.TrackError(exc);
                            System.Diagnostics.Debug.WriteLine("Exception " + exc.Message);
                        }
                    }

                    if (_futurePage != null)
                    {
                        UpdateCurrentPage(_futurePage);
                        _futurePage = null;
                    }
                }
                catch (Exception ex)
                {
                    Debugger.Break();
                    AnalyticsManager.Instance.TrackError(ex);
                    throw;
                }

            }
            catch (Exception ex)
            {
                Debugger.Break();
                AnalyticsManager.Instance.TrackError(ex);
                throw;
            }
        }

        /// <summary>
        /// Intantiate a page instantly
        /// </summary>
        /// <param name="type">Type of the page to be intantiated</param>
        /// <param name="beforeCallBack">The callback called before the instantiation</param>
        /// <param name="afterCallBack">The callback called after the instantiation</param>
        private async void AddPage(Type type, Action<IRootView> beforeCallBack = null, Action<IRootView> afterCallBack = null)
        {
            try
            {
                IRootView page = null;

                await Task.Run(() =>
                {
                    try
                    {
                        //page = (IRootView)Activator.CreateInstance(type);
                        page = Activator.CreateInstance(type) as IRootView;
                        if (page == null)
                        {
                            return;
                        }
                        page.PageCreationDone();
                    }
                    catch (Exception e)
                    {
                        Debugger.Break();
                        AnalyticsManager.Instance.TrackError(e);
                        throw e;
                    }
                });

                _pages.Add(page.GetType(), page);
                UpdateCurrentPage(type, beforeCallBack, afterCallBack);
            }
            catch (Exception ex)
            {
                Debugger.Break();
                AnalyticsManager.Instance.TrackError(ex);
                throw;
            }
        }

        private void AddPage(Type type, Action<IRootView> beforeCallBack = null, Action<IRootView> afterCallBack = null, params object[] args)
        {
            try
            {
                IRootView page = null;

                page = Activator.CreateInstance(type) as IRootView;
                if (page == null)
                {
                    return;
                }
                page.PageCreationDone();

                _pages.Add(page.GetType(), page);

                UpdateCurrentPage(type, beforeCallBack, afterCallBack);
            }
            catch (Exception ex)
            {
                Debugger.Break();
                AnalyticsManager.Instance.TrackError(ex);
                throw;
            }
        }

        private Type _futurePage = null;

        /// <summary>
        /// Sets a given page as current page and shows it
        /// </summary>
        /// <param name="type"></param>
        public void SetPage(Type type)
        {
            try
            {
                if (_pages.ContainsKey(type))
                {
                    UpdateCurrentPage(type);
                }
                else
                {
                    _futurePage = type;
                }

            }
            catch (Exception ex)
            {
                Debugger.Break();
                AnalyticsManager.Instance.TrackError(ex);
                throw;
            }
        }

        /// <summary>
        /// Sets a given page as current page and shows it
        /// </summary>
        /// <param name="type"></param>
        /// <param name="beforeCallBack">The callback called before the showing</param>
        /// <param name="afterCallBack">The callback called after the showing</param>
        public void SetCurrentPage(Type type,
                                   Action<IRootView> beforeCallBack = null,
                                   Action<IRootView> afterCallBack = null,
                                   params object[] args)
        {
            try
            {
                if (StartPageType != type && _splashPage != null && _splashPage.IsVisible)
                {
                    _splashPage.IsVisible = false;
                }
                if (!_pages.ContainsKey(type))
                {
                    AddPage(type, beforeCallBack, afterCallBack, args);
                }
                else
                {
                    UpdateCurrentPage(type, beforeCallBack, afterCallBack);
                }
            }
            catch (Exception ex)
            {
                Debugger.Break();
                AnalyticsManager.Instance.TrackError(ex);
                throw;
            }
        }

        private void UpdateCurrentPage(Type type, Action<IRootView> beforeCallBack = null, Action<IRootView> afterCallBack = null)
        {
            try
            {
                if (_pages.ContainsKey(type))
                {
                    var page = _pages[type] as ContentPage;
                    if (page != null)
                    {
                        if (page != _currentPage)
                        {
                            beforeCallBack?.Invoke(_pages[type]);
                            _currentPage = page;
                            if (!_pageMain.Children.Contains(page))
                            {
                                _pageMain.Children.Add(page);
                            }

                            ((IRootView)page).AboutToShow();
                            page.IsVisible = true;
                            _pageMain.CurrentPage = page;
                            afterCallBack?.Invoke(_pages[type]);
                            foreach (ContentPage p in _pageMain.Children)
                            {
                                if (page != p && _splashPage != p)
                                {
                                    p.IsVisible = false;
                                }
                            }
                        }
                    }
                }
                else
                {
                    Debug.WriteLine("Not cached yet");
                }
            }
            catch (Exception ex)
            {
                Debugger.Break();
                AnalyticsManager.Instance.TrackError(ex);
                throw;
            }
        }

        /// <summary>
        /// Sets the navigation pages for the titlebar left/right button for a page
        /// </summary>
        /// <param name="page"></param>
        public void SetNavPagesForPage(IRootView page)
        {
            try
            {
                page.Titlebar.LeftButton.Clicked += (s, e) =>
                {
                    if (page.LeftPageType != null)
                    {
                        SetCurrentPage(page.LeftPageType);
                    }
                };

                page.Titlebar.RightButton.Clicked += (s, e) =>
                {
                    if (page.RightPageType != null)
                    {
                        SetCurrentPage(page.RightPageType);
                    }
                };
            }
            catch (Exception ex)
            {
                Debugger.Break();
                AnalyticsManager.Instance.TrackError(ex);
                throw;
            }
        }

        /// <summary>
        /// Gets the DashboarTabPage
        /// </summary>
        /// <returns>The application's dashboard page</returns>
        public DashboardTabPage GetDashboardTabPage()
        {
            if (_pages.ContainsKey(typeof(DashboardTabPage)))
            {
                return _pages[typeof(DashboardTabPage)] as DashboardTabPage;
            }
            return null;
        }
    }
}
