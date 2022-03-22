using System;

namespace Dywham.Fabric.SaServices.Workers
{
    public abstract class DywhamSaWorker : IDywhamSaWorker
    {
        public IServiceControl ServiceControl { get; set; }

        public string Name { get; set; } = Guid.NewGuid().ToString();

        public abstract void DoWork();


        public virtual void OnException(Exception ex)
        { }

        public virtual void ShutdownRequested()
        { }
    }
}