using System.Collections.Generic;

namespace PodNoms.Api.Models
{
    public class Podcast : BaseModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Slug { get; set; }
        public string ImageUrl { get; set; }
        public IList<PodcastEntry> PodcastEntries { get; set; }
        public User User { get; set; }
    }
}