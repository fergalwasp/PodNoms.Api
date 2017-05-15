using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PodNoms.Api.Models;
using PodNoms.Api.Models.ViewModels;
using PodNoms.Api.Services.Processor;

namespace PodNoms.Api.Controllers.api {
    [Route("api/[controller]")]
    public class PodcastEntryController : Controller {
        private readonly IPodcastRepository _repository;
        private readonly ILogger _logger;
        private readonly IOptions<AppSettings> _options;
        private readonly IProcessorRetryClient _processorRetryClient;
        private readonly IProcessorInterface _processor;
        private readonly IMapper _mapper;

        public PodcastEntryController(IPodcastRepository repository,
            IProcessorInterface processorInterface,
            IProcessorRetryClient processorRetryClient,
            ILoggerFactory loggerFactory,
            IOptions<AppSettings> options,
            IMapper mapper) {
            _repository = repository;
            _processorRetryClient = processorRetryClient;
            _processor = processorInterface;
            _options = options;
            _logger = loggerFactory.CreateLogger<PodcastEntryController> ();
            _mapper = mapper;
        }

        [HttpGet("{uid}")]
        public IActionResult Get(string uid) {
            return Ok(_repository.GetEntryByUid(uid));
        }

        [HttpGet("{podcastId:int}")]
        public IActionResult Get(int id) {
            return Ok(_repository.GetEntry(id));
        }

        [HttpGet("queued")]
        public async Task<IActionResult> GetQueued() {
            var entries = await _repository.GetQueuedEntriesAsync();
            return Ok(entries);
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

        [HttpPost("resend")]
        public async Task<IActionResult> Resend([FromBody] PodcastEntryViewModel item) {
            if (!string.IsNullOrEmpty(item.Uid)) {
                var entry = await _repository.GetEntryAsync(item.Id);
                var callbackAddress = $"{_options.Value.SiteUrl}/api/processresult";
                try {
                    await _processor.SubmitNewAudioItem(
                        item.SourceUrl,
                        item.Uid,
                        callbackAddress);
                    return Ok(item);
                } catch (HttpRequestException) {
                    var result = Task.Run(() => {
                        _processorRetryClient.StartRetryLoop(item.SourceUrl, item.Uid, callbackAddress);
                    }).ContinueWith(e => _logger.LogDebug($"Retry loop exited: {e}"));
                }
                return Accepted(item);
            }
            return BadRequest(item);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] PodcastEntryViewModel item) {
            if (item != null && !string.IsNullOrEmpty(item.SourceUrl)) {
                var entry = _mapper.Map < PodcastEntryViewModel,
                    PodcastEntry > (item, o => {
                        o.AfterMap((src, dest) => {
                            dest.Processed = false;
                            dest.ProcessingStatus = ProcessingStatus.Waiting;
                            dest.CreateDate = System.DateTime.Now;
                        });
                    });
                entry = await this._repository.AddEntryAsync(item.PodcastId, entry);
                if (entry != null) {
                    var result = Task.Run(() => {
                        _processorRetryClient.StartRetryLoop(
                            item.SourceUrl,
                            entry.Uid,
                            $"{_options.Value.SiteUrl}/api/processresult");
                    }).ContinueWith(r => {
                        entry.ProcessingStatus = ProcessingStatus.Accepted;
                        this._repository.AddOrUpdateEntry(entry);
                    });
                    return Accepted(entry);
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