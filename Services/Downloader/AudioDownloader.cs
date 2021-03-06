using System;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace PodNoms.Api.Services.Downloader
{
    public class AudioDownloader : IDisposable
    {
        private readonly string url;
        public dynamic Properties { get; private set; }
        protected const string DOWNLOADRATESTRING = "iB/s";
        protected const string DOWNLOADSIZESTRING = "iB";
        protected const string ETASTRING = "ETA";
        protected const string OFSTRING = "of";

        public event EventHandler<DownloadProgress> DownloadProgress;
        public event EventHandler<String> PostProcessing;
        public AudioDownloader(string url)
        {
            this.url = url;
        }
        public void Dispose()
        {

            var StartInfo = new ProcessStartInfo
            {
                FileName = "pinfo",
                Arguments = $"argie bargie",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

        }
        public (string, string) DownloadAudio()
        {
            var outputFileName = $"{Guid.NewGuid().ToString()}.mp3";
            var outputFile = Path.Combine(Path.GetTempPath(), outputFileName);
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "youtube-dl",
                    Arguments = $"-o {outputFile} --audio-format mp3 -x \"{this.url}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            StringBuilder br = new StringBuilder();
            proc.Start();
            while (!proc.StandardOutput.EndOfStream)
            {
                string output = proc.StandardOutput.ReadLine();
                if (output.Contains("%"))
                {
                    var progress = _parseProgress(output);
                    if (DownloadProgress != null)
                    {
                        DownloadProgress(this, progress);
                    }
                }
                else
                {
                    if (PostProcessing != null)
                    {
                        PostProcessing(this, output);
                    }
                }
            }

            if (File.Exists(outputFile))
            {
                return (outputFile, outputFileName);
            }
            return (string.Empty, string.Empty);
        }

        public void DownloadInfo()
        {
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "youtube-dl",
                    Arguments = $"-j {this.url}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            StringBuilder br = new StringBuilder();
            proc.Start();
            while (!proc.StandardOutput.EndOfStream)
            {
                br.Append(proc.StandardOutput.ReadLine());
            }
            Properties = JsonConvert.DeserializeObject<ExpandoObject>(br.ToString());
        }

        private DownloadProgress _parseProgress(string output)
        {

            var result = new DownloadProgress();

            int progressIndex = output.LastIndexOf(' ', output.IndexOf('%')) + 1;
            string progressString = output.Substring(progressIndex, output.IndexOf('%') - progressIndex);
            result.Percentage = (int)Math.Round(double.Parse(progressString));

            int sizeIndex = output.LastIndexOf(' ', output.IndexOf(DOWNLOADSIZESTRING)) + 1;
            string sizeString = output.Substring(sizeIndex, output.IndexOf(DOWNLOADSIZESTRING) - sizeIndex + 2);
            result.TotalSize = sizeString;

            if (output.Contains(DOWNLOADRATESTRING))
            {
                int rateIndex = output.LastIndexOf(' ', output.LastIndexOf(DOWNLOADRATESTRING)) + 1;
                string rateString = output.Substring(rateIndex, output.LastIndexOf(DOWNLOADRATESTRING) - rateIndex + 4);
                result.CurrentSpeed = rateString;
            }

            if (output.Contains(ETASTRING))
            {
                result.ETA = output.Substring(output.LastIndexOf(' ') + 1);
            }
            return result;
        }
    }
}