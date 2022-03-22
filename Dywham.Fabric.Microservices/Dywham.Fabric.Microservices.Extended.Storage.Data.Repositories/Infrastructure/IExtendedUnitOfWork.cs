using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dywham.Fabric.Data.Repositories;
using Dywham.Fabric.Data.Repositories.EntityFramework;
using Dywham.Fabric.Data.Repositories.EntityFramework.Infrastructure;
using Dywham.Fabric.Microservices.Extended.Storage.Data.Repositories.Entities;

namespace Dywham.Fabric.Microservices.Extended.Storage.Data.Repositories.Infrastructure
{
    public interface IExtendedUnitOfWork<in TC> : IEfDatabaseContextUnitOfWork where TC : ExtendedDatabaseContext
    {
        Task UpdateOperationalExecutionAsync(Func<OperationalEntity, OperationalEntity> func, CancellationToken token = default);

        Task<TN> UpdateOperationalExecutionAsync<TN>(Func<TN, TN> func, CancellationToken token = default) where TN : class;

        IEfRepository<TR, TC> GetRepositoryFor<TR>() where TR : class, new();

        IExtendedRepository<TR, TC> GetExtendedRepositoryFor<TR>() where TR : ExtendedEntity, new();

        TR GetRepository<TR>() where TR : class, IRepository;

        Task EnsureUniquenessAsync(string key, CancellationToken token = default);

        Task EnsureUniquenessAsync(List<string> keys, CancellationToken token = default);

        Task DeleteUniquenessAsync(string key, CancellationToken token = default);

        Task DeleteUniquenessAsync(List<string> keys, CancellationToken token = default);

        Task<bool> VerifyUniquenessExistsAsync(string key, bool create, CancellationToken token = default);

        Task<bool> VerifyUniquenessExistsAsync(List<string> keys, bool create, CancellationToken token = default);

        Task<int> SaveChangesAsync();
    }
}