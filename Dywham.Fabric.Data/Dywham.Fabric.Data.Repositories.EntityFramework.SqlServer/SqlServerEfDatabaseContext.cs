using System.Data.Common;
using Dywham.Fabric.Data.Repositories.EntityFramework.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Dywham.Fabric.Data.Repositories.EntityFramework.SqlServer
{
    public class SqlServerEfDatabaseContext : EfDatabaseContext
    {
        protected SqlServerEfDatabaseContext()
        { }

        protected SqlServerEfDatabaseContext(string connectionString) : base(connectionString)
        { }

        protected SqlServerEfDatabaseContext(DbConnection connection) : base(connection)
        { }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            if (DbConnection == null)
            {
                optionsBuilder.UseSqlServer(ConnectionString, OnConfiguring);
            }
            else
            {
                optionsBuilder.UseSqlServer(DbConnection, OnConfiguring);
            }
        }

        protected virtual void OnConfiguring(SqlServerDbContextOptionsBuilder optionsBuilder)
        { }
    }
}