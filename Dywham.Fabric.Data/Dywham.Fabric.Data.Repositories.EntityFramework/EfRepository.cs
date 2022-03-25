using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Dywham.Fabric.Data.Repositories.EntityFramework.Infrastructure;
using Dywham.Fabric.Data.Repositories.Filters;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace Dywham.Fabric.Data.Repositories.EntityFramework
{
    public class EfRepository<T, TZ> : IEfRepository<T, TZ> where T : class, new() where TZ : EntityFrameworkDatabaseContext
    {
        private readonly IEntityFrameworkDatabaseContextFactory<TZ> _databaseContextFactory;
        private readonly object _lock = new();
        private TZ _dbContext;


        public EfRepository(IEntityFrameworkDatabaseContextFactory<TZ> databaseContextFactory)
        {
            _databaseContextFactory = databaseContextFactory;
        }


		public IList<IEntityGlobalFilter<T>> GlobalFilters { get; set; }

        protected TZ DbContext
        {
            get
            {
                lock (_lock)
                {
                    _dbContext ??= _databaseContextFactory.CreateInstance();
                }

                return _dbContext;
            }

            private set => _dbContext = value;
        }


        public void UseDbContext(TZ dbContext)
        {
            DbContext = dbContext;
        }

        public virtual Task<int> AddAsync(T entity, CancellationToken token = default)
        {
            DbContext.Set<T>().Add(entity);

            return DbContext.SaveChangesAsync(token);
        }

        public virtual Task<int> AddAsync(IList<T> entities, CancellationToken token = default)
        {
            DbContext.Set<T>().AddRange(entities);

            return DbContext.SaveChangesAsync(token);
        }

        public virtual async Task<int?> AddAndSilenceUniqueKeyConstraintAsync(T entity, CancellationToken token = default)
        {
            try
            {
                await DbContext.Set<T>().AddAsync(entity, token);

                return await DbContext.SaveChangesAsync(token);
            }
            catch (Exception ex)
            {
                if (FindIsDbUpdateException(ex)) return null;

                throw;
            }
        }

        public virtual async Task<int?> AddAndSilenceUniqueKeyConstraintAsync(IList<T> entities, CancellationToken token = default)
        {
            try
            {
                await DbContext.Set<T>().AddRangeAsync(entities, token);

                return await DbContext.SaveChangesAsync(token);
            }
            catch (Exception ex)
            {
                if (FindIsDbUpdateException(ex)) return null;

                throw;
            }
        }

        public virtual Task<T> FirstOrDefaultAsync(CancellationToken token = default)
        {
            return InitializeQueryable(new QueryProperty<T>()).FirstOrDefaultAsync(token);
        }

        public virtual Task<T> FirstOrDefaultAsync(QueryProperty<T> queryProperties, CancellationToken token = default)
        {
            return InitializeQueryable(queryProperties).FirstOrDefaultAsync(token);
        }

        public virtual Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> whereFunc, CancellationToken token = default)
        {
            return InitializeQueryable(new QueryProperty<T>
            {
                Where = set => set.Where(whereFunc)
            }).FirstOrDefaultAsync(token);
        }

        public virtual Task<T> FirstAsync(CancellationToken token = default)
        {
            return InitializeQueryable(new QueryProperty<T>()).FirstAsync(token);
        }

        public virtual Task<T> FirstAsync(QueryProperty<T> queryProperties, CancellationToken token = default)
        {
            return InitializeQueryable(queryProperties).FirstAsync(token);
        }
        
        public virtual Task<T> FirstAsync(Expression<Func<T, bool>> whereFunc, CancellationToken token = default)
        {
            return InitializeQueryable(new QueryProperty<T>
            {
                Where = set => set.Where(whereFunc)
            }).FirstAsync(token);
        }

        public Task<T> SingleAsync(CancellationToken token = default)
        {
            return InitializeQueryable(new QueryProperty<T>()).SingleAsync(token);
        }

        public virtual Task<T> SingleAsync(QueryProperty<T> queryProperties, CancellationToken token = default)
        {
            return InitializeQueryable(queryProperties).SingleAsync(token);
        }
        
        public virtual Task<T> SingleAsync(Expression<Func<T, bool>> whereFunc, CancellationToken token = default)
        {
            return InitializeQueryable(new QueryProperty<T>
            {
                Where = set => set.Where(whereFunc)
            }).SingleAsync(token);
        }

        public virtual Task<T> SingleOrDefaultAsync(CancellationToken token = default)
        {
            return InitializeQueryable(new QueryProperty<T>()).SingleOrDefaultAsync(token);
        }

        public virtual Task<T> SingleOrDefaultAsync(QueryProperty<T> queryProperties, CancellationToken token = default)
        {
            return InitializeQueryable(queryProperties).SingleOrDefaultAsync(token);
        }

        public virtual Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> whereFunc, CancellationToken token = default)
        {
            return InitializeQueryable(new QueryProperty<T>
            {
                Where = set => set.Where(whereFunc)
            }).SingleOrDefaultAsync(token);
        }

        public virtual Task<int> UpdateAsync(Expression<Func<T, bool>> whereFunc, Expression<Func<T, T>> updateFunc, CancellationToken token = default)
        {
            return InitializeQueryable(new QueryProperty<T>
            {
                Where = set => set.Where(whereFunc)
            }).UpdateAsync(updateFunc, token);
        }

        public virtual Task<int> UpdateAsync(QueryProperty<T> queryProperties, Expression<Func<T, T>> updateFunc, CancellationToken token = default)
        {
            return InitializeQueryable(queryProperties).UpdateAsync(updateFunc, token);
        }

        public virtual Task<bool> AnyAsync(CancellationToken token = default)
        {
            return InitializeQueryable(new QueryProperty<T>()).AnyAsync(token);
        }

        public virtual Task<bool> AnyAsync(QueryProperty<T> queryProperties, CancellationToken token = default)
        {
            return InitializeQueryable(queryProperties).AnyAsync(token);
        }

        public virtual Task<bool> AnyAsync(Expression<Func<T, bool>> whereFunc, CancellationToken token = default)
        {
            return InitializeQueryable(new QueryProperty<T>
            {
                Where = set => set.Where(whereFunc)
            }).AnyAsync(token);
        }

        public virtual Task<List<T>> AllAsync(CancellationToken token = default)
        {
            return InitializeQueryable(new QueryProperty<T>()).ToListAsync(token);
        }

        public virtual Task<bool> AllMatchAsync(Expression<Func<T, bool>> whereFunc, CancellationToken token = default)
        {
            return InitializeQueryable(new QueryProperty<T>()).AllAsync(whereFunc, token);
        }

        public virtual Task<bool> AllMatchAsync(QueryProperty<T> queryProperties, CancellationToken token = default)
        {
            return InitializeQueryable(queryProperties).AllAsync(x => true, token);
        }

        public virtual Task DeleteAsync(Expression<Func<T, bool>> whereFunc, CancellationToken token = default)
        {
            return InitializeQueryable(new QueryProperty<T>
            {
                Where = set => set.Where(whereFunc)
            }).DeleteAsync(token);
        }

        public virtual Task<long> CountAsync(CancellationToken token = default)
        {
            return InitializeQueryable(new QueryProperty<T>()).LongCountAsync(token);
        }

        public virtual Task<long> CountAsync(QueryProperty<T> queryProperties, CancellationToken token = default)
        {
            return InitializeQueryable(queryProperties).LongCountAsync(token);
        }

        public virtual Task<int> CountAsync(Expression<Func<T, bool>> whereFunc, CancellationToken token = default)
        {
            return InitializeQueryable(new QueryProperty<T>
            {
                Where = set => set.Where(whereFunc)
            }).CountAsync(token);
        }

        public virtual Task<List<T>> WhereAsync(QueryProperty<T> queryProperties, CancellationToken token = default)
        {
            return InitializeQueryable(queryProperties).ToListAsync(token);
        }

        public virtual Task<List<T>> WhereAsync(Expression<Func<T, bool>> whereFunc, CancellationToken token = default)
        {
            return InitializeQueryable(new QueryProperty<T>
            {
                Where = set => set.Where(whereFunc)
            }).ToListAsync(token);
        }

        public virtual Task<QueryResult<T>> WhereToExecutionResultAsync(Expression<Func<T, bool>> whereFunc, CancellationToken token)
        {
            return WhereToExecutionResultAsync(new QueryProperty<T>
            {
                Where = set => set.Where(whereFunc)
            }, token);
        }

        public virtual async Task<QueryResult<T>> WhereToExecutionResultAsync(QueryProperty<T> queryProperties, CancellationToken token)
        {
            var set = DbContext.Set<T>();
            var queryable = set.AsQueryable();

            if (queryProperties.Where != null)
            {
                queryable = queryProperties.Where(set);
            }

            queryable = ApplyGlobalFilters(queryable);

            var count = queryable.DeferredCount().FutureValue();

            if (queryProperties.OrderedSet != null)
            {
                queryable = OrderBy(queryable, queryProperties.OrderedSet.ColumnName, !queryProperties.OrderedSet.Asc);
            }

            if (queryProperties.StartIndex is > 0)
            {
                queryable = queryable.Skip(queryProperties.StartIndex.Value);
            }

            if (queryProperties.Limit is > 0)
            {
                queryable = queryable.Take(queryProperties.Limit.Value);
            }

            var data = await queryable.Future().ToListAsync(token);

            return new QueryResult<T>
            {
                Data = data,
                TotalCount = count.Value,
                SetSize = data.LongCount()
            };
        }

        public virtual Task<QueryResult<TY>> WhereToExecutionResultAsync<TY>(Expression<Func<T, bool>> whereFunc, Expression<Func<T, TY>> funcConvert, CancellationToken token) where TY : class
        {
            return WhereToExecutionResultAsync(new QueryProperty<T>
            {
                Where = set => set.Where(whereFunc)
            }, funcConvert, token);
        }

        public virtual async Task<QueryResult<TY>> WhereToExecutionResultAsync<TY>(QueryProperty<T> queryProperties, Expression<Func<T, TY>> funcConvert, CancellationToken token) where TY : class
        {
            var set = DbContext.Set<T>();
            var queryable = set.AsQueryable();

            if (queryProperties.Where != null)
            {
                queryable = queryProperties.Where(set);
            }

            queryable = ApplyGlobalFilters(queryable);

            var transformedQuery = queryable.Select(funcConvert);
            var count = transformedQuery.DeferredCount().FutureValue();

            if (queryProperties.OrderedSet != null)
            {
                transformedQuery = OrderBy(transformedQuery, queryProperties.OrderedSet.ColumnName, !queryProperties.OrderedSet.Asc);
            }

            if (queryProperties.StartIndex is > 0)
            {
                transformedQuery = transformedQuery.Skip(queryProperties.StartIndex.Value);
            }

            if (queryProperties.Limit is > 0)
            {
                transformedQuery = transformedQuery.Take(queryProperties.Limit.Value);
            }

            var data = await transformedQuery.Future().ToListAsync(token);

            return new QueryResult<TY>
            {
                Data = data,
                TotalCount = count.Value,
                SetSize = data.LongCount()
            };
        }

        public async Task<QueryResult<T>> GetAsync(QueryFilter filter, CancellationToken token)
        {
            var queryProps = new QueryProperty<T>
            {
                Limit = filter.Limit,
                StartIndex = filter.StartIndex,
                Where = x => x.AsNoTracking(),
                OrderedSet = new QueryResultOrdering
                {
                    Asc = filter.Asc,
                    ColumnName = filter.ColumnName
                }
            };

            var result = await WhereToExecutionResultAsync(queryProps, token);

            return new QueryResult<T>
            {
                Data = result.Data,
                SetSize = result.SetSize,
                TotalCount = result.TotalCount
            };
        }

        private IQueryable<T> InitializeQueryable(QueryProperty<T> queryProperties)
        {
            var set = DbContext.Set<T>();
            var queryable = set.AsQueryable();

            if (queryProperties.Where != null)
            {
                queryable = queryProperties.Where(set);
            }

            queryable = ApplyGlobalFilters(queryable);

            if (queryProperties.StartIndex is > 0)
            {
                queryable = queryable.Skip(queryProperties.StartIndex.Value);
            }

            if (queryProperties.Limit is > 0)
            {
                queryable = queryable.Take(queryProperties.Limit.Value);
            }

            if (queryProperties.OrderedSet != null)
            {
                queryable = OrderBy(queryable, queryProperties.OrderedSet.ColumnName, !queryProperties.OrderedSet.Asc);
            }

            return queryable;
        }

        private IQueryable<T> ApplyGlobalFilters(IQueryable<T> queryable)
        {
            if (GlobalFilters != null && GlobalFilters.Any())
            {
                return GlobalFilters.Aggregate(queryable, (current, globalFilter) => current.Where(globalFilter.Filter));
            }

            return queryable;
        }

        private static IQueryable<TEntity> OrderBy<TEntity>(IQueryable<TEntity> source, string orderByProperty, bool desc)
        {
            var command = desc ? "OrderByDescending" : "OrderBy";
            var type = typeof(TEntity);
            var property = type.GetProperty(orderByProperty);
            var parameter = Expression.Parameter(type, "p");
            // ReSharper disable once AssignNullToNotNullAttribute
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new[]
            {
                type, property.PropertyType
            }, source.Expression, Expression.Quote(orderByExpression));

            return source.Provider.CreateQuery<TEntity>(resultExpression);
        }

        private static bool FindIsDbUpdateException(Exception ex)
        {
            var result = ex is Microsoft.Data.SqlClient.SqlException {Number: 2601 or 2627};

            if (!result && ex.InnerException != null)
            {
                result = FindIsDbUpdateException(ex.InnerException);
            }

            return result;
        }
    }
}