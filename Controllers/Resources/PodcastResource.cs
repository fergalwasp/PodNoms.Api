using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PodNoms.Api.Controllers.Resources {
    public class PodcastResource {
        public PodcastResource() {
            this.PodcastEntries = new Collection<PodcastEntryResource>();
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Slug { get; set; }
        //public string ImageUrl { get; set; }
        public ICollection<PodcastEntryResource> PodcastEntries { get; set; }
    }
}