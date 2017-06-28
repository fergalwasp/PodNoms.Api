using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PodNoms.Api.Models;
using PodNoms.Api.Services;
using PodNoms.Api.Services.Storage;
using PodNoms.Api.Utils;

namespace PodNoms.Api.Controllers.api
{
    //[Authorize]
    [Route("/api/podcast/{id}/image")]
    public class ImageController : Controller
    {
        private readonly IPodcastRepository _repository;
        private IUnitOfWork _unitOfWork;
        private readonly ImageSettings _imageSettings;
        private readonly IImageStorage _imageStorage;

        public ImageController(IPodcastRepository repository, IUnitOfWork unitOfWork, IImageStorage imageStorage,
                                IOptions<ImageSettings> imageSettings)
        {
            this._imageStorage = imageStorage;
            this._imageSettings = imageSettings.Value;
            this._repository = repository;
            this._unitOfWork = unitOfWork;
        }
        [HttpPost]
        public async Task<IActionResult> Upload(int id, IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest("No file found in stream");
            if (file.Length > _imageSettings.MaxUploadFileSize) return BadRequest("Maximum file size exceeded");
            if (!_imageSettings.IsSupported(file.FileName)) return BadRequest("Invalid file type");

            var podcast = await _repository.GetAsync(id);
            if (podcast == null)
                return NotFound();

            var imageUrl = await _imageStorage.StoreImage(Path.GetTempPath(), file);
            podcast.Image = imageUrl;
            await this._unitOfWork.CompleteAsync();
            return Ok();
        }
    }
}