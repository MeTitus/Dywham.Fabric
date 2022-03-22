using System;

namespace Dywham.Fabric.Web.Api.Endpoint.Exceptions
{
    public class DywhamHttpException : Exception
    {
        public DywhamHttpException()
        { }

        public DywhamHttpException(string message) : base(message)
        { }
    }
}