namespace Dywham.Fabric.Microservices.Endpoint
{
    public interface IEndpointSettings
    {
        string EndpointName { get; set; } 

        bool EnableMonitoring { get; set; }

        string ErrorQueue { get; set; }

        string AuditQueue { get; set; }

        string MetricsQueue { get; set; }

        string ServiceControlQueue { get; set; }

        bool EnableMessageExecutionTracking { get; set; }

        bool EnableMessageExecutionAudit { get; set; }

        bool EnableDurableMessages { get; set; }

        bool EnableInstallers { get; set; }

        string LoggingConfiguration { get; set; }
    }
}