using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PodNoms.Api.Models;

namespace PodNoms.Api.Persistence {
    public class EntryRepository : IEntryRepository {
        private readonly PodnomsDbContext context;

        public EntryRepository(PodnomsDbContext context) {
            this.context = context;

        }
        public async Task<PodcastEntry> GetAsync(int id) {
            var entry = await context.PodcastEntries
                .SingleOrDefaultAsync(e => e.Id == id);
            return entry;
        }
        public async Task<PodcastEntry> AddAsync(PodcastEntry entry) {
            var result = await context.PodcastEntries.AddAsync(entry);
            return entry;
        }
        public async Task DeleteAsync(int id) {
            var entry = await GetAsync(id);
            context.Remove(entry);
        }
    }
}