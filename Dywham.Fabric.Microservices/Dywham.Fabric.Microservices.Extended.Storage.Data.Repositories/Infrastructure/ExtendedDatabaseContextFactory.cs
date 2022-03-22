using Dywham.Fabric.Data.Repositories.EntityFramework.Infrastructure;

namespace Dywham.Fabric.Microservices.Extended.Storage.Data.Repositories.Infrastructure
{
    public abstract class ExtendedDatabaseContextFactory<T> : EfDatabaseContextFactory<T> where T : ExtendedDatabaseContext
    {
        public IExtendedDatabaseSettings Settings { get; set; }
    }
}