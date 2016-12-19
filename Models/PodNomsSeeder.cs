using Microsoft.AspNetCore.Builder;

namespace PodNoms.Api.Models
{
    public static class DataSeeder
    {
        // TODO: Move this code when seed data is implemented in EF 7

        /// <summary>
        /// This is a workaround for missing seed data functionality in EF 7.0-rc1
        /// More info: https://github.com/aspnet/EntityFramework/issues/629
        /// </summary>
        /// <param name="app">
        /// An instance that provides the mechanisms to get instance of the database context.
        /// </param>
        public static void SeedData(this IApplicationBuilder app)
        {
/*            
            var db = app.ApplicationServices.GetService<PodnomsContext>();

            if (db.Podcasts.Count() == 0)
            {

                db.PodcastEntries.RemoveRange(db.Set<PodcastEntry>());
                db.Podcasts.RemoveRange(db.Set<Podcast>());

                for (var i = 1; i <= 10; i++)
                {
                    var podcast = new Podcast
                    {
                        Title = $"Podcast {HumanFriendlyInteger.IntegerToWritten(i)}",
                        Description = Randomisers.LoremIpsum(15, 100, 1, 10, 1),
                        ImageUrl = $"http://lorempixel.com/400/200/400x200?{i}"
                    };
                    db.Podcasts.UpdateRegistration(podcast);
                }
                db.SaveChanges();
            }
*/
        }
    }
}