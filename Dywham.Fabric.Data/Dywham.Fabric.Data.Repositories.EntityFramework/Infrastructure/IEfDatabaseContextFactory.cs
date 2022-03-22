namespace Dywham.Fabric.Data.Repositories.EntityFramework.Infrastructure
{
    public interface IEfDatabaseContextFactory<out T> where T : EfDatabaseContext
    {
        T CreateInstance();
    }
}