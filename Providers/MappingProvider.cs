using AutoMapper;
using Microsoft.Extensions.Options;
using PodNoms.Api.Controllers.Resources;
using PodNoms.Api.Models;

namespace PodNoms.Api.Providers {
    public class MappingProvider : Profile {
        public MappingProvider() {

            //Domain to API Resource
            CreateMap<Podcast, PodcastResource>();
            CreateMap<PodcastEntry, EntryResource>()
                .ForMember(
                    e => e.AudioUrl, 
                    e => e.MapFrom(m => $"https://podnomscdn.blob.core.windows.net/{m.AudioUrl}"));

            //API Resource to Domain
            CreateMap<PodcastResource, Podcast>();
            CreateMap<EntryResource, PodcastEntry>();
        }
    }
}