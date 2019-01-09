using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using BabyationApp.DataObjects;
using System.Net.Http;
using ModernHttpClient;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Net.Http.Headers;
using BabyationApp.Helpers;
using System.Collections.Generic;
using Plugin.Connectivity;
using System.Threading;
using Xamarin.Forms;

namespace BabyationApp.Managers
{
    public enum SyncState
    {
        Error = -1,
        Complete = 0,
        CompleteWithConflicts =1,
        Offline = 99
    }

    public enum UserType
    {
        Error = -1,
        ExistingUser = 0,
        NewUser = 1,
        NoUser = 99
    }


    public class DataManager
    {
        static DataManager _instance = null;
        static bool _initialized = false;
        private IMobileServiceClient client;
        private string _currentUserId = string.Empty;
        private string _currentProfileId = string.Empty;
        private bool _isLoggedIn = false;

        private IMobileServiceSyncTable<DataObjects.AccessGroup> accessGroupTable;
        private IMobileServiceSyncTable<DataObjects.AccessType> accessTypeTable;
        private IMobileServiceSyncTable<DataObjects.Children> childrenTable;
        private IMobileServiceSyncTable<DataObjects.CaregiverRequest> caregiverRequestTable;
        private IMobileServiceSyncTable<DataObjects.CaregiverRelation> caregiverRelationTable;
        private IMobileServiceSyncTable<DataObjects.Experience> experienceTable;
        private IMobileServiceSyncTable<DataObjects.HistoricalSession> historicalSessionTable;
        private IMobileServiceSyncTable<DataObjects.Media> mediaTable;
        private IMobileServiceSyncTable<DataObjects.Profile> profileTable;
        private IMobileServiceSyncTable<DataObjects.ProfileUser> profileUserTable;
        private IMobileServiceSyncTable<DataObjects.Pump> pumpTable;
        private IMobileServiceSyncTable<DataObjects.Reminder> reminderTable;
        private IMobileServiceSyncTable<DataObjects.ReminderUser> reminderUserTable;
        private IMobileServiceSyncTable<DataObjects.Role> roleTable;
        private IMobileServiceSyncTable<DataObjects.User> userTable;
        private IMobileServiceSyncTable<DataObjects.Firmware> firmwareTable;

        /// <summary>
        /// Event when Datamanager Initizalitaion is complete indicating that local table access is available
        /// </summary>
        public event EventHandler<object> InitializeCompleteEvent;
        /// <summary>
        /// Event when Datamanager has set the current authorized user in data manager
        /// </summary>
        public event EventHandler<object> SetUserCompleteEvent;

        
        protected virtual void OnSetUserCompleteEvent(object e)
        {
            EventHandler<object> handler = SetUserCompleteEvent;
            if(handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnRaiseInitializeComplete(object e)
        {
            EventHandler<object> handler = InitializeCompleteEvent;
            if(handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Access Group Table
        /// </summary>
        public IMobileServiceSyncTable<DataObjects.AccessGroup> AccessGroup { get { return accessGroupTable; } }
        /// <summary>
        /// Access Type Table
        /// </summary>
        public IMobileServiceSyncTable<DataObjects.AccessType> AccessType { get { return accessTypeTable; } }
        /// <summary>
        /// Child Table
        /// </summary>
        public IMobileServiceSyncTable<DataObjects.Children> Child { get { return childrenTable; } }
        /// <summary>
        /// CaregiverRequest Table
        /// </summary>
        public IMobileServiceSyncTable<DataObjects.CaregiverRequest> CaregiverRequest { get { return caregiverRequestTable; } }
        /// <summary>
        /// CaregiverRelation Table
        /// </summary>
        public IMobileServiceSyncTable<DataObjects.CaregiverRelation> CaregiverRelation { get { return caregiverRelationTable; } }
        /// <summary>
        /// Experience Table
        /// </summary>
        public IMobileServiceSyncTable<DataObjects.Experience> Experience { get { return experienceTable; } }
        /// <summary>
        /// Historical Sessin Table
        /// </summary>
        public IMobileServiceSyncTable<DataObjects.HistoricalSession> HistoricalSession { get { return historicalSessionTable; } }
        /// <summary>
        /// Media Table
        /// </summary>
        public IMobileServiceSyncTable<DataObjects.Media> Media { get { return mediaTable; } }
        /// <summary>
        /// Profile Table
        /// </summary>
        public IMobileServiceSyncTable<DataObjects.Profile> Profile { get { return profileTable; } }
        /// <summary>
        /// Profile User Table
        /// </summary>
        public IMobileServiceSyncTable<DataObjects.ProfileUser> ProfileUser { get { return profileUserTable; } }
        /// <summary>
        /// Pump Table
        /// </summary>
        public IMobileServiceSyncTable<DataObjects.Pump> Pump { get { return pumpTable; } }
        /// <summary>
        /// Reminder Table
        /// </summary>
        public IMobileServiceSyncTable<DataObjects.Reminder> Reminder { get {return reminderTable; } }
        /// <summary>
        /// Reminder User Table
        /// </summary>
        public IMobileServiceSyncTable<DataObjects.ReminderUser> ReminderUser { get { return reminderUserTable; } }
        /// <summary>
        /// Role Table
        /// </summary>
        public IMobileServiceSyncTable<DataObjects.Role> Role { get { return roleTable; } }
        /// <summary>
        /// User Table
        /// </summary>
        public IMobileServiceSyncTable<DataObjects.User> User { get { return userTable; } }
        /// <summary>
        /// Firmware Table
        /// </summary>
        public IMobileServiceSyncTable<DataObjects.Firmware> Firmware { get { return firmwareTable; } }

        /// <summary>
        /// Gets Instance of DataManager
        /// </summary>
        static public DataManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DataManager();
                }

                return _instance;
            }
        }

        /// <summary>
        /// Private Constructor to ensure only a single instance is created
        /// </summary>
        private DataManager()
        {
        }

        /// <summary>
        /// Initializes Local Table Storage for application
        /// </summary>
        /// <returns>System.Threading.Tasks.Task</returns>
        public async Task Initialize(MobileServiceClient msClient)
        {
            if (!_initialized)
            {
                try
                {
                    //client = new MobileServiceClient(_aPIbaseURL, new AuthHandler());
                    client = msClient;
                    const string fileName = "babyationstore.db";
                    //setup our local sqlite store and intialize our table
                    var store = new MobileServiceSQLiteStore(DependencyService.Get<IFileHelper>().GetLocalFilePath(fileName));
                    store.DefineTable<DataObjects.AccessGroup>();
                    store.DefineTable<DataObjects.AccessType>();
                    store.DefineTable<DataObjects.Children>();
                    store.DefineTable<DataObjects.CaregiverRequest>();
                    store.DefineTable<DataObjects.CaregiverRelation>();
                    store.DefineTable<DataObjects.Experience>();
                    store.DefineTable<DataObjects.HistoricalSession>();
                    store.DefineTable<DataObjects.Media>();
                    store.DefineTable<DataObjects.Profile>();
                    store.DefineTable<DataObjects.ProfileUser>();
                    store.DefineTable<DataObjects.Pump>();
                    store.DefineTable<DataObjects.Reminder>();
                    store.DefineTable<DataObjects.ReminderUser>();
                    store.DefineTable<DataObjects.Role>();
                    store.DefineTable<DataObjects.User>();
                    store.DefineTable<DataObjects.Firmware>();
                    await client.SyncContext.InitializeAsync(store);

                    accessGroupTable = this.client.GetSyncTable<DataObjects.AccessGroup>();
                    accessTypeTable = this.client.GetSyncTable<DataObjects.AccessType>();
                    childrenTable = this.client.GetSyncTable<DataObjects.Children>();
                    caregiverRequestTable = this.client.GetSyncTable<DataObjects.CaregiverRequest>();
                    caregiverRelationTable = this.client.GetSyncTable<DataObjects.CaregiverRelation>();
                    experienceTable = this.client.GetSyncTable<DataObjects.Experience>();
                    historicalSessionTable = this.client.GetSyncTable<DataObjects.HistoricalSession>();
                    mediaTable = this.client.GetSyncTable<DataObjects.Media>();
                    profileTable = this.client.GetSyncTable<DataObjects.Profile>();
                    profileUserTable = this.client.GetSyncTable<DataObjects.ProfileUser>();
                    pumpTable = this.client.GetSyncTable<DataObjects.Pump>();
                    reminderTable = this.client.GetSyncTable<DataObjects.Reminder>();
                    reminderUserTable = this.client.GetSyncTable<DataObjects.ReminderUser>();
                    roleTable = this.client.GetSyncTable<DataObjects.Role>();
                    userTable = this.client.GetSyncTable<DataObjects.User>();
                    firmwareTable = this.client.GetSyncTable<DataObjects.Firmware>();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                finally
                {
                    OnRaiseInitializeComplete(new object());
                }
            }
        }

        public void SignOut()
        {
            _currentProfileId = string.Empty;
            _currentUserId = string.Empty;
            _isLoggedIn = false;
        }

        /// <summary>
        /// Sets Current User Data in local tables.
        /// </summary>
        /// <param name="Id">String Guid of current authorized user</param>
        /// <returns>System.Threading.Tasks.Task</returns>
        public async Task SetNewUser(string Id)
        {
            try
            {
                DataObjects.Profile defaultProfile = null;
                //New User, User should be connected to internet for sign in to work.
                //Need to sync user table to see if user already exists.
                if (string.IsNullOrEmpty(_currentUserId) || _currentUserId == Guid.Empty.ToString())
                {
                    await SyncUser();
                }
                DataObjects.User currentUser = await userTable.LookupAsync(Id);

                //If user still not found after sync need to insert new user
                if (currentUser == null || currentUser.Id != Id)
                {
                    currentUser = new DataObjects.User() { Id = Id, ActiveDirectoryObjectId = Id };
                    await userTable.InsertAsync(currentUser);
                    defaultProfile = new DataObjects.Profile() { Id = Guid.NewGuid().ToString(), PrimaryUserId = currentUser.Id };
                    await AddUpdateProfile(defaultProfile);
                    await SetUserDefaultProfile(currentUser.Id, defaultProfile.Id);
                    await SyncUser();
                    await SyncProfile();

                } else
                {
                    //User Existed In Cloud, retrieve profile
                    await SyncProfile();
                    defaultProfile = await GetProfile(currentUser.DefaultProfileId);
                    if(defaultProfile == null)
                    {
                        defaultProfile = new DataObjects.Profile() { Id = Guid.NewGuid().ToString(), PrimaryUserId = currentUser.Id };
                        await AddUpdateProfile(defaultProfile);
                        await SetUserDefaultProfile(currentUser.Id, defaultProfile.Id);
                        await SyncUser();
                        await SyncProfile();
                    }
                }

                //Make Sure user has a profile.
                _currentUserId = Id;
                _currentProfileId = defaultProfile.Id;
                _isLoggedIn = true;
                await SyncPump();
                await SyncChild();
                await SyncMedia();
                await Task.WhenAll(new Task[] { SyncPresetExperiences(), 
                                                SyncUserExperiences(), 
                                                SyncHistoricalSessions(), 
                                                SyncCaregiverRequest(), 
                                                SyncCaregiverRelation() });
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
            finally
            {
                OnSetUserCompleteEvent(new object());
            }
        }

        /// <summary>
        /// Syncs Current User Table with mobile API values
        /// </summary>
        /// <param name="Id">Current Authorized User Id</param>
        /// <returns>System.Threading.Tasks.Task</returns>
        public async Task<SyncState> SyncUser()
        {
            SyncState state = SyncState.Offline;
            try
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    await userTable.PullAsync(Guid.NewGuid().ToString(), userTable.Select(u => u));
                    state = SyncState.Complete;
                }
            }
            catch (MobileServicePushFailedException exc)
            {
                await SimpleConflictResolution(exc);
                state = SyncState.CompleteWithConflicts;
            }
            catch (Exception ex)
            {
                state = SyncState.Error;
            }
            return state;
        }

        /// <summary>
        /// Syncs Pump Table with mobile API values
        /// </summary>
        /// <param name="Id">Pump Id to Sync</param>
        /// <returns>System.Threading.Tasks.Task</returns>

        public async Task<SyncState> SyncPump()
        {
            SyncState state = SyncState.Offline;
            if (CrossConnectivity.Current.IsConnected)
            {
                try
                {
                    await pumpTable.PullAsync(Guid.NewGuid().ToString(), pumpTable.Select(p => p));
                    state = SyncState.Complete;
                }
                catch (MobileServicePushFailedException exc)
                {
                    await SimpleConflictResolution(exc);
                    state = SyncState.CompleteWithConflicts;
                }
            }
            return state;
        }

        public async Task<SyncState> SyncFirmware()
        {
            SyncState state = SyncState.Offline;
            if (CrossConnectivity.Current.IsConnected)
            {
                try
                {
                    //gets the latest from the table
                    await firmwareTable.PullAsync(Guid.NewGuid().ToString(), firmwareTable.Select(f => f));
                    state = SyncState.Complete;
                }
                catch (MobileServicePushFailedException exc)
                {
                    await SimpleConflictResolution(exc);
                    state = SyncState.CompleteWithConflicts;
                }
                catch (Exception ex)
                {
                    state = SyncState.Error;
                }
            }
            return state;

        }

        /// <summary>
        /// Syncs Profile Table with mobile API values
        /// </summary>
        /// <param name="Id">Profile ID to sync</param>
        /// <returns>System.Threading.Tasks.Task</returns>
        public async Task<SyncState> SyncProfile()
        {
            SyncState state = SyncState.Offline;
            if (CrossConnectivity.Current.IsConnected)
            {
                try
                {
                    await profileTable.PullAsync(Guid.NewGuid().ToString(), profileTable.Select(p => p));
                }
                catch (MobileServicePushFailedException exc)
                {
                    await SimpleConflictResolution(exc);
                    state = SyncState.CompleteWithConflicts;
                }
                catch { state = SyncState.Error; }
            }
            return state;
        }

        /// <summary>
        /// Syncs Profile User and Pump Tables with mobile API values
        /// </summary>
        /// <param name="Id">Current Authorized User Id</param>
        /// <returns>System.Threading.Tasks.Task</returns>
        /*
        public async Task<SyncState> SyncUserData(string Id)
        {
            SyncState state = SyncState.Offline;
            if (CrossConnectivity.Current.IsConnected)
            {
                try
                {
                    string transId = "pullProfileUser" + Id;
                    if (transId.Length > 50)
                    {
                        transId = transId.Substring(0, 50);
                    }
                    await profileUserTable.PullAsync(transId, profileUserTable.Where(w => w.UserId == Id));
                }
                catch (MobileServicePushFailedException exc)
                {
                    await SimpleConflictResolution(exc);
                    state = SyncState.CompleteWithConflicts;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    state = SyncState.Error;
                }

                IList<string> profileIds = await profileUserTable.Select(s => s.ProfileId).ToListAsync();

                try
                {
                    string transId = "pullProfile" + Id;
                    if (transId.Length > 50)
                    {
                        transId = transId.Substring(0, 50);
                    }
                    await profileTable.PullAsync(transId, profileTable.Where(w => profileIds.Contains(w.Id)));
                }
                catch (MobileServicePushFailedException exc)
                {
                    await SimpleConflictResolution(exc);
                    if(state != SyncState.Error)
                    {
                        state = SyncState.CompleteWithConflicts;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    state = SyncState.Error;
                }

                try
                {
                    string transId = "pullPumps" + Id;
                    if (transId.Length > 50)
                    {
                        transId = transId.Substring(0, 50);
                    }
                    await profileTable.PullAsync(transId, pumpTable.Where(w => profileIds.Contains(w.ProfileId)));
                }
                catch (MobileServicePushFailedException exc)
                {
                    await SimpleConflictResolution(exc);
                    if (state != SyncState.Error)
                    {
                        state = SyncState.CompleteWithConflicts;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    state = SyncState.Error;
                }
                if(state != SyncState.Error && state != SyncState.CompleteWithConflicts)
                {
                    state = SyncState.Complete;
                }

            }
            return state;
        }
*/
        /// <summary>
        /// Adds or Inserts Pump Into Pump Table and Syncs with mobile API values
        /// </summary>
        /// <param name="pump">DataObjects.Pump row to add or update</param>
        /// <returns>System.Threading.Tasks.Task</returns>
        public async Task AddUpdatePump(DataObjects.Pump pump)
        {
            try
            {
                // Make sure the user is logged in so UserId and ProfileId are valid
                if (_isLoggedIn)
                {
                    pump.ProfileId = _currentProfileId;

                    if (null == await pumpTable.LookupAsync(pump.Id))
                    {
                        await pumpTable.InsertAsync(pump);
                    }
                    else
                    {
                        await pumpTable.UpdateAsync(pump);
                    }
                    await SyncPump();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error adding Pump: " + ex.Message);
            }
        }

        /// <summary>
        /// Return All Pumps in Pump Table
        /// </summary>
        /// <returns>System.Threading.Tasks.Task</returns>
        public async Task<IEnumerable<DataObjects.Pump>> GetAllPumps()
        {
            return await pumpTable?.Where(p => p.ProfileId == _currentProfileId).ToEnumerableAsync();
        }

        /// <summary>
        /// Deletes Pump From Local Tables the Syncs Changes with mobile api
        /// </summary>
        /// <param name="pump">DataObjects.Pump row to delete</param>
        /// <returns></returns>
        public async Task DeletePump(DataObjects.Pump pump)
        {
            try
            { 
                await pumpTable.DeleteAsync(pump);
                await SyncPump();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error removing pump: " + ex.Message);
            }
}

        /// <summary>
        /// Syncs User Experience Table with mobile api
        /// </summary>
        /// <param name="profileId">string profileid to sync</param>
        /// <returns>System.Theading.Tasks.Task</returns>
        public async Task<SyncState> SyncUserExperiences()
        {
            string emptyGuid = Guid.Empty.ToString();
            SyncState state = SyncState.Offline;
            if (CrossConnectivity.Current.IsConnected)
            {
                try
                {
                    await experienceTable.PullAsync(Guid.NewGuid().ToString(), experienceTable.Select(e => e));
                    state = SyncState.Complete;
                }
                catch (MobileServicePushFailedException exc)
                {
                    await SimpleConflictResolution(exc);
                    state = SyncState.CompleteWithConflicts;
                }
                catch { state = SyncState.Error; }
            }
            return state;
        }

#if DEBUG
        public async Task DeleteUserExperience(DataObjects.Experience experience)
        {
            try
            {
                // Make sure the user is logged in so UserId and ProfileId are valid
                if (_isLoggedIn)
                {
                    experience.ProfileId = _currentProfileId;

                    if (null != await experienceTable.LookupAsync(experience.Id))
                    {
                        await experienceTable.DeleteAsync(experience);
                    }
                    await SyncUserExperiences();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error deleting User Experience: " + ex.Message);
            }
        }
#endif

        /// <summary>
        /// Syncs Preset Experiences with mobile api
        /// </summary>
        /// <returns>System.Theading.Task.Tasks</returns>
        public async Task<SyncState> SyncPresetExperiences()
        {
            string emptyGuid = Guid.Empty.ToString();
            SyncState state = SyncState.Offline;
            if (CrossConnectivity.Current.IsConnected)
            {
                try
                {
                    //TODO: Verify requirement of where
                    await experienceTable.PullAsync(Guid.NewGuid().ToString(), experienceTable.Where(w => w.ProfileId == emptyGuid));
                    state = SyncState.Complete;
                }
                catch (MobileServicePushFailedException exc)
                {
                    await SimpleConflictResolution(exc);
                    state = SyncState.CompleteWithConflicts;
                }
                catch(Exception ex)
                {
                    var message = ex.Message;
                    state = SyncState.Error; }
            }
            return state;
        }

        /// <summary>
        /// Syncs Historical Sessions with mobile api
        /// </summary>
        /// <returns>System.Theading.Task.Tasks</returns>
        public async Task<SyncState> SyncHistoricalSessions()
        {
            SyncState state = SyncState.Offline;
            if (CrossConnectivity.Current.IsConnected)
            {
                try
                {
                    await historicalSessionTable.PullAsync(Guid.NewGuid().ToString(), historicalSessionTable.Select(h => h));
                    state = SyncState.Complete;
                }
                catch (MobileServicePushFailedException exc)
                {
                    await SimpleConflictResolution(exc);
                    state = SyncState.CompleteWithConflicts;
                }
                catch {
                    state = SyncState.Error;
                }
            }
            return state;
        }

        /// <summary>
        /// Adds or Updates a Historical Session and Syncs with mobile API values
        /// </summary>
        /// <param name="historicalSession">DataObjects.HistoricalSession row to add or update</param>
        /// <returns>System.Threading.Tasks.Task</returns>
        public async Task AddHistoricalSession(DataObjects.HistoricalSession historicalSession)
        {
            try
            {
                // Make sure the user is logged in so UserId and ProfileId are valid
                if (_isLoggedIn)
                {
                    historicalSession.UserId = _currentUserId;
                    historicalSession.ProfileId = _currentProfileId;

                    if (null == await historicalSessionTable.LookupAsync(historicalSession.Id))
                    {
                        await historicalSessionTable.InsertAsync(historicalSession);
                    }
                    else
                    {
                        await historicalSessionTable.UpdateAsync(historicalSession);
                    }
                    await SyncHistoricalSessions();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error adding Historical Session: " + ex.Message);
            }
        }

        /// <summary>
        /// Gets Historical Sessions
        /// </summary>
        /// <returns>System.Threading.Tasks.Task</returns>
        public async Task<IEnumerable<HistoricalSession>> GetHistoricalSessions()
        {
            return await historicalSessionTable.Where(h => h.ProfileId == _currentProfileId).ToEnumerableAsync();
        }

        /// <summary>
        /// Adds or Updates a Historical Session and Syncs with mobile API values
        /// </summary>
        /// <param name="historicalSession">DataObjects.HistoricalSession row to add or update</param>
        /// <returns>System.Threading.Tasks.Task</returns>
        public async Task AddUserExperience(DataObjects.Experience experience)
        {
            try
            {
                // Make sure the user is logged in so UserId and ProfileId are valid
                if (_isLoggedIn)
                {
                    experience.ProfileId = _currentProfileId;

                    if (null == await experienceTable.LookupAsync(experience.Id))
                    {
                        await experienceTable.InsertAsync(experience);
                    }
                    else
                    {
                        await experienceTable.UpdateAsync(experience);
                    }
                    await SyncUserExperiences();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error adding User Experience: " + ex.Message);
            }
        }

        /// <summary>
        /// Syncs Child Table with mobile api
        /// </summary>
        /// <param name="profileId">string profileid to sync</param>
        /// <returns>System.Theading.Tasks.Task</returns>
        public async Task<SyncState> SyncMedia()
        {
            SyncState state = SyncState.Offline;
            if (CrossConnectivity.Current.IsConnected)
            {
                try
                {
                    await mediaTable.PullAsync(Guid.NewGuid().ToString(), mediaTable.Select(m => m));
                    state = SyncState.Complete;
                }
                catch (MobileServicePushFailedException exc)
                {
					try
					{
						await SimpleConflictResolution(exc);
					}
					catch { }
                    state = SyncState.CompleteWithConflicts;
                }
                catch { state = SyncState.Error; }
            }
            return state;
        }

        /// <summary>
        /// Adds or Updates a Child and Syncs with mobile API values
        /// </summary>
        /// <param name="child">DataObjects.HistoricalSession row to add or update</param>
        /// <returns>System.Threading.Tasks.Task</returns>
        public async Task AddMedia(DataObjects.Media media)
        {
            // Make sure the user is logged in so UserId and ProfileId are valid
            if (_isLoggedIn)
            {
                media.ProfileId = _currentProfileId;

                if (null == await mediaTable.LookupAsync(media.Id))
                {
                    await mediaTable.InsertAsync(media);
                }
                else
                {
                    await mediaTable.UpdateAsync(media);
                }
                await SyncMedia();
            }
        }

        /// <summary>
        /// Deletes Pump From Local Tables the Syncs Changes with mobile api
        /// </summary>
        /// <param name="pump">DataObjects.Pump row to delete</param>
        /// <returns></returns>
        public async Task RemoveMedia(DataObjects.Media media)
        {
            try
            {
                await mediaTable.DeleteAsync(media);
                await SyncMedia();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error removing media: " + ex.Message);
            }
        }

        /// <summary>
        /// Syncs Child Table with mobile api
        /// </summary>
        /// <param name="profileId">string profileid to sync</param>
        /// <returns>System.Theading.Tasks.Task</returns>
        public async Task<SyncState> SyncChild()
        {
            //string emptyGuid = Guid.Empty.ToString();
            SyncState state = SyncState.Offline;
            if (CrossConnectivity.Current.IsConnected)
            {
                try
                {
                    await childrenTable.PullAsync(Guid.NewGuid().ToString(), childrenTable.Select(c => c));
                    state = SyncState.Complete;
                }
                catch (MobileServicePushFailedException exc)
                {
                    await SimpleConflictResolution(exc);
                    state = SyncState.CompleteWithConflicts;
                }
                catch (Exception ex)
                {
                    state = SyncState.Error;
                }
            }
            return state;
        }

        /// <summary>
        /// Adds or Updates a Child and Syncs with mobile API values
        /// </summary>
        /// <param name="child">DataObjects.HistoricalSession row to add or update</param>
        /// <returns>System.Threading.Tasks.Task</returns>
        public async Task AddChild(DataObjects.Children child)
        {
            // Make sure the user is logged in so UserId and ProfileId are valid
            if (_isLoggedIn)
            {
                child.ProfileId = _currentProfileId;

                if (null == await childrenTable.LookupAsync(child.Id))
                {
                    await childrenTable.InsertAsync(child);
                }
                else
                {
                    await childrenTable.UpdateAsync(child);
                }
                await SyncChild();
            }
        }

        /// <summary>
        /// Deletes Pump From Local Tables the Syncs Changes with mobile api
        /// </summary>
        /// <param name="pump">DataObjects.Pump row to delete</param>
        /// <returns></returns>
        public async Task RemoveChild(DataObjects.Children child)
        {
            await childrenTable.DeleteAsync(child);
            await SyncChild();
        }

        /// <summary>
        /// Gets Preset Experiences
        /// </summary>
        /// <returns>System.Threading.Tasks.Task</returns>
        public async Task<IEnumerable<Experience>> GetPresetExperiences()
        {
            string emptyGuid = Guid.Empty.ToString();
            return await experienceTable.Where(e => e.ProfileId == emptyGuid).ToEnumerableAsync();
        }

        /// <summary>
        /// Gets User Experiences
        /// </summary>
        /// <returns>System.Threading.Tasks.Task</returns>
        public async Task<IEnumerable<Experience>> GetUserExperiences()
        {
            string emptyGuid = Guid.Empty.ToString();
            return await experienceTable.Where(e => e.ProfileId == _currentProfileId).ToEnumerableAsync();
        }

        /// <summary>
        /// Sets User Default Profile
        /// </summary>
        /// <param name="userId">string user id</param>
        /// <param name="defaultProfileId">string profile id</param>
        /// <returns>System.Theading.Tasks.Task</returns>
        public async  Task<SyncState> SetUserDefaultProfile(string userId, string defaultProfileId)
        {
            DataObjects.User user = await userTable.LookupAsync(userId);
            if(user != null)
            {
                user.DefaultProfileId = defaultProfileId;
                await userTable.UpdateAsync(user);
            }
            return await SyncUser();
        }

        /// <summary>
        /// Gets A Profile Of A User
        /// </summary>
        /// <param name="userId">string user id of proile to get</param>
        /// <returns>System.Threading.Tasks.Task</returns>
        public async Task<Profile> GetUserProfiles(string defaultProfileId)
        {
            IEnumerable<Profile> profiles = await profileTable.Where(p => p.Id == defaultProfileId).ToEnumerableAsync();

            return profiles?.FirstOrDefault();
        }

        /// <summary>
        /// Gets A Profile Of A User
        /// </summary>
        /// <param name="userId">string user id of proile to get</param>
        /// <returns>System.Threading.Tasks.Task</returns>
        public async Task<User> GetUser()
        {
            User user = null;
            try
            {
                user = (await userTable.Where(u => u.Id == _currentUserId).ToEnumerableAsync()).FirstOrDefault();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error getting user: " + ex.Message);
            }
            return user;
        }

        public async Task<User> GetUserByProfileId(string defaultProfileId)
        {
            User user = null;
            try
            {
                user = (await userTable.Where(u => u.DefaultProfileId == defaultProfileId).ToEnumerableAsync()).FirstOrDefault();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error getting user: " + ex.Message);
            }
            return user;
        }

        /// <summary>
        /// Updates a User and Syncs with mobile API values
        /// </summary>
        /// <param name="user">DataObjects.User row to add or update</param>
        /// <returns>System.Threading.Tasks.Task</returns>
        public async Task UpdateUser(DataObjects.User user)
        {
            try
            {
                await userTable.UpdateAsync(user);
                await SyncUser();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error updating user: " + ex.Message);
            }
        }

        /// <summary>
        /// Syncs CaregiverRequest Table with mobile api
        /// </summary>
        /// <returns>System.Theading.Tasks.Task</returns>
        public async Task<SyncState> SyncCaregiverRequest()
        {
            SyncState state = SyncState.Offline;

            if (CrossConnectivity.Current.IsConnected)
            {
                try
                {
                    await caregiverRequestTable.PullAsync(Guid.NewGuid().ToString(), caregiverRequestTable.Select(c => c));
                    state = SyncState.Complete;
                }
                catch (MobileServicePushFailedException exc)
                {
                    await SimpleConflictResolution(exc);
                    state = SyncState.CompleteWithConflicts;
                }
                catch
                {
                    state = SyncState.Error;
                }
            }

            return state;
        }

        /// <summary>
        /// Syncs CaregiverRelation Table with mobile api
        /// </summary>
        /// <returns>System.Theading.Tasks.Task</returns>
        public async Task<SyncState> SyncCaregiverRelation()
        {
            SyncState state = SyncState.Offline;

            if (CrossConnectivity.Current.IsConnected)
            {
                try
                {
                    await caregiverRelationTable.PullAsync(Guid.NewGuid().ToString(), caregiverRelationTable.Select(r => r));
                    state = SyncState.Complete;
                }
                catch (MobileServicePushFailedException exc)
                {
                    await SimpleConflictResolution(exc);
                    state = SyncState.CompleteWithConflicts;
                }
                catch
                {
                    state = SyncState.Error;
                }
            }

            return state;
        }

        public async Task RemoveCaregiverRelation(DataObjects.CaregiverRelation cRelation)
        {
            await caregiverRelationTable.DeleteAsync(cRelation);
            await SyncCaregiverRelation();
        }

        public async Task RemoveCaregiverRequest(DataObjects.CaregiverRequest cRequest)
        {
            await caregiverRequestTable.DeleteAsync(cRequest);
            await SyncCaregiverRequest();
        }

        /// <summary>
        /// Gets A Profile
        /// </summary>
        /// <param name="profileId">id of proile to get</param>
        /// <returns>System.Threading.Tasks.Task</returns>
        public async Task<Profile> GetProfile(string profileId)
        {
            Profile profile = null;
            IEnumerable<Profile> profiles = await profileTable.Select(p => p).ToEnumerableAsync();
            foreach (Profile profileIter in profiles)
            {
                //Debug.WriteLine($"pID: {profileIter.Id}");
                if (profileIter.Id == profileId)
                {
                    //Debug.WriteLine($"pMail: {profileIter.Email}");
                    profile = profileIter;
                    break;
                }
            }
            return profile;
        }

        /// <summary>
        /// Get CaregiverRequests associated with a profile
        /// </summary>
        /// <param name="profileId">string profile id</param>
        /// <returns>System.Threading.Tasks.Task</returns>
        public async Task<IEnumerable<CaregiverRequest>> GetCaregiversRequests(string profileId)
        {
            return await caregiverRequestTable.Where(c => c.ProfileId == profileId).ToEnumerableAsync();
        }

        public async Task<CaregiverRequest> GetCaregiverRequest(string profileId, string requestId)
        {
            IEnumerable<CaregiverRequest> result = await GetCaregiversRequests(profileId);

            return result?.FirstOrDefault(r => r.ProfileId == requestId);
        }

        public async Task<IEnumerable<CaregiverRelation>> GetCaregiversRelations(string profileId)
        {
            //IEnumerable<CaregiverRelation> items = await caregiverRelationTable.Select(p => p).ToEnumerableAsync();
            //Debug.WriteLine(items);

            return await caregiverRelationTable.Select(c => c).ToEnumerableAsync();
        }

        public async Task<CaregiverRelation> GetCaregiverRelation(string profileId, string relationId)
        {
            IEnumerable<CaregiverRelation> result = await GetCaregiversRelations(profileId);

            return result?.FirstOrDefault(r => r.ProfileId == relationId);
        }

        /// <summary>
        /// Get Childern Associated With a Profile
        /// </summary>
        /// <param name="profileId">string profile id</param>
        /// <returns>System.Threading.Tasks.Task</returns>
        public async Task<IEnumerable<Children>> GetChildren(string profileId)
        {
            return await childrenTable.Where(c => c.ProfileId == profileId).ToEnumerableAsync();
        }

        /// <summary>
        /// Get media associated with the media ID
        /// </summary>
        /// <param name="id">string media id</param>
        /// <returns>System.Threading.Tasks.Task</returns>
        public async Task<IEnumerable<DataObjects.Media>> GetMedia(string id)
        {
            return await mediaTable.Where(m => m.Id == id).ToEnumerableAsync();
        }

        /// <summary>
        /// Get all media associated with the profile ID
        /// </summary>
        /// <returns>System.Threading.Tasks.Task</returns>
        public async Task<IEnumerable<DataObjects.Media>> GetMedia()
        {
            return await mediaTable.Where(m => m.ProfileId == _currentProfileId).ToEnumerableAsync();
        }

        /// <summary>
        /// Inserts or updates profile in local table storage and syncs with mobile api
        /// </summary>
        /// <param name="profile">DataObjects.Profile to add or update</param>
        /// <returns>System.Theading.Tasks.Task</returns>
        public async Task<SyncState> AddUpdateProfile(DataObjects.Profile profile)
        {
            if (null == await profileTable.LookupAsync(profile.Id))
            {
                await profileTable.InsertAsync(profile);
            }
            else
            {
                await profileTable.UpdateAsync(profile);
            }
            return await SyncProfile();
        }


        /// <summary>
        /// Performs Simple Conflict Resolution reverting to server copy if mobile api update fails
        /// </summary>
        /// <param name="mspfe">MobileServicePushFailedException occured during sync</param>
        /// <returns></returns>
        private async Task SimpleConflictResolution(MobileServicePushFailedException mspfe)
        {
            if(mspfe.PushResult != null)
            foreach (var error in mspfe.PushResult.Errors)
            {
                    if (error.OperationKind == MobileServiceTableOperationKind.Update && error.Result != null)
                    {
                        //Update failed, reverting to server's copy.
                        await error.CancelAndUpdateItemAsync(error.Result);
                    }
                    if (error.OperationKind == MobileServiceTableOperationKind.Insert)
                    {
                        await error.CancelAndUpdateItemAsync(error.Result);
                    }
                    else
                    {
                        // Discard local change.
                        await error.CancelAndDiscardItemAsync();
                    }

                    Debug.WriteLine(@"Error executing sync operation. Item: {0} ({1}). Operation discarded.",
                        error.TableName, error.Item["id"]);
                }

        }
    }
}
