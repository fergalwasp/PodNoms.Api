using System.Threading.Tasks;

namespace PodNoms.Api.Services.Realtime
{
    public interface IRealTimeUpdater
    {
        Task<bool> SendMessage(string channelName, string eventName, object data);
    }
}