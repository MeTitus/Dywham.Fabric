using Dywham.Fabric.Web.Api.Endpoint.Messaging;

namespace Dywham.Fabric.Web.Api.Extended.Endpoint
{
    public class ExtendedMessagingWebApiEndpointSettings : MessagingWebApiEndpointSettings
    {
        public string EnableCorsOrigins { get; set; }

        public int DefaultApiCount { get; set; }

        public int DefaultApiOffset { get; set; }

        public bool EnableSignalR { get; set; } = true;
    }
}