using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace PodNoms.Api.Services.Storage
{
    public interface IFileStorage
    {
        Task<string> StoreItem(string uploadFolderPath, string container, IFormFile file);
    }
}