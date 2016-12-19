using System.Net.Http;
using System.Threading.Tasks;

namespace PodNoms.Api.Services.Processor
{
    public interface IProcessorInterface{
        Task<HttpResponseMessage> SubmitNewAudioItem(string url, string id, string callbackUrl);
    }
}