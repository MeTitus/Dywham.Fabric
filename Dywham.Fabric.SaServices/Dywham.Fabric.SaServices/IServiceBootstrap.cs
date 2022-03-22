using System;
using Autofac;

namespace Dywham.Fabric.SaServices
{
    public interface IServiceBootstrap
    {
        IContainer Init();

        IContainer Init(ContainerBuilder builder);

        void OnServiceStopping(Exception ex);

        void OnServiceStopping(object sender, EventArgs e);
    }
}