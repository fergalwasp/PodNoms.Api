using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PodNoms.Api.Models;

namespace PodNoms.Api.Services
{
    public abstract class RestClient
    {
        HttpClientHandler ____proxy_handler___;
        private readonly IOptions<AppSettings> _options;
        protected readonly ILogger _logger;
        private readonly Uri _baseUri;
        protected RestClient(IOptions<AppSettings> options, ILoggerFactory loggerFactory)
        {
            _options = options;
            _logger = loggerFactory.CreateLogger<RestClient>();
            if (_options.Value.UseProxy)
            {
                ____proxy_handler___ = new HttpClientHandler
                {
                    UseCookies = true,
                    UseDefaultCredentials = false,
                    Proxy = new RestProxy("http://localhost:5555"),
                    UseProxy = true,
                };
            }
            _logger.LogDebug($"Processor: {_options.Value.ProcessorServerUrl}");
            _baseUri = new Uri(_options.Value.ProcessorServerUrl);
        }

        protected async Task<HttpResponseMessage> _submitMessage(string path, string body)
        {
            _logger.LogDebug($"RestClient: {_baseUri}{path}");
            using (var client = _options.Value.UseProxy ? new HttpClient(____proxy_handler___) : new HttpClient())
            {
                client.BaseAddress = _baseUri;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage result = await client.PostAsync(
                    path,
                    new StringContent(
                        body,
                        Encoding.UTF8,
                        "application/json"));
                _logger.LogDebug("RestClient: Awaiting result");
                if (result.IsSuccessStatusCode)
                {
                    _logger.LogDebug($"RestClient: Result from processor: {result.StatusCode}");
                }
                else
                {
                    _logger.LogDebug($"RestClient: Error sending to processor: {result.StatusCode}\n{result.ReasonPhrase}");
                    _logger.LogError($"RestClient: Error sending to processor: {result.StatusCode}\n{result.ReasonPhrase}");
                }
                _logger.LogDebug("RestClient: Returning");
                return result;
            };
        }
    }
}