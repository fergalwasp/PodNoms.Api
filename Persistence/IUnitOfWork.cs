using System;
using System.Threading.Tasks;

namespace PodNoms.Api.Models {
    public interface IUnitOfWork {
        Task CompleteAsync();
    }
}