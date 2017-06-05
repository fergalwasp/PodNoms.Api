using AutoMapper;
using PodNoms.Api.Controllers.Resources;
using PodNoms.Api.Models;

namespace PodNoms.Api.Providers {
    public class MappingProvider : Profile {
        public MappingProvider() {

            //Domain to API Resource
            CreateMap<Podcast, PodcastResource>();
            CreateMap<PodcastEntry, EntryResource>();

            //API Resource to Domain
            CreateMap<PodcastResource, Podcast>();
            CreateMap<EntryResource, PodcastEntry>();
        }
    }
}