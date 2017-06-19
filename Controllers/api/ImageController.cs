using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PodNoms.Api.Models;
using PodNoms.Api.Utils;

namespace PodNoms.Api.Controllers.api
{
    [Authorize]
    [Route("/api/podcast/{slug}/image")]
    public class ImageController : Controller
    {
        private readonly IPodcastRepository _repository;
        private IUnitOfWork _unitOfWork;
        private readonly ImageSettings _imageSettings;

        public ImageController(IPodcastRepository repository, IUnitOfWork unitOfWork,
                                IOptions<ImageSettings> imageSettings)
        {
            this._imageSettings = imageSettings.Value;
            this._repository = repository;
            this._unitOfWork = unitOfWork;
        }
        [HttpPost]
        public async Task<IActionResult> Upload(string slug, IFormFile file)
        {
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var podcast = await _repository.GetAsync(email, slug);
            if (podcast == null)
                return NotFound();

            if (file == null || file.Length == 0) return BadRequest("No file found in stream");
            if (file.Length > _imageSettings.MaxUploadFileSize) return BadRequest("Maximum file size exceeded");
            if (!_imageSettings.IsSupported(file.FileName)) return BadRequest("Invalid file type");

            var path = Path.GetTempPath();
            var fileName = Path.Combine(path, System.Guid.NewGuid().ToString() + Path.GetExtension(file.FileName));
            using (var stream = new FileStream(fileName, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            podcast.Image = await ImageUtils.ImageAsBase64(fileName);
            await this._unitOfWork.CompleteAsync();
            return Ok();
        }
    }
}