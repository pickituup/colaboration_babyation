using System;
using BabyationApp.Common;
using System.Windows.Input;

namespace BabyationApp.Models
{
    /// <summary>
    /// Class to represent ExperienceModel to the UI. Used on any Mode selection page.
    /// </summary>
    public class ModeItem : ObservableObject
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTimeOffset CreationDate { get; set; }

        public bool IsPredefined { get; set; }
        public bool IsNew { get; set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetPropertyChanged(ref _isSelected, value);
        }

        public ICommand SelectModeCommand { get; set; }
    }
}
