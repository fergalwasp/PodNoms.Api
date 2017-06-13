using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PodNoms.Api.Controllers.Resources {
    public class PodcastResource {
        public PodcastResource() {
            this.PodcastEntries = new Collection<EntryResource>();
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Slug { get; set; }
        public string ImageUrl { get; set; }
        public string RssUrl { get; set; }
        public ICollection<EntryResource> PodcastEntries { get; set; }
    }
}