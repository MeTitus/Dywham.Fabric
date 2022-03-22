using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Dywham.Fabric.Data.Repositories;
using Dywham.Fabric.Data.Repositories.EntityFramework;
using Dywham.Fabric.Microservices.Extended.Storage.Data.Repositories;
using Dywham.Fabric.Microservices.Extended.Storage.Data.Repositories.Entities;
using Dywham.Fabric.Microservices.Extended.Storage.Data.Repositories.Infrastructure;

namespace Dywham.Fabric.Microservices.Extended.Endpoint.Providers
{
    public class DataContextProvider<TC> : IDataContextProvider<TC> where TC : ExtendedDatabaseContext
    {
        private readonly object _lock = new();
        private readonly Dictionary<Type, IRepository> _repositories = new();
        public IComponentContext ComponentContext { get; set; }


        public async Task<IExtendedUnitOfWork<TC>> RunInAUnitOfWorkAsync(CancellationToken token = default)
        {
            var unitOfWork = ComponentContext.Resolve<IExtendedUnitOfWork<TC>>();

            await unitOfWork.InitAsync(token);

            return unitOfWork;
        }

        public TR GetRepository<TR>() where TR : class, IRepository
        {
            lock (_lock)
            {
                if (!_repositories.ContainsKey(typeof(TR)))
                {
                    _repositories.Add(typeof(TR), ComponentContext.Resolve<TR>());
                }

                return _repositories[typeof(TR)] as TR;
            }
        }

        public IEfRepository<TR, TC> GetRepositoryFor<TR>() where TR : class, new()
        {
            lock (_lock)
            {
                if (!_repositories.ContainsKey(typeof(TR)))
                {
                    _repositories.Add(typeof(TR), ComponentContext.Resolve<IEfRepository<TR, TC>>());
                }

                return _repositories[typeof(TR)] as IEfRepository<TR, TC>;
            }
        }

        public IExtendedRepository<TR, TC> GetExtendedRepositoryFor<TR>() where TR : ExtendedEntity, new()
        {
            lock (_lock)
            {
                if (!_repositories.ContainsKey(typeof(TR)))
                {
                    _repositories.Add(typeof(TR), ComponentContext.Resolve<IExtendedRepository<TR, TC>>());
                }

                return _repositories[typeof(TR)] as IExtendedRepository<TR, TC>;
            }
        }
    }
}