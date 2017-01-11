using System.Collections.Generic;
using PodNoms.Api.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PodNoms.Api.Utils;

public class PodcastRepository : IPodcastRepository
{
    private readonly PodnomsContext _context;

    public PodcastRepository(PodnomsContext context)
    {
        _context = context;
    }

    public IEnumerable<Podcast> GetAll()
    {
        var ret = _context.Podcasts
                  .Include(e => e.PodcastEntries)
                  .OrderByDescending(e => e.Id);

        return ret.ToList();
    }

    public IEnumerable<Podcast> GetAll(string UserId)
    {
        var ret = _context.Podcasts
                  .Where(u => u.User.EmailAddress == UserId)
                  .Include(e => e.PodcastEntries)
                  .OrderByDescending(e => e.Id);

        return ret.ToList();
    }

    public Podcast Get(int id)
    {
        var ret = _context.Podcasts
            .Where(p => p.Id == id)
            .Include(e => e.PodcastEntries)
            .FirstOrDefault();

        return ret;
    }

    public Podcast Get(string slug)
    {
        var ret = _context.Podcasts
            .Where(p => p.Slug == slug)
            .Include(e => e.PodcastEntries)
            .FirstOrDefault();

        return ret;
    }

    public async Task<Podcast> AddOrUpdate(Podcast item)
    {
        if (item.Id != 0)
        {
            _context.Podcasts.Attach(item);
        }
        else
        {
            item.ImageUrl = await ImageUtils.GetRemoteImageAsBase64($"http://lorempixel.com/400/200/?{System.Guid.NewGuid().ToString()}");
            _context.Podcasts.Add(item);
        }
        _context.SaveChanges();
        return item;
    }

    public string UpdateImageData(int PodcastId, string ImageData)
    {
        var podcast = _context.Podcasts.First(p => p.Id == PodcastId);

        if (podcast != null)
        {
            podcast.ImageUrl = ImageData;
            _context.SaveChanges();
            return ImageData;
        }
        return string.Empty;
    }

    public IEnumerable<PodcastEntry> GetAllEntries(int PodcastId)
    {
        var entries = _context.PodcastEntries
            .Where(e => e.Id == PodcastId)
            .AsEnumerable();
        return entries;
    }
    public IEnumerable<PodcastEntry> __getAllEntries()
    {
        var entries = _context.PodcastEntries
            .AsEnumerable();
        return entries;
    }

    public PodcastEntry GetEntry(int id)
    {
        var entry = _context.PodcastEntries
            .Where(e => e.Id == id)
            .Include(p => p.Podcast)
            .FirstOrDefault();
        return entry;
    }
    public PodcastEntry GetEntry(string slug)
    {
        var entry = _context.PodcastEntries
            .Where(e => e.Slug == slug)
            .Include(p => p.Podcast)
            .FirstOrDefault();
        return entry;
    }
    public PodcastEntry GetEntryByUid(string uid)
    {
        var entry = _context.PodcastEntries
            .FirstOrDefault(e => e.Uid == uid);
        return entry;
    }

    public PodcastEntry AddEntry(int PodcastId, PodcastEntry item)
    {
        var podcast = _context.Podcasts
            .FirstOrDefault(p => p.Id == PodcastId);

        if (podcast != null)
        {
            if (podcast.PodcastEntries == null)
            {
                podcast.PodcastEntries = new List<PodcastEntry>();
            }

            podcast.PodcastEntries.Add(item);
            _context.SaveChanges();

            return item;
        }
        return null;
    }
    public PodcastEntry AddOrUpdateEntry(PodcastEntry entry)
    {
        if (entry.Id != 0)
        {
            _context.PodcastEntries.Attach(entry);
        }
        else
        {
            _context.PodcastEntries.Add(entry);

        }
        _context.SaveChanges();
        return entry;
    }

    public int Delete(int id)
    {
        var podcast = _context.Podcasts.FirstOrDefault(p => p.Id == id);
        if (podcast != null)
        {
            foreach (var entry in podcast.PodcastEntries)
            {
                _context.Remove(entry);
            }
            _context.Remove<Podcast>(podcast);
            return _context.SaveChanges();
        }
        return -1;
    }
    public int DeleteEntry(int id)
    {
        var podcast = _context.PodcastEntries.FirstOrDefault(p => p.Id == id);
        if (podcast != null)
        {
            _context.Remove<PodcastEntry>(podcast);
            return _context.SaveChanges();
        }
        return -1;
    }
    public int DeleteEntry(string slug)
    {
        var podcast = _context.PodcastEntries.FirstOrDefault(p => p.Slug == slug);
        if (podcast != null)
        {
            _context.Remove<PodcastEntry>(podcast);
            return _context.SaveChanges();
        }
        return -1;
    }
}
