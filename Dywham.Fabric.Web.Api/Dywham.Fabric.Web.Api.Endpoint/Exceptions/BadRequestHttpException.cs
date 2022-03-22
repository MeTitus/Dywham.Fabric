namespace Dywham.Fabric.Web.Api.Endpoint.Exceptions
{
    public class BadRequestHttpException : DywhamHttpException
    {
        public BadRequestHttpException()
        { }

        public BadRequestHttpException(string message) : base(message)
        { }
    }
}
