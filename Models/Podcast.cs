using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PodNoms.Api.Models {
    public class Podcast : BaseModel {
        public int Id { get; set; }
        public User User { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Slug { get; set; }
        public string Image { get; set; }
        public ICollection<PodcastEntry> PodcastEntries { get; set; }
        public Podcast() {
            PodcastEntries = new Collection<PodcastEntry>();
        }
    }
}