using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PodNoms.Api.Models;
using PodNoms.Api.Services;
using PodNoms.Api.Services.Storage;
using PodNoms.Api.Utils;

namespace PodNoms.Api.Controllers
{
    //[Authorize]
    [Route("/podcast/{slug}/imageupload")]
    public class ImageUploadController : Controller
    {
        private readonly IPodcastRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ImageFileStorageSettings _settings;
        private readonly IFileStorage _storage;
        private readonly ILogger<ImageUploadController> _logger;

        public ImageUploadController(IPodcastRepository repository, IUnitOfWork unitOfWork, IFileStorage storage,
                                IOptions<ImageFileStorageSettings> settings, ILoggerFactory loggerFactory)
        {
            this._storage = storage;
            this._settings = settings.Value;
            this._repository = repository;
            this._unitOfWork = unitOfWork;
            this._logger = loggerFactory.CreateLogger<ImageUploadController>();
        }
        [HttpPost]
        public async Task<IActionResult> Upload(string slug, IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest("No file found in stream");
            if (file.Length > _settings.MaxUploadFileSize) return BadRequest("Maximum file size exceeded");
            if (!_settings.IsSupported(file.FileName)) return BadRequest("Invalid file type");

            var podcast = await _repository.GetAsync(slug);
            if (podcast == null)
                return NotFound();

            var imageUrl = await _storage.StoreItem(Path.GetTempPath(), "images", file);
            podcast.ImageUrl = imageUrl;
            await this._unitOfWork.CompleteAsync();
            return Ok();
        }
    }
}