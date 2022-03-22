using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using Dywham.Fabric.Utils;

namespace Dywham.Fabric.SaServices
{
    public static class HostedService
    {
        private static IServiceBootstrap _serviceBootstrap;
        private static Thread _mainThread;
        private static readonly AutoResetEvent Lock = new AutoResetEvent(false);


        public static void Start()
        {
            _mainThread = new Thread(() =>
            {
                try
                {
                    _serviceBootstrap = AssemblyUtils.CreateInstancesOf<IServiceBootstrap>().FirstOrDefault();

                    if (_serviceBootstrap == null)
                    {
                        Console.Error.WriteLine("No service bootstrap loader found");

                        Environment.Exit(0);
                    }

                    _serviceBootstrap.Init();
                }
                catch (Exception ex)
                {
                    _serviceBootstrap?.OnServiceStopping(ex);

                    Console.Error.WriteLine(ex.Message, ex);

                    if (!(ex is ReflectionTypeLoadException exception)) throw;

                    foreach (var loaderException in exception.LoaderExceptions)
                    {
                        Console.Error.WriteLine(loaderException.Message, loaderException);
                    }

                    Console.WriteLine(ex.Message);

                    Environment.Exit(1);
                }

                Lock.WaitOne();

            }) {IsBackground = false};

            _mainThread.Start();

            AppDomain.CurrentDomain.ProcessExit += ProcessClosing;
            AppDomain.CurrentDomain.UnhandledException += OnCurrentDomainUnhandledException;
            Console.CancelKeyPress += OnExit;
        }

        private static void OnCurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Start();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                Console.Error.WriteLine("An attempt has been made to restart the service, but it has failed. Service will now stop.");
            }
        }

        private static void OnExit(object sender, ConsoleCancelEventArgs e)
        {
            ProcessClosing(sender, e);

            Lock.Set();
        }

        private static void ProcessClosing(object sender, EventArgs e)
        {
            lock (typeof(HostedService))
            {
                _serviceBootstrap?.OnServiceStopping(sender, e);

                _serviceBootstrap = null;
            }

            Lock.Set();
        }
    }
}