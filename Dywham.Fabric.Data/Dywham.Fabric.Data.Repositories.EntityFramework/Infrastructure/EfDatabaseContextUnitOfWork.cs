using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Dywham.Fabric.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Dywham.Fabric.Data.Repositories.EntityFramework.Infrastructure
{
    public abstract class EfDatabaseContextUnitOfWork<T, TY> : IEfDatabaseContextUnitOfWork where T : IEfDatabaseContextFactory<TY> where TY : EfDatabaseContext, IDisposable
    {
        // ReSharper disable once StaticMemberInGenericType
        protected TY DbContext;
        private T _databaseContextFactory;
        // ReSharper disable once StaticMemberInGenericType
        protected static readonly Dictionary<Type, Dictionary<PropertyInfo, MethodInfo>> Properties = new();
        // ReSharper disable once StaticMemberInGenericType
        private static readonly object Lock = new();
        private bool _transactionCommitted;
        private IDbContextTransaction _transaction;
        private readonly object _object = new();


        protected EfDatabaseContextUnitOfWork(T databaseContextFactory)
        {
            _databaseContextFactory = databaseContextFactory;
        }


        protected virtual IsolationLevel IsolationLevel => IsolationLevel.Serializable;


        public virtual async Task InitAsync(CancellationToken token)
        {
            lock (this)
            {
                if (DbContext != null) return;
            }

            DbContext = _databaseContextFactory.CreateInstance();

            try
            {
                _transaction = await DbContext.Database.BeginTransactionAsync(IsolationLevel, token);

                lock (Lock)
                {
                    var type = GetType();

                    if (Properties.ContainsKey(type)) return;

                    Properties.Add(type, new Dictionary<PropertyInfo, MethodInfo>());

                    var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                        .Where(x => x.PropertyType.IsAssignableToGenericType(typeof(IEfRepository<,>)));

                    foreach (var property in properties)
                    {
                        const BindingFlags bindingFlags = BindingFlags.FlattenHierarchy |
                                                          BindingFlags.Instance |
                                                          BindingFlags.NonPublic |
                                                          BindingFlags.Public;

                        var types = property.PropertyType.GetInterfaces().ToList();

                        // We need to check the property's type too
                        types.Add(property.PropertyType);

                        var method = types
                            .SelectMany(x => x.GetMethods(bindingFlags))
                            .FirstOrDefault(x => x.Name.Equals("UseDbContext"));

                        if (method == null) continue;

                        Properties[type].Add(property, method);
                    }
                }
            }
            catch
            {
                Dispose();

                throw;
            }

            // Assign repository instances();
            // ReSharper disable once InconsistentlySynchronizedField
            foreach (var (key, value) in Properties[GetType()])
            {
                var instance = key.GetValue(this, null);

                value?.Invoke(instance, new object[] { DbContext });
            }
        }

        public virtual async Task<int> SaveChangesAsync(CancellationToken token = default)
        {
            if (DbContext == null)
            {
                throw new UnitOfWorkNotInitializedException();
            }

            var changesCount = await DbContext.SaveChangesAsync(token);

            try
            {
                await _transaction.CommitAsync(token);

                _transactionCommitted = true;
            }
            catch
            {
                Dispose();

                throw;
            }

            return changesCount;
        }

        public void Dispose()
        {
            lock (_object)
            {
                try
                {
                    if (_transaction == null) return;

                    if (!_transactionCommitted)
                    {
                        try
                        {
                            _transaction.RollbackAsync();
                        }
                        catch
                        {
                            // ignored
                        }
                    }

                    try
                    {
                        _transaction?.Dispose();
                    }
                    catch
                    {
                        // ignored
                    }

                    try
                    {
                        DbContext?.Dispose();
                    }
                    catch
                    {
                        // ignored
                    }
                }
                catch
                {
                    // ignored
                }

                _transaction = null;
            }
        }
    }
}