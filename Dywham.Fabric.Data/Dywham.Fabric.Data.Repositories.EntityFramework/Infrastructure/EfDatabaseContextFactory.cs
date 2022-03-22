namespace Dywham.Fabric.Data.Repositories.EntityFramework.Infrastructure
{
    public abstract class EfDatabaseContextFactory<T> : IEfDatabaseContextFactory<T> where T : EfDatabaseContext
    {
        public abstract T CreateInstance();
    }
}