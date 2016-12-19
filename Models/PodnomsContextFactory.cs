using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace PodNoms.Api.Models
{
    public class PodnomsContextFactory : IDbContextFactory<PodnomsContext>
    {
        public PodnomsContext Create(DbContextFactoryOptions options)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PodnomsContext>();
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            var configuration = builder.Build();

            var connectionString = configuration.GetConnectionString("Podnoms");
            optionsBuilder.UseSqlServer(connectionString);

            // Ensure that the SQLite database and sechema is created!
            var context = new PodnomsContext(optionsBuilder.Options);
            context.Database.EnsureCreated();
            return context;
        }
    }
}