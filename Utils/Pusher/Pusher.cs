using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PodNoms.Api.Models;
using PodNoms.Api.Services;
using PodNoms.Api.Utils.Crypt;

namespace PodNoms.Api.Utils.Pusher {
    public interface IPusherService {
        Task<HttpResponseMessage> Trigger(PusherMessage message);
    }
    public class PusherService : RestClient, IPusherService {
        private readonly PusherOptions _options;
        public PusherService(IOptions<AppSettings> settings, ILoggerFactory loggerFactory) : base(settings, loggerFactory) {
            _options = new PusherOptions {
                Cluster = "eu",
                    Encrypted = true
            };
        }

        public async Task<HttpResponseMessage> Trigger(PusherMessage message) {
            var body = JsonConvert.SerializeObject(message);
            var md5 = MD5Generator.CalculateMD5Hash(body);
            var KEY = "8a3501faef6636ca9a5ebbe6f31b5409";
            var timestamp = DateUtils.ConvertToUnixTimestamp(DateTime.Now);
            var hmac = HMACGenerator.CalculateHMACSHA256("7ad3773142a6692b25b8",
                $"POST\n/apps/242694/events\n" +
                $"auth_key={KEY}" +
                $"auth_timestamp={timestamp}&" +
                $"auth_version=1.0&" +
                $"body_md5={md5}");

            var uri = $"https://api-eu.pusher.com/apps/242694/events?" +
                $"body_md5={md5}&" +
                $"auth_version=1.0&" +
                $"auth_key={KEY}&" +
                $"auth_timestamp={timestamp}&" +
                $"auth_signature=8a3501faef6636ca9a5ebbe6f31b5409";

            _logger.LogDebug($"Sending pusher to: {uri}");
            return await _submitMessage(uri, body);
        }
    }
}