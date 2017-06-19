namespace PodNoms.Api.Models
{
    public class AppSettings
    {
        public bool UseProxy { get; set; }
        public string RssUrl { get; set; }
    }

    public class AudioStorageSettings
    {
        public string ConnectionString { get; set; }
        public string Container { get; set; }
        public string CdnUrl { get; set; }

    }
}