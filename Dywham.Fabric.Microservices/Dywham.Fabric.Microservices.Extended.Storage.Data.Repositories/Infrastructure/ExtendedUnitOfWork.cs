using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Dywham.Fabric.Data.Repositories;
using Dywham.Fabric.Data.Repositories.EntityFramework;
using Dywham.Fabric.Data.Repositories.EntityFramework.Infrastructure;
using Dywham.Fabric.Microservices.Extended.Storage.Data.Repositories.Entities;
using Dywham.Fabric.Providers;
using Dywham.Fabric.Providers.Serialization.Json;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace Dywham.Fabric.Microservices.Extended.Storage.Data.Repositories.Infrastructure
{
    public class ExtendedUnitOfWork<T> : EfDatabaseContextUnitOfWork<IEfDatabaseContextFactory<T>, T>, IExtendedUnitOfWork<T> where T : ExtendedDatabaseContext
    {
        // ReSharper disable once StaticMemberInGenericType
        private readonly object _lock = new();
        private readonly Dictionary<Type, IRepository> _repositories = new();


        public ExtendedUnitOfWork(IEfDatabaseContextFactory<T> databaseContextFactory) : base(databaseContextFactory)
        { }


        public IDateTimeProvider DateTimeProvider { get; set; }

        public IComponentContext ComponentContext { get; set; }

        public IJsonProvider JsonProvider { get; set; }

        protected override IsolationLevel IsolationLevel => IsolationLevel.ReadCommitted;


        public Task EnsureUniquenessAsync(string key, CancellationToken token = default)
        {
            return EnsureUniquenessAsync(new List<string> {key}, token);
        }

        public async Task EnsureUniquenessAsync(List<string> keys, CancellationToken token = default)
        {
            var dateTime = DateTimeProvider.GetUtcNow();

            try
            {
                // ReSharper disable once InconsistentlySynchronizedField
                await DbContext.UniquenessGuards.AddRangeAsync(keys.Select(x => new UniquenessGuardEntity
                {
                    UniqueKey = x.ToLower(),
                    DateTime = dateTime
                }).ToList(), token);
            }
            catch (Exception ex)
            {
                // ReSharper disable once ConvertIfStatementToSwitchStatement
                if (ex is DbUpdateConcurrencyException)
                {
                    throw new DBConcurrencyException("Optimistic concurrency control failed, retry.");
                }

                // ReSharper disable once MergeIntoPattern
                if (ex is DbUpdateException dbUpdateEx && dbUpdateEx.InnerException?.InnerException != null &&
                    dbUpdateEx.InnerException.InnerException is SqlException sqlException)
                {
                    switch (sqlException.Number)
                    {
                        case 2627: // Unique constraint error
                        case 547: // Constraint check violation
                        case 2601: // Duplicated key row error
                                   // Constraint violation exception
                                   // A custom exception of yours for concurrency issues

                                   throw new DBConcurrencyException("Optimistic concurrency control failed, retry.");

                        default:

                            throw new DBConcurrencyException("Optimistic concurrency control failed, retry.");
                    }
                }

                throw;
            }
        }

        public Task<bool> VerifyUniquenessExistsAsync(string key, bool create, CancellationToken token = default)
        {
            return VerifyUniquenessExistsAsync(new List<string> {key}, create, token);
        }
        
        public async Task<bool> VerifyUniquenessExistsAsync(List<string> keys, bool create, CancellationToken token = default)
        {
            var exists = await DbContext.UniquenessGuards.AnyAsync(x => keys.Contains(x.UniqueKey), token);

            if (exists) return true;

            await EnsureUniquenessAsync(keys, token);

            return false;
        }

        public Task DeleteUniquenessAsync(string key, CancellationToken token = default)
        {
            // ReSharper disable once InconsistentlySynchronizedField
            return DbContext.UniquenessGuards.Where(x => key.ToLower() == x.UniqueKey).DeleteAsync(token);
        }

        public Task DeleteUniquenessAsync(List<string> keys, CancellationToken token = default)
        {
            keys = keys.Select(x => x.ToLower()).ToList();

            // ReSharper disable once InconsistentlySynchronizedField
            return DbContext.UniquenessGuards.Where(x => keys.Contains(x.UniqueKey)).DeleteAsync(token);
        }

        public Task<int> SaveChangesAsync()
        {
            return SaveChangesAsync(CancellationToken.None);
        }

        public IEfRepository<TR, T> GetRepositoryFor<TR>() where TR : class, new()
        {
            lock (_lock)
            {
                if (_repositories.ContainsKey(typeof(TR))) return _repositories[typeof(TR)] as IEfRepository<TR, T>;

                var repository = ComponentContext.Resolve<IEfRepository<TR, T>>();

                repository.UseDbContext(DbContext);

                _repositories.Add(typeof(TR), repository);

                return _repositories[typeof(TR)] as IEfRepository<TR, T>;
            }
        }

        public IExtendedRepository<TR, T> GetExtendedRepositoryFor<TR>() where TR : ExtendedEntity, new()
        {
            lock (_lock)
            {
                if (_repositories.ContainsKey(typeof(TR))) return _repositories[typeof(TR)] as IExtendedRepository<TR, T>;

                var repository = ComponentContext.Resolve<IExtendedRepository<TR, T>>();

                repository.UseDbContext(DbContext);

                _repositories.Add(typeof(TR), repository);
            }

            // ReSharper disable once InconsistentlySynchronizedField
            return _repositories[typeof(TR)] as IExtendedRepository<TR, T>;
        }

        public TR GetRepository<TR>() where TR : class, IRepository
        {
            lock (_lock)
            {
                if (_repositories.ContainsKey(typeof(TR))) return _repositories[typeof(TR)] as TR;

                var repository = ComponentContext.Resolve<TR>();

                try
                {
                    typeof(TR).GetMethod("UseDbContext")?.Invoke(repository, new object[] { DbContext });
                }
                catch
                {
                    // ignored
                }

                _repositories.Add(typeof(TR), repository);
            }

            // ReSharper disable once InconsistentlySynchronizedField
            return _repositories[typeof(TR)] as TR;
        }

        public async Task UpdateOperationalExecutionAsync(Func<OperationalEntity, OperationalEntity> func, CancellationToken token = default)
        {
            var operational = await GetRepositoryFor<OperationalEntity>().SingleAsync(token);

            operational.Version++;

            var updatedEntity = func(operational);

            await EnsureUniquenessAsync($"operational-{operational.Version}", token);

            await GetRepositoryFor<OperationalEntity>().UpdateAsync(x => true, x => new OperationalEntity
            {
                Jobs = updatedEntity.Jobs,
                Settings = updatedEntity.Settings,
                Execution = updatedEntity.Execution,
                Version = operational.Version
            }, token);
        }

        public async Task<TN> UpdateOperationalExecutionAsync<TN>(Func<TN, TN> func, CancellationToken token = default) where TN : class
        {
            var operational = await GetRepositoryFor<OperationalEntity>().SingleAsync(token);
            var operationalExecution = JsonProvider.Deserialize<TN>(operational.Execution);

            operational.Version++;

            var updatedEntity = func(operationalExecution);

            await EnsureUniquenessAsync($"operational-{operational.Version}", token);

            await GetRepositoryFor<OperationalEntity>().UpdateAsync(x => true, x => new OperationalEntity
            {
                Execution = JsonProvider.Serialize(updatedEntity),
                Version = operational.Version
            }, token);

            return updatedEntity;
        }
    }
}