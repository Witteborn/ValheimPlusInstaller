using Octokit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
namespace ValheimPlusInstaller
{
    public class Config
    {
        public string ReleaseURLMod { get; private set; }
        public string DownloadDirectory { get; private set; }
        public string DownloadLocationMod { get; private set; }
        public string SaveFileLocation { get; private set; }
        public string BackupFile { get; private set; }
        public string GameLocation { get; private set; }
        public string Platform { get; private set; }
        public string ReleaseURLInstaller { get; private set; }
        public string DownloadLocationInstaller { get; private set; }
        public string ReleaseVersionInstaller { get; private set; }

        public async Task LoadAsync()
        {

            Platform = GetPlatform();
            string configFile = CreateConfig();
            string appsettingsLocalLow = GetAppsettingsLocalLow();
            string ironGateFolder = Path.Combine(appsettingsLocalLow, "IronGate");

            GameLocation = GetGamePath(configFile);
            SaveFileLocation = Path.Combine(ironGateFolder, "Valheim");
            BackupFile = Path.Combine(ironGateFolder, "Valheim.zip");
            DownloadDirectory = GetDownloadDirectory();


            string modfile = CreateModFilename(configFile);

            ReleaseURLMod = await GetLatestGithubReleaseDownloadUrlAsync("valheimPlus", "ValheimPlus") + modfile;
            DownloadLocationMod = Path.Combine(DownloadDirectory, modfile);

            string installerFile = CreateInstallerFilename();
            ReleaseURLInstaller = await GetLatestGithubReleaseDownloadUrlAsync("Witteborn", "ValheimPlusInstaller") + installerFile;
            DownloadLocationInstaller = Path.Combine(Directory.GetParent(System.AppContext.BaseDirectory).ToString(), installerFile);
            ReleaseVersionInstaller = await GetReleaseVersionAsync("Witteborn", "ValheimPlusInstaller");
        }

        private async Task<string> GetReleaseVersionAsync(string owner, string project)
        {
            GitHubClient client = new GitHubClient(new ProductHeaderValue(project));
            IReadOnlyList<Release> releases = await client.Repository.Release.GetAll(owner, project);
            Release latest = releases[0];
            return latest.TagName;
        }

        private string CreateInstallerFilename()
        {
            string filename = "ValheimPlusInstaller";
            string runtime = RuntimeInformation.RuntimeIdentifier.Replace("win10", "win");
            string ending = ".zip";

            return filename + "_" + runtime + ending;
        }

        private string CreateModFilename(string path)
        {
            string target = GetIsServer(path) ? "Server" : "Client";

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
            else
            {
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

        private async Task<string> GetLatestGithubReleaseDownloadUrlAsync(string owner, string project)
        {
            string releaseTag = await GetReleaseVersionAsync(owner, project);
            string link = $"https://github.com/{owner}/{project}/releases/download/{releaseTag}/";

            return link;
        }

        private static string GetGamePath(string path)
        {
            string gamepath = string.Empty;
            string[] lines = File.ReadAllLines(path);


            bool fileDoesntExist = File.Exists(path) == false;
            bool fileIsEmpty = lines.Length == 0;

            if (fileDoesntExist || fileIsEmpty || string.IsNullOrWhiteSpace(lines[0]))
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
