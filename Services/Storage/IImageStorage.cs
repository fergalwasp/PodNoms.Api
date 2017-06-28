using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace PodNoms.Api.Services.Storage
{
    public interface IImageStorage
    {
        Task<string> StoreImage(string uploadFolderPath, IFormFile file);
    }
}