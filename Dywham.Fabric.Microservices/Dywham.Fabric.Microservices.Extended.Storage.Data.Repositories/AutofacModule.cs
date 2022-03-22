using Autofac;

namespace Dywham.Fabric.Microservices.Extended.Storage.Data.Repositories
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(ExtendedRepository<,>))
                .As(typeof(IExtendedRepository<,>))
                .AsSelf()
                .InstancePerDependency();
        }
    }
}