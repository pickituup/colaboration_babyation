using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace BabyationApp.Models
{
    public class BabyModel : PictureModel
    {
        private ImageSource _image = null;
        public BabyModel()
        {
            IsDeleteRequested = false;
            IsSelected = false;
            //Picture = "icon_baby.png";
            DeleteCommand = new Command(() => OnDeleteBabyCommand());
            SelectCommand = new Command(() => OnSelectBabyCommand());
            MediaId = Guid.Empty.ToString();
        }
        public String Id { get; set; }
        public string MediaId { get; set; }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetPropertyChanged(ref _name, value);
        }

        private DateTime _birthday;
        public DateTime Birthday
        {
            get => _birthday;
            set => SetPropertyChanged(ref _birthday, value);
        }


        public ImageSource Image
        {
            get => _image;
            set => SetPropertyChanged(ref _image, value);
        }

        public ICommand DeleteCommand { get; set; }

        private void OnDeleteBabyCommand()
        {
            IsDeleteRequested = true;
        }

        public bool _isDeleteRequested = false;
        public bool IsDeleteRequested
        {
            get => _isDeleteRequested;
            set => SetPropertyChanged(ref _isDeleteRequested, value);
        }


        public ICommand SelectCommand { get; set; }

        private void OnSelectBabyCommand()
        {
            IsSelected = true;
        }
    }
}
