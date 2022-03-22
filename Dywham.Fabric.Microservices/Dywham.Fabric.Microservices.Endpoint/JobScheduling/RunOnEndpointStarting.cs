using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Dywham.Fabric.Microservices.Endpoint.Behaviors;
using NServiceBus;
using Quartz;
using Quartz.Impl;

namespace Dywham.Fabric.Microservices.Endpoint.JobScheduling
{
    public class RunOnEndpointStarting : IRunOnEndpointStartingBehavior
    {
        private IScheduler _scheduler;


        public async Task OnEndpointStartingAsync(IEndpointInstance endpointInstance, ILifetimeScope scope, CancellationToken token)
        {
            var jobs = scope.Resolve<IEnumerable<IDywhamJob>>().ToList();

            if (!jobs.Any()) return;

            _scheduler = await new StdSchedulerFactory().GetScheduler(token);

            foreach (var job in jobs)
            {
                var jobDetail = JobBuilder.Create<QuartzJobWrapper>()
                    // ReSharper disable once AssignNullToNotNullAttribute
                    .WithIdentity(job.GetType().AssemblyQualifiedName)
                    .Build();

                _scheduler.Context.Put("Container", scope);
                _scheduler.Context.Put("EndpointInstance", endpointInstance);

                if (await _scheduler.CheckExists(jobDetail.Key, token))
                {
                    await _scheduler.DeleteJob(jobDetail.Key, token).ConfigureAwait(true);
                }

                await _scheduler.ScheduleJob(jobDetail, job.Trigger, token);
            }

            await _scheduler.Start(token);
        }
    }
}