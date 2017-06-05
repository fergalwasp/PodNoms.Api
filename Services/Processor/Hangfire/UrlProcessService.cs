using System;
using System.ComponentModel;
using System.Dynamic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PodNoms.Api.Models;
using PodNoms.Api.Persistence;
using PodNoms.Api.Services.Downloader;
using PusherServer;

namespace PodNoms.Api.Services.Processor.Hangfire {
    public interface IUrlProcessService {
        Task<bool> GetInformation(int entryId);
        Task<bool> DownloadAudio(int entryId);
    }
    public class UrlProcessService : IUrlProcessService {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEntryRepository _repository;
        private readonly IConfiguration _config;
        private readonly ILogger _logger;
        private Pusher _pusher;

        public UrlProcessService(IEntryRepository repository, IUnitOfWork unitOfWork,
            IConfiguration config, ILoggerFactory loggerFactory) {
            this._logger = loggerFactory.CreateLogger<UrlProcessService>();
            this._repository = repository;
            this._unitOfWork = unitOfWork;
            this._config = config;

            var options = new PusherOptions();
            options.Cluster = config["Pusher:Cluster"];
            this._pusher = new Pusher(
                config["Pusher:AppId"],
                config["Pusher:Key"],
                config["Pusher:Secret"],
                options);
        }
        public async Task<bool> GetInformation(int entryId) {
            var entry = await _repository.GetAsync(entryId);
            if (entry == null || string.IsNullOrEmpty(entry.SourceUrl)) {
                _logger.LogError("Unable to process item");
                return false;
            }

            var downloader = new AudioDownloader(entry.SourceUrl);
            downloader.DownloadInfo();
            if (downloader.Properties != null) {
                entry.Title = downloader.Properties?.title;
                entry.Description = downloader.Properties?.description;
                entry.Author = downloader.Properties?.uploader;
                entry.ImageUrl = downloader.Properties?.thumbnail;
                await _unitOfWork.CompleteAsync();

                _logger.LogDebug("***DOWNLOAD INFO RETRIEVED****\n");
                _logger.LogDebug($"Title: {entry.Title}\nDescription: {entry.Description}\nAuthor: {entry.Author}\n");

                try {
                    var ITriggerOptions =
                        await _pusher.TriggerAsync(
                            $"{entry.Uid}__process_podcast",
                            "info_processed",
                            new {
                                message = entry.Title
                            });
                } catch (Exception ex) {
                    _logger.LogError(123456, ex, "Error sending pusher message");
                }
                return true;
            }
            return false;
        }

        public async Task<bool> DownloadAudio(int entryId) {
            var entry = await _repository.GetAsync(entryId);
            if (entry == null)
                return false;

            var downloader = new AudioDownloader(entry.SourceUrl);
            var outputFile =
                Path.Combine(System.IO.Path.GetTempPath(), $"{System.Guid.NewGuid().ToString()}.mp3");

            downloader.DownloadProgress += (s, e) => {
                Console.WriteLine(
                    $"Progress: {e.Percentage}% Speed: {e.CurrentSpeed} Size: {e.TotalSize}");
            };
            downloader.PostProcessing += (s, e) => {
                Console.WriteLine(e);
            };
            var file = downloader.DownloadAudio();
            entry.ProcessingStatus = ProcessingStatus.Processed;
            entry.Processed = true;
            await _unitOfWork.CompleteAsync();

            return true;
        }
    }
}