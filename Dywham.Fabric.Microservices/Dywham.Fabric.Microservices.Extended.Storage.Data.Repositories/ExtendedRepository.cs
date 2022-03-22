using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Dywham.Fabric.Data.Repositories.EntityFramework;
using Dywham.Fabric.Data.Repositories.EntityFramework.Infrastructure;
using Dywham.Fabric.Microservices.Extended.Storage.Data.Repositories.Entities;
using Dywham.Fabric.Microservices.Extended.Storage.Data.Repositories.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace Dywham.Fabric.Microservices.Extended.Storage.Data.Repositories
{
    //This is a generic repository, do not mark this as abstract as some services may never need to extend this one
    public class ExtendedRepository<T, TZ> : EfRepository<T, TZ>, IExtendedRepository<T, TZ>
        where T : ExtendedEntity, new()
        where TZ : ExtendedDatabaseContext
    {
        public ExtendedRepository(IEfDatabaseContextFactory<TZ> databaseContextFactory) : base(databaseContextFactory)
        {
            DbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }


        public Task DeleteAsync(Expression<Func<T, bool>> whereFunc, DateTime dateTime, CancellationToken token = default)
        {
            DbContext.Set<T>().AsNoTracking()
                .Where(x => x.IsArchived == false)
                .Where(whereFunc)
                .Update(x => new T
                {
                    LastChangedOn = dateTime
                });

            return DbContext.SaveChangesAsync(token);
        }
    }
}