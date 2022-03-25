namespace Dywham.Fabric.Data.Repositories.EntityFramework.Infrastructure
{
    public interface IEntityFrameworkDatabaseContextFactory<out T> where T : EntityFrameworkDatabaseContext
    {
        T CreateInstance();
    }
}