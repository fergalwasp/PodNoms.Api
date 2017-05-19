using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PodNoms.Api.Models;
using PodNoms.Api.Models.ViewModels;
using PodNoms.Api.Utils.Extensions;

namespace PodNoms.Api.Controllers.api {
    [Route("api/[controller]")]
    public class PodcastController : Controller {
        private readonly IPodcastRepository _podcastRepository;
        private readonly IUserRepository _userRepository;
        private readonly IOptions<AppSettings> _options;
        private readonly IMapper _mapper;

        public PodcastController(IPodcastRepository podcastRepository, IUserRepository userRepository,
            IOptions<AppSettings> options, IMapper mapper) {
            _podcastRepository = podcastRepository;
            _userRepository = userRepository;
            _options = options;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Get() {
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email) ? .Value;
            if (!string.IsNullOrEmpty(email)) {
                return new OkObjectResult(_podcastRepository.GetAll(email));
            }
            return new UnauthorizedResult();
        }

        [HttpGet("{slug}")]
        public ActionResult Get(string slug) {
            return new OkObjectResult(_podcastRepository.Get(slug));
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] PodcastViewModel item) {
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email) ? .Value;
            var user = _userRepository.Get(email);
            if (string.IsNullOrEmpty(email) || user == null)
                return new BadRequestObjectResult("Unable to looking user profile");
            //Hello Sailor
            if (ModelState.IsValid) {
                var podcast = _mapper.Map<Podcast>(item);
                if (string.IsNullOrEmpty(item.Slug)) {
                    var slug = item.Title.Slugify(
                        from e in _podcastRepository.GetAll() select e.Title);
                    podcast.Slug = slug;
                }
                podcast.User = user;
                var ret = await _podcastRepository.AddOrUpdateAsync(podcast);
                return new OkObjectResult(ret);
            }
            return BadRequest("Invalid request data");
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id) {
            _podcastRepository.Delete(id);
            return new OkObjectResult(_podcastRepository.GetAll());
        }

        [HttpGet("rssurl/{slug}")]
        public ActionResult RssUrl(string slug) {
            var entry = _podcastRepository.Get(slug);
            if (entry != null) {
                var result = new {
                    Data = $"{_options.Value.SiteUrl}/api/rss/{entry.Slug}"
                };
                return new OkObjectResult(result);
            }
            return NotFound();
        }
    }
}