using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dywham.Fabric.Data.Repositories.EntityFramework.Infrastructure
{
    public interface IEfDatabaseContextUnitOfWork : IDisposable
    {
        Task InitAsync(CancellationToken cancellationToken);

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}