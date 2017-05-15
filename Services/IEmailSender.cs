using System.Threading.Tasks;

namespace PodNoms.Api.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
