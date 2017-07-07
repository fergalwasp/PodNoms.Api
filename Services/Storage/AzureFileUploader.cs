using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using PodNoms.Api.Models;
using PodNoms.Api.Utils.Extensions;

namespace PodNoms.Api.Services.Storage
{
    public class AzureFileUploader : IFileUploader
    {
        private readonly IOptions<StorageSettings> _settings;
        private readonly ILogger _logger;
        public AzureFileUploader(IOptions<StorageSettings> settings, ILoggerFactory logger)
        {
            this._logger = logger.CreateLogger<AzureFileUploader>();
            this._settings = settings;

        }
        public async Task<string> UploadFile(string sourceFile, string containerName, string destinationFile)
        {
            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_settings.Value.ConnectionString);
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference(containerName);
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(destinationFile);

                using (var fileStream = System.IO.File.OpenRead(sourceFile))
                {
                    await blockBlob.UploadFromStreamAsync(fileStream);
                }
                return $"{containerName}/{destinationFile}";
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error uploading audio: {ex.Message}");
                throw ex;
            }
        }
    }
}