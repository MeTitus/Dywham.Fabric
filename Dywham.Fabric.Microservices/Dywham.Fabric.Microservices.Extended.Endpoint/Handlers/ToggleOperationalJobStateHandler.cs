using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dywham.Fabric.Microservices.Endpoint.Handlers;
using Dywham.Fabric.Microservices.Extended.Contracts.Messages.Commands;
using Dywham.Fabric.Microservices.Extended.Contracts.Messages.Events;
using Dywham.Fabric.Microservices.Extended.Endpoint.Jobs;
using Dywham.Fabric.Microservices.Extended.Endpoint.Providers;
using Dywham.Fabric.Microservices.Extended.Storage.Data.Repositories.Infrastructure;
using Dywham.Fabric.Providers.Serialization.Json;
using NServiceBus;

namespace Dywham.Fabric.Microservices.Extended.Endpoint.Handlers
{
    public abstract class ToggleOperationalJobStateHandler<T, TZ> : DywhamMessageHandler<T> 
        where T : ToggleOperationalJobState, new()
        where TZ : ExtendedDatabaseContext
    {
        public IDataContextProvider<TZ> DataContextProvider { get; set; }

        public IJsonProvider JsonProvider { get; set; }

        public IExtendedEndpointSettings EndpointSettings { get; set; }

        

        protected virtual Task OnOperationalJobStateChangeCompleted(Guid jobId, bool enabled, IExtendedUnitOfWork<TZ> uow, IMessageHandlerContext context)
        {
            return Task.CompletedTask;
        }

        protected override async Task HandleAsync(CancellationToken token)
        {
            var enabled = true;
            var jobId = Guid.Empty;
            var jobVersion = 0;

            using (var uow = await DataContextProvider.RunInAUnitOfWorkAsync(token))
            {
                await uow.UpdateOperationalExecutionAsync(x =>
                {
                    var jobs = JsonProvider.Deserialize<List<ExtendedJob>>(x.Jobs);
                    var job = jobs.Single(y => y.Id == Message.Id);

                    jobVersion = ++job.Version;

                    if (Message.Enable.HasValue)
                    {
                        job.Enabled = Message.Enable.Value;
                    }
                    else
                    {
                        job.Enabled = !job.Enabled;
                    }

                    enabled = job.Enabled;

                    jobId = job.Id;

                    x.Jobs = JsonProvider.Serialize(jobs);

                    return x;
                }, token);

                await uow.EnsureUniquenessAsync($"Job-{jobId}-{jobVersion}", token);

                await OnOperationalJobStateChangeCompleted(jobId, enabled, uow, Context);

                await uow.SaveChangesAsync(CancellationToken.None);
            }

            await Context.Publish(new OperationalJobStateToggled
            {
                Id = jobId,
                TrackingId = Message.TrackingId,
                ClientNotificationTracking = Message.ClientNotificationTracking,
                Enable = enabled,
                EndpointName = EndpointSettings.EndpointName
            });
        }
    }
}