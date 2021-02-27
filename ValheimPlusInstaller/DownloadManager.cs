using System;
using System.ComponentModel;
using System.Net;

namespace ValheimPlusInstaller
{
    public class DownloadManager
    {
        //public DownloadProgressChangedEventHandler DownloadProgressChanged { get; set; }
        //public AsyncCompletedEventHandler DownloadFileCompleted { get; set; }
        public WebClient Downloader { get; set; } = new WebClient();

        public DownloadManager()
        {
            // fake as if you are a browser making the request.
            Downloader.Headers.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 8.0)");
        }

        public void AddDownloadProgressChangedEventHandler(DownloadProgressChangedEventHandler eventHandler )
        {
            Downloader.DownloadProgressChanged += eventHandler;
        }

        public void AddDownloadFileCompletedEventHandler(AsyncCompletedEventHandler eventHandler)
        {
            Downloader.DownloadFileCompleted += eventHandler;
        }

        public void DownloadFile(string sourceUrl, string targetFolder)
        {
            Console.WriteLine($"Downloading\n" +
                $"From {sourceUrl}\n" +
                $"To {targetFolder}");
            Downloader.DownloadFileAsync(new Uri(sourceUrl), targetFolder);
            // wait for the current thread to complete, since the an async action will be on a new thread.
            while (Downloader.IsBusy) { }
        }
    }
}