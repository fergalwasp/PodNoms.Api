using System.Threading.Tasks;
using PodNoms.Api.Persistence;

namespace PodNoms.Api.Models {
    public class UnitOfWork : IUnitOfWork {
        private readonly PodnomsContext context;
        public UnitOfWork(PodnomsContext context) {
            this.context = context;

        }
        public async Task CompleteAsync() {
            await context.SaveChangesAsync();
        }
    }
}