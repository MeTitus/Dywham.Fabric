namespace Dywham.Fabric.Web.Api.Endpoint.Exceptions
{
    public class NotFoundHttpException : DywhamHttpException
    {
        public NotFoundHttpException()
        { }

        public NotFoundHttpException(string message) : base(message)
        { }
    }
}