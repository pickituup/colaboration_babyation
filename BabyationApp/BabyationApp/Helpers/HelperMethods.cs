using System;
using System.IO;
using FFImageLoading.Helpers;
using Xamarin.Forms;

namespace BabyationApp.Helpers
{
    /// <summary>
    /// This class is the Logger to log to
    /// </summary>
    public class CustomMiniLogger : IMiniLogger
    {
        /// <summary>
        /// Logs a DEBUG type message
        /// </summary>
        /// <param name="message"></param>
        public void Debug(string message)
        {
            System.Diagnostics.Debug.WriteLine("FFIL Debug : " + message);
        }

        /// <summary>
        /// Logs a ERROR type message
        /// </summary>
        /// <param name="errorMessage"></param>
        public void Error(string errorMessage)
        {
            System.Diagnostics.Debug.WriteLine("FFIL Error1 : " + errorMessage);
        }

        /// <summary>
        /// Logs an exception
        /// </summary>
        /// <param name="errorMessage">Error message to log to</param>
        /// <param name="ex">The exception to log to</param>
        public void Error(string errorMessage, Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("FFIL Error2 : " + errorMessage);
            System.Diagnostics.Debug.WriteLine("FFIL Error2.1 : " + ex.Message);
        }
    }

    /// <summary>
    /// This static class is to have the application wise helper methods
    /// </summary>
    public static class HelperMethods
    {
        /// <summary>
        /// Clone an StreamImageSource
        /// </summary>
        /// <param name="source">The ImageSourcce to be cloned to</param>
        /// <returns>The cloned ImageSource</returns>
        public static ImageSource CloneStreamImageSource(ImageSource source)
        {
            try
            {
                var streamImageSource = source as StreamImageSource;
                if (streamImageSource != null)
                {
                    var oldStream = streamImageSource.Stream(System.Threading.CancellationToken.None).Result as MemoryStream;
                    MemoryStream memorySteam = new MemoryStream(oldStream.ToArray());
                    memorySteam.Seek(0, SeekOrigin.Begin);
                    return ImageSource.FromStream(() => memorySteam);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception in CloneStreamImageSource - " + e.Message);
            }
            return source;
        }
    }
    
}
