using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace BabyationApp.Models
{
    /// <summary>
    /// Transition type enumeration
    /// </summary>
    public enum TransitionType
    {
        Letdown,
        Timed
    }

    public delegate void ExperienceEditEvent(ExperienceModel model);

    /// <summary>
    /// The Experience Model
    /// </summary>
    public class ExperienceModel : ModelItemBase
    {
        private string _id;
        private string _name;
        private string _createdBy = "me";
        private string _description;
        private bool _isNew = false;
        private TransitionType _transitionType = TransitionType.Letdown;
        private int _stimulationSpeed = -1;
        private int _stimulationSuction = -1;
        private int _expressionSpeed = -1;
        private int _expressionSuction = -1;
        private TimeSpan _duration;
        private TimeSpan _transitionTime;
        private DateTime _lastUsed = DateTime.UtcNow;
        private BreastType _breast = BreastType.Unspecified;
        private StorageType _storage = StorageType.Unspecified;
        private int _experienceId = 0;
        private DateTimeOffset _createdAt;

        /// <summary>
        /// Event which is raised whenever the experience is being edited
        /// </summary>
        public event ExperienceEditEvent Edit;

        /// <summary>
        /// Experience Model constructor
        /// </summary>
        public ExperienceModel()
        {
            EditCommand = new Command(() => Edit?.Invoke(this));

        }

        /// <summary>
        /// Clone an experience
        /// </summary>
        /// <param name="newGuid">Whether the cloned event should have a new GUID or retain the old one</param>
        /// <returns>The cloned experience</returns>
        public ExperienceModel Clone(bool newGuid)
        {
            ExperienceModel clonedExperienceModel = this.MemberwiseClone() as ExperienceModel;

            if (newGuid)
            {
                clonedExperienceModel.Id = Guid.NewGuid().ToString();
            }

            return clonedExperienceModel;
        }

        /// <summary>
        /// Copy the experience model passed in to this experience model
        /// </summary>
        /// <param name="from">The experience to copy from</param>
        public void Copy(ExperienceModel from)
        {
            Name = from.Name;
            Description = from.Description;
            StimulationSpeed = from.StimulationSpeed;
            StimulationSuction = from.StimulationSuction;
            ExpressionSpeed = from.ExpressionSpeed;
            ExpressionSuction = from.ExpressionSuction;
            Breast = from.Breast;
            Storage = from.Storage;
            ExperienceId = from.ExperienceId;
        }

        /// <summary>
        /// Get the edit command
        /// </summary>
        public ICommand EditCommand { get; set; }

        /// <summary>
        /// Get and set the experience ID
        /// </summary>
        public string Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        /// <summary>
        /// Gets or sets the created at.
        /// </summary>
        /// <value>The model creation time.</value>
        public DateTimeOffset CreatedAt
        {
            get => _createdAt;
            set => SetPropertyChanged(ref _createdAt, value);
        }

        /// <summary>
        /// Get and set the experience name
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                SetPropertyChanged(ref _name, value);
            }
        }

        /// <summary>
        /// Get and set who the experience was created by
        /// </summary>
        public string CreatedBy
        {
            get
            {
                return _createdBy;
            }

            set
            {
                SetPropertyChanged(ref _createdBy, value);
            }
        }

        /// <summary>
        /// Get and set who the experience description
        /// </summary>
        public string Description
        {
            get
            {
                return _description;
            }

            set
            {
                SetPropertyChanged(ref _description, value);
            }
        }

        /// <summary>
        /// Get and set if this is a new experience or an existing one
        /// </summary>
        public bool IsNew
        {
            get
            {
                return _isNew;
            }

            set
            {
                SetPropertyChanged(ref _isNew, value);
            }
        }

        /// <summary>
        /// Get and set the transition type
        /// </summary>
        /// <seealso cref="TransitionType"/>
        public TransitionType TransitionType
        {
            get
            {
                return _transitionType;
            }

            set
            {
                SetPropertyChanged(ref _transitionType, value);
            }
        }

        /// <summary>
        /// Get and set the stimulation speed for this experience
        /// </summary>
        public int StimulationSpeed
        {
            get
            {
                return _stimulationSpeed;
            }

            set
            {
                SetPropertyChanged(ref _stimulationSpeed, value);
            }
        }

        /// <summary>
        /// Get and set the stimulation suction for this experience
        /// </summary>
        public int StimulationSuction
        {
            get
            {
                return _stimulationSuction;
            }

            set
            {
                SetPropertyChanged(ref _stimulationSuction, value);
            }
        }

        /// <summary>
        /// Get and set the expression speed for this experience
        /// </summary>
        public int ExpressionSpeed
        {
            get
            {
                return _expressionSpeed;
            }

            set
            {
                SetPropertyChanged(ref _expressionSpeed, value);
            }
        }

        /// <summary>
        /// Get and set the expression suction for this experience
        /// </summary>
        public int ExpressionSuction
        {
            get
            {
                return _expressionSuction;
            }

            set
            {
                SetPropertyChanged(ref _expressionSuction, value);
            }
        }

        /// <summary>
        /// Get and set the duration for this experience
        /// </summary>
        public TimeSpan Duration
        {
            get
            {
                return _duration;
            }

            set
            {
                SetPropertyChanged(ref _duration, value);
            }
        }

        /// <summary>
        /// Get and set the transtion time for this experience
        /// </summary>
        public TimeSpan TransitionTime
        {
            get
            {
                return _transitionTime;
            }

            set
            {
                SetPropertyChanged(ref _transitionTime, value);
            }
        }

        /// <summary>
        /// Get and set which breasts this experience is for
        /// </summary>
        /// <seealso cref="BreastType"/>
        public BreastType Breast
        {
            get
            {
                return _breast;
            }

            set
            {
                SetPropertyChanged(ref _breast, value);
            }
        }

        /// <summary>
        /// Get and set the storage type for this experience
        /// </summary>
        /// <seealso cref="StorageType"/>
        public StorageType Storage
        {
            get
            {
                return _storage;
            }

            set
            {
                SetPropertyChanged(ref _storage, value);
            }
        }

        /// <summary>
        /// Get and set the experience ID for this experience
        /// </summary>
        public int ExperienceId
        {
            get
            {
                return _experienceId;
            }

            set
            {
                SetPropertyChanged(ref _experienceId, value);
            }
        }
    }
}
