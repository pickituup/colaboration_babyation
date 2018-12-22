using System;
using BabyationApp.Models;
using Xamarin.Forms;
using System.Windows.Input;
using BabyationApp.Controls.Views;
using BabyationApp.Managers;
using BabyationApp.Pages.FirstTimeUser;
using BabyationApp.Pages.Settings;
using BabyationApp.Extensions;
using BabyationApp.Interfaces;
using System.Threading.Tasks;
using System.Linq;
using BabyationApp.Helpers;

namespace BabyationApp.ViewModels
{
    public class BabyAdditionViewModel : BaseViewModel
    {
        private bool _babyInProgress;
        private bool _isProfilePageSession = default(bool);
        private readonly ProfileManager _currentManager;

        public BabyAdditionViewModel(ProfileManager profileManager, bool skippable = false, bool isProfilePage = false)
        {
            _currentManager = profileManager;
            IsSkipVisible = skippable;
            _isProfilePageSession = isProfilePage;

            CurrentBaby = _currentManager.CreateBaby();
        }

        public override void OnAppearing()
        {
            base.OnAppearing();

            // Cleanup
            if (!_babyInProgress)
            {
                if (CurrentBaby == null)
                {
                    CurrentBaby = _currentManager.CreateBaby();
                }

                Name = null;
                BirthdayDate = DateTime.MinValue;

                _babyInProgress = true;
            }
        }

        #region Public UI properties

        //public ImageSource Photo
        //{
        //    get => CurrentBaby.Picture ?? null;
        //    set
        //    {
        //        CurrentBaby.Picture = value;
        //        OnPropertyChanged(nameof(Photo));
        //    }
        //}
        public bool IsProfileSession => _isProfilePageSession;

        public string Name
        {
            get => CurrentBaby?.Name ?? String.Empty;
            set
            {
                if (null != CurrentBaby)
                {
                    CurrentBaby.Name = value;
                }
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(IsSaveReady));
            }
        }

        DateTime _birthdayDate;
        public DateTime BirthdayDate
        {
            get => _birthdayDate;
            set
            {
                _birthdayDate = value;
                if (null != CurrentBaby)
                {
                    CurrentBaby.Birthday = value;
                }

                OnPropertyChanged(nameof(BirthdayDate));
                OnPropertyChanged(nameof(BirthdayText));
                OnPropertyChanged(nameof(IsSaveReady));
            }
        }

        public string BirthdayText
        {
            get => IsBirthdayReady ? BirthdayDate.ToString("MM/dd/yyyy") : "__/__/____";
        }

        #endregion


        #region Data properties

        public ImageSource BabyPhoto
        {
            get
            {
                return CurrentBaby?.Picture ?? null;
            }
        }

        private BabyModel _currentBaby;
        public BabyModel CurrentBaby
        {
            get
            {
                return _currentBaby;
            }
            set
            {
                _currentBaby = value;

                OnPropertyChanged(nameof(CurrentBaby));
                OnPropertyChanged(nameof(BabyPhoto));
            }
        }

        public bool IsSkipVisible { get; }
        public bool IsSaveReady => IsNameReady && IsBirthdayReady;
        public bool IsNameReady => !String.IsNullOrEmpty(Name);
        public bool IsBirthdayReady { get; set; } = false;

        public DateTime MinimumDate => DateTime.Now.FirstDayOfYear().Subtract(TimeSpan.FromDays(1 + (5 * 365))); // 5 years
        public DateTime MaximumDate => DateTime.Today;

        #endregion


        #region Commands

        private ICommand _selectPhoto;
        public ICommand SelectPhoto
        {
            get
            {
                _selectPhoto = _selectPhoto ?? new Command(async (obj) =>
                {
                    await TakeImageAsync();
                });
                return _selectPhoto;
            }
        }

        private ICommand _pickerFocusCommand;
        public ICommand PickerFocusCommand
        {
            get
            {
                _pickerFocusCommand = _pickerFocusCommand ?? new Command((obj) =>
                {
                    if (!obj.GetType().Equals(typeof(DatePickerEx)))
                    {
                        return;
                    }
                    IsBirthdayReady = true;
                    ((DatePickerEx)obj).Focus();
                });
                return _pickerFocusCommand;
            }
        }

        private ICommand _skipCommand;
        public ICommand SkipCommand
        {
            get
            {
                _skipCommand = _skipCommand ?? new Command(Skip);
                return _skipCommand;
            }
        }

        private ICommand _saveCommand;
        public ICommand SaveCommand
        {
            get
            {
                _saveCommand = _saveCommand ?? new Command(Save);
                return _saveCommand;
            }
        }

        #endregion

        #region Private

        private async Task TakeImageAsync()
        {
            var photoResult = await PictureManager.Instance.SelectFromGalleryAsync();

            if (photoResult != null)
            {
                if (photoResult.Pictures.Count > 0)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        CurrentBaby.Picture = photoResult.Pictures.First();

                        OnPropertyChanged(nameof(CurrentBaby));
                        OnPropertyChanged(nameof(BabyPhoto));
                    });
                }
            }
        }

        private void Skip()
        {
            PageManager.Me.SetCurrentPage(typeof(PumpAdditionPage));
        }

        private void Save()
        {
            _currentManager.AddBaby(CurrentBaby);
            CurrentBaby = null;
            IsBirthdayReady = false;
            _babyInProgress = false;

            if (_isProfilePageSession)
            {
                PageManager.Me.SetCurrentPage(typeof(ProfilePage));
            }
            else
            {
                PageManager.Me.SetCurrentPage(typeof(AddAnotherChildPage));
            }
        }

        #endregion
    }
}
