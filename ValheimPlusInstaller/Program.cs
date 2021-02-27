using System;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;

namespace ValheimPlusInstaller
{
    internal class Program
    {
        public static Config Config { get; set; } = new();

        private static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("Loading config");
                await Config.LoadAsync();
                Console.WriteLine("Config loaded");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadKey();
                return;
            }

            if (Config.Platform.Equals("Windows"))
            {
                Console.WriteLine("Backing up save files");
                BackupSaveFile();
                Console.WriteLine("Save files backed up");
            }

            DownloadManager downloadManager = new();
            downloadManager.AddDownloadProgressChangedEventHandler(DownloadProgressChanged);
            downloadManager.AddDownloadFileCompletedEventHandler(DownloadFileCompleted);

            Console.WriteLine("Beginn downloading the latest release");
            downloadManager.DownloadFile(Config.DownloadUrl, Config.DownloadLocation);

            Console.ReadKey();
        }

        private static void BackupSaveFile()
        {
            if (File.Exists(Config.BackupFile)) {
                File.Delete(Config.BackupFile);
            }

            ZipFile.CreateFromDirectory(
                Config.SaveFileLocation, //From
                Config.BackupFile,       //To
                CompressionLevel.Optimal,
                false   // Include parent dir in zip?
                );
        }


        private static void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Console.WriteLine($"Progress: {e.ProgressPercentage}%");
        }

        private static void DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
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
                InstallMod();
            }
        }


        internal static void InstallMod()
        {
            Console.WriteLine($"Installing mod to {Config.GameLocation}");
            ZipFile.ExtractToDirectory(Config.DownloadLocation, Config.GameLocation, true);
            Console.WriteLine("Mod installed");
        }
    }
}