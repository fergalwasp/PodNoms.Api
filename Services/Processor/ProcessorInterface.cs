using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PodNoms.Api.Models;

namespace PodNoms.Api.Services.Processor
{
    public class ProcessorInterface : RestClient, IProcessorInterface
    {

        public ProcessorInterface(IOptions<AppSettings> options, ILoggerFactory loggerFactory) 
                : base(options, loggerFactory)
        {

        }

        public async Task<HttpResponseMessage> SubmitNewAudioItem(string url, string id, string callbackUrl)
        {
            var body = JsonConvert.SerializeObject(new {
                url = url,
                id = id,
                callbackUrl = callbackUrl
            });
            var path = "/api/processor";
            _logger.LogDebug($"Submitting to processor: {path}");
            return await _submitMessage(path, body);
        }
    }
}