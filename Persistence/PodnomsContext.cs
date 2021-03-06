using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using PodNoms.Api.Models;

namespace PodNoms.Api.Persistence {

    public class PodnomsDbContext : DbContext {
        public PodnomsDbContext(DbContextOptions<PodnomsDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
            // modelBuilder.Entity<PodcastEntry>()
            //     .HasIndex(p => new { p.PodcastId, p.SourceUrl })
            //     .IsUnique(true);
        }

        public DbSet<Podcast> Podcasts { get; set; }
        public DbSet<PodcastEntry> PodcastEntries { get; set; }
        public DbSet<User> Users { get; set; }
    }
}