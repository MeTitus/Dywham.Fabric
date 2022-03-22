using Dywham.Fabric.Microservices.Endpoint;
using Dywham.Fabric.Microservices.Extended.Storage.Data.Repositories.Infrastructure;

namespace Dywham.Fabric.Microservices.Extended.Endpoint
{
    public interface IExtendedEndpointSettings : IEndpointSettings, IExtendedDatabaseSettings
    {
        string BusConnectionString { get; set; }
        
        string BusPersistenceConnectionString { get; set; }

        string EndpointDynamicSettingsUpdaterJob { get; set; }

        string UniquenessGuardCleanupJob { get; set; }
    }
}