using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Dywham.Fabric.Data.Repositories.EntityFramework.Infrastructure;
using Dywham.Fabric.Utils;

namespace Dywham.Fabric.Data.Repositories.EntityFramework.DependencyInjection
{
    public class AutofacModules : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var assemblies = AssemblyUtils.GetAssemblies();

            builder.RegisterAssemblyTypes(assemblies)
                .Where(t => typeof(IRepository).IsAssignableFrom(t))
                .AsSelf()
                .AsImplementedInterfaces()
                .InstancePerDependency()
                .PropertiesAutowired();

            builder.RegisterGeneric(typeof(EfRepository<,>))
                .As(typeof(IEfRepository<,>))
                .InstancePerDependency()
                .AsSelf()
                .PropertiesAutowired();

            builder.RegisterAssemblyTypes(assemblies)
                .Where(t => typeof(IRepository<>).IsAssignableFrom(t))
                .InstancePerDependency()
                .AsSelf()
                .PropertiesAutowired();

            builder.RegisterAssemblyTypes(assemblies)
                .Where(t => typeof(IEfDatabaseContextUnitOfWork).IsAssignableFrom(t))
                .AsImplementedInterfaces()
                .AsSelf()
                .InstancePerDependency()
                .PropertiesAutowired();

            foreach (var type in assemblies.SelectMany(x => x.GetTypes())
                .Where(t => t.IsAssignableToGenericType(typeof(IEntityGlobalFilter<>)) && t.IsClass && !t.IsAbstract))
            {
                var interfaceType = type.GetInterface(typeof(IEntityGlobalFilter<>).Name);

                if(interfaceType == null) continue;

                // ReSharper disable once PossibleNullReferenceException
                var genericType = typeof(IEntityGlobalFilter<>).MakeGenericType(interfaceType.GenericTypeArguments[0]);

                builder.RegisterType(type)
                    .As(genericType)
                    .AsImplementedInterfaces()
                    .PropertiesAutowired()
                    .InstancePerDependency();
            }

            foreach (var type in assemblies.SelectMany(x => x.GetTypes())
                .Where(t => t.IsAssignableToGenericType(typeof(IEfDatabaseContextFactory<>)) && t.IsClass && !t.IsAbstract))
            {
                var baseTypes = new List<Type> {type};

                if (type.BaseType != null && type.BaseType.IsGenericType)
                {
                    baseTypes.AddRange(type.BaseType.GenericTypeArguments.Select(x => typeof(IEfDatabaseContextFactory<>).MakeGenericType(x.BaseType!)));
                }

                builder.RegisterType(type)
                    .As(baseTypes.ToArray())
                    .AsImplementedInterfaces()
                    .PropertiesAutowired()
                    .InstancePerDependency();

                baseTypes.Clear();
            }
        }
    }
}