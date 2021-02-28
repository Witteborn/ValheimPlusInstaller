using System;
using System.ComponentModel;
using System.IO.Compression;
using System.Net;

namespace ValheimPlusInstaller
{
    public class ModUpdater : DownloadManager
    {
        public ModUpdater(Config config)
            : base(config) { }
        protected override void OnDownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            // display completion status.
            bool errorOccured = e.Error != null;
            if (errorOccured)
            {
                Console.WriteLine(e.Error.Message);
                Console.WriteLine(e.Error.StackTrace);
            }
            else
            {
                Console.WriteLine("\nDownload Completed");
                Install();
            }
        }

        protected override void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Console.WriteLine($"Progress: {e.ProgressPercentage}%");
        }

        internal void Install()
        {
            Console.WriteLine($"Installing mod to {Config.GameLocation}");
            ZipFile.ExtractToDirectory(Config.DownloadLocationMod, Config.GameLocation, true);
            Console.WriteLine("Mod installed");
        }
    }
}
