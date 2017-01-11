using Microsoft.AspNetCore.Mvc;
using PodNoms.Api.Models;
using PodNoms.Api.Models.ViewModels;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using PodNoms.Api.Utils.Extensions;
using Microsoft.Extensions.Options;
using AutoMapper;

namespace PodNoms.Api.Controllers.api
{
    [Authorize]
    [Route("api/[controller]")]
    public class PodcastController : Controller
    {
        private readonly IPodcastRepository _podcastRepository;
        private readonly IUserRepository _userRepository;
        private readonly IOptions<AppSettings> _options;
        private readonly IMapper _mapper;

        public PodcastController(IPodcastRepository podcastRepository, IUserRepository userRepository,
            IOptions<AppSettings> options, IMapper mapper)
        {
            _podcastRepository = podcastRepository;
            _userRepository = userRepository;
            _options = options;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Get()
        {
            if (User == null) return new UnauthorizedResult();
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (!string.IsNullOrEmpty(email))
            {
                return new OkObjectResult(_podcastRepository.GetAll(email));
            }
            return new UnauthorizedResult();
        }

        [HttpGet("{slug}")]
        public ActionResult Get(string slug)
        {
            return new OkObjectResult(_podcastRepository.Get(slug));
        }

        [HttpPost]
        public ActionResult Post([FromBody] PodcastViewModel item)
        {
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var user = _userRepository.Get(email);
            if (string.IsNullOrEmpty(email) || user == null)
                return new BadRequestObjectResult("Unable to looking user profile");

            if (ModelState.IsValid)
            {
                var podcast = _mapper.Map<Podcast>(item);
                var slug = item.Title.Slugify(
                    from e in _podcastRepository.GetAll()
                    select e.Title);
                podcast.Slug = slug;
                var ret = _podcastRepository.AddOrUpdate(podcast);
                return new OkObjectResult(ret);
            }

            /*
            var merged = new Podcast
            {
                Title = item.Title,
                Description = item.Description ?? "",
                ImageUrl = item.ImageUrl ?? "",
                CreateDate = System.DateTime.Now,
                UpdateDate = System.DateTime.Now,
                User = user
            };
            if (item.Id != null && item.Id != -1)
            {
                var podcast = _podcastRepository.Get((int) item.Id);
                if (podcast != null)
                {
                    _podcastRepository.AddOrUpdate(podcast);
                    return new OkObjectResult(podcast);
                }
            }
            else if (item != null && !string.IsNullOrEmpty(item.Title))
            {
                var slug = item.Title.Slugify(
                    from e in _podcastRepository.GetAll()
                    select e.Title);
                merged.Slug = slug;
                var podcast = _podcastRepository.AddOrUpdate(merged);

                return new OkObjectResult(podcast);
            }
            */
            return BadRequest("Invalid request data");
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            _podcastRepository.Delete(id);
            return new OkObjectResult(_podcastRepository.GetAll());
        }

        [HttpGet("rssurl/{slug}")]
        public ActionResult RssUrl(string slug)
        {
            var entry = _podcastRepository.Get(slug);
            if (entry != null)
            {
                var result = new
                {
                    Data = $"{_options.Value.SiteUrl}/api/rss/{entry.Slug}"
                };
                return new OkObjectResult(result);
            }
            return NotFound();
        }
    }
}