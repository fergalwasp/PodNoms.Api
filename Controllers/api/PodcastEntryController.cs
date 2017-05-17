using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PodNoms.Api.Models;
using PodNoms.Api.Models.ViewModels;
using PodNoms.Api.Services.Processor;
using PodNoms.Api.Services.Processor.Hangfire;

namespace PodNoms.Api.Controllers.api {
    [Route("api/[controller]")]
    public class PodcastEntryController : Controller {
        private readonly IPodcastRepository _repository;
        private readonly ILogger _logger;
        private readonly IOptions<AppSettings> _options;
        private readonly IMapper _mapper;

        public PodcastEntryController(IPodcastRepository repository, ILoggerFactory loggerFactory,
            IOptions<AppSettings> options, IMapper mapper) {
            _repository = repository;
            _options = options;
            _logger = loggerFactory.CreateLogger<PodcastEntryController>();
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

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] PodcastEntryViewModel item) {
            if (item != null && !string.IsNullOrEmpty(item.SourceUrl)) {
                var entry = _mapper.Map<PodcastEntryViewModel, PodcastEntry>(item, o => {
                        o.AfterMap((src, dest) => {
                            dest.Processed = false;
                            dest.ProcessingStatus = ProcessingStatus.Waiting;
                            dest.CreateDate = System.DateTime.Now;
                        });
                    });
                entry = await this._repository.AddEntryAsync(item.PodcastId, entry);
                if (entry != null) {
                    BackgroundJob.Enqueue<IUrlProcessService>(service => service.ProcessUrl(
                        entry.Uid,
                        item.SourceUrl));
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