using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabyationApp.Models
{
    public enum Frequency
    {
        OneTime,
        Daily,
        Weekly
    }

    public class ReminderModel : ModelItemBase
    {
        private Guid _id;
        private string _name;
        private SessionType _sessionType;
        private Guid _soundId;
        private bool _isAutoStart;
        private Guid _experienceId;
        private Frequency _frequency;
        private TimeSpan? _timeOffset;
        private DateTime? _time;

        public Guid Id
        {
            get => _id;
            set => _id = value;
        }

        public string Name
        {
            get => _name;
            set => SetPropertyChanged(ref _name, value);
        }

        public SessionType SessionType
        {
            get => _sessionType;
            set => SetPropertyChanged(ref _sessionType, value);
        }

        public Guid SoundId
        {
            get => _soundId;
            set => SetPropertyChanged(ref _soundId, value);
        }

        public bool IsAutoStart
        {
            get => _isAutoStart;
            set => SetPropertyChanged(ref _isAutoStart, value);
        }

        public Guid ExperienceId
        {
            get => _experienceId;
            set => SetPropertyChanged(ref _experienceId, value);
        }

        public Frequency Frequency
        {
            get => _frequency;
            set => SetPropertyChanged(ref _frequency, value);
        }

        public TimeSpan? TimeOffset
        {
            get => _timeOffset;
            set => SetPropertyChanged(ref _timeOffset, value);
        }

        public DateTime? Time
        {
            get => _time;
            set => SetPropertyChanged(ref _time, value);
        }
    }
}
