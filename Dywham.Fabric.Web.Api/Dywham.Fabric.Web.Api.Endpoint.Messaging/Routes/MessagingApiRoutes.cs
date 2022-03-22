using Dywham.Fabric.Web.Api.Endpoint.Messaging.Providers.Messaging;
using Dywham.Fabric.Web.Api.Endpoint.Routes;

namespace Dywham.Fabric.Web.Api.Endpoint.Messaging.Routes
{
    public class MessagingApiRoutes : ApiRoutes
    {
        public IBusDispatcher BusDispatcher { get; set; }
    }
}