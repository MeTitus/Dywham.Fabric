namespace Dywham.Fabric.Web.Api.Endpoint
{
    public class WebApiEndpointSettings
    {
        public string EndpointName { get; set; }

        public string EndpointVersion { get; set; }

        public string EndpointDescription { get; set; }

        public bool EnableSwagger { get; set; } = true;

        public bool EnableCompression { get; set; } = true;

        public string SwaggerDocumentationFile { get; set; }

        public string LoggingConfiguration { get; set; }
    }
}