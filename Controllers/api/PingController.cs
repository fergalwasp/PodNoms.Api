using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PodNoms.Api.Controllers.api {
    [Route("api/[controller]")]
    public class PingController : Controller {
        [Authorize]
        [HttpGet("secure")]
        public string PingSecured() {
            return "Hello Sailor";
        }

        [Authorize]
        [HttpGet("claims")]
        public object Claims() {
            return User.Claims.Select(c =>
                new {
                    Type = c.Type,
                        Value = c.Value
                });
        }

        [Authorize]
        [HttpGet("identity")]
        public object Identity() {
            return new {
                Name = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName) ? .Value,
                    EmailAddress = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email) ? .Value,
                    PrimarySid = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.PrimarySid) ? .Value,
                    Sid = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid) ? .Value,
                    UserId = User.Claims.FirstOrDefault(c => c.Type == "subject") ? .Value,
                    ProfileImage = User.Claims.FirstOrDefault(c => c.Type == "picture") ? .Value
            };
        }

        [HttpGet]
        [RouteAttribute("insecure")]
        public string PingInsecure() {
            return "Hello Sailor";
        }
    }
}