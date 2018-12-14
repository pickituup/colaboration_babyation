using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabyationApp.Helpers
{
    /// <summary>
    /// Helper Function Interface for platform specific local file path
    /// </summary>
    public interface IFileHelper
    {
        /// <summary>
        /// Gets platform specific local file path
        /// </summary>
        /// <param name="filename">string file name</param>
        /// <returns>string local file path</returns>
        string GetLocalFilePath(string filename);
    }
}
