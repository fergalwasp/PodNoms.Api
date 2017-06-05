using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using PodNoms.Api.Models;
using PodNoms.Api.Persistence;

namespace PodNoms.Api.Services.Auth {
    public class AuthenticationMiddleware {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AuthenticationMiddleware> _logger;

        public AuthenticationMiddleware(IUserRepository userRepository, ILoggerFactory loggerFactory) {
            _userRepository = userRepository;
            _logger = loggerFactory.CreateLogger<AuthenticationMiddleware> ();
        }

        public static Task OnTokenValidated(TokenValidatedContext context) {
            var userRepository =
                (IUserRepository) context.HttpContext.RequestServices.GetService(typeof (IUserRepository));
            var claimsIdentity = context.Ticket.Principal.Identity as ClaimsIdentity;
            if (claimsIdentity != null) {
                claimsIdentity.AddClaim(new Claim("id_token",
                    context.Request.Headers["Authorization"][0].Substring(context.Ticket.AuthenticationScheme.Length + 1)));

                var picture = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "picture") ? .Value;
                var name = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "name") ? .Value;

                var data = new {
                    Name = name,
                        EmailAddress = claimsIdentity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email) ? .Value,
                        Sid = claimsIdentity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid) ? .Value,
                        UserId = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "subject") ? .Value,
                        ProfileImage = picture
                };
                userRepository.UpdateRegistration(data.EmailAddress, data.Name, data.Sid, data.UserId,
                    data.ProfileImage);
            }
            return Task.FromResult(0);
        }
    }
}