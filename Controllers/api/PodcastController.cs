#region imports
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
using PodNoms.Api.Persistence;
using PodNoms.Api.Services.Processor.Hangfire;
using PodNoms.Api.Utils.Extensions;
#endregion
namespace PodNoms.Api.Controllers.api
{
    [Authorize]
    [Route("api/[controller]")]
    public class PodcastController : Controller
    {
        private readonly IPodcastRepository _repository;
        private readonly IUserRepository _userRepository;
        private readonly IOptions<AppSettings> _settings;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;

        public PodcastController(IPodcastRepository repository, IUserRepository userRepository,
            IOptions<AppSettings> options, IMapper mapper, IUnitOfWork unitOfWork)
        {
            this._uow = unitOfWork;
            this._repository = repository;
            this._userRepository = userRepository;
            this._settings = options;
            this._mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<PodcastResource>> Get()
        {
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (!string.IsNullOrEmpty(email))
            {
                var podcasts = await _repository.GetAllAsync(email);
                return _mapper.Map<List<Podcast>, List<PodcastResource>>(podcasts.ToList());
            }
            throw new Exception("This is bollocks!");
        }

        [HttpGet("rssurl/{slug}")]
        public async Task<IActionResult> GetRssUrl(string slug)
        {
            var item = await _repository.GetAsync(slug);
            return new OkObjectResult(new {
                Url = $"{_settings.Value.RssUrl}{item.Slug}"
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreatePodcast([FromBody] PodcastResource podcast)
        {
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var user = _userRepository.Get(email);
            if (string.IsNullOrEmpty(email) || user == null)
                return new BadRequestObjectResult("Unable to look up user profile");

            if (ModelState.IsValid)
            {
                var item = _mapper.Map<PodcastResource, Podcast>(podcast);
                item.User = user;

                var ret = await _repository.AddOrUpdateAsync(item);
                await _uow.CompleteAsync();
                return new OkObjectResult(_mapper.Map<Podcast, PodcastResource>(ret));
            }
            return BadRequest("Invalid request data");
        }
    }
}