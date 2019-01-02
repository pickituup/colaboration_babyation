using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace BabyationApp.Models
{
    public class ProfileModel : ModelItemBase
    {
        private ObservableCollection<BabyModel> _babies = new ObservableCollection<BabyModel>();
        private BabyModel _currentBaby;
        private string _name = "";
        private List<PeopleModel> _peoples = new List<PeopleModel>();
        public ProfileModel()
        {
            ShowBabyDeleteAlert = false;

            // Demo:
            _peoples.Add(new PeopleModel(this) { Email = "abc@.happy.com" });
            _peoples.Add(new PeopleModel(this) { Email = "def@.unhappy.com" });
            _peoples.Add(new PeopleModel(this) { Email = "ghj@.sloppy.com" });
            _peoples.Add(new PeopleModel(this) { Email = "klm@.dry.com" });

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

        public bool CaregiverAccountSelected { get; set; }

        private PeopleModel _currentCaregiver;
        public PeopleModel CurrentCaregiver
        {
            get => _currentCaregiver;
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

        public List<PeopleModel> Caregivers
        {
            get => _peoples;
        }

        public bool AddCaregiver(PeopleModel caregiver)
        {
            if( _peoples.Contains(caregiver) )
            {
                _errorMessage = "Caregiver already exists";
                return false;
            }

            _peoples.Add(caregiver);

            return true;
        }

        public bool DeleteCaregiver(PeopleModel caregiver)
        {
            bool result = false;
            if(_peoples.Contains(caregiver))
            {
                result = _peoples.Remove(caregiver);
            }

            SetPropertyChanged(nameof(Caregivers));

            return result;
        }
    }
}
