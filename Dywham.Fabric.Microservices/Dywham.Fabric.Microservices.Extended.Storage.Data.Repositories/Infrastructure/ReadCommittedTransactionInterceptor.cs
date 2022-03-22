using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Dywham.Fabric.Microservices.Extended.Storage.Data.Repositories.Infrastructure
{
    public class ReadCommittedTransactionInterceptor : DbTransactionInterceptor
    {
        public override InterceptionResult<DbTransaction> TransactionStarting(DbConnection connection, TransactionStartingEventData eventData, InterceptionResult<DbTransaction> result)
        {
            base.TransactionStarting(connection, eventData, result);

            return InterceptionResult<DbTransaction>.SuppressWithResult(connection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted));
        }
    }
}