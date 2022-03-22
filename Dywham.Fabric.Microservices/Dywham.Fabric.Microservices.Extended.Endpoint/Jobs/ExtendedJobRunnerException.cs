using System;

namespace Dywham.Fabric.Microservices.Extended.Endpoint.Jobs
{
    [Serializable]
    public class ExtendedJobRunnerException : Exception
    {
        public ExtendedJobRunnerException(string message) : base(message)
        { }
    }
}