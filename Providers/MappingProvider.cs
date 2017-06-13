using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using PodNoms.Api.Controllers.Resources;
using PodNoms.Api.Models;

namespace PodNoms.Api.Providers
{
    public class MappingProvider : Profile
    {
        private readonly IConfiguration _options;
        public MappingProvider() { }
        public MappingProvider(IConfiguration options)
        {
            this._options = options;

            //Domain to API Resource
            CreateMap<Podcast, PodcastResource>()
            .ForMember(
                e => e.RssUrl,
                e => e.MapFrom(m => $"{this._options.GetSection("App")["RssUrl"]}{m.Slug}"));
            CreateMap<PodcastEntry, EntryResource>()
                .ForMember(
                    e => e.AudioUrl,
                    e => e.MapFrom(m => $"{this._options.GetSection("AudioStorage")["CdnUrl"]}{m.AudioUrl}"));

            //API Resource to Domain
            CreateMap<PodcastResource, Podcast>();
            CreateMap<EntryResource, PodcastEntry>();
        }
    }
}