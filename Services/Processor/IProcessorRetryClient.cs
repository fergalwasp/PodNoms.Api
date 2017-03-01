using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PodNoms.Api.Models;
using Polly;

namespace PodNoms.Api.Services.Processor {
    public interface IProcessorRetryClient {
        Task<bool> SubmitProcessorRequest(string url, string uid);

        Policy GetRetryPolicy();
        void StartRetryLoop(string url, string uid);
        Task<bool> StartRetryLoopAsync(string url, string uid);
    }

    public class ProcessorRetryClient : IProcessorRetryClient {
        private readonly IProcessorInterface _processor;
        private readonly ILogger<ProcessorRetryClient> _logger;
        private readonly IOptions<AppSettings> _options;
        private readonly string _callbackAddress;

        public ProcessorRetryClient(IProcessorInterface processor, ILoggerFactory loggerFactory, IOptions<AppSettings> options) {
            _processor = processor;
            _logger = loggerFactory.CreateLogger<ProcessorRetryClient> ();
            _options = options;
            _callbackAddress = $"{_options.Value.SiteUrl}/api/processresult";
        }
        public async Task<bool> SubmitProcessorRequest(string url, string uid) {
            var result = await _processor.SubmitNewAudioItem(
                url,
                uid,
                _callbackAddress);
            return result.StatusCode == HttpStatusCode.Accepted;
        }

        public Policy GetRetryPolicy() {
            // in total we will make 3 attempts
            //.WaitAndRetryAsync(10, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)))
            return Policy
                .Handle<Exception> ()
                .WaitAndRetryAsync(10, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                    (ex, retryCount) =>
                    _logger.LogWarning($"Unable to connect to Job Server. Retry attempt {retryCount}. {ex}"));
        }
        public void StartRetryLoop(string url, string uid) {
            StartRetryLoopAsync(url, uid)
                .ContinueWith(r => _logger.LogDebug("Done"));
        }
        public async Task<bool> StartRetryLoopAsync(string url, string uid) {
            return await GetRetryPolicy().ExecuteAsync(async() => {
                var result = await _processor.SubmitNewAudioItem(
                    url,
                    uid,
                    _callbackAddress);
                if (result.StatusCode == HttpStatusCode.OK) {
                    _logger.LogDebug("Job server responded");
                    return true;
                } else {
                    _logger.LogDebug("Job server did not respond");
                }
                return false;
            });
        }
    }
}