using System.Threading.Tasks;

namespace PodNoms.Api.Services
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}
