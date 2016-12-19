namespace PodNoms.Api.Models
{
    using Microsoft.EntityFrameworkCore;

    public class PodnomsContext : DbContext //IdentityDbContext<ApplicationUser>
    {
        public PodnomsContext(DbContextOptions<PodnomsContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Podcast> Podcasts { get; set; }
        public DbSet<PodcastEntry> PodcastEntries { get; set; }
        public DbSet<User> Users { get; set; }
    }
}