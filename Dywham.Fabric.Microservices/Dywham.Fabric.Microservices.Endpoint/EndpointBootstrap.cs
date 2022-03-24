using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Dywham.Fabric.Microservices.Endpoint.Adapters.EventAudit;
using Dywham.Fabric.Microservices.Endpoint.Behaviors;
using Dywham.Fabric.Microservices.Endpoint.JobScheduling;
using Dywham.Fabric.Utils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NServiceBus;
using NServiceBus.Configuration.AdvancedExtensibility;
using NServiceBus.Extensions.Logging;
using NServiceBus.Logging;
using Log4NetProvider = Dywham.Fabric.Microservices.Endpoint.Adapters.Logging.Log4NetProvider;

namespace Dywham.Fabric.Microservices.Endpoint
{
    public abstract class EndpointBootstrap : BusEndpointBootstrap<EndpointSettings>
    { }

    public abstract class BusEndpointBootstrap<T> : IEndpointBootstrap where T : EndpointSettings
    {
        private readonly object _lock = new();
        private string _settingsFileName = "endpointSettings.json";
        private IMessageDispatcher _dywhamEndpointInstance;
        private ILifetimeScope _scope;


        protected BusEndpointBootstrap()
        {
            LoadEndpointSettings(AppDomain.CurrentDomain.BaseDirectory);
        }

        protected BusEndpointBootstrap(string settingsFileName)
        {
            _settingsFileName = settingsFileName;

            LoadEndpointSettings(AppDomain.CurrentDomain.BaseDirectory);
        }


        protected log4net.ILog Logger { get; set; }

        protected T EndpointSettings { get; set; }


        public bool Start()
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            Console.CancelKeyPress += OnExit;

            ConfigureLogging();

            Logger.Info("Endpoint initialization starting");

            try
            {
                var configuration = new EndpointConfiguration(EndpointSettings.EndpointName);
                var assemblies = AssemblyUtils.GetAssemblies();

                ConfigureConfiguration(configuration, assemblies);

                ConfigureMessageConventions(configuration);

                if (EndpointSettings.EnableMonitoring)
                {
                    ConfigureMonitoring(configuration);
                }

                if (EndpointSettings.EnableMessageExecutionTracking)
                {
                    configuration.Pipeline.Register(typeof(OutgoingMessageTrackingBehavior), "Outgoing Message Execution Tracking Behavior");
                    configuration.Pipeline.Register(typeof(IncomingMessageTrackingBehavior), "Incoming Message Execution Tracking Behavior");
                }

                var containerSettings = configuration.UseContainer(new AutofacServiceProviderFactory());

                containerSettings.ConfigureContainer(builder =>
                {
                    ConfigureComponentsRegistrations(builder, assemblies);

                    ConfigureComponentsRegistrations(builder, assemblies);

                    builder.RegisterBuildCallback(scope =>
                    {
                        _scope = scope;
                    });
                });

                var startableEndpoint = global::NServiceBus.Endpoint.Create(configuration).GetAwaiter().GetResult();

                Logger.Info("Endpoint initialization finished");

                var dywhamEndpointInstance = (MessageDispatcher) _dywhamEndpointInstance;

                dywhamEndpointInstance.SetStopAction(Stop);
                dywhamEndpointInstance.SetName(EndpointSettings.EndpointName);
                dywhamEndpointInstance.SetEndpoint(startableEndpoint.Start().GetAwaiter().GetResult());

                var extraConfigurations = _scope.ResolveOptional<IList<IEndpointStartupBehavior>>();

                if (extraConfigurations != null)
                {
                    foreach (var extraConfiguration in extraConfigurations)
                    {
                        extraConfiguration
                            .OnEndpointStartingAsync(_dywhamEndpointInstance.EndpointInstance, _scope,
                                CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();
                    }
                }

                OnInitializationCompletedAsync(_dywhamEndpointInstance.EndpointInstance, _scope).ConfigureAwait(false).GetAwaiter().GetResult();

                return true;
            }
            catch (Exception ex)
            {
                Stop(ex);
            }

            return false;
        }

        protected virtual Task OnInitializationCompletedAsync(IEndpointInstance endpointInstance, ILifetimeScope container)
        {
            return Task.CompletedTask;
        }

        protected virtual Task OnEndpointStoppingAsync(IEndpointInstance endpoint)
        {
            return Task.CompletedTask;
        }

        protected virtual void ConfigureConfiguration(EndpointConfiguration configuration, Assembly[] assemblies)
        {
            configuration.GetSettings().Set("NServiceBus.HostInformation.DisplayName", EndpointSettings.EndpointName);

            ConfigureSerialization(configuration);

            if (EndpointSettings.EnableDurableMessages)
            {
                configuration.EnableDurableMessages();
            }

            if (!string.IsNullOrEmpty(EndpointSettings.ErrorQueue))
            {
                configuration.SendFailedMessagesTo(EndpointSettings.ErrorQueue);
            }

            if (!string.IsNullOrEmpty(EndpointSettings.AuditQueue))
            {
                configuration.AuditProcessedMessagesTo(EndpointSettings.AuditQueue);
            }

            if (EndpointSettings.EnableInstallers)
            {
                configuration.EnableInstallers();
            }
        }

        protected virtual void ConfigureComponentsRegistrations(ContainerBuilder builder, Assembly[] assemblies)
        {
            builder.RegisterAssemblyModules(assemblies);
            builder.RegisterAssemblyTypes(assemblies)
                .Where(t => typeof(IEndpointStartupBehavior).IsAssignableFrom(t) && !t.IsAbstract)
                .AsImplementedInterfaces()
                .SingleInstance()
                .PropertiesAutowired();
            builder.RegisterAssemblyTypes(assemblies)
                .Where(t => typeof(IMessageReceivedBehavior).IsAssignableFrom(t) && !t.IsAbstract)
                .AsImplementedInterfaces()
                .SingleInstance()
                .PropertiesAutowired();
            builder.RegisterAssemblyTypes(assemblies)
                .Where(t => typeof(IMessageDispatchedBehavior).IsAssignableFrom(t) && !t.IsAbstract)
                .AsImplementedInterfaces()
                .SingleInstance()
                .PropertiesAutowired();
            builder.Register(_ => EndpointSettings)
                .As<T>()
                .As<EndpointSettings>()
                .As<IEndpointSettings>()
                .AsSelf()
                .AsImplementedInterfaces()
                .SingleInstance();
            builder.RegisterAssemblyTypes(assemblies)
                .AsClosedTypesOf(typeof(IHandleMessages<>))
                .AsImplementedInterfaces()
                .PropertiesAutowired();
            builder.RegisterAssemblyTypes(assemblies)
                .Where(t => typeof(IJobScheduler).IsAssignableFrom(t) && !t.IsAbstract)
                .As<IJobScheduler>()
                .InstancePerDependency()
                .PropertiesAutowired()
                .AsSelf();
            builder.RegisterAssemblyTypes(assemblies)
                .Where(t => typeof(IEventAuditProvider).IsAssignableFrom(t) && !t.IsAbstract)
                .As<IEventAuditProvider>()
                .InstancePerDependency()
                .PropertiesAutowired();

            _dywhamEndpointInstance = new MessageDispatcher();

            builder.RegisterInstance(_dywhamEndpointInstance)
                .As<IMessageDispatcher>()
                .SingleInstance()
                .PropertiesAutowired();
        }

        protected virtual void ConfigureMessageConventions(EndpointConfiguration configuration)
        {}

        protected virtual void ConfigureMonitoring(EndpointConfiguration configuration)
        {
            if(string.IsNullOrWhiteSpace(EndpointSettings.MetricsQueue)) return;

            var metrics = configuration.EnableMetrics();

            metrics.SendMetricDataToServiceControl(EndpointSettings.MetricsQueue, TimeSpan.FromSeconds(10));

            configuration.SendHeartbeatTo(EndpointSettings.ServiceControlQueue, TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(35));
        }

        protected virtual void ConfigureSerialization(EndpointConfiguration configuration)
        { }

        private void ConfigureLogging()
        {
            LogManager.UseFactory(new ExtensionsLoggerFactory(new LoggerFactory(new List<ILoggerProvider>
            {
                new Log4NetProvider(EndpointSettings.LoggingConfiguration)
            })));

            Logger = log4net.LogManager.GetLogger(GetType());
        }

        private void LoadEndpointSettings(string path)
        {
            var settingsType = typeof(T);

            try
            {
                _settingsFileName = Path.Combine(path, _settingsFileName);

                if (!File.Exists(_settingsFileName))
                {
                    throw new InvalidOperationException($"Settings file cannot be found at: {_settingsFileName}");
                }

                EndpointSettings = (T)JsonConvert.DeserializeObject(File.ReadAllText(_settingsFileName), settingsType);
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException($"Could not register custom configuration of type {settingsType.FullName}, exception -> {exception.Message}");
            }
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs arg)
        {
            var ex = arg.ExceptionObject as Exception;

            if (ex != null)
            {
                Logger.Error(ex);
            }

            if (!arg.IsTerminating) return;

            Stop(ex);
        }

        private void OnExit(object sender, ConsoleCancelEventArgs e)
        {
            Stop();
        }

        public void Stop()
        {
            lock (_lock)
            {
                if (_dywhamEndpointInstance == null || _dywhamEndpointInstance.EndpointInstance == null) return;

                OnEndpointStoppingAsync(_dywhamEndpointInstance.EndpointInstance).ConfigureAwait(false).GetAwaiter().GetResult();

                Task.Run(() =>
                {
                    try
                    {
                        if (_dywhamEndpointInstance == null || _dywhamEndpointInstance.EndpointInstance == null) return;

                        _dywhamEndpointInstance.EndpointInstance.Stop().ConfigureAwait(false).GetAwaiter().GetResult();

                        _dywhamEndpointInstance = null;
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }

                    Environment.Exit(0);
                });
            }
        }

        private void Stop(Exception ex)
        {
            Logger.Error(ex);

            if (!(ex is ReflectionTypeLoadException exception)) throw ex;

            foreach (var loaderException in exception.LoaderExceptions)
            {
                Logger.Error(loaderException);
            }

            OnEndpointStoppingAsync(_dywhamEndpointInstance.EndpointInstance).ConfigureAwait(false).GetAwaiter().GetResult();

            Environment.Exit(1);
        }
    }
}