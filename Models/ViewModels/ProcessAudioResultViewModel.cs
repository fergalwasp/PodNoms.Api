namespace PodNoms.Api.Models.ViewModels
{
    public class ProcessAudioResultViewModel
    {
        public string Uid { get; set; }
        public bool Success { get; set; }
        public string Author { get; set; }
        public string AudioUrl { get; set; }
        public float AudioLength { get; set; }
        public long AudioFileSize { get; set; }
        public string ImageUrl { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}