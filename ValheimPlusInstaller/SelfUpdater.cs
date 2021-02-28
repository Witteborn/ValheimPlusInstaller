using System;
using System.ComponentModel;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace ValheimPlusInstaller
{
    public class SelfUpdater : DownloadManager
    {
        public SelfUpdater(Config config) : base(config) { }

        protected override void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Console.WriteLine($"Progress: {e.ProgressPercentage}%");
        }
        protected override void OnDownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            // display completion status.
            bool errorOccured = e.Error != null;
            if (errorOccured)
            {
                Console.WriteLine(e.Error.Message);
                Console.WriteLine(e.Error.StackTrace);
            }
        }

        internal bool IsUpdateAvailable()
        {
            Version localVersion = GetAssemblyVersion();
            Version releaseVersion = Version.Parse(Config.ReleaseVersionInstaller);

            return localVersion < releaseVersion;
        }

        public Version GetAssemblyVersion()
        {

            Assembly a = Assembly.GetExecutingAssembly();

            if (a == null)
            {
                throw new ApplicationException("Executing assembly is null!");
            }

            AssemblyFileVersionAttribute att = a.GetCustomAttribute<AssemblyFileVersionAttribute>();

            if (att == null)
            {
                throw new ApplicationException("Could not find attribute of type '" + typeof(AssemblyFileVersionAttribute).FullName + "'!");
            }

            if (!Version.TryParse(att.Version, out Version version))
            {
                throw new ApplicationException("Error while parsing assembly file version!");
            }

            return version;
        }


        internal new async Task DownloadFileAsync(string releaseURLInstaller, string downloadLocationInstaller)
        {
            await base.DownloadFileAsync(releaseURLInstaller, downloadLocationInstaller);
        }
    }
}
