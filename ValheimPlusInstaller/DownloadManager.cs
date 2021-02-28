using System;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;

namespace ValheimPlusInstaller
{
    public abstract class DownloadManager
    {
        private WebClient Downloader { get; set; } = new WebClient();
        protected Config Config { get; set; }

        public DownloadManager(Config config)
        {
            Config = config;
            // fake as if you are a browser making the request.
            Downloader.Headers.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 8.0)");
            Downloader.DownloadFileCompleted += OnDownloadFileCompleted;
            Downloader.DownloadProgressChanged += OnDownloadProgressChanged;
        }

        protected abstract void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e);
        protected abstract void OnDownloadFileCompleted(object sender, AsyncCompletedEventArgs e);

        public async Task DownloadFileAsync(string sourceUrl, string targetFolder)
        {
            Console.WriteLine("Beginn downloading the latest release");
            Console.WriteLine($"Downloading\n" +
                              $"From {sourceUrl}\n" +
                              $"To {targetFolder}");
            await Downloader.DownloadFileTaskAsync(new Uri(sourceUrl), targetFolder);
        }
    }
}