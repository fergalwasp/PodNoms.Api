using System;
using System.ComponentModel;
using System.IO;
using Microsoft.Extensions.Configuration;
using NYoutubeDL;
using NYoutubeDL.Options;
using PodNoms.Api.Models;
using PodNoms.Api.Services.Downloader;
using PodNoms.Api.Utils.Pusher;
using static NYoutubeDL.Helpers.Enums;

namespace PodNoms.Api.Services.Processor.Hangfire {
    public interface IUrlProcessService {
        void ProcessUrl(string url);
    }
    public class UrlProcessService : IUrlProcessService {
        private readonly IPusherService _pusherService;
        private readonly IPodcastRepository _repository;

        public class ProcessObject {
            public string name { get; set; }
            public override string ToString() { return name; }
        }
        public UrlProcessService(IPusherService pusherService, IPodcastRepository repository) {
            this._pusherService = pusherService;
            this._repository = repository;
        }
        public void ProcessUrl(string url) {
            var guid = System.Guid.NewGuid().ToString();
            Console.WriteLine($"Processing started: {url}");
            var youtubeDl = new YoutubeDL();
            youtubeDl.Options.FilesystemOptions.Output = Path.Combine(System.IO.Path.GetTempPath(), "podnoms", $"{guid}.mp3");
            youtubeDl.Options.PostProcessingOptions.AudioFormat = AudioFormat.mp3;
            youtubeDl.Options.PostProcessingOptions.ExtractAudio = true;
            youtubeDl.VideoUrl = url;

            youtubeDl.StandardOutputEvent += (sender, output) => {
                Console.WriteLine(output);
            };
            youtubeDl.StandardErrorEvent += (sender, errorOutput) => {
                Console.WriteLine(errorOutput);
            };

            var downloadInfo = youtubeDl.GetDownloadInfo();

            youtubeDl.Info.PropertyChanged += (s, e) => {
                Console.WriteLine(e);
            };

            var download = youtubeDl.PrepareDownload();
            youtubeDl.Download().WaitForExit();

            //at this point (I'm guessing) audio should be downloaded

            Console.WriteLine("Audio extracted succesfully");
            _pusherService.Trigger(
                new PusherMessage {
                    name = "audio-processed",
                        channel = "jobs-channel",
                        data = new PusherPayload {
                            message = guid
                        }
                });
        }
    }
}