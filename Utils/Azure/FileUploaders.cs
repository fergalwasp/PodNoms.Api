using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using PodNoms.Api.Models;

namespace PodNoms.Api.Utils.Azure {
    public interface IFileUploader {
        Task<string> UploadFile(string sourceFile, string destinationFile);
    }
    public class FileUploader : IFileUploader {
        private readonly IOptions<AudioStorageSettings> _settings;
        private readonly ILogger _logger;
        public FileUploader(IOptions<AudioStorageSettings> settings, ILoggerFactory logger) {
            this._logger = logger.CreateLogger<FileUploader>();
            this._settings = settings;

        }
        public async Task<string> UploadFile(string sourceFile, string destinationFile) {

            try {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_settings.Value.ConnectionString);
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference(_settings.Value.Container);
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(destinationFile);

                using(var fileStream = System.IO.File.OpenRead(sourceFile)) {
                    await blockBlob.UploadFromStreamAsync(fileStream);
                }
                return $"{_settings.Value.Container}/{destinationFile}";
            } catch (Exception ex) {
                _logger.LogError($"Error uploading audio: {ex.Message}");
                throw ex;
            }
        }
    }
}