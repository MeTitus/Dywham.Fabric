using System;
using System.IO;
using System.Linq;
using System.Text;
using Autofac;
using Dywham.Fabric.Data.Repositories;
using Dywham.Fabric.Microservices.Extended.Contracts;
using Dywham.Fabric.Providers.IO;
using Dywham.Fabric.Providers.Serialization.Json;
using Dywham.Fabric.Providers.Web.Comms.SignalR.Server;
using Dywham.Fabric.Web.Api.Endpoint.Messaging;
using Dywham.Fabric.Web.Api.Endpoint.Messaging.Providers.Messaging;
using Dywham.Fabric.Web.Api.Extended.Contracts.Events;
using Dywham.Fabric.Web.Api.Extended.Endpoint.Filters;
using Dywham.Fabric.Web.Api.Extended.Endpoint.Providers.ClientNotifications;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NServiceBus;

namespace Dywham.Fabric.Web.Api.Extended.Endpoint
{
    public class ExtendedMessagingWebApiEndpointBootstrap<T> : MessagingWebApiEndpointBootstrap<T> where T : ExtendedMessagingWebApiEndpointSettings
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>

                    builder.WithOrigins(Settings.EnableCorsOrigins.Split(","))
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
                        .WithExposedHeaders("x-resourcecollectionempty"));
            });

            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(ApiCommandCollisionAvoidanceFilterAttribute), 2);
            });

            services.AddSignalR(hubOptions => { hubOptions.EnableDetailedErrors = true; });
        }

        public override void ConfigureComponentsRegistrations(ContainerBuilder builder)
        {
            base.ConfigureComponentsRegistrations(builder);

            builder.RegisterAssemblyTypes(RuntimeAssemblies)
                .Where(t => typeof(IRepository).IsAssignableFrom(t))
                .AsImplementedInterfaces()
                .AsSelf()
                .InstancePerLifetimeScope()
                .PropertiesAutowired();

            builder.RegisterAssemblyTypes(RuntimeAssemblies)
                .Where(t => typeof(IRepository<>).IsAssignableFrom(t))
                .AsImplementedInterfaces()
                .AsSelf()
                .InstancePerLifetimeScope()
                .PropertiesAutowired();

            if (Settings.EnableSignalR)
            {
                builder.RegisterType<ClientNotificationsProvider>()
                    .AsImplementedInterfaces()
                    .PropertiesAutowired()
                    .SingleInstance()
                    .OnActivated(x =>
                    {
                        x.Instance.RegisterEventTypes(RuntimeAssemblies.SelectMany(y => y.GetTypes())
                            .Where(y => typeof(ExtendedEventModel).IsAssignableFrom(y) && !y.IsAbstract)
                            .ToList());
                    });
            }

            builder.RegisterType<ActionContextAccessor>().As<IActionContextAccessor>().SingleInstance();
            builder.RegisterType<ParametersFilterAttribute>().PropertiesAutowired().SingleInstance();
            builder.RegisterModule(new SignalRAutofacModule());
            builder.RegisterType<IOProvider>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<JsonProvider>().As<IJsonProvider>().PropertiesAutowired().SingleInstance();
            builder.RegisterType<BusDispatcher>().As<IBusDispatcher>().PropertiesAutowired().InstancePerDependency()
                .OnActivated(x =>
                {
                    var httpContextAccessor = x.Context.Resolve<IActionContextAccessor>();

                    x.Instance.OnBeforeSending = message =>
                    {
                        if (message is IRequiresUserIdentification requiresUserIdentification)
                        {
                            // ReSharper disable once AssignNullToNotNullAttribute
                            requiresUserIdentification.UserId = Guid.Parse(httpContextAccessor.ActionContext!.RouteData.Values["UserId"]?.ToString());
                        }
                    };
                });
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            base.Configure(app, env);

            app.UseHsts();
            app.UseDeveloperExceptionPage();
            app.UseCors("CorsPolicy");
            app.UseStaticFiles();
            app.UseRouting();
            app.UseEndpoints(c => { c.MapHub<NotificationsHub>("/signalr/notifications"); });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseStaticFiles();
            app.UseRouting();
        }

        protected override void ConfigureSerialization(EndpointConfiguration configuration)
        {
            var serialization = configuration.UseSerialization<NewtonsoftSerializer>();

            serialization.WriterCreator(stream =>
            {
                var streamWriter = new StreamWriter(stream, new UTF8Encoding(false));

                return new JsonTextWriter(streamWriter)
                {
                    Formatting = Formatting.Indented
                };
            });
        }
    }
}