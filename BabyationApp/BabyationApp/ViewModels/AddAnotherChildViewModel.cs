using System;
using System.Windows.Input;
using BabyationApp.Pages.Settings;
using Xamarin.Forms;
using BabyationApp.Pages.FirstTimeUser;
namespace BabyationApp.ViewModels
{
    public class AddAnotherChildViewModel
    {
        public AddAnotherChildViewModel()
        {
        }

        #region Commands

        private ICommand _noCommand;
        public ICommand NOCommand
        {
            get
            {
                _noCommand = _noCommand ?? new Command(() =>
                {
                    PageManager.Me.SetCurrentPage(typeof(ProfilePage));
                });

                return _noCommand;
            }
        }

        private ICommand _yesCommand;
        public ICommand YESCommand
        {
            get
            {
                _yesCommand = _yesCommand ?? new Command(() =>
                {
                    PageManager.Me.SetCurrentPage(typeof(BabyAdditionPage));
                });

                return _yesCommand;
            }
        }

        #endregion
    }
}
