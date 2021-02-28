using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
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

            await UpdateIfNeededAsync();

            if (Config.Platform.Equals("Windows"))
            {
                Console.WriteLine("Backing up save files");
                BackupSaveFile();
                Console.WriteLine("Save files backed up");
            }

            ModUpdater modUpdater = new ModUpdater(Config);

            await modUpdater.DownloadFileAsync(Config.ReleaseURLMod, Config.DownloadLocationMod);

            Console.ReadKey();
        }

        private static async Task UpdateIfNeededAsync()
        {
            SelfUpdater selfUpdater = new SelfUpdater(Config);
            if (selfUpdater.IsUpdateAvailable())
            {
                Console.WriteLine("A newer version of this program (not the mod) is available.");
                Console.Write("Would you like to update to the latest release? (y/n):");
                char answer = Console.ReadLine()[0];
                Console.WriteLine();

                if (answer.Equals('y'))
                {
                    Console.WriteLine("Updating...");
                    await selfUpdater.DownloadFileAsync(Config.ReleaseURLInstaller, Config.DownloadLocationInstaller);

                    Console.WriteLine("\nDownload Completed");
                    Console.WriteLine("Please update the rest manually");

                    var downloadFolder = Directory.GetParent(
                            Config.DownloadLocationInstaller
                            ).ToString();


                    Process.Start(new ProcessStartInfo()
                    {
                        FileName = downloadFolder,
                        UseShellExecute = true,
                        Verb = "open"
                    });

                    Environment.Exit(0);
                }
                else
                {
                    Console.WriteLine("Keeping current version");
                }
            }
        }

        private static void BackupSaveFile()
        {
            if (File.Exists(Config.BackupFile))
            {
                File.Delete(Config.BackupFile);
            }

            ZipFile.CreateFromDirectory(
                Config.SaveFileLocation, //From
                Config.BackupFile,       //To
                CompressionLevel.Optimal,
                false   // Include parent dir in zip?
                );
        }
    }
}