using Autofac;
using Dywham.Fabric.Utils;

namespace Dywham.Fabric.Data.Repositories.DependencyInjection
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
        }
    }
}