using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace BabyationApp.Models
{
    public class PeopleModel : ModelItemBase
    {
        private ProfileModel _profileModel;
        public PeopleModel(ProfileModel profile)
        {
            _profileModel = profile;
            DeletePeople = new Command(() => OnDeletePeopleCommand());
        }
        public String Name { get; set; }

        public ICommand DeletePeople { get; set; }

        private void OnDeletePeopleCommand()
        {
        }
    }
}
