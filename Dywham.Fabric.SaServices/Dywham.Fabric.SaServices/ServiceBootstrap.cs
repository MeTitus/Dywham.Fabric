using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Autofac;
using Dywham.Fabric.SaServices.Workers;
using Dywham.Fabric.Utils;
using log4net.Config;
using Newtonsoft.Json;
using LogManager = log4net.LogManager;

namespace Dywham.Fabric.SaServices
{
    public abstract class ServiceBootstrap : ServiceBootstrap<ServiceSettings>
    { }

    public abstract class ServiceBootstrap<T> : IServiceBootstrap where T : ServiceSettings
    {
        private string _settingsFileName = "settings.json";


        protected ServiceBootstrap()
        {
            LoadEndpointSettings(AppDomain.CurrentDomain.BaseDirectory);
        }

        protected ServiceBootstrap(string settingsFileName)
        {
            _settingsFileName = settingsFileName;

            LoadEndpointSettings(AppDomain.CurrentDomain.BaseDirectory);
        }


        protected log4net.ILog Logger { get; set; }

        protected IContainer Container { get; set; }

        protected T EndpointSettings { get; set; }


        public IContainer Init()
        {
            return Init(new ContainerBuilder());
        }

        public IContainer Init(ContainerBuilder builder)
        {
            try
            {
                DefineLogging();

                Logger.Info("Service initialization starting");

                var assemblies = AssemblyUtils.GetAssemblies();

                OnConfigurationStarting(builder, assemblies);

                RegisterDefaultComponents(builder, assemblies);

                Container = builder.Build();

                OnConfigurationFinished(Container);

                Logger.Info("Endpoint initialization finished");

                OnServiceStarting(Container);

                return Container;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);

                if (ex is not ReflectionTypeLoadException exception) throw;

                foreach (var loaderException in exception.LoaderExceptions)
                {
                    if (loaderException != null) Logger.Error(loaderException.Message, loaderException);
                }

                OnServiceStopping(ex);

                throw;
            }
        }

        protected virtual void DefineLogging()
        {
            using (var stream = new MemoryStream())
            {
                var writer = new StreamWriter(stream);

                writer.Write(EndpointSettings.LoggingConfiguration);
                writer.Flush();

                stream.Position = 0;

                var repo = LogManager.CreateRepository(Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));

                XmlConfigurator.Configure(repo, stream);
            }

            Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);
        }

        protected void RegisterDefaultComponents(ContainerBuilder builder, Assembly[] assemblies)
        {
            builder.RegisterAssemblyModules(assemblies);

            builder.RegisterType<ServiceControl>().As<IServiceControl>().SingleInstance().PropertiesAutowired();

            builder.RegisterAssemblyTypes(assemblies)
                .Where(t => typeof(IDywhamSaWorker).IsAssignableFrom(t) && !t.IsAbstract)
                .AsSelf()
                .AsImplementedInterfaces()
                .InstancePerDependency()
                .PropertiesAutowired();

            builder.RegisterAssemblyTypes(assemblies)
                .Where(t => typeof(IDywhamSaTimedWorker).IsAssignableFrom(t) && !t.IsAbstract)
                .AsSelf()
                .AsImplementedInterfaces()
                .InstancePerDependency()
                .PropertiesAutowired();

            builder.Register(_ => EndpointSettings)
                .As<T>()
                .AsImplementedInterfaces()
                .AsSelf()
                .InstancePerDependency();
        }

        protected void LoadEndpointSettings(string path)
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

        public virtual void OnConfigurationStarting(ContainerBuilder builder, Assembly[] assemblies)
        { }

        public virtual void OnConfigurationFinished(IContainer container)
        { }

        public virtual void OnServiceStarting(IContainer container)
        {
            ((ServiceControl)container.Resolve<IServiceControl>()).Init(container);
        }

        public virtual void OnServiceStopping(Exception ex)
        {
            Exit(1);
        }

        public virtual void OnServiceStopping(object sender, EventArgs e)
        {
            Exit(0);
        }

        private void Exit(int errorCode)
        {
            ((ServiceControl)Container.Resolve<IServiceControl>()).Shutdown();

            // ReSharper disable once PossibleNullReferenceException
            var applicationPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

            // ReSharper disable once AssignNullToNotNullAttribute
            Process.Start(applicationPath);

            Environment.Exit(errorCode);
        }
    }
}