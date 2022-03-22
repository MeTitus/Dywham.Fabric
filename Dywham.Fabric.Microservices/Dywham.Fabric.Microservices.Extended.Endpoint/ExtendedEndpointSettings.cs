using Dywham.Fabric.Microservices.Endpoint;

namespace Dywham.Fabric.Microservices.Extended.Endpoint
{
    public abstract class ExtendedEndpointSettings : EndpointSettings, IExtendedEndpointSettings
    {
        public string BusConnectionString { get; set; }

        public string BusPersistenceConnectionString { get; set; }

        public string UniquenessGuardCleanupJob { get; set; }

        public string EndpointDynamicSettingsUpdaterJob { get; set; }

        public string RepositoriesDataSource { get; set; }
    }
}