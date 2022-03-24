using System;
using System.Threading;
using System.Threading.Tasks;
using Dywham.Fabric.Microservices.Endpoint.Adapters.EventAudit;
using Dywham.Fabric.Microservices.Extended.Storage.Data.Repositories.Entities;
using Dywham.Fabric.Microservices.Extended.Storage.Data.Repositories.Infrastructure;

namespace Dywham.Fabric.Microservices.Extended.Endpoint.Providers
{
    public class EventStoreProvider<TZ> : IEventAuditProvider where TZ : ExtendedDatabaseContext
    {
        public IDataContextProvider<TZ> DataContextProvider { get; set; }


        public async Task StoreAsync(EventAuditEntry eventAuditEntry, CancellationToken token)
        {
            var entity = new EventAuditingEntity
            {
                DateTime = eventAuditEntry.DateTime ?? DateTime.UtcNow,
                TypeName = eventAuditEntry.PayloadTypeName,
                Identities = eventAuditEntry.Identities != null
                    ? string.Join(",", eventAuditEntry.Identities)
                    : string.Empty,
                Payload = eventAuditEntry.Payload
            };

            if (eventAuditEntry.OriginatedInTheContextOfPayload != null)
            {
                entity.OriginatedInTheContextOfTypeName = eventAuditEntry.OriginatedInTheContextOfPayloadTypeName;
                entity.OriginatedInTheContextOfPayload = eventAuditEntry.OriginatedInTheContextOfPayload;
                entity.TrackingId = eventAuditEntry.TrackingId;
            }

            await DataContextProvider.GetRepositoryFor<EventAuditingEntity>().AddAsync(entity, token);
        }
    }
}