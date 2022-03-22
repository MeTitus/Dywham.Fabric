using System;

namespace Dywham.Fabric.Providers
{
    public interface IUniqueIdentifierGenerator
    {
        Guid Generate();
    }
}
