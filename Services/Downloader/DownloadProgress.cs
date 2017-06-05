namespace PodNoms.Api.Services.Downloader {
    public class DownloadProgress {
        public double Percentage { get; set; }
        public string TotalSize;
        public string CurrentSpeed { get; set; }
        public string ETA { get; set; }
    }
}