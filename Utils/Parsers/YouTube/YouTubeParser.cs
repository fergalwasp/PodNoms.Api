using System;
using System.Diagnostics;
using Newtonsoft.Json;

namespace PodNoms.Api.Utils.Parsers.YouTube
{
    public class YouTubeParser : Parser, IAudioParser
    {
        private const string CONVERT_PROGRAM = "youtube-dl";
        private string _url;
        private string _audioFile;
        private RootObject _informationObject;
        public YouTubeParser(string url)
        {
            _url = url;
            _audioFile = string.Empty;
        }

        public bool RunParser()
        {
            Process p = _startProcess(CONVERT_PROGRAM, $"--dump-json {_url}");
            var json = _outputProcess(p);
            try
            {
                _informationObject = JsonConvert.DeserializeObject<RootObject>(json);
                return _informationObject != null;
            }
            catch (JsonReaderException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (JsonSerializationException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }

        public string GetTitle()
        {
            return _informationObject.title;
        }

        public string GetAuthor()
        {
            return _informationObject.uploader;
        }

        public string GetDescription()
        {
            return _informationObject.description;
        }
        public string GetImage()
        {
            return _informationObject.thumbnail;
        }
        public string GetAudio()
        {
            return _audioFile;
        }
        /*
public string GetAudio()
{
   var outputFile = $"{System.IO.Path.GetTempPath()}{System.Guid.NewGuid().ToString()}.mp3";
   var arguments = $" --extract-audio --audio-format mp3 -o \"{outputFile}.%(ext)s\" {_url}";

   Process proc = _startProcess(CONVERT_PROGRAM, arguments);
   Console.WriteLine(_outputProcess(proc));
   return outputFile;
}
public string GetAuthor()
{
   var proc = _startProcess("youtube-dl", $"--get-description {_url}");
   return _outputProcess(proc);
}
public string GetDescription()
{
   var proc = _startProcess("youtube-dl", $"--get-description {_url}");
   return _outputProcess(proc);
}
public string GetImage()
{
   var proc = _startProcess("youtube-dl", $"--get-thumbnail  {_url}");
   return _outputProcess(proc);
}
public string GetTitle()
{
   var proc = _startProcess("youtube-dl", $"--get-title {_url}");
   return _outputProcess(proc);
}*/
    }
}