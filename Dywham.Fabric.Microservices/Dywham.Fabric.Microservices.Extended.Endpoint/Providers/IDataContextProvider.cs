using System.Threading;
using System.Threading.Tasks;
using Dywham.Fabric.Data.Repositories;
using Dywham.Fabric.Data.Repositories.EntityFramework;
using Dywham.Fabric.Microservices.Extended.Storage.Data.Repositories;
using Dywham.Fabric.Microservices.Extended.Storage.Data.Repositories.Entities;
using Dywham.Fabric.Microservices.Extended.Storage.Data.Repositories.Infrastructure;

namespace Dywham.Fabric.Microservices.Extended.Endpoint.Providers
{
    public interface IDataContextProvider<TC> where TC : ExtendedDatabaseContext
    {
        Task<IExtendedUnitOfWork<TC>> RunInAUnitOfWorkAsync(CancellationToken token = default);

        TR GetRepository<TR>() where TR : class, IRepository;

        IEfRepository<TR, TC> GetRepositoryFor<TR>() where TR : class, new();

        IExtendedRepository<TR, TC> GetExtendedRepositoryFor<TR>() where TR : ExtendedEntity, new();
    }
}