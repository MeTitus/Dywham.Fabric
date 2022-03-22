using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace Dywham.Fabric.Data.Repositories.EntityFramework.Infrastructure
{
    public abstract class EfDatabaseContext : DbContext
    {
        protected EfDatabaseContext()
        { }

        protected EfDatabaseContext(string connectionString)
        {
            ConnectionString = connectionString;
        }

        protected EfDatabaseContext(DbConnection connection)
        {
            DbConnection = connection;
        }


        protected DbConnection DbConnection { get; set; }

        public string ConnectionString { get; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.EnableDetailedErrors();
        }
    }
}