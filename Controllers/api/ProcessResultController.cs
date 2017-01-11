using System.Linq;
using Microsoft.AspNetCore.Mvc;
using PodNoms.Api.Models;
using PodNoms.Api.Models.ViewModels;
using PodNoms.Api.Utils.Extensions;
using PodNoms.Api.Utils.Pusher;

namespace PodNoms.Api.Controllers.api
{
    [Route("api/[controller]")]
    public class ProcessResultController : Controller
    {
        private readonly IPodcastRepository _podcastRepository;
        private readonly IPusherService _pusherService;
        public ProcessResultController(IPodcastRepository repository, IPusherService pusherService)
        {
            _podcastRepository = repository;
            _pusherService = pusherService;
        }

        [HttpPost]
        public ProcessAudioResultViewModel ProcessResult(ProcessAudioResultViewModel result)
        {
            var entry = _podcastRepository.GetEntryByUid(result.Uid);
            var slug = result.Title.Slugify(
                from e in
                    this._podcastRepository.GetAllEntries(entry.Id)
                select e.Title);


            if (entry != null)
            {
                entry.Author = result.Author;
                entry.ImageUrl = result.ImageUrl;
                entry.AudioUrl = result.AudioUrl;
                entry.AudioLength = result.AudioLength;
                entry.AudioFileSize = result.AudioFileSize;
                entry.Title = result.Title;
                entry.Slug = slug;
                entry.Description = result.Description;
            }
            _podcastRepository.AddOrUpdateEntry(entry);

            //send realtime event (currently pusher)
            var pusherResult = _pusherService.Trigger(
                new PusherMessage
                {
                    name = "audio_processed",
                    channel = "jobs_channel",
                    data = new PusherPayload
                    {
                        message = "hello world"
                    }
                });
            //new { message = "hello world" } );
            return result;
        }
    }
}