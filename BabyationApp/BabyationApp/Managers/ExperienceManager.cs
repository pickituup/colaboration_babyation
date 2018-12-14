using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using BabyationApp.Models;
using Xamarin.Forms;
using BabyationApp.DataObjects;
using BabyationApp.Helpers;

namespace BabyationApp.Managers
{
    /// <summary>
    /// Arguments sent when an experience is added
    /// </summary>
    public class ExperienceAddedEventArgs : EventArgs
    {
        public string Id { get; internal set; }
    }

    /// <summary>
    /// Arguments sent when an experience changes
    /// </summary>
    public class ExperienceChangedEventArgs : EventArgs
    {
        public string Id { get; internal set; }
    }

    /// <summary>
    /// The ExperienceManager class
    /// </summary>
    public class ExperienceManager
    {
        private static ExperienceManager _instance = null;
        private ObservableCollection<ExperienceModel> _userExperiences = new ObservableCollection<ExperienceModel>();
        private ObservableCollection<ExperienceModel> _presetExperiences = new ObservableCollection<ExperienceModel>();
        private ObservableCollection<ExperienceModel> _allExperiences = new ObservableCollection<ExperienceModel>();
        private ExperienceModel _currentExperience = null;
        private int _nextUserExperienceId = 0;
        private bool _knownExperiencesParsed = false;
        private string _knownExperiencesString;
        private List<string> _knownExperiences = new List<string>();
        private List<string> _newExperiences = new List<string>();

        public event ExperienceEditEvent EditExperience;
        public event EventHandler CurrentExperienceChanged;
        public event EventHandler<ExperienceAddedEventArgs> ExperienceAddedEvent;
        public event EventHandler<ExperienceChangedEventArgs> ExperienceChangedEvent;

        /// <summary>
        /// Get the ExperienceManager singleton
        /// </summary>
        /// <returns>The ExperienceManager singleton</returns>
        public static ExperienceManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ExperienceManager();
                }
                return _instance;
            }
        }

        /// <summary>
        /// The ExperienceManager constructor
        /// </summary>
        private ExperienceManager()
        {
        }

        /// <summary>
        /// Reset the ExperienceManager. This can be used when a user logs out and
        /// a new user logs in.
        /// </summary>
        public void Reset()
        {
            _userExperiences.Clear();
            _presetExperiences.Clear();
            _allExperiences.Clear();
            Settings.KnownExperiences = string.Empty;
            _knownExperiencesParsed = false;
            _knownExperiences = new List<string>();
            _newExperiences = new List<string>();
            CreateDefaultExperiences();
        }

        /// <summary>
        /// Initialize the ExperienceManager
        /// </summary>
        /// <returns>System.Threading.Tasks.Task</returns>
        public async Task Initialize()
        {
            DetermineKnownExperiences();
            CreateDefaultExperiences();
            await Sync();
        }

        /// <summary>
        /// Determine which of the synched experiences were already known and
        /// which are new.
        /// </summary>
        private void DetermineKnownExperiences()
        {
            if (!_knownExperiencesParsed)
            {
                _knownExperiencesParsed = true;
                _knownExperiencesString = Settings.KnownExperiences;
                if (_knownExperiencesString != string.Empty)
                {
                    string[] experiences = _knownExperiencesString.Split(',');

                    foreach (string experience in experiences)
                    {
                        if (experience != string.Empty)
                        {
                            _knownExperiences.Add(experience);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sync the known experiences and save them locally so they are remembered
        /// on the next reboot.
        /// </summary>
        private void SyncKnownExperiences()
        {
            string knownExperiences = _knownExperiencesString;
            bool first = true;

            foreach (string experience in _newExperiences)
            {
                if (first)
                {
                    first = false;
                    knownExperiences += experience;
                }
                else
                {
                    knownExperiences += "," + experience;
                }
            }

            if (Settings.KnownExperiences != knownExperiences)
            {
                Settings.KnownExperiences = knownExperiences;
            }
        }

        /// <summary>
        /// Create the default experiences. This ensures that the experiences are available
        /// the first time a user starts the app even if they do not have a network connection.
        /// </summary>
        public void CreateDefaultExperiences()
        {
            ExperienceModel experienceModel;

            DateTimeOffset creationDate = DateTimeOffset.Parse("01/01/2015");

            // These also exist on the database and changes on the database will overwrite these defaults.
            // We include them here in case the app is not online the first time it is used
            experienceModel = new ExperienceModel()
            {
                Id = "920bfda9-9bd0-4641-b644-415d8aed04ac",
                Name = "First Time Pumper",
                CreatedBy = "babyation",
                Description = "Experience the ease of this setting for anyone who is using a pump for the first time",
                ExpressionSpeed = 4,
                ExpressionSuction = 5,
                StimulationSpeed = 2,
                StimulationSuction = 3,
                Breast = BreastType.Both,
                ExperienceId = 128,
                Duration = TimeSpan.FromSeconds(1800),
                TransitionType = TransitionType.Timed,
                TransitionTime = TimeSpan.FromSeconds(120),
                CreatedAt = creationDate.AddSeconds(1)
            };
            experienceModel.Edit += model => { OnEditExperience(model); };
            experienceModel.IsNew = !_knownExperiences.Contains(experienceModel.Id);

            _presetExperiences.Add(experienceModel);
            _allExperiences.Add(experienceModel);

            experienceModel = new ExperienceModel()
            {
                Id = "920bfda9-9bd0-4641-b644-415d8aed04ad",
                Name = "Power Pumping",
                CreatedBy = "babyation",
                Description = "Use this setting to help increase your milkflow",
                ExpressionSpeed = 5,
                ExpressionSuction = 4,
                StimulationSpeed = 1,
                StimulationSuction = 2,
                Breast = BreastType.Both,
                ExperienceId = 129,
                Duration = TimeSpan.FromSeconds(1800),
                TransitionType = TransitionType.Timed,
                TransitionTime = TimeSpan.FromSeconds(120),
                CreatedAt = creationDate.AddSeconds(2)
        };
            experienceModel.Edit += model => { OnEditExperience(model); };
            experienceModel.IsNew = !_knownExperiences.Contains(experienceModel.Id);

            _presetExperiences.Add(experienceModel);
            _allExperiences.Add(experienceModel);

            CurrentExperience = _presetExperiences[0];
        }

        /// <summary>
        /// Sync the ExperienceManager with the cloud
        /// </summary>
        /// <returns>System.Threading.Tasks.Task</returns>
        public async Task Sync()
        {
            ExperienceModel experienceModel;
            DataManager dataManager = DataManager.Instance;
            IEnumerable<Experience> experiences;
            bool isNew;
            try
            {
                DetermineKnownExperiences();

                experiences = await dataManager.GetPresetExperiences();

                if (experiences.Any())
                {
                    foreach (Experience experience in experiences)
                    {
                        isNew = !_knownExperiences.Contains(experience.Id);

                        experienceModel = new ExperienceModel()
                        {
                            Id = experience.Id,
                            Name = experience.Name,
                            CreatedBy = "babyation",
                            Description = experience.Description,
                            ExpressionSpeed = experience.ExpressionSpeed,
                            ExpressionSuction = experience.ExpressionSuction,
                            StimulationSpeed = experience.StimulationSpeed,
                            StimulationSuction = experience.StimulationSuction,
                            Breast = (BreastType)experience.Breast,
                            IsNew = isNew,
                            ExperienceId = experience.ExperienceId,
                            Duration = TimeSpan.FromSeconds(experience.Duration),
                            TransitionType = (TransitionType)experience.TransistionType,
                            TransitionTime = TimeSpan.FromSeconds(experience.TransistionTime),
                            Storage = (StorageType)experience.Storage,
                            CreatedAt = experience.CreatedAt
                    };

                        var existing = _allExperiences.Where(e => e.Id == experience.Id).FirstOrDefault();

                        if (existing == null)
                        {
                            _presetExperiences.Add(experienceModel);
                            _allExperiences.Add(experienceModel);
                            experienceModel.Edit += model => { OnEditExperience(model); };

                            if (isNew)
                            {
                                _newExperiences.Add(experience.Id);
                            }
                        }
                        else
                        {
                            existing.Copy(experienceModel);
                        }
                    }
                }

                experiences = await dataManager.GetUserExperiences();

                if (experiences.Any())
                {
                    foreach (Experience experience in experiences)
                    {
                        isNew = !_knownExperiences.Contains(experience.Id);

                        experienceModel = new ExperienceModel()
                        {
                            Id = experience.Id,
                            Name = experience.Name,
                            CreatedBy = "me",
                            Description = experience.Description,
                            ExpressionSpeed = experience.ExpressionSpeed,
                            ExpressionSuction = experience.ExpressionSuction,
                            StimulationSpeed = experience.StimulationSpeed,
                            StimulationSuction = experience.StimulationSuction,
                            Breast = (BreastType)experience.Breast,
                            IsNew = isNew,
                            ExperienceId = experience.ExperienceId,
                            Duration = TimeSpan.FromSeconds(experience.Duration),
                            TransitionType = (TransitionType)experience.TransistionType,
                            TransitionTime = TimeSpan.FromSeconds(experience.TransistionTime),
                            CreatedAt = experience.CreatedAt
                        };

                        var existing = _allExperiences.Where(e => e.Id == experience.Id).FirstOrDefault();

                        if (existing == null)
                        {
                            _userExperiences.Add(experienceModel);
                            _allExperiences.Add(experienceModel);
                            experienceModel.Edit += model => { OnEditExperience(model); };

                            if (isNew)
                            {
                                _newExperiences.Add(experience.Id);
                            }
                        }
                        else
                        {
                            existing.Copy(experienceModel);
                        }

                        if (experienceModel.ExperienceId > _nextUserExperienceId)
                        {
                            _nextUserExperienceId = experienceModel.ExperienceId + 1;
                        }
                    }
                }

                if (_newExperiences.Count > 0)
                {
                    SyncKnownExperiences();
                }

                if (CurrentExperience == null)
                {
                    if (_presetExperiences.Count > 0)
                    {
                        CurrentExperience = _presetExperiences[0];
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Shutdown the ExperienceManager. This can also sync data, but for now
        /// does not since it is synched any time it changes.
        /// </summary>
        public void Shutdown()
        {

        }

        /// <summary>
        /// Create an experience and fill out the default values
        /// </summary>
        /// <returns>A default experience</returns>
        public ExperienceModel Create()
        {
            var em = new ExperienceModel();
            em.Id = Guid.NewGuid().ToString();
            em.CreatedBy = "me";
            em.TransitionType = TransitionType.Timed;
            em.TransitionTime = TimeSpan.FromMinutes(2);
            em.Duration = TimeSpan.FromMinutes(30);
            em.ExperienceId = _nextUserExperienceId;
            em.Edit += model => { OnEditExperience(model); };
            em.CreatedAt = DateTimeOffset.UtcNow;

            _nextUserExperienceId++;

            return em;
        }

        /// <summary>
        /// Called when the front end begins editing an experience
        /// </summary>
        /// <param name="exp">The experience being edited</param>
        private void OnEditExperience(ExperienceModel exp)
        {
            bool newGuid = _presetExperiences.Contains(exp);

            exp = exp.Clone(newGuid);
            exp.IsSelected = false;
            exp.CreatedBy = "me";
            exp.Edit += model => { OnEditExperience(model); };

            EditExperience?.Invoke(exp);
        }

        /// <summary>
        /// Save changes to a user or preset experience.
        /// </summary>
        /// <remarks>
        /// Note that this may change the UserExperiences and PresetExperiences list items. In general
        /// a new experience will be added to the UserExperiences list, and a preset experience will
        /// be moved to the UserExperiences list if it is edited.
        /// </remarks>
        /// <param name="experience"></param>
        public async void Save(ExperienceModel experienceModel)
        {
            var existingExperience = _userExperiences.FirstOrDefault(e => e.Id == experienceModel.Id);

            if (existingExperience == null)
            {
                // Add the new experience
                experienceModel.IsNew = true;
                _userExperiences.Add(experienceModel);
                _allExperiences.Add(experienceModel);
                ExperienceAddedEvent?.Invoke(this, new ExperienceAddedEventArgs() { Id = experienceModel.Id });
            }
            else
            {
                // Copy the changes in
                existingExperience.Copy(experienceModel);
                ExperienceChangedEvent?.Invoke(this, new ExperienceChangedEventArgs() { Id = experienceModel.Id });
            }

            // Sync with the database
            Experience experience = new Experience()
            {
                Id = experienceModel.Id,
                Name = experienceModel.Name,
                Description = experienceModel.Description,
                ExpressionSpeed = (byte)experienceModel.ExpressionSpeed,
                ExpressionSuction = (byte)experienceModel.ExpressionSuction,
                StimulationSpeed = (byte)experienceModel.StimulationSpeed,
                StimulationSuction = (byte)experienceModel.StimulationSuction,
                Breast = (byte)experienceModel.Breast,
                ExperienceId = (byte)experienceModel.ExperienceId,
                Duration = Convert.ToInt32(experienceModel.Duration.TotalSeconds),
                TransistionType = (byte)experienceModel.TransitionType,
                TransistionTime = Convert.ToInt32(experienceModel.TransitionTime.TotalSeconds),
                Storage = (byte)experienceModel.Storage,
                CreatedAt = experienceModel.CreatedAt
        };

            await DataManager.Instance.AddUserExperience(experience);

            EditingExperience = null;
        }

        /// <summary>
        /// Get an experience by the ID (GUID)
        /// </summary>
        /// <param name="id">The experience ID</param>
        /// <returns>The experience referred to by the ID or null if none</returns>
        public ExperienceModel Get(string id)
        {
            var existingExperience = _allExperiences.FirstOrDefault(e => e.Id == id);

            return existingExperience;
        }

        /// <summary>
        /// Get an experience by the device experience ID (integer)
        /// </summary>
        /// <param name="experienceId">The device ID</param>
        /// <returns>The experience referred to by the device experience ID or null if none</returns>
        public ExperienceModel GetFromExperienceId(int experienceId)
        {
            var existingExperience = _allExperiences.FirstOrDefault(e => e.ExperienceId == experienceId);

            return existingExperience;
        }

        /// <summary>
        /// Get the current experience in use
        /// </summary>
        /// <returns>The current experience or null if none</returns>
        public ExperienceModel CurrentExperience
        {
            get
            {
                return _currentExperience;
            }
            set
            {
                if (_currentExperience != value)
                {
                    if (_currentExperience != null)
                    {
                        _currentExperience.IsSelected = false;
                    }

                    _currentExperience = value;

                    if (_currentExperience != null)
                    {
                        _currentExperience.IsSelected = true;
                    }
                    CurrentExperienceChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Get the current experience being edited
        /// </summary>
        /// <returns>The current experience being edited or null if none</returns>
        public ExperienceModel EditingExperience
        {
            get; set;
        }

        /// <summary>
        /// Get a list of all user defined experiences
        /// </summary>
        /// <returns>A list of user defined experiences</returns>
        public ObservableCollection<ExperienceModel> UserExperiences
        {
            get
            {
                return _userExperiences;
            }
        }

        /// <summary>
        /// Get a list of the preset experiences
        /// </summary>
        /// <returns>A list of preset experiences</returns>
        public ObservableCollection<ExperienceModel> PresetExperiences
        {
            get
            {
                return _presetExperiences;
            }
        }

        /// <summary>
        /// Get a list of all user and preset experiences
        /// </summary>
        /// <returns>A list of all user and preset experiences</returns>
        public ObservableCollection<ExperienceModel> AllExperiences
        {
            get
            {
                return _allExperiences;
            }
        }

#if DEBUG
        public async Task DeleteExperience(ExperienceModel experienceModel)
        {
            if (null == experienceModel)
                return;

            // Sync with the database
            Experience experience = new Experience()
            {
                Id = experienceModel.Id,
                Name = experienceModel.Name,
                Description = experienceModel.Description,
                ExpressionSpeed = (byte)experienceModel.ExpressionSpeed,
                ExpressionSuction = (byte)experienceModel.ExpressionSuction,
                StimulationSpeed = (byte)experienceModel.StimulationSpeed,
                StimulationSuction = (byte)experienceModel.StimulationSuction,
                Breast = (byte)experienceModel.Breast,
                ExperienceId = (byte)experienceModel.ExperienceId,
                Duration = Convert.ToInt32(experienceModel.Duration.TotalSeconds),
                TransistionType = (byte)experienceModel.TransitionType,
                TransistionTime = Convert.ToInt32(experienceModel.TransitionTime.TotalSeconds),
                Storage = (byte)experienceModel.Storage,
                CreatedAt = experienceModel.CreatedAt
            };

            await DataManager.Instance.DeleteUserExperience(experience);

            await DataManager.Instance.SyncUserExperiences();

            await Sync();
        }
#endif
    }
}
