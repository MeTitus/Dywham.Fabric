using System;

namespace Dywham.Fabric.Providers
{
    public class UniqueIdentifierGenerator: IUniqueIdentifierGenerator
    {
        public Guid Generate()
        {
            return Guid.NewGuid();
        }
    }
}