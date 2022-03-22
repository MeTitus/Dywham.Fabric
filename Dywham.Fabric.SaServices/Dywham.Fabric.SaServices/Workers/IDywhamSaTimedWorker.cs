using System;
using System.Threading.Tasks;
using Quartz;

namespace Dywham.Fabric.SaServices.Workers
{
    public interface IDywhamSaTimedWorker
    {
        // ReSharper disable once UnusedMember.Global
        IServiceControl ServiceControl { get; set; }

        ITrigger Trigger { get; }

        string Name { get; set; }


        void OnException(Exception ex);

        Task DoWorkAsync(IJobExecutionContext context);
    }
}