using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Autofac;
using Dywham.Fabric.Data.Repositories.EntityFramework;
using Dywham.Fabric.Microservices.Endpoint;
using Dywham.Fabric.Microservices.Endpoint.JobScheduling;
using Dywham.Fabric.Microservices.Endpoint.Providers.Audit;
using Dywham.Fabric.Microservices.Extended.Contracts.Messages.Commands;
using Dywham.Fabric.Microservices.Extended.Endpoint.Jobs;
using Dywham.Fabric.Microservices.Extended.Endpoint.Providers;
using Dywham.Fabric.Microservices.Extended.Storage.Data.Repositories;
using Dywham.Fabric.Microservices.Extended.Storage.Data.Repositories.Entities;
using Dywham.Fabric.Microservices.Extended.Storage.Data.Repositories.Infrastructure;
using Dywham.Fabric.Providers;
using Dywham.Fabric.Providers.IO;
using Dywham.Fabric.Providers.Serialization.Json;
using Dywham.Fabric.Utils;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using NServiceBus;

namespace Dywham.Fabric.Microservices.Extended.Endpoint
{
    public abstract class ExtendedEndpointBootstrap<T, TZ, TY> : BusEndpointBootstrap<T>
        where T : ExtendedEndpointSettings
        where TZ : ExtendedEndpointDynamicSettings, new()
        where TY : ExtendedDatabaseContext
    {
        protected ExtendedEndpointBootstrap()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }


        protected override void ConfigureConfiguration(EndpointConfiguration configuration, Assembly[] assemblies)
        {
            base.ConfigureConfiguration(configuration, assemblies);

            configuration.Recoverability().Immediate(x => x.NumberOfRetries(2));
            configuration.Recoverability().Delayed(x => x.NumberOfRetries(1));
            configuration.Recoverability().AddUnrecoverableException(typeof(ExtendedJobRunnerException));
            configuration.LimitMessageProcessingConcurrencyTo(100);

            ConfigureTransport(configuration, assemblies);

            ConfigureFeatures(configuration);
        }

        protected override void ConfigureSerialization(EndpointConfiguration configuration)
        {
            var serialization = configuration.UseSerialization<NewtonsoftSerializer>();

            serialization.WriterCreator(stream =>
            {
                var streamWriter = new StreamWriter(stream, new UTF8Encoding(false));

                return new JsonTextWriter(streamWriter)
                {
                    Formatting = Formatting.None
                };
            });
        }

        protected virtual void ConfigureFeatures(EndpointConfiguration configuration)
        {
            var outbox = configuration.EnableOutbox();

            outbox.KeepDeduplicationDataFor(TimeSpan.FromHours(1));
            outbox.RunDeduplicationDataCleanupEvery(TimeSpan.FromDays(1));
        }

        protected virtual void ConfigureTransport(EndpointConfiguration configuration, Assembly[] assemblies)
        {
            var transport = configuration.UseTransport<RabbitMQTransport>();

            transport.UseConventionalRoutingTopology();
            transport.ConnectionString(EndpointSettings.BusConnectionString);

            ConfigurePersistence(configuration, transport, assemblies);
        }

        protected virtual void ConfigurePersistence(EndpointConfiguration configuration, TransportExtensions<RabbitMQTransport> transport, Assembly[] assemblies)
        {
            var routing = transport.Routing();

            routing.RouteToEndpoint(typeof(RegisterJobError), EndpointSettings.EndpointName);

            var persistence = configuration.UsePersistence<SqlPersistence>();

            persistence.SqlDialect<SqlDialect.MsSqlServer>();
            persistence.ConnectionBuilder(() => new SqlConnection(EndpointSettings.BusPersistenceConnectionString));

            configuration.UsePersistence<SqlPersistence>();

            var subscriptions = persistence.SubscriptionSettings();

            subscriptions.CacheFor(TimeSpan.FromMinutes(30));
        }

        protected override void ConfigureComponentsRegistrations(ContainerBuilder builder, Assembly[] assemblies)
        {
            base.ConfigureComponentsRegistrations(builder, assemblies);

            builder.RegisterType<IOProvider>().As<IIOProvider>().SingleInstance();
            builder.RegisterType<JsonProvider>().As<IJsonProvider>().SingleInstance().PropertiesAutowired();
            builder.RegisterType<StorageGatewayProvider>().As<IStorageGatewayProvider>().SingleInstance().PropertiesAutowired();
            builder.RegisterType<DateTimeProvider>().As<IDateTimeProvider>().SingleInstance();

            var registerType = builder.RegisterType<TZ>().As<TZ>();

            foreach (var t in typeof(TZ).GetAllBaseTypes().Where(x => !x.IsAbstract && !x.IsInterface))
            {
                registerType = registerType.As(t);
            }

            registerType.SingleInstance()
                .AsImplementedInterfaces()
                .OnActivating(x =>
                {
                    var jsonProvider = x.Context.Resolve<IJsonProvider>();
                    var repository = x.Context.Resolve<IEfRepository<OperationalEntity, TY>>();
                    var operational = repository.FirstOrDefaultAsync().ConfigureAwait(false).GetAwaiter().GetResult();

                    if (operational == null || string.IsNullOrEmpty(operational.Settings)) return;

                    x.ReplaceInstance(jsonProvider.Deserialize<TZ>(operational.Settings));
                });

            if (!string.IsNullOrWhiteSpace(EndpointSettings.UniquenessGuardCleanupJob))
            {
                builder.RegisterType<UniquenessGuardCleanupJob<TY>>()
                    .AsSelf()
                    .As<IDywhamJob>()
                    .As<ExtendedJobRunner<TY>>()
                    .InstancePerDependency()
                    .PropertiesAutowired();
            }

            if (!string.IsNullOrWhiteSpace(EndpointSettings.EndpointDynamicSettingsUpdaterJob))
            {
                builder.RegisterType<ExtendedEndpointDynamicSettingsUpdaterJob<TY, TZ>>()
                    .AsSelf()
                    .As<IDywhamJob>()
                    .As<ExtendedJobRunner<TY>>()
                    .InstancePerDependency()
                    .PropertiesAutowired();
            }

            builder.RegisterType<DataContextProvider<TY>>()
                .As<IDataContextProvider<TY>>()
                .InstancePerDependency()
                .AsImplementedInterfaces()
                .PropertiesAutowired();

            builder.RegisterGeneric(typeof(ExtendedRepository<,>))
                .As(typeof(IExtendedRepository<,>))
                .PropertiesAutowired()
                .InstancePerDependency();

            builder.RegisterType<ExtendedUnitOfWork<TY>>()
                .AsImplementedInterfaces()
                .PropertiesAutowired()
                .InstancePerDependency();

            builder.RegisterGeneric(typeof(EfRepository<,>))
                .As(typeof(IEfRepository<,>))
                .PropertiesAutowired()
                .InstancePerDependency();

            builder.RegisterType<EventStoreProvider<TY>>().As<IEventAuditProvider>()
                .InstancePerDependency()
                .PropertiesAutowired();
        }
    }

    public abstract class ExtendedEndpointBootstrap<T, TY> : ExtendedEndpointBootstrap<T, ExtendedEndpointDynamicSettings, TY>
        where T : ExtendedEndpointSettings
        where TY : ExtendedDatabaseContext
    { }
}