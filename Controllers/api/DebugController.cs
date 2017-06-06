using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using PodNoms.Api.Models;

namespace PodNoms.Api.Controllers.api {
    [Route("api/[controller]")]
    public class DebugController : Controller {
        private readonly IOptions<AudioStorageSettings> _settings;

        public DebugController(IOptions<AudioStorageSettings> settings) {
            this._settings = settings;

        }
        public string Get() {
            // Configuration["MySecret"]
            return $@"Hello Sailor\nConnectionString:{_settings.Value.ConnectionString}\n
                      Container:{_settings.Value.Container}";
        }
    }
}