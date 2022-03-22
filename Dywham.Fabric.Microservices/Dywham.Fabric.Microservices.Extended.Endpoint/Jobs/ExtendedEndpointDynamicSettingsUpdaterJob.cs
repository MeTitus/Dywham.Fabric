using System.Threading.Tasks;
using Dywham.Fabric.Microservices.Extended.Storage.Data.Repositories.Infrastructure;
using Dywham.Fabric.Utils;
using NServiceBus;
using Quartz;

namespace Dywham.Fabric.Microservices.Extended.Endpoint.Jobs
{
    public class ExtendedEndpointDynamicSettingsUpdaterJob<T, TZ> : ExtendedJobRunner<T>
        where T : ExtendedDatabaseContext
        where TZ : ExtendedEndpointDynamicSettings, new()
    {
        public IExtendedEndpointSettings EndpointSettings { get; set; }
        
        public TZ DynamicSettings { get; set; }


        public override string Name => "EndpointDynamicSettingsUpdaterJob";

        public override ITrigger Trigger => TriggerBuilder.Create()
            .WithCronSchedule(EndpointSettings.EndpointDynamicSettingsUpdaterJob, x => x.WithMisfireHandlingInstructionDoNothing())
            .WithIdentity(Name)
            .StartNow()
            .Build();


        public override async Task ExecuteJobAsync(IJobExecutionContext context, IEndpointInstance endpointInstance, ExtendedJob job)
        {
            var operational = await OperationalRepository.FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(operational.Settings))
            {
                //clear all properties
                DynamicSettings = new TZ();

                return;
            }

            var newSettings = JsonProvider.Deserialize<TZ>(operational.Settings);

            foreach (var property in typeof(TZ).GetProperties())
            {
                var value = newSettings.GetPropertyValue(property.Name);
                
                DynamicSettings.SetPropertyValue(property.Name, value);
            }
        }
    }
}