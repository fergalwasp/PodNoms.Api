using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PodNoms.Api.Models;
using PodNoms.Api.Models.ViewModels;
using PodNoms.Api.Utils.Pusher;

namespace PodNoms.Api.Controllers.api
{
    [Route("api/[controller]")]
    public class ProcessResultController : Controller {
        private readonly IPodcastRepository _podcastRepository;
        private readonly IPusherService _pusherService;
        private readonly ILogger<ProcessResultController> _logger;

        public ProcessResultController(IPodcastRepository repository, IPusherService pusherService, ILoggerFactory loggerFactory) {
            _podcastRepository = repository;
            _pusherService = pusherService;
            _logger = loggerFactory.CreateLogger<ProcessResultController> ();
        }

        [HttpPost]
        public IActionResult ProcessResult(ProcessAudioResultViewModel result) {
            var entry = _podcastRepository.GetEntryByUid(result.Uid);

            if (entry != null) {
                entry.Author = result.Author;
                entry.ImageUrl = result.ImageUrl;
                entry.AudioUrl = result.AudioUrl;
                entry.AudioLength = result.AudioLength;
                entry.AudioFileSize = result.AudioFileSize;
                entry.Title = result.Title;
                entry.Description = result.Description;
                entry.Processed = true;
                _podcastRepository.AddOrUpdateEntry(entry);

                //send realtime event (currently pusher)
                var pusherResult = _pusherService.Trigger(
                    new PusherMessage {
                        name = "audio-processed",
                            channel = "jobs-channel",
                            data = new PusherPayload {
                                message = result.Uid
                            }
                    });
                return Ok(result);
            }
            _logger.LogError("Error processing result: unable to locate entry");
            return BadRequest("Unable to locate mix");
        }
    }
}