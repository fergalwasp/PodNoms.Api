namespace PodNoms.Api.Utils.Parsers
{
    interface IAudioParser
    {
        bool RunParser();
        string GetTitle();
        string GetAuthor();
        string GetDescription();
        string GetImage();
        string GetAudio();
    }
}