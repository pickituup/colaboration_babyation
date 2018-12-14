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
        //private List<PeopleModel> _peoples = new List<PeopleModel>();
        public ProfileModel()
        {
            ShowBabyDeleteAlert = false;
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
    }
}
