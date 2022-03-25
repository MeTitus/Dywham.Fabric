using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Dywham.Fabric.Data.Repositories
{
    public interface IRepository
    { }

    public interface IRepository<T> : IRepository where T : class, new()
    {
        Task<int> AddAsync(T entity, CancellationToken token = default);

        Task<int> AddAsync(IList<T> entities, CancellationToken token = default);

        Task<int?> AddAndSilenceUniqueKeyConstraintAsync(T entity, CancellationToken token = default);

        Task<int?> AddAndSilenceUniqueKeyConstraintAsync(IList<T> entities, CancellationToken token = default);

        Task<T> FirstOrDefaultAsync(CancellationToken token = default);

        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> whereFunc, CancellationToken token = default);

        Task<T> FirstAsync(CancellationToken token = default);

        Task<T> FirstAsync(Expression<Func<T, bool>> whereFunc, CancellationToken token = default);

        Task<T> SingleAsync(CancellationToken token = default);

        Task<T> SingleAsync(Expression<Func<T, bool>> whereFunc, CancellationToken token = default);

        Task<T> SingleOrDefaultAsync(CancellationToken token = default);

        Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> whereFunc, CancellationToken token = default);

        Task<bool> AnyAsync(CancellationToken token = default);

        Task<bool> AnyAsync(Expression<Func<T, bool>> whereFunc, CancellationToken token = default);

        Task<bool> AllMatchAsync(Expression<Func<T, bool>> whereFunc, CancellationToken token = default);

        Task DeleteAsync(Expression<Func<T, bool>> whereFunc, CancellationToken token = default);

        Task<long> CountAsync(CancellationToken token = default);

        Task<int> CountAsync(Expression<Func<T, bool>> whereFunc, CancellationToken token = default);

        Task<int> UpdateAsync(Expression<Func<T, bool>> whereFunc, Expression<Func<T, T>> updateFunc, CancellationToken token = default);

        Task<List<T>> WhereAsync(Expression<Func<T, bool>> whereFunc, CancellationToken token = default);

        Task<QueryResult<T>> WhereToExecutionResultAsync(Expression<Func<T, bool>> whereFunc, CancellationToken token);

        Task<QueryResult<TY>> WhereToExecutionResultAsync<TY>(Expression<Func<T, bool>> whereFunc, Expression<Func<T, TY>> funcConvert, CancellationToken token) where TY : class;
    }
}