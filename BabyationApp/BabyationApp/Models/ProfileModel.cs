using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using BabyationApp.Managers;

namespace BabyationApp.Models
{
    public class ProfileModel : ModelItemBase
    {
        private ObservableCollection<BabyModel> _babies = new ObservableCollection<BabyModel>();
        private BabyModel _currentBaby;
        private string _name = "";
        private ObservableCollection<CaregiverModel> _caregivers = new ObservableCollection<CaregiverModel>();
        public ProfileModel()
        {
            ShowBabyDeleteAlert = false;

            CaregiverAccountSelected = false;
        }

        private bool _showBabyDeleteAlert = false;

        public bool ShowBabyDeleteAlert
        {
            get => _showBabyDeleteAlert;
            set => SetPropertyChanged(ref _showBabyDeleteAlert, value);
        }

        public String Name
        {
            get => _name;
            set => SetPropertyChanged(ref _name, value);
        }

        public String Email { get; set; }

        public bool HasBabies => 0 < (Babies?.Count ?? 0);

        public ObservableCollection<BabyModel> Babies
        {
            get { return _babies; }
        }

        public BabyModel CurrentBaby
        {
            get
            {
                return _currentBaby;
            }
            set
            {
                if (_currentBaby != value)
                {
                    if (_currentBaby != null)
                    {
                        _currentBaby.IsSelected = false;
                    }

                    _currentBaby = value;

                    if (_currentBaby != null)
                    {
                        _currentBaby.IsSelected = true;
                    }

                    SetPropertyChanged(nameof(CurrentBaby));
                }
            }
        }

        private bool _caregiverAccountSelected;
        public bool CaregiverAccountSelected 
        { 
            get => _caregiverAccountSelected;
            set => SetPropertyChanged(ref _caregiverAccountSelected, value);
        }

        private CaregiverModel _currentCaregiver;
        public CaregiverModel CurrentCaregiver
        {
            get
            {
                if( null == _currentCaregiver )
                {
                    _currentCaregiver = _caregivers.FirstOrDefault(c => c.CaregiverId == ProfileId);
                }
                return _currentCaregiver;
            }
            set
            {
                SetPropertyChanged(ref _currentCaregiver, value);
                if (null == value)
                {
                    CaregiverAccountSelected = false;
                }
            }
        }

        private String _errorMessage;
        public String ErrorMessage => _errorMessage;

        public ObservableCollection<CaregiverModel> Caregivers
        {
            get => _caregivers;
        }

        public string ProfileId { get; set; }
    }
}
