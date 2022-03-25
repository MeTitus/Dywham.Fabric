using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace Dywham.Fabric.Data.Repositories.EntityFramework.Infrastructure
{
    public abstract class EntityFrameworkDatabaseContext : DbContext
    {
        protected EntityFrameworkDatabaseContext()
        { }

        protected EntityFrameworkDatabaseContext(string connectionString)
        {
            ConnectionString = connectionString;
        }

        protected EntityFrameworkDatabaseContext(DbConnection connection)
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