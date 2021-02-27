using Octokit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
namespace ValheimPlusInstaller
{
    internal class Config
    {
        public string DownloadUrl { get; private set; }
        public string DownloadDirectory { get; private set; }
        public string DownloadLocation { get; private set; }
        public string SaveFileLocation { get; private set; }
        public string BackupFile { get; private set; }
        public string GameLocation { get; private set; }
        public string Platform { get; private set; }

        public async Task LoadAsync() {

            Platform = GetPlatform();
            string configFile = CreateConfig();
            string appsettingsLocalLow = GetAppsettingsLocalLow();
            string ironGateFolder = Path.Combine(appsettingsLocalLow, "IronGate");

            GameLocation = GetGamePath(configFile);
            SaveFileLocation = Path.Combine(ironGateFolder, "Valheim");
            BackupFile = Path.Combine(ironGateFolder, "Valheim.zip");
            DownloadDirectory = GetDownloadDirectory();


            string filename = CreateFilename(configFile);

            DownloadUrl = await GetDownloadUrlAsync(configFile)+filename;
            DownloadLocation = Path.Combine(DownloadDirectory, filename);
        }

        private string CreateFilename(string path)
        {
            string target = GetIsServer(path) ? "Server": "Client";

            string ending = ".zip";

            return Platform + target + ending;

        }

        private static string GetPlatform()
        {
            string platform = string.Empty;

            bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            bool isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
            bool isOSX = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
            bool isUnix = isLinux || isOSX;

            if (isWindows)
            {
                platform = "Windows";
            }
            else if (isUnix)
            {
                platform = "Unix";
            }
            else {
                throw new Exception("Platform not supported");
            }

            return platform;
        }

        private static bool GetIsServer(string path)
        {
            try
            {
                string line = File.ReadAllLines(path)[1];

                return line.ToLower().Equals("server");
            }
            catch
            {
                return false;
            }
        }

        private static string GetDownloadDirectory()
        {
            return Path.Combine(
                         Directory.GetParent(
                             Environment.GetFolderPath(Environment.SpecialFolder.InternetCache)
                             ).ToString()
                         );
        }

        private static async Task<string> GetDownloadUrlAsync(string path)
        {
            GitHubClient client = new GitHubClient(new ProductHeaderValue("ValheimPlus"));
            IReadOnlyList<Release> releases = await client.Repository.Release.GetAll("valheimPlus", "ValheimPlus");
            Release latest = releases[0];

            string link = $"https://github.com/valheimPlus/ValheimPlus/releases/download/{latest.TagName}/";

            return link;
        }

        private static string GetGamePath(string path)
        {
            string gamepath = string.Empty;
            string[] lines = File.ReadAllLines(path);


            bool fileDoesntExist = File.Exists(path) == false;
            bool fileIsEmpty = lines.Length == 0;

            if (fileDoesntExist|| fileIsEmpty || string.IsNullOrWhiteSpace(lines[0]))
            {
                string input = string.Empty;
                while (Directory.Exists(input) == false)
                {
                    Console.WriteLine("Please insert your game path:");
                    input = Console.ReadLine();

                }
                gamepath = input;
                File.WriteAllText(path, input);
            }
            else
            {
                gamepath = lines[0];
            }

            return gamepath;
        }

        private static string GetAppsettingsLocalLow()
        {
            return Path.Combine(
                        Directory.GetParent(
                            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                            ).ToString()
                        , "LocalLow"
                        );
        }

        private static string CreateConfig()
        {

            string path = "Config.txt";
            if (File.Exists(path) == false)
            {
                File.CreateText(path);
            }

            return path;
        }


    }
}
