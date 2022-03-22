using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using Dywham.Fabric.Utils;

namespace Dywham.Fabric.Microservices.Endpoint
{
    public static class HostedService
    {
        private static IEndpointBootstrap _busEndpointBootstrap;
        private static readonly AutoResetEvent AutoResetEvent = new(false);


        public static void Start()
        {
            try
            {
                _busEndpointBootstrap = AssemblyUtils.CreateInstancesOf<IEndpointBootstrap>().FirstOrDefault();

                if (_busEndpointBootstrap == null)
                {
                    Console.Error.WriteLine("No BusEndpointBootstrap loader found");

                    Environment.Exit(0);
                }

                if (_busEndpointBootstrap.Start())
                {
                    AutoResetEvent.WaitOne();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message, ex);

                if (!(ex is ReflectionTypeLoadException exception)) throw;

                foreach (var loaderException in exception.LoaderExceptions)
                {
                    if (loaderException != null) Console.Error.WriteLine(loaderException.Message, loaderException);
                }

                Console.WriteLine(ex.Message);
            }
        }

        public static void Stop()
        {
            _busEndpointBootstrap.Stop();

            AutoResetEvent.Set();
        }
    }
}