using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using HandlebarsDotNet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PodNoms.Api.Models;
using PodNoms.Api.Models.ViewModels.RssViewModels;
using PodNoms.Api.Utils;
using PodNoms.Api.Utils.Extensions;
using X.Web.RSS;

namespace PodNoms.Api.Controllers.api
{
    [Route("[controller]")]
    public class RssController : Controller
    {
        private readonly IPodcastRepository _repository;
        private readonly ILogger _logger;
        private readonly AppSettings _appOptions;
        private readonly StorageSettings _storageOptions;

        public RssController(IPodcastRepository repository,
            IOptions<AppSettings> appOptions,
            IOptions<StorageSettings> storageOptions,
            ILoggerFactory loggerFactory)
        {
            _repository = repository;
            _appOptions = appOptions.Value;
            _storageOptions = storageOptions.Value;
            _logger = loggerFactory.CreateLogger<RssController>();
        }
        private static string CleanInvalidXmlChars(string text, float xmlVersion = 1.1f)
        {
            const string patternVersion1_0 = @"&#x((10?|[2-F])FFF[EF]|FDD[0-9A-F]|7F|8[0-46-9A-F]9[0-9A-F]);";
            const string patternVersion1_1 = @"&#x((10?|[2-F])FFF[EF]|FDD[0-9A-F]|[19][0-9A-F]|7F|8[0-46-9A-F]|0?[1-8BCEF]);";
            string Pattern = xmlVersion == 1.0f ? patternVersion1_0 : patternVersion1_1;
            string newString = string.Empty;
            Regex regex = new Regex(Pattern, RegexOptions.IgnoreCase);
            if (regex.IsMatch(text))
                newString = regex.Replace(text, "");
            else
                newString = text;

            //remove FUCKING EMOJI!!!!!!!!!
            string result = Regex.Replace(newString, @"\p{Cs}", "");
            return result;
        }

        [HttpGet("{slug}")]
        [Produces("application/xml")]
        public async Task<IActionResult> Get(string slug)
        {
            var podcast = await _repository.GetAsync(slug);
            if (podcast != null)
            {
                string xml = ResourceReader.ReadResource("podcast.xml", _logger);
                var template = Handlebars.Compile(xml);

                var compiled = new PodcastEnclosureViewModel
                {
                    Title = podcast.Title,
                    Description = podcast.Description,
                    Author = "PodNoms Podcasts",
                    Image = podcast.Image,
                    Link = $"{_appOptions.RssUrl}podcast/{podcast.Id}",
                    PublishDate = podcast.CreateDate.ToRFC822String(),
                    Language = "en-IE",
                    Copyright = $"© {DateTime.Now.Year} PodNoms",
                    Items = (
                        from e in podcast.PodcastEntries
                        select new PodcastEnclosureItemViewModel
                        {
                            Title = CleanInvalidXmlChars(e.Title),
                            Description = CleanInvalidXmlChars(e.Description),
                            Author = CleanInvalidXmlChars(e.Author),
                            UpdateDate = e.CreateDate.ToRFC822String(),
                            AudioUrl = $"{_storageOptions.CdnUrl}{e.AudioUrl}",
                            AudioFileSize = e.AudioFileSize
                        }
                    ).ToList()
                };
                var result = template(compiled);
                var rss = RssDocument.Load(result);
                //return new OkObjectResult(rss);
                return Content(result, "application/xml");
            }
            return NotFound();
        }
    }
}
