using System.Data.Common;
using Dywham.Fabric.Data.Repositories.EntityFramework.SqlServer;
using Dywham.Fabric.Microservices.Extended.Storage.Data.Repositories.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dywham.Fabric.Microservices.Extended.Storage.Data.Repositories.Infrastructure
{
    public abstract class ExtendedDatabaseContext : SqlServerEfDatabaseContext
    {
        protected ExtendedDatabaseContext()
        { }

        protected ExtendedDatabaseContext(string connectionString) : base(connectionString)
        { }

        protected ExtendedDatabaseContext(DbConnection connection) : base(connection)
        { }


        public virtual DbSet<UniquenessGuardEntity> UniquenessGuards { get; set; }

        public virtual DbSet<OperationalEntity> Operational { get; set; }

        public virtual DbSet<EventAuditingEntity> Events { get; set; }
        

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            optionsBuilder.AddInterceptors(new ReadCommittedTransactionInterceptor());
        }
    }
}