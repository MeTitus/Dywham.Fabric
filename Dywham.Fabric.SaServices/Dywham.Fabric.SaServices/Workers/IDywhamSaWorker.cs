using System;

namespace Dywham.Fabric.SaServices.Workers
{
    public interface IDywhamSaWorker
    {
        // ReSharper disable once UnusedMember.Global
        IServiceControl ServiceControl { get; set; }

        string Name { get; set; }


        void DoWork();

        void OnException(Exception ex);

        void ShutdownRequested();
    }
}