using System.Threading.Tasks;
using PodNoms.Api.Persistence;

namespace PodNoms.Api.Models {
    public class UnitOfWork : IUnitOfWork {
        private readonly PodnomsDbContext context;
        public UnitOfWork(PodnomsDbContext context) {
            this.context = context;

        }
        public async Task CompleteAsync() {
            await context.SaveChangesAsync();
        }
    }
}