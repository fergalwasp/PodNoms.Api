using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PodNoms.Api.Models;
using PodNoms.Api.Services.Storage;

namespace PodNoms.Api.Controllers
{
    //[Authorize]
    [Route("/podcast/{slug}/audioupload")]
    public class AudioUploadController : Controller
    {
        private readonly IPodcastRepository _repository;
        private IUnitOfWork _unitOfWork;
        private readonly AudioFileStorageSettings _settings;
        private readonly StorageSettings _storageSettings;
        private readonly IFileStorage _storage;
        private readonly ILogger<ImageUploadController> _logger;

        public AudioUploadController(IPodcastRepository repository, IUnitOfWork unitOfWork,
                IFileStorage storage, IOptions<AudioFileStorageSettings> settings, IOptions<StorageSettings> storageSettings, ILoggerFactory loggerFactory)
        {
            this._storage = storage;
            this._settings = settings.Value;
            this._storageSettings = storageSettings.Value;
            this._repository = repository;
            this._unitOfWork = unitOfWork;
            this._logger = loggerFactory.CreateLogger<ImageUploadController>();
        }
        [HttpPost]
        public async Task<IActionResult> Upload(string slug, IFormFile file)
        {
            _logger.LogDebug($"Settings are\nMaxUploadFileSize: {_settings.MaxUploadFileSize}");
            if (file == null || file.Length == 0) return BadRequest("No file found in stream");
            if (file.Length > _settings.MaxUploadFileSize) return BadRequest("Maximum file size exceeded");
            if (!_settings.IsSupported(file.FileName)) return BadRequest("Invalid file type");

            var podcast = await _repository.GetAsync(slug);
            if (podcast == null)
                return NotFound();

            var audioUrl = await _storage.StoreItem(Path.GetTempPath(), "audio", file);
            podcast.PodcastEntries.Add(new PodcastEntry
            {
                AudioUrl = audioUrl,
                Title = Path.GetFileName(Path.GetFileNameWithoutExtension(file.FileName)),
                ImageUrl = $"{_storageSettings.CdnUrl}static/images/default-entry.png",
                Processed = true,
                ProcessingStatus = ProcessingStatus.Processed
            });

            await this._unitOfWork.CompleteAsync();
            return Ok();
        }
    }
}