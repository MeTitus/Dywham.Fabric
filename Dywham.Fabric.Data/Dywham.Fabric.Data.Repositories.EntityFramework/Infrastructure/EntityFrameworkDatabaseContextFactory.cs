namespace Dywham.Fabric.Data.Repositories.EntityFramework.Infrastructure
{
    public abstract class EntityFrameworkDatabaseContextFactory<T> : IEntityFrameworkDatabaseContextFactory<T>
        where T : EntityFrameworkDatabaseContext
    {
        public abstract T CreateInstance();
    }
}