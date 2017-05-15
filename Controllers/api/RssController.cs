using System;
using System.Linq;
using HandlebarsDotNet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PodNoms.Api.Models;
using PodNoms.Api.Models.ViewModels.RssViewModels;
using PodNoms.Api.Utils;
using PodNoms.Api.Utils.Extensions;

namespace PodNoms.Api.Controllers.api
{
    [Route("api/[controller]")]
    public class RssController : Controller
    {
        private readonly IPodcastRepository _repository;
        private readonly IOptions<AppSettings> _options;
        private readonly ILogger _logger;

        public RssController(IPodcastRepository repository, 
                    IOptions<AppSettings> options,
                    ILoggerFactory loggerFactory)
        {
            _repository = repository;
            _options = options;
            _logger = loggerFactory.CreateLogger<RssController>();
        }

        [HttpGet("{slug}")]
        [Produces("application/xml")]
        public string Get(string slug)
        {
            var podcast = _repository.Get(slug);
            if (podcast != null)
            {
                string xml = ResourceReader.ReadResource("podcast.xml", _logger);
                var template = Handlebars.Compile(xml);

                var compiled = new PodcastEnclosureViewModel
                {
                    Title = podcast.Title,
                    Description = podcast.Description,
                    Author = "PodNoms Podcasts",
                    Link = $"{_options.Value.SiteUrl}/podcast/{podcast.Id}",
                    ImageUrl = podcast.ImageUrl,
                    PublishDate = podcast.CreateDate.ToRFC822String(),
                    Language = "en-IE",
                    Copyright = $"© {DateTime.Now.Year} PodNoms",
                    Items = (from e in podcast.PodcastEntries
                             select new PodcastEnclosureItemViewModel
                             {
                                 Title = e.Title,
                                 Description = e.Description,
                                 Author = e.Author,
                                 UpdateDate = e.CreateDate.ToRFC822String(),
                                 AudioUrl = e.AudioUrl,
                                 AudioFileSize = e.AudioFileSize
                             }).ToList()
                };
                var result = template(compiled);
                return result;
            }
            return "";
        }
    }
}