using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Dywham.Fabric.Data.Repositories.Filters;
using Microsoft.EntityFrameworkCore;

namespace Dywham.Fabric.Data.Repositories.EntityFramework
{
    public interface IEfRepository<T> : IRepository<T> where T : class, new()
    {
        Task<T> FirstOrDefaultAsync(QueryProperty<T> queryProperties, CancellationToken token = default);

        Task<T> FirstAsync(QueryProperty<T> queryProperties, CancellationToken token = default);

        Task<T> SingleAsync(QueryProperty<T> queryProperties, CancellationToken token = default);

        Task<T> SingleOrDefaultAsync(QueryProperty<T> queryProperties, CancellationToken token = default);

        Task<bool> AnyAsync(QueryProperty<T> queryProperties, CancellationToken token = default);

        Task<List<T>> AllAsync(CancellationToken token = default);

        Task<bool> AllMatchAsync(QueryProperty<T> queryProperties, CancellationToken token = default);

        Task<long> CountAsync(QueryProperty<T> queryProperties, CancellationToken token = default);

        Task<int> UpdateAsync(QueryProperty<T> queryProperties, Expression<Func<T, T>> updateFunc, CancellationToken token = default);

        Task<List<T>> WhereAsync(QueryProperty<T> queryProperties, CancellationToken token = default);

        Task<QueryResult<T>> WhereToExecutionResultAsync(QueryProperty<T> queryProperties, CancellationToken token);

        Task<QueryResult<T>> GetAsync(QueryFilter filter, CancellationToken token);

        Task<QueryResult<TY>> WhereToExecutionResultAsync<TY>(QueryProperty<T> queryProperties, Expression<Func<T, TY>> funcConvert, CancellationToken token) where TY : class;
    }

    public interface IEfRepository<T, in TZ> : IEfRepository<T> where T : class, new() where TZ : DbContext
    {
        public void UseDbContext(TZ dbContext);
    }
}