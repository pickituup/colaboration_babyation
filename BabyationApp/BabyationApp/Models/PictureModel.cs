using BabyationApp.Common;
using BabyationApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using BabyationApp.Helpers;

namespace BabyationApp.Models
{
    public class PictureModel : ModelItemBase
    {


        public PictureModel()
        {
            IsPictureChangedByUser = false;

            SelectPicture = new Command(async () => await TakePictureAsync());
        }


        private ImageSource _picture;
        public ImageSource Picture
        {
            get
            {
                return this._picture;
            }
            set => SetPropertyChanged(ref _picture, value);
        }


        public ICommand SelectPicture { get; set; }

        public bool IsPictureChangedByUser { get; private set; }

        private async Task TakePictureAsync()
        {
            var photoResult = await PictureManager.Instance.TakePictureAsync();

            if (photoResult != null)
            {
                IsPictureChangedByUser = true;
                if (photoResult.Pictures.Count > 0)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Picture = photoResult.Pictures.First();
                    });
                }
                IsPictureChangedByUser = false;
            }
        }
    }
}
