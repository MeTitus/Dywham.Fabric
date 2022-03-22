using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Threading;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Dywham.Fabric.Utils;
using Dywham.Fabric.Web.Api.Endpoint.Behaviors;
using Dywham.Fabric.Web.Api.Endpoint.Filters;
using Dywham.Fabric.Web.Api.Endpoint.Providers.Security;
using Dywham.Fabric.Web.Api.Endpoint.Routes;
using log4net;
using log4net.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Dywham.Fabric.Web.Api.Endpoint
{
    public abstract class WebApiEndpointBootstrap : WebApiEndpointBootstrap<WebApiEndpointSettings>
    {
        // ReSharper disable once UnusedMember.Global
        protected readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);
    }

    public abstract class WebApiEndpointBootstrap<T> where T : WebApiEndpointSettings
    {
        protected Assembly[] RuntimeAssemblies { get; set; }

        protected T Settings { get; set; }

        protected virtual string SettingsFile { get; set; } = "appsettings.json";

        protected string ExecutionPath { get; set; }

        protected ILifetimeScope ApplicationContainer { get; set; }

        protected virtual bool EnableSecurityVerification { get; set; } = false;


        public virtual void ConfigureServices(IServiceCollection services)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            var uri = new UriBuilder(Assembly.GetExecutingAssembly().Location);

            ExecutionPath = Uri.UnescapeDataString(uri.Path);
            ExecutionPath = Path.GetDirectoryName(ExecutionPath);

            LoadEndpointSettings(ExecutionPath);

            if (Settings.EnableCompression)
            {
                services.AddResponseCompression(options =>
                {
                    options.EnableForHttps = true;
                    options.Providers.Add<BrotliCompressionProvider>();
                });

                services.Configure<BrotliCompressionProviderOptions>(options =>
                {
                    options.Level = CompressionLevel.Fastest;
                });
            }

            RuntimeAssemblies = AssemblyUtils.GetAssemblies(ExecutionPath);

            ConfigureLogging();

            if (Settings.EnableSwagger)
            {
                services.AddSwaggerGen(config =>
                {
                    config.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                    config.CustomSchemaIds(x => x.FullName);
                    config.SwaggerDoc(Settings.EndpointVersion, new OpenApiInfo
                    {
                        Title = Settings.EndpointDescription,
                        Version = Settings.EndpointVersion
                    });

                    if (string.IsNullOrWhiteSpace(Settings.SwaggerDocumentationFile)) return;

                    var xmlDocFile = Path.Combine(Path.Combine(ExecutionPath, Settings.SwaggerDocumentationFile));

                    config.IncludeXmlComments(xmlDocFile);
                });

                services.AddSwaggerGenNewtonsoftSupport();
            }

            services.AddControllers()
                .AddControllersAsServices()
                .AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy
                    {
                        ProcessDictionaryKeys = false,
                        OverrideSpecifiedNames = true
                    }
                }).AddJsonOptions(x =>
                {
                    x.JsonSerializerOptions.WriteIndented = true;
                    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            services.AddControllers(config =>
            {
                config.Filters.Add(new ApiEndpointBaseActionFilterAttribute());

                if (EnableSecurityVerification)
                {
                    config.Filters.Add(new SecurityProviderActionFilterAttribute {Order = 0});
                }
            });
        }

        public virtual void ConfigureComponentsRegistrations(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(RuntimeAssemblies)
                .Where(t => typeof(IRunOnEndpointStarting).IsAssignableFrom(t) && !t.IsAbstract)
                .AsImplementedInterfaces()
                .SingleInstance()
                .PropertiesAutowired();
            builder.RegisterAssemblyModules(RuntimeAssemblies);
            builder.RegisterAssemblyTypes(RuntimeAssemblies)
                .Where(t => typeof(ApiRoutes).IsAssignableFrom(t))
                .InstancePerLifetimeScope()
                .PropertiesAutowired();
            builder.RegisterAssemblyTypes(RuntimeAssemblies)
                .Where(t => typeof(ISecurityProvider).IsAssignableFrom(t))
                .As<ISecurityProvider>()
                .InstancePerLifetimeScope()
                .PropertiesAutowired();

            var settingTypes = Settings.GetType().GetAllBaseTypes().Where(x => x.IsClass).ToArray();

            builder.RegisterInstance(Settings).AsImplementedInterfaces().AsSelf().SingleInstance().As(settingTypes);

            var extraConfigurations = ApplicationContainer.ResolveOptional<IList<IRunOnEndpointStarting>>();

            if (extraConfigurations == null) return;

            foreach (var extraConfiguration in extraConfigurations)
            {
                extraConfiguration.OnEndpointStartingAsync(ApplicationContainer,
                    CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();
            }
        }

        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (Settings.EnableCompression)
            {
                app.UseResponseCompression();
            }

            if (Settings.EnableSwagger)
            {
                ConfigureSwagger(app);
            }

            ApplicationContainer = app.ApplicationServices.GetAutofacRoot();
        }

        public virtual void ConfigureSwagger(IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/{Settings.EndpointVersion}/swagger.json", $"{Settings.EndpointName}");
                c.RoutePrefix = string.Empty;
            });
        }

        protected virtual void ConfigureLogging()
        {
            using (var stream = new MemoryStream())
            {
                var writer = new StreamWriter(stream);

                writer.Write(Settings.LoggingConfiguration);
                writer.Flush();

                stream.Position = 0;

                var repo = LogManager.CreateRepository(Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));

                XmlConfigurator.Configure(repo, stream);
            }
        }

        private void LoadEndpointSettings(string path)
        {
            var settingsType = typeof(T);

            try
            {
                var configFilePath = Path.Combine(path, SettingsFile);

                if (!File.Exists(configFilePath))
                {
                    throw new InvalidOperationException($"Settings file cannot be found at: {configFilePath}");
                }

                Settings = (T)JsonConvert.DeserializeObject(File.ReadAllText(configFilePath), settingsType);
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException($"Could not register custom configuration of type {settingsType.FullName}, exception -> {exception.Message}");
            }
        }
    }
}