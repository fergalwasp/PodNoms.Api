using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PodNoms.Api.Models.ViewModels {
    public class PodcastViewModel {
        public PodcastViewModel() {
            this.PodcastEntries = new Collection<EntryViewModel>();
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Slug { get; set; }
        public string Image { get; set; }
        public string RssUrl { get; set; }
        public ICollection<EntryViewModel> PodcastEntries { get; set; }
    }
}