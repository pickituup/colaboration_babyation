using System;
using System.IO;
using Xamarin.Forms;
using BabyationApp.Helpers;
using BabyationApp.iOS.Helpers;

[assembly: Dependency(typeof(FileHelper))]
namespace BabyationApp.iOS.Helpers
{
    /// <summary>
    /// iOS Implementation of IFileHelper
    /// </summary>
    public class FileHelper : IFileHelper
    {
        /// <summary>
        /// Gets platform specific local file path
        /// </summary>
        /// <param name="filename">string file name</param>
        /// <returns>string local file path</returns>
        public string GetLocalFilePath(string filename)
        {
            string docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string libFolder = Path.Combine(docFolder, "..", "Library", "Databases");

            if (!Directory.Exists(libFolder))
            {
                Directory.CreateDirectory(libFolder);
            }

            return Path.Combine(libFolder, filename);
        }
    }
}