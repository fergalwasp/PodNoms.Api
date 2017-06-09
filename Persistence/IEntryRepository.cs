using System.Collections.Generic;
using System.Threading.Tasks;
using PodNoms.Api.Models;

namespace PodNoms.Api.Persistence
{
    public interface IEntryRepository
    {
        Task<PodcastEntry> GetAsync(int id);
        Task<IEnumerable<PodcastEntry>> GetAllAsync(int podcastId);
        Task<PodcastEntry> AddAsync(PodcastEntry entry);
        Task DeleteAsync(int id);
    }
}