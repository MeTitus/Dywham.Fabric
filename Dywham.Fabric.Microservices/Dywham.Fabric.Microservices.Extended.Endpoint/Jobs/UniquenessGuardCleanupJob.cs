using System.Threading.Tasks;
using Dywham.Fabric.Data.Repositories.EntityFramework;
using Dywham.Fabric.Microservices.Extended.Storage.Data.Repositories.Entities;
using Dywham.Fabric.Microservices.Extended.Storage.Data.Repositories.Infrastructure;
using Dywham.Fabric.Providers;
using NServiceBus;
using Quartz;

namespace Dywham.Fabric.Microservices.Extended.Endpoint.Jobs
{
    public class UniquenessGuardCleanupJob<T> : ExtendedJobRunner<T> where T : ExtendedDatabaseContext
    {
        public IExtendedEndpointSettings EndpointSettings { get; set; }

        public IDateTimeProvider DateTimeProvider { get; set; }

        public IEfRepository<UniquenessGuardEntity, T> UniquenessGuardRepository { get; set; }

        public override string Name => "UniquenessGuardCleanupJob";

        public override ITrigger Trigger => TriggerBuilder.Create()
            .WithCronSchedule(EndpointSettings.UniquenessGuardCleanupJob, x => x.WithMisfireHandlingInstructionDoNothing())
            .WithIdentity(Name)
            .StartNow()
            .Build();


        public override async Task ExecuteJobAsync(IJobExecutionContext context, IEndpointInstance endpointInstance, ExtendedJob job)
        {
            var dateTime = DateTimeProvider.GetUtcNow();

            await UniquenessGuardRepository.DeleteAsync(x => x.DateTime <= dateTime.AddDays(-30));
        }
    }
}