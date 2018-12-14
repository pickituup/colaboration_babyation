using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.IO;
using FFImageLoading;

namespace BabyationApp.Managers
{
    public enum MediaType
    {
        Image,
        Video
    }

    public class Media
    {
        public string Id { get; set; }
        public MediaType Type { get; set; }
        public ImageSource Image { get; set; }
    }

    class MediaManager
    {
        static private MediaManager _instance = null;

        private ObservableCollection<Media> _media = new ObservableCollection<Media>();
        private Media _currentPumpPicture = null;

        public Action<Media> CurrentPumpPictureChanged;

        public static MediaManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MediaManager();
                }
                return _instance;
            }
        }

        public async Task Initialize()
        {
            await Sync();
        }

        public async Task Sync()
        {
            DataManager dataManager = DataManager.Instance;
            IEnumerable<DataObjects.Media> allMedia;
            Media existing;

            allMedia = await dataManager.GetMedia();

            foreach (DataObjects.Media mediaIter in allMedia)
            {
                existing = _media.Where(m => m.Id == mediaIter.Id).FirstOrDefault();

                if (existing == null)
                {
                    Media media = new Media()
                    {
                        Id = mediaIter.Id,
                        Type = MediaType.Image,
                        Image = ImageSource.FromStream(() => new MemoryStream(mediaIter.Data))
                    };

                    _media.Add(media);

                    if (CurrentPumpPicture == null)
                    {
                        CurrentPumpPicture = media;
                    }
                }
            }
        }

        public Media CreateImageMedia(ImageSource image)
        {
            return new Media()
            {
                Id = Guid.NewGuid().ToString(),
                Type = MediaType.Image,
                Image = image
            };
        }

        public async Task Add(Media media)
        {
            DataManager dataManager = DataManager.Instance;
            MemoryStream stream = null;
            DataObjects.Media dataMedia = null;
            FileImageSource fileImageSource = null;

            fileImageSource = media.Image as FileImageSource;

			Stream downsampleStream = null;
            // If this is still from a file downsample it to be stored in the database
            if (fileImageSource != null)
            {
                downsampleStream = await ImageService.Instance.LoadFile(fileImageSource.File).DownSample(width: 300).AsJPGStreamAsync();
            }
            else
            {
                StreamImageSource streamsource = media.Image as StreamImageSource;

                if (streamsource != null)
                {
                    downsampleStream = await ImageService.Instance.LoadStream(
						sct => { 
						return ((IStreamImageSource)streamsource).GetStreamAsync(); 
					}).DownSample(width: 300).AsJPGStreamAsync();
                }
            }

			if (downsampleStream != null)
			{
				stream = new MemoryStream();
				await downsampleStream.CopyToAsync(stream);
			    if (Device.RuntimePlatform == Device.iOS)
			    {
			        // Lets use the downsample one rather the big-size image selected by user to lower the app memory usage
			        // (Actually we are using the downsample one from datamanger when the app starts)
			        stream.Seek(0, SeekOrigin.Begin);
			        media.Image = ImageSource.FromStream(() => stream);
			    }
			}

            if (stream != null)
            {
                // Add to the Data Manager
                dataMedia = new DataObjects.Media()
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = (byte)MediaType.Image,
                    Data = stream.ToArray()
                };

                await dataManager.AddMedia(dataMedia);

                media.Id = dataMedia.Id;
                _media.Add(media);
            }
        }

        public async Task Update(Media media)
        {
            DataManager dataManager = DataManager.Instance;
            MemoryStream stream = null;
            DataObjects.Media dataMedia = null;
            FileImageSource fileImageSource = null;

            fileImageSource = media.Image as FileImageSource;

            // If this is still from a file downsample it to be stored in the database
            if (fileImageSource != null)
            {
                stream = new MemoryStream();
                var output = await ImageService.Instance.LoadFile(fileImageSource.File).DownSample(width: 300).AsJPGStreamAsync();
                await output.CopyToAsync(stream);
            }
            else
            {
                StreamImageSource streamsource = media.Image as StreamImageSource;

                if (streamsource?.Stream != null)
                {
                    stream = (await ((IStreamImageSource)streamsource).GetStreamAsync()) as MemoryStream;
                }
            }

            if (stream != null)
            {
                // Add to the Data Manager
                dataMedia = new DataObjects.Media()
                {
                    Id = media.Id,
                    Type = (byte)media.Type,
                    Data = stream.ToArray()
                };

                await dataManager.AddMedia(dataMedia);
            }
        }

        public async Task Add(IEnumerable<Media> media)
        {
            foreach (Media mediaIter in media)
            {
                await Add(mediaIter);
            }
        }

        public async Task Remove(string id)
        {
            DataManager dataManager = DataManager.Instance;
            DataObjects.Media dataMedia;

            if (id != Guid.Empty.ToString())
            {
                // Remove media from Data Manager
                dataMedia = new DataObjects.Media()
                {
                    Id = id,
                };

                await dataManager.RemoveMedia(dataMedia);
            }
        }

        public async Task Remove(Media media)
        {
            _media.Remove(media);

            await Remove(media.Id);
        }

        public async Task Remove(IEnumerable<Media> media)
        {
            DataManager dataManager = DataManager.Instance;

            foreach (Media mediaIter in media)
            {
                await Remove(mediaIter);
            }
        }

        public Media Get(string id)
        {
            return _media.Where(m => m.Id == id).FirstOrDefault();
        }

        public Media CurrentPumpPicture
        {
            get
            {
                return _currentPumpPicture;
            }
            set
            {
                if (CurrentPumpPicture != value)
                {
                    _currentPumpPicture = value;
                    CurrentPumpPictureChanged?.Invoke(value);
                }
            }
        }

        public ObservableCollection<Media> Media
        {
            get
            {
                return _media;
            }
        }
    }
}
