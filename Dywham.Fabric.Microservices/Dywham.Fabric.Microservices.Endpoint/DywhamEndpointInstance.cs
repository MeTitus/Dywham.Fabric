using System;
using NServiceBus;

namespace Dywham.Fabric.Microservices.Endpoint
{
    public class DywhamEndpointInstance : IDywhamEndpointInstance
    {
        private Action _stop;


        public IEndpointInstance EndpointInstance { get; private set; }

        public string Name { get; private set; }


        public void Stop()
        {
            _stop.Invoke();
        }
        
        public void SetEndpoint(IEndpointInstance instance)
        {
            EndpointInstance = instance;
        }

        public void SetStopAction(Action stop)
        {
            _stop = stop;
        }

        public void SetName(string name)
        {
            Name = name;
        }
    }
}