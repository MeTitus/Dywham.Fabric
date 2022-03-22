using System;
using System.Threading.Tasks;
using Autofac;
using log4net;
using Quartz;

namespace Dywham.Fabric.SaServices.Workers
{
    public class QuartzJobWrapper : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var container = (IContainer)context.Scheduler.Context.Get("Container");
                var job = (IDywhamSaTimedWorker)container.Resolve(Type.GetType(context.JobDetail.Key.Name));

                try
                {
                    await job.DoWorkAsync(context);
                }
                catch (Exception ex)
                {
                    try
                    {
                        job.OnException(ex);
                    }
                    catch(Exception innerEx)
                    {
                        Logger.Error($"Worker '{job.Name}' {innerEx}");

                        return;
                    }

                    Logger.Error($"Worker '{job.Name}' {ex}");
                }

                Logger.Info($"Worker '{job.Name}' has finished");
            }
            catch(Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }
}