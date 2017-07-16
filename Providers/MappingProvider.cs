using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using PodNoms.Api.Models;
using PodNoms.Api.Models.ViewModels;

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
            CreateMap<Podcast, PodcastViewModel>()
                .ForMember(
                    e => e.RssUrl,
                    e => e.MapFrom(m => $"{this._options.GetSection("App")["RssUrl"]}{m.Slug}"))
                .ForMember(
                    e => e.ImageUrl,
                    e => e.MapFrom(m => m.ImageUrl.StartsWith("data:image") ? m.ImageUrl : $"{this._options.GetSection("Storage")["CdnUrl"]}{m.ImageUrl}"));

            CreateMap<PodcastEntry, EntryViewModel>()
                .ForMember(
                    e => e.AudioUrl,
                    e => e.MapFrom(m => $"{this._options.GetSection("Storage")["CdnUrl"]}{m.AudioUrl}"));

            //API Resource to Domain
            CreateMap<PodcastViewModel, Podcast>();
            CreateMap<EntryViewModel, PodcastEntry>();
        }
    }
}