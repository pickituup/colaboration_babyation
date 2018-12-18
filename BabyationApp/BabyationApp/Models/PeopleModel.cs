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
        }

        public String Email { get; set; }
    }
}
