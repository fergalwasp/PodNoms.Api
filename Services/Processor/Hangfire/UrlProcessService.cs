using System;
using System.ComponentModel;
using System.Dynamic;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using PodNoms.Api.Controllers.Resources;
using PodNoms.Api.Models;
using PodNoms.Api.Persistence;
using PodNoms.Api.Services.Downloader;
using PodNoms.Api.Utils.Azure;
using PusherServer;

namespace PodNoms.Api.Services.Processor.Hangfire
{
    public interface IUrlProcessService
    {
        Task<bool> GetInformation(int entryId);
        Task<bool> DownloadAudio(int entryId);
    }
    public class UrlProcessService : IUrlProcessService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEntryRepository _repository;
        private readonly IConfiguration _config;
        private readonly ILogger _logger;
        private Pusher _pusher;
        private readonly IFileUploader _fileUploader;
        private readonly IMapper _mapper;
        private JsonSerializer _serializer;

        public UrlProcessService(IEntryRepository repository, IUnitOfWork unitOfWork,
            IConfiguration config, ILoggerFactory logger, IFileUploader fileUploader, IMapper mapper)
        {
            this._fileUploader = fileUploader;
            this._logger = logger.CreateLogger<UrlProcessService>();
            this._repository = repository;
            this._unitOfWork = unitOfWork;
            this._config = config;
            this._mapper = mapper;
            this._serializer = new JsonSerializer
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            var options = new PusherOptions();
            options.Cluster = config["Pusher:Cluster"];
            this._pusher = new Pusher(
                config["Pusher:AppId"],
                config["Pusher:Key"],
                config["Pusher:Secret"],
                options);
        }
        #region Internals
        private async Task<bool> _sendPusherUpdate(PodcastEntry entry)
        {
            var result = _mapper.Map<PodcastEntry, EntryResource>(entry);
            var jobj = JObject.FromObject(result, this._serializer);
            return await _sendPusherUpdate(entry.Uid, "info_processed", jobj);
        }
        private async Task<bool> _sendPusherUpdate(string uid, string message, object data)
        {
            try
            {
                var ITriggerOptions = await _pusher.TriggerAsync(
                    $"{uid}__process_podcast",
                    message,
                    data);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(123456, ex, "Error sending pusher message");
            }
            return false;
        }
        #endregion
        public async Task<bool> GetInformation(int entryId)
        {
            var entry = await _repository.GetAsync(entryId);
            if (entry == null || string.IsNullOrEmpty(entry.SourceUrl))
            {
                _logger.LogError("Unable to process item");
                return false;
            }

            var downloader = new AudioDownloader(entry.SourceUrl);
            downloader.DownloadInfo();
            if (downloader.Properties != null)
            {
                entry.Title = downloader.Properties?.title;
                entry.Description = downloader.Properties?.description;
                entry.Author = downloader.Properties?.uploader;
                entry.ImageUrl = downloader.Properties?.thumbnail;
                entry.ProcessingStatus = ProcessingStatus.Processing;

                await _unitOfWork.CompleteAsync();

                _logger.LogDebug("***DOWNLOAD INFO RETRIEVED****\n");
                _logger.LogDebug($"Title: {entry.Title}\nDescription: {entry.Description}\nAuthor: {entry.Author}\n");

                var pusherResult = await _sendPusherUpdate(entry);
                return true;
            }
            return false;
        }
        public async Task<bool> DownloadAudio(int entryId)
        {
            var entry = await _repository.GetAsync(entryId);
            if (entry == null)
                return false;

            var downloader = new AudioDownloader(entry.SourceUrl);
            var outputFile =
            Path.Combine(System.IO.Path.GetTempPath(), $"{System.Guid.NewGuid().ToString()}.mp3");

            downloader.DownloadProgress += async (s, e) =>
            {
                Console.WriteLine(
                $"Progress: {e.Percentage}% Speed: {e.CurrentSpeed} Size: {e.TotalSize}");
                await _sendPusherUpdate(
                    entry.Uid,
                    "info_progress",
                    JObject.FromObject(new
                    {
                        Percentage = e.Percentage,
                        CurrentSpeed = e.CurrentSpeed,
                        TotalSize = e.TotalSize
                    }, this._serializer));
            };
            downloader.PostProcessing += (s, e) =>
            {
                Console.WriteLine(e);
            };
            var (sourceFile, fileName) = downloader.DownloadAudio();
            if (!string.IsNullOrEmpty(sourceFile))
            {
                entry.ProcessingStatus = ProcessingStatus.Uploading;
                await _sendPusherUpdate(entry);
                
                var cdnFile = await _fileUploader.UploadFile(sourceFile, fileName);
                entry.AudioUrl = cdnFile;
                entry.ProcessingStatus = ProcessingStatus.Processed;
                entry.Processed = true;
                await _unitOfWork.CompleteAsync();

                var pusherResult = await _sendPusherUpdate(entry);
            }
            return false;
        }
    }
}