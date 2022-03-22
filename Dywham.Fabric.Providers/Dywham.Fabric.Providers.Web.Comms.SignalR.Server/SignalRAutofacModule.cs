using Autofac;

namespace Dywham.Fabric.Providers.Web.Comms.SignalR.Server
{
    public class SignalRAutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ConnectionMappings>().As<IConnectionMappings>()
                .PropertiesAutowired().SingleInstance();
            builder.RegisterType<SignalRServerProvider>().AsImplementedInterfaces()
                .PropertiesAutowired().SingleInstance();
        }
    }
}