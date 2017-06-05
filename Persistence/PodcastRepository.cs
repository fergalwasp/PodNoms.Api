using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PodNoms.Api.Models;
using PodNoms.Api.Persistence;
using PodNoms.Api.Utils;
using PodNoms.Api.Utils.Extensions;

namespace PodNoms.Api.Models {
    public class PodcastRepository : IPodcastRepository {
        private readonly PodnomsDbContext context;
        public PodcastRepository(PodnomsDbContext context) {
            this.context = context;
        }
        public async Task<Podcast> GetAsync(int id) {
            var ret = await context.Podcasts
                .Where(p => p.Id == id)
                .Include(e => e.PodcastEntries)
                .FirstOrDefaultAsync();

            return ret;
        }
        public async Task<Podcast> GetAsync(string slug) {
            var ret = await context.Podcasts
                .Where(p => p.Slug == slug)
                .Include(e => e.PodcastEntries)
                .FirstOrDefaultAsync();

            return ret;
        }
        public async Task<IEnumerable<Podcast>> GetAllAsync() {
            var ret = context.Podcasts
                .Include(e => e.PodcastEntries)
                .OrderByDescending(e => e.Id);

            return await ret.ToListAsync();
        }
        public async Task<IEnumerable<Podcast>> GetAllAsync(string emailAddress) {
            var ret = context.Podcasts
                .Where(u => u.User.EmailAddress == emailAddress)
                .Include(e => e.PodcastEntries)
                .OrderByDescending(e => e.Id);
            return await ret.ToListAsync();
        }
        public async Task<Podcast> AddOrUpdateAsync(Podcast item) {
            if (item.Id != 0) {
                context.Podcasts.Attach(item);
                context.Entry(item).State = EntityState.Modified;
            } else {
                item.ImageUrl = await ImageUtils.GetRemoteImageAsBase64($"http://lorempixel.com/400/200/?{System.Guid.NewGuid().ToString()}");
                context.Podcasts.Add(item);
            }
            return item;
        }
        public async Task<int> DeleteAsync(int id) {
            var podcast = await context.Podcasts.SingleAsync(p => p.Id == id);
            if (podcast != null) {
                if (podcast.PodcastEntries != null) {
                    foreach(var entry in podcast.PodcastEntries) {
                        context.Remove(entry);
                    }
                }
                context.Remove<Podcast>(podcast);
            }
            return -1;
        }
    }
}