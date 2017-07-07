using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PodNoms.Api.Models;
using PodNoms.Api.Models.ViewModels;
using PodNoms.Api.Persistence;

namespace PodNoms.Api.Controllers {
    [Authorize]
    [Route("[controller]")]
    public class ProfileController : Controller {
        private IUserRepository _userRepository;

        public ProfileController(IUserRepository userRepository) {
            _userRepository = userRepository;
        }

        public ActionResult Get() {
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email) ? .Value;
            var user = _userRepository.Get(email);
            if (user != null) {
                return new OkObjectResult(new ProfileViewModel {
                    Id = user.Id,
                        Email = user.EmailAddress,
                        Name = user.FullName,
                        Description = "TODO",
                        ProfileImage = user.ProfileImage,
                        ApiKey = user.ApiKey
                });
            }
            return new NotFoundResult();
        }

        [HttpPost]
        public ActionResult Post([FromBody] ProfileViewModel item) {
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email) ? .Value;
            var user = _userRepository.Get(email);

            if (user == null || user.Id != item.Id)
                return new UnauthorizedResult();

            user.Id = item.Id;
            user.EmailAddress = item.Email;
            user.FullName = item.Name;
            user.ProfileImage = item.ProfileImage;
            user.ApiKey = item.ApiKey;

            _userRepository.AddOrUpdate(user);
            return new OkObjectResult(item);
        }

        [HttpPost("/updateapikey")]
        public ActionResult UpdateApiKey() {
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email) ? .Value;
            var user = _userRepository.Get(email);
            if (user != null) {
                var newKey = _userRepository.UpdateApiKey(email);
                return new OkObjectResult(newKey);
            }
            return new NotFoundResult();
        }
    }
}