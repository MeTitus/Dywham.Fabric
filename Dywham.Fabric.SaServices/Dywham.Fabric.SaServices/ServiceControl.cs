using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Dywham.Fabric.SaServices.Workers;
using Quartz;
using Quartz.Impl;

namespace Dywham.Fabric.SaServices
{
    public class ServiceControl : IServiceControl
    {
        private IScheduler _scheduler;
        private IContainer _container;
        private List<Tuple<Task, IDywhamSaWorker>> _serviceTasks;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        internal void Init(IContainer container)
        {
            _container = container;

            LoadServices();

            LoadJobs();
        }

        private void LoadServices()
        {
            var services = _container.Resolve<IEnumerable<IDywhamSaWorker>>().ToList();

            if (!services.Any()) return;

            _serviceTasks = new List<Tuple<Task, IDywhamSaWorker>>();

            foreach (var service in services)
            {
                _serviceTasks.Add(new Tuple<Task, IDywhamSaWorker>(Task.Run(() =>
                {
                    try
                    {
                        service.DoWork();
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            service.OnException(ex);
                        }
                        catch (Exception innerEx)
                        {
                            Logger.Error($"Worker '{service.Name}' {innerEx}");

                            return;
                        }

                        Logger.Error($"Worker '{service.Name}' {ex}");
                    }

                    Logger.Info($"Worker '{service}' has finished");

                    var serviceToRemove = _serviceTasks.First(x => x.Item2 == service);

                    _serviceTasks.Remove(serviceToRemove);

                }, _cancellationTokenSource.Token), service));
            }
        }

        private void LoadJobs()
        {
            var jobs = _container.Resolve<IEnumerable<IDywhamSaTimedWorker>>().ToList();

            if (!jobs.Any()) return;

            _scheduler = new StdSchedulerFactory().GetScheduler().Result;

            foreach (var job in jobs)
            {
                var jobDetail = JobBuilder.Create<QuartzJobWrapper>()
                    // ReSharper disable once AssignNullToNotNullAttribute
                    .WithIdentity(job.GetType().AssemblyQualifiedName)
                    .Build();

                _scheduler.Context.Put("Container", _container);

                if (_scheduler.CheckExists(jobDetail.Key).Result)
                {
                    _scheduler.DeleteJob(jobDetail.Key).ConfigureAwait(true).GetAwaiter().GetResult();
                }

                _scheduler.ScheduleJob(jobDetail, job.Trigger).GetAwaiter().GetResult();
            }

            _scheduler.Start().GetAwaiter().GetResult();
        }

        public void Shutdown()
        {
            _scheduler?.Shutdown();

            if (_serviceTasks != null && _serviceTasks.Any())
            {
                var tasks = _serviceTasks.Select(x => Task.Run(() =>
                {
                    try
                    {
                        x.Item2.ShutdownRequested();
                    }
                    // ReSharper disable once EmptyGeneralCatchClause
                    catch
                    { }

                }, _cancellationTokenSource.Token));

                Task.WaitAll(tasks.ToArray(), 5000);
            }

            _cancellationTokenSource.Cancel();
        }
    }
}