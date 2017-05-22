using Microsoft.EntityFrameworkCore;
using PodNoms.Api.Models;

namespace PodNoms.Api.Persistence {

    public class PodnomsContext : DbContext {
        public PodnomsContext(DbContextOptions<PodnomsContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
        }
        /*
            modelBuilder.Entity<PodcastEntry>()
                .HasIndex(p => new { p.PodcastId, p.SourceUrl })
                .IsUnique(true);
        }
        */
        public DbSet<Podcast> Podcasts { get; set; }
        public DbSet<User> Users { get; set; }
    }
}