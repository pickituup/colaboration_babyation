using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BabyationApp.Models;
using BabyationApp.Helpers;
using BabyationApp.Managers;
using BabyationApp.DataObjects;
using Xamarin.Forms;
using System.IO;
using FFImageLoading;

namespace BabyationApp.Managers
{
    public delegate void EventProfilePropertyChanged(ProfileModel profile, string propertyName);
    public delegate void EventBabyModelPropertyChanged(BabyModel baby, string propertyName);
    public class ProfileManager
    {
        public event EventHandler CurrentBabyChanged;
        private static ProfileManager _instance = null;
        private ProfileModel _currentProfile = null;
        private bool _isFirstTimeUser = false;

        public static ProfileManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ProfileManager();
                }
                return _instance;
            }
        }

        private ProfileManager()
        {
        }

        public bool IsFirstTimeUser
        {
            get
            {
                return _isFirstTimeUser;
            }
            set
            {
                _isFirstTimeUser = value;
            }
        }

        public event EventProfilePropertyChanged ProfilePropertyChanged;
        public event EventBabyModelPropertyChanged BabyModelPropertyChanged;

        public void Reset()
        {
            _currentProfile = null;
        }

        public async Task Initialize()
        {
            await Sync();
        }

        public async Task Sync()
        {
            User user = await DataManager.Instance.GetUser();
            Profile profile = await DataManager.Instance.GetUserProfiles(LoginManager.Instance.UserId);
            MediaManager mediaManager = MediaManager.Instance;

            if ((user != null) && (profile != null))
            {
                IEnumerable<Children> children = await DataManager.Instance.GetChildren(profile.Id);
                BabyModel babyModel;
                string userName = (user.Name != null) ? user.Name : Name;

                // These will be grabbed from LoginManager once it is finished
                if (_currentProfile == null )
                {
                    _currentProfile = new ProfileModel()
                    {
                        Name = userName,
                        Email = UserEmail
                    };
                    _currentProfile.PropertyChanged += _currentProfile_PropertyChanged;

                    ProfilePropertyChanged?.Invoke(_currentProfile, nameof(CurrentProfile));
                }
                else if (_currentProfile.Name != userName || _currentProfile.Email != UserEmail)
                {
                    _currentProfile.Name = userName;
                    _currentProfile.Email = UserEmail;
                    _currentProfile.PropertyChanged += _currentProfile_PropertyChanged;

                    ProfilePropertyChanged?.Invoke(_currentProfile, nameof(CurrentProfile));
                }
              
                foreach (Children child in children)
                {
                    var existing = _currentProfile.Babies.Where(c => c.Id == child.Id).FirstOrDefault();

                    if (existing == null)
                    {
                        babyModel = new BabyModel()
                        {
                            Id = child.Id,
                            Name = child.Name,
                            Birthday = child.Birthday,
                        };

                        Media media = mediaManager.Get(child.MediaId);

                        if (media != null)
                        {
                            babyModel.Picture = media.Image;
                            babyModel.MediaId = media.Id;
                        }

                        babyModel.PropertyChanged += BabyModel_PropertyChanged;

                        _currentProfile.Babies.Add(babyModel);

                        if (_currentProfile.CurrentBaby == null)
                        {
                            _currentProfile.CurrentBaby = babyModel;
                        }
                    }
                }
            }
        }

        public BabyModel CreateBaby()
        {
            BabyModel babyModel = new BabyModel();
            babyModel.Id = Guid.NewGuid().ToString();
            babyModel.PropertyChanged += BabyModel_PropertyChanged;
            return babyModel;
        }

        public async void AddBaby(BabyModel babyModel)
        {
            if (_currentProfile != null)
            {
                try
                {
                    DataManager dataManager = DataManager.Instance;
                    MediaManager mediaManager = MediaManager.Instance;
                    Media media = null;

                    _currentProfile.Babies.Add(babyModel);

                    // If no current baby is set, make this the current baby as well
                    if (_currentProfile.CurrentBaby == null)
                    {
                        _currentProfile.CurrentBaby = babyModel;
                    }

                    if (babyModel.Picture != null)
                    {
                        media = mediaManager.CreateImageMedia(babyModel.Picture);
                        await mediaManager.Add(media);
                        babyModel.MediaId = media.Id;
                    }

                    Children child = new Children()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = babyModel.Name,
                        Birthday = DateTime.Now
                    };

                    if (media != null)
                    {
                        child.MediaId = media.Id;
                    }

                    await dataManager.AddChild(child);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("Exception in AddBaby - " + e.Message);
                }
            }
        }

        public async void RemoveBaby(BabyModel babyModel)
        {
            if (_currentProfile != null)
            {
                DataManager dataManager = DataManager.Instance;
                MediaManager mediaManager = MediaManager.Instance;

                _currentProfile.Babies.Remove(babyModel);

                if (_currentProfile.CurrentBaby == babyModel)
                {
                    _currentProfile.CurrentBaby = _currentProfile.Babies.FirstOrDefault();
                }

                if (babyModel.MediaId != Guid.Empty.ToString())
                {
                    await mediaManager.Remove(babyModel.MediaId);
                }

                // Remove child from Data Manager
                Children child = new Children()
                {
                    Id = babyModel.Id,
                };

                await dataManager.RemoveChild(child);
            }
        }

        private async void BabyModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_currentProfile == null)
            {
                return;
            }

            BabyModel babyModel = (BabyModel)sender;            

            if (e.PropertyName == "Picture")
            {
                babyModel = _currentProfile.CurrentBaby;

                if (babyModel != null)
                {
                    MediaManager mediaManager = MediaManager.Instance;

                    Media media = mediaManager.Get(babyModel.MediaId);

                    if (media == null)
                    {
                        // Insert
                        media = mediaManager.CreateImageMedia(babyModel.Picture);
                        await mediaManager.Add(media);
                        babyModel.MediaId = media.Id;
                    }
                    else
                    {
                        // Update
                        media.Image = babyModel.Picture;
                        await mediaManager.Update(media);
                    }
                }
            }
            else if (e.PropertyName == "IsSelected")
            {
                _currentProfile.CurrentBaby = babyModel;
            }
            else if (e.PropertyName == "IsDeleteRequested")
            {
                _currentProfile.ShowBabyDeleteAlert = babyModel.IsDeleteRequested;
            }

            BabyModelPropertyChanged?.Invoke(babyModel, e.PropertyName);
        }

        private async void _currentProfile_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentBaby")
            {
                CurrentBabyChanged?.Invoke(this, EventArgs.Empty);
            }
            else if (e.PropertyName == "Name")
            {
                // Save the name change
                User user = await DataManager.Instance.GetUser();

                user.Name = _currentProfile.Name;

                await DataManager.Instance.UpdateUser(user);
            }
        }

        public ProfileModel CurrentProfile
        {
            get
            {
                return _currentProfile;
            }
        }

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

    }
}
