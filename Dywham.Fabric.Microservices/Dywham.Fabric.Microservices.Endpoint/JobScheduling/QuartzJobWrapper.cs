using System;
using System.Threading.Tasks;
using Autofac;
using log4net;
using NServiceBus;
using Quartz;

namespace Dywham.Fabric.Microservices.Endpoint.JobScheduling
{
    public class QuartzJobWrapper : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(QuartzJobWrapper));


        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var container = (IContainer) context.Scheduler.Context.Get("Container");
                var endpointInstance = (IEndpointInstance) context.Scheduler.Context.Get("EndpointInstance");
                // ReSharper disable once AssignNullToNotNullAttribute
                var job = (IJobScheduler) container.Resolve(Type.GetType(context.JobDetail.Key.Name));

                await job.ExecuteAsync(context, endpointInstance);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);

                throw;
            }
        }
    }
}