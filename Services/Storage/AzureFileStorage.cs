using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace PodNoms.Api.Services.Storage
{
    public class AzureFileStorage : CachedFileStorage, IFileStorage
    {
        private readonly IFileUploader _fileUploader;

        public AzureFileStorage(IFileUploader fileUploader)
        {
            this._fileUploader = fileUploader;

        }
        public async Task<string> StoreItem(string uploadFolderPath, string container, IFormFile file)
        {
            var cachedFile = await this.CacheItem(uploadFolderPath, file);
            var result = await _fileUploader.UploadFile(cachedFile, container, Path.GetFileName(cachedFile));
            return result;
        }
    }
}