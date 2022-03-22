namespace Dywham.Fabric.Microservices.Endpoint
{
    public class EndpointSettings : IEndpointSettings
    {
        public virtual string EndpointName { get; set; } = "EndpointName";

        public bool EnableMonitoring { get; set; }

        public virtual string ErrorQueue { get; set; }

        public virtual string AuditQueue { get; set; }

        public virtual string MetricsQueue { get; set; }

        public virtual string ServiceControlQueue { get; set; }

        public virtual bool EnableMessageExecutionTracking { get; set; } = true;

        public virtual bool EnableMessageExecutionAudit { get; set; } = true;

        public virtual bool EnableDurableMessages { get; set; } = true;

        public virtual bool EnableInstallers { get; set; } = true;

        public string LoggingConfiguration { get; set; }
    }
}