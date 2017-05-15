using System.Collections.Generic;
using System.Threading.Tasks;

namespace PodNoms.Api.Models
{
    public interface IPodcastRepository
    {
        IEnumerable<Podcast> GetAll();
        IEnumerable<Podcast> GetAll(string UserId);
        Podcast Get(int id);
        Podcast Get(string slug);
        Task<Podcast> AddOrUpdateAsync(Podcast item);
        string UpdateImageData(int PodcastId, string ImageData);

        IEnumerable<PodcastEntry> GetAllEntries(int PodcastId);
        IEnumerable<PodcastEntry> GetAllEntries();
        PodcastEntry GetEntry(int id);
        PodcastEntry GetEntry(string slug);
        PodcastEntry GetEntryByUid(string uid);
        PodcastEntry AddEntry(int PodcastId, PodcastEntry item);
        Task<PodcastEntry> AddEntryAsync(int PodcastId, PodcastEntry item);
        PodcastEntry AddOrUpdateEntry(PodcastEntry item);
        Task<PodcastEntry> AddOrUpdateEntryAsync(PodcastEntry item);
        int Delete(int id);
        int DeleteEntry(int id);
        int DeleteEntry(string slug);
        Task<List<PodcastEntry>> GetQueuedEntriesAsync();
        List<PodcastEntry> GetEntryByStatus(ProcessingStatus status);
        Task<PodcastEntry> GetEntryAsync(int id);
    }
}
