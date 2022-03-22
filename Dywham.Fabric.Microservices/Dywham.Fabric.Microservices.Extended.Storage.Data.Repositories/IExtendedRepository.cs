using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Dywham.Fabric.Data.Repositories.EntityFramework;
using Dywham.Fabric.Microservices.Extended.Storage.Data.Repositories.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dywham.Fabric.Microservices.Extended.Storage.Data.Repositories
{
    public interface IExtendedRepository<T, in TC> : IEfRepository<T, TC>
        where T : ExtendedEntity, new()
        where TC : DbContext
    {
        Task DeleteAsync(Expression<Func<T, bool>> whereFunc, DateTime dateTime, CancellationToken token = default);
    }
}