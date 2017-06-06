namespace PodNoms.Api.Models {
    public class AppSettings {
        public string SiteUrl { get; set; }
        public bool UseProxy { get; set; }
    }

    public class AudioStorageSettings {
        public string ConnectionString { get; set; }
        public string Container { get; set; }
    }
}