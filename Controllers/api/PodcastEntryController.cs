using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PodNoms.Api.Models;
using PodNoms.Api.Models.ViewModels;
using PodNoms.Api.Services.Processor;
using PodNoms.Api.Utils.Extensions;

namespace PodNoms.Api.Controllers.api
{
    [Route("api/[controller]")]
    public class PodcastEntryController : Controller
    {
        private readonly IPodcastRepository _repository;
        private readonly IProcessorInterface _processor;
        private readonly ILogger _logger;
        private readonly IOptions<AppSettings> _options;

        public PodcastEntryController(IPodcastRepository repository,
            IProcessorInterface processor,
            ILoggerFactory loggerFactory,
            IOptions<AppSettings> options)
        {
            _repository = repository;
            _processor = processor;
            _options = options;
            _logger = loggerFactory.CreateLogger<ManageController>();
        }

        [HttpGet]
        public IActionResult GetAll(int podcastId)
        {
            return new OkObjectResult(_repository.GetAllEntries(podcastId));
        }

        [HttpPost]
        public IActionResult Add([FromBody] PodcastEntryViewModel item)
        {
            if (item != null && !string.IsNullOrEmpty(item.SourceUrl))
            {
                if (string.IsNullOrEmpty(item.ImageUrl))
                    item.ImageUrl = $"http://lorempixel.com/400/200?{System.Guid.NewGuid().ToString()}";
                if (string.IsNullOrEmpty(item.Title))
                    item.Title = "Processing audio";
                if (string.IsNullOrEmpty(item.Description))
                    item.Description = "In the meantime, here's a zombie";

                var slug = item.Title.Slugify(
                    from e in this._repository.__getAllEntries()
                    select e.Title);

                var entry = new PodcastEntry
                {
                    Author = item.Author,
                    Uid = System.Guid.NewGuid().ToString(),
                    Title = item.Title,
                    Description = item.Description,
                    Slug = slug,
                    ImageUrl = item.ImageUrl,
                    SourceUrl = item.SourceUrl,
                    AudioUrl = item.AudioUrl,
                    CreateDate = System.DateTime.Now
                };
                entry = this._repository.AddEntry(item.PodcastId, entry);
                if (entry != null)
                {
                    var callbackAddress =
                        $"{_options.Value.SiteUrl}/api/processresult";
                    _logger.LogDebug($"Posting podcast to jobs, callback is: {callbackAddress}");
                    var processorResult = _processor.SubmitNewAudioItem(
                        item.SourceUrl,
                        entry.Uid,
                        callbackAddress);

                    return new OkObjectResult(entry);
                }
            }
            return BadRequest("Invalid request data");
        }

        [HttpDelete("{slug}")]
        public ActionResult Delete(string slug)
        {
            var entry = _repository.GetEntry(slug);
            if (entry != null)
            {
                var podcastId = entry.Podcast.Id;
                this._repository.DeleteEntry(slug);

                var podcast = _repository.Get(podcastId);
                if (podcast.PodcastEntries != null)
                    return new OkObjectResult(podcast.PodcastEntries);
                else
                    return new OkResult();
            }

            return new NotFoundResult();
        }

    }
}
