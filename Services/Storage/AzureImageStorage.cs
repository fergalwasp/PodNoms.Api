using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace PodNoms.Api.Services.Storage
{
    public class AzureImageStorage : CachedImagedStorage, IImageStorage
    {
        private readonly IFileUploader _fileUploader;

        public AzureImageStorage(IFileUploader fileUploader)
        {
            this._fileUploader = fileUploader;

        }
        public async Task<string> StoreImage(string uploadFolderPath, IFormFile file)
        {
            var cachedFile = await this.CacheItem(uploadFolderPath, file);
            var result = await _fileUploader.UploadFile(cachedFile, "images", Path.GetFileName(cachedFile));
            return result;
        }
    }
}