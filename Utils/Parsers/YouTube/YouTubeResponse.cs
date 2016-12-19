using System.Collections.Generic;

namespace PodNoms.Api.Utils.Parsers.YouTube
{
    internal sealed class HttpHeaders
    {
        public string __invalid_name__AcceptCharset { get; set; }
        public string __invalid_name__AcceptLanguage { get; set; }
        public string __invalid_name__AcceptEncoding { get; set; }
        public string Accept { get; set; }
        public string __invalid_name__UserAgent { get; set; }
    }
    internal sealed class RequestedFormat
    {
        public int? asr { get; set; }
        public double tbr { get; set; }
        public string protocol { get; set; }
        public object language { get; set; }
        public string format { get; set; }
        public string url { get; set; }
        public string vcodec { get; set; }
        public string format_note { get; set; }
        public int? preference { get; set; }
        public int? height { get; set; }
        public int? width { get; set; }
        public string ext { get; set; }
        public int? filesize { get; set; }
        public int? fps { get; set; }
        public string format_id { get; set; }
        public HttpHeaders http_headers { get; set; }
        public string acodec { get; set; }
        public int? abr { get; set; }
    }
    internal sealed class Subtitles
    {
    }
    internal sealed class AutomaticCaptions
    {
    }
    internal sealed class Thumbnail
    {
        public string url { get; set; }
        public string id { get; set; }
    }
    internal sealed class Format
    {
        public int? asr { get; set; }
        public double tbr { get; set; }
        public string protocol { get; set; }
        public string format { get; set; }
        public string format_note { get; set; }
        public int? height { get; set; }
        public int? preference { get; set; }
        public string format_id { get; set; }
        public object language { get; set; }
        public HttpHeaders http_headers { get; set; }
        public string url { get; set; }
        public string vcodec { get; set; }
        public int? abr { get; set; }
        public int? width { get; set; }
        public string ext { get; set; }
        public int? filesize { get; set; }
        public int? fps { get; set; }
        public string acodec { get; set; }
        public string container { get; set; }
        public object player_url { get; set; }
        public string resolution { get; set; }
    }
    internal sealed class RootObject
    {
        public string upload_date { get; set; }
        public object creator { get; set; }
        public int? height { get; set; }
        public int? like_count { get; set; }
        public int? duration { get; set; }
        public string fulltitle { get; set; }
        public string id { get; set; }
        public List<RequestedFormat> requested_formats { get; set; }
        public int? view_count { get; set; }
        public object playlist { get; set; }
        public string title { get; set; }
        public string _filename { get; set; }
        public string format { get; set; }
        public string ext { get; set; }
        public object playlist_index { get; set; }
        public int? dislike_count { get; set; }
        public double average_rating { get; set; }
        public int? abr { get; set; }
        public string uploader_url { get; set; }
        public Subtitles subtitles { get; set; }
        public int? fps { get; set; }
        public object stretched_ratio { get; set; }
        public int? age_limit { get; set; }
        public object annotations { get; set; }
        public string webpage_url_basename { get; set; }
        public string acodec { get; set; }
        public string display_id { get; set; }
        public AutomaticCaptions automatic_captions { get; set; }
        public string description { get; set; }
        public List<string> tags { get; set; }
        public object requested_subtitles { get; set; }
        public object start_time { get; set; }
        public string uploader { get; set; }
        public string format_id { get; set; }
        public string uploader_id { get; set; }
        public List<string> categories { get; set; }
        public List<Thumbnail> thumbnails { get; set; }
        public string license { get; set; }
        public object alt_title { get; set; }
        public string extractor_key { get; set; }
        public string vcodec { get; set; }
        public string thumbnail { get; set; }
        public object vbr { get; set; }
        public object is_live { get; set; }
        public string extractor { get; set; }
        public object end_time { get; set; }
        public string webpage_url { get; set; }
        public List<Format> formats { get; set; }
        public object resolution { get; set; }
        public int? width { get; set; }
    }
}