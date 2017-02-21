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
        Task<bool> StartRetryLoop(string url, string uid);
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
        public async Task<bool> StartRetryLoop(string url, string uid) {
            try {
                //first submit the request
                await SubmitProcessorRequest(url, uid);
            } catch (HttpRequestException) {
                //if failed, enter retry loop
                _logger.LogDebug("Unable to connect to job server, starting retry cycle.");
                int a = 1;
                try {
                    await Policy.Handle<HttpRequestException> ()
                        .WaitAndRetryAsync(10, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)))
                        .ExecuteAsync(async() => {
                            _logger.LogDebug($"Submitting retry: {a++} next wait: {Math.Pow(2, a)} secs");
                            var result = await _processor.SubmitNewAudioItem(
                                url,
                                uid,
                                _callbackAddress);
                            if (result.StatusCode == HttpStatusCode.OK) {
                                _logger.LogDebug("Job server responded");
                            } else {
                                _logger.LogDebug("Job server did not respond");
                            }
                        });
                } catch (InvalidOperationException ex) {
                    _logger.LogError($"Polly doesn't want a cracker: {ex.Message}");
                } catch (Exception ex) {
                    _logger.LogError($"Polly doesn't want a cracker: {ex.Message}");
                }
            }
            return false;
        }
    }
}