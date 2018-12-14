using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BabyationApp.Interfaces
{
    /// <summary>
    /// This class is used as result to fetch/find/takes picture in planform depended code
    /// </summary>
    public class ImageResult
    {
        public ImageResult()
        {
            Paths = new List<string>();
            Pictures = new List<ImageSource>();
        }
        public List<String> Paths { get; set; }
        public List<ImageSource> Pictures { get; set; }
    }

    /// <summary>
    /// The interface to take/select picture. 
    /// </summary>
    /// <remarks>
    /// Classes are inheriting this interface in the platform dependent project and register in the dependency service
    /// </remarks>
    public interface IPictureProvider
    {
        Task<ImageResult> TakePictureAsync();
        Task<ImageResult> SelectFromGalleryAsync(bool allowMultiple = false);
    }

    /// <summary>
    /// The interface to maintain image chaces on platforms
    /// </summary>
    /// <remarks>
    /// Classes are inheriting this interface in the platform dependent project and register in the dependency service
    /// </remarks>
    public interface IPictureCache
    {
        bool Contains(String file);
        void CacheFromFile(String file);
        void CacheFromFileAync(String file);
    }
}
