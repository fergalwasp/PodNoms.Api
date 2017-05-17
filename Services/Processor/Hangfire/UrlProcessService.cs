using System;
using System.ComponentModel;
using System.IO;
using Microsoft.Extensions.Configuration;
using NYoutubeDL;
using NYoutubeDL.Options;
using PodNoms.Api.Models;
using PodNoms.Api.Services.Downloader;
using PusherServer;
using static NYoutubeDL.Helpers.Enums;

namespace PodNoms.Api.Services.Processor.Hangfire {
    public interface IUrlProcessService {
        void ProcessUrl(string uid, string url);
    }
    public class UrlProcessService : IUrlProcessService {
        private readonly IPodcastRepository _repository;

        public class ProcessObject {
            public string name { get; set; }
            public override string ToString() { return name; }
        }
        public UrlProcessService(IPodcastRepository repository) {
            this._repository = repository;
        }
        public void ProcessUrl(string uid, string url) {
            var guid = System.Guid.NewGuid().ToString();
            Console.WriteLine($"Processing started: {url}");
            var youtubeDl = new YoutubeDL();
            
            youtubeDl.Options.FilesystemOptions.Output = Path.Combine(System.IO.Path.GetTempPath(), "podnoms", $"{guid}.mp3");
            youtubeDl.Options.PostProcessingOptions.AudioFormat = AudioFormat.mp3;
            youtubeDl.Options.PostProcessingOptions.ExtractAudio = true;
            youtubeDl.Options.ThumbnailImagesOptions.ListThumbnails = true;
            youtubeDl.Options.ThumbnailImagesOptions.WriteAllThumbnails = true;
            youtubeDl.Options.ThumbnailImagesOptions.WriteThumbnail = true;

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
            var options = new PusherOptions();
            options.Cluster = "eu";
            var entry = _repository.GetEntryByUid(uid);
            if (entry != null) {
                entry.Processed = true;
                entry.Title = downloadInfo.Title;

                var pusher = new Pusher("242694", "80e33149d1e70ae7907a", "2c0aca674b8216f5629e", options);
                pusher.TriggerAsync($"{uid}__process_podcast", "audio-processed", new {
                    message = downloadInfo.Title
                });
            }
        }
    }
}