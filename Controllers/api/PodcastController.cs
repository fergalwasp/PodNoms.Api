using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PodNoms.Api.Controllers.Resources;
using PodNoms.Api.Models;
using PodNoms.Api.Models.ViewModels;
using PodNoms.Api.Services.Processor.Hangfire;
using PodNoms.Api.Utils.Extensions;

namespace PodNoms.Api.Controllers.api {
    //[Authorize]
    [Route("api/[controller]")]
    public class PodcastController : Controller {
        private readonly IPodcastRepository podcastRepository;
        private readonly IUserRepository userRepository;
        private readonly IOptions<AppSettings> options;
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;

        public PodcastController(IPodcastRepository podcastRepository, IUserRepository userRepository,
            IOptions<AppSettings> options, IMapper mapper, IUnitOfWork unitOfWork) {
            this.unitOfWork = unitOfWork;
            this.podcastRepository = podcastRepository;
            this.userRepository = userRepository;
            this.options = options;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<PodcastResource>> Get() {
            var email = "fergal.moran@gmail.com"; //User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email) ? .Value;
            if (!string.IsNullOrEmpty(email)) {
                var podcasts = await podcastRepository.GetAllAsync(email);
                return mapper.Map<List<Podcast>, List<PodcastResource>>(podcasts.ToList());
            }
            throw new Exception("This is bollocks!");
        }

        [HttpPost]
        public async Task<IActionResult> CreatePodcast([FromBody] PodcastResource podcast) {
            var email = "fergal.moran@gmail.com"; //User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email) ? .Value;
            var user = userRepository.Get(email);
            if (string.IsNullOrEmpty(email) || user == null)
                return new BadRequestObjectResult("Unable to look up user profile");

            if (ModelState.IsValid) {
                var item = mapper.Map<PodcastResource, Podcast>(podcast);
                item.User = user;
                var ret = await podcastRepository.AddOrUpdateAsync(item);
                await unitOfWork.CompleteAsync();
                
                foreach(var entry in item.PodcastEntries){
                    if (entry.ProcessingStatus == ProcessingStatus.Accepted){
                        //new entry, needs to be processed
                        entry.Uid = System.Guid.NewGuid().ToString();
                        BackgroundJob.Enqueue<IUrlProcessService>(service => service.ProcessUrl(
                            entry.Uid,
                            entry.SourceUrl));                     
                    }
                }
                return new OkObjectResult(mapper.Map<Podcast, PodcastResource>(ret));
            }
            return BadRequest("Invalid request data");
        }
    }
}
/*
        [HttpGet("{slug}")]
        public async Task<ActionResult> Get(string slug) {
            return new OkObjectResult(await _podcastRepository.GetAsync(slug));
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] PodcastViewModel item) {
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email) ? .Value;
            var user = _userRepository.Get(email);
            if (string.IsNullOrEmpty(email) || user == null)
                return new BadRequestObjectResult("Unable to look up user profile");

            if (ModelState.IsValid) {
                var podcast = _mapper.Map<Podcast>(item);
                //TODO: move slugify logic
                podcast.User = user;
                var ret = await _podcastRepository.AddOrUpdateAsync(podcast);
                await _unitOfWork.CompleteAsync();
                return new OkObjectResult(ret);
            }
            return BadRequest("Invalid request data");
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id) {
            _podcastRepository.DeleteAsync(id);
            return new OkObjectResult(_podcastRepository.GetAllAsync());
        }

        [HttpGet("rssurl/{slug}")]
        public async Task<ActionResult> RssUrl(string slug) {
            var entry = await _podcastRepository.GetAsync(slug);
            if (entry != null) {
                var result = new {
                    Data = $"__vscode_pp_lerp_start__"+_options.Value.SiteUrl+"__vscode_pp_lerp_end__/api/rss/__vscode_pp_lerp_start__"+entry.Slug+"__vscode_pp_lerp_end__"
                };
                return new OkObjectResult(result);
            }
            return NotFound();
        }
*/