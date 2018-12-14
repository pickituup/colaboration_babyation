using System;
using System.Threading.Tasks;
using BabyationApp.Interfaces;
using BabyationApp.Pages;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Xamarin.Forms;
using BabyationApp.Resources;
using Plugin.Media;
using System.Diagnostics;
using System.Linq;
using Plugin.Media.Abstractions;

namespace BabyationApp.Helpers
{
    public class PictureManager : IPictureProvider
    {
        static private PictureManager _instance = null;
        static public PictureManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PictureManager();
                }

                return _instance;
            }
        }

        public PictureManager()
        {
        }

        #region Interface

        public async Task<ImageResult> TakePictureAsync()
        {
            bool result = await CheckPermissions();

            if (result && await CheckHardware())
            {
                var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    PhotoSize = PhotoSize.Small,
                    CompressionQuality = 50,
                    DefaultCamera = CameraDevice.Rear
                });

                if (file == null)
                    return null;

                Debug.WriteLine($"File Location: {file.Path}");

                ImageSource imageSource = ImageSource.FromStream(() =>
                {
                    var stream = file.GetStream();
                    return stream;
                });

                if (null != imageSource)
                {
                    ImageResult res = new ImageResult();
                    res.Paths.Append(file.Path);
                    res.Pictures.Add(imageSource);
                    return res;
                }
            }
            return null;
        }

        public async Task<ImageResult> SelectFromGalleryAsync(bool allowMultiple = false)
        {
            bool result = await CheckPermissions();

            if(result && await CheckSoftware())
            {
                var file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
                {
                    PhotoSize = PhotoSize.Medium
                });

                if (file == null)
                    return null;

                Debug.WriteLine($"File Location: {file.Path}");

                ImageSource imageSource = ImageSource.FromStream(() =>
                {
                    var stream = file.GetStream();
                    return stream;
                });

                if (null != imageSource)
                {
                    ImageResult res = new ImageResult();
                    res.Paths.Append(file.Path);
                    res.Pictures.Add(imageSource);
                    return res;
                }
            }
            return null;
        }

        #endregion

        #region Private

        private async Task<bool> CheckPermissions()
        {
            var cameraStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
            var mediaStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.MediaLibrary);
            var storageStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);
            var photoStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Photos);

            if (cameraStatus != PermissionStatus.Granted || storageStatus != PermissionStatus.Granted || mediaStatus != PermissionStatus.Granted || photoStatus != PermissionStatus.Granted)
            {
                var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Camera, Permission.Storage, Permission.MediaLibrary, Permission.Photos });
                cameraStatus = results[Permission.Camera];
                storageStatus = results[Permission.Storage];
                mediaStatus = results[Permission.MediaLibrary];
                photoStatus = results[Permission.Photos];
            }

            if(cameraStatus == PermissionStatus.Granted && storageStatus == PermissionStatus.Granted && mediaStatus == PermissionStatus.Granted && photoStatus == PermissionStatus.Granted)
            {
                return true;
            }
            else
            {
                ModalAlertPage.ShowAlertWithClose(AppResource.CameraPermissionsDenied);

                return false;
            }
               

            //await DisplayAlert("Permissions Denied", "Unable to take photos.", "OK");
            //On iOS you may want to send your user to the settings screen.
            //CrossPermissions.Current.OpenAppSettings();
        }

        private async Task<bool> CheckHardware()
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                ModalAlertPage.ShowAlertWithClose(AppResource.NoCamera);

                return false;
            }
            return true;
        }

        private async Task<bool> CheckSoftware()
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsPickPhotoSupported || !CrossMedia.Current.IsPickVideoSupported)
            {
                ModalAlertPage.ShowAlertWithClose(AppResource.PhotoPermissionsDenied);

                return false;
            }
            return true;
        }

        #endregion
    }
}
