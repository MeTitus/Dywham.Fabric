namespace Dywham.Fabric.Web.Api.Endpoint.Exceptions
{
    public class UnauthorizedHttpException : DywhamHttpException
    {
        public UnauthorizedHttpException()
        { }

        public UnauthorizedHttpException(string message) : base(message)
        { }
    }
}