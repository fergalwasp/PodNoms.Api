using System.Collections.Generic;
using System.Threading.Tasks;

namespace PodNoms.Api.Models {

    public interface IPodcastRepository {
        Task<Podcast> GetAsync(int id);
        Task<Podcast> GetAsync(string slug);
        Task<IEnumerable<Podcast>> GetAllAsync();
        Task<IEnumerable<Podcast>> GetAllAsync(string emailAddress);
        Task<Podcast> AddOrUpdateAsync(Podcast item);
        Task<int> DeleteAsync(int id);
    }
}