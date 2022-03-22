using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Autofac;
using Dywham.Fabric.Web.Api.Endpoint.Behaviors;
using Dywham.Fabric.Web.Api.Endpoint.Messaging.Behaviors;
using Dywham.Fabric.Web.Api.Endpoint.Messaging.Providers.Messaging;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;

namespace Dywham.Fabric.Web.Api.Endpoint.Messaging
{
    public abstract class MessagingWebApiEndpointBootstrap : MessagingWebApiEndpointBootstrap<MessagingWebApiEndpointSettings>
    { }

    public abstract class MessagingWebApiEndpointBootstrap<T> : WebApiEndpointBootstrap<T> where T : MessagingWebApiEndpointSettings
    {
        private EndpointConfiguration _endpointConfiguration;


        protected IEndpointInstance EndpointInstance { get; private set; }

        protected virtual string NServiceBusEndpointName { get; private set; }


        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            if (string.IsNullOrEmpty(NServiceBusEndpointName))
            {
                // ReSharper disable once PossibleNullReferenceException
                NServiceBusEndpointName = Assembly.GetEntryAssembly().GetName().Name;
            }

            _endpointConfiguration = new EndpointConfiguration(NServiceBusEndpointName);

            ConfigureSerialization(_endpointConfiguration);

            ConfigureMessageConventions(_endpointConfiguration);

            ConfigureBus(_endpointConfiguration);
        }

        public virtual void ConfigureBus(EndpointConfiguration configuration)
        { }

        public override void ConfigureComponentsRegistrations(ContainerBuilder builder)
        {
            base.ConfigureComponentsRegistrations(builder);

            EndpointInstance = NServiceBus.Endpoint.Start(_endpointConfiguration).GetAwaiter().GetResult();

            builder.RegisterAssemblyTypes(RuntimeAssemblies)
                .Where(t => typeof(IRunOnEndpointStarting).IsAssignableFrom(t) && !t.IsAbstract)
                .AsImplementedInterfaces()
                .SingleInstance()
                .PropertiesAutowired();
            builder.Register(_ => EndpointInstance).As<IEndpointInstance>().SingleInstance().PropertiesAutowired();
            builder.RegisterType<BusDispatcher>().As<IBusDispatcher>().SingleInstance().PropertiesAutowired();

            var extraConfigurations = ApplicationContainer.ResolveOptional<IList<IExtendedRunOnEndpointStarting>>();

            if (extraConfigurations == null) return;

            foreach (var extraConfiguration in extraConfigurations)
            {
                extraConfiguration.OnEndpointStartingAsync(EndpointInstance, ApplicationContainer,
                    CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();
            }
        }

        protected virtual void ConfigureMessageConventions(EndpointConfiguration endpointConfiguration)
        {
            static bool CheckTypeMatches(Type type, string endsWith) => type?.Namespace != null && type.Name.EndsWith(endsWith);

            endpointConfiguration.Conventions()
                .DefiningCommandsAs(type => CheckTypeMatches(type, "Command"))
                .DefiningEventsAs(type => CheckTypeMatches(type, "Event"));
        }

        protected virtual void ConfigureSerialization(EndpointConfiguration configuration)
        { }
    }
}