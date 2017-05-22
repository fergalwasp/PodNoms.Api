using System.Runtime.Serialization;

namespace PodNoms.Api.Models {
    public enum ProcessingStatus {
        Waiting,
        Accepted,
        Processing,
        Processed
    }
    public class PodcastEntry : BaseModel {

        public int Id { get; set; }
        public string Uid { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Slug { get; set; }
        public string SourceUrl { get; set; }
        public string AudioUrl { get; set; }
        public float AudioLength { get; set; }
        public long AudioFileSize { get; set; }
        public string ImageUrl { get; set; }
        public string ProcessingPayload { get; set; }
        public ProcessingStatus ProcessingStatus { get; set; } = ProcessingStatus.Accepted;
        public bool Processed { get; set; }
        public int PodcastId { get; set; }
        public Podcast Podcast { get; set; }
    }
}