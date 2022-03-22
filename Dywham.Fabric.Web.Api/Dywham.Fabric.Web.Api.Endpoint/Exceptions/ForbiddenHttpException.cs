namespace Dywham.Fabric.Web.Api.Endpoint.Exceptions
{
    public class ForbiddenHttpException : DywhamHttpException
    {
        public ForbiddenHttpException()
        { }

        public ForbiddenHttpException(string message) : base(message)
        { }
    }
}