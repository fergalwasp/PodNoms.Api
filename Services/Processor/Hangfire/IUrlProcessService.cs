using System.Threading.Tasks;

namespace PodNoms.Api.Services.Processor.Hangfire
{
    public interface IUrlProcessService
    {
        Task<bool> GetInformation(int entryId);
        Task<bool> DownloadAudio(int entryId);
    }
}