using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PodNoms.Api.Models;
using PodNoms.Api.Models.ViewModels;
using PodNoms.Api.Services.Processor;
using PodNoms.Api.Utils.Extensions;
using Polly;

namespace PodNoms.Api.Controllers.api {
    [Route("api/[controller]")]
    public class PodcastEntryController : Controller {
        private readonly IPodcastRepository _repository;
        private readonly IProcessorInterface _processor;
        private readonly ILogger _logger;
        private readonly IOptions<AppSettings> _options;

        public PodcastEntryController(IPodcastRepository repository,
            IProcessorInterface processor,
            ILoggerFactory loggerFactory,
            IOptions<AppSettings> options) {
            _repository = repository;
            _processor = processor;
            _options = options;
            _logger = loggerFactory.CreateLogger<ManageController> ();
        }

        [HttpGet("{uid}")]
        public IActionResult Get(string uid) {
            return Ok(_repository.GetEntryByUid(uid));
        }

        [HttpGet("{podcastId:int}")]
        public IActionResult Get(int id) {
            return Ok(_repository.GetEntry(id));
        }

        [HttpGet("waiting")]
        public IActionResult GetWaiting() {
            var entries = _repository.GetEntryByStatus(ProcessingStatus.Waiting);
            return Ok(entries);
        }

        [HttpGet("accepted")]
        public IActionResult GetAccepted() {
            var entries = _repository.GetEntryByStatus(ProcessingStatus.Accepted);
            return Ok(entries);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] PodcastEntryViewModel item) {
            if (item != null && !string.IsNullOrEmpty(item.SourceUrl)) {
                if (string.IsNullOrEmpty(item.Title))
                    item.Title = item.SourceUrl;

                var entry = new PodcastEntry {
                    Author = item.Author,
                        Uid = item.Uid,
                        Title = item.Title,
                        Description = item.Description,
                        ImageUrl = item.ImageUrl,
                        SourceUrl = item.SourceUrl,
                        AudioUrl = item.AudioUrl,
                        Processed = false,
                        ProcessingStatus = ProcessingStatus.Waiting,
                        CreateDate = System.DateTime.Now
                };
                entry = this._repository.AddEntry(item.PodcastId, entry);
                if (entry != null) {
                    var callbackAddress = $"{_options.Value.SiteUrl}/api/processresult";

                    var processorResult = await _processor.SubmitNewAudioItem(
                        item.SourceUrl,
                        entry.Uid,
                        callbackAddress);
                    try {
                        if (processorResult.StatusCode == HttpStatusCode.OK) {
                            entry.ProcessingStatus = ProcessingStatus.Accepted;
                            this._repository.AddOrUpdateEntry(entry);
                            return Ok(entry);
                        }
                    } catch (HttpRequestException ex) {
                        var retryPolicy = Policy.Handle<HttpRequestException> ().Retry(15);
                        await retryPolicy.Execute(async() => {
                            processorResult = await _processor.SubmitNewAudioItem(
                                item.SourceUrl,
                                entry.Uid,
                                callbackAddress);
                            if (processorResult.StatusCode == HttpStatusCode.OK) {
                                entry.ProcessingStatus = ProcessingStatus.Accepted;
                                this._repository.AddOrUpdateEntry(entry);
                            }
                        });
                        return Accepted(entry);
                    }
                }
            }
            return BadRequest("Invalid request data");
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id) {
            var entry = _repository.GetEntry(id);
            if (entry != null) {
                var podcastId = entry.Podcast.Id;
                this._repository.DeleteEntry(id);

                var podcast = _repository.Get(podcastId);
                if (podcast.PodcastEntries != null)
                    return Ok(podcast.PodcastEntries);
                else
                    return new OkResult();
            }
            return new NotFoundResult();
        }

    }
}