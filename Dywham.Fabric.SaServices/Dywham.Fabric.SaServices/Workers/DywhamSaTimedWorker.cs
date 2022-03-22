using System;
using System.Threading.Tasks;
using Quartz;

namespace Dywham.Fabric.SaServices.Workers
{
    public abstract class DywhamSaTimedWorker : IDywhamSaTimedWorker
    {
        public IServiceControl ServiceControl { get; set; }

        public string Name { get; set; } = Guid.NewGuid().ToString();

        public abstract ITrigger Trigger { get; }


        public abstract Task DoWorkAsync(IJobExecutionContext context);

        public virtual void OnException(Exception ex)
        { }
    }
}