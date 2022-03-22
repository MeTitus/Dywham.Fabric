using System;

namespace Dywham.Fabric.Microservices.Extended.Contracts
{
    public interface IRequiresUserIdentification
    {
        public Guid UserId { get; set; }
    }
}