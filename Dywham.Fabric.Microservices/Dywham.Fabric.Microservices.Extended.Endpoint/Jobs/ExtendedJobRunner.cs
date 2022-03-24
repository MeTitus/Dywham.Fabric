using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dywham.Fabric.Data.Repositories.EntityFramework;
using Dywham.Fabric.Microservices.Endpoint.JobScheduling;
using Dywham.Fabric.Microservices.Extended.Contracts.Messages.Commands;
using Dywham.Fabric.Microservices.Extended.Storage.Data.Repositories.Entities;
using Dywham.Fabric.Microservices.Extended.Storage.Data.Repositories.Infrastructure;
using Dywham.Fabric.Providers.Serialization.Json;
using log4net;
using NServiceBus;
using Quartz;

namespace Dywham.Fabric.Microservices.Extended.Endpoint.Jobs
{
    public abstract class ExtendedJobRunner<T> : IJobScheduler where T : ExtendedDatabaseContext
    {
        // ReSharper disable once StaticMemberInGenericType
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);


        public abstract ITrigger Trigger { get; }

        public abstract string Name { get; }

        public IEfRepository<OperationalEntity, T> OperationalRepository { get; set; }

        public IJsonProvider JsonProvider { get; set; }


        public abstract Task ExecuteJobAsync(IJobExecutionContext context, IEndpointInstance endpointInstance, ExtendedJob job);

        public async Task ExecuteAsync(IJobExecutionContext context, IEndpointInstance endpointInstance)
        {
            if (GetType() != typeof(UniquenessGuardCleanupJob<T>))
            {
                var operationalJob = await OperationalRepository.FirstOrDefaultAsync();
                var execute = false;
                var error = string.Empty;
                Guid? jobId = null;

                if (operationalJob == null || string.IsNullOrWhiteSpace(operationalJob.Jobs))
                {
                    execute = true;
                }
                else
                {
                    var jobs = JsonProvider.Deserialize<List<ExtendedJob>>(operationalJob.Jobs);
                    var job = jobs.SingleOrDefault(x => x.Name.Equals(Name, StringComparison.OrdinalIgnoreCase));

                    if (job == null)
                    {
                        error = $"Job '{Name}' was not found in the Operational table";
                    }
                    else if (!job.Enabled)
                    {
                        Logger.Info($"Job '{job.Name}' is not active");
                    }
                    else
                    {
                        jobId = job.Id;

                        execute = true;
                    }
                }

                if (execute)
                {
                    try
                    {
                        await ExecuteJobAsync(context, endpointInstance, null);

                        return;
                    }
                    catch (Exception ex)
                    {
                        error = ex.ToString();
                    }
                }

                await endpointInstance.SendLocal(new RegisterJobError
                {
                    Error = error,
                    Name = Name,
                    TrackingId = Guid.NewGuid(),
                    Id = jobId
                });
            }
        }
    }
}