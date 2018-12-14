using System;
using System.IO;
using Xamarin.Forms;
using BabyationApp.Helpers;
using BabyationApp.Droid.Helpers;

[assembly: Dependency(typeof(FileHelper))]
namespace BabyationApp.Droid.Helpers
{
    /// <summary>
    /// Android Implementation of IFileHelper
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
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            return Path.Combine(path, filename);
        }
    }
}